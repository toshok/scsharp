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
		Hud,
		UI,
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
		Surface backbuffer;

		Layer paintingLayer = Layer.Count;

		bool fullscreen;

		List<PainterDelegate> pendingRemoves;
		List<PainterDelegate> pendingAdds;
		bool pendingClear;

		public const int SCREEN_RES_X = 640;
		public const int SCREEN_RES_Y = 480;

#if !RELEASE
		DateTime last_time;
		byte[] fontpal;
		double fps;
		int frame_count;
#endif

		static Painter instance;

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
			Pcx pcx = new Pcx ();
			pcx.ReadFromStream ((Stream)Game.Instance.PlayingMpq.GetResource ("game\\tfontgam.pcx"), 0, 0);
			fontpal = pcx.Palette;
#endif

			this.millis = millis;

			this.fullscreen = fullscreen;
			Fullscreen = fullscreen;
			
			/* init our list of painter delegates */
			layers = new List<PainterDelegate>[(int)Layer.Count];
			for (Layer i = Layer.Background; i < Layer.Count; i ++)
				layers[(int)i] = new List<PainterDelegate>();

			pendingRemoves = new List<PainterDelegate>();
			pendingAdds = new List<PainterDelegate>();

			/* and set ourselves up to invalidate at a regular interval*/
                        Events.Tick +=new TickEventHandler (Tick);
		}

#if false
		public Painter (Surface surf, int millis)
		{
#if !RELEASE
			Pcx pcx = new Pcx ();
			pcx.ReadFromStream ((Stream)Game.Instance.PlayingMpq.GetResource ("game\\tfontgam.pcx"), 0, 0);
			fontpal = pcx.Palette;
#endif
			this.millis = millis;

			this.paintingSurface = surf;

			backbuffer = paintingSurface.CreateCompatibleSurface (paintingSurface.Size);
			backbuffer.Fill (new Rectangle (new Point (0, 0), backbuffer.Size), Color.Black);

			/* init our list of painter delegates */
			layers = new List<PainterDelegate>[(int)Layer.Count];
			for (Layer i = Layer.Background; i < Layer.Count; i ++)
				layers[(int)i] = new List<PainterDelegate>();

			pendingRemoves = new List<PainterDelegate>();
			pendingAdds = new List<PainterDelegate>();

			/* and set ourselves up to invalidate at a regular interval*/
                        Events.Tick +=new TickEventHandler (Tick);
		}
#endif

		public bool Fullscreen {
			get { return fullscreen; }
			set {
				if (fullscreen != value || paintingSurface == null) {
					fullscreen = value;

					if (fullscreen)
						paintingSurface = Video.SetVideoMode (SCREEN_RES_X, SCREEN_RES_Y);
					else
						paintingSurface = Video.SetVideoModeWindow (SCREEN_RES_X, SCREEN_RES_Y);

					backbuffer = paintingSurface.CreateCompatibleSurface (paintingSurface.Size);
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
			if (layer == paintingLayer) {
				pendingAdds.Add (painter);
			}
			else {
				layers[(int)layer].Add (painter);
				Invalidate ();
			}
		}

		public void Remove (Layer layer, PainterDelegate painter)
		{
			if (layer == paintingLayer)
				pendingRemoves.Add (painter);
			else {
				layers[(int)layer].Remove (painter);
				Invalidate ();
			}
		}

		public void Clear (Layer layer)
		{
			if (layer == paintingLayer) {
				pendingClear = true;
				pendingAdds.Clear ();
				pendingRemoves.Clear ();
			}
			else {
				layers[(int)layer].Clear ();
			}
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
			/* make sure we invalidate the region where we're going to draw the fps/related info */
			Invalidate (new Rectangle (new Point (10, 10), new Size (150, 50)));
#endif
			//Console.WriteLine ("Redraw");

			if (Painting != null)
				Painting (this, EventArgs.Empty);

			if (dirty.IsEmpty)
				return;

			//Console.WriteLine (" + dirty = {0}", dirty);

			total_elapsed = 0;

			now = DateTime.Now;

                        backbuffer.Fill(dirty, Color.Black);
			
			for (Layer i = Layer.Background; i < Layer.Count; i ++) {
				paintingLayer = i;

				DrawLayer (layers[(int)i]);

				if (pendingClear ||
				    pendingAdds.Count != 0 ||
				    pendingRemoves.Count != 0)
					Invalidate ();

				if (pendingClear)
					layers[(int)i].Clear ();
				for (int j = 0; j < pendingAdds.Count; j ++)
					layers[(int)i].Add (pendingAdds[j]);
				pendingAdds.Clear ();
				for (int j = 0; j < pendingRemoves.Count; j ++)
					layers[(int)i].Remove (pendingRemoves[j]);
				pendingRemoves.Clear ();
			}

			paintingLayer = Layer.Count;

#if !RELEASE
			frame_count ++;
			if (frame_count == 30) {

				DateTime after = DateTime.Now;

				fps = 1.0 / (after - last_time).TotalSeconds * 30;

				last_time = after;
				frame_count = 0;
			}

			Surface s;

			s = GuiUtil.ComposeText (String.Format ("fps: {0,2:F}", fps),
						 GuiUtil.GetFonts (Game.Instance.PlayingMpq)[1],
						 fontpal);

			backbuffer.Blit (s, new Point (10, 10));

#endif
			paintingSurface.Blit (backbuffer);

			paintingSurface.Flip ();

			dirty = Rectangle.Empty;
		}

		public void DrawLayer (List<PainterDelegate> painters)
		{
			foreach (PainterDelegate p in painters) {
				p (now);
			}
		}

		public void Blit (Surface surf, Point p)
		{
			backbuffer.Blit (surf, p);
		}

		public void Blit (Surface surf)
		{
			backbuffer.Blit (surf);
		}

		public void Blit (Surface surf, Rectangle r1, Rectangle r2)
		{
			backbuffer.Blit (surf, r1, r2);
		}

		public void DrawBox (Rectangle rect, Color color)
		{
			backbuffer.DrawBox (rect, color);
		}

		Rectangle dirty = Rectangle.Empty;

		public Rectangle Dirty {
			get { return dirty; }
		}

		public void Invalidate (Rectangle r)
		{
			//Console.WriteLine ("invalidating {0}", r);
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
			get { return backbuffer.Width; }
		}

		public int Height {
			get { return backbuffer.Height; }
		}
	}
}
