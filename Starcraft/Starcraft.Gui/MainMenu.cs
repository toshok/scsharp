using System;
using System.IO;
using System.Threading;

using Gtk;
using Gdk;

namespace Starcraft
{
	public class MainMenu : UIScreen
	{
		Mpq mpq;
		Bin ui;

		public MainMenu (Mpq mpq)
		{
			this.mpq = mpq;
		}

		public override void Load ()
		{
			ThreadPool.QueueUserWorkItem (ResourceLoader);
		}

		CursorAnimator cursor;
		Gdk.Pixbuf background;

		void ResourceLoader (object state)
		{
			Console.WriteLine ("loading arrow cursor");
			cursor = new CursorAnimator ((Grp)mpq.GetResource (Builtins.Palmm_ArrowGrp));
			cursor.SetHotSpot (64, 64);
			Console.WriteLine ("loading main menu background");
			background = new Gdk.Pixbuf ((Stream)mpq.GetResource (Builtins.Palmm_BackgroundPcx));
			Console.WriteLine ("loading main menu ui elements");
			ui = (Bin)mpq.GetResource (Builtins.rez_GluMainBin);

			/* resolve external entities */
			foreach (UIElement e in ui.Elements) {
				if (e.type == UIElementType.Image)
					e.resolvedData = new Gdk.Pixbuf ((Stream)mpq.GetResource (e.text));
			}

			// notify we're ready to roll
			(new ThreadNotify (new ReadyEvent (FinishedLoading))).WakeupMain ();
		}

		void FinishedLoading ()
		{
			Background = background;
			Cursor = cursor;
			UIPainter = new UIPainter (ui);

			// emit the Ready event
			RaiseReadyEvent ();
		}
	}
}
