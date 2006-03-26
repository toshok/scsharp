/*
 * $RCSfile$
 * Copyright (C) 2004, 2005 David Hudson (jendave@yahoo.com)
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

using Tao.Sdl;

namespace SdlDotNet
{
	/// <summary>
	/// Event arguments for Keyboard Events.
	/// </summary>
	public class KeyboardEventArgs : SdlEventArgs
	{
		/// <summary>
		/// Keyboard event args
		/// </summary>
		/// <param name="down">
		/// True if the key is pressed, False if it was released
		/// </param>
		/// <param name="key">The Sdl virtual keycode</param>
		/// <param name="modifierKeys">Current modifier flags</param>
		public KeyboardEventArgs( 
			Key key, 
			ModifierKeys modifierKeys,
			bool down)
		{
			Sdl.SDL_Event evt = new Sdl.SDL_Event();
			evt.key.which = 0;
			evt.key.keysym.scancode = 0;
			evt.key.keysym.sym = (int)key;
			evt.key.keysym.mod = (int)modifierKeys;
			if (down)
			{
				evt.key.state = (byte)ButtonKeyState.Pressed;
				evt.type = (byte)EventTypes.KeyDown;
			}
			else
			{
				evt.key.state = (byte)ButtonKeyState.NotPressed;
				evt.type = (byte)EventTypes.KeyUp;
			}
			this.EventStruct = evt;
		}

		/// <summary>
		/// Keyboard event args. Does not check modifier keys.
		/// </summary>
		/// <param name="down">
		/// True if the key is pressed, False if it was released
		/// </param>
		/// <param name="key">The Sdl virtual keycode</param>
		public KeyboardEventArgs( Key key, bool down) : this(key, ModifierKeys.None, down)
		{
		}

		internal KeyboardEventArgs(Sdl.SDL_Event evt) : base(evt)
		{
		}
		
		/// <summary>
		/// The device index of the keyboard
		/// </summary>
		public byte Device
		{
			get
			{
				return this.EventStruct.key.which;
			}
		}

		/// <summary>
		/// Is key pressed
		/// </summary>
		public bool Down
		{
			get
			{
				return (this.EventStruct.key.state == Sdl.SDL_PRESSED);
			}
		}

		/// <summary>
		/// The scancode of the key
		/// </summary>
		public byte Scancode
		{
			get
			{
				return this.EventStruct.key.keysym.scancode;
			}
		}


		/// <summary>
		/// Key
		/// </summary>
		public Key Key
		{
			get
			{
				return (Key)this.EventStruct.key.keysym.sym;
			}
		}
        
		/// <summary>
		/// modifier Key
		/// </summary>
		public ModifierKeys Mod
		{
			get
			{
				return (ModifierKeys)this.EventStruct.key.keysym.mod;
			}
		}
	}
}
