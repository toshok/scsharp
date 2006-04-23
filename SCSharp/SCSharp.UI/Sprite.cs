
using SdlDotNet;

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp.UI
{

	public class Sprite
	{
		//int sprite_entry;

		ushort images_entry;
		ushort iscript_entry;

		string grp_path;
		Grp grp;
		//Mpq mpq;
		IScriptRunner runner;

		public Sprite (Mpq mpq, int sprite_entry, byte[] palette, int x, int y)
		{
			//this.mpq = mpq;
			//this.sprite_entry = sprite_entry;

			images_entry = GlobalResources.Instance.SpritesDat.GetImagesDatEntry (sprite_entry);
			//			Console.WriteLine ("image_dat_entry == {0}", images_entry);

			ushort grp_index = GlobalResources.Instance.ImagesDat.GetGrpIndex (images_entry);
			//			Console.WriteLine ("grp_index = {0}", grp_index);
			grp_path = GlobalResources.Instance.ImagesTbl[grp_index-1];
			//			Console.WriteLine ("grp_path = {0}", grp_path);

			grp = (Grp)mpq.GetResource ("unit\\" + grp_path);

			iscript_entry = GlobalResources.Instance.ImagesDat.GetIScriptIndex (images_entry);
			//			Console.WriteLine ("iscript_entry = {0}", iscript_entry);
			//			Console.WriteLine ("iscript_entry_offset = {0}", GlobalResources.Instance.IScriptBin.GetScriptEntryOffset (iscript_entry));

			runner = new IScriptRunner (grp, GlobalResources.Instance.IScriptBin.GetScriptEntryOffset (iscript_entry), palette);
			runner.SetPosition (x, y);

			if (sprite_entry == 252) {
				runner.ListAnimations();
				runner.Debug = true;
			}
		}

		public void RunAnimation (AnimationType animation_type)
		{
			runner.RunScript (animation_type);
		}

		public void AddToPainter (Painter painter)
		{
			runner.AddToPainter (painter);
		}

		public void RemoveFromPainter (Painter painter)
		{
			runner.RemoveFromPainter (painter);
		}

		public bool Tick (Surface surf, DateTime now)
		{
			return runner.Tick (surf, now);
		}

		public void GetPosition (out int x, out int y)
		{
			runner.GetPosition (out x, out y);
		}
	}

}
