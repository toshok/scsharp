using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class TitleScreen : UIScreen
	{
		public TitleScreen (Mpq mpq) : base (mpq, "glue\\Palmm", Builtins.rez_TitleDlgBin)
		{
			background_path = Builtins.TitlePcx;
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();
			Cursor = null; /* clear out the cursor */

			// notify we're ready to roll
			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
		}
	}
}
