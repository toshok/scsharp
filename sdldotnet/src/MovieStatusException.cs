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
using System.Runtime.Serialization;

namespace SdlDotNet 
{
	/// <summary>
	/// Represents an error resulting from a movie not playing correctly
	/// </summary>
	[Serializable()]
	public class MovieStatusException : SdlException 
	{
		/// <summary>
		/// Represents an error resulting from a movie not playing correctly
		/// </summary>
		public MovieStatusException() 
		{
			MovieStatusException.Generate();
		}

		/// <summary>
		/// Represents an error resulting from a movie not playing correctly
		/// </summary>
		/// <param name="message">Execption string</param>
		public MovieStatusException(string message): base(message)
		{
		}

		/// <summary>
		/// Represents an error resulting from a movie not playing correctly
		/// </summary>
		/// <param name="message">Exception string</param>
		/// <param name="exception"></param>
		public MovieStatusException(string message, Exception exception) : base(message, exception)
		{
		}

		/// <summary>
		/// Represents an error resulting from a movie not playing correctly
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected MovieStatusException(SerializationInfo info, StreamingContext context) : base( info, context )
		{
		}
	}
}
