/*
 * $RCSfile: BounceSprite.cs,v $
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
using System.Drawing;
using System;
using System.Threading;

namespace SdlDotNet.Examples.SpriteGuiDemos
{
	/// <summary>
	/// 
	/// </summary>
	public class BounceSprite : BoundedSprite
	{
		static Random rand = new Random();
		private int dx;
		private int dy;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="surfaces"></param>
		/// <param name="rect"></param>
		/// <param name="coordinates"></param>
		public BounceSprite(SurfaceCollection surfaces, Rectangle rect, Point coordinates)
			: base(surfaces, rect, coordinates)
		{
			this.dx = rand.Next(-10, 11);
			this.dy = rand.Next(-10, 11);
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
			this.X += (int) (args.SecondsElapsed * 10 * dx);
			this.Y += (int) (args.SecondsElapsed * 10 * dy);

			// Adjust our entropy
			dx += rand.Next(-5, 6);
			dy += rand.Next(-5, 6);

			// Call the base which also normalizes the bounds
			base.Update(args);

			// Normalize the directions
			if (this.X == SpriteBounds.Left)
			{
				dx = rand.Next(1, 10);
			}

			if (this.X == SpriteBounds.Right)
			{
				dx = ((-1) * rand.Next(1, 10));
			}

			if (this.Y == SpriteBounds.Top)
			{
				dy = rand.Next(1, 10);
			}

			if (this.Y == SpriteBounds.Bottom)
			{
				dy = ((-1) * rand.Next(1, 10));
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
