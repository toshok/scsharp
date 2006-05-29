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

	public delegate void PainterDelegate (Surface surf, DateTime now);

	public class Painter 
	{
		List<PainterDelegate>[] layers;
		int millis;
		int total_elapsed;

		DateTime now; /* the time of the last animation tick */

		Surface paintingSurface;
		Surface backbuffer;

		Layer paintingLayer;

		bool fullscreen;

		List<PainterDelegate> pendingRemoves;
		List<PainterDelegate> pendingAdds;
		bool pendingClear;

		public const int SCREEN_RES_X = 640;
		public const int SCREEN_RES_Y = 480;

		public Painter (bool fullscreen, int millis)
		{
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

		public Painter (Surface surf, int millis)
		{
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
					backbuffer.Fill (new Rectangle (new Point (0, 0), backbuffer.Size), Color.Black);
				}
			}
		}

		public void Add (Layer layer, PainterDelegate painter)
		{
			if (layer == paintingLayer) {
				pendingAdds.Add (painter);
			}
			else {
				layers[(int)layer].Add (painter);
			}
		}

		public void Remove (Layer layer, PainterDelegate painter)
		{
			if (layer == paintingLayer)
				pendingRemoves.Add (painter);
			else
				layers[(int)layer].Remove (painter);
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

		void Tick (object sender, TickEventArgs e)
		{
			total_elapsed += e.TicksElapsed;

			if (total_elapsed < millis)
				return;

			total_elapsed = 0;

			now = DateTime.Now;

                        backbuffer.Fill(new Rectangle(new Point(0, 0), backbuffer.Size), Color.Black);
			
			for (Layer i = Layer.Background; i < Layer.Count; i ++) {
				paintingLayer = i;

				DrawLayer (layers[(int)i]);

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

			paintingSurface.Blit (backbuffer);

			paintingSurface.Flip ();
		}

		void DrawLayer (List<PainterDelegate> painters)
		{
			foreach (PainterDelegate p in painters)
				p (backbuffer, now);
		}
	}
}
