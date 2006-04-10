using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class ListBoxElement : UIElement
	{
		List<string> items;
		int cursor = -1;

		public ListBoxElement (Mpq mpq, BinElement el, byte[] palette)
			: base (mpq, el, palette)
		{
			items = new List<string> ();
		}

		public void KeyboardDown (KeyboardEventArgs args)
		{
			bool selection_changed = false;

			/* navigation keys */
			if (args.Key == Key.UpArrow) {
				if (cursor > 0) {
					cursor--;
					selection_changed = true;
				}
			}
			else if (args.Key == Key.DownArrow) {
				if (cursor < items.Count - 1) {
					cursor++;
					selection_changed = true;
				}
			}

			if (selection_changed) {
				ClearSurface ();
				if (SelectionChanged != null)
					SelectionChanged (cursor);
			}

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

		public bool Contains (string item)
		{
			return items.Contains (item);
		}

		protected override Surface CreateSurface ()
		{
			Surface surf = new Surface (Width, Height);

			int y = 0;
			for (int i = 0; i < items.Count; i ++) {
				Surface item_surface = GuiUtil.ComposeText (items[i], Font, Palette,
									    cursor == i ? 4 : 24);

				surf.Blit (item_surface, new Point (0, y));
				y += item_surface.Height;
			}

			return surf;
		}

		public event ListBoxSelectionChanged SelectionChanged;
	}

	public delegate void ListBoxSelectionChanged (int selectedIndex);

}
