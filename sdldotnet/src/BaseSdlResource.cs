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

using Tao.Sdl;

namespace SdlDotNet 
{
	/// <summary>
	/// Base class for SdlResources
	/// </summary>
	/// <remarks>
	/// Several SdlDotNet classes inherit from this class
	/// </remarks>
	public abstract class BaseSdlResource : IDisposable 
	{
		private bool disposed;
		private IntPtr handle;

		/// <summary>
		/// Creates class using a handle.
		/// </summary>
		/// <param name="handle">
		/// Pointer to unmanaged Sdl resource
		/// </param>
		/// <remarks>
		/// Often used by internal classes.
		/// </remarks>
		protected BaseSdlResource(IntPtr handle) 
		{
			this.handle = handle;
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks>
		/// Used by outside classes.
		/// </remarks>
		protected BaseSdlResource() 
		{
		}

		/// <summary>
		/// Allows an Object to attempt to free resources 
		/// and perform other cleanup operations before the Object 
		/// is reclaimed by garbage collection.
		/// </summary>
		/// <remarks>
		/// Frees managed resources
		/// </remarks>
		~BaseSdlResource() 
		{
			Dispose(false);
		}

		/// <summary>
		/// 
		/// </summary>
		protected internal IntPtr Handle
		{
			get
			{
				return this.handle;
			}
			set
			{
				this.handle = value;
				GC.KeepAlive(this);
			}
		}

		/// <summary>
		/// Closes and destroys this object
		/// </summary>
		/// <remarks>
		/// Destroys managed and unmanaged objects
		/// </remarks>
		public void Dispose() 
		{
			Dispose(true);
			GC.SuppressFinalize(this); 
		}

		/// <summary>
		/// Dispose objects
		/// </summary>
		/// <param name="disposing">
		/// If true, it will dispose close the handle
		/// </param>
		/// <remarks>
		/// Will dispose managed and unmanaged resources.
		/// </remarks>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
				}
				CloseHandle();
				//handle = IntPtr.Zero;
			}
			this.disposed = true;
		}

		/// <summary>
		/// Close the handle.
		/// </summary>
		/// <remarks>
		/// Used to close handle to unmanaged SDL resources
		/// </remarks>
		protected abstract void CloseHandle();
		
		/// <summary>
		/// Closes and destroys this object
		/// </summary>
		/// <remarks>
		/// Same as Dispose(true)
		/// </remarks>
		public void Close() 
		{
			Dispose();
		}
	}
}
