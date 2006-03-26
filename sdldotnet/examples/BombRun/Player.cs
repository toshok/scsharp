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
	public class Player : Sprite
	{
		bool left;
		bool right;
		bool jump;
		bool fire;

		// the tick of the last fire action
		int lastfire;

		int jumpstart;
		bool falling;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="location"></param>
		/// <param name="playerSurface"></param>
		public Player(Surface playerSurface, Point location) : 
			base(playerSurface, location)
		{
			jumpstart = location.Y;
			base.Surface.TransparentColor = Color.White;
			base.Size = base.Surface.Size;
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
			float change = 5f;
			float jumpspeed = 10f;

			if (jump || falling)
			{
				change = change / 2;
			}

			if (left)
			{
				this.X -= (int)change;
			}
			if (right)
			{
				this.X += (int)change;
			}

			if (jump)
			{
				if (this.Y < jumpstart - Video.Screen.Height / 3)
				{
					jump = false;
					falling = true;
				}
				else
				{
					this.Y -= (int)(jumpspeed * 1.5f);
				}
			}
			else if (falling)
			{
				this.Y += (int)jumpspeed;

				if(this.Y > jumpstart)
				{
					this.Y = jumpstart;
					falling = false;
				}
			}

			if (this.X < 0)
			{
				this.X = 0;
			}

			if (this.X + this.Surface.Size.Width > Video.Screen.Width)
			{
				this.X = Video.Screen.Width - this.Surface.Width;
			}

			// fire if needed. the 250 stands for the delay between two shots
			if (fire && lastfire + 250 < Timer.TicksElapsed)
			{
				if (WeaponFired != null)
				{
					WeaponFired(this, new FireEventArgs(this.Position));
				}

				// dont forget this
				lastfire = Timer.TicksElapsed;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void Update(KeyboardEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			switch (args.Key)
			{
				// the =Down trick works quite well
				case Key.LeftArrow: 
					left = args.Down; 
					break;

				case Key.RightArrow: 
					right = args.Down; 
					break;

				case Key.UpArrow:
					if (args.Down && !falling)
					{
						jump = true;
					}
					else if (!args.Down)
					{
						jump = false;
						falling = true;
					}
					break;
				case Key.Space: 
					fire = args.Down; 
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public event FireEventHandler WeaponFired;

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