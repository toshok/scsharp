using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
{
	public class MainMenu : UIScreen
	{
		public MainMenu (Mpq mpq) : base (mpq, "glue\\Palmm", Builtins.rez_GluMainBin)
		{
		}

		const int EXIT_ELEMENT_INDEX = 2;
		const int SINGLEPLAYER_ELEMENT_INDEX = 3;
		const int MULTIPLAYER_ELEMENT_INDEX = 4;
		const int CAMPAIGNEDITOR_ELEMENT_INDEX = 5;
		const int INTRO_ELEMENT_INDEX = 8;
		const int CREDITS_ELEMENT_INDEX = 9;
		const int VERSION_ELEMENT_INDEX = 10;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Elements[VERSION_ELEMENT_INDEX].Text = "v0.0000000001";

			Elements[SINGLEPLAYER_ELEMENT_INDEX].Activate +=
				delegate () {
					GameModeDialog d = new GameModeDialog (this, mpq);
					d.Cancel += delegate () { DismissDialog (); };
					d.Activate += delegate (bool expansion) {
						Console.WriteLine ("dude, you're getting a {0}", expansion ? "expansion" : "original");
						DismissDialog ();
						GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
						Game.Instance.SwitchToScreen (UIScreenType.Login);
					};
					ShowDialog (d);
				};

			Elements[MULTIPLAYER_ELEMENT_INDEX].Activate +=
				delegate () {
					GameModeDialog d = new GameModeDialog (this, mpq);
					d.Cancel += delegate () { DismissDialog (); };
					d.Activate += delegate (bool expansion) {
						Console.WriteLine ("dude, you're getting a {0}", expansion ? "expansion" : "original");
						DismissDialog ();
						GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
						Game.Instance.SwitchToScreen (UIScreenType.Connection);
					};
					ShowDialog (d);
				};

			Elements[CAMPAIGNEDITOR_ELEMENT_INDEX].Activate +=
				delegate () {
					OkDialog d = new OkDialog (this, mpq,
								   "The campaign editor functionality is not available in SCSharp");
					ShowDialog (d);
				};

			Elements[INTRO_ELEMENT_INDEX].Activate +=
				delegate () {
					OkDialog d = new OkDialog (this,
								   mpq,
								   "Cinematics are not available in SCSharp");
					ShowDialog (d);
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
