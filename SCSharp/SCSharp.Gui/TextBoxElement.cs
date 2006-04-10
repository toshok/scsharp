using System;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
{
	public class TextBoxElement : UIElement
	{
		StringBuilder value;
		int cursor = 0;

		public TextBoxElement (Mpq mpq, BinElement el, byte[] palette)
			: base (mpq, el, palette)
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
