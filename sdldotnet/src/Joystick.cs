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
using System.Globalization;

using Tao.Sdl;

namespace SdlDotNet 
{
	/// <summary>
	/// Struct for trackball motion
	/// </summary>
	public struct BallMotion
	{
		int x;
		int y;

		/// <summary>
		/// Ball motion
		/// </summary>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		public BallMotion(int positionX, int positionY)
		{
			this.x = positionX;
			this.y = positionY;
		}

		/// <summary>
		/// motion in X-axis
		/// </summary>
		public int MotionX
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		/// <summary>
		/// Motion in Y-axis
		/// </summary>
		public int MotionY
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		/// <summary>
		/// String output
		/// </summary>
		/// <returns>String repesentation.</returns>
		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "({0},{1})", x, y);
		}
		/// <summary>
		/// Equals
		/// </summary>
		/// <param name="obj">Object for comparison</param>
		/// <returns>If objects are equal, this returns true.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (obj.GetType() != typeof(BallMotion))
				return false;
                
			BallMotion c = (BallMotion)obj;   
			return ((this.x == c.x) && (this.y == c.y));
		}

		/// <summary>
		/// Equals operator
		/// </summary>
		/// <param name="c1">object to compare</param>
		/// <param name="c2">object to compare</param>
		/// <returns>If objects are equal, this returns true.</returns>
		public static bool operator== (BallMotion c1, BallMotion c2)
		{
			return ((c1.x == c2.x) && (c1.y == c2.y));
		}
		
		/// <summary>
		/// Not equals operator
		/// </summary>
		/// <param name="c1">object to compare</param>
		/// <param name="c2">object to compare</param>
		/// <returns>If objects are not equal, this returns true.</returns>
		public static bool operator!= (BallMotion c1, BallMotion c2)
		{
			return !(c1 == c2);
		}

		/// <summary>
		/// GetHashCode openrator
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode()
		{
			return x ^ y;

		}
	}
	/// <summary>
	/// Represents a joystick on the system
	/// </summary>
	public class Joystick : BaseSdlResource 
	{
		private int index;
		private bool disposed;
		private const float JOYSTICK_ADJUSTMENT = 32768;
		private const float JOYSTICK_SCALE = 65535;
		//private const short JOYSTICK_THRESHHOLD = 3277;

		static Joystick()
		{
			Joysticks.Initialize();
		}

		/// <summary>
		/// open joystick at index number
		/// </summary>
		/// <param name="index"></param>
		public Joystick(int index)
		{
			if (Joysticks.IsValidJoystickNumber(index))
			{
				this.Handle = Sdl.SDL_JoystickOpen(index);
			}
			if (this.Handle == IntPtr.Zero)
			{
				throw SdlException.Generate();
			}
			else
			{
				this.index = index;
			}
		}
		internal Joystick(IntPtr handle) 
		{
			this.Handle = handle;
			this.index = Sdl.SDL_JoystickIndex(handle); 
		}

		/// <summary>
		/// Destroys the surface object and frees its memory
		/// </summary>
		/// <param name="disposing">True for manual disposing</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.disposed)
				{
					if (disposing)
					{
					}
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		/// Closes Joystick handle
		/// </summary>
		protected override void CloseHandle() 
		{
			try
			{
				if (this.Handle != IntPtr.Zero)
				{
					Sdl.SDL_JoystickClose(this.Handle);
				}
			}
			finally
			{
				this.Handle = IntPtr.Zero;
			}
		}
		

//		/// <summary>
//		/// Returns true if the joystick has been initialized
//		/// </summary>
//		public bool IsInitialized
//		{
//			get
//			{
//				if (Sdl.SDL_JoystickOpened(this.index) == (int) SdlFlag.TrueValue)
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
		/// Gets the 0-based numeric index of this joystick
		/// </summary>
		public int Index 
		{
			get 
			{ 
				return this.index;
			}
		}

		/// <summary>
		/// Gets the number of axes on this joystick (usually 2 for each stick handle)
		/// </summary>
		public int NumberOfAxes 
		{
			get 
			{ 
				int result = Sdl.SDL_JoystickNumAxes(this.Handle); 
				GC.KeepAlive(this);
				return result;
			}
		}

		/// <summary>
		/// Gets the number of trackballs on this joystick
		/// </summary>
		public int NumberOfBalls 
		{
			get 
			{ 
				int result = Sdl.SDL_JoystickNumBalls(this.Handle); 
				GC.KeepAlive(this);
				return result;
			}
		}

		/// <summary>
		/// Gets the number of hats on this joystick
		/// </summary>
		public int NumberOfHats 
		{
			get 
			{ 
				int result = Sdl.SDL_JoystickNumHats(this.Handle); 
				GC.KeepAlive(this);
				return result;
			}
		}

		/// <summary>
		/// Gets the number of buttons on this joystick
		/// </summary>
		public int NumberOfButtons 
		{
			get 
			{ 
				int result = Sdl.SDL_JoystickNumButtons(this.Handle); 
				GC.KeepAlive(this);
				return result;
			}
		}

		/// <summary>
		/// Gets the name of this joystick
		/// </summary>
		public string Name 
		{
			get 
			{ 
				string result = Sdl.SDL_JoystickName(this.Index); 
				GC.KeepAlive(this);
				return result;
			}
		}

		/// <summary>
		/// Gets the current axis position
		/// </summary>
		/// <param name="axis">Vertical or horizontal axis</param>
		/// <returns>Joystick position</returns>
		public float GetAxisPosition(JoystickAxis axis)
		{
			return  ((float)(Sdl.SDL_JoystickGetAxis(this.Handle, (int) axis) + JOYSTICK_ADJUSTMENT) / JOYSTICK_SCALE);
		}

		/// <summary>
		/// Gets the ball motion
		/// </summary>
		/// <param name="ball">ball</param>
		/// <returns>Ballmotion struct</returns>
		public BallMotion GetBallMotion(int ball)
		{
			int motionX;
			int motionY;

			if (Sdl.SDL_JoystickGetBall(
				this.Handle, ball, out motionX, out motionY) == 
				(int) SdlFlag.Success)
			{
				return new BallMotion(motionX, motionY);
			}
			else
			{
				throw new SdlException();
			}
		}

		/// <summary>
		/// Gets the current button state
		/// </summary>
		/// <param name="button">Button to query</param>
		/// <returns>Pressed or not pressed</returns>
		public ButtonKeyState GetButtonState(int button)
		{
			return (ButtonKeyState) Sdl.SDL_JoystickGetButton(this.Handle, button);
		}

		/// <summary>
		/// Gets the current Hat state
		/// </summary>
		/// <param name="hat">Hat to query</param>
		/// <returns>Hat state</returns>
		public JoystickHatStates GetHatState(int hat)
		{
			return (JoystickHatStates) Sdl.SDL_JoystickGetHat(this.Handle, (int) hat);
		}
	}
}
