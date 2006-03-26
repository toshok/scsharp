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

using System.Runtime.InteropServices;
using System.Drawing;
using Tao.Sdl;

namespace SdlDotNet
{
	/// <summary>
	/// Contains basic color conversions and operations
	/// </summary>
	public sealed class SdlColor
	{
		SdlColor()
		{}

		/// <summary>
		/// Converts .NET Color struct to SDL_Color
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		internal static Sdl.SDL_Color ConvertColor(Color color)
		{
			return new Sdl.SDL_Color(color.R, color.G, color.B, color.A);
		}

//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="color"></param>
//		/// <returns></returns>
//		internal static Color ConvertColor(Sdl.SDL_Color color)
//		{
//			return Color.FromArgb(0, color.r, color.g, color.b);
//		}

		/// <summary>
		/// Adds the values of one color to another color.
		/// </summary>
		/// <param name="color1">The first color.</param>
		/// <param name="color2">The second color.</param>
		/// <returns>The resultant color.</returns>
		public static Color Add(Color color1, Color color2)
		{
			byte r;
			byte g;
			byte b;
			byte a;

			if (color1.R + color2.R > byte.MaxValue)
			{
				r = byte.MaxValue;
			}
			else
			{
				r = (byte)(color1.R + color2.R);
			}
			if (color1.G + color2.G > byte.MaxValue)
			{
				g = byte.MaxValue;
			}
			else
			{
				g = (byte)(color1.G + color2.G);
			}
			if (color1.B + color2.B > byte.MaxValue)
			{
				b = byte.MaxValue;
			}
			else
			{
				b = (byte)(color1.B + color2.B);
			}
			if (color1.A + color2.A > byte.MaxValue)
			{
				a = byte.MaxValue;
			}
			else
			{
				a = (byte)(color1.A + color2.A);
			}
			return Color.FromArgb(a, r, g, b);
		}

		/// <summary>
		/// Subtracts the values of one color from another color.
		/// </summary>
		/// <param name="color1">
		/// The first color.
		/// </param>
		/// <param name="color2">
		/// The second color, the color to subtract.
		/// </param>
		/// <returns>The resultant color.</returns>
		public static Color Subtract(Color color1, Color color2)
		{
			byte r;
			byte g;
			byte b;
			byte a;

			if (color1.R - color2.R < byte.MinValue)
			{
				r = byte.MinValue;
			}
			else
			{
				r = (byte)(color1.R - color2.R);
			}
			if (color1.G - color2.G < byte.MinValue)
			{
				g = byte.MinValue;
			}
			else
			{
				g = (byte)(color1.G - color2.G);
			}
			if (color1.B - color2.B < byte.MinValue)
			{
				b = byte.MinValue;
			}
			else
			{
				b = (byte)(color1.B - color2.B);
			}
			if (color1.A - color2.A < byte.MinValue)
			{
				a = byte.MinValue;
			}
			else
			{
				a = (byte)(color1.A - color2.A);
			}
			return Color.FromArgb(a, r, g, b);
		}

		/// <summary>
		/// Inverts the color, giving it a negative effect.
		/// </summary>
		/// <param name="color">The color to invert.</param>
		/// <returns>The inverted color.</returns>
		public static Color Invert(Color color)
		{
            // The following line of code works as well.
            //return SdlColor.Subtract(Color.White, color);
            byte r;
            byte g;
            byte b;
            byte a;

            if (Color.White.R - color.R < byte.MinValue)
            {
                r = byte.MinValue;
            }
            else
            {
                r = (byte)(Color.White.R - color.R);
            }
            if (Color.White.G - color.G < byte.MinValue)
            {
                g = byte.MinValue;
            }
            else
            {
                g = (byte)(Color.White.G - color.G);
            }
            if (Color.White.B - color.B < byte.MinValue)
            {
                b = byte.MinValue;
            }
            else
            {
                b = (byte)(Color.White.B - color.B);
            }
            if (Color.White.A - color.A < byte.MinValue)
            {
                a = byte.MinValue;
            }
            else
            {
                a = (byte)(Color.White.A - color.A);
            }
            return Color.FromArgb(a, r, g, b);
        }

		/// <summary>
		/// Multiplies the values of one color with another color.
		/// </summary>
		/// <param name="color1">
		/// The first color.
		/// </param>
		/// <param name="color2">
		/// The second color.
		/// </param>
		/// <returns>
		/// The resultant color of the first and the second color multiplied.
		/// </returns>
		public static Color Multiply(Color color1, Color color2)
		{
			byte r;
			byte g;
			byte b;
			byte a;

			if (color1.R * color2.R > byte.MaxValue)
			{
				r = byte.MaxValue;
			}
			else
			{
				r = (byte)(color1.R * color2.R);
			}
			if (color1.G * color2.G > byte.MaxValue)
			{
				g = byte.MaxValue;
			}
			else
			{
				g = (byte)(color1.G * color2.G);
			}
			if (color1.B * color2.B > byte.MaxValue)
			{
				b = byte.MaxValue;
			}
			else
			{
				b = (byte)(color1.B * color2.B);
			}
			if (color1.A * color2.A > byte.MaxValue)
			{
				a = byte.MaxValue;
			}
			else
			{
				a = (byte)(color1.A * color2.A);
			}
			return Color.FromArgb(a, r, g, b);
		}
	}
}
