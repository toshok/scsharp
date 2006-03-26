

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Starcraft {

	public interface MpqResource
	{
		void ReadFromStream (Stream stream);
	}

	public abstract class Mpq
	{
		protected Mpq () { }

		protected abstract Stream GetStreamForResource (string path);

		protected Type GetTypeFromResourcePath (string path)
		{
			string ext = Path.GetExtension (path);
			if (ext.ToLower() == ".tbl") {
				return typeof (Tbl);
			}
			else if (ext.ToLower () == ".grp") {
				return typeof (Grp);
			}
			else if (ext.ToLower () == ".bin") {
				if (path.ToLower().EndsWith ("aiscript.bin")) /* must come before iscript.bin */
					return null;
				else if (path.ToLower().EndsWith ("iscript.bin"))
					return typeof (IScriptBin);
				else
					return typeof (Bin);
			}
			else if (ext.ToLower () == ".chk") {
				return typeof (Chk);
			}
			else if (ext.ToLower () == ".dat") {
				if (path.ToLower().EndsWith ("images.dat"))
					return typeof (ImagesDat);
				else if (path.ToLower().EndsWith ("sprites.dat"))
					return typeof (SpritesDat);
			}
			else if (ext.ToLower () == ".smk") {
				return typeof (Smk);
			}

			return null;
		}

		public object GetResource (string path)
		{
			Stream stream = GetStreamForResource (path);
			Type t = GetTypeFromResourcePath (path);
			if (t == null)
				return stream;

			MpqResource res = Activator.CreateInstance (t) as MpqResource;

			if (res == null) return null;

			res.ReadFromStream (stream);

			return res;
		}
	}

	public class MpqDirectory : Mpq
	{
		Dictionary<string,string> file_hash;
		string mpq_dir_path;

		public MpqDirectory (string path)
		{
			mpq_dir_path = path;
			file_hash = new Dictionary<string,string> ();

			RecurseDirectoryTree (mpq_dir_path);
		}

		string ConvertBackSlashes (string path)
		{
			while (path.IndexOf ('\\') != -1)
				path = path.Replace ('\\', Path.DirectorySeparatorChar);

			return path;
		}

		protected override Stream GetStreamForResource (string path)
		{
			string rebased_path = ConvertBackSlashes (Path.Combine (mpq_dir_path, path));

			Console.WriteLine ("looking for path {0}", rebased_path.ToLower());

			string real_path = file_hash[rebased_path.ToLower ()];
			if (real_path == null)
				throw new Exception (); /* XXX */

			Console.WriteLine ("found {0}", real_path);

			return File.OpenRead (Path.Combine (mpq_dir_path, real_path));
		}

		void RecurseDirectoryTree (string path)
		{
			string[] files = Directory.GetFiles (path);
			foreach (string f in files) {
				string platform_path = ConvertBackSlashes (f);
				file_hash.Add (f.ToLower(), platform_path);
			}

			string[] directories = Directory.GetDirectories (path);
			foreach (string d in directories) {
				RecurseDirectoryTree (d);
			}
		}
	}

	public class MpqArchive : Mpq
	{
		public MpqArchive (string path)
		{
		}

		protected override Stream GetStreamForResource (string path)
		{
			throw new NotImplementedException ();
		}
	}
}
