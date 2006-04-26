using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{

	public class GameScreen : UIScreen
	{
		//Mpq scenario_mpq;

		Surface hud;
		Chk scenario;

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
		const int TARG_CURSOR_Y = 0;
		const int TARG_CURSOR_R = 0;

		CursorAnimator[] TargetCursors;

		//byte[] unit_palette;
		byte[] tileset_palette;

		//		Player[] players;

		public GameScreen (Mpq mpq,
				   Mpq scenario_mpq,
				   Chk scenario) : base (mpq)
		{
			this.effectpal_path = "game\\tblink.pcx";
			this.arrowgrp_path = "cursor\\MagG.grp";
			this.fontpal_path = "game\\tfontgam.pcx";
			//this.scenario_mpq = scenario_mpq;
			this.scenario = scenario;
			ScrollCursors = new CursorAnimator[8];
		}

		Surface[] starfield_layers;

		void PaintStarfield (Surface surf, DateTime dt)
		{
			float scroll_factor = 1.0f;
			float[] factors = new float[starfield_layers.Length];

			for (int i = 0; i < starfield_layers.Length; i ++) {
				factors[i] = scroll_factor;
				scroll_factor *= 0.75f;
			}

			for (int i = starfield_layers.Length - 1; i >= 0; i --) {
				int scroll_x = (int)(topleft_x * factors[i]);
				int scroll_y = (int)(topleft_y * factors[i]);

				if (scroll_x > Game.SCREEN_RES_X) scroll_x %= Game.SCREEN_RES_X;
				if (scroll_y > Game.SCREEN_RES_Y) scroll_y %= Game.SCREEN_RES_Y;

				surf.Blit (starfield_layers[i],
					   new Rectangle (new Point (0,0),
							  new Size (Game.SCREEN_RES_X - scroll_x,
								    Game.SCREEN_RES_Y - scroll_y)),
					   new Rectangle (new Point (scroll_x, scroll_y),
							  new Size (Game.SCREEN_RES_X - scroll_x,
								    Game.SCREEN_RES_Y - scroll_y)));

				if (scroll_x != 0)
					surf.Blit (starfield_layers[i],
						   new Rectangle (new Point (Game.SCREEN_RES_X - scroll_x, 0),
								  new Size (scroll_x, Game.SCREEN_RES_Y - scroll_y)),
						   new Rectangle (new Point (0, scroll_y),
								  new Size (scroll_x, Game.SCREEN_RES_Y - scroll_y)));

				if (scroll_y != 0)
					surf.Blit (starfield_layers[i],
						   new Rectangle (new Point (0, Game.SCREEN_RES_Y - scroll_y),
								  new Size (Game.SCREEN_RES_X - scroll_x, scroll_y)),
						   new Rectangle (new Point (scroll_x, 0),
								  new Size (Game.SCREEN_RES_X - scroll_x, scroll_y)));

				if (scroll_x != 0 || scroll_y != 0)
					surf.Blit (starfield_layers[i],
						   new Rectangle (new Point (Game.SCREEN_RES_X - scroll_x, Game.SCREEN_RES_Y - scroll_y),
								  new Size (scroll_x, scroll_y)),
						   new Rectangle (new Point (0, 0),
								  new Size (scroll_x, scroll_y)));
			}
		}

		Surface map_surf;

		void PaintMap (Surface surf, DateTime dt)
		{
			surf.Blit (map_surf,
				   new Rectangle (new Point (0,0),
						  new Size (Game.SCREEN_RES_X - topleft_x,
							    Game.SCREEN_RES_Y - topleft_y)),
				   new Rectangle (new Point (topleft_x, topleft_y),
						  new Size (Game.SCREEN_RES_X,
							    Game.SCREEN_RES_Y)));
		}

		void PaintHud (Surface surf, DateTime dt)
		{
			surf.Blit (hud);
		}

		void PaintMinimap (Surface surf, DateTime dt)
		{
			Rectangle rect = new Rectangle (new Point ((int)((float)topleft_x / (float)map_surf.Width * MINIMAP_WIDTH + MINIMAP_X),
								   (int)((float)topleft_y / (float)map_surf.Height * MINIMAP_HEIGHT + MINIMAP_Y)),
							new Size ((int)((float)Game.SCREEN_RES_X / (float)map_surf.Width * MINIMAP_WIDTH),
								  (int)((float)Game.SCREEN_RES_Y / (float)map_surf.Height * MINIMAP_HEIGHT)));

			surf.DrawBox (rect, Color.Green);
		}

		public override void AddToPainter (Painter painter)
		{
			base.AddToPainter (painter);

			painter.Add (Layer.Hud, PaintHud);
			painter.Add (Layer.Hud, PaintMinimap);

			if (scenario.Tileset == Tileset.Platform)
				painter.Add (Layer.Background, PaintStarfield);

			painter.Add (Layer.Map, PaintMap);
			SpriteManager.AddToPainter (painter);
			painter.Add (Layer.Background, ScrollPainter);
		}

		public override void RemoveFromPainter (Painter painter)
		{
			base.RemoveFromPainter (painter);
			painter.Remove (Layer.Hud, PaintHud);
			painter.Remove (Layer.Hud, PaintMinimap);

			if (scenario.Tileset == Tileset.Platform)
				painter.Remove (Layer.Background, PaintStarfield);

			painter.Remove (Layer.Map, PaintMap);
			SpriteManager.RemoveFromPainter (painter);

			painter.Remove (Layer.Background, ScrollPainter);
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Pcx pcx = new Pcx ();
			pcx.ReadFromStream ((Stream)mpq.GetResource ("game\\tunit.pcx"), -1, -1);
			//unit_palette = pcx.Palette;

			pcx = new Pcx ();
			pcx.ReadFromStream ((Stream)mpq.GetResource ("tileset\\badlands\\dark.pcx"), 0, 0);
			tileset_palette = pcx.Palette;

			hud = GuiUtil.SurfaceFromStream ((Stream)mpq.GetResource (String.Format (Builtins.Game_ConsolePcx,
												 Util.RaceCharLower[(int)Game.Instance.Race])),
							 254, 0);

			if (scenario.Tileset == Tileset.Platform) {
				Spk starfield = (Spk)mpq.GetResource ("parallax\\star.spk");

				starfield_layers = new Surface [starfield.Layers.Length];
				for (int i = 0; i < starfield_layers.Length; i ++) {
					starfield_layers[i] = new Surface (Game.SCREEN_RES_X, Game.SCREEN_RES_Y);

					starfield_layers[i].TransparentColor = Color.Black;

					for (int o = 0; o < starfield.Layers[i].Objects.Length; o ++) {
						ParallaxObject obj = starfield.Layers[i].Objects[o];

						starfield_layers[i].Fill (new Rectangle (new Point (obj.X, obj.Y), new Size (2,2)),
									  Color.White);
					}
				}
			}

			map_surf = MapRenderer.RenderToSurface (mpq, scenario);

			PlaceInitialUnits ();

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
				//TargetCursors[i].SetHotSpot (60, 60);
			}
		}

		void ClipTopLeft ()
		{
			if (topleft_x < 0) topleft_x = 0;
			if (topleft_y < 0) topleft_y = 0;

			if (topleft_x > map_surf.Width - Game.SCREEN_RES_X) topleft_x = map_surf.Width - Game.SCREEN_RES_X;
			if (topleft_y > map_surf.Height - Game.SCREEN_RES_Y) topleft_y = map_surf.Height - Game.SCREEN_RES_Y;
		}

		public void ScrollPainter (Surface surf, DateTime dt)
		{
			topleft_x += horiz_delta;
			topleft_y += vert_delta;

			ClipTopLeft ();

			SpriteManager.SetUpperLeft (topleft_x, topleft_y);
		}

		bool buttonDownInMinimap;

		void Recenter (int x, int y)
		{
			topleft_x = x - Game.SCREEN_RES_X / 2;
			topleft_y = y - Game.SCREEN_RES_Y / 2;

			ClipTopLeft ();
		}

		void RecenterFromMinimap (int x, int y)
		{
			int map_x = (int)((float)(x - MINIMAP_X) / MINIMAP_WIDTH * map_surf.Width);
			int map_y = (int)((float)(y - MINIMAP_Y) / MINIMAP_HEIGHT * map_surf.Height);

			Recenter (map_x, map_y);
		}

		public override void MouseButtonDown (MouseButtonEventArgs args)
		{
			if (args.X > MINIMAP_X && args.X < MINIMAP_X + MINIMAP_WIDTH &&
			    args.Y > MINIMAP_Y && args.Y < MINIMAP_Y + MINIMAP_HEIGHT) {
				RecenterFromMinimap (args.X, args.Y);
				buttonDownInMinimap = true;
			}
		}
		
		public override void MouseButtonUp (MouseButtonEventArgs args)
		{
			if (buttonDownInMinimap)
				buttonDownInMinimap = false;
		}

		public override void PointerMotion (MouseMotionEventArgs args)
		{
			if (buttonDownInMinimap) {
				RecenterFromMinimap (args.X, args.Y);
			}
			else {
				if (args.X < MOUSE_MOVE_BORDER) {
					horiz_delta = -SCROLL_DELTA;
				}
				else if (args.X + MOUSE_MOVE_BORDER > Game.SCREEN_RES_X) {
					horiz_delta = SCROLL_DELTA;
				}
				else {
					horiz_delta = 0;
				}

				if (args.Y < MOUSE_MOVE_BORDER) {
					vert_delta = -SCROLL_DELTA;
				}
				else if (args.Y + MOUSE_MOVE_BORDER > Game.SCREEN_RES_Y) {
					vert_delta = SCROLL_DELTA;
				}
				else {
					vert_delta = 0;
				}

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

				/* are we over a unit?  if so, display */
				foreach (Sprite s in SpriteManager.sprites) {
					int sx, sy;
					s.GetPosition (out sx, out sy);

					if (args.X + topleft_x > sx && args.X + topleft_x <= sx + 10 /* XXX */
					    && args.Y + topleft_y > sy && args.Y + topleft_y <= sy + 10 /* XXX */)
						Game.Instance.Cursor = TargetCursors[TARG_CURSOR_G];
				}
			}
		}

		public override void KeyboardUp (KeyboardEventArgs args)
		{
			if (args.Key == Key.RightArrow
			    || args.Key == Key.LeftArrow) {
				horiz_delta = 0;
			}
			else if (args.Key == Key.UpArrow
				 || args.Key == Key.DownArrow) {
				vert_delta = 0;
			}
		}

		public override void KeyboardDown (KeyboardEventArgs args)
		{
			switch (args.Key) {
			case Key.F10:
				GameMenuDialog d = new GameMenuDialog (this, mpq);

				d.ReturnToGame += delegate () { DismissDialog (); };
				ShowDialog (d);
				break;

			case Key.RightArrow:
				horiz_delta = SCROLL_DELTA;
				break;
			case Key.LeftArrow:
				horiz_delta = -SCROLL_DELTA;
				break;
			case Key.DownArrow:
				vert_delta = SCROLL_DELTA;
				break;
			case Key.UpArrow:
				vert_delta = -SCROLL_DELTA;
				break;
			}
		}

		void PlaceInitialUnits ()
		{
			List<UnitInfo> units = scenario.Units;

			List<UnitInfo> startLocations = new List<UnitInfo>();

			foreach (UnitInfo unit in units) {
				Sprite sprite = null;

				if (unit.unit_id == 0xffff)
					break;

				int flingy_id = GlobalResources.Instance.UnitsDat.GetFlingyId (unit.unit_id);

				/* we handle start locations in a special way, below */
				if (flingy_id == 140) {
					startLocations.Add (unit);
					continue;
				}

				int sprite_id = GlobalResources.Instance.FlingyDat.GetSpriteId (flingy_id);

				sprite = SpriteManager.CreateSprite (mpq, sprite_id, tileset_palette, unit.x, unit.y);

				sprite.RunAnimation (AnimationType.Init);
			}

			/* now place the starting units (a base and
			 * three harvesters), if we aren't using map
			 * settings */
			foreach (UnitInfo sl in startLocations) {
				Console.WriteLine ("Creating sprite for start location");

				/* terran command center = 252,
				   protos nexus = 211 */
				Sprite sprite = SpriteManager.CreateSprite (mpq, 211, tileset_palette, sl.x, sl.y);

				sprite.RunAnimation (AnimationType.Init);
			}

			/* for now assume the player is at startLocations[0], and center the view there */
			Recenter (startLocations[0].x, startLocations[0].y);
		}
	}
}
