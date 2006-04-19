using System;
using System.IO;
using System.Configuration;

using SCSharp;

using SdlDotNet;

public class Driver
{
	public static void Main (string[] args)
	{
		bool fullscreen = false;

		Game g = new Game (ConfigurationManager.AppSettings["StarcraftDirectory"],
				   ConfigurationManager.AppSettings["CDDirectory"]);

		if (args.Length > 0)
			if (args[0] == "/fullscreen")
				fullscreen = true;

		g.Startup(fullscreen);
	}
}
