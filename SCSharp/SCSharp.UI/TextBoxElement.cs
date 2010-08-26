//
// SCSharp.UI.TextBoxElement
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

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
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

		public void KeyboardDown (KeyboardEventArgs args)
		{
			bool changed = false;

			/* navigation keys */
			if (args.Key == Key.LeftArrow) {
				if (cursor > 0) cursor--;
			}
			else if (args.Key == Key.RightArrow) {
				if (cursor < value.Length) cursor++;
			}
			else if (args.Key == Key.Home) {
				cursor = 0;
			}
			else if (args.Key == Key.End) {
				cursor = value.Length;
			}
			/* keys that modify the text */
			else if (args.Key == Key.Backspace) {
				if (value.Length > 0) {
					value = value.Remove (cursor-1, 1);
					cursor--;
					changed = true;
				}
			}
			else {
				char[] cs = Encoding.ASCII.GetChars (new byte[] {(byte)args.Key});
				foreach (char c in cs) {
					if (!Char.IsLetterOrDigit (c) && c != ' ')
						continue;
					char cc;
					if ((args.Mod & (ModifierKeys.RightShift | ModifierKeys.LeftShift)) != 0)
						cc = Char.ToUpper (c);
					else
						cc = c;
					value.Insert (cursor++, cc);
					changed = true;
				}
				changed = true;
			}

			if (changed)
				Text = Value;
		}

		public int ValueLength {
			get { return value.Length; }
		}

		public string Value {
			get { return value.ToString(); }
		}

		protected override Surface CreateSurface ()
		{
			return GuiUtil.ComposeText (Text, Font, Palette, Width, Height,
						    Sensitive ? 4 : 24);
		}
	}

}
