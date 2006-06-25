//
// SCSharp.UI.Painter
//
// Authors:
//	Chris Toshok (toshok@hungry.com)
//
// (C) 2006 The Hungry Programmers (http://www.hungry.com/)
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using SdlDotNet;
using SdlDotNet.Sprites;
using System.Drawing;

namespace SCSharp.UI
{

	public enum Layer
	{
		Background,
		Map,
		Shadow,
		Selection,
		Unit,
		Health,
		UI,
		Hud,
		Popup,
		DialogDimScreenHack,
		DialogBackground,
		DialogUI,
		Tooltip,
		Cursor,

		Count
	}

	public delegate void PainterDelegate (DateTime now);

	public class Painter 
	{
		List<PainterDelegate>[] layers;
		int millis;
		int total_elapsed;

		DateTime now; /* the time of the last animation tick */

		Surface paintingSurface;

		bool fullscreen;

		public const int SCREEN_RES_X = 640;
		public const int SCREEN_RES_Y = 480;

#if !RELEASE
		DateTime last_time;
		byte[] fontpal;
		double fps;
		int frame_count;
		Surface fps_surface;
		static bool debug_dirty;
		static bool show_fps;
#endif

		static Painter instance;

#if !RELEASE
		static Painter ()
		{
			string v;
			v = ConfigurationManager.AppSettings ["ShowFps"];
			if (v != null)
				show_fps = Boolean.Parse (v);

			v = ConfigurationManager.AppSettings ["DebugPainterDirty"];
			if (v != null)
				debug_dirty = Boolean.Parse (v);
		}
#endif

		public static void InitializePainter (bool fullscreen, int millis)
		{
			if (instance != null)
				throw new Exception ("only one painter instance allowed");

			instance = new Painter (fullscreen, millis);
		}

		public static Painter Instance {
			get { return instance; }
		}

		private Painter (bool fullscreen, int millis)
		{
#if !RELEASE
			if (show_fps) {
				Pcx pcx = new Pcx ();
				pcx.ReadFromStream ((Stream)Game.Instance.PlayingMpq.GetResource ("game\\tfontgam.pcx"), 0, 0);
				fontpal = pcx.Palette;
			}
#endif

			this.millis = millis;

			Fullscreen = fullscreen;
			
			/* init our list of painter delegates */
			layers = new List<PainterDelegate>[(int)Layer.Count];
			for (Layer i = Layer.Background; i < Layer.Count; i ++)
				layers[(int)i] = new List<PainterDelegate>();

			/* and set ourselves up to invalidate at a regular interval*/
                        Events.Tick += Tick;
		}

		public bool Fullscreen {
			get { return fullscreen; }
			set {
				if (fullscreen != value || paintingSurface == null) {
					fullscreen = value;

					if (fullscreen)
						paintingSurface = Video.SetVideoMode (SCREEN_RES_X, SCREEN_RES_Y);
					else
						paintingSurface = Video.SetVideoModeWindow (SCREEN_RES_X, SCREEN_RES_Y);

					Invalidate ();
				}
			}
		}

		public void Prepend (Layer layer, PainterDelegate painter)
		{
			layers[(int)layer].Insert (0, painter);
		}

		public void Add (Layer layer, PainterDelegate painter)
		{
			layers[(int)layer].Add (painter);
		}

		public void Remove (Layer layer, PainterDelegate painter)
		{
			layers[(int)layer].Remove (painter);
		}

		public void Clear (Layer layer)
		{
			layers[(int)layer].Clear ();
		}

		int paused;

		public void Pause ()
		{
			paused++;
		}

		public void Resume ()
		{
			paused--;
		}

		void Tick (object sender, TickEventArgs e)
		{
			total_elapsed += e.TicksElapsed;

			if (total_elapsed < millis)
				return;

			if (paused == 0)
				Redraw ();
		}

		public event EventHandler Painting;

		public void Redraw ()
		{
#if !RELEASE
			Rectangle fps_rect = Rectangle.Empty;
			if (show_fps) {
				fps_rect = new Rectangle (new Point (10, 10), new Size (80, 30));
				frame_count ++;
				if (frame_count == 50) {

					DateTime after = DateTime.Now;

					fps = 1.0 / (after - last_time).TotalSeconds * 50;
					last_time = after;
					frame_count = 0;

					/* make sure we invalidate the region where we're going to draw the fps/related info */
					Invalidate (fps_rect);
				}
			}
#endif
			//Console.WriteLine ("Redraw");

			if (Painting != null)
				Painting (this, EventArgs.Empty);

			if (dirty.IsEmpty)
				return;

			//Console.WriteLine (" + dirty = {0}", dirty);

			total_elapsed = 0;

			now = DateTime.Now;

#if !RELEASE
			if (debug_dirty) {
				paintingSurface.Fill (dirty, Color.Red);
				paintingSurface.Flip ();
			}
#endif
			paintingSurface.Fill(dirty, Color.Black);

			for (Layer i = Layer.Background; i < Layer.Count; i ++) {
				DrawLayer (layers[(int)i]);
			}

#if !RELEASE
			if (show_fps) {
				if (fps_surface != null)
					fps_surface.Dispose ();

				fps_surface = GuiUtil.ComposeText (String.Format ("fps: {0,0:F}", fps),
								   GuiUtil.GetFonts (Game.Instance.PlayingMpq)[1],
								   fontpal);

				paintingSurface.Blit (fps_surface, new Point (10,10));
			}
#endif
			paintingSurface.Flip ();

			dirty = Rectangle.Empty;

			total_elapsed = (DateTime.Now - now).Milliseconds;
		}

		public void DrawLayer (List<PainterDelegate> painters)
		{
			for (int i = 0; i < painters.Count; i ++)
				painters[i] (now);
		}

		public void Blit (Surface surf, Point p)
		{
			paintingSurface.Blit (surf, p);
		}

		public void Blit (Surface surf)
		{
			paintingSurface.Blit (surf);
		}

		public void Blit (Surface surf, Rectangle r1, Rectangle r2)
		{
			paintingSurface.Blit (surf, r1, r2);
		}

		public void DrawBox (Rectangle rect, Color color)
		{
			paintingSurface.DrawBox (rect, color);
		}

		Rectangle dirty = Rectangle.Empty;

		public Rectangle Dirty {
			get { return dirty; }
		}

		public void Invalidate (Rectangle r)
		{
			if (r.X >= Painter.SCREEN_RES_X
			    || r.Y >= Painter.SCREEN_RES_Y
			    || r.X + r.Width <= 0
			    || r.Y + r.Height <= 0)
				return;

			if (r.X < 0) {
				r.Width += r.X;
				r.X = 0;
			}
			if (r.Y < 0) {
				r.Height += r.Y;
				r.Y = 0;
			}

			if (r.X + r.Width > Painter.SCREEN_RES_X)
				r.Width = Painter.SCREEN_RES_X - r.X;

			if (r.Y + r.Height > Painter.SCREEN_RES_Y)
				r.Height = Painter.SCREEN_RES_Y - r.Y;
			
			if (dirty.IsEmpty)
				dirty = r;
			else if (dirty.Contains (r))
				return;
			else
				dirty = Rectangle.Union (dirty, r);
		}

		public void Invalidate ()
		{
			Invalidate (new Rectangle (0, 0, Painter.SCREEN_RES_X, Painter.SCREEN_RES_Y));
		}

		public int Width {
			get { return paintingSurface.Width; }
		}

		public int Height {
			get { return paintingSurface.Height; }
		}
	}
}
