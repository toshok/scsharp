using System;
using System.IO;
using System.Configuration;

using Starcraft;

using SdlDotNet;

public class Driver
{
	public static void Main (string[] args)
	{
		Game g = new Game (ConfigurationManager.AppSettings["StarcraftDirectory"]);

		g.Startup();
	}
}
