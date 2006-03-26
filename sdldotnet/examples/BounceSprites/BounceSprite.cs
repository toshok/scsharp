/*
 * $RCSfile: BounceSprite.cs,v $
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

using System;
using System.Drawing;

using SdlDotNet;
using SdlDotNet.Sprites;

namespace SdlDotNet.Examples.BounceSprites
{
	/// <summary>
	/// 
	/// </summary>
	public class BounceSprite : AnimatedSprite
	{
		#region Fields
		//Move sprites 10 pixels per tick
		private int dx = 10;
		private int dy = 10;

		//Sprites will be bounded by the screen edges minus 
		//their size so they will not go off the screen
		private Rectangle bounds = new Rectangle();
		#endregion Fields

		#region Constructor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surfaces"></param>
		/// <param name="coordinates"></param>
		public BounceSprite(SurfaceCollection surfaces, Point coordinates)
			: base(surfaces, coordinates)
		{

			if (surfaces == null)
			{
				throw new ArgumentNullException("surfaces");
			}
			//Sprites will be bounded by the screen edges minus 
			//their size so they will not go off the screen
			this.bounds = 
				new Rectangle(0, 0, Video.Screen.Rectangle.Width - 
				(int) surfaces.Size.Width, Video.Screen.Rectangle.Height - 
				(int) surfaces.Size.Height);
			//The sprite can be dragged
			this.Animate = true;
			this.AllowDrag = true;
		}
		#endregion Constructor

		#region Event Update Methods
		/// <summary>
		/// Every tick will update the animation frame
		/// </summary>
		/// <param name="args"></param>
		public override void Update(TickEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			//Call the base method
			base.Update(args);

			//set the surface to point to the new frame. This creates the animation
			//this.Surface = this.Surfaces[Frame];

			//Change the sprite coordinates if the sprite is not being dragged
			if (!this.BeingDragged)
			{
				//Console.WriteLine("Sprite: " + i++);
				this.X += dx;
				this.Y += dy;

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

				// Revrse the directions when the sprite hits an edge
				if (this.X == bounds.Left)
				{
					dx = (Math.Abs(this.dx));
				}

				if (this.X == bounds.Right)
				{
					dx = -1*(Math.Abs(this.dx));
				}

				if (this.Y == bounds.Top)
				{
					dy = (Math.Abs(this.dy));
				}

				if (this.Y == bounds.Bottom)
				{
					dy = -1*(Math.Abs(this.dy));
				}
			}
		}
		/// <summary>
		/// If the mouse click hits a sprite, 
		/// then the sprite will be marked as 'being dragged'
		/// </summary>
		/// <param name="args"></param>
		public override void Update(MouseButtonEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (this.IntersectsWith(new Point(args.X, args.Y)))
			{
				// If we are being held down, pick up the marble
				if (args.ButtonPressed)
				{
					if (args.Button == MouseButton.PrimaryButton)
					{
						this.BeingDragged = true;
						this.Animate = false;
					}
					else
					{
						this.Kill();
					}
				}
				else
				{
					this.BeingDragged = false;
					this.Animate = true;
				}
			}
		}

		/// <summary>
		/// If the sprite is picked up, this moved the sprite to follow
		/// the mouse.
		/// </summary>
		public override void Update(MouseMotionEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (!AllowDrag)
			{
				return;
			}

			// Move the window as appropriate
			if (this.BeingDragged)
			{
				this.X += args.RelativeX;
				this.Y += args.RelativeY;
			}
		}
		#endregion Event Update Methods

		#region IDisposable
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
		#endregion IDisposable
	}
}