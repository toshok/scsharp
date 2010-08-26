//
// SCSharp.Mpq.Mpq
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
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace SCSharp
{
	public interface MpqResource
	{
		void ReadFromStream (Stream stream);
	}

	public abstract class Mpq : IDisposable
	{
		Dictionary<string,object> cached_resources;

		protected Mpq ()
		{
			cached_resources = new Dictionary <string,object>();
		}

		public abstract Stream GetStreamForResource (string path);

		protected Type GetTypeFromResourcePath (string path)
		{
			string ext = Path.GetExtension (path);
			if (ext.ToLower() == ".tbl") {
				return typeof (Tbl);
			}
			else if (ext.ToLower () == ".fnt") {
				return typeof (Fnt);
			}
			else if (ext.ToLower () == ".got") {
				return typeof (Got);
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
				if (path.ToLower().EndsWith ("flingy.dat"))
					return typeof (FlingyDat);
				else if (path.ToLower().EndsWith ("images.dat"))
					return typeof (ImagesDat);
				else if (path.ToLower().EndsWith ("sfxdata.dat"))
					return typeof (SfxDataDat);
				else if (path.ToLower().EndsWith ("sprites.dat"))
					return typeof (SpritesDat);
				else if (path.ToLower().EndsWith ("units.dat"))
					return typeof (UnitsDat);
				else if (path.ToLower().EndsWith ("mapdata.dat"))
					return typeof (MapDataDat);
				else if (path.ToLower().EndsWith ("portdata.dat"))
					return typeof (PortDataDat);
			}
			else if (ext.ToLower () == ".spk") {
				return typeof (Spk);
			}

			return null;
		}

		public object GetResource (string path)
		{
			if (cached_resources.ContainsKey (path))
				return cached_resources[path];

			Stream stream = GetStreamForResource (path);
			if (stream == null)
				return null;

			Type t = GetTypeFromResourcePath (path);
			if (t == null)
				return stream;

			MpqResource res = Activator.CreateInstance (t) as MpqResource;

			if (res == null) return null;

			res.ReadFromStream (stream);

			/* don't cache .smk files */
			if (!path.ToLower().EndsWith (".smk"))
				cached_resources [path] = res;

			return res;
		}

		public virtual void Dispose ()
		{
		}
	}

	public class MpqContainer : Mpq
	{
		List<Mpq> mpqs;

		public MpqContainer ()
		{
			mpqs = new List<Mpq>();
		}

		public void Add (Mpq mpq)
		{
			if (mpq == null)
				return;
			mpqs.Add (mpq);
		}

		public void Remove (Mpq mpq)
		{
			if (mpq == null)
				return;
			mpqs.Remove (mpq);
		}

		public void Clear ()
		{
			mpqs.Clear ();
		}

		public override void Dispose ()
		{
			foreach (Mpq mpq in mpqs)
				mpq.Dispose ();
		}

		public override Stream GetStreamForResource (string path)
		{
			foreach (Mpq mpq in mpqs) {
				Stream s = mpq.GetStreamForResource (path);
				if (s != null)
					return s;
			}

			Console.WriteLine ("returning null stream for resource: {0}", path);
			return null;
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

		public override Stream GetStreamForResource (string path)
		{
			string rebased_path = ConvertBackSlashes (Path.Combine (mpq_dir_path, path));

			if (file_hash.ContainsKey (rebased_path.ToLower ())) {
				string real_path = file_hash[rebased_path.ToLower ()];
				if (real_path != null) {
					Console.WriteLine ("using {0}", real_path);
					return File.OpenRead (real_path);
				}
			}
			return null;
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
		MpqReader.MpqArchive mpq;

		public MpqArchive (string path)
		{
			mpq = new MpqReader.MpqArchive (path);
		}

		public override Stream GetStreamForResource (string path)
		{
			try {
				return mpq.OpenFile (path);
			}
			catch (FileNotFoundException) {
				return null;
			}
		}

		public override void Dispose ()
		{
			mpq.Dispose ();
		}
	}
}
