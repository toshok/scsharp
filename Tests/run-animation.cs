
using System;
using System.IO;

using Starcraft;

using SdlDotNet;
using SdlDotNet.Sprites;

public class RunAnimation {

	int sprite_number = 146;
	int animation_type = 11;

	Mpq mpq;
	Starcraft.Sprite sprite;
	Painter painter;

	public RunAnimation (Mpq mpq)
	{
		this.mpq = mpq;

		GlobalResources globals = new GlobalResources (mpq);
		//		globals.Ready += GlobalsReady;

		globals.Load ();
		
		CreateWindow ();

		Timer.DelaySeconds (5);

		GlobalsReady ();
	}

	void CreateWindow ()
	{
		Video.WindowIcon ();
		Video.WindowCaption = "animation viewer";
		Video.SetVideoModeWindow (320, 200);

		Mouse.ShowCursor = false;

		painter = new Painter (100);
		SpriteManager.AddToPainter (painter);
	}

        bool die ()
	{
		sprite.RunAnimation (1);
		return false;
	}

	void GlobalsReady ()
	{
		Console.WriteLine ("GlobalsReady");
		sprite = SpriteManager.CreateSprite (mpq, sprite_number);
		sprite.RunAnimation (animation_type);

		//		Timer.DelaySeconds (3);

		//		GLib.Timeout.Add (10000, die);
	}

	void Quit (object sender, QuitEventArgs e)
	{
		Events.QuitApplication();
	}

	public static void Main (string[] args) {
		Mpq mpq = new MpqDirectory (args[0]);

		RunAnimation la = new RunAnimation (mpq);

		Events.Quit += Quit;
		Events.Run ();
	}
}
