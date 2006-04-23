using System;
using System.IO;
using System.Threading;

using Gtk;
using Gdk;

namespace SCSharp.UI
{
	public class ScoreScreen : UIScreen
	{
		SwooshPainter swoosher;
		Painter painter;
		CursorAnimator cursor;
		Gdk.Pixbuf background;
		Bin ui;
		Mpq mpq;
		bool translucent;

		Gdk.Pixbuf pMainPb;

		public ScoreScreen (Mpq mpq)
		{
			this.mpq = mpq;
		}

		void DoneSwooshingIn ()
		{
			Console.WriteLine ("Done Swooshing In");
			Game.Instance.Cursor = cursor;

			painter.Remove (Layer.UI, swoosher.Paint);
			painter.Add (Layer.UI, UIPainter.Paint);
			RaiseDoneSwooshing();
		}

		void pMainPainter (Gdk.Pixbuf pb, DateTime now)
		{
			if (translucent)
				Console.WriteLine ("translucent!");
			pMainPb.Composite (pb, 0, 0, pMainPb.Width, pMainPb.Height,
					   0, 0, 1, 1, InterpType.Nearest, 0xff);
		}

		public override void SwooshIn ()
		{
			swoosher = new SwooshPainter (SwooshPainter.Direction.FromLeft,
						      pMainPainter,
						      0, 0, translucent ? (int)(0xff * 0.75) : 0xff);
			swoosher.DoneSwooshing += DoneSwooshingIn;
			painter.Add (Layer.UI, swoosher.Paint);
		}

		public override void SwooshOut ()
		{
			base.SwooshOut();
		}

		public override void AddToPainter (Painter painter)
		{
			this.painter = painter;
			painter.Add (Layer.Background, BackgroundPainter);
		}

		public override void RemoveFromPainter (Painter painter)
		{
			base.RemoveFromPainter (painter);
		}

		protected override void ResourceLoader (object state)
		{
			Console.WriteLine ("loading arrow cursor");
			cursor = new CursorAnimator ((Grp)mpq.GetResource (String.Format (Builtins.Palv_ArrowGrp, Util.RaceChar[(int)Game.Instance.Race])));
			cursor.SetHotSpot (64, 64);
			Console.WriteLine ("loading score background");
			background = new Gdk.Pixbuf ((Stream)mpq.GetResource (String.Format (Builtins.Palv_BackgroundPcx, Util.RaceChar[(int)Game.Instance.Race])));
			Console.WriteLine ("loading score screen ui elements");
			ui = (Bin)mpq.GetResource (Builtins.rez_GluScoreBin);

			if (ui.Elements[1].type != UIElementType.Image)
				throw new Exception ();
			ui.Elements[1].text = String.Format (Builtins.Scorev_pMainPcx, Util.RaceChar[(int)Game.Instance.Race]);

			/* resolve external entities */
			foreach (UIElement e in ui.Elements) {
				if (e.type == UIElementType.Image) {
					//XXX
					if (e.text != null && e.text != "") {
						Console.WriteLine ("loading image {0}", e.text);
						e.resolvedData = new Surface (Bitmap.FromStream ((Stream)mpq.GetResource (e.text)));
					}
				}
			}

			ui.Elements[1].DumpFlags();

			if ((ui.Elements[1].flags & UIElementFlags.ApplyTranslucency) == UIElementFlags.ApplyTranslucency)
				translucent = true;
			pMainPb = (Gdk.Pixbuf)ui.Elements[1].resolvedData;
		}

		protected override void FinishedLoading ()
		{
			Background = background;
			UIPainter = new UIPainter (ui);

			base.FinishedLoading ();
		}
	}
}
