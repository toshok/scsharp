using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
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
