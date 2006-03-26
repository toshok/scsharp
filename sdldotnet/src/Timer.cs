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
	/// The Timer class holds methods for time-related functions
	/// </summary>
	public sealed class Timer
	{
		Timer()
		{}

		static Timer()
		{
			Initialize();
		}

		/// <summary>
		/// Closes and destroys this object
		/// </summary>
		public static void Close() 
		{
			if (Sdl.SDL_WasInit(Sdl.SDL_INIT_TIMER) != 0)
			{
				Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_TIMER);
			}
		}

		/// <summary>
		/// Initialize timer.
		/// </summary>
		public static void Initialize()
		{
			if ((Sdl.SDL_WasInit(Sdl.SDL_INIT_TIMER)) 
				== (int) SdlFlag.FalseValue)
			{
				if (Sdl.SDL_Init(Sdl.SDL_INIT_TIMER) != (int) SdlFlag.Success)
				{
					throw SdlException.Generate();
				}
			}
		}

//		/// <summary>
//		/// Queries if the Timer subsystem has been intialized.
//		/// </summary>
//		/// <returns>
//		/// True if Timer subsystem has been initialized, false if it has not.
//		/// </returns>
//		public static bool IsInitialized
//		{
//			get
//			{
//				if ((Sdl.SDL_WasInit(Sdl.SDL_INIT_TIMER) & Sdl.SDL_INIT_TIMER) 
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
		/// Gets the number of milliseconds since Sdl was initialized.  
		/// This is not a high-resolution timer.
		/// </summary>
		public static int TicksElapsed
		{
			get
			{
				return Sdl.SDL_GetTicks();
			}
		}

		/// <summary>
		/// Gets the number of seconds since Sdl was initialized.  
		/// This is not a high-resolution timer.
		/// </summary>
		public static float SecondsElapsed
		{
			get
			{
				return Timer.TicksElapsed / 1000;
			}
		}

		/// <summary>
		/// Wait a number of milliseconds.
		/// </summary>
		/// <param name="delayTime">Delay time in milliseconds</param>
		public static void DelayTicks(int delayTime)
		{
			Sdl.SDL_Delay(delayTime);
		}

		/// <summary>
		/// Wait a number of milliseconds.
		/// </summary>
		/// <param name="delayTime">Delay time in seconds</param>
		public static void DelaySeconds(int delayTime)
		{
			int delayTimeMax = Int32.MaxValue / 1000;
			int delayTimeTemp = delayTime * 1000;
			if (delayTime <= delayTimeMax)
			{
				Sdl.SDL_Delay(delayTimeTemp);
			}
			else
			{
				throw new OverflowException("delayTime is too large");
			}
		}

		/// <summary>
		/// Converts number of CD frames to seconds
		/// </summary>
		/// <param name="frames">Number of frames</param>
		/// <returns>Seconds</returns>
		public static double FramesToSeconds(int frames) 
		{
			return (double)(frames / Sdl.CD_FPS);
		}

		/// <summary>
		/// Converts seconds to number of CD frames
		/// </summary>
		/// <param name="seconds">Number of seconds</param>
		/// <returns>Frames</returns>
		public static int SecondsToFrames(int seconds) 
		{
			if (seconds <= Int32.MaxValue / Sdl.CD_FPS)
			{
				return (seconds * Sdl.CD_FPS);
			}
			else
			{
				throw new OverflowException();
			}
		}
		
		/// <summary>
		/// Converts frames to Timespan
		/// </summary>
		/// <param name="frames">Number of frames</param>
		/// <returns>Timespan</returns>
		public static TimeSpan FramesToTime(int frames)
		{
			return new TimeSpan((long)Timer.FramesToSeconds(frames) * TimeSpan.TicksPerSecond);
		}

		/// <summary>
		/// Converts seconds to Timespan
		/// </summary>
		/// <param name="seconds">Number of seconds</param>
		/// <returns>Timespan</returns>
		public static TimeSpan SecondsToTime(int seconds)
		{
			return new TimeSpan(seconds * TimeSpan.TicksPerSecond);
		}
	}
}
