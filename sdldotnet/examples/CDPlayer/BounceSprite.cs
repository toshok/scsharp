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

namespace SdlDotNet.Examples.CDPlayer
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
			//Sprites will be bounded by the screen edges minus 
			//their size so they will not go off the screen
			this.bounds = 
				new Rectangle(0, 0, 0, 0);
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

			//Change the sprite coordinates if the sprite is not being dragged
			if (!this.BeingDragged)
			{
				this.X += dx;
				this.Y += dy;

				// Bounce off the left
				if (this.X < bounds.Left)
				{
					dx = (Math.Abs(this.dx));
					this.X = bounds.Left;
				}

				// Bounce off the top
				if (this.Y < bounds.Top)
				{
					dy = (Math.Abs(this.dy));
					this.Y = bounds.Top;
				}

				// Bounce off the bottom
				if ((this.Y + this.Height) > bounds.Bottom)
				{
					dy = -1*(Math.Abs(this.dy));
					this.Y = bounds.Bottom - this.Height;
				}
				// Bounce off the right
				if (this.X + this.Width > bounds.Right)
				{
					dx = -1*(Math.Abs(this.dx));
					this.X = bounds.Right - this.Width;
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
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void Update(VideoResizeEventArgs args)
		{
			if( args == null )
			{
				throw new ArgumentNullException("args");
			}
			this.bounds = new Rectangle(0, 0, args.Width, args.Height);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void Update(KeyboardEventArgs args)
		{
			if( args == null )
			{
				throw new ArgumentNullException("args");
			}
			if (args.Key == SdlDotNet.Key.S && args.Down == true)
			{
				this.BeingDragged = true;
				this.Animate = false;
			}
			else if (args.Key == SdlDotNet.Key.S && args.Down == false)
			{
				this.BeingDragged = false;
				this.Animate = true;
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

		/// <summary>
		/// Boundaries for sprites
		/// </summary>
		public Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
			set
			{
				this.bounds = value;
			}
		}
		#region IDisposable
		private bool disposed;

		/// <summary>
		/// Destroys the surface object and frees its memory
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				try
				{
					if (disposing)
					{
					}
					this.disposed = true;
				}
				finally
				{
					base.Dispose(disposing);
					this.disposed = true;
				}
			}
			base.Dispose(disposing);
		}
		#endregion IDisposable
	}
}
