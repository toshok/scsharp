using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class LoginScreen : UIScreen
	{
		public LoginScreen (Mpq mpq) : base (mpq, "glue\\PalNl", Builtins.rez_GluLoginBin)
		{
		}

		const int OK_ELEMENT_INDEX = 4;
		const int CANCEL_ELEMENT_INDEX = 5;
		const int NEW_ELEMENT_INDEX = 6;
		const int DELETE_ELEMENT_INDEX = 7;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Elements[OK_ELEMENT_INDEX].Activate +=
				delegate () {
					Console.WriteLine ("switch to the race selection screen, yay!");
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (UIScreenType.MainMenu);
				};

			Elements[NEW_ELEMENT_INDEX].Activate +=
				delegate () {
					Console.WriteLine ("create character for registry!");
				};

			Elements[DELETE_ELEMENT_INDEX].Activate +=
				delegate () {
					Console.WriteLine ("delete character from registry!");
				};

			// notify we're ready to roll
			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
		}
	}
}
