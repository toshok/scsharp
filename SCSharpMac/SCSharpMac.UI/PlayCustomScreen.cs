//
// SCSharp.UI.PlayCustomScreen
//
// Authors:
//	Chris Toshok (toshok@gmail.com)
//
// Copyright 2006-2010 Chris Toshok
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
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
		const int HUMANSLOT_FORMAT_INDEX = 37;
		const int COMPUTERSLOT_FORMAT_INDEX = 35;

		const int FILELISTBOX_ELEMENT_INDEX = 7;
		const int CURRENTDIR_ELEMENT_INDEX = 8;
		const int MAPTITLE_ELEMENT_INDEX = 9;
		const int MAPDESCRIPTION_ELEMENT_INDEX = 10;
		const int MAPSIZE_ELEMENT_INDEX = 11;
		const int MAPTILESET_ELEMENT_INDEX = 12;
		const int MAPPLAYERS1_ELEMENT_INDEX = 14;
		const int MAPPLAYERS2_ELEMENT_INDEX = 15;

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
		Got selectedGot;

		ListBoxElement file_listbox;
		ComboBoxElement gametype_combo;

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
			combo.AddItem (GlobalResources.Instance.GluAllTbl.Strings[128], true); /* Computer */
		}

		string[] files;
		string[] directories;
		Got[] templates;

		void PopulateFileList ()
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
		}

		void PopulateGameTypes ()
		{
			/* load the templates we're interested in displaying */
			StreamReader sr = new StreamReader ((Stream)mpq.GetResource ("templates\\templates.lst"));
			List<Got> templateList = new List<Got>();
			string l;

			while ((l = sr.ReadLine ()) != null) {
				string t = l.Replace ("\"", "");

				Got got = (Got)mpq.GetResource ("templates\\" + t);
				if (got == null)
					continue;

				if (got.ComputerPlayersAllowed && got.NumberOfTeams == 0) {
					Console.WriteLine ("adding template {0}:{1}", got.UIGameTypeName, got.UISubtypeLabel);
					templateList.Add (got);
				}
			}

			templates = new Got[templateList.Count];
			templateList.CopyTo (templates, 0);

			Array.Sort (templates, delegate (Got g1, Got g2) { return g1.ListPosition - g2.ListPosition; });

			/* fill in the game type menu.
			   we only show the templates that allow computer players, have 0 teams */
			foreach (Got got in templates) {
				gametype_combo.AddItem (got.UIGameTypeName);
			}
			gametype_combo.SelectedIndex = 0;

			GameTypeSelectionChanged (gametype_combo.SelectedIndex);
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
			gametype_combo = (ComboBoxElement)Elements[GAMETYPECOMBO_ELEMENT_INDEX];

			/* initially populate the map list by scanning the maps/ directory in the starcraftdir */
			mapdir = Path.Combine (Game.Instance.RootDirectory, "maps");
			curdir = mapdir;

			PopulateGameTypes ();
			PopulateFileList ();

			file_listbox.SelectionChanged += FileListSelectionChanged;
			gametype_combo.SelectionChanged += GameTypeSelectionChanged;

			Elements[OK_ELEMENT_INDEX].Activate +=
				delegate () {
					if (selectedScenario == null) {
						// the selected entry is a directory, switch to it
						if (curdir != mapdir && file_listbox.SelectedIndex == 0)
							curdir = Directory.GetParent (curdir).FullName;
						else
							curdir = directories[file_listbox.SelectedIndex];

						PopulateFileList ();
					}
					else {
						Game.Instance.SwitchToScreen (new GameScreen (mpq,
											      selectedScenario,
											      selectedChk,
											      selectedGot));
					}
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (new RaceSelectionScreen (mpq));
				};


			/* make sure the PLAYER1 player combo reads
			 * the player's name and is desensitized */
			((ComboBoxElement)Elements[PLAYER1_COMBOBOX_PLAYER]).AddItem (/*XXX player name*/"toshok");
			Elements[PLAYER1_COMBOBOX_PLAYER].Sensitive = false;
		}

		void UpdatePlayersDisplay ()
		{
			if (selectedGot.UseMapSettings) {
				string slotString;

				slotString = GlobalResources.Instance.GluAllTbl.Strings[HUMANSLOT_FORMAT_INDEX];
				slotString = slotString.Replace ("%c", " "); /* should probably be a tab.. */
				slotString = slotString.Replace ("%s",
								 (selectedChk == null
								  ? ""
								  : String.Format ("{0}",
										   selectedChk.NumHumanSlots)));

				Elements[MAPPLAYERS1_ELEMENT_INDEX].Text = slotString;
				Elements[MAPPLAYERS1_ELEMENT_INDEX].Visible = true;

				slotString = GlobalResources.Instance.GluAllTbl.Strings[COMPUTERSLOT_FORMAT_INDEX];
				slotString = slotString.Replace ("%c", " "); /* should probably be a tab.. */
				slotString = slotString.Replace ("%s",
								 (selectedChk == null
								  ? ""
								  : String.Format ("{0}",
										   selectedChk.NumComputerSlots)));

				Elements[MAPPLAYERS2_ELEMENT_INDEX].Text = slotString;
				Elements[MAPPLAYERS2_ELEMENT_INDEX].Visible = true;
			}
			else {
				string numPlayersString = GlobalResources.Instance.GluAllTbl.Strings[NUMPLAYERS_FORMAT_INDEX];

				numPlayersString = numPlayersString.Replace ("%c", " "); /* should probably be a tab.. */
				numPlayersString = numPlayersString.Replace ("%s",
									     (selectedChk == null
									      ? ""
									      : String.Format ("{0}",
											       selectedChk.NumPlayers)));

				Elements[MAPPLAYERS1_ELEMENT_INDEX].Text = numPlayersString;
				Elements[MAPPLAYERS1_ELEMENT_INDEX].Visible = true;
				Elements[MAPPLAYERS2_ELEMENT_INDEX].Visible = false;
			}

			int i = 0;
			if (selectedChk != null) {
				for (i = 0; i < max_players; i ++) {
					if (selectedGot.UseMapSettings) {
						if (i >= selectedChk.NumComputerSlots + 1) break;
					}
					else {
						if (i >= selectedChk.NumPlayers) break;
					}

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

		void GameTypeSelectionChanged (int selectedIndex)
		{
			/* the display of the number of players
			 * changes depending upon the template */
			selectedGot = templates[selectedIndex];

			UpdatePlayersDisplay ();
		}

		void FileListSelectionChanged (int selectedIndex)
		{
			string map_path = Path.Combine (curdir, file_listbox.SelectedItem);

			if (selectedScenario !=null)
				selectedScenario.Dispose ();

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

			Elements[MAPSIZE_ELEMENT_INDEX].Text = mapSizeString;
			Elements[MAPTILESET_ELEMENT_INDEX].Text = tileSetString;

			UpdatePlayersDisplay ();
		}

		public override void KeyboardDown (NSEvent theEvent)
		{
			if ((theEvent.ModifierFlags & NSEventModifierMask.NumericPadKeyMask) == NSEventModifierMask.NumericPadKeyMask &&
				(theEvent.Characters[0] == (char)NSKey.UpArrow ||
					theEvent.Characters[0] == (char)NSKey.DownArrow))
				file_listbox.KeyboardDown (theEvent);
			else
				base.KeyboardDown (theEvent);
		}
	}
}
