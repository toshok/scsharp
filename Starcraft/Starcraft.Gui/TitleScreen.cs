using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class TitleScreen : UIScreen
	{
		public TitleScreen (Mpq mpq) : base (mpq)
		{
		}

		Surface background;
		Bin titleBin;
		UIPainter titlePainter;

		protected override void ResourceLoader (object state)
		{
			try {
				SdlDotNet.Timer.DelayTicks (50);
				Console.WriteLine ("loading title screen background");
				background = GuiUtil.SurfaceFromStream ((Stream)mpq.GetResource (Builtins.TitlePcx));
				Console.WriteLine ("loading title screen ui elements");
				titleBin = (Bin)mpq.GetResource (Builtins.rez_TitleDlgBin);

				titlePainter = new UIPainter (titleBin, mpq);

				// notify we're ready to roll
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
			}
			catch (Exception e) {
				Console.WriteLine ("Resource loader for Title Screen failed: {0}", e);
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (Events.QuitApplication)));
			}
		}

		protected override void FinishedLoading ()
		{
			Background = background;
			UI = titleBin;
			UIPainter = titlePainter;

			base.FinishedLoading ();
		}
	}
}
