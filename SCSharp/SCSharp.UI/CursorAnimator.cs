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

		DateTime last;
		TimeSpan delta_to_change = TimeSpan.FromMilliseconds (100);
		int current_frame;

		Surface[] surfaces;

		uint x;
		uint y;

		uint hot_x;
		uint hot_y;

		byte[] palette;

		public CursorAnimator (Grp grp, byte[] palette)
		{
			this.grp = grp;
			this.x = 100;
			this.y = 100;
			this.palette = palette;
			surfaces = new Surface[grp.FrameCount];
		}

		public void SetHotSpot (uint hot_x, uint hot_y)
		{
			this.hot_x = hot_x;
			this.hot_y = hot_y;
		}

		public void SetPosition (uint x, uint y)
		{
			this.x = x;
			this.y = y;
		}

		public uint HotX {
			get { return hot_x; }
		}

		public uint HotY {
			get { return hot_y; }
		}

		public void Paint (Surface surf, DateTime now)
		{
			delta_to_change -= now - last;
			if (delta_to_change < TimeSpan.Zero) {
				current_frame++;
				delta_to_change = TimeSpan.FromMilliseconds (200);
			}
			last = now;

			int draw_x = (int)(x - hot_x);
			int draw_y = (int)(y - hot_y);

			if (current_frame == grp.FrameCount)
				current_frame = 0;

			if (surfaces[current_frame] == null)
				surfaces[current_frame] = GuiUtil.CreateSurfaceFromBitmap (grp.GetFrame (current_frame),
											   grp.Width, grp.Height,
											   palette,
											   true);

			surf.Blit (surfaces[current_frame], new Point (draw_x, draw_y));
		}
	}
}
