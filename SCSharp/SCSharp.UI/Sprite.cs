//
// SCSharp.UI.Sprite
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

/* opcode descriptions and info here 
http://www.campaigncreations.org/starcraft/bible/chap4_ice_opcodes.shtml
http://www.stormcoast-fortress.net/cntt/tutorials/camsys/tilesetdependent/?PHPSESSID=7365d884cf33fc614c0b96d966872177

*/

using SdlDotNet;

using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp.UI
{

	public class Sprite
	{
		static Random rng = new Random (Environment.TickCount);

		Sprite dependentSprite;

		ushort images_entry;

		string grp_path;
		Grp grp;
		byte[] palette;

		byte[] buf;
		ushort script_start; /* the pc of the script that started us off */
		ushort pc;
		uint iscript_entry;
		ushort script_entry_offset;

		int current_frame = -1;
		int facing = 0;

		bool trace= true;

		static bool show_sprite_borders;

		Mpq mpq;

		int x;
		int y;

		Sprite parent_sprite;

		static Sprite ()
		{
#if DEBUG
			string sb = ConfigurationManager.AppSettings ["ShowSpriteBorders"];
			if (sb != null) {
				show_sprite_borders = Boolean.Parse (sb);
			}
#endif
		}

		public Sprite (Mpq mpq, int sprite_entry, byte[] palette, int x, int y)
		{
			this.mpq = mpq;
			this.palette = palette;

			images_entry = GlobalResources.Instance.SpritesDat.ImagesDatEntries [sprite_entry];
			//			Console.WriteLine ("image_dat_entry == {0}", images_entry);

			uint grp_index = GlobalResources.Instance.ImagesDat.GrpIndexes [images_entry];
			//			Console.WriteLine ("grp_index = {0}", grp_index);
			grp_path = GlobalResources.Instance.ImagesTbl[(int)grp_index-1];
			//			Console.WriteLine ("grp_path = {0}", grp_path);

			grp = (Grp)mpq.GetResource ("unit\\" + grp_path);

			iscript_entry = GlobalResources.Instance.ImagesDat.IScriptIndexes [images_entry];
			script_entry_offset = GlobalResources.Instance.IScriptBin.GetScriptEntryOffset (iscript_entry);

			Console.WriteLine ("new sprite: unit\\{0} @ {1}x{2} (image {3}, iscript id {4}, script_entry_offset {5:X})",
					   grp_path, x, y, images_entry, iscript_entry, script_entry_offset);

			this.buf = GlobalResources.Instance.IScriptBin.Contents;

			/* make sure the offset points to "SCPE" */
			if (Util.ReadDWord (buf, script_entry_offset) != 0x45504353)
				Console.WriteLine ("invalid script_entry_offset");

			SetPosition (x,y);
		}

		public Sprite (Sprite parentSprite, ushort images_entry, byte[] palette)
		{
			this.parent_sprite = parentSprite;
			this.mpq = parentSprite.mpq;
			this.palette = palette;
			this.images_entry = images_entry;

			uint grp_index = GlobalResources.Instance.ImagesDat.GrpIndexes [images_entry];

			grp_path = GlobalResources.Instance.ImagesTbl[(int)grp_index-1];

			grp = (Grp)mpq.GetResource ("unit\\" + grp_path);

			this.buf = GlobalResources.Instance.IScriptBin.Contents;
			iscript_entry = GlobalResources.Instance.ImagesDat.IScriptIndexes [images_entry];

			script_entry_offset = GlobalResources.Instance.IScriptBin.GetScriptEntryOffset (iscript_entry);

			Console.WriteLine ("new dependent sprite: unit\\{0} (image {1}, iscript id {2}, script_entry_offset {3:X})",
					   grp_path, images_entry, iscript_entry, script_entry_offset);

			/* make sure the offset points to "SCEP" */
			if (Util.ReadDWord (buf, script_entry_offset) != 0x45504353)
				Console.WriteLine ("invalid script_entry_offset");

			int x, y;
			parentSprite.GetPosition (out x, out y);
			SetPosition (x,y);
		}


		enum IScriptOpcode {
			playfram          = 0x00,   // short frame */
			playframtile      = 0x01,   // short frame */
			sethorpos         = 0x02,   // byte byte   */
			setvertpos        = 0x03,   // byte byte */
			setpos            = 0x04,   // byte byte, byte byte */
			wait              = 0x05,   // byte byte */
			waitrand          = 0x06,   // byte byte, byte byte */
			_goto             = 0x07,   // short label */
			imgol             = 0x08,   // short imageid, byte byte, byte byte */
			imgul             = 0x09,   // short imageid, byte byte, byte byte */
			imgolorig         = 0x0A,   // short imageid */
			switchul          = 0x0B,   // short imageid */
			__0c              = 0x0C,
			imgoluselo        = 0x0D,   // short imageid, byte byte, byte byte */
			imguluselo        = 0x0E,   // short imageid, byte byte, byte byte
			sprol             = 0x0F,   //   short spriteid, byte byte, byte byte
			highsprol         = 0x10,   //   short spriteid, byte byte, byte byte
			lowsprul          = 0x11,   //   short spriteid, byte byte, byte byte
			uflunstable       = 0x12,   //   short flingy
			spruluselo        = 0x13,   //   short spriteid, byte byte, byte byte
			sprul             = 0x14,   //   short spriteid, byte byte, byte byte
			sproluselo        = 0x15,   //   short spriteid, byte overlayid
			end		  = 0x16, 
			setflipstate      = 0x17,   //   byte flipstate
			playsnd           = 0x18,   //   short soundid
			playsndrand       = 0x19,   //   byte sounds(, short soundid)*sounds
			playsndbtwn       = 0x1A,   //   short soundid, short soundid
			domissiledmg	  = 0x1B, 
			attackmelee       = 0x1C,   //   byte sounds(, short soundid)*sounds
			followmaingraphic = 0x1D, 
			randcondjmp       = 0x1E,   //   byte byte, short label
			turnccwise        = 0x1F,   //   byte byte
			turncwise         = 0x20,   //   byte byte
			turnlcwise	  = 0x21, 
			turnrand          = 0x22,   //   byte byte
			setspawnframe     = 0x23,   //   byte byte
			sigorder          = 0x24,   //   byte signalid
			attackwith        = 0x25,   //   byte weapon
			attack		  = 0x26, 
			castspell	  = 0x27, 
			useweapon         = 0x28,   //   byte weaponid
			move              = 0x29,   //   byte byte
			gotorepeatattk	  = 0x2A, 
			engframe          = 0x2B,   //   short frame
			engset            = 0x2C,   //   short frame
			__2d		  = 0x2D, 
			nobrkcodestart	  = 0x2E, 
			nobrkcodeend	  = 0x2F, 
			ignorerest	  = 0x30, 
			attkshiftproj     = 0x31,   //   byte byte
			tmprmgraphicstart = 0x32, 
			tmprmgraphicend	  = 0x33, 
			setfldirect       = 0x34,   //   byte byte
			call              = 0x35,   //   short label
			_return		  = 0x36, 
			setflspeed        = 0x37,   //   short speed
			creategasoverlays = 0x38,   //   byte gasoverlay
			pwrupcondjmp      = 0x39,   //   short label
			trgtrangecondjmp  = 0x3A,   //   short short, short label
			trgtarccondjmp    = 0x3B,   //   short short, short short, short label
			curdirectcondjmp  = 0x3C,   //   short short, short short, short label
			imgulnextid       = 0x3D,   //   byte byte, byte byte
			__3e		  = 0x3E, 
			liftoffcondjmp    = 0x3F,   //   short label
			warpoverlay       = 0x40,   //   short frame
			orderdone         = 0x41,   //   byte signalid
			grdsprol          = 0x42,   //   short spriteid, byte byte, byte byte
			__43		  = 0x43, 
			dogrddamage	  = 0x44 
		}

		void Trace (string fmt, params object[] args)
		{
			if (trace) {
				string msg = string.Format (fmt, args);
				Console.Write ("{0}: {1}", GetHashCode (), msg);
			}
		}

		void TraceLine (string fmt, params object[] args)
		{
			if (trace) {
				string msg = string.Format (fmt, args);
				Console.WriteLine ("{0}: {1}", GetHashCode (), msg);
			}
		}

		public bool Debug {
			get { return trace; }
			set { trace = value; }
		}

		public Mpq Mpq {
			get { return mpq; }
		}

		public int CurrentFrame {
			get { return current_frame; }
		}

		public void SetPosition (int x, int y)
		{
			this.x = x;
			this.y = y;
			if (dependentSprite != null)
				dependentSprite.SetPosition (x, y);
		}

		public void GetPosition (out int xo, out int yo)
		{
			xo = this.x;
			yo = this.y;
		}

		public void GetTopLeftPosition (out int xo, out int yo)
		{
			xo = this.x;
			yo = this.y;

			if (sprite_surface != null) {
				xo -= sprite_surface.Width / 2;
				yo -= sprite_surface.Height / 2;
			}
		}

		public void Face (int facing)
		{
			this.facing = facing;
		}

		public void RunScript (ushort script_start)
		{
			this.script_start = script_start;
			pc = script_start;
			if (dependentSprite != null)
				dependentSprite.RunScript (script_start);
		}

		public void RunScript (AnimationType animationType)
		{
			TraceLine ("running script type = {0}", animationType);

			int offset_to_script_type = (4 /* "SCEP" */ + 1 /* the script entry "type" */ + 3 /* the spacers */ +
						     (int)animationType * 2);

			script_start = Util.ReadWord (buf, script_entry_offset + offset_to_script_type);
			pc = script_start;
			TraceLine ("pc = {0}", pc);
		}

		ushort ReadWord (ref ushort pc)
		{
			ushort retval = Util.ReadWord (buf, pc);
			pc += 2;
			return retval;
		}

		byte ReadByte (ref ushort pc)
		{
			byte retval = buf[pc];
			pc ++;
			return retval;
		}

		Surface sprite_surface;

		void PaintSprite (DateTime now)
		{
			if (sprite_surface == null)
				return;

			/* make sure the sprite is on screen before doing the rectangle intersection/blit stuff */
			if (sprite_surface != null) {
				if ((x > SpriteManager.X - sprite_surface.Width / 2) && (x - sprite_surface.Width / 2 <= SpriteManager.X + Painter.SCREEN_RES_X)
				    && (y > SpriteManager.Y - sprite_surface.Height / 2) && (y - sprite_surface.Height / 2 <= SpriteManager.Y + 375)) {
					Painter.Blit (sprite_surface,
						      new Point (x - SpriteManager.X - sprite_surface.Width / 2,
								 y - SpriteManager.Y - sprite_surface.Height / 2));

					if (show_sprite_borders) {
						Painter.DrawBox (new Rectangle (new Point (x - SpriteManager.X - sprite_surface.Width / 2,
											   y - SpriteManager.Y - sprite_surface.Height / 2),
										new Size (sprite_surface.Width - 1,
											  sprite_surface.Height - 1)),
								 Color.Green);
					}
				}
			}
		}

		public void AddToPainter ()
		{
			Painter.Add (Layer.Unit, PaintSprite);
		}

		public void RemoveFromPainter ()
		{
			Painter.Remove (Layer.Unit, PaintSprite);
		}

		void DoPlayFrame (int frame_num)
		{
			if (current_frame != frame_num) {
				current_frame = frame_num;

				if (sprite_surface != null)
					sprite_surface.Dispose();

				sprite_surface = GuiUtil.CreateSurfaceFromBitmap (grp.GetFrame (frame_num),
										  grp.Width, grp.Height,
										  palette,
										  true);
				Invalidate ();
			}
		}

		public void Invalidate ()
		{
			Painter.Invalidate (new Rectangle (new Point (x - SpriteManager.X - sprite_surface.Width / 2,
								      y - SpriteManager.Y - sprite_surface.Height / 2),
							   sprite_surface.Size));
		}

		int waiting;

		public bool Tick (int millis_elapsed)
		{
			ushort warg1;
			ushort warg2;
			ushort warg3;
			byte barg1;
			byte barg2;
			//			byte barg3;

			if (pc == 0)
				return true;

			if (waiting > 0) {
				waiting--;
				return true;
			}
			
			Trace ("{0}: ", pc);
			switch ((IScriptOpcode)buf[pc++]) {
			case IScriptOpcode.playfram:
				warg1 = ReadWord (ref pc);
				TraceLine ("playfram: {0}", warg1);
				DoPlayFrame (warg1 + facing % 16);
				break;
			case IScriptOpcode.playframtile:
				warg1 = ReadWord (ref pc);
				TraceLine ("playframetile: {0}", warg1);
				break;
			case IScriptOpcode.sethorpos:
				barg1 = ReadByte (ref pc);
				TraceLine ("sethorpos: {0}", barg1);
				break;
			case IScriptOpcode.setpos:
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("setpos: {0} {1}", barg1, barg2);
				break;
			case IScriptOpcode.setvertpos:
				barg1 = ReadByte (ref pc);
				TraceLine ("setvertpos: {0}", barg1);
				break;
			case IScriptOpcode.wait:
				barg1 = ReadByte (ref pc);
				TraceLine ("wait: {0}", barg1);
				waiting = barg1;
				break;
			case IScriptOpcode.waitrand:
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("waitrand: {0} {1}", barg1, barg2);
				waiting = rng.Next(255) > 127 ? barg1 : barg2;
				break;
			case IScriptOpcode._goto:
				warg1 = ReadWord (ref pc);
				TraceLine ("goto: {0}", warg1);
				pc = warg1;
				break;
			case IScriptOpcode.imgol:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("imgol: {0} {1} {2}", warg1, barg1, barg2);
				break;
			case IScriptOpcode.imgul:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("imgul: {0} {1} {2}", warg1, barg1, barg2);
				Sprite dependent_sprite = SpriteManager.CreateSprite (this, warg1, palette);
				dependent_sprite.RunScript (AnimationType.Init);
				break;
			case IScriptOpcode.imgolorig:
				warg1 = ReadWord (ref pc);
				TraceLine ("imgolorig: {0}", warg1);
				break;
			case IScriptOpcode.switchul:
				warg1 = ReadWord (ref pc);
				TraceLine ("switchul: {0}", warg1);
				break;
			// __0c unknown
			case IScriptOpcode.imgoluselo:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("imgoluselo: {0} {1} {2}", warg1, barg1, barg2);
				break;
			case IScriptOpcode.imguluselo:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("imguluselo: {0} {1} {2}", warg1, barg1, barg2);
				break;
			case IScriptOpcode.sprol:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("sprol: {0} {1} {2}", warg1, barg1, barg2);
				break;
			case IScriptOpcode.highsprol:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("highsprol: {0} {1} {2}", warg1, barg1, barg2);
				break;
			case IScriptOpcode.lowsprul:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("lowsprul: {0} ({1},{2})", warg1, barg1, barg2);
 				Sprite s = SpriteManager.CreateSprite (warg1, palette, x, y);
 				s.RunScript (AnimationType.Init);
				dependentSprite = s;
				break;

				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("lowsprul: {0} {1} {2}", warg1, barg1, barg2);
				break;
			case IScriptOpcode.uflunstable:
				warg1 = ReadWord (ref pc);
				TraceLine ("uflunstable: {0}", warg1);
				break;
			case IScriptOpcode.spruluselo:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("spruluselo: {0} {1} {2}", warg1, barg1, barg2);
				break;
			case IScriptOpcode.sprul:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("sprul: {0} {1} {2}", warg1, barg1, barg2);
				break;
			case IScriptOpcode.sproluselo:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("sproleuselo: {0} {1} {2}", warg1, barg1, barg2);
				break;
			case IScriptOpcode.end:
				TraceLine ("end");
				return false;
			case IScriptOpcode.setflipstate:
				barg1 = ReadByte (ref pc);
				TraceLine ("setflipstate: {0}", barg1);
				break;
			case IScriptOpcode.playsnd:
				warg1 = ReadWord (ref pc);
				TraceLine ("playsnd: {0} ({1})", warg1 - 1, GlobalResources.Instance.SfxDataTbl[(int)GlobalResources.Instance.SfxDataDat.FileIndexes [warg1 - 1]]);
				break;
			case IScriptOpcode.playsndrand: {
				barg1 = ReadByte (ref pc);
				ushort[] wargs = new ushort[barg1];
				for (byte b = 0; b < barg1; b ++) {
					wargs[b] = ReadWord (ref pc);
				}
				Trace ("playsndrand: {0} (");
				for (int i = 0; i < wargs.Length; i ++) {
					Trace ("{0}", wargs[i]);
					if (i < wargs.Length - 1)
						Trace (", ");
				}
				TraceLine (")");
				break;
			}
			case IScriptOpcode.playsndbtwn:
				warg1 = ReadWord (ref pc);
				warg2 = ReadWord (ref pc);
				TraceLine ("playsndbtwn: {0} {1}", warg1, warg2);
				break;
			case IScriptOpcode.domissiledmg:
				TraceLine ("domissiledmg: unknown args");
				break;
			case IScriptOpcode.attackmelee: {
				barg1 = ReadByte (ref pc);
				ushort[] wargs = new ushort[barg1];
				for (byte b = 0; b < barg1; b ++) {
					wargs[b] = ReadWord (ref pc);
				}
				Trace ("attackmelee: {0} (");
				for (int i = 0; i < wargs.Length; i ++) {
					Trace ("{0}", wargs[i]);
					if (i < wargs.Length - 1)
						Trace (", ");
				}
				TraceLine (")");
				break;
			}
			case IScriptOpcode.followmaingraphic:
				TraceLine ("followmaingraphic:");
				if (parent_sprite != null)
					DoPlayFrame (parent_sprite.CurrentFrame);
				break;
			case IScriptOpcode.randcondjmp:
				barg1 = ReadByte (ref pc);
				warg1 = ReadWord (ref pc);
				TraceLine ("randcondjmp: {0} {1}", barg1, warg1);
				int rand = rng.Next(255);
				if (rand > barg1) {
					TraceLine ("+ choosing goto branch");
					pc = warg1;
				}
				break;
			case IScriptOpcode.turnccwise:
				barg1 = ReadByte (ref pc);
				TraceLine ("turnccwise: {0}", barg1);
				if (facing - barg1 < 0)
					facing = 15 - barg1;
				else
					facing -= barg1;
				break;
			case IScriptOpcode.turncwise:
				barg1 = ReadByte (ref pc);
				TraceLine ("turncwise: {0}", barg1);
				if (facing + barg1 > 15)
					facing = facing + barg1 - 15;
				else
					facing += barg1;
				break;
			case IScriptOpcode.turnlcwise:
				TraceLine ("turnlcwise: unknown args");
				break;
			case IScriptOpcode.turnrand:
				TraceLine ("turnrand:");
				if (rng.Next(255) > 127)
					goto case IScriptOpcode.turnccwise;
				else
					goto case IScriptOpcode.turncwise;
				break;
			case IScriptOpcode.setspawnframe:
				barg1 = ReadByte (ref pc);
				TraceLine ("setspawnframe {0}", barg1);
				break;
			case IScriptOpcode.sigorder:
				barg1 = ReadByte (ref pc);
				TraceLine ("sigorder {0}", barg1);
				break;
			case IScriptOpcode.attackwith:
				barg1 = ReadByte (ref pc);
				TraceLine ("attackwith {0}", barg1);
				break;
			case IScriptOpcode.attack:
				TraceLine ("attack:");
				break;
			case IScriptOpcode.castspell:
				TraceLine ("castspell:");
				break;
			case IScriptOpcode.useweapon:
				barg1 = ReadByte (ref pc);
				TraceLine ("useweapon: {0}", barg1);
				break;
			case IScriptOpcode.move:
				barg1 = ReadByte (ref pc);
				TraceLine ("move: {0}", barg1);
				break;
			case IScriptOpcode.gotorepeatattk:
				TraceLine ("gotorepeatattk");
				break;
			case IScriptOpcode.engframe:
				warg1 = ReadWord (ref pc);
				TraceLine ("engframe: {0}", warg1);
				break;
			case IScriptOpcode.engset:
				warg1 = ReadWord (ref pc);
				TraceLine ("engset: {0}", warg1);
				break;
			// __2d unknown
			case IScriptOpcode.nobrkcodestart:
				TraceLine ("nobrkcodestart:");
				break;
			case IScriptOpcode.nobrkcodeend:
				TraceLine ("nobrkcodeend:");
				break;
			case IScriptOpcode.ignorerest:
				TraceLine ("ignorerest");
				break;
			case IScriptOpcode.attkshiftproj:
				barg1 = ReadByte (ref pc);
				TraceLine ("attkshiftproj: {0}", barg1);
				break;
			case IScriptOpcode.tmprmgraphicstart:
				TraceLine ("tmprmgraphicstart:");
				break;
			case IScriptOpcode.tmprmgraphicend:
				TraceLine ("tmprmgraphicend:");
				break;
			case IScriptOpcode.setfldirect:
				barg1 = ReadByte (ref pc);
				TraceLine ("setfldirect: {0}", barg1);
				DoPlayFrame (barg1);
				break;
			case IScriptOpcode.call:
				warg1 = ReadWord (ref pc);
				TraceLine ("call: {0}", warg1);
				break;
			case IScriptOpcode._return:
				TraceLine ("return:");
				break;
			case IScriptOpcode.setflspeed:
				barg1 = ReadByte (ref pc);
				TraceLine ("setflspeed: {0}", barg1);
				break;
			case IScriptOpcode.creategasoverlays:
				barg1 = ReadByte (ref pc);
				TraceLine ("creategasoverlays: {0}", barg1);
				break;
			case IScriptOpcode.pwrupcondjmp:
				warg1 = ReadWord (ref pc);
				TraceLine ("pwrupcondjmp: {0}", warg1);
				break;
			case IScriptOpcode.trgtrangecondjmp:
				warg1 = ReadWord (ref pc);
				warg2 = ReadWord (ref pc);
				TraceLine ("trgtrangecondjmp {0} {1}", warg1, warg2);
				break;
			case IScriptOpcode.trgtarccondjmp:
				warg1 = ReadWord (ref pc);
				warg2 = ReadWord (ref pc);
				warg3 = ReadWord (ref pc);
				TraceLine ("trgtarccondjmp {0} {1} {2}", warg1, warg2, warg3);
				break;
			case IScriptOpcode.curdirectcondjmp:
				warg1 = ReadWord (ref pc);
				warg2 = ReadWord (ref pc);
				warg3 = ReadWord (ref pc);
				TraceLine ("curdirectcondjmp {0} {1} {2}", warg1, warg2, warg3);
				break;
			case IScriptOpcode.imgulnextid:
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("imgulnextid {0} {1}", barg1, barg2);
				break;
			// __3e unknown
			case IScriptOpcode.liftoffcondjmp:
				warg1 = ReadWord (ref pc);
				TraceLine ("liftoffcondjmp {0}", warg1);
				break;
			case IScriptOpcode.warpoverlay:
				warg1 = ReadWord (ref pc);
				TraceLine ("warpoverlay {0}", warg1);
				break;
			case IScriptOpcode.orderdone:
				barg1 = ReadByte (ref pc);
				TraceLine ("orderdone {0}", barg1);
				break;
			case IScriptOpcode.grdsprol:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("grdsprol {0} {1} {2}", warg1, barg1, barg2);
				break;
			// __43 unknown
			case IScriptOpcode.dogrddamage:
				TraceLine ("dogrddamage");
				break;
			default:
				Console.WriteLine ("Unknown iscript opcode: 0x{0:x}", buf[pc-1]);
				break;
			}

			return true;
		}
	}

	public enum AnimationType
	{
		Init,
		Death,
		GndAttkInit,
		AirAttkInit,
		SpAbility1,
		GndAttkRpt,
		AirAttkRpt,
		SpAbility2,
		GndAttkToIdle,
		AirAttkToIdle,
		SpAbility3,
		Walking,
		WalkingToIdle,
		BurrowInit,
		ConstrctHarvst,
		IsWorking,
		Landing,
		LiftOff,
		Unknown18,
		Unknown19,
		Unknown20,
		Unknown21,
		Unknown22,
		Unknown23,
		Unknown24,
		Burrow,
		UnBurrow,
		Unknown27
	}
}
