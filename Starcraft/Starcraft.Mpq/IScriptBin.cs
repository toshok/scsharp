

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Starcraft {

	public class IScriptBin : MPQResource
	{
		byte[] buf;
		Dictionary<ushort,ushort> entries;

		const int entry_table_offset = 0x0082e0;

		public IScriptBin ()
		{
			entries = new Dictionary<ushort,ushort>();
		}

		void MPQResource.ReadFromStream (Stream stream)
		{
			buf = new byte [stream.Length];
			stream.Read (buf, 0, buf.Length);

			int p = entry_table_offset;
			try {
			while (p < stream.Length) {
				ushort images_id = Util.ReadWord (buf, p);
				ushort offset = Util.ReadWord (buf, p+2);
				entries[images_id] = offset;
				p += 4;
			}
			} catch (Exception e) { Console.WriteLine (e);}
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
