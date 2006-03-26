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
using System.Runtime.Serialization;

using Tao.Sdl;

namespace SdlDotNet 
{
	/// <summary>
	/// Represents a run-time error from the Sdl library.
	/// </summary>
	[Serializable()]
	public class SdlException : Exception 
	{
		/// <summary>
		/// Returns basic exception
		/// </summary>
		public SdlException() 
		{
			SdlException.Generate();
		}
		/// <summary>
		/// Initializes an SdlException instance
		/// </summary>
		/// <param name="message">
		/// The string representing the error message
		/// </param>
		public SdlException(string message): base(message)
		{
		}

		/// <summary>
		/// Returns exception
		/// </summary>
		/// <param name="message">Exception message</param>
		/// <param name="exception">Exception type</param>
		public SdlException(string message, Exception exception) : base(message, exception)
		{
		}

		/// <summary>
		/// Returns SerializationInfo
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SdlException(SerializationInfo info, StreamingContext context) : base( info, context )
		{
		}

		/// <summary>
		/// Generates an SdlException based on the last Sdl Error code.
		/// </summary>
		/// <returns>
		/// A new SdlException object
		/// </returns>
		public static SdlException Generate() 
		{
			string msg = Sdl.SDL_GetError();

			if (msg.IndexOf("Surface was lost") == -1)
			{
				return new SdlException(msg);
			}
			else
			{
				return new SurfaceLostException(msg);
			}
		}

		/// <summary>
		/// Returns SDL error.
		/// </summary>
		public static string GetError
		{
			get
			{
				return Sdl.SDL_GetError();
			}
		}
	}
}
