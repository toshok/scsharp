/*
 * $RCSfile: Sprite.cs,v $
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
using System.Collections;
using System.Globalization;

using SdlDotNet;

namespace SdlDotNet.Sprites
{
	/// <summary>
	/// Sprite class contains both a Surface and a Rectangle so that 
	/// an object can be easily displayed and manipulated.
	/// </summary>
	public class Sprite : IDisposable
	{
		/// <summary>
		/// Basic constructor. 
		/// </summary>
		/// <remarks>
		/// Use this with caution. 
		/// This is provided as a convenience. 
		/// Please give the spirte a Surface and a rectangle.</remarks>
		public Sprite()
		{
		}

		/// <summary>
		/// Create a new Sprite
		/// </summary>
		/// <param name="position">Starting position</param>
		/// <param name="surface">Surface of Sprite</param>
		public Sprite(Surface surface, Point position) : 
			this(surface)
		{
			this.rect = new Rectangle(position.X, position.Y, surface.Width, surface.Height);
		}

		/// <summary>
		/// Create new Sprite at (0, 0)
		/// </summary>
		/// <param name="surface">Surface of Sprite</param>
		public Sprite(Surface surface)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			this.rect = new Rectangle(0, 0, surface.Width, surface.Height);
			this.surf = surface;
		}

		/// <summary>
		/// Creates a new sprite using the given surface file.
		/// </summary>
		/// <param name="surfaceFile">
		/// The file path of the surface to use as the sprite.
		/// </param>
		public Sprite(string surfaceFile) : this(new Surface(surfaceFile))
		{
		}

		/// <summary>
		/// Create new Sprite
		/// </summary>
		/// <param name="position">Position of Sprite</param>
		/// <param name="positionZ">Z coordinate of Sprite</param>
		/// <param name="surface">Surface of Sprite</param>
		public Sprite(Surface surface, Point position, int positionZ) : 
			this(surface, position)
		{
			this.coordinateZ = positionZ;
		}

		/// <summary>
		/// Create new Sprite
		/// </summary>
		/// <param name="surface">
		/// Surface of Sprite
		/// </param>
		/// <param name="rectangle">
		/// Rectangle of sprite indicating position and size.
		/// </param>
		public Sprite(Surface surface, Rectangle rectangle)
		{
			this.surf = surface;
			this.rect = rectangle;
		}

		/// <summary>
		/// Create new Sprite
		/// </summary>
		/// <param name="surface">Surface of Sprite</param>
		/// <param name="rectangle">
		/// Rectangle of sprite indicating position and size.
		/// </param>
		/// <param name="positionZ">Z coordinate of Sprite</param>
		public Sprite(Surface surface, Rectangle rectangle, int positionZ): 
			this(surface, rectangle)
		{
			this.coordinateZ = positionZ;
		}

		//		/// <summary>
		//		/// 
		//		/// </summary>
		//		/// <param name="coordinates"></param>
		//		/// <param name="surface"></param>
		//		/// <param name="group"></param>
		//		public Sprite(Surface surface, Vector coordinates, SpriteCollection group) : 
		//			this(surface, coordinates)
		//		{
		//			this.AddInternal(group);
		//		}

		/// <summary>
		/// Create new sprite
		/// </summary>
		/// <param name="position">position of Sprite</param>
		/// <param name="positionZ">Z coordinate of Sprite</param>
		/// <param name="surface">Surface of Sprite</param>
		/// <param name="group">
		/// SpriteCollection group to put Sprite into.
		/// </param>
		public Sprite(Surface surface, Point position, int positionZ, SpriteCollection group): 
			this(surface, position, positionZ)
		{
			this.AddInternal(group);
		}

		/// <summary>
		/// Create new sprite
		/// </summary>
		/// <param name="position">position of Sprite</param>
		/// <param name="surface">Surface of Sprite</param>
		/// <param name="group">
		/// SpriteCollection group to put Sprite into.
		/// </param>
		public Sprite(Surface surface, Point position , SpriteCollection group):
			this(surface, position)
		{
			this.AddInternal(group);
		}

		#region Display
		private Surface surf;
		/// <summary>
		/// Gets and sets the surface of the sprite.
		/// </summary>
		public virtual Surface Surface
		{
			get
			{
				return surf;
			}
			set
			{
				surf = value;
			}
		}

		/// <summary>
		/// Returns a surface of the rendered sprite.
		/// </summary>
		public virtual Surface Render()
		{
			return this.surf;
		}

		/// <summary>
		/// Renders the sprite onto the destination surface.
		/// </summary>
		/// <param name="destination">
		/// The surface to be rendered onto.
		/// </param>
		/// <returns>
		/// Actual rectangle blit to by method. Sometime clipping may occur.
		/// </returns>
		public virtual Rectangle Render(Surface destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			return destination.Blit(this);
		}
		#endregion

		#region Events
		/// <summary>
		/// Processes Active events
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(ActiveEventArgs args)
		{
		}
		/// <summary>
		/// Processes the keyboard.
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(KeyboardEventArgs args)
		{
		}

		/// <summary>
		/// Processes a mouse button. This event is trigger by the SDL
		/// system. 
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(MouseButtonEventArgs args)
		{
		}

		/// <summary>
		/// Processes a mouse motion event. This event is triggered by
		/// SDL. Only
		/// sprites that are MouseSensitive are processed.
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(MouseMotionEventArgs args)
		{
		}
		
		/// <summary>
		/// Processes a joystick motion event. This event is triggered by
		/// SDL. Only
		/// sprites that are JoystickSensitive are processed.
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(JoystickAxisEventArgs args)
		{
		}
		
		/// <summary>
		/// Processes a joystick button event. This event is triggered by
		/// SDL. Only
		/// sprites that are JoystickSensitive are processed.
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(JoystickButtonEventArgs args)
		{
		}
		
		/// <summary>
		/// Processes a joystick hat motion event. This event is triggered by
		/// SDL. Only
		/// sprites that are JoystickSensitive are processed.
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(JoystickHatEventArgs args)
		{
		}
		
		/// <summary>
		/// Processes a joystick hat motion event. This event is triggered by
		/// SDL. Only
		/// sprites that are JoystickSensitive are processed.
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(JoystickBallEventArgs args)
		{
		}

		/// <summary>
		/// Processes Quit Events
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(QuitEventArgs args)
		{
		}
		/// <summary>
		/// Process User Events
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(UserEventArgs args)
		{
		}

		/// <summary>
		/// Process VideoExposeEvents
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(VideoExposeEventArgs args)
		{
		}

		/// <summary>
		/// Process VideoResizeEvents
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(VideoResizeEventArgs args)
		{
		}

		/// <summary>
		/// Process ChannelFinishedEvents
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(ChannelFinishedEventArgs args)
		{
		}

		/// <summary>
		/// Process MusicFinishedEvents
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(MusicFinishedEventArgs args)
		{
		}
		
		/// <summary>
		/// All sprites are tickable, regardless if they actual do
		/// anything. This ensures that the functionality is there, to be
		/// overridden as needed.
		/// </summary>
		/// <param name="args">Event args</param>
		public virtual void Update(TickEventArgs args)
		{
		}
		#endregion

		#region Geometry

		private Rectangle rect;
		/// <summary>
		/// Gets and sets the sprite's surface rectangle.
		/// </summary>
		public Rectangle Rectangle
		{
			get 
			{ 
				if (rect.IsEmpty)
				{
					this.rect = this.Surface.Rectangle;
				}
				return this.rect;
			}
			set
			{
				this.rect = value;
			}
		}

		private Rectangle rectDirty;
		/// <summary>
		/// Rectangles that have changed and need to be updated
		/// </summary>
		public Rectangle RectangleDirty
		{
			get 
			{ 
				return this.rectDirty;
			}
			set
			{
				this.rectDirty = value;
			}
		}

		/// <summary>
		/// Gets and sets the sprites current x,y location.
		/// </summary>
		public Point Position
		{
			get 
			{ 
				return new Point(rect.X, rect.Y); 
			}
			set
			{
				rect.X = value.X;
				rect.Y = value.Y;
			}
		}

		/// <summary>
		/// Center point of Sprite
		/// </summary>
		public Point Center
		{
			get
			{
				return new Point(((this.X) + (this.Width)/2),
					((this.Y) + (this.Height)/2));
			}
			set
			{
				this.X = (value.X - this.Width/2);
				this.Y = (value.Y - this.Height/2);
			}
		}

		private int coordinateZ;

		/// <summary>
		/// Gets and sets the sprite's x location.
		/// </summary>
		public int X
		{
			get
			{
				return this.rect.X;
			}
			set
			{
				this.rect.X = value;
			}
		}

		/// <summary>
		/// Gets and sets the sprite's y location.
		/// </summary>
		public int Y
		{
			get
			{
				return this.rect.Y;
			}
			set
			{
				this.rect.Y = value;
			}
		}

		/// <summary>
		/// Gets and sets the sprite's z coordinate.
		/// </summary>
		public int Z
		{
			get
			{
				return this.coordinateZ;
			}
			set
			{
				this.coordinateZ = value;
			}
		}

		/// <summary>
		/// Gets the left edge of the sprite.
		/// </summary>
		public int Left
		{
			get
			{
				return this.X;
			}
		}

		/// <summary>
		/// Gets the right edge of the sprite.
		/// </summary>
		public int Right
		{
			get
			{
				return this.X + this.Width;
			}
		}

		/// <summary>
		/// Gets the top edge of the sprite.
		/// </summary>
		public int Top
		{
			get
			{
				return this.Y;
			}
		}

		/// <summary>
		/// Gets the bottom edge of the sprite.
		/// </summary>
		public int Bottom
		{
			get
			{
				return this.Y + this.Height;
			}
		}

		/// <summary>
		/// Gets and sets the sprite's size.
		/// </summary>
		public Size Size
		{
			get 
			{ 
				return new Size(rect.Width, rect.Height);
			}
			set
			{
				rect.Width = value.Width;
				rect.Height = value.Height;
			}
		}

		/// <summary>
		/// Gets and sets the sprite's height.
		/// </summary>
		public virtual int Height
		{
			get
			{
				return this.rect.Height;
			}
			set
			{
				this.rect.Height = value;
			}
		}

		/// <summary>
		/// Gets and sets the sprite's width.
		/// </summary>
		public virtual int Width
		{
			get
			{
				return this.rect.Width;
			}
			set
			{
				this.rect.Width = value;
			}
		}

		/// <summary>
		/// Sprite has changed and need to be redisplayed
		/// </summary>
		public bool Dirty
		{
			get
			{
				return (!this.rect.Equals(this.rectDirty));
			}
		}

		#region IntersectsWith
		/// <summary>
		/// Checks if Sprite intersects with a point
		/// </summary>
		/// <param name="point">Point to intersect with</param>
		/// <returns>True if Sprite intersects with the Point</returns>
		public virtual bool IntersectsWith(Point point)
		{
			return this.rect.IntersectsWith(new Rectangle(point, new Size(0, 0)));
		}

		/// <summary>
		/// Checks if Sprite intersects with a rectangle
		/// </summary>
		/// <param name="rectangle">rectangle to intersect with
		/// </param>
		/// <returns>True if Sprite intersect with Rectangle</returns>
		public virtual bool IntersectsWith(Rectangle rectangle)
		{
			return this.rect.IntersectsWith(rectangle);
		}

		/// <summary>
		/// Check if two Sprites intersect
		/// </summary>
		/// <param name="sprite">Sprite to check intersection with</param>
		/// <returns>True if sprites intersect</returns>
		public virtual bool IntersectsWith(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			return this.IntersectsWith(sprite.Rectangle);
		}

		/// <summary>
		/// Checks if Sprite intersects with a rectangle with tolerance
		/// </summary>
		/// <param name="rect">rectangle to intersect with
		/// </param>
		/// <param name="tolerance">The tolerance of the collision check</param>
		/// <returns>True if Sprite intersect with Rectangle</returns>
		public virtual bool IntersectsWith(Rectangle rect, int tolerance)
		{
			if(rect.Right - this.Left < tolerance)	return false;
			if(rect.X - this.Right > -tolerance)	return false;
			if(rect.Bottom - this.Y < tolerance)	return false;
			if(rect.Y - this.Bottom > -tolerance)	return false;
			return true;
		}

		/// <summary>
		/// Check if two Sprites intersect
		/// </summary>
		/// <param name="sprite">Sprite to check intersection with</param>
		/// <param name="tolerance">The amount of tolerance to give the collision.</param>
		/// <returns>True if sprites intersect</returns>
		public virtual bool IntersectsWith(Sprite sprite, int tolerance)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			if(sprite.Right - this.Left < tolerance)	return false;
			if(sprite.X - this.Right > -tolerance)		return false;
			if(sprite.Bottom - this.Y < tolerance)		return false;
			if(sprite.Y - this.Bottom > -tolerance)		return false;
			return true;
		}

		/// <summary>
		/// Checks for collision between two sprites using a radius from the center of the sprites.
		/// </summary>
		/// <param name="sprite">The sprite to compare to.</param>
		/// <param name="radius">The radius of the current sprite. Defaults to the radius of the sprite.</param>
		/// <param name="radiusOther">The other sprite's radius. Defaults to the radius of the sprite.</param>
		/// <param name="tolerance">The size of the buffer zone for collision detection. Defaults to 0.</param>
		/// <returns>True if they intersect, false if they don't.</returns>
		/// <remarks>If they radius is not given, it calculates it for you using half the width plus half the height.</remarks>
		public virtual bool IntersectsWithRadius(Sprite sprite, int radius, int radiusOther, int tolerance)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			Point center1 = this.Center;
			Point center2 = sprite.Center;
			int xdiff = center2.X - center1.X;	// x plane difference
			int ydiff = center2.Y - center1.Y;	// y plane difference
	
			// distance between the circles centres squared
			int dcentre_sq = (ydiff*ydiff) + (xdiff*xdiff);
	
			// calculate sum of radiuses squared
			int r_sum_sq = radius + radiusOther;
			r_sum_sq *= r_sum_sq;

			return (dcentre_sq - r_sum_sq <= (tolerance * tolerance));
		}

		/// <summary>
		/// Checks for collision between two sprites using a radius from the center of the sprites.
		/// </summary>
		/// <param name="sprite">The sprite to compare to.</param>
		/// <param name="radius">The radius of the current sprite. Defaults to the radius of the sprite.</param>
		/// <param name="radiusOther">The other sprite's radius. Defaults to the radius of the sprite.</param>
		/// <returns>True if they intersect, false if they don't.</returns>
		/// <remarks>The offset defaults to 0.</remarks>
		public virtual bool IntersectsWithRadius(Sprite sprite, int radius, int radiusOther)
		{
			return IntersectsWithRadius(sprite, radius, radiusOther, 0);
		}

		/// <summary>
		/// Checks for collision between two sprites using a radius from the center of the sprites.
		/// </summary>
		/// <param name="sprite">The sprite to compare to.</param>
		/// <param name="radius">The radius of the sprites.</param>
		/// <returns>True if they intersect, false if they don't.</returns>
		public virtual bool IntersectsWithRadius(Sprite sprite, int radius)
		{
			return IntersectsWithRadius(sprite, radius, radius, 0);
		}

		/// <summary>
		/// Checks for collision between two sprites using a radius from the center of the sprites.
		/// </summary>
		/// <param name="sprite">The sprite to compare to.</param>
		/// <returns>True if they intersect, false if they don't.</returns>
		/// <remarks>The radius for both the sprites is calculated by using half the width and half the height.</remarks>
		public virtual bool IntersectsWithRadius(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			int r1 = (this.Width + this.Height) / 4;
			int r2 = (sprite.Width + sprite.Height) / 4;
			return IntersectsWithRadius(sprite, r1, r2, 0);
		}

		/// <summary>
		/// Check to see if Sprite intersects with any sprite in a SpriteCollection
		/// </summary>
		/// <param name="spriteCollection">Collection to chekc the intersection with</param>
		/// <returns>True if sprite intersects with any sprite in collection</returns>
		public virtual bool IntersectsWith(SpriteCollection spriteCollection)
		{
			if (spriteCollection == null)
			{
				throw new ArgumentNullException("spriteCollection");
			}
			foreach(Sprite sprite in spriteCollection)
			{
				if(this.IntersectsWith(sprite))
				{
					return true;
				}
			}
			return false;
		}
		#endregion IntersectsWith
		#endregion

		#region Properties
		/// <summary>
		/// True if Sprite is a member of a SpriteCollection
		/// </summary>
		public virtual bool Alive
		{
			get
			{
				return (groups.Count > 0);
			}
		}

		ArrayList groups = new ArrayList();
		/// <summary>
		/// collections that the Sprite is a member of.
		/// </summary>
		public virtual ArrayList Collections
		{
			get
			{
				return groups;
			}
		}

		/// <summary>
		/// remove sprite from all collections
		/// </summary>
		public virtual void Kill()
		{
			for (int i = 0; i < this.groups.Count; i++)
			{
				((SpriteCollection)groups[i]).Remove(this);
			}
		}

		/// <summary>
		/// Add Sprite to collection
		/// </summary>
		/// <param name="group">collection to add to</param>
		public virtual void Add(SpriteCollection group)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			this.groups.Add(group);
			group.AddInternal(this);
		}

		/// <summary>
		/// Add Sprite to collection. Use in special situations
		/// </summary>
		/// <param name="group">Collection to add to</param>
		public void AddInternal(SpriteCollection group)
		{
			this.groups.Add(group);
		}

		/// <summary>
		/// remove Sprite from Collection
		/// </summary>
		/// <param name="group">collection to remove sprite from</param>
		public virtual void Remove(SpriteCollection group)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			this.groups.Remove(group);
			group.RemoveInternal(this);
		}

		/// <summary>
		/// remove Sprite from collection. Use in special situations
		/// </summary>
		/// <param name="group">collection to remove sprite from</param>
		public void RemoveInternal(SpriteCollection group)
		{
			this.groups.Remove(group);
		}

		private bool allowDrag;
		/// <summary>
		/// Allows sprite to be dragged via the mouse
		/// </summary>
		public bool AllowDrag
		{
			get
			{
				return allowDrag;
			}
			set
			{
				allowDrag = value;
			}
		}

		private bool beingDragged;

		/// <summary>
		/// true when sprite is being dragged by the mouse
		/// </summary>
		public bool BeingDragged
		{
			get
			{
				return beingDragged;
			}
			set
			{
				beingDragged = value;
			}
		}

		private bool visible = true;

		/// <summary>
		/// Gets and sets whether or not the sprite is visible when rendered.
		/// </summary>
		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
			}
		}

		/// <summary>
		/// Gets and sets the alpha associated with the sprite's surface.
		/// </summary>
		public virtual byte Alpha
		{
			get
			{
				return surf.Alpha;
			}
			set
			{
				surf.Alpha = value;
			}
		}

		/// <summary>
		/// Gets and sets the transparent color associated with the sprite's surface.
		/// </summary>
		public virtual Color TransparentColor
		{
			get
			{
				return surf.TransparentColor;
			}
			set
			{
				surf.TransparentColor = value;
			}
		}
		#endregion

		#region IDisposable Members

		private bool disposed;

		/// <summary>
		/// Destroy sprite
		/// </summary>
		/// <param name="disposing">If true, remove all unamanged resources</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.surf != null)
					{
						this.surf.Dispose();
						this.surf = null;
					}
					this.Kill();
				}
				this.disposed = true;
			}
		}
		/// <summary>
		/// Destroy object
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Destroy object
		/// </summary>
		public void Close() 
		{
			Dispose();
		}

		/// <summary>
		/// Destroy object
		/// </summary>
		~Sprite() 
		{
			Dispose(false);
		}

		#endregion
	}
}
