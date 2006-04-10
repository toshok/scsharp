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

		const int LISTBOX_ELEMENT_INDEX = 6;
		const int TITLE_ELEMENT_INDEX = 7;
		const int DESCRIPTION_ELEMENT_INDEX = 9;
		const int OK_ELEMENT_INDEX = 10;
		const int CANCEL_ELEMENT_INDEX = 11;

#if INCLUDE_ALL_NETWORK_OPTIONS
		const int num_choices = 4;
#else
		const int num_choices = 1;
#endif
		const int title_startidx = 95;
		const int description_startidx = 99;

		string[] titles;
		string[] descriptions;

		ListBoxElement listbox;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			titles = new string[num_choices];
			descriptions = new string[num_choices];

#if INCLUDE_ALL_NETWORK_OPTIONS			
			for (int i = 0; i < num_choices; i ++) {
				titles[i] = GlobalResources.Instance.GluAllTbl[ title_startidx + i ];
				descriptions[i] = GlobalResources.Instance.GluAllTbl[ description_startidx + i ];
			}
#else
				titles[0] = GlobalResources.Instance.GluAllTbl[ title_startidx + 3 ];
				descriptions[0] = GlobalResources.Instance.GluAllTbl[ description_startidx + 3 ];
#endif
			listbox = (ListBoxElement)Elements[LISTBOX_ELEMENT_INDEX];

			foreach (string s in titles)
				listbox.AddItem (s);

			listbox.SelectedIndex = 0;
			HandleSelectionChanged (0);

			listbox.SelectionChanged += HandleSelectionChanged;

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

		void HandleSelectionChanged (int selectedIndex)
		{
			Elements[TITLE_ELEMENT_INDEX].Text = titles[selectedIndex];
			Elements[DESCRIPTION_ELEMENT_INDEX].Text = descriptions[selectedIndex];
		}

		public override void KeyboardDown (KeyboardEventArgs args)
		{
			if (args.Key == Key.DownArrow
			    || args.Key == Key.UpArrow) {
				listbox.KeyboardDown (args);
			}
			else
				base.KeyboardDown (args);
		}
	}
}
