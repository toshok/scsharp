//
// SCSharpMac.UI.TextBoxElement
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
using System.IO;
using System.Text;
using System.Threading;

using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
{
	// TODO: draw an actual cursor..
	public class TextBoxElement : UIElement
	{
		StringBuilder value;
		int cursor = 0;

		public TextBoxElement (UIScreen screen, BinElement el, byte[] palette)
			: base (screen, el, palette)
		{
			value = new StringBuilder();
		}
		
		public void KeyboardDown (NSEvent theEvent)
		{
			Console.WriteLine ("what up, my nig");
			
			bool changed = false;

			if ((theEvent.ModifierFlags & NSEventModifierMask.NumericPadKeyMask) == NSEventModifierMask.NumericPadKeyMask) {
				/* navigation keys */
				if (theEvent.Characters[0] == (char)NSKey.LeftArrow) {
					if (cursor > 0) cursor--;
				}
				else if (theEvent.Characters[0] == (char)NSKey.RightArrow) {
					if (cursor < value.Length) cursor++;
				}
			}
			else if ((theEvent.ModifierFlags & NSEventModifierMask.FunctionKeyMask) == NSEventModifierMask.FunctionKeyMask) {		
				if (theEvent.Characters[0] == (char)NSKey.Home) {
					cursor = 0;
				}
				else if (theEvent.Characters[0] == (char)NSKey.End) {
					cursor = value.Length;
				}
			}
			/* keys that modify the text */
			else if (theEvent.Characters[0] == (char)0x7f) {
				if (value.Length > 0) {
					value = value.Remove (cursor-1, 1);
					cursor--;
					changed = true;
				}
			}
			else {
				foreach (char c in theEvent.CharactersIgnoringModifiers) {
					if (!Char.IsLetterOrDigit (c) && c != ' ')
						continue;
					char cc;
					if ((theEvent.ModifierFlags & NSEventModifierMask.AlphaShiftKeyMask) == NSEventModifierMask.AlphaShiftKeyMask)
						cc = Char.ToUpper (c);
					else
						cc = c;
					value.Insert (cursor++, cc);
					changed = true;
				}
				changed = true;
			}

			if (changed) {
				Text = Value;
				Layer.SetNeedsDisplay();
			}
		}
		
		public int ValueLength {
			get { return value.Length; }
		}

		public string Value {
			get { return value.ToString(); }
		}

		protected override CALayer CreateLayer ()
		{
			CALayer layer = CALayer.Create ();

			layer.AnchorPoint = new PointF (0, 0);
			layer.Bounds = new RectangleF (0, 0, Width, Height);
			
			layer.Delegate = new TextElementLayerDelegate (this);
			
			return layer;
		}
	}
	
	class TextElementLayerDelegate : CALayerDelegate {
		TextBoxElement el;
		
		public TextElementLayerDelegate (TextBoxElement el)
		{
			this.el = el;
		}
		
		public override void DrawLayer (CALayer layer, CGContext context)
		{
			GuiUtil.RenderTextToContext (context, new PointF (0, el.Height / 2), el.Text, el.Font, el.Palette, el.Sensitive ? 4 : 24);
		}
	}

}

