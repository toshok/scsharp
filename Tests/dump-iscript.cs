using System;
using System.IO;
using System.Collections.Generic;

using SCSharp;
using SCSharp.UI;

public class DumpIScript {

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
		Unknown16,
		Landing,
		LiftOff,
		Producing,  /* used when a terran building is training troops */
		Unknown20,
		Unknown21,
		Unknown22,
		Unknown23,
		Unknown24,
		Burrow,
		UnBurrow,
		Unknown27
	}

	/* IScript opcodes */
	enum IScriptOpcode {
		PlayFrame = 0x00,
		PlayTilesetFrame = 0x01,
		Unknown02 = 0x02,
		ShiftGraphicVert = 0x03,
		Unknown04 = 0x04,
		Wait = 0x05,
		Wait2Rand = 0x06,
		Goto = 0x07,
		PlaceActiveOverlay = 0x08,
		PlaceActiveUnderlay = 0x09,
		Unknown0a = 0x0a,
		SwitchUnderlay = 0x0b,
		Unknown0c = 0x0c,
		PlaceOverlay = 0x0d,
		Unknown0e = 0x0e,
		PlaceIndependentOverlay = 0x0f,
		PlaceIndependentOverlayOnTop = 0x10,
		PlaceIndependentUnderlay = 0x11,
		Unknown12 = 0x12,
		DisplayOverlayWithLO = 0x13,
		Unknown14 = 0x14,
		DisplayIndependentOverlayWithLO = 0x15,
		EndAnimation = 0x16,
		Unknown17 = 0x17,
		PlaySound = 0x18,
		PlayRandomSound = 0x19,
		PlayRandomSoundRange = 0x1a,
		DoDamage = 0x1b,
		AttackWithWeaponAndPlaySound = 0x1c,
		FollowFrameChange = 0x1d,
		RandomizerValueGoto = 0x1e,
		TurnCCW = 0x1f,
		TurnCW = 0x20,
		Turn1CW = 0x21,
		TurnRandom = 0x22,
		Unknown23 = 0x23,
		Unknown24 = 0x24,
		Attack = 0x25,
		AttackWithAppropriateWeapon = 0x26,
		CastSpell = 0x27,
		UseWeapon = 0x28,
		MoveForward = 0x29,
		AttackLoopMarker = 0x2a,
		Unknown2b = 0x2b,
		Unknown2c = 0x2c,
		Unknown2d = 0x2d,
		BeginPlayerLockout = 0x2e,
		EndPlayerLockout = 0x2f,
		IgnoreOtherOpcodes = 0x30,
		AttackWithDirectionalProjectile = 0x31,
		Hide = 0x32,
		Unhide = 0x33,
		PlaySpecificFrame = 0x34,
		Unknown35 = 0x35,
		Unknown36 = 0x36,
		Unknown37 = 0x37,
		Unknown38 = 0x38,
		IfPickedUp = 0x39,
		IfTargetInRangeGoto = 0x3a,
		IfTargetInArcGoto = 0x3b,
		Unknown3c = 0x3c,
		Unknown3d = 0x3d,
		Unknown3e = 0x3e,
		Unknown3f = 0x3f,
		Unknown40 = 0x40,
		Unknown41 = 0x41,
		Unknown42 = 0x42, /* ICE manual says this is something dealing with sprites */

		Unknown80 = 0x80
	}

	class Block {
		public readonly ushort Pc;
		ushort endPc;

		public Block (ushort pc)
		{
			Pc = pc;
			endPc = pc;
		}

		public ushort EndPc {
			get { return endPc; }
			set { endPc = value; }
		}
	}

	List<Block> blocks;
	IScriptBin bin;
	byte[] buf;
	Dictionary<ushort,string> labels;

	public DumpIScript (Mpq mpq)
	{
		new GlobalResources(mpq, null).LoadSingleThreaded ();

		bin = GlobalResources.Instance.IScriptBin;
		buf = bin.Contents;

		blocks = new List<Block>();
		labels = new Dictionary<ushort,string>();
	}

	static int counter = 0;
	string GetLabel (ushort pc)
	{
		return GetLabel (pc, String.Format ("local{0}", counter++));
	}

	string GetLabel (ushort pc, string label)
	{
		if (labels.ContainsKey (pc))
			return labels[pc];

		labels.Add (pc, label);
		return label;
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

	string GetGrpNameFromImageId (ushort arg)
	{
		uint grp_index = GlobalResources.Instance.ImagesDat.GrpIndexes [arg];
		return grp_index == 0 ? "NONE" : GlobalResources.Instance.ImagesTbl [ (int)grp_index - 1];
	}

	string GetGrpNameFromSpriteId (ushort arg)
	{
		ushort image_entry = GlobalResources.Instance.SpritesDat.ImagesDatEntries [arg];
		return GetGrpNameFromImageId (image_entry);
	}

	bool DumpOpcode (ref ushort pc)
	{
		ushort warg1;
		ushort warg2;
		//			ushort warg3;
		byte barg1;
		byte barg2;
		//			byte barg3;
		Block dest;

		bool retval = false;
		IScriptOpcode op = (IScriptOpcode)buf[pc];

		Console.Write ("{0:x}\t{1} ", pc, op);

		pc++;

		try {
			switch (op) {
			case IScriptOpcode.PlayFrame:
				warg1 = ReadWord (ref pc);
				Console.Write ("{0}", warg1);
				break;
			case IScriptOpcode.PlayTilesetFrame:
				warg1 = ReadWord (ref pc);
				Console.Write ("{0}", warg1);
				break;
			case IScriptOpcode.ShiftGraphicVert:
				barg1 = ReadByte (ref pc);
				Console.Write ("{0}", barg1);
				break;
			case IScriptOpcode.Wait:
				barg1 = ReadByte (ref pc);
				Console.Write ("{0}", barg1);
				break;
			case IScriptOpcode.Wait2Rand:
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				Console.Write ("{0} {1}", barg1, barg2);
				break;
			case IScriptOpcode.Goto:
				warg1 = ReadWord (ref pc);

				Console.Write ("{0}", GetLabel (warg1));
				dest = new Block (warg1);
				blocks.Add (dest);
				retval = true;
				break;
			case IScriptOpcode.PlaceActiveOverlay:
				warg1 = ReadWord (ref pc);
				warg2 = ReadWord (ref pc);
				Console.Write ("{0} ({1}) {2}",
					       warg1,
					       GetGrpNameFromImageId (warg1),
					       warg2);
				break;
			case IScriptOpcode.PlaceActiveUnderlay:
				warg1 = ReadWord (ref pc);
				warg2 = ReadWord (ref pc);
				Console.Write ("{0} ({1}) {2}",
					       warg1,
					       GetGrpNameFromImageId (warg1),
					       warg2);
				break;
			case IScriptOpcode.MoveForward:
				barg1 = ReadByte (ref pc);
				Console.Write ("{0}", barg1);
				break;
			case IScriptOpcode.RandomizerValueGoto:
				barg1 = ReadByte (ref pc);
				warg1 = ReadWord (ref pc);
				Console.Write ("{0} {1}", barg1, GetLabel (warg1));
				dest = new Block (warg1);
				blocks.Add (dest);
				break;
			case IScriptOpcode.TurnRandom:
				break;
			case IScriptOpcode.TurnCCW:
				barg1 = ReadByte (ref pc);
				Console.Write ("{0}", barg1);
				break;
			case IScriptOpcode.TurnCW:
				barg1 = ReadByte (ref pc);
				Console.Write ("{0}", barg1);
				break;
			case IScriptOpcode.Turn1CW:
				break;
			case IScriptOpcode.PlaySound:
				warg1 = ReadWord (ref pc);
				Console.Write ("{0} ({1})", warg1 - 1,
					       GlobalResources.Instance.SfxDataTbl[(int)GlobalResources.Instance.SfxDataDat.FileIndexes [warg1 - 1]]);
				break;
			case IScriptOpcode.PlayRandomSound:
				barg1 = ReadByte (ref pc);
				Console.Write ("{0}", barg1);
				for (int i = 0; i < barg1; i ++) {
					warg1 = ReadWord (ref pc);
					Console.Write (" {0}", warg1);
				}
				break;
			case IScriptOpcode.PlayRandomSoundRange:
				warg1 = ReadWord (ref pc);
				warg2 = ReadWord (ref pc);
				Console.Write ("{0} {1}", warg1, warg2);
				Console.WriteLine (" [");
				for (int i = warg1; i < warg2; i ++)
					Console.Write (" {0}", GlobalResources.Instance.SfxDataTbl[(int)GlobalResources.Instance.SfxDataDat.FileIndexes [i - 1]]);
				Console.Write (" ]");
				break;
			case IScriptOpcode.PlaySpecificFrame:
				barg1 = ReadByte (ref pc);
				Console.Write ("{0}", barg1);
				break;
			case IScriptOpcode.PlaceIndependentUnderlay:
				warg1 = ReadWord (ref pc);
				barg1 = ReadByte (ref pc);
				barg2 = ReadByte (ref pc);
				Console.Write ("{0} ({1}) ({2},{3})",
					       warg1,
					       GetGrpNameFromSpriteId (warg1),
					       barg1, barg2);
				break;
			case IScriptOpcode.AttackWithWeaponAndPlaySound:
				barg1 = ReadByte (ref pc);
				Console.Write ("{0} ", barg1);

				Console.Write ("[");
				for (int i = 0; i < barg1; i ++) {
					warg1 = ReadWord (ref pc);
					Console.Write (" {0}",
						       GlobalResources.Instance.SfxDataTbl[(int)GlobalResources.Instance.SfxDataDat.FileIndexes [warg1 - 1]]);
				}
				Console.Write (" ]");
				break;
			case IScriptOpcode.EndAnimation:
				retval = true;
				break;
			case IScriptOpcode.FollowFrameChange:
			case IScriptOpcode.BeginPlayerLockout:
			case IScriptOpcode.EndPlayerLockout:
			case IScriptOpcode.AttackLoopMarker:
				/* no arguments */
				break;
			case IScriptOpcode.UseWeapon:
				warg1 = ReadWord (ref pc);
				Console.Write ("{0}", warg1);
				break;
			case IScriptOpcode.Unknown0a:
				barg1 = ReadByte (ref pc);
				Console.Write ("{0}", barg1);
				break;
			case IScriptOpcode.Unknown24:
				barg1 = ReadByte (ref pc);
				Console.Write ("{0}", barg1);
				break;
			case IScriptOpcode.Unknown38:
				warg1 = ReadWord (ref pc);
				Console.Write ("{0}", warg1);
				break;
			case IScriptOpcode.Unknown3c:
				warg1 = ReadWord (ref pc);
				Console.Write ("{0}", warg1);
				break;
			case IScriptOpcode.Unknown3d:
				warg1 = ReadWord (ref pc);
				Console.Write ("{0}", warg1);
				break;
			case IScriptOpcode.Unknown3f:
				warg1 = ReadWord (ref pc);
				Console.Write ("{0}", warg1);
				break;
			case IScriptOpcode.Unknown40:
				warg1 = ReadWord (ref pc);
				Console.Write ("{0}", warg1);
				break;
			case IScriptOpcode.Unknown41:
				warg1 = ReadWord (ref pc);
				Console.Write ("{0}", warg1);
				break;
			case IScriptOpcode.Unknown42:
				warg1 = ReadWord (ref pc);
				Console.Write ("{0}", warg1);
				break;
			default:
				Console.Write (" <Unhandled> ");
				ushort pc2;
				pc2 = pc;
				Console.Write ("[");
				for (int i = 0; i < 3; i ++)
					Console.Write (" {0:x}", ReadWord (ref pc2));
				Console.Write (" ]");
				pc2 = pc;
				Console.Write ("/[");
				for (int i = 0; i < 6; i ++)
					Console.Write (" {0:x}", ReadByte (ref pc2));
				Console.WriteLine (" ]");
				retval = true;
				break;
			}
		}
		catch (Exception e) {
			Console.Write ("*exception*");
		}
		finally {
			Console.WriteLine ();
		}

		return retval;	
	}

	void DumpBlock (Block block)
	{
		/* make sure we haven't already dumped this block */
		for (int i = 0; i < blocks.Count; i ++) {
			if (blocks[i] == block)
				break;
			if (block.Pc >= blocks[i].Pc &&
			    block.EndPc <= blocks[i].EndPc)
				return;
		}

		ushort pc = block.Pc;
		Console.WriteLine ();
		bool done = false;

		while (!done) {
			block.EndPc = pc;
			if (labels.ContainsKey (pc))
				Console.WriteLine (".label {0}", labels[pc]);
			done = DumpOpcode (ref pc);
		}
	}

	public void Dump (string prefix, int entry)
	{
		int entry_offset = bin.GetScriptEntryOffset ((ushort)entry);
		/* make sure the offset points to "SCEP" */
		if (Util.ReadDWord (buf, entry_offset) != 0x45504353)
			throw new Exception("invalid script_entry_offset");

		Console.WriteLine (";");
		Console.WriteLine ("\t.header {0}", prefix);

		foreach (AnimationType animationType in Enum.GetValues (typeof (AnimationType))) {
			int offset_to_script_type = (4 /* "SCEP" */ + 1 /* the script entry "type" */ + 3 /* the spacers */ +
						     (int)animationType * 2);
			ushort script_start = Util.ReadWord (buf, entry_offset + offset_to_script_type);

			string blockname = String.Format ("{0}{1}", prefix, animationType.ToString());

			blockname = GetLabel (script_start, blockname);
			Console.WriteLine ("\t.animation{0}: {1} ({2:x})", animationType, script_start == 0 ? "NONE" : script_start > buf.Length ? "OUT OF RANGE" : blockname, script_start);

			if (script_start < buf.Length
			    && script_start != 0)
				blocks.Add (new Block (script_start));
		}
		
		Console.WriteLine (";");

		int index = 0;

		while (index < blocks.Count) {
			Block block_to_dump = blocks[index];
			DumpBlock (block_to_dump);
			index++;
		}
	}

	public static void Main (string[] args)
	{
		string entry_name = args[1];
		int entry = Int32.Parse (args[2]);

		Mpq mpq = new MpqArchive (args[0]);
		DumpIScript dumper = new DumpIScript (mpq);
		dumper.Dump (entry_name, entry);
	}
}
