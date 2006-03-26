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
using System.Runtime.InteropServices;
//using System.Globalization;
using System.Reflection;
using System.Resources;

using Tao.Sdl;

namespace SdlDotNet 
{
	/// <summary>
	/// Contains methods for playing audio CDs.
	/// Obtain an instance of this class by accessing 
	/// the CDAudio property of the main Sdl object
	/// </summary>
	/// <remarks>
	/// Contains methods for playing audio CDs.
	/// </remarks>
	public sealed class CDRom
	{
//		static ResourceManager stringManager;
		CDRom()
		{}

		static CDRom()
		{
			Initialize();
		}

		/// <summary>
		/// Closes and destroys this object
		/// </summary>
		/// <remarks></remarks>
		public static void Close() 
		{
			Events.CloseCDRom();
		}

		/// <summary>
		/// Starts the CDRom subsystem. 
		/// </summary>
		/// <remarks>
		/// This normally automatically started when 
		/// the CDRom class is initialized.
		/// </remarks>
		public static void Initialize()
		{
			if ((Sdl.SDL_WasInit(Sdl.SDL_INIT_CDROM)) 
				== (int) SdlFlag.FalseValue)
			{
				if (Sdl.SDL_Init(Sdl.SDL_INIT_CDROM) != (int) SdlFlag.Success)
				{
					throw SdlException.Generate();
				}
			}
		}

//		/// <summary>
//		/// Queries if the CDRom subsystem has been intialized.
//		/// </summary>
//		/// <remarks>
//		/// </remarks>
//		/// <returns>
//		/// True if CDRom subsystem has been initialized, false if it has not.
//		/// </returns>
//		public static bool IsInitialized
//		{
//			get
//			{
//				if ((Sdl.SDL_WasInit(Sdl.SDL_INIT_CDROM) & Sdl.SDL_INIT_CDROM) 
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
		/// Gets the number of CD-ROM drives available on the system
		/// </summary>
		/// <remarks>
		/// </remarks>
		public static int NumberOfDrives 
		{
			get 
			{
				int ret = Sdl.SDL_CDNumDrives();
				if (ret == (int) CDStatus.Error)
				{
					throw SdlException.Generate();
				}
				return ret;
			}
		}

		/// <summary>
		/// Checks if the drive number uis valid
		/// </summary>
		/// <param name="index">drive number</param>
		/// <returns>
		/// true is the number is greater than 0 and less 
		/// than the number of drives on the system.
		/// </returns>
		public static bool IsValidDriveNumber(int index)
		{
			if (index >= 0 && index < NumberOfDrives)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Opens a CD-ROM drive for manipulation
		/// </summary>
		/// <param name="index">
		/// The number of the drive to open, from 0 - CDAudio.NumDrives
		/// </param>
		/// <returns>
		/// The CDDrive object representing the CD-ROM drive
		/// </returns>
		/// <remarks>
		/// Opens a CD-ROM drive for manipulation
		/// </remarks>
		public static CDDrive OpenDrive(int index) 
		{
			IntPtr cd = Sdl.SDL_CDOpen(index);
			if (!IsValidDriveNumber(index) || (cd == IntPtr.Zero))
			{
				throw SdlException.Generate();
			}
			return new CDDrive(cd, index);
		}
		/// <summary>
		/// Returns a platform-specific name for a CD-ROM drive
		/// </summary>
		/// <param name="index">The number of the drive</param>
		/// <returns>A platform-specific name, i.e. "D:\"</returns>
		/// <remarks>
		/// </remarks>
		public static string DriveName(int index) 
		{
			if (!IsValidDriveNumber(index))
			{
//				throw new SdlException(stringManager.GetString("Device index out of range", CultureInfo.CurrentUICulture));
				throw new SdlException("Device index out of range");
			}
			return Sdl.SDL_CDName(index);
		}
	}
}
