/*
 * $RCSfile$
 * Copyright (C) 2004 D. R. E. Moonfire (d.moonfire@mfgames.com)
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

namespace SdlDotNet.Examples.GuiExample
{
	/// <summary>
	/// 
	/// </summary>
	public class Padding
	{
		private int [] padding = new int [] { 0, 0, 0, 0 };

		/// <summary>
		/// 
		/// </summary>
		public Padding()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eachItem"></param>
		public Padding(int eachItem)
		{
			padding = new int [] { eachItem, eachItem, eachItem, eachItem };
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sides"></param>
		/// <param name="height"></param>
		public Padding(int sides, int height)
		{
			padding = new int [] { sides, height, sides, height };
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		public Padding(int left, int top, int right, int bottom)
		{
			padding = new int [] { left, top, right, bottom };
		}

		/// <summary>
		/// 
		/// </summary>
		public int Horizontal
		{
			get { return Left + Right; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int Vertical
		{
			get { return Top + Bottom; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int Left
		{
			get { return padding[0]; }
			set { padding[0] = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int Right
		{
			get { return padding[2]; }
			set { padding[2] = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int Top
		{
			get { return padding[1]; }
			set { padding[1] = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int Bottom
		{
			get { return padding[3]; }
			set { padding[3] = value; }
		}
	}
}
