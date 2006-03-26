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
	public class MouseMotionEventArgs : SdlEventArgs 
	{
		/// <summary>
		/// MouseMotion
		/// </summary>
		/// <param name="buttonPressed">The current mouse button state</param>
		/// <param name="button">Button pressed</param>
		/// <param name="positionX">The current X coordinate</param>
		/// <param name="positionY">The current Y coordinate</param>
		/// <param name="relativeX">
		/// The difference between the last X coordinate and current</param>
		/// <param name="relativeY">
		/// The difference between the last Y coordinate and current</param>
		public MouseMotionEventArgs(bool buttonPressed, MouseButton button, short positionX, short positionY, 
			short relativeX, short relativeY)
		{
			Sdl.SDL_Event evt = new Sdl.SDL_Event();
			evt.motion.xrel = relativeX;
			evt.motion.yrel = relativeY;
			evt.motion.which = (byte)button;
			evt.motion.x = positionX;
			evt.motion.y = positionY;
			evt.type = (byte)EventTypes.MouseMotion;
			if (buttonPressed)
			{
				evt.motion.state = (byte)ButtonKeyState.Pressed;
			}
			else
			{
				evt.motion.state = (byte)ButtonKeyState.NotPressed;
			}
			this.EventStruct = evt;
		}

		internal MouseMotionEventArgs(Sdl.SDL_Event evt) : base(evt)
		{
		}

		/// <summary>
		/// True if button pressed
		/// </summary>
		public bool ButtonPressed
		{
			get
			{
				return (this.EventStruct.motion.state != (byte)ButtonKeyState.NotPressed);
			}
		}

		/// <summary>
		/// Returns which button was pressed
		/// </summary>
		public MouseButton Button
		{
			get
			{
				if ((this.EventStruct.motion.state&Sdl.SDL_BUTTON_LMASK) != 0)
				{
					return MouseButton.PrimaryButton;
				}
				else if ((this.EventStruct.motion.state&Sdl.SDL_BUTTON_RMASK) != 0)
				{
					return MouseButton.SecondaryButton;
				}
				else if ((this.EventStruct.motion.state&Sdl.SDL_BUTTON_MMASK) != 0)
				{
					return MouseButton.MiddleButton;
				}
				else if ((this.EventStruct.motion.state&Sdl.SDL_BUTTON((byte)MouseButton.WheelDown)) != 0)
				{
					return MouseButton.WheelDown;
				}
				else if ((this.EventStruct.motion.state&Sdl.SDL_BUTTON((byte)MouseButton.WheelUp)) != 0)
				{
					return MouseButton.WheelUp;
				}
				else
				{
					return MouseButton.None;
				}				
			}
		}

		/// <summary>
		/// X position of mouse
		/// </summary>
		public short X
		{
			get
			{
				return this.EventStruct.motion.x;
			}
		}

		/// <summary>
		/// Y position of mouse
		/// </summary>
		public short Y
		{
			get
			{ 
				return this.EventStruct.motion.y;
			}
		}

		/// <summary>
		/// change in X position of mouse
		/// </summary>
		public short RelativeX
		{
			get
			{
				return this.EventStruct.motion.xrel;
			}
		}

		/// <summary>
		/// Change in Y position of mouse
		/// </summary>
		public short RelativeY
		{
			get
			{
				return this.EventStruct.motion.yrel;
			}
		}
		/// <summary>
		/// Returns Point(X, Y) of mouse
		/// </summary>
		public Point Position
		{
			get
			{
				return new Point(this.EventStruct.motion.x, this.EventStruct.motion.y);
			}
		}
	}
}
