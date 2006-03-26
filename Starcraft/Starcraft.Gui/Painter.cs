using System;
using System.Collections.Generic;
using Gtk;
using Gdk;

namespace Starcraft {

	public enum Layer {
		Background,
		Map,
		Shadow,
		Selection,
		Unit,
		Health,
		Hud,
		UI,
		Tooltip,
		Cursor,

		Count
	}

	public delegate void PainterDelegate (Gdk.Pixbuf pb, DateTime now);

	class TextElement {
		public readonly int x;
		public readonly int y;
		public readonly Pango.Layout layout;

		public TextElement (int x, int y, Pango.Layout layout)
		{
			this.x = x;
			this.y = y;
			this.layout = layout;
		}
	}

	public class Painter 
	{
		List<PainterDelegate>[] layers;
		Gtk.DrawingArea da;
		Gdk.Pixbuf pb;
		DateTime now; /* the time of the last animation tick */
		Gdk.GC text_gc;

		public Painter (Gtk.DrawingArea da, uint millis)
		{
			this.da = da;
			
			pb = new Gdk.Pixbuf (Colorspace.Rgb, true, 8,
					     da.Allocation.Width, da.Allocation.Height);

			/* init our list of painter delegates */
			layers = new List<PainterDelegate>[(int)Layer.Count];
			for (Layer i = Layer.Background; i < Layer.Count; i ++) {
				layers[(int)i] = new List<PainterDelegate>();
			}

			/* create the text GC on realize */
			da.Realized += OnRealize;

			/* immediately redraw everything on an expose */
			da.ExposeEvent += OnExpose;

			/* catch resizes of our drawing area so we can resize our pixbuf */
			da.SizeAllocated += OnSizeAllocated;

			/* and set ourselves up to invalidate at a regular interval*/
			GLib.Timeout.Add (millis, Animate);
		}

		public void Add (Layer layer, PainterDelegate painter)
		{
			layers[(int)layer].Add (painter);
			da.QueueDraw ();
		}

		public void Remove (Layer layer, PainterDelegate painter)
		{
			layers[(int)layer].Remove (painter);
			da.QueueDraw ();
		}

		public void Clear (Layer layer)
		{
			layers[(int)layer].Clear ();
			da.QueueDraw();
		}

		List<TextElement> texts;

		public void DrawText (int x, int y, string text)
		{
			Pango.Layout layout = da.CreatePangoLayout (text);
			texts.Add (new TextElement (x, y, layout));
		}

		public void QueueRedraw ()
		{
			da.QueueDraw ();
		}

		bool Animate ()
		{
			now = DateTime.Now;

			pb.Fill (0x000000ff);

			texts = new List<TextElement> ();

			for (Layer i = Layer.Background; i < Layer.Count; i ++)
				DrawLayer (layers[(int)i]);

			da.QueueDraw ();

			return true;
		}

		void DrawLayer (List<PainterDelegate> painters)
		{
			foreach (PainterDelegate p in painters)
				p (pb, now);
		}

		void OnSizeAllocated (object o, SizeAllocatedArgs args)
		{
			pb = new Gdk.Pixbuf (Colorspace.Rgb, true, 8,
					     da.Allocation.Width, da.Allocation.Height);
			pb.Fill (0x000000ff);
		}

		void OnRealize (object o, EventArgs e)
		{
			text_gc = new Gdk.GC (da.GdkWindow);

			Gdk.Color green_color = new Gdk.Color (0, 0xff, 0);
			Gdk.Colormap colormap = Gdk.Colormap.System;

			colormap.AllocColor (ref green_color, true, true);

			text_gc.Foreground = green_color;
		}

		void OnExpose (object o, EventArgs e)
		{
			pb.RenderToDrawable (da.GdkWindow, da.Style.ForegroundGC (StateType.Normal),
					     0, 0,
					     0, 0,
					     -1, -1,
					     RgbDither.None, 0, 0);

			if (texts != null)
				foreach (TextElement te in texts)
					da.GdkWindow.DrawLayout (text_gc,
								 te.x, te.y, te.layout);
		}

	}
}
