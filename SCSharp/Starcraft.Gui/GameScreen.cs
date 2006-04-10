using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft {

	public class GameScreen : UIScreen {
		Mpq scenario_mpq;

		Surface hud;
		Chk scenario;

		public GameScreen (Mpq mpq, Mpq scenario_mpq) : base (mpq)
		{
			this.scenario_mpq = scenario_mpq;
		}

		public override void AddToPainter (Painter painter)
		{
			painter.Add (Layer.Hud,
				     delegate (Surface surf, DateTime dt) {
					surf.Blit (hud); } );
		}

		protected override void ResourceLoader ()
		{
			hud = GuiUtil.SurfaceFromStream ((Stream)mpq.GetResource (String.Format (Builtins.Game_ConsolePcx, Util.RaceCharLower[(int)Game.Instance.Race])));

			Console.WriteLine ("loading scenario.chk");
			scenario = (Chk) scenario_mpq.GetResource ("scenario.chk");

			
			// notify we're ready to roll
			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
		}

		protected override void FinishedLoading ()
		{
			base.FinishedLoading ();
		}
	}
}
