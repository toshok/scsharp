
using System;
using System.IO;

using Starcraft;

using Gtk;
using Gdk;
using GLib;

public class AnimateGRP {
	static Gtk.Window window;
	static Gtk.Widget drawing_area;

	static GRP grp;

	const int WALK_CYCLE_START = 136;
	const int WALK_CYCLE_END = 255;

	const int DEATH_CYCLE_START = 408;
	const int DEATH_CYCLE_END = 414;
	
	const int FRAME_STEP = 17;

	static bool walking = true;
	static int num_walks = 0;
	static int death_timer = 0;

	static Gdk.Pixbuf current_frame;
	static int current_frame_num = WALK_CYCLE_START;

	static void CreateWindow ()
	{
		window = new Gtk.Window ("grp animation");

		drawing_area = new Gtk.DrawingArea ();
		window.Add (drawing_area);
		window.ShowAll ();
	}

	static void OnExposed (object o, ExposeEventArgs args)
	{
		if (current_frame == null)
			return;
		current_frame.RenderToDrawable (drawing_area.GdkWindow, drawing_area.Style.ForegroundGC (StateType.Normal),
						0, 0,
						0, 0,
						-1, -1,
						RgbDither.None, 0, 0);
	}

	static byte[] CreatePixbufData (byte[,] grid, ushort width, ushort height, byte[] palette)
	{
		byte[] rv = new byte[width * height * 3];
		int i = 0;
		int x, y;

		for (y = height - 1; y >= 0; y --) {
			for (x = width - 1; x >= 0; x--) {
				rv[i++] = palette[ grid[y,x] * 3 ];
				rv[i++] = palette[ grid[y,x] * 3 + 1];
				rv[i++] = palette[ grid[y,x] * 3 + 2];
			}
		}

		return rv;
	}

	static bool Animate ()
	{
		if (walking) {
			current_frame_num += FRAME_STEP;

			if (current_frame_num > WALK_CYCLE_END) {
				num_walks++;
				if (num_walks > 10) {
					num_walks = 0;
					walking = false;
					current_frame_num = DEATH_CYCLE_START;
				}
				else {
					current_frame_num = WALK_CYCLE_START;
				}
			}
		}
		else /* dying */ {
			if (current_frame_num == DEATH_CYCLE_END) {
				death_timer ++;
				if (death_timer == 30) {
					death_timer = 0;
					walking = true;
					current_frame_num = WALK_CYCLE_START;
				}
			}
			else
				current_frame_num ++;
		}

		byte[] pixbuf_data = CreatePixbufData (grp.GetFrame (current_frame_num),
						       grp.Width, grp.Height,
						       Palette.default_palette);

		Gdk.Pixbuf temp = new Gdk.Pixbuf (pixbuf_data,
						  Colorspace.Rgb,
						  false,
						  8,
						  grp.Width, grp.Height,
						  grp.Width * 3,
						  null);

		current_frame = temp.ScaleSimple (grp.Width * 2, grp.Height * 2, InterpType.Nearest);

		temp.Dispose();

		drawing_area.QueueDraw ();

		return true;
	}

	public static void Main (string[] args) {
		Application.Init();

		string filename = args[0];

		Console.WriteLine ("grp file {0}", filename);

		FileStream fs = File.OpenRead (filename);

		grp = new GRP ();

		((MPQResource)grp).ReadFromStream (fs);

		CreateWindow ();
		drawing_area.ExposeEvent += OnExposed;

		GLib.Timeout.Add (100, Animate);

		Application.Run ();
	}

}
