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

namespace SdlDotNet.Examples.GuiExample
{
	/// <summary>
	/// Class to manager internal sprites, such as window
	/// components. This uses a sprite manager at its core, but does
	/// have some additional functionality.
	/// </summary>
	public class VerticalPacker : Packer
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="manager"></param>
		public VerticalPacker(GuiManager manager)
			: base(manager)
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="point"></param>
		public VerticalPacker(GuiManager manager, Point point)
			: base(manager, point)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="positionZ"></param>
		public VerticalPacker(GuiManager manager, int positionZ)
			: base(manager, new Point(0,0))
		{
			base.Z = positionZ;
		}
		#endregion

		#region Drawing
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override Surface Render()
		{
			this.Surface = base.Render();
			//this.Surface.Fill(this.GuiManager.BackgroundColor);
			// Draw all of our left components
			int y = 0;

			foreach (Sprite s in HeadSprites)
			{
				// Ignore hidden
				if (!s.Visible)
				{
					continue;
				}
	
				// Translate it and blit
				s.Y = y;

				// Update the coordinates for the next one
				y += s.Size.Height + InnerPadding.Vertical;
				//s.X = x;
			}

			// Draw our right components
			y = this.Y + Size.Height - MarginPadding.Bottom;

			foreach (Sprite s in TailSprites)
			{
				// Ignore hidden
				if (!s.Visible)
				{
					continue;
				}
	
				// Translate it and blit
				y -= s.Size.Height + InnerPadding.Vertical;
				s.Y = y;
			}
			this.Sprites.Draw(this.Surface);
			return this.Surface;
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
}
