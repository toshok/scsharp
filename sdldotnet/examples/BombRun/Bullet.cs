/* This file is part of BombRun
 * (c) 2003 Sijmen Mulder
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Drawing;

using SdlDotNet;
using SdlDotNet.Sprites;

namespace SdlDotNet.Examples.BombRun
{
	/// <summary>
	/// Bullet fired by Player
	/// </summary>
	public class Bullet : Sprite
	{
		int speedX;
		int speedY;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="location"></param>
		/// <param name="speedX"></param>
		/// <param name="speedY"></param>
		public Bullet(Point location, int speedX, int speedY) : base(Video.Screen.CreateCompatibleSurface(8, 16), location)
		{
			this.speedX = speedX;
			this.speedY = speedY;
			base.Surface.Fill(new Rectangle(new Point(0,0), new Size(8, 16)), Color.DarkBlue);
		}

		/// <summary>
		/// If the bullet goes off the screen, 
		/// it is removed from the collection.
		/// </summary>
		/// <param name="args"></param>
		public override void Update(TickEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			this.X += (int)(args.SecondsElapsed * this.speedX);
			this.Y += (int)(args.SecondsElapsed * this.speedY);

			if (this.X + this.Surface.Size.Width < 0 ||
				this.X > Video.Screen.Width ||
				this.Y + this.Surface.Size.Height < 0 ||
				this.Y > Video.Screen.Height)
			{
				this.Kill();
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