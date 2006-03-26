/*
 * $RCSfile$
 * Copyright (C) 2005 David Hudson (jendave@yahoo.com)
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

using SdlDotNet;
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;

namespace SdlDotNet.Sprites
{
	/// <summary>
	/// The SpriteCollection is a special case of sprite. It is used to
	/// group other sprites into an easily managed whole. The sprite
	/// manager has no size.
	/// </summary>
	/// <remarks>
	/// This class is useful for GUIs when you want to group sprites into one larger sprite.
	/// </remarks>
	public class SpriteContainer : Sprite
	{
//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="coordinates"></param>
//		/// <param name="surface"></param>
//		public SpriteContainer(Surface surface, Vector coordinates) : 
//			base(surface, coordinates)
//		{
//		}

		/// <summary>
		/// Creates SpriteContainer from a Surface
		/// </summary>
		/// <param name="surface">Main sprite Surface</param>
		public SpriteContainer(Surface surface) : 
			base(surface)
		{
		}

		/// <summary>
		/// Creates SpriteContainer from a Surface
		/// </summary>
		/// <param name="surface">Main sprite surface</param>
		/// <param name="rectangle">Rectangle for spirte</param>
		public SpriteContainer(Surface surface, Rectangle rectangle) : 
			base(surface, rectangle)
		{
		}

		/// <summary>
		/// Creates SpriteContainer from a Surface
		/// </summary>
		/// <param name="surface">Main sprite surface</param>
		/// <param name="rectangle">Rectangle for spirte</param>
		/// <param name="positionZ">Z coordinate of sprite</param>
		public SpriteContainer(Surface surface, Rectangle rectangle, int positionZ) : 
base(surface, rectangle, positionZ)
		{
		}

		private SpriteCollection sprites = new SpriteCollection();
		/// <summary>
		/// Collection of sprites that make up the SpriteContainer
		/// </summary>
		/// <remarks>
		/// These sprites will be blit onto the main Surface of the SpriteContainer.
		/// </remarks>
		public SpriteCollection Sprites
		{
			get
			{
				return sprites;
			}
		}

		private bool disposed;
		/// <summary>
		/// Destroy object
		/// </summary>
		/// <param name="disposing">If true, dispose unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.disposed)
				{
					if (disposing)
					{
						foreach (Sprite s in this.sprites)
						{
							s.Dispose();							
						}
					}
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		/// Displays all spirtes stored in the Collection as one Sprite
		/// </summary>
		/// <returns>Surface that has all sprites blit onto it.</returns>
		public override Surface Render()
		{
			this.sprites.Draw(this.Surface);
			return this.Surface;
		}
	}
}