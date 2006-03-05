using System;
using System.IO;

using Starcraft;
using Gtk;

public class Driver
{
	static MPQ GetMPQ (string path)
	{
		if (Directory.Exists (path))
			return new MPQDirectory (path);
		else if (File.Exists (path))
			return new MPQArchive (path);
		else
			throw new Exception (); // XX
	}

	public static void Main (string[] args)
	{
		Application.Init();

		Game g = new Game (GetMPQ (args[0])); // XXX

		g.Startup();

		g.SetScenario (GetMPQ (args[1])); // XXX

		Application.Run ();
	}
}
