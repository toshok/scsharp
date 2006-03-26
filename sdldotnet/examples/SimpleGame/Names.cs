// Copyright 2005 David Hudson (jendave@yahoo.com)
// This file is part of SimpleGame.
//
// SimpleGame is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// SimpleGame is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SimpleGame; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;

namespace SdlDotNet.Examples.SimpleGame
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public sealed class Names
	{
		static string windowCaption = "SDL.NET - Simple Game";
		static string logFile = "SimpleGame.log";
		static string logFile2 = "SimpleGame2.log";
		static string logFile3 = "SimpleGame3.log";
		static string startingSimpleGame = "Starting SimpleGame.";
		static string quittingSimpleGame = "Quitting SimpleGame.";
		static int startingSector = 0;

		Names()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public static string WindowCaption
		{
			get
			{
				return windowCaption;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static string LogFile
		{
			get
			{
				return logFile;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static string StartingSimpleGame
		{
			get
			{
				return startingSimpleGame;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static string QuittingSimpleGame
		{
			get
			{
				return quittingSimpleGame;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static int StartingSector
		{
			get
			{
				return startingSector;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static string LogFile2
		{
			get
			{
				return logFile2;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public static string LogFile3
		{
			get
			{
				return logFile3;
			}
		}
	}
}
