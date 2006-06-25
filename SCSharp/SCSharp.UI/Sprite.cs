//
// SCSharp.UI.Sprite
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

		bool trace;

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

			Console.WriteLine ("new sprite: unit\\{0} @ {1}x{2} (image {3})", grp_path, x, y, images_entry);

			this.buf = GlobalResources.Instance.IScriptBin.Contents;
			iscript_entry = GlobalResources.Instance.ImagesDat.IScriptIndexes [images_entry];

			script_entry_offset = GlobalResources.Instance.IScriptBin.GetScriptEntryOffset (iscript_entry);
			/* make sure the offset points to "SCEP" */
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
			/* make sure the offset points to "SCEP" */
			if (Util.ReadDWord (buf, script_entry_offset) != 0x45504353)
				Console.WriteLine ("invalid script_entry_offset");

			int x, y;
			parentSprite.GetPosition (out x, out y);
			SetPosition (x,y);
		}

		/* IScript opcodes */

		const byte PlayFrame = 0x00;
		const byte PlayTilesetFrame = 0x01;
		/* 0x02 = unknown */
		const byte ShiftGraphicVert = 0x03;
		/* 0x04 = unknown */
		const byte Wait = 0x05;
		const byte Wait2Rand = 0x06;
		const byte Goto = 0x07;
		const byte PlaceActiveOverlay = 0x08;
		const byte PlaceActiveUnderlay = 0x09;
		/* 0x0a = unknown */
		const byte SwitchUnderlay = 0x0b;
		/* 0x0c = unknown */
		const byte PlaceOverlay = 0x0d;
		/* 0x0e = unknown */
		const byte PlaceIndependentOverlay = 0x0f;
		const byte PlaceIndependentOverlayOnTop = 0x10;
		const byte PlaceIndependentUnderlay = 0x11;
		/* 0x12 = unknown */
		const byte DisplayOverlayWithLO = 0x13;
		/* 0x14 = unknown */
		const byte DisplayIndependentOverlayWithLO = 0x15;
		const byte EndAnimation = 0x16;
		/* 0x17 = unknown */
		const byte PlaySound = 0x18;
		const byte PlayRandomSound = 0x19;
		const byte PlayRandomSoundRange = 0x1a;
		const byte DoDamage = 0x1b;
		const byte AttackWithWeaponAndPlaySound = 0x1c;
		const byte FollowFrameChange = 0x1d;
		const byte RandomizerValueGoto = 0x1e;
		const byte TurnCCW = 0x1f;
		const byte TurnCW = 0x20;
		const byte Turn1CW = 0x21;
		const byte TurnRandom = 0x22;
		/* 0x23 = unknown */
		const byte Attack = 0x25;
		const byte AttackWithAppropriateWeapon = 0x26;
		const byte CastSpell = 0x27;
		const byte UseWeapon = 0x28;
		const byte MoveForward = 0x29;
		const byte AttackLoopMarker = 0x2a;
		/* 0x2b = unknown */
		/* 0x2c = unknown */
		/* 0x2d = unknown */
		const byte BeginPlayerLockout = 0x2e;
		const byte EndPlayerLockout = 0x2f;
		const byte IgnoreOtherOpcodes = 0x30;
		const byte AttackWithDirectionalProjectile = 0x31;
		const byte Hide = 0x32;
		const byte Unhide = 0x33;
		const byte PlaySpecificFrame = 0x34;
		/* 0x35 = unknown */
		/* 0x36 = unknown */
		/* 0x37 = unknown */
		const byte Unknown38 = 0x38;
		const byte IfPickedUp = 0x39;
		const byte IfTargetInRangeGoto = 0x3a;
		const byte IfTargetInArcGoto = 0x3b;
		const byte Unknown3c = 0x3c;
		const byte Unknown3d = 0x3d;
		/* 0x3e = unknown */
		const byte Unknown3f = 0x3f;
		const byte Unknown40 = 0x40;
		const byte Unknown41 = 0x41;
		const byte Unknown42 = 0x42; /* ICE manual says this is something dealing with sprites */

		void Trace (string fmt, params object[] args)
		{
			if (trace)
				Console.Write (fmt, args);
		}

		void TraceLine (string fmt, params object[] args)
		{
			if (trace)
				Console.WriteLine (fmt, args);
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

		public void RunScript (ushort script_start)
		{
			this.script_start = script_start;
			pc = script_start;
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

					Rectangle dest = new Rectangle (new Point (x - SpriteManager.X - sprite_surface.Width / 2,
										   y - SpriteManager.Y - sprite_surface.Height / 2),
									new Size (sprite_surface.Width, sprite_surface.Height));

					dest = Rectangle.Intersect (dest, Painter.Dirty);

					if (!dest.IsEmpty) {
						Rectangle source = dest;
						source.X -= x - SpriteManager.X - sprite_surface.Width / 2;
						source.Y -= y - SpriteManager.Y - sprite_surface.Height / 2;

						Painter.Blit (sprite_surface, dest, source);

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

#if false
					Painter.Blit (sprite_surface, new Point (x - SpriteManager.X - sprite_surface.Width / 2,
										 y - SpriteManager.Y - sprite_surface.Height / 2));

				}
			}
#endif
		}

		public void AddToPainter ()
		{
			Painter.Add (Layer.Unit, PaintSprite);
		}

		public void RemoveFromPainter ()
		{
			Painter.Add (Layer.Unit, PaintSprite);
		}

		void DoPlayFrame (int frame_num)
		{
			if (current_frame != frame_num) {
				current_frame = frame_num;

				if (sprite_surface != null)
					sprite_surface.Dispose();

				// XXX
				sprite_surface = GuiUtil.CreateSurfaceFromBitmap (grp.GetFrame (frame_num),
										  grp.Width, grp.Height,
										  palette,
										  true);

				Painter.Invalidate (new Rectangle (new Point (x - SpriteManager.X - sprite_surface.Width / 2,
									      y - SpriteManager.Y - sprite_surface.Height / 2),
								   new Size (sprite_surface.Width,
									     sprite_surface.Height)));
			}
		}

		int waiting;

		public bool Tick (DateTime now)
		{
			ushort warg1;
			ushort warg2;
			//			ushort warg3;
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
			switch (buf[pc++]) {
			case PlayFrame:
				warg1 = ReadWord (ref pc);
				TraceLine ("PlayFrame: {0}", warg1);
				DoPlayFrame (warg1 + facing % 16);
				break;
			case PlayTilesetFrame:
				warg1 = ReadWord (ref pc);
				TraceLine ("PlayTilesetFrame: {0}", warg1);
				break;
			case ShiftGraphicVert:
				barg1 = ReadByte (ref pc);
				TraceLine ("ShiftGraphicVert: {0}", barg1);
				break;
			case Wait:
				barg1 = ReadByte (ref pc);
				TraceLine ("Wait: {0}", barg1);
				waiting = barg1;
				break;
			case Wait2Rand:
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("Wait2: {0} {1}", barg1, barg2);
				waiting = rng.Next(255) > 127 ? barg1 : barg2;
				break;
			case Goto:
				warg1 = ReadWord (ref pc);
				TraceLine ("Goto: {0}", warg1);
				pc = warg1;
				break;
			case PlaceActiveOverlay:
				warg1 = ReadWord (ref pc);
				warg2 = ReadWord (ref pc);
				TraceLine ("PlaceActiveOverlay: {0} {1}", warg1, warg2);
				break;
			case PlaceActiveUnderlay:
				warg1 = ReadWord (ref pc);
				warg2 = ReadWord (ref pc);
				TraceLine ("PlaceActiveUnderlay: {0} {1}", warg1, warg2);
				Sprite dependent_sprite = SpriteManager.CreateSprite (this, warg1, palette);
				dependent_sprite.RunScript (AnimationType.Init);
				break;
			case MoveForward:
				barg1 = ReadByte (ref pc);
				TraceLine ("Move forward %1 units: {0}", barg1);
				break;
			case RandomizerValueGoto:
				barg1 = ReadByte (ref pc);
				warg1 = ReadWord (ref pc);
				TraceLine ("Randomized (with test value) goto: {0} {1}", barg1, warg1);
				int rand = rng.Next(255);
				if (rand > barg1) {
					TraceLine ("+ choosing goto branch");
					pc = warg1;
				}
				break;
			case TurnRandom:
				TraceLine ("Turn graphic number of frames in random direction (CCW or CW)");
				if (rng.Next(255) > 127)
					goto case TurnCCW;
				else
					goto case TurnCW;
			case TurnCCW:
				barg1 = ReadByte (ref pc);
				TraceLine ("Turn graphic number of frames CCW: {0}", barg1);
				if (facing - barg1 < 0)
					facing = 15 - barg1;
				else
					facing -= barg1;
				break;
			case TurnCW:
				barg1 = ReadByte (ref pc);
				TraceLine ("Turn graphic number of frames CW: {0}", barg1);
				if (facing + barg1 > 15)
					facing = facing + barg1 - 15;
				else
					facing += barg1;
				break;
			case Turn1CW:
				TraceLine ("Turn graphic 1 frame clockwise");
				break;
			case PlaySound:
				warg1 = ReadWord (ref pc);
				TraceLine ("Play sound: {0} ({1})", warg1 - 1, GlobalResources.Instance.SfxDataTbl[(int)GlobalResources.Instance.SfxDataDat.FileIndexes [warg1 - 1]]);
				break;
			case PlayRandomSoundRange:
				warg1 = ReadWord (ref pc);
				warg2 = ReadWord (ref pc);
				TraceLine ("Play random sound in range: {0}-{1}", warg1, warg2);
				break;
			case PlaySpecificFrame:
				barg1 = ReadByte (ref pc);
				TraceLine ("PlaySpecificFrame: {0}", barg1);
				DoPlayFrame (barg1);
				break;
			case PlaceIndependentUnderlay:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				TraceLine ("PlaceIndependentUnderlay: {0} ({1},{2})", warg1, barg1, barg2);
				Sprite s = SpriteManager.CreateSprite (warg1, palette, x, y);
				s.RunScript (AnimationType.Init);
				break;
			case EndAnimation:
				return false;
			case Unknown38:
				warg1 = ReadWord (ref pc);
				TraceLine ("Unknown 0x38 iscript opcode, arg {0}", warg1);
				break;
			case Unknown3c:
				warg1 = ReadWord (ref pc);
				TraceLine ("Unknown 0x3c iscript opcode, arg {0}", warg1);
				break;
			case Unknown3d:
				warg1 = ReadWord (ref pc);
				TraceLine ("Unknown 0x3d iscript opcode, arg {0}", warg1);
				break;
			case Unknown3f:
				warg1 = ReadWord (ref pc);
				TraceLine ("Unknown 0x3f iscript opcode, arg {0}", warg1);
				break;
			case Unknown40:
				warg1 = ReadWord (ref pc);
				TraceLine ("Unknown 0x40 iscript opcode, arg {0}", warg1);
				break;
			case Unknown41:
				warg1 = ReadWord (ref pc);
				TraceLine ("Unknown 0x41 iscript opcode, arg {0}", warg1);
				break;
			case Unknown42:
				warg1 = ReadWord (ref pc);
				TraceLine ("Unknown 0x42 iscript opcode, arg {0}", warg1);
				break;
			case FollowFrameChange:
				if (parent_sprite != null)
					DoPlayFrame (parent_sprite.CurrentFrame);
				break;
			case SwitchUnderlay:
			case PlaceOverlay:
			case PlaceIndependentOverlay:
			case PlaceIndependentOverlayOnTop:
			case DisplayOverlayWithLO:
			case DisplayIndependentOverlayWithLO:
			case PlayRandomSound:
			case DoDamage:
			case AttackWithWeaponAndPlaySound:
			case Attack:
			case AttackWithAppropriateWeapon:
			case CastSpell:
			case UseWeapon:
			case AttackLoopMarker:
			case BeginPlayerLockout:
			case EndPlayerLockout:
			case IgnoreOtherOpcodes:
			case AttackWithDirectionalProjectile:
			case Hide:
			case Unhide:
			case IfPickedUp:
			case IfTargetInRangeGoto:
			case IfTargetInArcGoto:
				Console.WriteLine ("Unhandled iscript opcode: 0x{0:x}", buf[pc-1]);
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
		Other,
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
