//
// SCSharpMac.UI.ListBoxElement
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.CoreAnimation;

using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
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

		public void KeyboardDown (NSEvent theEvent)
		{
			bool selection_changed = false;

			/* navigation keys */
			if (theEvent.CharactersIgnoringModifiers[0] == (char)NSKey.UpArrow) {
				if (cursor > 0) {
					cursor--;
					selection_changed = true;

					if (cursor < first_visible)
						first_visible = cursor;
				}
			}
			else if (theEvent.CharactersIgnoringModifiers[0] == (char)NSKey.DownArrow) {
				if (cursor < items.Count - 1) {
					cursor++;
					selection_changed = true;

					if (cursor >= first_visible + num_visible)
						first_visible = cursor - num_visible + 1;
				}
			}

			if (selection_changed) {
				Invalidate ();
				if (SelectionChanged != null)
					SelectionChanged (cursor);
			}
		}
		
		bool selecting;
		int selectionIndex;
		
#if notyet
		public override void MouseWheel (MouseButtonEventArgs args)
		{
			bool need_redraw = false;

			if (args.Button == MouseButton.WheelUp) {
				if (first_visible > 0) {
					first_visible --;
					need_redraw = true;
				}
			}
			else {
				if (first_visible + num_visible < items.Count - 1) {
					first_visible ++;
					need_redraw = true;
				}
			}

			if (need_redraw)
				Invalidate ();
		}

		public override void MouseButtonDown (MouseButtonEventArgs args)
		{
			bool need_redraw = false;
			/* if we're over the scrollbar handle that here */

			/* otherwise start our selection */
			selecting = true;

			/* otherwise, select the clicked-on item (if
			 * there is one) */
			int index = (args.Y - Y1) / Font.LineSize + first_visible;
			if (index < items.Count) {
				selectionIndex = index;
				need_redraw = true;
			}

			if (need_redraw)
				Invalidate ();
		}

		public override void PointerMotion (MouseMotionEventArgs args)
		{
			if (!selecting)
				return;

			int index = (args.Y - Y1) / Font.LineSize + first_visible;
			if (index < items.Count) {
				selectionIndex = index;
				Invalidate ();
			}
		}

		public override void MouseButtonUp (MouseButtonEventArgs args)
		{
			if (!selecting)
				return;

			selecting = false;
			if (selectionIndex != cursor) {
				cursor = selectionIndex;
				if (SelectionChanged != null)
					SelectionChanged (cursor);
			}

			Invalidate ();
		}
#endif

		public bool Selectable {
			get { return selectable; }
			set { selectable = value; }
		}
		
		public bool Selecting {
			get { return selecting; }
		}
		
		public int SelectedIndex {
			get { return cursor; }
			set {
				if (value < 0 || value > items.Count)
					throw new ArgumentException ("value");
				if (cursor != value) {
					cursor = value;
					
					if (SelectionChanged != null)
						SelectionChanged (cursor);

					Invalidate ();
				}
			}
		}
		
		public int SelectionIndex {
			get { return selectionIndex; }
		}
		
		public string SelectedItem {
			get { return items[cursor]; }
		}
		
		public List<string> Items {
			get { return items; }			
		}
		
		public int FirstVisible {
			get { return first_visible; }
		}
		
		public int NumVisible {
			get { return num_visible; }
		}
		
		public void AddItem (string item)
		{
			items.Add (item);
			if (cursor == -1) {
				cursor = 0;

				if (SelectionChanged != null)
					SelectionChanged (cursor);
			}
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

		protected override CALayer CreateLayer ()
		{
			CALayer layer = CALayer.Create ();
			
			layer.Bounds = new RectangleF (0, 0, Width, Height);
			layer.AnchorPoint = new PointF (0,0);
			layer.Delegate = new ListBoxElementLayerDelegate (this);			
			
			layer.BorderColor = new CGColor (0, 1, 0, 1);
			layer.BorderWidth = 1;
			
			layer.SetNeedsDisplay ();
			
			return layer;
		}

		public event ListBoxSelectionChanged SelectionChanged;
	}

	public delegate void ListBoxSelectionChanged (int selectedIndex);
	
	class ListBoxElementLayerDelegate : CALayerDelegate {
		ListBoxElement el;
		
		public ListBoxElementLayerDelegate (ListBoxElement el)
		{
			this.el = el;
		}
		
		public override void DrawLayer (CALayer layer, CGContext context)
		{
			if (el.Items != null) {
				int y = el.Bounds.Height - el.Font.LineSize;
				for (int i = el.FirstVisible; i < el.FirstVisible + el.NumVisible; i ++) {
					if (i >= el.Items.Count)
						return;
					GuiUtil.RenderTextToContext (context, new PointF (0, y),
												el.Items[i], el.Font, el.Palette, 4);
#if notyet
												(!el.Selectable ||
									     		(!el.Selecting && el.SelectedIndex == i) ||
									     		(el.Selecting && el.SelectionIndex == i)) ? 4 : 24);
#endif
					y -= el.Font.LineSize;
				}
			}
		}
	}
}

