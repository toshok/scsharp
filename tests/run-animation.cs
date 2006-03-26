
using System;
using System.IO;

using Starcraft;

using Gtk;
using Gdk;
using GLib;

public class RunAnimation {

	Painter painter;
	Gtk.Window window;
	Gtk.DrawingArea drawing_area;

	int sprite_number = 146;
	int animation_type = 11;

	MPQ mpq;
	Sprite sprite;

	public RunAnimation (MPQ mpq)
	{
		this.mpq = mpq;

		GlobalResources globals = new GlobalResources (mpq);
		globals.Ready += GlobalsReady;

		globals.Load ();
		
		CreateWindow ();
	}

	void CreateWindow ()
	{
		window = new Gtk.Window ("animation viewer");

		drawing_area = new Gtk.DrawingArea ();

		painter = new Painter (drawing_area, 30);

		SpriteManager.AddToPainter (painter);

		window.Add (drawing_area);
		window.ShowAll ();
	}

        bool die ()
	{
		sprite.RunAnimation (1);
		return false;
	}

	void GlobalsReady ()
	{
		sprite = SpriteManager.CreateSprite (mpq, sprite_number);
		sprite.RunAnimation (animation_type);

		GLib.Timeout.Add (10000, die);
	}

	public static void Main (string[] args) {
		Application.Init();

		MPQ mpq = new MPQDirectory (args[0]);

		RunAnimation la = new RunAnimation (mpq);

		Application.Run ();
	}
}
