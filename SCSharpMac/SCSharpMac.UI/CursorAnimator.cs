//
// SCSharpMac.UI.CursorAnimator
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

using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;
using MonoMac.AppKit;

using SCSharp;

using System.Drawing;

namespace SCSharpMac.UI
{
	public class CursorAnimator : CALayer
	{
		Grp grp;
	
		long	 totalElapsed;
		int millisDelay = 100;

		int current_frame;
		
		CGImage[] frames;

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
			frames = new CGImage[grp.FrameCount];
			
			Bounds = new RectangleF (100, 100, grp.Width, grp.Height);
			AnchorPoint = new PointF (0, 0);
			SetNeedsDisplay ();			
		}

		public void SetHotSpot (int hot_x, int hot_y)
		{
			this.hot_x = hot_x;
			this.hot_y = hot_y;
			Position = new PointF (x - hot_x, y - hot_y);
		}

		public void SetPosition (int x, int y)
		{
			this.x = x;
			this.y = y;
			Position = new PointF (x - hot_x, y - hot_y);
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
			Game.Instance.Tick += CursorTick;
		}

		public void RemoveFromPainter ()
		{
			Game.Instance.Tick -= CursorTick;
		}

		public void CursorTick (object sender, TickEventArgs e)
		{
			totalElapsed += e.MillisecondsElapsed;

			if (totalElapsed < millisDelay)
				return;

			totalElapsed = 0;
			current_frame++;
			SetNeedsDisplay ();
		}
		
		public override void DrawInContext (CGContext ctx)
		{
			current_frame = current_frame % frames.Length;

			if (frames[current_frame] == null)
				frames[current_frame] = GuiUtil.CGImageFromBitmap (grp.GetFrame (current_frame),
											   grp.Width, grp.Height,
											   palette,
											   true);
				
			ctx.DrawImage (new RectangleF (x, y, grp.Width, grp.Height), frames[current_frame]);
		}
	}
}