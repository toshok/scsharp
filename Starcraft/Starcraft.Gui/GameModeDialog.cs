using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class GameModeDialog : UIDialog
	{
		public GameModeDialog (Mpq mpq) : base (mpq, "glue\\Palmm", Builtins.rez_GluGameModeBin)
		{
			background_path = "glue\\Palmm\\retail_ex.pcx";
		}

		const int ORIGINAL_ELEMENT_INDEX = 1;
		const int CANCEL_ELEMENT_INDEX = 4;
		const int EXPANSION_ELEMENT_INDEX = 3;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Elements[ORIGINAL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Activated != null)
						Activated (false);
				};

			Elements[EXPANSION_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Activated != null)
						Activated (true);
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Canceled != null)
						Canceled ();
				};

			// notify we're ready to roll
			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
		}

		public event DialogEvent Canceled;
		public event GameModeActivatedDelegate Activated;
	}

	public delegate void GameModeActivatedDelegate (bool expansion);
}
