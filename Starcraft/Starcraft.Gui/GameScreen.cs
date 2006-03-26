using System;
using System.IO;
using System.Threading;

using Gtk;
using Gdk;

namespace Starcraft {

	public class GameScreen : UIScreen {
		MPQ mpq;
		MPQ scenario_mpq;

		Gdk.Pixbuf hud;
		CHK scenario;

		public GameScreen (MPQ mpq, MPQ scenario_mpq)
		{
			this.mpq = mpq;
			this.scenario_mpq = scenario_mpq;
		}

		public override void Load ()
		{
			ThreadPool.QueueUserWorkItem (ResourceLoader);
		}

		public override void AddToPainter (Painter painter)
		{
			painter.Add (Layer.Hud,
				     delegate (Gdk.Pixbuf pb, DateTime dt) {
					hud.Composite (pb, 0, 0, hud.Width, hud.Height,
						      0, 0, 1, 1, InterpType.Nearest, 0xff); });
		}

		void ResourceLoader (object state)
		{
			Console.WriteLine ("loading hud");
			hud = new Gdk.Pixbuf ((Stream)mpq.GetResource (String.Format (Builtins.Game_ConsolePcx, Util.RaceCharLower[(int)Game.Instance.Race])));

			Console.WriteLine ("loading scenario.chk");
			scenario = (CHK) scenario_mpq.GetResource ("scenario.chk");

			// notify we're ready to roll
			(new ThreadNotify (new ReadyEvent (FinishedLoading))).WakeupMain ();
		}

		void FinishedLoading ()
		{
			// emit the Ready event
			RaiseReadyEvent ();
		}
	}
}
