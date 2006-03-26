/*
 * $RCSfile: BoundedSprite.cs,v $
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

using SdlDotNet;
using SdlDotNet.Sprites;
using System;
using System.Drawing;

namespace SdlDotNet.Examples.SpriteGuiDemos
{
	/// <summary>
	/// 
	/// </summary>
	public class BoundedSprite : AnimatedDemoSprite
	{
		private Rectangle bounds = new Rectangle();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="surfaces"></param>
		/// <param name="bounds"></param>
		/// <param name="coordinates"></param>
		public BoundedSprite(SurfaceCollection surfaces, Rectangle bounds, Point coordinates)
			: base(surfaces, coordinates)
		{
			if (surfaces == null)
			{
				throw new ArgumentNullException("surfaces");
			}
			this.bounds = bounds;
			int tempHeight;
			int tempWidth;
			tempWidth = this.bounds.Size.Width - (int) surfaces.Size.Width;
			tempHeight = this.bounds.Size.Height - (int) surfaces.Size.Height;
			this.bounds.Size = new Size(tempWidth, tempHeight);
		}

		/// <summary>
		/// 
		/// </summary>
		public Rectangle SpriteBounds
		{
			get 
			{ 
				return bounds; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void Update(TickEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			// Animate
			base.Update(args);

			// Bounce off the left
			if (this.X < bounds.Left)
			{
				this.X = bounds.Left;
			}

			// Bounce off the top
			if (this.Y < bounds.Top)
			{
				this.Y = bounds.Top;
			}

			// Bounce off the bottom
			if (this.Y > bounds.Bottom)
			{
				this.Y = bounds.Bottom;
			}
			// Bounce off the right
			if (this.X > bounds.Right)
			{
				this.X = bounds.Right;
			}
		}

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
