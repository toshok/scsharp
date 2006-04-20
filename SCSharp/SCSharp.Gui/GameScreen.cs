//#define SHOW_STARFIELD
#define SHOW_MAP

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using SdlDotNet;
using System.Drawing;

namespace SCSharp {

	public class GameScreen : UIScreen {
		Mpq scenario_mpq;

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

		const int CURSOR_UL = 0;
		const int CURSOR_U  = 1;
		const int CURSOR_UR = 2;
		const int CURSOR_R  = 3;
		const int CURSOR_DR = 4;
		const int CURSOR_D  = 5;
		const int CURSOR_DL = 6;
		const int CURSOR_L  = 7;

		CursorAnimator[] ScrollCursors;

		byte[] unit_palette;
		byte[] tileset_palette;

		public GameScreen (Mpq mpq,
				   Mpq scenario_mpq,
				   Chk scenario) : base (mpq)
		{
			this.effectpal_path = "game\\tblink.pcx";
			this.arrowgrp_path = "cursor\\arrow.grp";
			this.fontpal_path = "game\\tfontgam.pcx";
			this.scenario_mpq = scenario_mpq;
			this.scenario = scenario;
			ScrollCursors = new CursorAnimator[8];
		}

#if SHOW_STARFIELD
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

#elif SHOW_MAP
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
#endif

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

#if SHOW_STARFIELD
			painter.Add (Layer.Background, PaintStarfield);
#elif SHOW_MAP
			painter.Add (Layer.Map, PaintMap);
			SpriteManager.AddToPainter (painter);
#endif
			painter.Add (Layer.Background, ScrollPainter);
		}

		public override void RemoveFromPainter (Painter painter)
		{
			base.RemoveFromPainter (painter);
			painter.Remove (Layer.Hud, PaintHud);
			painter.Remove (Layer.Hud, PaintMinimap);

#if SHOW_STARFIELD
			painter.Remove (Layer.Background, PaintStarfield);
#elif SHOW_MAP
			painter.Remove (Layer.Map, PaintMap);
			SpriteManager.RemoveFromPainter (painter);
#endif
			painter.Remove (Layer.Background, ScrollPainter);
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Pcx pcx = new Pcx ();
			pcx.ReadFromStream ((Stream)mpq.GetResource ("game\\tunit.pcx"), -1, -1);
			unit_palette = pcx.Palette;

			pcx = new Pcx ();
			pcx.ReadFromStream ((Stream)mpq.GetResource ("tileset\\badlands\\dark.pcx"), 0, 0);
			tileset_palette = pcx.Palette;

			hud = GuiUtil.SurfaceFromStream ((Stream)mpq.GetResource (String.Format (Builtins.Game_ConsolePcx,
												 Util.RaceCharLower[(int)Game.Instance.Race])),
							 254, 0);

#if SHOW_STARFIELD
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

#elif SHOW_MAP
			map_surf = MapRenderer.RenderToSurface (mpq, scenario);

			PlaceInitialUnits ();
#else
			map_surf = new Surface (Game.SCREEN_RES_X, Game.SCREEN_RES_Y);
			map_surf.Fill (new Rectangle (0, 0, map_surf.Width - 1, map_surf.Height - 1), Color.Black);
#endif

			// load the cursors we'll show when scrolling with the mouse
			string[] cursornames = new string[] {
				"cursor\\ScrollUL.grp",
				"cursor\\ScrollU.grp",
				"cursor\\ScrollUR.grp",
				"cursor\\ScrollR.grp",
				"cursor\\ScrollDR.grp",
				"cursor\\ScrollD.grp",
				"cursor\\ScrollDL.grp",
				"cursor\\ScrollL.grp"
			};
			ScrollCursors = new CursorAnimator [cursornames.Length];
			for (int i = 0; i < cursornames.Length; i ++) {
				ScrollCursors[i] = new CursorAnimator ((Grp)mpq.GetResource (cursornames[i]),
								       effectpal.Palette);
				ScrollCursors[i].SetHotSpot (60, 60);
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

		void RecenterFromMinimap (int x, int y)
		{
			int map_x = (int)((float)(x - MINIMAP_X) / MINIMAP_WIDTH * map_surf.Width);
			int map_y = (int)((float)(y - MINIMAP_Y) / MINIMAP_HEIGHT * map_surf.Height);

			topleft_x = map_x - Game.SCREEN_RES_X / 2;
			topleft_y = map_y - Game.SCREEN_RES_Y / 2;

			ClipTopLeft ();
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
						Game.Instance.Cursor = ScrollCursors[CURSOR_UL];
					else if (vert_delta > 0)
						Game.Instance.Cursor = ScrollCursors[CURSOR_DL];
					else
						Game.Instance.Cursor = ScrollCursors[CURSOR_L];
				else if (horiz_delta > 0)
					if (vert_delta < 0)
						Game.Instance.Cursor = ScrollCursors[CURSOR_UR];
					else if (vert_delta > 0)
						Game.Instance.Cursor = ScrollCursors[CURSOR_DR];
					else
						Game.Instance.Cursor = ScrollCursors[CURSOR_R];
				else
					if (vert_delta < 0)
						Game.Instance.Cursor = ScrollCursors[CURSOR_U];
					else if (vert_delta > 0)
						Game.Instance.Cursor = ScrollCursors[CURSOR_D];
					else
						Game.Instance.Cursor = Cursor;
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

			foreach (UnitInfo unit in units) {
				Sprite sprite = null;

				switch (unit.unit_id)
				{
				case 176:
					sprite = SpriteManager.CreateSprite (mpq, 279, tileset_palette, unit.x, unit.y);
					break;
				case 177:
					sprite = SpriteManager.CreateSprite (mpq, 280, tileset_palette, unit.x, unit.y);
					break;
				case 178:
					sprite = SpriteManager.CreateSprite (mpq, 281, tileset_palette, unit.x, unit.y);
					break;
				case 188:
					sprite = SpriteManager.CreateSprite (mpq, 275, tileset_palette, unit.x, unit.y);
					break;
				default:
					break;
				}

				if (sprite != null)
					sprite.RunAnimation (20);
			}
		}
	}
}
