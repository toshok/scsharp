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
	/// Events args for when a channel finishes playing a sound.
	/// </summary>
	/// <remarks>
	/// This will create event args to trigger an 
	/// event after a sound has finished playing.
	/// </remarks>
	public class ChannelFinishedEventArgs : UserEventArgs 
	{
		/// <summary>
		/// Constructor 
		/// </summary>
		/// <remarks>Can be passed into the event queue.</remarks>
		/// <param name="channel">The channel which has finished</param>
		public ChannelFinishedEventArgs(int channel) : base()
		{
			this.channel = channel;
		}
		
		int channel;

		/// <summary>
		/// Return channel number
		/// </summary>
		/// <remarks></remarks>
		public int Channel
		{
			get
			{
				return this.channel;
			}
			set
			{
				this.channel = value;
			}
		}
	}
}
