

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp {

	public class IScriptBin : MpqResource
	{
		byte[] buf;
		Dictionary<ushort,ushort> entries;

		const int entry_table_offset = 0x0082e0;

		public IScriptBin ()
		{
			entries = new Dictionary<ushort,ushort>();
		}

		public void ReadFromStream (Stream stream)
		{
			buf = new byte [stream.Length];
			stream.Read (buf, 0, buf.Length);

			Console.WriteLine ("reading from 0x{0:x} to 0x{1:x}",
					   entry_table_offset, buf.Length);

			int p = entry_table_offset;
			while (p < buf.Length) {
				ushort images_id = Util.ReadWord (buf, p);
				ushort offset = Util.ReadWord (buf, p+2);
				entries[images_id] = offset;
				p += 4;
			}
		}

		public byte[] Contents {
			get { return buf; }
		}

		public ushort GetScriptEntryOffset (ushort images_id) {
			if (!entries.ContainsKey (images_id))
				throw new Exception ();
			return entries[images_id];
		}
	}

}
