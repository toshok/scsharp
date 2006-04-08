using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class ConnectionScreen : UIScreen
	{
		public ConnectionScreen (Mpq mpq) : base (mpq, "glue\\PalNl", Builtins.rez_GluConnBin)
		{
		}

		const int TITLE_ELEMENT_INDEX = 7;
		const int DESCRIPTION_ELEMENT_INDEX = 9;
		const int OK_ELEMENT_INDEX = 10;
		const int CANCEL_ELEMENT_INDEX = 11;

		const int num_choices = 4;
		const int title_startidx = 95;
		const int description_startidx = 99;

		string[] titles;
		string[] descriptions;

		Surface[] titleSurfaces;
		Surface[] helpSurfaces;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			titles = new string[num_choices];
			descriptions = new string[num_choices];

			for (int i = 0; i < num_choices; i ++) {
				titles[i] = GlobalResources.Instance.GluAllTbl[ title_startidx + i ];
				descriptions[i] = GlobalResources.Instance.GluAllTbl[ description_startidx + i ];
			}

			Elements[TITLE_ELEMENT_INDEX].Text = titles[1];
			Elements[DESCRIPTION_ELEMENT_INDEX].Text = descriptions[1];

			Elements[OK_ELEMENT_INDEX].Activate +=
				delegate () {
					Console.WriteLine ("we should do something here... ");
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (UIScreenType.MainMenu);
				};

			// notify we're ready to roll
			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
		}

		public override void KeyboardDown (KeyboardEventArgs args)
		{
			if (args.Key == Key.DownArrow) {
				Console.WriteLine ("select next connection type");
			}
			else if (args.Key == Key.UpArrow) {
				Console.WriteLine ("select previous connection type");
			}
			else
				base.KeyboardDown (args);
		}
	}
}
