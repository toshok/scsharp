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
using System.Drawing;
using System.Runtime.InteropServices;
//using System.Globalization;
using System.Reflection;
using System.Resources;

using Tao.Sdl;

namespace SdlDotNet
{
	/// <summary>
	/// Information for current video mode..
	/// </summary>
	public sealed class VideoInfo
	{
//		static ResourceManager stringManager;

		VideoInfo()
		{
		}

		static VideoInfo()
		{
			Video.Initialize();
//			stringManager = 
//				new ResourceManager("en-US", Assembly.GetExecutingAssembly());
		}

		private static Sdl.SDL_VideoInfo VideoInfoStruct
		{
			get
			{
				IntPtr videoInfoPointer = Sdl.SDL_GetVideoInfo();
				if(videoInfoPointer == IntPtr.Zero) 
				{
//					throw new SdlException(stringManager.GetString("Video query failed: " 
//						+ Sdl.SDL_GetError(), CultureInfo.CurrentUICulture));
					throw new SdlException("Video query failed: " 
						+ Sdl.SDL_GetError());
				}
				return (Sdl.SDL_VideoInfo)
					Marshal.PtrToStructure(videoInfoPointer, 
					typeof(Sdl.SDL_VideoInfo));
			}
		}

		private static Sdl.SDL_PixelFormat PixelFormat
		{
			get
			{
				return (Sdl.SDL_PixelFormat)
					Marshal.PtrToStructure(VideoInfoStruct.vfmt, 
					typeof(Sdl.SDL_PixelFormat));
			}
		}

		/// <summary>
		/// Bits Per Pixel. Typically 8, 16 or 32.
		/// </summary>
		public static byte BitsPerPixel
		{
			get
			{
				return VideoInfo.PixelFormat.BitsPerPixel;
			}
		}

		/// <summary>
		/// Bytes per pixel. Typically 1, 2 or 4.
		/// </summary>
		public static byte BytesPerPixel
		{
			get
			{
				return VideoInfo.PixelFormat.BytesPerPixel;
			}
		}

		/// <summary>
		/// Alpha Mask
		/// </summary>
		public static int AlphaMask
		{
			get
			{
				return VideoInfo.PixelFormat.Amask;
			}
		}

		/// <summary>
		/// Red Mask
		/// </summary>
		public static int RedMask
		{
			get
			{
				return VideoInfo.PixelFormat.Rmask;
			}
		}

		/// <summary>
		/// Green Mask
		/// </summary>
		public static int GreenMask
		{
			get
			{
				return VideoInfo.PixelFormat.Gmask;
			}
		}

		/// <summary>
		/// Blue Mask
		/// </summary>
		public static int BlueMask
		{
			get
			{
				return VideoInfo.PixelFormat.Bmask;
			}
		}

		/// <summary>
		/// Binary left shift of blue component
		/// </summary>
		public static int BlueShift
		{
			get
			{
				return VideoInfo.PixelFormat.Bshift;
			}
		}

		/// <summary>
		/// Binary left shift of red component
		/// </summary>
		public static int RedShift
		{
			get
			{
				return VideoInfo.PixelFormat.Rshift;
			}
		}

		/// <summary>
		/// Binary left shift of green component
		/// </summary>
		public static int GreenShift
		{
			get
			{
				return VideoInfo.PixelFormat.Gshift;
			}
		}

		/// <summary>
		/// Binary left shift of alpha component
		/// </summary>
		public static int AlphaShift
		{
			get
			{
				return VideoInfo.PixelFormat.Ashift;
			}
		}

		/// <summary>
		/// Precision loss of alpha component
		/// </summary>
		public static int AlphaLoss
		{
			get
			{
				return VideoInfo.PixelFormat.Aloss;
			}
		}

		/// <summary>
		/// Precision loss of red component
		/// </summary>
		public static int RedLoss
		{
			get
			{
				return VideoInfo.PixelFormat.Rloss;
			}
		}

		/// <summary>
		/// Precision loss of green component
		/// </summary>
		public static int GreenLoss
		{
			get
			{
				return VideoInfo.PixelFormat.Gloss;
			}
		}

		/// <summary>
		/// Precision loss of blue component
		/// </summary>
		public static int BlueLoss
		{
			get
			{
				return VideoInfo.PixelFormat.Bloss;
			}
		}

		/// <summary>
		/// Checks if the video system has a hardware surface
		/// </summary>
		public static bool HasHardwareSurfaces
		{
			get
			{
				return (VideoInfo.VideoInfoStruct.hw_available == 1);
			}
		}

		/// <summary>
		/// checks if window manager is available
		/// </summary>
		public static bool HasWindowManager
		{
			get
			{
				return (VideoInfo.VideoInfoStruct.wm_available == 1);
			}
		}

		/// <summary>
		/// Checks if hardware blits are available
		/// </summary>
		public static bool HasHardwareBlits
		{
			get
			{
				return (VideoInfo.VideoInfoStruct.blit_hw == 1);
			}
		}

		/// <summary>
		/// Checks if hardware colorkey blits are accelerated
		/// </summary>
		public static bool HasHardwareColorKeyBlits
		{
			get
			{
				return (VideoInfo.VideoInfoStruct.blit_hw_CC == 1);
			}
		}

		/// <summary>
		/// Checks if hardware alpha blits are accelerated
		/// </summary>
		public static bool HasHardwareAlphaBlits
		{
			get
			{
				return (VideoInfo.VideoInfoStruct.blit_hw_A == 1);
			}
		}

		/// <summary>
		/// Checks if software to hardware blits are accelerated
		/// </summary>
		public static bool HasSoftwareBlits
		{
			get
			{
				return (VideoInfo.VideoInfoStruct.blit_sw == 1);
			}
		}

		/// <summary>
		/// Checks if software to hardware colorkey blits are accelerated
		/// </summary>
		public static bool HasSoftwareColorKeyBlits
		{
			get
			{
				return (VideoInfo.VideoInfoStruct.blit_sw_CC == 1);
			}
		}

		/// <summary>
		/// Software to hardware alpha blits are accelerated
		/// </summary>
		public static bool HasSoftwareAlphaBlits
		{
			get
			{
				return (VideoInfo.VideoInfoStruct.blit_sw_A == 1);
			}
		}

		/// <summary>
		/// Color fills are accelerated
		/// </summary>
		public static bool HasColorFills
		{
			get
			{
				return (VideoInfo.VideoInfoStruct.blit_fill == 1);
			}
		}

		/// <summary>
		/// Total video memory
		/// </summary>
		public static int VideoMemory
		{
			get
			{
				return VideoInfo.VideoInfoStruct.video_mem;
			}
		}
	}
}
