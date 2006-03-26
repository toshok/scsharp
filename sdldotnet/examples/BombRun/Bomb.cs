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
	/// 
	/// </summary>
	public class Bomb : Sprite
	{
		int speed;
		static float maxspeed = 250;
		static Random random = new Random();

		/// <summary>
		/// 
		/// </summary>
		public Bomb(Surface bombSurface) : base(bombSurface)
		{
			base.Surface.TransparentColor = Color.White;
			base.Size = this.Surface.Size;
			Reset();
		}

		private void Reset()
		{
			this.X = random.Next(Video.Screen.Width - this.Surface.Width);
			this.Y = this.Surface.Height - random.Next(Video.Screen.Height);
			this.speed = 
				random.Next((int)BombRun.BombSpeed,
				(int)BombRun.BombSpeed * 2);
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
			this.Y += (int)(args.SecondsElapsed * speed);
			//Console.WriteLine(args.SecondsElapsed);

			if(this.Y > Video.Screen.Height)
			{
				Reset();
			}

			if(BombRun.BombSpeed > maxspeed)
			{
				BombRun.BombSpeed = maxspeed / 2;
				maxspeed = maxspeed * 2;
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