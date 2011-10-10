//
// SCSharp.UI.GameScreen
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
using MonoMac.CoreGraphics;
using MonoMac.CoreAnimation;

using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
{

	public class GameScreen : UIScreen
	{
		enum HudLabels {
			UnitName,
			ResourceUsed,
			ResourceProvided,
			ResourceTotal,
			ResourceMax,

			Count
		}

		//Mpq scenario_mpq;

		Chk scenario;
		Got template;

		/* the deltas associated with scrolling */
		int horiz_delta;
		int vert_delta;

		/* the x,y of the topleft of the screen */
		int topleft_x, topleft_y;

		/* magic number alert.. */
		const int MINIMAP_X = 7 ;
		const int MINIMAP_Y = 349 ;
		const int MINIMAP_WIDTH = 125;
		const int MINIMAP_HEIGHT = 125;

		const int SCROLL_DELTA = 15;
		const int MOUSE_MOVE_BORDER = 10;

		const int SCROLL_CURSOR_UL = 0;
		const int SCROLL_CURSOR_U  = 1;
		const int SCROLL_CURSOR_UR = 2;
		const int SCROLL_CURSOR_R  = 3;
		const int SCROLL_CURSOR_DR = 4;
		const int SCROLL_CURSOR_D  = 5;
		const int SCROLL_CURSOR_DL = 6;
		const int SCROLL_CURSOR_L  = 7;

		CursorAnimator[] ScrollCursors;

		const int TARG_CURSOR_G = 0;
		const int TARG_CURSOR_Y = 1;
		const int TARG_CURSOR_R = 2;

		CursorAnimator[] TargetCursors;

		const int MAG_CURSOR_G = 0;
		const int MAG_CURSOR_Y = 1;
		const int MAG_CURSOR_R = 2;

		CursorAnimator[] MagCursors;

		Tbl statTxt;
		Grp wireframe;
		Grp cmdicons;
		byte[] cmdicon_palette;

		//byte[] unit_palette;
		byte[] tileset_palette;

		//		Player[] players;

		List<Unit> units;

		ImageElement hudElement;
		GrpElement wireframeElement;
		MovieElement portraitElement;
		MapRenderer mapRenderer;
		GrpButtonElement[] cmdButtonElements;
		LabelElement[] labelElements;

		public GameScreen (Mpq mpq,
				   Mpq scenario_mpq,
				   Chk scenario,
				   Got template) : base (mpq)
		{
			effectpal_path = "game\\tblink.pcx";
			arrowgrp_path = "cursor\\arrow.grp";
			fontpal_path = "game\\tfontgam.pcx";
			//this.scenario_mpq = scenario_mpq;
			this.scenario = scenario;
			this.template = template;
			ScrollCursors = new CursorAnimator[8];
		}

		public GameScreen (Mpq mpq,
				   string prefix,
				   Chk scenario) : base (mpq)
		{
			this.effectpal_path = "game\\tblink.pcx";
			this.arrowgrp_path = "cursor\\arrow.grp";
			this.fontpal_path = "game\\tfontgam.pcx";
			//this.scenario_mpq = scenario_mpq;
			this.scenario = scenario;
			ScrollCursors = new CursorAnimator[8];
		}

		// FIXME: we need the starfield!
#if notyet
		Surface[] starfield_layers;

		void PaintStarfield (DateTime dt)
		{
			float scroll_factor = 1.0f;
			float[] factors = new float[starfield_layers.Length];

			for (int i = 0; i < starfield_layers.Length; i ++) {
				factors[i] = scroll_factor;
				scroll_factor *= 0.75f;
			}

			Rectangle dest;
			Rectangle source;

			for (int i = starfield_layers.Length - 1; i >= 0; i --) {
				int scroll_x = (int)(topleft_x * factors[i]);
				int scroll_y = (int)(topleft_y * factors[i]);

				if (scroll_x > Painter.SCREEN_RES_X) scroll_x %= Painter.SCREEN_RES_X;
				if (scroll_y > 375) scroll_y %= 375;

				
				dest = new Rectangle (new Point (0,0),
						      new Size (Painter.SCREEN_RES_X - scroll_x,
								375 - scroll_y));
				source = dest;
				source.X += scroll_x;
				source.Y += scroll_y;

				Painter.Blit (starfield_layers[i],
					      dest, source);

				if (scroll_x != 0) {
					dest = new Rectangle (new Point (Painter.SCREEN_RES_X - scroll_x, 0),
							      new Size (scroll_x, 375 - scroll_y));
					source = dest;
					source.X -= Painter.SCREEN_RES_X - scroll_x;
					source.Y += scroll_y;

					Painter.Blit (starfield_layers[i],
						      dest, source);
				}

				if (scroll_y != 0) {
					dest = new Rectangle (new Point (0, 375 - scroll_y),
							      new Size (Painter.SCREEN_RES_X - scroll_x, scroll_y));
					source = dest;
					source.X += scroll_x;
					source.Y -= 375 - scroll_y;

					Painter.Blit (starfield_layers[i],
						      dest, source);
				}

				if (scroll_x != 0 || scroll_y != 0) {
					dest = new Rectangle (new Point (Painter.SCREEN_RES_X - scroll_x, 375 - scroll_y),
							      new Size (scroll_x, scroll_y));
					source = dest;
					source.X -= Painter.SCREEN_RES_X - scroll_x;
					source.Y -= 375 - scroll_y;

					Painter.Blit (starfield_layers[i],
						      dest, source);
				}
			}
		}
#endif
		
		ushort[] button_xs = new ushort[] { 506, 552, 598 };
		ushort[] button_ys = new ushort[] { 360, 400, 440 };

#if notyet
		void PaintMinimap (DateTime dt)
		{
			Rectangle rect = new Rectangle (new Point ((int)((float)topleft_x / (float)mapRenderer.MapWidth * MINIMAP_WIDTH + MINIMAP_X),
								   (int)((float)topleft_y / (float)mapRenderer.MapHeight * MINIMAP_HEIGHT + MINIMAP_Y)),
							new Size ((int)((float)Painter.SCREEN_RES_X / (float)mapRenderer.MapWidth * MINIMAP_WIDTH),
								  (int)((float)Painter.SCREEN_RES_Y / (float)mapRenderer.MapHeight * MINIMAP_HEIGHT)));

			Painter.DrawBox (rect, Color.Green);
		}
#endif

		public override void AddToPainter ()
		{
			base.AddToPainter ();
			
			InsertSublayerBelow (mapRenderer.MapLayer, hudElement.Layer);
			
			Game.Instance.Tick += ScrollTick;
			
//			Painter.Add (Layer.Hud, PaintMinimap);

//			if (scenario.Tileset == Tileset.Platform)
//				Painter.Add (Layer.Background, PaintStarfield);

//			SpriteManager.AddToPainter ();
		}

		public override void RemoveFromPainter ()
		{
			base.RemoveFromPainter ();

//			Painter.Remove (Layer.Hud, PaintMinimap);

//			if (scenario.Tileset == Tileset.Platform)
//				Painter.Remove (Layer.Background, PaintStarfield);

//			SpriteManager.RemoveFromPainter ();
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			/* create the element corresponding to the hud */
			hudElement = new ImageElement (this, 0, 0, 640, 480, TranslucentIndex);
			hudElement.Palette = fontpal.RGBData;
			hudElement.Text = String.Format (Builtins.Game_ConsolePcx, Util.RaceCharLower[(int)Game.Instance.Race]);
			hudElement.Visible = true;
			Elements.Add (hudElement);

			/* create the portrait playing area */
			portraitElement = new MovieElement (this, 415, 415, 48, 48, false);
			portraitElement.Visible = true;
			Elements.Add (portraitElement);
			
			Pcx pcx = new Pcx ();
			pcx.ReadFromStream ((Stream)mpq.GetResource ("game\\tunit.pcx"), -1, -1);
			//unit_palette = pcx.Palette;

			pcx = new Pcx ();
			pcx.ReadFromStream ((Stream)mpq.GetResource ("tileset\\badlands\\dark.pcx"), 0, 0);
			tileset_palette = pcx.Palette;

#if notyet
			if (scenario.Tileset == Tileset.Platform) {
				Spk starfield = (Spk)mpq.GetResource ("parallax\\star.spk");

				starfield_layers = new Surface [starfield.Layers.Length];
				for (int i = 0; i < starfield_layers.Length; i ++) {
					starfield_layers[i] = new Surface (Painter.SCREEN_RES_X, Painter.SCREEN_RES_Y);

					starfield_layers[i].TransparentColor = Color.Black;

					for (int o = 0; o < starfield.Layers[i].Objects.Length; o ++) {
						ParallaxObject obj = starfield.Layers[i].Objects[o];

						starfield_layers[i].Fill (new Rectangle (new Point (obj.X, obj.Y), new Size (2,2)),
									  Color.White);
					}
				}
			}
#endif

			mapRenderer = new MapRenderer (mpq, scenario, 640/*Painter.SCREEN_RES_X*/, 480/*Painter.SCREEN_RES_Y*/);
			mapRenderer.MapLayer.AnchorPoint = new PointF (0, 0);
			
#if notyet
			// load the cursors we'll show when scrolling with the mouse
			string[] cursornames = new string[] {
				"cursor\\ScrollUL.grp",
				"cursor\\ScrollU.grp",
				"cursor\\ScrollUR.grp",
				"cursor\\ScrollR.grp",
				"cursor\\ScrollDR.grp",
				"cursor\\ScrollD.grp",
				"cursor\\ScrollDL.grp",
				"cursor\\ScrollL.grp",
			};
			ScrollCursors = new CursorAnimator [cursornames.Length];
			for (int i = 0; i < cursornames.Length; i ++) {
				ScrollCursors[i] = new CursorAnimator ((Grp)mpq.GetResource (cursornames[i]),
								       effectpal.Palette);
				ScrollCursors[i].SetHotSpot (60, 60);
			}

			// load the mag cursors
			string[] magcursornames = new string[] {
				"cursor\\MagG.grp",
				"cursor\\MagY.grp",
				"cursor\\MagR.grp"
			};
			MagCursors = new CursorAnimator [magcursornames.Length];
			for (int i = 0; i < magcursornames.Length; i ++) {
				MagCursors[i] = new CursorAnimator ((Grp)mpq.GetResource (magcursornames[i]),
								    effectpal.Palette);
				MagCursors[i].SetHotSpot (60, 60);
			}

			// load the targeting cursors
			string[] targetcursornames = new string[] {
				"cursor\\TargG.grp",
				"cursor\\TargY.grp",
				"cursor\\TargR.grp"
			};
			TargetCursors = new CursorAnimator [targetcursornames.Length];
			for (int i = 0; i < targetcursornames.Length; i ++) {
				TargetCursors[i] = new CursorAnimator ((Grp)mpq.GetResource (targetcursornames[i]),
								       effectpal.Palette);
				TargetCursors[i].SetHotSpot (60, 60);
			}
#endif
			/* the following could be made global to speed up the entry to the game screen.. */
			statTxt = (Tbl)mpq.GetResource ("rez\\stat_txt.tbl");

			// load the wireframe image info
			wireframe = (Grp)mpq.GetResource ("unit\\wirefram\\wirefram.grp");

			// load the command icons
			cmdicons = (Grp)mpq.GetResource ("unit\\cmdbtns\\cmdicons.grp");
			pcx = new Pcx ();
			pcx.ReadFromStream ((Stream)mpq.GetResource ("unit\\cmdbtns\\ticon.pcx"), 0, 0);
			cmdicon_palette = pcx.Palette;

			// create the wireframe display element
			wireframeElement = new GrpElement (this, wireframe, cmdicon_palette, 170, 390);
			wireframeElement.Visible = false;
			Elements.Add (wireframeElement);

			labelElements = new LabelElement [(int)HudLabels.Count];

			labelElements[(int)HudLabels.UnitName] = new LabelElement (this, fontpal.Palette,
										   GuiUtil.GetFonts (Mpq)[1],
										   254, 390);
			labelElements[(int)HudLabels.ResourceUsed] = new LabelElement (this, fontpal.Palette,
										       GuiUtil.GetFonts (Mpq)[0],
										       292, 420);
			labelElements[(int)HudLabels.ResourceProvided] = new LabelElement (this, fontpal.Palette,
											   GuiUtil.GetFonts (Mpq)[0],
											   292, 434);
			labelElements[(int)HudLabels.ResourceTotal] = new LabelElement (this, fontpal.Palette,
											GuiUtil.GetFonts (Mpq)[0],
											292, 448);
			labelElements[(int)HudLabels.ResourceMax] = new LabelElement (this, fontpal.Palette,
										      GuiUtil.GetFonts (Mpq)[0],
										      292, 462);

			for (int i = 0; i < labelElements.Length; i ++)
				Elements.Add (labelElements[i]);

			cmdButtonElements = new GrpButtonElement[9];
			int x = 0;
			int y = 0;
			for (int i = 0; i < cmdButtonElements.Length; i ++) {
				cmdButtonElements[i] = new GrpButtonElement (this, cmdicons, cmdicon_palette, button_xs[x], button_ys[y]);
				x++;
				if (x == 3) {
					x = 0;
					y++;
				}
				cmdButtonElements[i].Visible = false;
				Elements.Add (cmdButtonElements[i]);
			}

			PlaceInitialUnits ();
		}

		void ClipTopLeft ()
		{
			if (topleft_x < 0) topleft_x = 0;
			if (topleft_y < 0) topleft_y = 0;

//			if (topleft_x > mapRenderer.MapWidth - Painter.SCREEN_RES_X) topleft_x = mapRenderer.MapWidth - Painter.SCREEN_RES_X;
//			if (topleft_y > mapRenderer.MapHeight - Painter.SCREEN_RES_Y) topleft_y = mapRenderer.MapHeight - Painter.SCREEN_RES_Y;
		}

		void UpdateCursor ()
		{
#if notyet
			if (mouseOverElement == null) {
				/* are we over a unit?  if so, display the mag cursor */
				unitUnderCursor = null;
				for (int i = 0; i < units.Count; i ++) {
					Unit u = units[i];
					Sprite s = u.Sprite;

					int sx, sy;

					s.GetPosition (out sx, out sy);

					int cursor_x = Game.Instance.Cursor.X + topleft_x;
					int cursor_y = Game.Instance.Cursor.Y + topleft_y;

					int half_width = u.Width / 2;
					int half_height = u.Height / 2;

					if (cursor_x < sx + half_width && cursor_x > sx - half_width
					    && cursor_y < sy + half_height && cursor_y > sy - half_height) {
						Game.Instance.Cursor = MagCursors[MAG_CURSOR_G];
						unitUnderCursor = u;
						break;
					}
				}
			}
#endif
		}

		int scroll_elapsed;

		public void ScrollTick (object sender, TickEventArgs e)
		{
			scroll_elapsed += (int)e.MillisecondsElapsed;

			if (scroll_elapsed < 6/*XXX*/)
				return;

			scroll_elapsed = 0;

			if (horiz_delta == 0 && vert_delta == 0)
				return;

			int old_topleft_x = topleft_x;
			int old_topleft_y = topleft_y;

			topleft_x += horiz_delta;
			topleft_y += vert_delta;

			ClipTopLeft ();

			if (old_topleft_x == topleft_x
			    && old_topleft_y == topleft_y)
				return;

			SpriteManager.SetUpperLeft (topleft_x, topleft_y);
			mapRenderer.SetUpperLeft (topleft_x, topleft_y);

			UpdateCursor ();

#if notyet
			Painter.Invalidate (new Rectangle (new Point (0,0),
							   new Size (Painter.SCREEN_RES_X, 375)));
#endif
		}

		bool buttonDownInMinimap;
		Unit unitUnderCursor;
		Unit selectedUnit;

		void Recenter (int x, int y)
		{
			topleft_x = (int)(x - Bounds.Width / 2);
			topleft_y = (int)(y - Bounds.Height / 2);

			ClipTopLeft ();

			SpriteManager.SetUpperLeft (topleft_x, topleft_y);
			mapRenderer.SetUpperLeft (topleft_x, topleft_y);

			UpdateCursor ();


//			Painter.Invalidate (new Rectangle (new Point (0,0),
//							   new Size (Painter.SCREEN_RES_X, Painter.SCREEN_RES_Y)));
		}

		void RecenterFromMinimap (int x, int y)
		{
			int map_x = (int)((float)(x - MINIMAP_X) / MINIMAP_WIDTH * mapRenderer.MapWidth);
			int map_y = (int)((float)(y - MINIMAP_Y) / MINIMAP_HEIGHT * mapRenderer.MapHeight);

			Recenter (map_x, map_y);
		}

#if notyet
		public override void MouseButtonDown (MouseButtonEventArgs args)
		{
			if (mouseOverElement != null) {
				base.MouseButtonDown (args);
			}
			else if (args.X > MINIMAP_X && args.X < MINIMAP_X + MINIMAP_WIDTH &&
				 args.Y > MINIMAP_Y && args.Y < MINIMAP_Y + MINIMAP_HEIGHT) {
				RecenterFromMinimap (args.X, args.Y);
				buttonDownInMinimap = true;
			}
			else {
				if (selectedUnit != unitUnderCursor) {

					Console.WriteLine ("hey there, keyboard.modifierkeystate = {0}", Keyboard.ModifierKeyState);

					// if we have a selected unit and we
					// right click (or ctrl-click?), try
					// to move there
					if (selectedUnit != null && (Keyboard.ModifierKeyState & ModifierKeys.ShiftKeys) != 0) {
						Console.WriteLine ("And... we're walking");

						int pixel_x = args.X + topleft_x;
						int pixel_y = args.Y + topleft_y;

						// calculate the megatile
						int megatile_x = pixel_x >> 5;
						int megatile_y = pixel_y >> 5;

						Console.WriteLine ("megatile {0},{1}", megatile_x, megatile_y);

						// the mini tile
						int minitile_x = pixel_x >> 2;
						int minitile_y = pixel_y >> 2;

						Console.WriteLine ("minitile {0},{1} ({2},{3} in the megatile)",
								   minitile_x, minitile_y,
								   minitile_x - megatile_x * 8,
								   minitile_y - megatile_y * 8);

						if (selectedUnit.YesSound != null) {
							string sound_resource = String.Format ("sound\\{0}", selectedUnit.YesSound);
							Console.WriteLine ("sound_resource = {0}", sound_resource);
							GuiUtil.PlaySound (mpq, sound_resource);
						}

						selectedUnit.Move (mapRenderer, minitile_x, minitile_y);

						return;
					}
					
					portraitElement.Stop ();
					
					selectedUnit = unitUnderCursor;

					if (selectedUnit == null) {
						portraitElement.Visible = false;
						wireframeElement.Visible = false;
						for (int i = 0; i < (int)HudLabels.Count; i ++)
							labelElements[i].Visible = false;
					}
					else {
						Console.WriteLine ("selected unit: {0}", selectedUnit);
						Console.WriteLine ("selectioncircle = {0}", selectedUnit.SelectionCircleOffset);

						if (selectedUnit.Portrait == null) {
							portraitElement.Visible = false;
						}
						else {
							string portrait_resource = String.Format ("portrait\\{0}0.smk",
												  selectedUnit.Portrait);

							portraitElement.Player = new SmackerPlayer ((Stream)mpq.GetResource (portrait_resource), 1);
							portraitElement.Play ();
							portraitElement.Visible = true;
						}

						if (selectedUnit.WhatSound != null) {
							string sound_resource = String.Format ("sound\\{0}", selectedUnit.WhatSound);
							Console.WriteLine ("sound_resource = {0}", sound_resource);
							GuiUtil.PlaySound (mpq, sound_resource);
						}

						/* set up the wireframe */
						wireframeElement.Frame = selectedUnit.UnitId;
						wireframeElement.Visible = true;

						/* then display info about the selected unit */
						labelElements[(int)HudLabels.UnitName].Text = statTxt[selectedUnit.UnitId];
						if (true /* XXX unit is a building */) {
							labelElements[(int)HudLabels.ResourceUsed].Text = statTxt[820+(int)Game.Instance.Race];
							labelElements[(int)HudLabels.ResourceProvided].Text = statTxt[814+(int)Game.Instance.Race];
							labelElements[(int)HudLabels.ResourceTotal].Text = statTxt[817+(int)Game.Instance.Race];
							labelElements[(int)HudLabels.ResourceMax].Text = statTxt[823+(int)Game.Instance.Race];

							for (int i = 0; i < (int)HudLabels.Count; i ++)
								labelElements[i].Visible = true;
						}

						/* then fill in the command buttons */
						int[] cmd_indices;

						switch (selectedUnit.UnitId) {
						case 106:
							cmd_indices = new int[] { 7, -1, -1, -1, -1, 286, -1, -1, 282 };
							break;
						default:
							Console.WriteLine ("cmd_indices == -1");
							cmd_indices = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
							break;
						}

						for (int i = 0; i < cmd_indices.Length; i ++) {
							if (cmd_indices[i] == -1) {
								cmdButtonElements[i].Visible = false;
							}
							else {
								cmdButtonElements[i].Visible = true;
								cmdButtonElements[i].Frame = cmd_indices[i];
							}
						}
					}
				}
			}
		}
		
		public override void MouseButtonUp (MouseButtonEventArgs args)
		{
			if (mouseDownElement != null)
				base.MouseButtonUp (args);
			else if (buttonDownInMinimap)
				buttonDownInMinimap = false;
		}

		public override void PointerMotion (MouseMotionEventArgs args)
		{
			if (buttonDownInMinimap) {
				RecenterFromMinimap (args.X, args.Y);
			}
			else {
				base.PointerMotion (args);

				// if the mouse is over one of the
				// normal UIElements in the HUD, deal
				// with it
				if (mouseOverElement != null) {
					// XXX for now, just return.
					return;
				}

				// if the mouse is over the hud area, return
				int hudIndex = (args.X + args.Y * hudElement.Pcx.Width) * 4;
				if (hudElement.Pcx.RgbaData [hudIndex] == 0xff) {
					Game.Instance.Cursor = Cursor;
					return;
				}

				if (args.X < MOUSE_MOVE_BORDER) {
					horiz_delta = -SCROLL_DELTA;
				}
				else if (args.X + MOUSE_MOVE_BORDER > Painter.SCREEN_RES_X) {
					horiz_delta = SCROLL_DELTA;
				}
				else {
					horiz_delta = 0;
				}

				if (args.Y < MOUSE_MOVE_BORDER) {
					vert_delta = -SCROLL_DELTA;
				}
				else if (args.Y + MOUSE_MOVE_BORDER > Painter.SCREEN_RES_Y) {
					vert_delta = SCROLL_DELTA;
				}
				else {
					vert_delta = 0;
				}

				// we update the cursor to show the
				// scrolling animations here, since it
				// only happens on pointer motion.
				if (horiz_delta < 0)
					if (vert_delta < 0)
						Game.Instance.Cursor = ScrollCursors[SCROLL_CURSOR_UL];
					else if (vert_delta > 0)
						Game.Instance.Cursor = ScrollCursors[SCROLL_CURSOR_DL];
					else
						Game.Instance.Cursor = ScrollCursors[SCROLL_CURSOR_L];
				else if (horiz_delta > 0)
					if (vert_delta < 0)
						Game.Instance.Cursor = ScrollCursors[SCROLL_CURSOR_UR];
					else if (vert_delta > 0)
						Game.Instance.Cursor = ScrollCursors[SCROLL_CURSOR_DR];
					else
						Game.Instance.Cursor = ScrollCursors[SCROLL_CURSOR_R];
				else
					if (vert_delta < 0)
						Game.Instance.Cursor = ScrollCursors[SCROLL_CURSOR_U];
					else if (vert_delta > 0)
						Game.Instance.Cursor = ScrollCursors[SCROLL_CURSOR_D];
					else
						Game.Instance.Cursor = Cursor;


				UpdateCursor ();
			}
		}
#endif

		public override void KeyboardUp (NSEvent theEvent)
		{
			char key = theEvent.CharactersIgnoringModifiers[0];
			
			if (key == (char)NSKey.RightArrow
			    || key == (char)NSKey.LeftArrow) {
				horiz_delta = 0;
			}
			else if (key == (char)NSKey.UpArrow
				 || key == (char)NSKey.DownArrow) {
				vert_delta = 0;
			}
		}

		public override void KeyboardDown (NSEvent theEvent)
		{
			switch (theEvent.CharactersIgnoringModifiers[0]) {
			case (char)27: /*Key.F10:*/
				GameMenuDialog d = new GameMenuDialog (this, mpq);

				d.ReturnToGame += delegate () { DismissDialog (); };
				ShowDialog (d);
				break;
			case (char)NSKey.RightArrow:
				horiz_delta = SCROLL_DELTA;
				break;
			case (char)NSKey.LeftArrow:
				horiz_delta = -SCROLL_DELTA;
				break;
			case (char)NSKey.DownArrow:
				vert_delta = SCROLL_DELTA;
				break;
			case (char)NSKey.UpArrow:
				vert_delta = -SCROLL_DELTA;
				break;
			}
		}

		void PlaceInitialUnits ()
		{
			List<UnitInfo> unit_infos = scenario.Units;

			List<Unit> startLocations = new List<Unit>();

			units = new List<Unit>();

			foreach (UnitInfo unitinfo in unit_infos) {
				if (unitinfo.unit_id == 0xffff)
					break;

				Unit unit = new Unit (unitinfo);

				/* we handle start locations in a special way, below */
				if (unit.FlingyId == 140) {
					startLocations.Add (unit);
					continue;
				}

				//players[unitinfo.player].AddUnit (unit);

				unit.CreateSprite (mpq, tileset_palette);
				units.Add (unit);
			}

			if (template != null && (template.InitialUnits != InitialUnits.UseMapSettings)) {
				foreach (Unit sl in startLocations) {
					/* terran command center = 106,
					   zerg hatchery = 131,
					   protoss nexus = 154 */

					Unit unit = new Unit (154);
					unit.X = sl.X;
					unit.Y = sl.Y;

					unit.CreateSprite (mpq, tileset_palette);
					units.Add (unit);
				}
			}

			/* for now assume the player is at startLocations[0], and center the view there */
			Recenter (startLocations[0].X, startLocations[0].Y);
		}
	}
}
