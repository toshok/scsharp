/*
 * $RCSfile: MultipleMode.cs,v $
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
using System.Drawing;
using System;

namespace SdlDotNet.Examples.SpriteGuiDemos
{
	/// <summary>
	/// 
	/// </summary>
	public class MultipleMode : DemoMode
	{
		private Sprite sprite1;
		private Sprite sprite2;
		private Sprite sprite3;
		private Sprite sprite4;
		static Random rand = new Random();

		private Size size;

		private SpriteCollection all = new SpriteCollection();

		Rectangle rect;

		/// <summary>
		/// Constructs the internal sprites needed for our demo.
		/// </summary>
		public MultipleMode()
		{
			
			// Create the fragment marbles
			SurfaceCollection td = LoadMarble("marble1");
			SurfaceCollection td2 = LoadMarble("marble2");
			SurfaceCollection td3 = LoadMarble("marble3");
			SurfaceCollection td4 = LoadMarble("marble4");
			SurfaceCollection td5 = LoadMarble("marble5");

			// Load the floor
			SurfaceCollection floorTiles = LoadFloor();

			// Place the floors
			int rows = 10;
			int cols = 13;
			size = new Size(floorTiles[0].Size.Width * cols,
				floorTiles[0].Size.Height * rows);
			rect = new Rectangle(new Point(0, 0), size);
			Console.WriteLine("MultiViewPort Size: " + size);

			for (int i = 0; i < cols; i++)
			{
				for (int j = 0; j < rows; j++)
				{
					// Create the sprite
					AnimatedSprite dw =
						new AnimatedSprite(floorTiles,
						new Point(i * floorTiles[0].Size.Width,
						j * floorTiles[0].Size.Height));
					all.Add(dw);
				}
			}

			// Load the bouncing sprites
			for (int i = 0; i < 15; i++)
			{
				BounceSprite d = 
					new BounceSprite(td,
					rect, 
					new Point(rand.Next(rect.Left, rect.Right - 
					(int) td.Size.Width),
					rand.Next(rect.Top, rect.Bottom - 
					(int) td.Size.Height)));
				all.Add(d);
			}

			// Only one container may be tickable when they all talk to the
			// same inner tick manager.

			// Set up container #1
			sprite1 = new BounceSprite(td2, rect, new Point(rand.Next(rect.Left, rect.Right - 
				(int) td2.Size.Width),
				rand.Next(rect.Top, rect.Bottom - 
				(int) td2.Size.Height)));
			all.Add(sprite1);

			// Set up container #2
			sprite2 = new BounceSprite(td3, rect, new Point(rand.Next(rect.Left, rect.Right - 
				(int) td3.Size.Width),
				rand.Next(rect.Top, rect.Bottom - 
				(int) td3.Size.Height)));
			all.Add(sprite2);

			// Set up container #3
			sprite3 = new BounceSprite(td4, rect, new Point(rand.Next(rect.Left, rect.Right - 
				(int) td4.Size.Width),
				rand.Next(rect.Top, rect.Bottom - 
				(int) td4.Size.Height)));
			all.Add(sprite3);
      
			// Set up container #4
			sprite4 = new BounceSprite(td5, rect, new Point(rand.Next(rect.Left, rect.Right - 
				(int) td5.Size.Width),
				rand.Next(rect.Top, rect.Bottom - 
				(int) td5.Size.Height)));
			all.Add(sprite4);

			all.EnableTickEvent();
			surf1 = this.Surface.CreateCompatibleSurface(380, 250);
			surf2 = this.Surface.CreateCompatibleSurface(380, 250);
			surf3 = this.Surface.CreateCompatibleSurface(380, 250);
			surf4 = this.Surface.CreateCompatibleSurface(380, 250);
		}

		Surface surf1;
		Surface surf2;
		Surface surf3;
		Surface surf4;

		/// <summary>
		/// 
		/// </summary>
		public override Surface RenderSurface()
		{	
			this.Surface.Fill(Color.Black);
			foreach (Sprite s in all)
			{
				Rectangle offsetRect = s.Rectangle;
				offsetRect.Offset(AdjustBoundedViewport(sprite1, surf1));
				surf1.Blit(s, offsetRect);
				offsetRect = s.Rectangle;
				offsetRect.Offset(AdjustBoundedViewport(sprite2, surf2));
				surf2.Blit(s, offsetRect);
				offsetRect = s.Rectangle;
				offsetRect.Offset(AdjustBoundedViewport(sprite3, surf3));
				surf3.Blit(s, offsetRect);
				offsetRect = s.Rectangle;
				offsetRect.Offset(AdjustBoundedViewport(sprite4, surf4));
				surf4.Blit(s, offsetRect);
			}
			this.Surface.Blit(surf1, new Point(10, 10));
			this.Surface.Blit(surf2, new Point(410, 10));
			this.Surface.Blit(surf3, new Point(10, 280));
			this.Surface.Blit(surf4, new Point(410, 280));	
		
			return this.Surface;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static Point AdjustViewport(Sprite sprite, Surface surf)
		{
			if (surf == null)
			{
				throw new ArgumentNullException("surf");
			}
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			return new Point(
				surf.Size.Width / 2 - 
				sprite.Size.Width / 2 - 
				sprite.X,
				surf.Size.Height / 2 - 
				sprite.Size.Height / 2 - 
				sprite.Y);
		}

		/// <summary>
		/// 
		/// </summary>
		public Rectangle ViewRect
		{
			get
			{
				return rect;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Point AdjustBoundedViewport(Sprite sprite, Surface surf)
		{
			if (surf == null)
			{
				throw new ArgumentNullException("surf");
			}
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			Point offset = AdjustViewport(sprite, surf);

			// Check to see if the window is too small
			bool doWidth = true;
			bool doHeight = true;
			
			if (this.ViewRect.Width < surf.Size.Width)
			{
				doWidth = false;
			}
			
			if (this.ViewRect.Height < surf.Size.Height)
			{
				doHeight = false;
			}
			
			if (!doWidth && !doHeight)
			{
				return offset;
			}
			
			// Find out the "half" point for the sprite in the view
			int mx = sprite.X + sprite.Size.Width / 2;
			int my = sprite.Y + sprite.Size.Height / 2;
			
			// Figure out the coordinates
			int x1 = mx - surf.Size.Width / 2;
			int x2 = mx + surf.Size.Width / 2;
			int y1 = my - surf.Size.Height / 2;
			int y2 = my + surf.Size.Height / 2;
			
			// Make sure we don't exceed the bounds
			if (doWidth && x1 < this.ViewRect.Left)
			{
				offset.X -= this.ViewRect.Left - x1;
			}
			
			if (doHeight && y1 < this.ViewRect.Top)
			{
				offset.Y -= this.ViewRect.Top - y1;
			}
			
			if (doWidth && x2 > this.ViewRect.Right)
			{
				offset.X += x2 - this.ViewRect.Right;
			}
			
			if (doHeight && y2 > this.ViewRect.Bottom)
			{
				offset.Y += y2 - this.ViewRect.Bottom;
			}
			return new Point(offset.X, offset.Y);
		}

		/// <summary>
		/// Adds the internal sprite manager to the outer one.
		/// </summary>
		public override void Start(SpriteCollection manager)
		{
			base.Start(manager);
		}

		/// <summary>
		/// Removes the internal manager from the controlling manager.
		/// </summary>
		public override void Stop(SpriteCollection manager)
		{
			base.Stop(manager);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString() 
		{ 
			return "Multiple"; 
		}

		private bool disposed;

		/// <summary>
		/// 
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
						this.Surface.Dispose();
						foreach (Sprite s in this.Sprites)
						{
							IDisposable disposableObj = s as IDisposable;
							if (disposableObj != null)
							{
								disposableObj.Dispose( );
							}
						}
						this.surf1.Dispose();
						this.surf2.Dispose();
						this.surf3.Dispose();
						this.surf4.Dispose();
						this.sprite1.Dispose();
						this.sprite2.Dispose();
						this.sprite3.Dispose();
						this.sprite4.Dispose();
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
	}
}
