
using System;
using System.Collections.Generic;

using SdlDotNet;

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
