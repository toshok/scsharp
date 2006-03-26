
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
using System.Runtime.Serialization;

namespace SdlDotNet.Examples.Triad
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable()]
	public class GameException : SdlException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GameException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public GameException(string message) : base(message)
		{
			if(message == null)
			{
				throw new ApplicationException("Please provide a message.");
			}
		}

		/// <summary>
		/// Basic Exception
		/// </summary>
		public GameException()
		{
			GameException.Generate();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		public GameException(string message, Exception exception) : base(message, exception)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected GameException(SerializationInfo info, StreamingContext context) : base( info, context )
		{
		}
	}
}
