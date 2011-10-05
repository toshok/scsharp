//
// SCSharp.UI.ReadyRoomScreen
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

using MonoMac.Foundation;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
{
	public class ReadyRoomScreen : UIScreen
	{
		public ReadyRoomScreen (Mpq mpq,
					string scenario_prefix,
					int start_element_index,
					int cancel_element_index,
					int skiptutorial_element_index,
					int replay_element_index,
					int transmission_element_index,
					int objectives_element_index,
					int first_portrait_element_index)
			: base (mpq,
				String.Format ("glue\\Ready{0}", Util.RaceChar[(int)Game.Instance.Race]),
				String.Format (Builtins.rez_GluRdyBin, Util.RaceCharLower[(int)Game.Instance.Race]))
		{
			background_path = String.Format ("glue\\PalR{0}\\Backgnd.pcx", Util.RaceCharLower[(int)Game.Instance.Race]);
			fontpal_path = String.Format ("glue\\PalR{0}\\tFont.pcx", Util.RaceCharLower[(int)Game.Instance.Race]);
			effectpal_path = String.Format ("glue\\PalR{0}\\tEffect.pcx", Util.RaceCharLower[(int)Game.Instance.Race]);
			arrowgrp_path = String.Format ("glue\\PalR{0}\\arrow.grp", Util.RaceCharLower[(int)Game.Instance.Race]);

			this.start_element_index = start_element_index;
			this.cancel_element_index = cancel_element_index;
			this.skiptutorial_element_index = skiptutorial_element_index;
			this.replay_element_index = replay_element_index;
			this.transmission_element_index = transmission_element_index;
			this.objectives_element_index = objectives_element_index;
			this.first_portrait_element_index = first_portrait_element_index;

			this.scenario = (Chk)mpq.GetResource (scenario_prefix + "\\staredit\\scenario.chk");
			this.scenario_prefix = scenario_prefix;
		}

		BriefingRunner runner;
		Chk scenario;
		string scenario_prefix;
		int start_element_index;
		int cancel_element_index;
		int skiptutorial_element_index;
		int replay_element_index;
		int transmission_element_index;
		int objectives_element_index;
		int first_portrait_element_index;

		List<MovieElement> portraits;
		List<ImageElement> hframes;
		List<ImageElement> frames;

		protected override void ResourceLoader ()
		{
			TranslucentIndex = 138;

			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}' {3}", i, Elements[i].Type, Elements[i].Text, Elements[i].Flags);

			if (scenario_prefix.EndsWith ("tutorial")) {
				Elements[skiptutorial_element_index].Visible = true;
				/* XXX Activate */
			}

			Elements[cancel_element_index].Activate +=
				delegate () {
					StopBriefing ();
					Game.Instance.SwitchToScreen (UIScreenType.Login);
				};

			Elements[replay_element_index].Activate +=
				delegate () {
					StopBriefing ();
					PlayBriefing ();
				};

			Elements[start_element_index].Activate +=
				delegate () {
					StopBriefing ();
//					Game.Instance.SwitchToScreen (new GameScreen (mpq, scenario_prefix, scenario));
				};

			runner = new BriefingRunner (this, scenario, scenario_prefix);

			portraits = new List<MovieElement> ();
			hframes = new List<ImageElement> ();
			frames = new List<ImageElement> ();

			for (int i = 0; i < 4; i ++) {
				MovieElement m = new MovieElement (this,
								   Elements[first_portrait_element_index + i].BinElement,
								   Elements[first_portrait_element_index + i].Palette,
								   true);

				m.X1 += 3;
				m.Y1 += 3;
				m.Width -= 6;
				m.Height -= 6;

				ImageElement f = new ImageElement (this,
								   Elements[first_portrait_element_index + i].BinElement,
								   Elements[first_portrait_element_index + i].Palette,
								   TranslucentIndex);
				f.Text = String.Format ("glue\\Ready{0}\\{0}Frame{1}.pcx",
							Util.RaceChar[(int)Game.Instance.Race],
							i + 1);

				ImageElement h = new ImageElement (this,
								   Elements[first_portrait_element_index + i].BinElement,
								   Elements[first_portrait_element_index + i].Palette,
								   TranslucentIndex);
				h.Text = String.Format ("glue\\Ready{0}\\{0}FrameH{1}.pcx",
							Util.RaceChar[(int)Game.Instance.Race],
							i + 1);

				f.Visible = false;
				h.Visible = false;
				m.Visible = false;
				
				portraits.Add (m);
				hframes.Add (h);
				frames.Add (f);

				Elements.Add (m);
				Elements.Add (h);
				Elements.Add (f);
			}
		}

		void StopBriefing ()
		{
			Game.Instance.Tick -= runner.Tick;
			runner.Stop ();

			Elements[transmission_element_index].Visible = false;
			Elements[transmission_element_index].Text = "";

			Elements[objectives_element_index].Visible = false;
			Elements[objectives_element_index].Text = "";

			for (int i = 0; i < 4; i ++) {
				Elements[first_portrait_element_index + i].Visible = false;
				portraits[i].Visible = false;
			}
		}

		void PlayBriefing ()
		{
			runner.Play ();
		}

		public override void AddToPainter ()
		{
			base.AddToPainter ();

			Game.Instance.Tick += runner.Tick;
		}

		public override void RemoveFromPainter ()
		{
			base.RemoveFromPainter ();

			foreach (UIElement el in Elements) {
				if (el is MovieElement)
					((MovieElement)el).Stop ();
			}
		}

		public void SetObjectives (string str)
		{
			Elements[objectives_element_index].Visible = true;
			Elements[objectives_element_index].Text = str;
		}

		public void SetTransmissionText (string str)
		{
			Elements[transmission_element_index].Visible = true;
			Elements[transmission_element_index].Text = str;
		}

		int highlightedPortrait = -1;
		public void HighlightPortrait (int slot)
		{
			if (highlightedPortrait != -1)
				UnhighlightPortrait (highlightedPortrait);

			hframes[slot].Visible = true;
			frames[slot].Visible = false;
			highlightedPortrait = slot;
			portraits[highlightedPortrait].Dim (0);
		}
		public void UnhighlightPortrait (int slot)
		{
			if (portraits[slot].Visible) {
				hframes[slot].Visible = false;
				frames[slot].Visible = true;
				portraits[highlightedPortrait].Dim (100);
			}
		}

		public void ShowPortrait (int unit, int slot)
		{
			Console.WriteLine ("showing portrait {0} (unit {1}, portrait index {2}) in slot {3}",
					   "" /*portrait_resource*/, unit,
					   GlobalResources.Instance.PortDataDat.PortraitIndexes [unit],
					   slot);

			uint portraitIndex = GlobalResources.Instance.UnitsDat.Portraits [unit];

			string portrait_resource = String.Format ("portrait\\{0}0.smk",
								  GlobalResources.Instance.PortDataTbl[(int)GlobalResources.Instance.PortDataDat.PortraitIndexes [(int)portraitIndex]]);

			portraits[slot].Player = new SmackerPlayer ((Stream)Mpq.GetResource (portrait_resource), 1);

			portraits[slot].Dim (100);
			portraits[slot].Play ();
			portraits[slot].Visible = true;
			hframes[slot].Visible = false;
			frames[slot].Visible = true;
		}

		public void HidePortrait (int slot)
		{
			portraits[slot].Visible = false;
			hframes[slot].Visible = false;
			frames[slot].Visible = true;

			portraits[slot].Stop ();
		}

		public static ReadyRoomScreen Create (Mpq mpq,
						      string scenario_prefix)
		{
			switch (Game.Instance.Race) {
			case Race.Terran:
				return new TerranReadyRoomScreen (mpq, scenario_prefix);
			case Race.Protoss:
				return new ProtossReadyRoomScreen (mpq, scenario_prefix);
			case Race.Zerg:
				return new ZergReadyRoomScreen (mpq, scenario_prefix);
			default:
				return null;
			}
		}
	}

	class TerranReadyRoomScreen : ReadyRoomScreen
	{
		public TerranReadyRoomScreen (Mpq mpq,
					      string scenario_prefix)
			: base (mpq,
				scenario_prefix,
				START_ELEMENT_INDEX,
				CANCEL_ELEMENT_INDEX,
				SKIPTUTORIAL_ELEMENT_INDEX,
				REPLAY_ELEMENT_INDEX,
				TRANSMISSION_ELEMENT_INDEX,
				OBJECTIVES_ELEMENT_INDEX,
				FIRST_PORTRAIT_ELEMENT_INDEX)
		{
		}

		const int START_ELEMENT_INDEX = 1;
		const int CANCEL_ELEMENT_INDEX = 9;
		const int SKIPTUTORIAL_ELEMENT_INDEX = 11;
		const int REPLAY_ELEMENT_INDEX = 12;
		const int FIRST_PORTRAIT_ELEMENT_INDEX = 13;
		const int TRANSMISSION_ELEMENT_INDEX = 17;
		const int OBJECTIVES_ELEMENT_INDEX = 18;
	}

	class ProtossReadyRoomScreen : ReadyRoomScreen
	{
		public ProtossReadyRoomScreen (Mpq mpq,
					      string scenario_prefix)
			: base (mpq,
				scenario_prefix,
				START_ELEMENT_INDEX,
				CANCEL_ELEMENT_INDEX,
				SKIPTUTORIAL_ELEMENT_INDEX,
				REPLAY_ELEMENT_INDEX,
				TRANSMISSION_ELEMENT_INDEX,
				OBJECTIVES_ELEMENT_INDEX,
				FIRST_PORTRAIT_ELEMENT_INDEX)
		{
		}
		
		const int START_ELEMENT_INDEX = 1;
		const int CANCEL_ELEMENT_INDEX = 9;
		const int SKIPTUTORIAL_ELEMENT_INDEX = 11;
		const int REPLAY_ELEMENT_INDEX = 12;
		const int FIRST_PORTRAIT_ELEMENT_INDEX = 13;
		const int TRANSMISSION_ELEMENT_INDEX = 17;
		const int OBJECTIVES_ELEMENT_INDEX = 18;
	}

	class ZergReadyRoomScreen : ReadyRoomScreen
	{
		public ZergReadyRoomScreen (Mpq mpq,
					      string scenario_prefix)
			: base (mpq,
				scenario_prefix,
				START_ELEMENT_INDEX,
				CANCEL_ELEMENT_INDEX,
				SKIPTUTORIAL_ELEMENT_INDEX,
				REPLAY_ELEMENT_INDEX,
				TRANSMISSION_ELEMENT_INDEX,
				OBJECTIVES_ELEMENT_INDEX,
				FIRST_PORTRAIT_ELEMENT_INDEX)
		{
		}
		
		const int START_ELEMENT_INDEX = 1;
		const int CANCEL_ELEMENT_INDEX = 10;
		const int SKIPTUTORIAL_ELEMENT_INDEX = 12;
		const int REPLAY_ELEMENT_INDEX = 13;
		const int FIRST_PORTRAIT_ELEMENT_INDEX = 14;
		const int TRANSMISSION_ELEMENT_INDEX = 18;
		const int OBJECTIVES_ELEMENT_INDEX = 19;
	}

	class BriefingRunner
	{
		TriggerData triggerData;
		Chk scenario;
		ReadyRoomScreen screen;
		string prefix;

		int sleepUntil;
		int totalElapsed;
		int current_action;

		public BriefingRunner (ReadyRoomScreen screen, Chk scenario,
				       string scenario_prefix)
		{
			this.screen = screen;
			this.scenario = scenario;
			this.prefix = scenario_prefix;
			triggerData = scenario.BriefingData;
		}

		public void Play ()
		{
			current_action = 0;
		}

		public void Stop ()
		{
			sleepUntil = 0;
		}

		public void Tick (object sender, TickEventArgs e)
		{
			TriggerAction[] actions = triggerData.Triggers[0].Actions;

			if (current_action == actions.Length)
				return;
			
			// FIXME we need to figure out how many ticks per second SC1 used for normal mode... 6?
			
			totalElapsed += (int)e.MillisecondsElapsed;

			/* if we're presently waiting, make sure
			   enough time has gone by.  otherwise
			   return */
			if (totalElapsed < sleepUntil * 166 /* 166 = 1/6th of a second */)
				return;

			totalElapsed = 0;

			while (current_action < actions.Length) {
				TriggerAction action = actions[current_action];

				current_action ++;

				switch (action.Action) {
				case 0: /* no action */
					break;
				case 1:
					sleepUntil = (int)action.Delay;
					return;
				case 2:
					GuiUtil.PlaySound (screen.Mpq, prefix + "\\" + scenario.GetMapString ((int)action.WavIndex));
					sleepUntil = (int)action.Delay;
					return;
				case 3:
					screen.SetTransmissionText (scenario.GetMapString ((int)action.TextIndex));
					break;
				case 4:
					screen.SetObjectives (scenario.GetMapString ((int)action.TextIndex));
					break;
				case 5:
					Console.WriteLine ("show portrait:");
					Console.WriteLine ("location = {0}, textindex = {1}, wavindex = {2}, delay = {3}, group1 = {4}, group2 = {5}, unittype = {6}, action = {7}, switch = {8}, flags = {9}",
							   action.Location,
							   action.TextIndex,
							   action.WavIndex,
							   action.Delay,
							   action.Group1,
							   action.Group2,
							   action.UnitType,
							   action.Action,
							   action.Switch,
							   action.Flags);
					screen.ShowPortrait ((int)action.UnitType, (int)action.Group1);
					Console.WriteLine (scenario.GetMapString ((int)action.TextIndex));
					break;
				case 6:
					screen.HidePortrait ((int)action.Group1);
					break;
				case 7:
					Console.WriteLine ("Display Speaking Portrait(Slot, Time)");
					Console.WriteLine (scenario.GetMapString ((int)action.TextIndex));
					break;
				case 8:
					Console.WriteLine ("Transmission(Text, Slot, Time, Modifier, Wave, WavTime)");
					screen.SetTransmissionText (scenario.GetMapString ((int)action.TextIndex));
					screen.HighlightPortrait ((int)action.Group1);
					GuiUtil.PlaySound (screen.Mpq, prefix + "\\" + scenario.GetMapString ((int)action.WavIndex));
					sleepUntil = (int)action.Delay;
					return;
				default:
					break;
				}
			}
		}
	}

	public class EstablishingShot : MarkupScreen {
		string markup_resource;
		string scenario_prefix;

		public EstablishingShot (string markup_resource, string scenario_prefix, Mpq mpq) : base (mpq)
		{
			this.markup_resource = markup_resource;
			this.scenario_prefix = scenario_prefix;
		}

		protected override void LoadMarkup ()
		{
			AddMarkup ((Stream)mpq.GetResource (markup_resource));
		}

		protected override void MarkupFinished ()
		{
			Game.Instance.SwitchToScreen (ReadyRoomScreen.Create (mpq, scenario_prefix));
		}
	}
}
