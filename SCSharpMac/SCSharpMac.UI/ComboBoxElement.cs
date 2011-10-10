//
// SCSharpMac.UI.ComboBoxElement
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
	public class ComboBoxElement : UIElement
	{
		List<string> items;
		int cursor = -1;
		CALayer dropdownLayer;
		int dropdown_hover_index;

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
		
		public int DropdownHoverIndex {
			get { return dropdown_hover_index; }
		}
		
		public string SelectedItem {
			get { return items[cursor]; }
		}
		
		public List<string> Items {
			get { return items; }
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
		}

		public void RemoveAt (int index)
		{
			items.RemoveAt (index);
			if (items.Count == 0)
				cursor = -1;
		}

		public void Clear ()
		{
			items.Clear ();
			cursor = -1;
		}

		public bool Contains (string item)
		{
			return items.Contains (item);
		}

		public override void MouseButtonDown (NSEvent theEvent)
		{
			Console.WriteLine ("Showing dropdown");
			ShowDropdown();
		}

		public override void MouseButtonUp (NSEvent theEvent)
		{
			Console.WriteLine ("Hiding dropdown");
			HideDropdown ();
		}

		public override void PointerMotion (NSEvent theEvent)
		{
			/* if the dropdown is visible, see if we're inside it */
			if (dropdownLayer.Hidden)
				return;
			
			PointF p = new PointF (theEvent.LocationInWindow.Y - dropdownLayer.Position.Y, theEvent.LocationInWindow.Y - dropdownLayer.Position.Y);
			
			Console.WriteLine ("point relative to dropdownlayer = {0}", p);
			if (/*
			      starcraft doesn't include this check..  should we?
			      p.X >= 0 && p.X < dropdownLayer.Bounds.Width &&
			    */
			    p.Y >= 0 && p.Y < dropdownLayer.Bounds.Height) {

				int new_hover_index = (int)((dropdownLayer.Bounds.Height - p.Y) / Font.LineSize);

				Console.WriteLine ("new_hover_item = {0}", new_hover_index);
				
				if (dropdown_hover_index != new_hover_index) {
					dropdown_hover_index = new_hover_index;
					dropdownLayer.SetNeedsDisplay ();
				}
			}
		}

		void ShowDropdown ()
		{
			dropdown_hover_index = cursor;
			if (dropdownLayer == null)
				CreateDropdownLayer ();
			dropdownLayer.Hidden = false;
			dropdownLayer.AnchorPoint = new PointF (0, 0);
			dropdownLayer.Position = new PointF (X1, Layer.Position.Y - dropdownLayer.Bounds.Height);
		}

		void HideDropdown ()
		{
			if (dropdownLayer == null)
				CreateDropdownLayer ();
			dropdownLayer.Hidden = true;
			if (cursor != dropdown_hover_index) {
				cursor = dropdown_hover_index;
				Invalidate ();
				if (SelectionChanged != null)
					SelectionChanged (cursor);
			}
		}

		protected override CALayer CreateLayer ()
		{
			CALayer layer = CALayer.Create ();
			layer.Bounds = new RectangleF (0, 0, Width, Height);
			
			layer.Delegate = new ComboBoxElementLayerDelegate (this);
			
			layer.BorderWidth = 1;
			layer.BorderColor = new CGColor (1, 0, 0, 1);
			
			return layer;
		}

		void CreateDropdownLayer ()
		{
			dropdownLayer = CALayer.Create ();
			dropdownLayer.Bounds = new RectangleF (0, 0, Width, items.Count * Font.LineSize);
			
			dropdownLayer.Delegate = new ComboBoxElementDropdownLayerDelegate (this);
			
			dropdownLayer.BackgroundColor = new CGColor (0, 0, 0, 1);
			dropdownLayer.BorderWidth = 1;
			dropdownLayer.BorderColor = new CGColor (1,1,0, 1);
			dropdownLayer.SetNeedsDisplay ();
			ParentScreen.AddSublayer (dropdownLayer);
		}

		public event ComboBoxSelectionChanged SelectionChanged;
	}

	public delegate void ComboBoxSelectionChanged (int selectedIndex);
	
	class ComboBoxElementDropdownLayerDelegate : CALayerDelegate {
		ComboBoxElement el;
		
		public ComboBoxElementDropdownLayerDelegate (ComboBoxElement el)
		{
			this.el = el;
		}
		
		public override void DrawLayer (CALayer layer, CGContext context)
		{
			int y = 0;
			for (int i = el.Items.Count - 1; i >=0 ; i --) {
				GuiUtil.RenderTextToContext (context, new PointF (0, y), el.Items[i], el.Font, el.Palette,
											 i == el.DropdownHoverIndex ? 4 : 24);
				
				y += (int)el.Font.LineSize;
			}
			
		}
		
	}
	
	class ComboBoxElementLayerDelegate : CALayerDelegate {
		ComboBoxElement el;
		
		public ComboBoxElementLayerDelegate (ComboBoxElement el)
		{
			this.el = el;
		}
		
		public override void DrawLayer (CALayer layer, CGContext context)
		{
			/* XXX draw the arrow (and border) */

			if (el.SelectedIndex != -1) {
				GuiUtil.RenderTextToContext (context, new PointF (0, 0), el.SelectedItem, el.Font, el.Palette, 4);
			}
			
		}
	}
}

