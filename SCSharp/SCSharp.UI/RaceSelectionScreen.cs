//
// SCSharp.UI.RaceSelectionScreen
//
// Authors:
//	Chris Toshok (toshok@hungry.com)
//
// (C) 2006 The Hungry Programmers (http://www.hungry.com/)
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

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class RaceSelectionScreen : UIScreen
	{
		public RaceSelectionScreen (Mpq mpq) 
			: base (mpq, "glue\\PalNl",
				Game.Instance.PlayingBroodWar ? Builtins.rez_GluExpcmpgnBin : Builtins.rez_GluCmpgnBin)
		{
			background_path = null;
		}

		int[] BroodwarCampaigns_MapDataStart = new int[] {
			31,
			40,
			49
		};

		Race[] BroodWarRaces = new Race[] {
			Race.Protoss,
			Race.Terran,
			Race.Zerg
		};

		int[] StarcraftCampaigns_MapDataStart = new int[] {
			0,
			11,
			21
		};

		Race[] StarcraftRaces = new Race[] {
			Race.Terran,
			Race.Zerg,
			Race.Protoss
		};

		const int LOADSAVED_ELEMENT_INDEX = 3;
		const int THIRD_CAMPAIGN_ELEMENT_INDEX = 4;
		const int FIRST_CAMPAIGN_ELEMENT_INDEX = 5;
		const int SECOND_CAMPAIGN_ELEMENT_INDEX = 6;
		const int CANCEL_ELEMENT_INDEX = 7;
		const int PLAYCUSTOM_ELEMENT_INDEX = 8;
		const int SECOND_BUT_FIRST_INCOMPLETE_INDEX = 9;
		const int THIRD_BUT_FIRST_INCOMPLETE_INDEX = 10;
		const int THIRD_BUT_SECOND_INCOMPLETE_INDEX = 11;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[THIRD_CAMPAIGN_ELEMENT_INDEX].MouseEnterEvent += 
				delegate () {
					Console.WriteLine ("over third campaign element");
					if (true /* XXX this should come from the player's file */) {
						Elements[THIRD_BUT_FIRST_INCOMPLETE_INDEX].Visible = true;
					}
				};

			Elements[THIRD_CAMPAIGN_ELEMENT_INDEX].MouseLeaveEvent += 
				delegate () {
					if (true /* XXX this should come from the player's file */) {
						Elements[THIRD_BUT_FIRST_INCOMPLETE_INDEX].Visible = false;
					}
				};

			Elements[SECOND_CAMPAIGN_ELEMENT_INDEX].MouseEnterEvent += 
				delegate () {
					Console.WriteLine ("over second campaign element");
					if (true /* XXX this should come from the player's file */) {
						Elements[SECOND_BUT_FIRST_INCOMPLETE_INDEX].Visible = true;
					}
				};

			Elements[SECOND_CAMPAIGN_ELEMENT_INDEX].MouseLeaveEvent += 
				delegate () {
					if (true /* XXX this should come from the player's file */) {
						Elements[SECOND_BUT_FIRST_INCOMPLETE_INDEX].Visible = false;
					}
				};

			Elements[FIRST_CAMPAIGN_ELEMENT_INDEX].Activate +=
				delegate () {
					SelectCampaign (0);
				};

			Elements[SECOND_CAMPAIGN_ELEMENT_INDEX].Activate +=
				delegate () {
					SelectCampaign (1);
				};

			Elements[THIRD_CAMPAIGN_ELEMENT_INDEX].Activate +=
				delegate () {
					SelectCampaign (2);
				};


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

		void SelectCampaign (int campaign)
		{
			uint mapdata_index;
			string prefix;
			string markup;

			Game.Instance.Race = (Game.Instance.PlayingBroodWar ? BroodWarRaces : StarcraftRaces)[campaign];
			
			mapdata_index = GlobalResources.Instance.MapDataDat.GetFileIndex ((uint)(Game.Instance.PlayingBroodWar ? BroodwarCampaigns_MapDataStart : StarcraftCampaigns_MapDataStart)[campaign]);

			prefix = GlobalResources.Instance.MapDataTbl [(int)mapdata_index];
			markup = String.Format ("rez\\Est{0}{1}{2}.txt",
						Util.RaceChar[(int)Game.Instance.Race],
						prefix.EndsWith ("tutorial") ? "0t" : prefix.Substring (prefix.Length - 2),
						Game.Instance.PlayingBroodWar ? "x" : "");

			Game.Instance.SwitchToScreen (new EstablishingShot (markup, prefix, mpq));
		}
	}
}
