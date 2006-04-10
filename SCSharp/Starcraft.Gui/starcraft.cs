using System;
using System.IO;

using Starcraft;

using SdlDotNet;

public class Driver
{
	static Mpq GetMpq (string path)
	{
		if (Directory.Exists (path))
			return new MpqDirectory (path);
		else if (File.Exists (path))
			return new MpqArchive (path);
		else
			throw new Exception (); // XX
	}

	public static void Main (string[] args)
	{
		Application.Init();

		Game g = new Game (GetMpq (args[0])); // XXX

		g.Startup();

		g.SetScenario (GetMpq (args[1])); // XXX

		Application.Run ();
	}
}
