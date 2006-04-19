using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
{
	public class PlayCustomScreen : UIScreen
	{
		public PlayCustomScreen (Mpq mpq) : base (mpq, "glue\\PalNl", Builtins.rez_GluCustmBin)
		{
		}

		const int MAPSIZE_FORMAT_INDEX = 32;
		const int MAPDIM_FORMAT_INDEX = 31; // XXX we don't use this one yet..
		const int TILESET_FORMAT_INDEX = 33;

		const int FILELISTBOX_ELEMENT_INDEX = 7;
		const int CURRENTDIR_ELEMENT_INDEX = 8;
		const int MAPTITLE_ELEMENT_INDEX = 9;
		const int MAPDESCRIPTION_ELEMENT_INDEX = 10;
		const int MAPINFO1_ELEMENT_INDEX = 11;
		const int MAPINFO2_ELEMENT_INDEX = 12;
		const int MAPINFO3_ELEMENT_INDEX = 14;
		const int MAPINFO4_ELEMENT_INDEX = 15;

		const int OK_ELEMENT_INDEX = 16;
		const int CANCEL_ELEMENT_INDEX = 17;

		const int GAMETYPECOMBO_ELEMENT_INDEX = 20;

		const int GAMESUBTYPE_LABEL_ELEMENT_INDEX = 19;
		const int GAMESUBTYPE_COMBO_ELEMENT_INDEX = 21;
		
		const int PLAYER1_COMBOBOX_PLAYER = 22;
		const int PLAYER1_COMBOBOX_RACE = 30;

		const int max_players = 8;

		string mapdir;
		Mpq selectedScenario;
		Chk selectedChk;

		ListBoxElement file_listbox;

		void InitializeRaceCombo (ComboBoxElement combo)
		{
			combo.AddItem ("Zerg"); /* XXX these should all come from some string constant table someplace */
			combo.AddItem ("Terran");
			combo.AddItem ("Protoss");
			combo.AddItem ("Random", true);
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			/* these don't ever show up in the UI, that i know of... */
			Elements[GAMESUBTYPE_LABEL_ELEMENT_INDEX].Visible = false;
			Elements[GAMESUBTYPE_COMBO_ELEMENT_INDEX].Visible = false;

			/* initialize all the race combo boxes */
			for (int i = 0; i < max_players; i ++)
				InitializeRaceCombo ((ComboBoxElement)Elements[PLAYER1_COMBOBOX_RACE + i]);

			/* populate the map list by scanning the maps/ directory in the starcraftdir */
			mapdir = Path.Combine (Game.Instance.RootDirectory, "maps");

			string[] files = Directory.GetFiles (mapdir, "*.sc*");
		
			Elements[CURRENTDIR_ELEMENT_INDEX].Text = Path.GetFileName (mapdir);

			file_listbox = (ListBoxElement)Elements[FILELISTBOX_ELEMENT_INDEX];

			for (int i = 0; i < files.Length; i ++) {
				string lower = files[i].ToLower();
				if (lower.EndsWith (".scm") || lower.EndsWith (".scx"))
					file_listbox.AddItem (Path.GetFileName (files[i]));
			}

			file_listbox.SelectedIndex = 0;
			HandleSelectionChanged (0);

			file_listbox.SelectionChanged += HandleSelectionChanged;

			Elements[OK_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (new GameScreen (mpq, selectedScenario));
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (UIScreenType.RaceSelection);
				};

			// notify we're ready to roll
			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
		}

		void HandleSelectionChanged (int selectedIndex)
		{
			string map_path = Path.Combine (mapdir, file_listbox.SelectedItem);

			Console.WriteLine ("selectedScenario = {0}", map_path);

			selectedScenario = new MpqArchive (map_path);

			selectedChk = (Chk)selectedScenario.GetResource ("staredit/scenario.chk");

			string mapSizeString = GlobalResources.Instance.GluAll.Strings[MAPSIZE_FORMAT_INDEX];
			mapSizeString = mapSizeString.Replace ("%c", " "); /* should probably be a tab.. */
			mapSizeString = mapSizeString.Replace ("%s",
							       String.Format ("{0}x{1}",
									      selectedChk.Width,
									      selectedChk.Height));
		}

		public override void KeyboardDown (KeyboardEventArgs args)
		{
			if (args.Key == Key.DownArrow
			    || args.Key == Key.UpArrow) {
				file_listbox.KeyboardDown (args);
			}
			else
				base.KeyboardDown (args);
		}
	}
}
