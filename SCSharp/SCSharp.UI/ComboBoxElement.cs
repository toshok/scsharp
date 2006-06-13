//
// SCSharp.UI.ComboBoxElement
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
	public class ComboBoxElement : UIElement
	{
		List<string> items;
		int cursor = -1;
		Surface dropdownSurface;
		bool dropdown_visible;
		int selected_item;

		public ComboBoxElement (UIScreen screen, BinElement el, byte[] palette)
			: base (screen, el, palette)
		{
			items = new List<string> ();
		}

		public int SelectedIndex {
			get { return cursor; }
			set { cursor = value;
			      Invalidate (); }
		}

		public string SelectedItem {
			get { return items[cursor]; }
		}

		public void AddItem (string item)
		{
			AddItem (item, true);
		}

		public void AddItem (string item, bool select)
		{
			items.Add (item);
			if (select || cursor == -1)
				cursor = items.IndexOf (item);
			Invalidate ();
		}

		public void RemoveAt (int index)
		{
			items.RemoveAt (index);
			if (items.Count == 0)
				cursor = -1;
			Invalidate ();
		}

		public void Clear ()
		{
			items.Clear ();
			cursor = -1;
			Invalidate ();
		}

		public bool Contains (string item)
		{
			return items.Contains (item);
		}

		public override void MouseButtonDown (MouseButtonEventArgs args)
		{
			ShowDropdown();
		}

		public override void MouseButtonUp (MouseButtonEventArgs args)
		{
			HideDropdown ();
		}

		public override void PointerMotion (MouseMotionEventArgs args)
		{
			/* if the dropdown is visible, see if we're inside it */
			if (!dropdown_visible)
				return;

			if (/*
			      starcraft doesn't include this check..  should we?
			      args.X >= X1 && args.X < X1 + dropdownSurface.Width &&
			    */
			    args.Y >= Y1 + Height && args.Y < Y1 + Height + dropdownSurface.Height) {

				int new_selected_item = (args.Y - (Y1 + Height)) / Font.LineSize;

				if (selected_item != new_selected_item) {
					selected_item = new_selected_item;
					CreateDropdownSurface ();
				}
			}
		}

		void PaintDropdown (DateTime dt)
		{
			Painter.Instance.Blit (dropdownSurface, new Point (X1, Y1 + Height));
		}

		void ShowDropdown ()
		{
			dropdown_visible = true;
			selected_item = cursor;
			CreateDropdownSurface ();
			Painter.Instance.Add (Layer.Popup, PaintDropdown);
		}

		void HideDropdown ()
		{
			dropdown_visible = false;
			if (cursor != selected_item) {
				cursor = selected_item;
				if (SelectionChanged != null)
					SelectionChanged (cursor);
				Invalidate ();
			}
			Painter.Instance.Remove (Layer.Popup, PaintDropdown);
		}

		protected override Surface CreateSurface ()
		{
			Surface surf = new Surface (Width, Height);

			/* XXX draw the arrow (and border) */

			if (cursor != -1) {
				Surface item_surface = GuiUtil.ComposeText (items[cursor], Font, Palette, 4);

				item_surface.TransparentColor = Color.Black;
				surf.Blit (item_surface, new Point (0, 0));
			}

			return surf;
		}

		void CreateDropdownSurface ()
		{
			dropdownSurface = new Surface (Width, items.Count * Font.LineSize);

			int y = 0;
			for (int i = 0; i < items.Count; i ++) {
				Surface item_surface = GuiUtil.ComposeText (items[i], Font, Palette,
									    i == selected_item ? 4 : 24);

				item_surface.TransparentColor = Color.Black;

				dropdownSurface.Blit (item_surface, new Point (0, y));
				y += item_surface.Height;
			}
		}

		public event ComboBoxSelectionChanged SelectionChanged;
	}

	public delegate void ComboBoxSelectionChanged (int selectedIndex);
}
