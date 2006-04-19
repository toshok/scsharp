using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
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
			      ClearSurface (); }
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

		void PaintDropdown (Surface surf, DateTime dt)
		{
			surf.Blit (dropdownSurface, new Point (X1, Y1 + Height));
		}

		void ShowDropdown ()
		{
			dropdown_visible = true;
			selected_item = cursor;
			CreateDropdownSurface ();
			ParentScreen.Painter.Add (Layer.Popup, PaintDropdown);
		}

		void HideDropdown ()
		{
			dropdown_visible = false;
			if (cursor != selected_item) {
				cursor = selected_item;
				ClearSurface ();
			}
			ParentScreen.Painter.Remove (Layer.Popup, PaintDropdown);
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
