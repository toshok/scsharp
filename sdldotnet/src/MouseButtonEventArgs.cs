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
using System.Drawing;

using Tao.Sdl;

namespace SdlDotNet
{
	/// <summary>
	/// Event arguments for MouseMotion Events.
	/// </summary>
	public class MouseButtonEventArgs : SdlEventArgs 
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="button">The mouse button</param>
		/// <param name="buttonPressed">True if the button is pressed, 
		/// False if it is released</param>
		/// <param name="positionX">The current X coordinate</param>
		/// <param name="positionY">The current Y coordinate</param>
		public MouseButtonEventArgs(MouseButton button, bool buttonPressed, short positionX, short positionY)
		{
			Sdl.SDL_Event evt = new Sdl.SDL_Event();
			evt.button.button = (byte)button;
			evt.button.which = 0;
			evt.button.x = positionX;
			evt.button.y = positionY;
			if (buttonPressed)
			{
				evt.button.state = (byte)ButtonKeyState.Pressed;
				evt.type = (byte)EventTypes.MouseButtonDown;
			}
			else
			{
				evt.button.state = (byte)ButtonKeyState.NotPressed;
				evt.type = (byte)EventTypes.MouseButtonUp;
			}
			this.EventStruct = evt;
		}

		internal MouseButtonEventArgs(Sdl.SDL_Event evt) : base(evt)
		{
		}

		/// <summary>
		/// Which mouse button created the event
		/// </summary>
		public MouseButton Button
		{
			get
			{
				return (MouseButton)this.EventStruct.button.button;
			}
		}

		/// <summary>
		/// True if button is pressed
		/// </summary>
		public bool ButtonPressed
		{
			get
			{
				return (this.EventStruct.button.state == (byte)ButtonKeyState.Pressed);
			}
		}

		/// <summary>
		/// X position of mouse at time of event
		/// </summary>
		public short X
		{
			get
			{
				return this.EventStruct.button.x;
			}
		}

		/// <summary>
		/// Y position of mouse at time of event 
		/// </summary>
		public short Y
		{
			get
			{ 
				return this.EventStruct.button.y;
			}
		}

		/// <summary>
		/// Return Point(X, Y) of mouse
		/// </summary>
		public Point Position
		{
			get
			{
				return new Point(this.EventStruct.button.x, this.EventStruct.button.y);
			}
		}
	}
}
