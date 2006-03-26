/*
 * $RCSfile$
 * Copyright (C) 2004 D. R. E. Moonfire (d.moonfire@mfgames.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using SdlDotNet.Sprites;
using SdlDotNet;
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;

namespace SdlDotNet.Examples.GuiExample
{
	/// <summary>
	/// 
	/// </summary>
	public class GuiMenuPopup : VerticalPacker
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="manager"></param>
		public GuiMenuPopup(GuiManager manager)
			: base(manager, 12000)
		{
			this.Visible = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "(menu {0})", base.ToString());
		}

		#region Drawing
		/// <summary>
		/// 
		/// </summary>
		public void ShowMenu()
		{
			this.Visible = true;
			this.Render();
		}

		/// <summary>
		/// 
		/// </summary>
		public void HideMenu()
		{
			this.Visible = false;
		}
		#endregion

		#region Sprites
		/// <summary>
		/// 
		/// </summary>
		/// <param name="menuItem"></param>
		public void Add(Sprite menuItem)
		{
			AddHead(menuItem);
			Redo();
		}

		private void Redo()
		{
			int height = 0;
			int width = 0;
			for (int i = 0; i < this.Sprites.Count; i++)
			{
				this.Sprites[i].X = this.menuTitle.X;
				this.Sprites[i].Y = this.menuTitle.Y + this.Sprites[i].Height * i;
				height += this.Sprites[i].Height;
				if (this.Sprites[i].Width > width)
				{
					width = this.Sprites[i].Width;
				}
			}
			this.Surface = new Surface(width,height);
			this.Rectangle = new Rectangle(this.menuTitle.X, this.menuTitle.Y + this.menuTitle.Height, width, height);
			base.Render();
		}
		#endregion

		#region Events
		/// <summary>
		/// Menus are a special case of a sprite. If the mouse is
		/// selected, then it shows the entire sprite, regardless of the
		/// packing size.
		/// </summary>
		public override void Update(MouseButtonEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (this.Visible)
			{
				// If we are being held down, pick up the marble
				if (args.ButtonPressed)
				{
				}
				else
				{
					// Check for an item
					if (selected != null)
					{
						selected.OnMenuSelected(selectedIndex);
						selected.IsSelected = false;
						selected = null;
					}
					// Remove the menu
					HideMenu();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override Surface Render()
		{
			Redo();
			return this.Surface;
		}

		/// <summary>
		/// Uses the mouse motion to determine what menu item is actually
		/// selected and hilight it. If the menu is not selected, it does
		/// nothing.
		/// </summary>
		public override void Update(MouseMotionEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			// Retrieve the item for these coordinates
			int ndx = 0;
			GuiMenuItem gmi = 
				(GuiMenuItem) SelectSprite(new Point(args.X - this.X, args.Y- this.Y), ref ndx);

			// Check to see if we need to deselect an old one
			if (selected != null && (gmi == null || gmi != selected))
			{
				// Clear out selected
				selected.IsSelected = false;
				selected = null;
			}

			// If we have a menu, select it
			if (gmi != null)
			{
				gmi.IsSelected = true;
				selected = gmi;
				selectedIndex = ndx;
			}
		}
		#endregion

		#region Properties
		private GuiMenuItem selected;
		private int selectedIndex;

		private GuiMenuTitle menuTitle;
		/// <summary>
		/// 
		/// </summary>
		public GuiMenuTitle MenuTitle
		{
			get
			{
				return menuTitle;
			}
			set 
			{
				menuTitle = value;
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.Position = menuTitle.Position;
			}
		}


		#endregion
		private bool disposed;
		/// <summary>
		/// Destroys the surface object and frees its memory
		/// </summary>
		/// <param name="disposing">If ture, dispose unmanaged resources</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.disposed)
				{
					if (disposing)
					{
						this.menuTitle.Dispose();
						this.selected.Dispose();
					}
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
	}
}
