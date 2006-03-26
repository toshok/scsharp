
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Starcraft {

	public class Sprite {
		int sprite_entry;

		ushort images_entry;
		ushort iscript_entry;

		string grp_path;
		GRP grp;
		MPQ mpq;
		IScriptRunner runner;

		public Sprite (MPQ mpq, int sprite_entry)
		{
			this.mpq = mpq;
			this.sprite_entry = sprite_entry;

			images_entry = GlobalResources.Instance.SpritesDat.GetImagesDatEntry (sprite_entry);
			Console.WriteLine ("image_dat_entry == {0}", images_entry);

			ushort grp_index = GlobalResources.Instance.ImagesDat.GetGrpIndex (images_entry);
			Console.WriteLine ("grp_index = {0}", grp_index);
			grp_path = GlobalResources.Instance.ImagesTbl.Strings[grp_index-1];
			Console.WriteLine ("grp_path = {0}", grp_path);

			grp = (GRP)mpq.GetResource ("unit\\" + grp_path);

			iscript_entry = GlobalResources.Instance.ImagesDat.GetIScriptIndex (images_entry);
			Console.WriteLine ("iscript_entry = {0}", iscript_entry);
			Console.WriteLine ("iscript_entry_offset = {0}", GlobalResources.Instance.IScriptBin.GetScriptEntryOffset (iscript_entry));

			runner = new IScriptRunner (grp, GlobalResources.Instance.IScriptBin.GetScriptEntryOffset (iscript_entry));

		}

		public void RunAnimation (int animation_type)
		{
			runner.RunScriptType (animation_type);
		}

		public void AddToPainter (Painter painter)
		{
			runner.AddToPainter (painter);
		}

		public void RemoveFromPainter (Painter painter)
		{
			runner.RemoveFromPainter (painter);
		}

		public bool Tick (DateTime now)
		{
			return runner.Tick (now);
		}
	}

}
