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
using System.Reflection;
using System.Diagnostics;

using SdlDotNet;

namespace SdlDotNet.Examples.SimpleGame
{
	/// <summary>
	/// Summary description for Game.
	/// </summary>
	public sealed class GameMain
	{
		GameMain()
		{}

		[STAThread]
		static void Main() 
		{
			if (CheckInstance() == null)
			{
				EventManager eventManager = new EventManager();
				InputController inputController = 
					new InputController(eventManager);
				Game game = new Game(eventManager);
				game.Start();
				inputController.Run();
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static Process CheckInstance()
		{ 
			Process current = Process.GetCurrentProcess(); 
			Process[] processes = Process.GetProcessesByName (current.ProcessName); 

			//Loop through the running processes in with the same name 
			foreach (Process process in processes) 
			{ 
				//Ignore the current process 
				if (process.Id != current.Id) 
				{ 
					//Make sure that the process is running from the exe file. 
					if (Assembly.GetExecutingAssembly().Location.
						Replace("/", "\\") == current.MainModule.FileName) 
					{  
						//Return the other process instance.  
						return process;  
					}  
				}  
			} 
			//No other instance was found, return null.  
			return null;  
		}
	}
}
