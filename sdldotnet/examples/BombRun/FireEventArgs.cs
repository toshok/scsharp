/* This file is part of BombRun
 * (c) 2003 Sijmen Mulder
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
using System;
using System.Drawing;

namespace SdlDotNet.Examples.BombRun
{
	/// <summary>
	/// 
	/// </summary>
	public delegate void FireEventHandler(object sender, FireEventArgs e);
	/// <summary>
	/// 
	/// </summary>
	public delegate void DisposeRequestEventHandler(object sender, EventArgs e);
	/// <summary>
	/// 
	/// </summary>
	public class FireEventArgs : EventArgs
	{
		Point location;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="location"></param>
		public FireEventArgs(Point location)
		{
			this.location = location;
		}

		/// <summary>
		/// 
		/// </summary>
		public Point Location
		{
			get
			{
				return location;
			}
		}
	}
}