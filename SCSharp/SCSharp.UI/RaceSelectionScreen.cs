using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
{
	public class RaceSelectionScreen : UIScreen
	{
		public RaceSelectionScreen (Mpq mpq) : base (mpq, "glue\\PalNl", Builtins.rez_GluExpcmpgnBin)
		{
			background_path = null;
		}

		const int LOADSAVED_ELEMENT_INDEX = 3;
		const int ZERG_ELEMENT_INDEX = 4;
		const int PROTOSS_ELEMENT_INDEX = 5;
		const int TERRAN_ELEMENT_INDEX = 6;
		const int CANCEL_ELEMENT_INDEX = 7;
		const int PLAYCUSTOM_ELEMENT_INDEX = 8;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (UIScreenType.Login);
				};

			Elements[LOADSAVED_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (new LoadSavedScreen (mpq));
				};

			Elements[PLAYCUSTOM_ELEMENT_INDEX].Activate += 
				delegate () {
					Game.Instance.SwitchToScreen (new PlayCustomScreen (mpq));
				};
		}
	}
}
