//
// SCSharp.UI.ListBoxElement
//
// Authors:
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class ListBoxElement : UIElement
	{
		List<string> items;
		int cursor = -1;
		bool selectable = true;
		int num_visible;
		int first_visible;

		public ListBoxElement (UIScreen screen, BinElement el, byte[] palette)
			: base (screen, el, palette)
		{
			items = new List<string> ();

			num_visible = Height / Font.LineSize;
			first_visible = 0;
		}

		public void KeyboardDown (KeyboardEventArgs args)
		{
			bool selection_changed = false;

			/* navigation keys */
			if (args.Key == Key.UpArrow) {
				if (cursor > 0) {
					cursor--;
					selection_changed = true;

					if (cursor < first_visible)
						first_visible = cursor;
				}
			}
			else if (args.Key == Key.DownArrow) {
				if (cursor < items.Count - 1) {
					cursor++;
					selection_changed = true;

					if (cursor >= first_visible + num_visible)
						first_visible = cursor - num_visible + 1;
				}
			}

			if (selection_changed) {
				ClearSurface ();
				if (SelectionChanged != null)
					SelectionChanged (cursor);
			}

		}

		public bool Selectable {
			get { return selectable; }
			set { selectable = value; }
		}

		public int SelectedIndex {
			get { return cursor; }
			set { cursor = value;
			      ClearSurface (); }
		}

		public string SelectedItem {
			get { return items[cursor]; }
		}

		public void AddItem (string item)
		{
			items.Add (item);
			if (cursor == -1)
				cursor = 0;
			ClearSurface ();
		}

		public void RemoveAt (int index)
		{
			items.RemoveAt (index);
			if (items.Count == 0)
				cursor = -1;
			ClearSurface ();
		}

		public void Clear ()
		{
			items.Clear ();
			cursor = -1;
			ClearSurface ();
		}

		public bool Contains (string item)
		{
			return items.Contains (item);
		}

		protected override Surface CreateSurface ()
		{
			Surface surf = new Surface (Width, Height);

			int y = 0;
			for (int i = first_visible; i < first_visible + num_visible; i ++) {
				if (i >= items.Count)
					break;
				Surface item_surface = GuiUtil.ComposeText (items[i], Font, Palette,
									    (!selectable || cursor == i) ? 4 : 24);

				surf.Blit (item_surface, new Point (0, y));
				y += item_surface.Height;
			}

			surf.TransparentColor = Color.Black; /* XXX */

			return surf;
		}

		public event ListBoxSelectionChanged SelectionChanged;
	}

	public delegate void ListBoxSelectionChanged (int selectedIndex);
}
