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
using System.Drawing;
using System.Globalization;

namespace SdlDotNet.Examples.GuiExample
{
	/// <summary>
	/// Despite its name, the menubar title is actually the part in the
	/// menubarbar that gives the name.
	/// </summary>
	public class GuiMenuTitle : HorizontalPacker, IMenuPopupController
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="title"></param>
		/// <param name="menuBar"></param>
		public GuiMenuTitle(GuiManager manager, GuiMenuBar menuBar, string title)
			: base(manager)
		{
			this.menubar = menuBar;
			this.popup = new GuiMenuPopup(manager);
			this.popup.MenuTitle = this;
      
			if (manager == null)
			{
				throw new ArgumentNullException("manager");
			}
			TextSprite ts = new TextSprite(title, manager.MenuFont);
			this.Rectangle = ts.Rectangle;
			this.Position = menubar.Position;
			AddHead(ts);
		}

		#region Sprites
		/// <summary>
		/// 
		/// </summary>
		/// <param name="menuItem"></param>
		public void Add(Sprite menuItem)
		{
			this.popup.Add(menuItem);
		}
		#endregion

		#region Events
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void Update(MouseButtonEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (!args.ButtonPressed)
			{
				IsSelected = false;
				popup.HideMenu();
			}
			else
			{
			if (this.IntersectsWith(new Point(args.X - this.menubar.X, args.Y - this.menubar.Y)))
				{
					if (args.ButtonPressed)
					{
						Console.WriteLine("TitleClicked");
						IsSelected = true;
						popup.ShowMenu();
					}
				}
			}
		}
		#endregion

		#region Operators
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "(menu-title {0})", base.ToString());
		}
		#endregion

		#region Properties
		private GuiMenuBar menubar;
		private GuiMenuPopup popup;

		private bool selected;

		/// <summary>
		/// 
		/// </summary>
		public bool IsSelected
		{
			get 
			{ 
				return selected; 
			}
			set 
			{ 
				selected = value; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public GuiMenuBar MenuBar
		{
			get 
			{ 
				return menubar; 
			}
			set 
			{ 
				menubar = value; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public GuiMenuPopup Popup
		{
			get 
			{ 
				return popup; 
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
						this.popup.Dispose();
						this.menubar.Dispose();
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
