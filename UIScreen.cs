
using System;
using System.IO;
using System.Threading;

using Gtk;
using Gdk;

namespace Starcraft
{
	public delegate void ReadyDelegate ();

	public class UIScreen
	{
		Gdk.Pixbuf background;
		CursorAnimator cursor;
		UIPainter ui_painter;

		bool swooshHandler ()
		{
			RaiseDoneSwooshing ();
			return false;
		}

		public virtual void SwooshIn ()
		{
			GLib.Idle.Add (swooshHandler);
		}

		public virtual void SwooshOut ()
		{
			GLib.Idle.Add (swooshHandler);
		}

		public virtual void Load ()
		{
		}

		public virtual void AddToPainter (Painter painter)
		{
			if (background != null) {
				Console.WriteLine ("adding ui background to painter");
				painter.Add (Layer.Background, BackgroundPainter);
			}

			if (ui_painter != null) {
				Console.WriteLine ("adding ui painter to painter");
				painter.Add (Layer.UI, ui_painter.Paint);
			}

			if (cursor != null) {
				Console.WriteLine ("setting game cursor to ui cursor");
				Game.Instance.Cursor = cursor;
			}
		}

		public virtual void RemoveFromPainter (Painter painter)
		{
			if (background != null)
				painter.Remove (Layer.Background, BackgroundPainter);
			if (ui_painter != null)
				painter.Remove (Layer.UI, ui_painter.Paint);
			if (cursor != null)
				Game.Instance.Cursor = null;
		}

		public event DoneSwooshingDelegate DoneSwooshing;
		public event ReadyDelegate Ready;

		protected void RaiseReadyEvent ()
		{
			if (Ready != null)
				Ready ();
		}

		protected void RaiseDoneSwooshing ()
		{
			if (DoneSwooshing != null)
				DoneSwooshing ();
		}

		protected Gdk.Pixbuf Background {
			get { return background; }
			set { background = value; }
		}

		protected CursorAnimator Cursor {
			get { return cursor; }
			set { cursor = value; }
		}

		protected UIPainter UIPainter {
			get { return ui_painter; }
			set { ui_painter = value; }
		}

		protected void BackgroundPainter (Gdk.Pixbuf pb, DateTime dt)
		{
			background.Composite (pb, 0, 0, background.Width, background.Height,
					      0, 0, 1, 1, InterpType.Nearest, 0xff);
		}
	}

}
