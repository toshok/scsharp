using System;
using Gdk;

namespace Starcraft {
	public class CursorAnimator {
		GRP grp;

		DateTime last;
		TimeSpan delta_to_change = TimeSpan.FromMilliseconds (200);
		int current_frame;

		uint x;
		uint y;

		uint hot_x;
		uint hot_y;

		public CursorAnimator (GRP grp)
		{
			this.grp = grp;
			this.x = 100;
			this.y = 100;
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

		public void Paint (Gdk.Pixbuf pb, DateTime now)
		{
			delta_to_change -= now - last;
			if (delta_to_change < TimeSpan.Zero) {
				current_frame++;
				delta_to_change = TimeSpan.FromMilliseconds (200);
			}
			last = now;

			int draw_x = (int)(x - hot_x);
			int draw_y = (int)(y - hot_y);
			int draw_width = grp.Width;
			int draw_height = grp.Height;

			if (current_frame == grp.FrameCount)
				current_frame = 0;

			byte[] pixbuf_data = Util.CreatePixbufData (grp.GetFrame (current_frame),
								    grp.Width, grp.Height,
								    Palette.default_palette,
								    true);
			Gdk.Pixbuf frame = new Gdk.Pixbuf (pixbuf_data,
							   Colorspace.Rgb,
							   true,
							   8,
							   grp.Width, grp.Height,
							   grp.Width * 4,
							   null);

			frame.AddAlpha (true, 0, 0, 0);


			if (draw_x < 0 || draw_x + grp.Width > pb.Width ||
			    draw_y < 0 || draw_y + grp.Height > pb.Height) {

				int sub_x, sub_y, sub_width, sub_height;

				sub_x = 0;
				sub_y = 0;
				sub_width = grp.Width;
				sub_height = grp.Height;

				if (draw_x < 0) {
					sub_x = -1 * draw_x;
					sub_width -= sub_x;
					draw_x = 0;
				}
				if (draw_y < 0) {
					sub_y = -1 * draw_y;
					sub_height -= sub_y;
					draw_y = 0;
				}

				if (draw_x + grp.Width > pb.Width)
					sub_width = pb.Width - draw_x;

				if (draw_y + grp.Width > pb.Height)
					sub_height = pb.Height - draw_y;

				Gdk.Pixbuf sub = new Gdk.Pixbuf (frame, sub_x, sub_y, sub_width, sub_height);
				sub.AddAlpha (true, 0, 0, 0);

				frame.Dispose();
				frame = sub;
				draw_width = sub_width;
				draw_height = sub_height;
			}

			frame.Composite (pb, (int)draw_x, (int)draw_y, draw_width, draw_height,
					 (int)draw_x, (int)draw_y, 1, 1, InterpType.Nearest, 0xff);

			frame.Dispose();
		}
	}
}
