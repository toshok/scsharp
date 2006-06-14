//
// SCSharp.UI.ButtonElement
//
// Author:
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
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class ButtonElement : UIElement
	{
		public ButtonElement (UIScreen screen, BinElement el, byte[] palette)
			: base (screen, el, palette)
		{
		}

		Surface text_surf;
		int text_x, text_y;

		void CalculateTextPosition ()
		{
			if (text_surf == null)
				text_surf = GuiUtil.ComposeText (Text, Font, Palette, -1, -1,
								 Sensitive ? 4 : 24);

			if ((Flags & ElementFlags.CenterTextHoriz) == ElementFlags.CenterTextHoriz)
				text_x = (Width - text_surf.Width) / 2;
			else if ((Flags & ElementFlags.RightAlignText) == ElementFlags.RightAlignText)
				text_x = (Width - text_surf.Width);
			else
				text_x = 0;

			if ((Flags & ElementFlags.CenterTextVert) == ElementFlags.CenterTextVert)
				text_y = (Height - text_surf.Height) / 2;
			else if ((Flags & ElementFlags.BottomAlignText) == ElementFlags.BottomAlignText)
				text_y = (Height - text_surf.Height);
			else
				text_y = 0;
		}

		protected override Surface CreateSurface ()
		{
			Surface surf = new Surface (Width, Height);

			surf.TransparentColor = Color.Black; /* XXX */

			text_surf = null;
			CalculateTextPosition ();

			surf.Blit (text_surf, new Point (text_x, text_y));

			return surf;
		}

		public Point TextPosition {
			get { CalculateTextPosition (); return new Point (text_x, text_y); }
		}

		public override void MouseButtonDown (MouseButtonEventArgs args)
		{
		}

		public override void MouseButtonUp (MouseButtonEventArgs args)
		{
			if (PointInside (args.X, args.Y))
				OnActivate ();
		}

		public override void MouseEnter ()
		{
			if (Sensitive && (Flags & ElementFlags.RespondToMouse) == ElementFlags.RespondToMouse) {
				/* highlight the text */
				GuiUtil.PlaySound (Mpq, Builtins.MouseoverWav);
			}
			base.MouseEnter ();
		}
	}

}
