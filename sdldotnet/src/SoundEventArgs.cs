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
	/// SoundEvent Arguments.
	/// </summary>
	public class SoundEventArgs : UserEventArgs 
	{
		/// <summary>
		/// SoundEventsArgs describe the action to take on a Sound
		/// </summary>
		/// <param name="action">Stop or Fade out</param>
		/// <param name="fadeoutTime">time to faseout</param>
		public SoundEventArgs(SoundAction action, int fadeoutTime)
		{
			this.action = action;
			this.fadeoutTime = fadeoutTime;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="action"></param>
		public SoundEventArgs(SoundAction action)
		{
			this.action = action;
		}

		private int fadeoutTime;
		private SoundAction action;
		/// <summary>
		/// Get/Set Fadeout Time
		/// </summary>
		public int FadeoutTime
		{
			get
			{
				return this.fadeoutTime;
			}
		}

		/// <summary>
		/// Get/Set Action to take on Sound
		/// </summary>
		public SoundAction Action
		{
			get
			{
				return this.action;
			}
		}
	}
}
