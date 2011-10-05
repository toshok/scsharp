//
// SCSharpMac.UI.ButtonElement
//
// Author:
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
using System.IO;
using System.Text;
using System.Threading;

using MonoMac.CoreAnimation;
using MonoMac.AppKit;

using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
{
	public class ButtonElement : UIElement
	{
		public ButtonElement (UIScreen screen, BinElement el, byte[] palette)
			: base (screen, el, palette)
		{
		}

		CALayer text_layer;
		int text_x, text_y;

		void CalculateTextPosition ()
		{
			if (text_layer == null)
				text_layer = GuiUtil.ComposeText (Text, Font, Palette, -1, -1, Sensitive ? 4 : 24);

			if ((Flags & ElementFlags.CenterTextHoriz) == ElementFlags.CenterTextHoriz)
				text_x = (int)((Width - text_layer.Bounds.Width) / 2);
			else if ((Flags & ElementFlags.RightAlignText) == ElementFlags.RightAlignText)
				text_x = (int)(Width - text_layer.Bounds.Width);
			else
				text_x = 0;

			if ((Flags & ElementFlags.CenterTextVert) == ElementFlags.CenterTextVert)
				text_y = (int)((Height - text_layer.Bounds.Height) / 2 - text_layer.Bounds.Height);
			else if ((Flags & ElementFlags.TopAlignText) == ElementFlags.TopAlignText)
				text_y = (int)(Height - text_layer.Bounds.Height);
			else
				text_y = (int)0;
		}

		protected override CALayer CreateLayer ()
		{
			CALayer layer = CALayer.Create ();
			layer.Bounds = new RectangleF (0, 0, Width, Height);

			text_layer = null;
			CalculateTextPosition ();
			
			text_layer.AnchorPoint = new PointF (0, 0);
			text_layer.Position = new PointF (text_x, text_y);
			layer.AddSublayer (text_layer);
#if DEBUG_UIELEMENT_BOUNDS			
			text_layer.BorderWidth = 1;
			text_layer.BorderColor = new MonoMac.CoreGraphics.CGColor (1, 0, 0, 1);
#endif
			
			return layer;
		}

		public Point TextPosition {
			get { CalculateTextPosition (); return new Point (text_x, text_y); }
		}
		
		public override void MouseButtonDown (NSEvent theEvent)
		{
		}

		public override void MouseButtonUp (NSEvent theEvent)
		{
			PointF ui_pt = ParentScreen.ScreenToLayer (theEvent.LocationInWindow);
			
			if (PointInside (ui_pt))
				OnActivate ();
		}

		public override void MouseEnter ()
		{
#if notyet
			if (Sensitive && (Flags & ElementFlags.RespondToMouse) == ElementFlags.RespondToMouse) {
				/* highlight the text */
				GuiUtil.PlaySound (Mpq, Builtins.MouseoverWav);
			}
#endif
			base.MouseEnter ();
		}
	}

}

