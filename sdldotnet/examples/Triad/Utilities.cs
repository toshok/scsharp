
//*****************************************************************************
//	This program is free software; you can redistribute it and/or
//	modify it under the terms of the GNU General Public License
//	as published by the Free Software Foundation; either version 2
//	of the License, or (at your option) any later version.
//	This program is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//	GNU General Public License for more details.
//	You should have received a copy of the GNU General Public License
//	along with this program; if not, write to the Free Software
//	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//	
//	Created by Michael Rosario
//	July 29th,2003
//	Contact me at mrosario@scrypt.net	
//*****************************************************************************



using System;
using System.Drawing;
using SdlDotNet;

namespace SdlDotNet.Examples.Triad
{

	/// <summary>
	/// 
	/// </summary>
	public sealed class Utilities
	{
		private Utilities()
		{}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="rectangle"></param>
		/// <param name="color"></param>
		public static void DrawRect(Surface surface, Rectangle rectangle, Color color)
		{
			Utilities.DrawRect(surface,rectangle.Location,rectangle.Size,color);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="location"></param>
		/// <param name="size"></param>
		/// <param name="color"></param>
		public static void DrawRect(Surface surface, Point location, Size size, Color color)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			int x;
			int y;

			//Draw top line...
			for(x=location.X; x<location.X + size.Width; x++)
			{
				surface.DrawPixel(x,location.Y,color);
			}

			//Draw bottom line...
			for(x=location.X; x<location.X + size.Width; x++)
			{
				surface.DrawPixel(x,location.Y + size.Height,color);
			}

			//Draw left line...
			for(y=location.Y; y<location.Y+size.Height;y++)
			{
				surface.DrawPixel(location.X,y,color);
			}

			//Draw right line... 
			for(y=location.Y; y<location.Y+size.Height;y++)
			{
				surface.DrawPixel(location.X+size.Width,y,color);
			}
		}
	}
}
