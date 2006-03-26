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
	/// Event args for user-defined events.
	/// </summary>
	public class UserEventArgs : SdlEventArgs 
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public UserEventArgs()
		{
			Sdl.SDL_Event evt = new Sdl.SDL_Event();
			evt.type = (byte)EventTypes.UserEvent;
			evt.user.type =  (byte)EventTypes.UserEvent;
			this.EventStruct = evt;
		}
		/// <summary>
		/// Constructor for using a given user event
		/// </summary>
		/// <param name="userEvent">The user event object</param>
		public UserEventArgs(object userEvent)
		{
			Sdl.SDL_Event evt = new Sdl.SDL_Event();
			this.userEvent = userEvent;
			evt.type = (byte)EventTypes.UserEvent;
			evt.user.type =  (byte)EventTypes.UserEvent;
			this.EventStruct = evt;
		}

		/// <summary>
		/// 
		/// </summary>
		internal UserEventArgs(Sdl.SDL_Event evt) : base(evt)
		{
		}
		
		object userEvent;
		/// <summary>
		/// 
		/// </summary>
		public object UserEvent
		{
			get
			{
				return this.userEvent;
			}
		}

		/// <summary>
		/// User code that uniquely defines this user event.
		/// </summary>
		public int UserCode
		{
			get
			{
				return this.EventStruct.user.code;
			}
			set
			{
				Sdl.SDL_Event evt = this.EventStruct;
				evt.user.code = value;
				this.EventStruct = evt;
			}
		}
	}
}
