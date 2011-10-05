//
// SCSharp.UI.RaceSelectionScreen
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
using System.Collections.Generic;
using System.IO;
using System.Threading;

using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
{
	public class RaceSelectionScreen : UIScreen
	{
		public RaceSelectionScreen (Mpq mpq) 
			: base (mpq, "glue\\PalCs",
				Game.Instance.PlayingBroodWar ? Builtins.rez_GluExpcmpgnBin : Builtins.rez_GluCmpgnBin)
		{
		}

		struct RaceData {
			public Race race;
			public string normalMovie;
			public string onMovie;
			public int mapDataStart;

			public RaceData (Race race, string normalMovie, string onMovie, int mapDataStart) {
				this.race = race;
				this.normalMovie = normalMovie;
				this.onMovie = onMovie;
				this.mapDataStart = mapDataStart;
			}
		}

		RaceData[] StarcraftCampaigns = new RaceData[] {
			new RaceData ( Race.Terran, "glue\\campaign\\terr.smk", "glue\\campaign\\terron.smk", 0 ),
			new RaceData ( Race.Zerg, "glue\\campaign\\zerg.smk", "glue\\campaign\\zergon.smk", 11 ),
			new RaceData ( Race.Protoss, "glue\\campaign\\prot.smk", "glue\\campaign\\proton.smk", 21 ),

		};

		RaceData[] BroodwarCampaigns = new RaceData[] {
			new RaceData ( Race.Protoss, "glue\\Expcampaign\\XProt.smk", "glue\\Expcampaign\\XProtOn.smk", 31 ),
			new RaceData ( Race.Terran, "glue\\Expcampaign\\XTerr.smk", "glue\\Expcampaign\\XTerrOn.smk", 40 ),
			new RaceData ( Race.Zerg, "glue\\Expcampaign\\XZerg.smk", "glue\\Expcampaign\\XZergOn.smk", 49 ),
		};

		const int LOADSAVED_ELEMENT_INDEX = 3;
		const int LOADREPLAY_ELEMENT_INDEX = 4;
		const int THIRD_CAMPAIGN_ELEMENT_INDEX = 5;
		const int FIRST_CAMPAIGN_ELEMENT_INDEX = 6;
		const int SECOND_CAMPAIGN_ELEMENT_INDEX = 7;
		const int CANCEL_ELEMENT_INDEX = 8;
		const int PLAYCUSTOM_ELEMENT_INDEX = 9;
		const int SECOND_BUT_FIRST_INCOMPLETE_INDEX = 10;
		const int THIRD_BUT_FIRST_INCOMPLETE_INDEX = 11;
		const int THIRD_BUT_SECOND_INCOMPLETE_INDEX = 12;

		List<UIElement> smkElements;

		public override void AddToPainter ()
		{
			base.AddToPainter ();
			foreach (MovieElement el in smkElements)
				el.Play ();
		}

		public override void RemoveFromPainter ()
		{
			base.RemoveFromPainter ();
			foreach (MovieElement el in smkElements)
				el.Stop ();

			diskPlayer = null;
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}' : {3}", i, Elements[i].Type, Elements[i].Text, Elements[i].Flags);

			Elements[FIRST_CAMPAIGN_ELEMENT_INDEX].Flags |= ElementFlags.BottomAlignText;
			Elements[SECOND_CAMPAIGN_ELEMENT_INDEX].Flags |= ElementFlags.BottomAlignText;
			Elements[THIRD_CAMPAIGN_ELEMENT_INDEX].Flags |= ElementFlags.BottomAlignText;

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
//					Game.Instance.SwitchToScreen (new LoadSavedScreen (mpq));
				};

			Elements[PLAYCUSTOM_ELEMENT_INDEX].Activate += 
				delegate () {
//					Game.Instance.SwitchToScreen (new PlayCustomScreen (mpq));
				};

			smkElements = new List<UIElement>();

			AddMovieElements (FIRST_CAMPAIGN_ELEMENT_INDEX, 0, -40, 0);
			AddMovieElements (SECOND_CAMPAIGN_ELEMENT_INDEX, 1, 0, 0);
			AddMovieElements (THIRD_CAMPAIGN_ELEMENT_INDEX, 2, 0, 0);
			
			foreach (var ui_el in smkElements) {
				ui_el.Layer.Position = new PointF (ui_el.X1, Bounds.Height - ui_el.Y1);
				ui_el.Layer.AnchorPoint = new PointF (0, 0);
				AddSublayer (ui_el.Layer);				
			}			
		}

		void SelectCampaign (int campaign)
		{
			uint mapdata_index;
			string prefix;
			string markup;

			Game.Instance.Race = (Game.Instance.PlayingBroodWar ? BroodwarCampaigns : StarcraftCampaigns)[campaign].race;
			
			mapdata_index = GlobalResources.Instance.MapDataDat.FileIndexes [(Game.Instance.PlayingBroodWar ? BroodwarCampaigns : StarcraftCampaigns)[campaign].mapDataStart];

			prefix = GlobalResources.Instance.MapDataTbl [(int)mapdata_index];
			markup = String.Format ("rez\\Est{0}{1}{2}.txt",
						Util.RaceChar[(int)Game.Instance.Race],
						prefix.EndsWith ("tutorial") ? "0t" : prefix.Substring (prefix.Length - 2),
						Game.Instance.PlayingBroodWar ? "x" : "");

			Game.Instance.SwitchToScreen (new EstablishingShot (markup, prefix, mpq));
		}

		SmackerPlayer diskPlayer;

		void AddMovieElements (int elementIndex, int campaign, int off_x, int off_y)
		{
			MovieElement normalElement, onElement, diskElement;

			if (diskPlayer == null)
				diskPlayer = new SmackerPlayer ((Stream)Mpq.GetResource (Game.Instance.PlayingBroodWar ? "glue\\Expcampaign\\disk.smk" : "glue\\campaign\\disk.smk"), 1);

			diskElement = new MovieElement (this,
							Elements[elementIndex].BinElement,
							Elements[elementIndex].Palette,
							diskPlayer);

			diskElement.X1 = (ushort)(Elements[elementIndex].X1 + ((Elements[elementIndex].Width - diskElement.MovieSize.Width) / 2));
			diskElement.Y1 = (ushort)(((ButtonElement)Elements[elementIndex]).TextPosition.Y - diskElement.MovieSize.Height);

			normalElement = new MovieElement (this,
							  Elements[elementIndex].BinElement,
							  Elements[elementIndex].Palette,
							  (Game.Instance.PlayingBroodWar ? BroodwarCampaigns : StarcraftCampaigns)[campaign].normalMovie);

			normalElement.X1 = (ushort)(Elements[elementIndex].X1 + ((Elements[elementIndex].Width - normalElement.MovieSize.Width) / 2) + off_x);
			normalElement.Y1 = (ushort)(((ButtonElement)Elements[elementIndex]).TextPosition.Y - normalElement.MovieSize.Height + off_y);

			onElement = new MovieElement (this,
						      Elements[elementIndex].BinElement,
						      Elements[elementIndex].Palette,
						      (Game.Instance.PlayingBroodWar ? BroodwarCampaigns : StarcraftCampaigns)[campaign].onMovie);

			onElement.X1 = (ushort)(Elements[elementIndex].X1 + ((Elements[elementIndex].Width - onElement.MovieSize.Width) / 2));
			onElement.Y1 = (ushort)(((ButtonElement)Elements[elementIndex]).TextPosition.Y - onElement.MovieSize.Height);

			smkElements.Add (diskElement);
			smkElements.Add (normalElement);
			smkElements.Add (onElement);

			onElement.Visible = false;
			normalElement.Dim (100);

			Elements[elementIndex].MouseEnterEvent += 
				delegate () {
					normalElement.Dim (0);
					onElement.Visible = true;
				};
			Elements[elementIndex].MouseLeaveEvent += 
				delegate () {
					normalElement.Dim (100);
					onElement.Visible = false;
				};
		}
	}
}
