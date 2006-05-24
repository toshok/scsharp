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

using SdlDotNet;
using SdlDotNet.Sprites;
using System.Drawing;

namespace SCSharp.UI
{

	public enum Layer
	{
		Foo,

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

		public Painter (Surface paintingSurface, int millis)
		{
			this.millis = millis;
			this.paintingSurface = paintingSurface;

			/* create an initialize our video surface */
			backbuffer = paintingSurface.CreateCompatibleSurface (paintingSurface.Size);
			backbuffer.Fill (new Rectangle (new Point (0, 0), backbuffer.Size), Color.Black);
			
			/* init our list of painter delegates */
			layers = new List<PainterDelegate>[(int)Layer.Count];
			for (Layer i = Layer.Background; i < Layer.Count; i ++)
				layers[(int)i] = new List<PainterDelegate>();

			/* and set ourselves up to invalidate at a regular interval*/
                        Events.Tick +=new TickEventHandler (Tick);
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

		void Tick (object sender, TickEventArgs e)
		{
			total_elapsed += e.TicksElapsed;

			if (total_elapsed < millis)
				return;

			total_elapsed = 0;

			now = DateTime.Now;

                        backbuffer.Fill(new Rectangle(new Point(0, 0), backbuffer.Size), Color.Black);
			
			for (Layer i = Layer.Background; i < Layer.Count; i ++)
				DrawLayer (layers[(int)i]);

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
