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
	/// The SpriteCollection is used to group sprites into an easily managed whole. 
	/// </summary>
	/// <remarks>The sprite manager has no size.</remarks>
	public class SpriteCollection : CollectionBase, ICollection
	{
		#region Constructors
		/// <summary>
		/// Creates a new SpriteCollection without any elements in it.
		/// </summary>
		public SpriteCollection() : base()
		{
		}

		/// <summary>
		/// Creates a new SpriteCollection with one sprite element in it.
		/// </summary>
		/// <param name="sprite">Sprite to add to collection</param>
		public SpriteCollection(Sprite sprite) : base()
		{
			this.AddInternal(sprite);
		}

		/// <summary>
		/// Creates a new SpriteCollection based off a different sprite collection.
		/// </summary>
		/// <param name="spriteCollection">Add Spritecollection to this SpriteCollection</param>
		public SpriteCollection(SpriteCollection spriteCollection) : base()
		{
			this.AddInternal(spriteCollection);
		}


		#endregion

		#region Display
		private RectangleCollection lostRects = new RectangleCollection();
		/// <summary>
		/// Draws all surfaces within the collection on the given destination.
		/// </summary>
		/// <param name="destination">The destination surface.</param>
		public virtual RectangleCollection Draw(Surface destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			RectangleCollection rects = new RectangleCollection();
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Visible)
				{
					rects.Add(destination.Blit(this[i].Render(), this[i].Rectangle));
					if (this[i].Dirty)
					{
						rects.Add(this[i].RectangleDirty);
					}
				}
				this[i].RectangleDirty = Rectangle.Intersect(this[i].Rectangle, destination.Rectangle);
			}
			return rects;
		}

		/// <summary>
		/// Erases SpriteCollection from surface
		/// </summary>
		/// <param name="surface">
		/// Surface to remove the SpriteCollection from</param>
		/// <param name="background">B
		/// ackground to use to paint over Sprites in SpriteCollection
		/// </param>
		public void Erase(Surface surface, Surface background)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			for (int i = 0; i < this.lostRects.Count; i++)
			{
				surface.Blit(background, this.lostRects[i], this.lostRects[i]);
			}
			surface.Update(this.lostRects);

			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Visible)
				{
					surface.Blit(background, this[i].Rectangle, this[i].Rectangle);
				}
			}
			this.lostRects.Clear();
		}

		#endregion

		#region Sprites
		/// <summary>
		/// Adds sprite to group
		/// </summary>
		/// <param name="sprite">Sprite to add</param>
		public virtual int Add(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			sprite.AddInternal(this);
			return (List.Add(sprite));
		}

		/// <summary>
		/// Adds sprite to group
		/// </summary>
		/// <param name="sprite">Sprite to add</param>
		public int AddInternal(Sprite sprite)
		{
			return (List.Add(sprite));
		}

		/// <summary>
		/// Adds sprites from another group to this group
		/// </summary>
		/// <param name="spriteCollection">SpriteCollection to add Sprites from</param>
		public virtual int Add(SpriteCollection spriteCollection)
		{
			if (spriteCollection == null)
			{
				throw new ArgumentNullException("spriteCollection");
			}
			for (int i = 0; i < spriteCollection.Count; i++)
			{
				spriteCollection[i].AddInternal(this);
				this.List.Add(spriteCollection[i]);
			}
			return this.Count;
		}

		private int AddInternal(SpriteCollection spriteCollection)
		{
			if (spriteCollection == null)
			{
				throw new ArgumentNullException("spriteCollection");
			}
			for (int i = 0; i < spriteCollection.Count; i++)
			{
				spriteCollection[i].AddInternal(this);
				List.Add(spriteCollection[i]);
			}
			return this.Count;
		}

		/// <summary>
		/// Rectangles of Sprites that have been removed
		/// </summary>
		/// <remarks>
		/// These Rectangles are kept temprorily until their 
		/// positions can be properly erased.
		/// </remarks>
		protected RectangleCollection LostRects
		{
			get
			{
				return this.lostRects;
			}
		}

		/// <summary>
		/// Removes sprite from group
		/// </summary>
		/// <param name="sprite">Sprite to remove</param>
		public virtual void Remove(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			this.lostRects.Add(sprite.RectangleDirty);
			sprite.RemoveInternal(this);
			List.Remove(sprite);
		}

		/// <summary>
		/// Removes sprite from group
		/// </summary>
		/// <param name="sprite">Sprite to remove</param>
		public void RemoveInternal(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			this.lostRects.Add(sprite.RectangleDirty);
			List.Remove(sprite);
		}

		/// <summary>
		/// Removes sprite from this group if they are contained in the given group
		/// </summary>
		/// <param name="spriteCollection">
		/// Remove SpriteCollection from this SpriteCollection.
		/// </param>
		public virtual void Remove(SpriteCollection spriteCollection)
		{
			if (spriteCollection == null)
			{
				throw new ArgumentNullException("spriteCollection");
			}
			for (int i = 0; i < spriteCollection.Count; i++)
			{
				if (this.Contains(spriteCollection[i]))
				{
					this.Remove(spriteCollection[i]);
				}
			}
		}

		/// <summary>
		/// Checks if sprite is in the container
		/// </summary>
		/// <param name="sprite">Sprite to query for</param>
		/// <returns>True is the sprite is in the container.</returns>
		public bool Contains(Sprite sprite)
		{
			return (List.Contains(sprite));
		}

		#endregion

		#region Geometry

		/// <summary>
		/// Gets the size of the first sprite in the collection, otherwise a size of 0,0.
		/// </summary>
		/// <returns>The size of the first sprite in the collection.</returns>
		public virtual Size Size
		{
			get
			{
				if (this.Count > 0)
				{
					return this[0].Size;
				}
				else
				{
					return new Size(0, 0);
				}
			}
		}
		#endregion

		#region Events

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableActiveEvent()
		{
			Events.AppActive += new ActiveEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableJoystickAxisEvent()
		{
			Events.JoystickAxisMotion += new JoystickAxisEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableJoystickBallEvent()
		{
			Events.JoystickBallMotion += new JoystickBallEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableJoystickButtonEvent()
		{
			Events.JoystickButtonDown += new JoystickButtonEventHandler(Update);
			Events.JoystickButtonUp += new JoystickButtonEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableJoystickButtonDownEvent()
		{
			Events.JoystickButtonDown += new JoystickButtonEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableJoystickButtonUpEvent()
		{
			Events.JoystickButtonUp += new JoystickButtonEventHandler(Update);
		}		

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableJoystickHatEvent()
		{
			Events.JoystickHatMotion += new JoystickHatEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableKeyboardEvent()
		{
			Events.KeyboardUp += new KeyboardEventHandler(Update);
			Events.KeyboardDown += new KeyboardEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableKeyboardDownEvent()
		{
			Events.KeyboardDown += new KeyboardEventHandler(Update);
		}
		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableKeyboardUpEvent()
		{
			Events.KeyboardUp += new KeyboardEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableMouseButtonEvent()
		{
			Events.MouseButtonDown += new MouseButtonEventHandler(Update);
			Events.MouseButtonUp += new MouseButtonEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableMouseButtonDownEvent()
		{
			Events.MouseButtonDown += new MouseButtonEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableMouseButtonUpEvent()
		{
			Events.MouseButtonUp += new MouseButtonEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableMouseMotionEvent()
		{
			Events.MouseMotion += new MouseMotionEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableUserEvent()
		{
			Events.UserEvent += new UserEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableQuitEvent()
		{
			Events.Quit += new QuitEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableVideoExposeEvent()
		{
			Events.VideoExpose += new VideoExposeEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableVideoResizeEvent()
		{
			Events.VideoResize += new VideoResizeEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableChannelFinishedEvent()
		{
			Events.ChannelFinished += new ChannelFinishedEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableMusicFinishedEvent()
		{
			Events.MusicFinished += new MusicFinishedEventHandler(Update);
		}

		/// <summary>
		/// Enables Event for SpriteCollection
		/// </summary>
		public void EnableTickEvent()
		{
			Events.Tick += new TickEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableActiveEvent()
		{
			Events.AppActive -= new ActiveEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableJoystickAxisEvent()
		{
			Events.JoystickAxisMotion -= new JoystickAxisEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableJoystickBallEvent()
		{
			Events.JoystickBallMotion -= new JoystickBallEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableJoystickButtonEvent()
		{
			Events.JoystickButtonDown -= new JoystickButtonEventHandler(Update);
			Events.JoystickButtonUp -= new JoystickButtonEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableJoystickButtonDownEvent()
		{
			Events.JoystickButtonDown -= new JoystickButtonEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableJoystickButtonUpEvent()
		{
			Events.JoystickButtonUp -= new JoystickButtonEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableJoystickHatEvent()
		{
			Events.JoystickHatMotion -= new JoystickHatEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableKeyboardEvent()
		{
			Events.KeyboardUp -= new KeyboardEventHandler(Update);
			Events.KeyboardDown -= new KeyboardEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableKeyboardDownEvent()
		{
			Events.KeyboardDown -= new KeyboardEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableKeyboardUpEvent()
		{
			Events.KeyboardUp -= new KeyboardEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableMouseButtonEvent()
		{
			Events.MouseButtonDown -= new MouseButtonEventHandler(Update);
			Events.MouseButtonUp -= new MouseButtonEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableMouseButtonDownEvent()
		{
			Events.MouseButtonDown -= new MouseButtonEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableMouseButtonUpEvent()
		{
			Events.MouseButtonUp -= new MouseButtonEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableMouseMotionEvent()
		{
			Events.MouseMotion -= new MouseMotionEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableUserEvent()
		{
			Events.UserEvent -= new UserEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableQuitEvent()
		{
			Events.Quit -= new QuitEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableVideoExposeEvent()
		{
			Events.VideoExpose -= new VideoExposeEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableVideoResizeEvent()
		{
			Events.VideoResize -= new VideoResizeEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableChannelFinishedEvent()
		{
			Events.ChannelFinished -= new ChannelFinishedEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableMusicFinishedEvent()
		{
			Events.MusicFinished -= new MusicFinishedEventHandler(Update);
		}

		/// <summary>
		/// Disables Event for SpriteCollection
		/// </summary>
		public void DisableTickEvent()
		{
			Events.Tick -= new TickEventHandler(Update);
		}


		/// <summary>
		/// Processes an active event.
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, ActiveEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}

		/// <summary>
		/// Processes a joystick motion event. This event is triggered by
		/// SDL. Only
		/// sprites that are JoystickSensitive are processed.
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, JoystickAxisEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}
		
		/// <summary>
		/// Processes a joystick hat motion event. This event is triggered by
		/// SDL. Only
		/// sprites that are JoystickSensitive are processed.
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, JoystickBallEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}
		
		/// <summary>
		/// Processes a joystick button event. This event is triggered by
		/// SDL. Only
		/// sprites that are JoystickSensitive are processed.
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, JoystickButtonEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}
		
		/// <summary>
		/// Processes a joystick hat motion event. This event is triggered by
		/// SDL. Only
		/// sprites that are JoystickSensitive are processed.
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, JoystickHatEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}
		
		/// <summary>
		/// Processes the keyboard.
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, KeyboardEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}

		/// <summary>
		/// Processes a mouse button. This event is trigger by the SDL
		/// system. 
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, MouseButtonEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}

		/// <summary>
		/// Processes a mouse motion event. This event is triggered by
		/// SDL. Only
		/// sprites that are MouseSensitive are processed.
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, MouseMotionEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}
		
		/// <summary>
		/// Processes a Quit event
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, QuitEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}

		/// <summary>
		/// Processes a user event
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, UserEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}

		/// <summary>
		/// Processes a VideoExposeEvent
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, VideoExposeEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}

		/// <summary>
		/// Processes a VideoResizeEvent
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, VideoResizeEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}

		/// <summary>
		/// Processes a ChannelFinishedEvent
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, ChannelFinishedEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}

		/// <summary>
		/// Processes a MusicfinishedEvent
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, MusicFinishedEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}

		/// <summary>
		/// All sprites are tickable, regardless if they actual do
		/// anything. This ensures that the functionality is there, to be
		/// overridden as needed.
		/// </summary>
		/// <param name="sender">Object that sent event</param>
		/// <param name="e">Event arguments</param>
		private void Update(object sender, TickEventArgs e)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Update(e);
			}
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets and sets a sprite in the collection based on the index.
		/// </summary>
		public Sprite this[int index]
		{
			get
			{
				return ((Sprite)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}
		#endregion

		/// <summary>
		/// Removes sprites from all SpriteCollections
		/// </summary>
		public virtual void Kill()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].Kill();
			}
		}

		/// <summary>
		/// Detects if a given sprite intersects with any sprites in this sprite collection.
		/// </summary>
		/// <param name="sprite">Sprite to intersect with</param>
		/// <returns>
		/// SpriteCollection of sprite in this SpriteCollection that 
		/// intersect with the given Sprite
		/// </returns>
		public virtual SpriteCollection IntersectsWith(Sprite sprite)
		{
			SpriteCollection intersection = new SpriteCollection();
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].IntersectsWith(sprite))
				{
					intersection.Add(this[i]);
				}
			}
			return intersection;
		}

		/// <summary>
		/// Detects if any sprites in a given SpriteCollection 
		/// intersect with any sprites in this SpriteCollection.
		/// </summary>
		/// <param name="spriteCollection">
		/// SpriteCollection to check intersections
		/// </param>
		/// <returns>
		/// Hashtable with sprites in this SpriteCollection as 
		/// keys and SpriteCollections containing sprites they 
		/// intersect with from the given SpriteCollection
		/// </returns>
		public virtual Hashtable IntersectsWith(SpriteCollection spriteCollection)
		{
			if (spriteCollection == null)
			{
				throw new ArgumentNullException("spriteCollection");
			}
			Hashtable intersection = new Hashtable();
			for (int i = 0; i < this.Count; i++)
			{
				for (int j = 0; j < spriteCollection.Count; j++)
					if (this[i].IntersectsWith(spriteCollection[j]))
					{
						if (intersection.Contains(this[i]))
						{
							((SpriteCollection)intersection[this[i]]).Add(spriteCollection[j]);
						}
						else
						{
							intersection.Add(this[i], spriteCollection[j]);
						}
					}
			}
			return intersection;
		}

		/// <summary>
		/// Provide the explicit interface member for ICollection.
		/// </summary>
		/// <param name="array">Array to copy collection to</param>
		/// <param name="index">Index at which to insert the collection items</param>
		void ICollection.CopyTo(Array array, int index)
		{
			this.List.CopyTo(array, index);
		}

		/// <summary>
		/// Provide the explicit interface member for ICollection.
		/// </summary>
		/// <param name="array">Array to copy collection to</param>
		/// <param name="index">Index at which to insert the collection items</param>
		public virtual void CopyTo(Sprite[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		/// <summary>
		/// Insert a Sprite into the collection
		/// </summary>
		/// <param name="index">Index at which to insert the sprite</param>
		/// <param name="sprite">Sprite to insert</param>
		public virtual void Insert(int index, Sprite sprite)
		{
			List.Insert(index, sprite);
		} 

		/// <summary>
		/// Gets the index of the given sprite in the collection.
		/// </summary>
		/// <param name="sprite">The sprite to search for.</param>
		/// <returns>The index of the given sprite.</returns>
		public virtual int IndexOf(Sprite sprite)
		{
			return List.IndexOf(sprite);
		} 
	}
}
