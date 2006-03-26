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
	/// Summary description for JoystickHatEventArgs.
	/// </summary>
	public class JoystickHatEventArgs : SdlEventArgs 
	{
		/// <summary>
		/// Joystick Hat event args
		/// </summary>
		/// <param name="device">The joystick index</param>
		/// <param name="hatIndex">The hat index</param>
		/// <param name="hatValue">The new hat position</param>
		public JoystickHatEventArgs(byte device, byte hatIndex, byte hatValue)
		{
			Sdl.SDL_Event evt = new Sdl.SDL_Event();
			evt.jhat.which = device;
			evt.jhat.hat = hatIndex;
			evt.jhat.val = hatValue;
			evt.type = (byte)EventTypes.JoystickHatMotion;
			this.EventStruct = evt;
		}

		internal JoystickHatEventArgs(Sdl.SDL_Event evt) : base(evt)
		{
		}

		/// <summary>
		/// joystick
		/// </summary>
		public int Device
		{
			get
			{
				return this.EventStruct.jhat.which;
			}
		}

		/// <summary>
		/// Hat Index
		/// </summary>
		public int HatIndex
		{
			get
			{
				return this.EventStruct.jhat.hat;
			}
		}

		/// <summary>
		/// Hat value
		/// </summary>
		public int HatValue
		{
			get
			{ 
				return this.EventStruct.jhat.val;
			}
		}
	}
}
