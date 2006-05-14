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

		string[] BroodWarCampaigns = new string[] {
			"campaign\\expprotoss\\protoss01",
			"campaign\\expterran\\terran01",
			"campaign\\expzerg\\zerg01"
		};

		Race[] BroodWarRaces = new Race[] {
			Race.Protoss,
			Race.Terran,
			Race.Zerg
		};

		string[] StarcraftCampaigns = new string[] {
			"campaign\\protoss\\protoss01",
			"campaign\\terran\\terran01",
			"campaign\\zerg\\zerg01"
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
					Game.Instance.Race = (Game.Instance.PlayingBroodWar ? BroodWarRaces : StarcraftRaces)[0];
					string prefix = (Game.Instance.PlayingBroodWar ? BroodWarCampaigns : StarcraftCampaigns)[(int)Game.Instance.Race];
					Game.Instance.SwitchToScreen (ReadyRoomScreen.Create (mpq, prefix));
				};

			Elements[SECOND_CAMPAIGN_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.Race = (Game.Instance.PlayingBroodWar ? BroodWarRaces : StarcraftRaces)[1];
					string prefix = (Game.Instance.PlayingBroodWar ? BroodWarCampaigns : StarcraftCampaigns)[(int)Game.Instance.Race];
					Game.Instance.SwitchToScreen (ReadyRoomScreen.Create (mpq, prefix));
				};

			Elements[THIRD_CAMPAIGN_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.Race = (Game.Instance.PlayingBroodWar ? BroodWarRaces : StarcraftRaces)[2];
					string prefix = (Game.Instance.PlayingBroodWar ? BroodWarCampaigns : StarcraftCampaigns)[(int)Game.Instance.Race];
					Game.Instance.SwitchToScreen (ReadyRoomScreen.Create (mpq, prefix));
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
	}
}
