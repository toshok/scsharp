

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp {

	public class SfxDataDat : MpqResource
	{
		const uint file_offset = 0x00;

		const int NUM_RECORDS = 1127;
		const int NUM_FIELDS = 1;

		byte[] buf;

		public SfxDataDat ()
		{
		}

		public void ReadFromStream (Stream stream)
		{
			buf = new byte [NUM_RECORDS * NUM_FIELDS * 4];
			stream.Read (buf, 0, buf.Length);
		}

		public ushort GetFileIndex (uint index)
		{
			return Util.ReadWord (buf, (int)(file_offset + index * 4));
		}
	}

}
