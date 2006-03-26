/*
 * $RCSfile$
 * Copyright (C) 2005 Rob Loach (http://www.robloach.net)
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
using System.Threading;

using Tao.Sdl;

namespace SdlDotNet
{
	/// <summary>
	/// Event arguments for a Framerate tick.
	/// </summary>
	public class  TickEventArgs : UserEventArgs 
	{
		private int lastTick;
		private int tick;
		private int fps;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tick">
		/// The current tick.
		/// </param>
		/// <param name="lastTick">
		/// The tick count that it was at last frame.
		/// </param>
		/// <param name="fps">Frames per second</param>
		public TickEventArgs(int tick, int lastTick, int fps)
		{
			this.tick = tick;
			this.lastTick = lastTick;
			this.fps = fps;
		}

		/// <summary>
		/// Gets when the last frame tick occurred.
		/// </summary>
		public int LastTick
		{
			get
			{
				return this.lastTick;
			}
		}
		
		/// <summary>
		/// Gets the FPS as of the event call. Events.FPS is an alternative.
		/// </summary>
		public int Fps
		{
			get
			{
				return this.fps;
			}
		}

		/// <summary>
		/// Gets the current SDL tick time.
		/// </summary>
		public int Tick
		{
			get
			{
				return this.tick;
			}
		}
		
		/// <summary>
		/// Gets the difference in time between the 
		/// current tick and the last tick.
		/// </summary>
		public int TicksElapsed
		{
			get
			{
				return this.tick - this.lastTick;
			}
		}

		/// <summary>
		/// Seconds elapsed between the last tick and the current tick
		/// </summary>
		public float SecondsElapsed
		{
			get
			{
				return (this.TicksElapsed / 1000.0f);
			}
		}
	}
}