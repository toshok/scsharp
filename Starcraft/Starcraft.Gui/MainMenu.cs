using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class MainMenu : UIScreen
	{
		public MainMenu (Mpq mpq) : base (mpq, "glue\\Palmm", Builtins.rez_GluMainBin)
		{
		}

		const int EXIT_ELEMENT_INDEX = 2;
		const int SINGLEPLAYER_ELEMENT_INDEX = 3;
		const int MULTIPLAYER_ELEMENT_INDEX = 4;
		const int CREDITS_ELEMENT_INDEX = 9;
		const int VERSION_ELEMENT_INDEX = 10;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Elements[VERSION_ELEMENT_INDEX].Text = "v0.0000000001";

			Elements[SINGLEPLAYER_ELEMENT_INDEX].Activate +=
				delegate () {
					GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
					Game.Instance.SwitchToScreen (UIScreenType.Login);
				};

			Elements[MULTIPLAYER_ELEMENT_INDEX].Activate +=
				delegate () {
					GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
					Game.Instance.SwitchToScreen (UIScreenType.Connection);
				};

			Elements[CREDITS_ELEMENT_INDEX].Activate += 
				delegate () {
					Game.Instance.SwitchToScreen (new CreditsScreen (mpq));
				};

			Elements[EXIT_ELEMENT_INDEX].Activate += 
				delegate () {
					Game.Instance.Quit ();
				};


			// notify we're ready to roll
			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
		}
	}
}
