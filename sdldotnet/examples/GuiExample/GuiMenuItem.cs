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
	/// 
	/// </summary>
	public class GuiMenuItem : HorizontalPacker
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="manager"></param>
		public GuiMenuItem(GuiManager manager)
			: base(manager)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="text"></param>
		public GuiMenuItem(GuiManager manager, string text)
			: base(manager)
		{
			if (manager == null)
			{
				throw new ArgumentNullException("manager");
			}
			TextSprite ts = new TextSprite(text, manager.BaseFont);
			base.Surface = ts.Surface;
			AddLeft(ts);
		}

		#region Sprites
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sprite"></param>
		public void AddLeft(Sprite sprite)
		{
			AddHead(sprite);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sprite"></param>
		public void AddRight(Sprite sprite)
		{
			AddTail(sprite);
		}
		#endregion

		#region Drawing
				/// <summary>
		/// 
		/// </summary>
		public override Surface Render()
		{
			//this.Surface.Fill(this.GuiManager.BackgroundColor);
			// Draw all of our left components
			int x = 0;

			int width = 0;
			foreach (Sprite s in HeadSprites)
			{
				// Ignore hidden
				if (!s.Visible)
				{
					continue;
				}
	
				// Translate it and blit
				s.X = x;

				// Update the coordinates for the next one
				x += s.Size.Width + InnerPadding.Horizontal;
//				if (s.Size.Width > width)
//				{
//					width = s.Size.Width;
//				}
				width = 100;
			}

			this.Surface = new Surface( width, this.HeadSprites[0].Height);

			foreach (Sprite s in TailSprites)
			{
				// Ignore hidden
				if (!s.Visible)
				{
					continue;
				}
	
				// Translate it and blit
				//s.X = this.X + this.Size.Width - MarginPadding.Right /*- s.Size.Width*/ - InnerPadding.Horizontal;
				s.X = this.X + this.Surface.Width - s.Width;
			}
			this.Sprites.Draw(this.Surface);
			return this.Surface;
		}
		#endregion

		#region Events
		/// <summary>
		/// 
		/// </summary>
		public event MenuItemEventHandler ItemSelectedEvent;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		public virtual void OnMenuSelected(int index)
		{
			if (ItemSelectedEvent != null)
			{
				ItemSelectedEvent(this, new MenuItemEventArgs(index));
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
			return String.Format(CultureInfo.CurrentCulture, "(menu-item {0})", base.ToString());
		}
		#endregion

		#region Properties
		private bool isSelected;

		/// <summary>
		/// 
		/// </summary>
		public bool IsSelected
		{
			get 
			{ 
				return isSelected; 
			}
			set 
			{ 
				isSelected = value; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override Padding InnerPadding
		{
			get 
			{ 
				return GuiManager.MenuItemInnerPadding; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override Padding MarginPadding
		{
			get 
			{ 
				return GuiManager.MenuItemPadding; 
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

	/// <summary>
	/// 
	/// </summary>
	public delegate void MenuItemEventHandler(object sender, MenuItemEventArgs e);
}
