using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class ReadyRoomScreen : UIScreen
	{
		public ReadyRoomScreen (Mpq mpq) : base (mpq, "glue\\ReadyT", Builtins.rez_GluRdyTBin)
		{
			background_path = null;
			fontpal_path = "glue\\Palmm\\tFont.pcx";
		}
		
		const int START_ELEMENT_INDEX = 1;
		const int CANCEL_ELEMENT_INDEX = 9;
		const int SKIPTUTORIAL_ELEMENT_INDEX = 11;
		const int REPLAY_ELEMENT_INDEX = 12;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (UIScreenType.Login);
				};
		}
	}
}
