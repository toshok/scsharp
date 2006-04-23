using System;
using SdlDotNet;

using System.Drawing;
using System.Drawing.Imaging;

namespace SCSharp {
	public class CursorAnimator {
		Grp grp;

		DateTime last;
		TimeSpan delta_to_change = TimeSpan.FromMilliseconds (200);
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
