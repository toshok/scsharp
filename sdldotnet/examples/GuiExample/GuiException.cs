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

using System;
using System.Runtime.Serialization;

using SdlDotNet;

namespace SdlDotNet.Examples.GuiExample
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable()]
	public class GuiException : SdlException
	{
		/// <summary>
		/// 
		/// </summary>
		public GuiException()
		{
			// Add any type-specific logic, and supply the default message.
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public GuiException(string message): base(message) 
		{
			// Add any type-specific logic.
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public GuiException(string message, Exception innerException): base (message, innerException)
		{
			// Add any type-specific logic for inner exceptions.
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected GuiException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// Implement type-specific serialization constructor logic.
		}

	}
}
