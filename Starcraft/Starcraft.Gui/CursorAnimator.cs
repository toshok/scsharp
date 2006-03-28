using System;
using SdlDotNet;

using System.Drawing;
using System.Drawing.Imaging;

namespace Starcraft {
	public class CursorAnimator {
		Grp grp;

		DateTime last;
		TimeSpan delta_to_change = TimeSpan.FromMilliseconds (200);
		int current_frame;

		uint x;
		uint y;

		uint hot_x;
		uint hot_y;

		public CursorAnimator (Grp grp)
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

			Surface frame = GuiUtil.CreateSurfaceFromBitmap (grp.GetFrame (current_frame),
									 grp.Width, grp.Height,
									 Palette.default_palette,
									 false);

			surf.Blit (frame, new Point (draw_x, draw_y));
		}
	}
}
