//
// SCSharp.UI.SwooshPainter
//
// Authors:
//	Chris Toshok (toshok@gmail.com)
//
// Copyright 2006-2010 Chris Toshok
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

using SdlDotNet.Core;
using SdlDotNet.Graphics;


namespace SCSharp.UI
{

	public delegate void DoneSwooshingDelegate ();

#if false
	public class SwooshPainter
	{
		public enum Direction {
			FromLeft,
			FromRight
		}

		bool initialized_current_x;
		int current_x;
		int delta_x = 20;
		TimeSpan delta_to_change = TimeSpan.FromMilliseconds (50);
		DateTime last;
		int eventual_x;
		int y;
		Gdk.Pixbuf sub_pixbuf;
		Direction direction;
		PainterDelegate painter;
		int alpha;

		public SwooshPainter (Direction direction,
				      PainterDelegate painter,
				      int eventual_x, int y, int alpha)
		{
			this.y = y;
			this.eventual_x = eventual_x;
			this.direction = direction;
			this.painter = painter;
			this.alpha = alpha;
		}

		void CreateSubPixbuf (Gdk.Pixbuf pb)
		{
			sub_pixbuf = new Gdk.Pixbuf (Colorspace.Rgb, true, 8, pb.Width, pb.Height);
		}

		public void Paint (Gdk.Pixbuf pb, DateTime now)
		{
			if (!initialized_current_x) {
				initialized_current_x = true;

				if (direction == Direction.FromLeft)
					current_x = -1 * pb.Width;
				else
					current_x = pb.Width * 2;
			}

			if (current_x == eventual_x) {
				painter (pb, now);
				RaiseDoneSwooshing();
				return;
			}
				
			if (sub_pixbuf == null)
				CreateSubPixbuf(pb);

			sub_pixbuf.Fill (0x000000);

			/* first off call the subpainter to update our display */
			painter (sub_pixbuf, now);

			if (last == DateTime.MinValue)
				last = now;

			/* then move ourselves if we need to */
			delta_to_change -= now - last;
			if (delta_to_change < TimeSpan.Zero) {
				if (direction == Direction.FromLeft) {
					current_x += delta_x;
					if (current_x > eventual_x) {
						current_x = eventual_x;
					}
				}
				else {
					current_x -= delta_x;
					if (current_x < eventual_x) {
						current_x = eventual_x;
					}
				}

				delta_to_change = TimeSpan.FromMilliseconds (50);
			}
			last = now;

			/* then composite our entire pixbuf on the root pb */
			if (current_x < 0) {
				if (current_x + sub_pixbuf.Width <= 0)
					return;
				sub_pixbuf.Composite (pb, 0, y, sub_pixbuf.Width + current_x, sub_pixbuf.Height,
						      current_x, y, 1, 1, InterpType.Nearest, alpha);
			}
			else if (current_x + sub_pixbuf.Width >= pb.Width) {
				if (current_x > pb.Width)
					return;
				sub_pixbuf.Composite (pb, current_x, y, pb.Width - current_x, sub_pixbuf.Height,
						      current_x, y, 1, 1, InterpType.Nearest, alpha);
			}
			else {
				sub_pixbuf.Composite (pb, current_x, y, sub_pixbuf.Width, sub_pixbuf.Height,
						      current_x, y, 1, 1, InterpType.Nearest, alpha);
			}
		}

		void RaiseDoneSwooshing ()
		{
			if (DoneSwooshing != null)
				DoneSwooshing ();
		}

		public event DoneSwooshingDelegate DoneSwooshing;
	}
#endif

}
