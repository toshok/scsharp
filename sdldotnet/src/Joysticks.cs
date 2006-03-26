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

using System;

using Tao.Sdl;

namespace SdlDotNet 
{
	/// <summary>
	/// Provides methods for querying the number and make-up of the joysticks on a system.
	/// You can obtain an instance of this class by accessing the Joysticks property of the main Sdl object.
	/// Note that actual joystick input is handled by the Events class
	/// </summary>
	public sealed class Joysticks 
	{
		Joysticks()
		{}

		static Joysticks()
		{
			Initialize();
		}

		/// <summary>
		/// Starts joystick subsystem
		/// </summary>
		public static void Initialize()
		{
			if ((Sdl.SDL_WasInit(Sdl.SDL_INIT_JOYSTICK)) 
				== (int) SdlFlag.FalseValue)
			{
				if (Sdl.SDL_Init(Sdl.SDL_INIT_JOYSTICK) != (int) SdlFlag.Success)
				{
					throw SdlException.Generate();
				}
			}
		}

//		/// <summary>
//		/// Queries if the Joystick subsystem has been intialized.
//		/// </summary>
//		/// <remarks>
//		/// </remarks>
//		/// <returns>True if Joystick subsystem has been initialized, false if it has not.</returns>
//		public static bool IsInitialized
//		{
//			get
//			{
//				if ((Sdl.SDL_WasInit(Sdl.SDL_INIT_JOYSTICK) & Sdl.SDL_INIT_JOYSTICK) 
//					!= (int) SdlFlag.FalseValue)
//				{
//					return true;
//				}
//				else 
//				{
//					return false;
//				}
//			}
//		}

		/// <summary>
		/// Returns true if the joystick has been initialized
		/// </summary>
		public static bool IsJoystickInitialized(int index)
		{
				if (Sdl.SDL_JoystickOpened(index) != (int) SdlFlag.FalseValue)
				{
					return true;
				}
				else
				{
					return false;
				}
		}

		/// <summary>
		/// Closes and destroys this object
		/// </summary>
		public static void Close() 
		{
			Events.CloseJoysticks();
		}

		/// <summary>
		/// Returns the number of joysticks on this system
		/// </summary>
		/// <returns>
		/// The number of joysticks
		/// </returns>
		public static int NumberOfJoysticks 
		{
			get
			{
				return Sdl.SDL_NumJoysticks();
			}
		}

		/// <summary>
		/// Checks to see if joystick number is valid
		/// </summary>
		/// <param name="index">Index of joystick to query</param>
		/// <returns>True if joystick is valid</returns>
		public static bool IsValidJoystickNumber(int index)
		{
			if (index >= 0 && index < NumberOfJoysticks)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Creates a joystick object to read information about a joystick
		/// </summary>
		/// <param name="index">The 0-based index of the joystick to read</param>
		/// <returns>A Joystick object</returns>
		public static Joystick OpenJoystick(int index) 
		{
			IntPtr joystick = Sdl.SDL_JoystickOpen(index);
			if (!IsValidJoystickNumber(index) || (joystick == IntPtr.Zero))
			{
				throw SdlException.Generate();
			}
			return new Joystick(joystick);
		}
	}
}
