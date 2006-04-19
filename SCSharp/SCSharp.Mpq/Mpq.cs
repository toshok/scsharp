using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace SCSharp {

	public interface MpqResource
	{
		void ReadFromStream (Stream stream);
	}

	public abstract class Mpq
	{
		protected Mpq () { }

		protected internal abstract Stream GetStreamForResource (string path);

		protected Type GetTypeFromResourcePath (string path)
		{
			string ext = Path.GetExtension (path);
			if (ext.ToLower() == ".tbl") {
				return typeof (Tbl);
			}
			else if (ext.ToLower () == ".fnt") {
				return typeof (Fnt);
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
				else if (path.ToLower().EndsWith ("sfxdata.dat"))
					return typeof (SfxDataDat);
				else if (path.ToLower().EndsWith ("sprites.dat"))
					return typeof (SpritesDat);
			}
			else if (ext.ToLower () == ".smk") {
				return typeof (Smk);
			}
			else if (ext.ToLower () == ".spk") {
				return typeof (Spk);
			}

			return null;
		}

		public object GetResource (string path)
		{
			Stream stream = GetStreamForResource (path);
			if (stream == null)
				return null;

			Type t = GetTypeFromResourcePath (path);
			if (t == null)
				return stream;

			MpqResource res = Activator.CreateInstance (t) as MpqResource;

			if (res == null) return null;

			res.ReadFromStream (stream);

			return res;
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
			mpqs.Add (mpq);
		}

		protected internal override Stream GetStreamForResource (string path)
		{
			foreach (Mpq mpq in mpqs) {
				Stream s = mpq.GetStreamForResource (path);
				if (s != null)
					return s;
			}

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

		protected internal override Stream GetStreamForResource (string path)
		{
			string rebased_path = ConvertBackSlashes (Path.Combine (mpq_dir_path, path));

			if (file_hash.ContainsKey (rebased_path.ToLower ())) {
				string real_path = file_hash[rebased_path.ToLower ()];
				if (real_path != null)
					return File.OpenRead (real_path);
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
		IntPtr mpqHandle;

		public MpqArchive (string path)
		{
			if (!Storm.SFileOpenArchive (path, 0, 0, out mpqHandle))
				throw new Exception (String.Format ("Could not load .mpq file at {0}", path));
		}

		protected internal override Stream GetStreamForResource (string path)
		{
			IntPtr fileHandle;
			uint fileSize;
			uint numRead;

			if (!Storm.SFileOpenFileEx (mpqHandle, path, 0, out fileHandle))
				return null;

			fileSize = Storm.SFileGetFileInfo (fileHandle, SFileInfo.FileSize);

			byte[] buf = new byte[fileSize];

			if (!Storm.SFileReadFile (fileHandle, buf, fileSize, out numRead, (IntPtr)0)) {
				Storm.SFileCloseFile (fileHandle);
				return null;
			}

			if (fileSize != numRead) {
				Storm.SFileCloseFile (fileHandle);
				return null;
			}

			Storm.SFileCloseFile (fileHandle);
			return new MemoryStream (buf);
		}
	}

	/* this should remain in sync with what's in StormLib.h */
	enum SFileInfo {
		ArchiveSize     =  1,      // MPQ size (value from header)
		HashTbaleSize  =  2,      // Size of hash table, in entries
		BlockTableSize =  3,      // Number of entries in the block table
		BlockSize       =  4,      // Size of file block (in bytes)
		HashTable       =  5,      // Pointer to Hash table (TMPQHash *)
		BlockTable      =  6,      // Pointer to Block Table (TMPQBlock *)
		NumFiles        =  7,      // Real number of files within archive

		HashIndex       =  8,      // Hash index of file in MPQ
		CodeName1        =  9,      // The first codename of the file
		CodeName2        = 10,      // The second codename of the file
		LocaleId         = 11,      // Locale ID of file in MPQ
		BlockIndex       = 12,      // Index to Block Table
		FileSize        = 13,      // Original file size
		CompressedSize  = 14,      // Compressed file size
		Flags            = 15,      // File flags
		Position         = 16,      // File position within archive
		Seed             = 17,      // File decryption seed
		SeedUnfixed     = 18      // Decryption seed not fixed to file pos and size
	}

	static class Storm {
		[DllImport ("Storm.dll")]
		public extern static bool SFileOpenArchive (string archiveFilename,
							    uint priority,
							    uint flags,
							    out IntPtr handle);

		[DllImport ("Storm.dll")]
		public extern static bool SFileOpenFileEx (IntPtr mpqHandle,
							   string filePath,
							   uint searchScope,
							   out IntPtr fileHandle);

		[DllImport ("Storm.dll")]
		public extern static bool SFileGetFileSize (IntPtr fileHandle,
							    out uint fileSize);

		[DllImport ("Storm.dll")]
		public extern static uint SFileGetFileInfo (IntPtr fileHandle, SFileInfo info);

		[DllImport ("Storm.dll")]
		public extern static bool SFileReadFile (IntPtr fileHandle,
							 byte[] buf,
							 uint numberOfBytesToRead,
							 out uint numberOfBytesRead,
							 IntPtr unused);

		[DllImport ("Storm.dll")]
		public extern static bool SFileCloseFile (IntPtr fileHandle);
	}
}
