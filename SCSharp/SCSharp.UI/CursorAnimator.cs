//
// SCSharp.UI.CursorAnimator
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
using SdlDotNet;

using System.Drawing;
using System.Drawing.Imaging;

namespace SCSharp.UI
{
	public class CursorAnimator
	{
		Grp grp;

		int totalElapsed;
		int millisDelay = 200;

		int current_frame;

		Surface[] surfaces;

		int x;
		int y;

		int hot_x;
		int hot_y;

		byte[] palette;

		public CursorAnimator (Grp grp, byte[] palette)
		{
			this.grp = grp;
			this.x = 100;
			this.y = 100;
			this.palette = palette;
			surfaces = new Surface[grp.FrameCount];
		}

		public void SetHotSpot (int hot_x, int hot_y)
		{
			Painter.Instance.Invalidate (new Rectangle (x - hot_x, y - hot_y, grp.Width, grp.Height));
			this.hot_x = hot_x;
			this.hot_y = hot_y;
			Painter.Instance.Invalidate (new Rectangle (x - hot_x, y - hot_y, grp.Width, grp.Height));
		}

		public void SetPosition (int x, int y)
		{
			Painter.Instance.Invalidate (new Rectangle (x - hot_x, y - hot_y, grp.Width, grp.Height));
			this.x = x;
			this.y = y;
			Painter.Instance.Invalidate (new Rectangle (x - hot_x, y - hot_y, grp.Width, grp.Height));
		}

		public int X {
			get { return x; }
		}

		public int Y {
			get { return y; }
		}

		public int HotX {
			get { return hot_x; }
		}

		public int HotY {
			get { return hot_y; }
		}

		public void AddToPainter ()
		{
			Painter.Instance.Add (Layer.Cursor, Paint);
                        Events.Tick += CursorTick;
		}

		public void RemoveFromPainter ()
		{
			Painter.Instance.Remove (Layer.Cursor, Paint);
                        Events.Tick -= CursorTick;
		}

		public void CursorTick (object sender, TickEventArgs e)
		{
			totalElapsed += e.TicksElapsed;

			if (totalElapsed < millisDelay)
				return;

			totalElapsed = 0;
			current_frame ++;
			Painter.Instance.Invalidate (new Rectangle (x - hot_x, y - hot_y, grp.Width, grp.Height));
		}

		void Paint (DateTime now)
		{
			Rectangle dest = Rectangle.Intersect (Painter.Instance.Dirty,
							      new Rectangle (x - hot_x, y - hot_y, grp.Width, grp.Height));
			if (dest.IsEmpty)
				return;

			int draw_x = (int)(x - hot_x);
			int draw_y = (int)(y - hot_y);

			Rectangle source = dest;
			source.X -= draw_x;
			source.Y -= draw_y;

			if (current_frame == grp.FrameCount)
				current_frame = 0;

			if (surfaces[current_frame] == null)
				surfaces[current_frame] = GuiUtil.CreateSurfaceFromBitmap (grp.GetFrame (current_frame),
											   grp.Width, grp.Height,
											   palette,
											   true);

			Painter.Instance.Blit (surfaces[current_frame], dest, source);
		}
	}
}
