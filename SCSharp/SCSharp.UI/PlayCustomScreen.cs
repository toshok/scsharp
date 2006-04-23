using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class PlayCustomScreen : UIScreen
	{
		public PlayCustomScreen (Mpq mpq) : base (mpq, "glue\\PalNl", Builtins.rez_GluCustmBin)
		{
		}

		const int MAPSIZE_FORMAT_INDEX = 32;
		const int MAPDIM_FORMAT_INDEX = 31; // XXX we don't use this one yet..
		const int TILESET_FORMAT_INDEX = 33;
		const int NUMPLAYERS_FORMAT_INDEX = 30;

		const int FILELISTBOX_ELEMENT_INDEX = 7;
		const int CURRENTDIR_ELEMENT_INDEX = 8;
		const int MAPTITLE_ELEMENT_INDEX = 9;
		const int MAPDESCRIPTION_ELEMENT_INDEX = 10;
		const int MAPSIZE_ELEMENT_INDEX = 11;
		const int MAPTILESET_ELEMENT_INDEX = 12;
		const int MAPPLAYERS_ELEMENT_INDEX = 14;
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
		string curdir;

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

		void InitializePlayerCombo (ComboBoxElement combo)
		{
			combo.AddItem (GlobalResources.Instance.GluAllTbl.Strings[130]); /* Closed */
			combo.AddItem (GlobalResources.Instance.GluAllTbl.Strings[128], true); /* Closed */
		}

		string[] files;
		string[] directories;

		void PopulateUIFromDir ()
		{
			file_listbox.Clear ();

			string[] dir = Directory.GetDirectories (curdir);
			List<string> dirs = new List<string>();
			if (curdir != mapdir) {
				dirs.Add ("Up One Level");
			}
			foreach (string d in dir) {
				string dl = Path.GetFileName (d).ToLower ();

				if (curdir == mapdir) {
					if (!Game.Instance.IsBroodWar
					    && dl == "broodwar")
						continue;

					if (dl == "replays")
						continue;
				}

				dirs.Add (d);
			}

			directories = dirs.ToArray();

			files = Directory.GetFiles (curdir, "*.sc*");
		
			Elements[CURRENTDIR_ELEMENT_INDEX].Text = Path.GetFileName (curdir);

			for (int i = 0; i < directories.Length; i ++) {
				file_listbox.AddItem (String.Format ("[{0}]", Path.GetFileName (directories[i])));
			}

			for (int i = 0; i < files.Length; i ++) {
				string lower = files[i].ToLower();
				if (lower.EndsWith (".scm") || lower.EndsWith (".scx"))
					file_listbox.AddItem (Path.GetFileName (files[i]));
			}

			file_listbox.SelectedIndex = directories.Length;
			HandleSelectionChanged (directories.Length);
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
			for (int i = 0; i < max_players; i ++) {
				InitializePlayerCombo ((ComboBoxElement)Elements[PLAYER1_COMBOBOX_PLAYER + i]);
				InitializeRaceCombo ((ComboBoxElement)Elements[PLAYER1_COMBOBOX_RACE + i]);
			}

			file_listbox = (ListBoxElement)Elements[FILELISTBOX_ELEMENT_INDEX];

			/* initially populate the map list by scanning the maps/ directory in the starcraftdir */
			mapdir = Path.Combine (Game.Instance.RootDirectory, "maps");
			curdir = mapdir;

			PopulateUIFromDir ();

			file_listbox.SelectionChanged += HandleSelectionChanged;

			Elements[OK_ELEMENT_INDEX].Activate +=
				delegate () {
					if (selectedScenario == null) {
						// the selected entry is a directory, switch to it
						if (curdir != mapdir)
							if (file_listbox.SelectedIndex == 0)
								curdir = Directory.GetParent (curdir).FullName;
							else
								curdir = directories[file_listbox.SelectedIndex - 1];

						else
								curdir = directories[file_listbox.SelectedIndex];

						PopulateUIFromDir ();
					}
					else {
						Game.Instance.SwitchToScreen (new GameScreen (mpq,
											      selectedScenario,
											      selectedChk));
					}
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (UIScreenType.RaceSelection);
				};

			/* make sure the PLAYER1 player combo reads
			 * the player's name and is desensitized */
			((ComboBoxElement)Elements[PLAYER1_COMBOBOX_PLAYER]).AddItem (/*XXX player name*/"toshok");
			Elements[PLAYER1_COMBOBOX_PLAYER].Sensitive = false;
		}

		void HandleSelectionChanged (int selectedIndex)
		{
			string map_path = Path.Combine (curdir, file_listbox.SelectedItem);

			if (selectedIndex < directories.Length) {
				selectedScenario = null;
				selectedChk = null;
			}
			else {
				selectedScenario = new MpqArchive (map_path);

				selectedChk = (Chk)selectedScenario.GetResource ("staredit\\scenario.chk");
			}

			Elements[MAPTITLE_ELEMENT_INDEX].Text = selectedChk == null ? "" : selectedChk.Name;
			Elements[MAPDESCRIPTION_ELEMENT_INDEX].Text = selectedChk == null ? "" : selectedChk.Description;

			string mapSizeString = GlobalResources.Instance.GluAllTbl.Strings[MAPSIZE_FORMAT_INDEX];
			//			string mapDimString = GlobalResources.Instance.GluAllTbl.Strings[MAPDIM_FORMAT_INDEX];
			string tileSetString = GlobalResources.Instance.GluAllTbl.Strings[TILESET_FORMAT_INDEX];
			string numPlayersString = GlobalResources.Instance.GluAllTbl.Strings[NUMPLAYERS_FORMAT_INDEX];

			mapSizeString = mapSizeString.Replace ("%c", " "); /* should probably be a tab.. */
			mapSizeString = mapSizeString.Replace ("%s",
							       (selectedChk == null
								? ""
								: String.Format ("{0}x{1}",
										 selectedChk.Width,
										 selectedChk.Height)));

			tileSetString = tileSetString.Replace ("%c", " "); /* should probably be a tab.. */
			tileSetString = tileSetString.Replace ("%s",
							       (selectedChk == null
								? ""
								: String.Format ("{0}",
										 selectedChk.Tileset)));

			numPlayersString = numPlayersString.Replace ("%c", " "); /* should probably be a tab.. */
			numPlayersString = numPlayersString.Replace ("%s",
								     (selectedChk == null
								      ? ""
								      : String.Format ("{0}",
										       selectedChk.NumPlayers)));

			Elements[MAPSIZE_ELEMENT_INDEX].Text = mapSizeString;
			Elements[MAPTILESET_ELEMENT_INDEX].Text = tileSetString;
			Elements[MAPPLAYERS_ELEMENT_INDEX].Text = numPlayersString;
			Elements[MAPPLAYERS_ELEMENT_INDEX].Visible = true;

			int i = 0;
			if (selectedChk != null) {
				for (i = 0; i < max_players; i ++) {
					if (i >= selectedChk.NumPlayers) break;
					if (i > 0)
						((ComboBoxElement)Elements[PLAYER1_COMBOBOX_PLAYER + i]).SelectedIndex = 1;
					((ComboBoxElement)Elements[PLAYER1_COMBOBOX_RACE + i]).SelectedIndex = 3;
					Elements[PLAYER1_COMBOBOX_PLAYER + i].Visible = true;
					Elements[PLAYER1_COMBOBOX_RACE + i].Visible = true;
				}
			}
			for (int j = i; j < max_players; j ++) {
				Elements[PLAYER1_COMBOBOX_PLAYER + j].Visible = false;
				Elements[PLAYER1_COMBOBOX_RACE + j].Visible = false;
			}
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
