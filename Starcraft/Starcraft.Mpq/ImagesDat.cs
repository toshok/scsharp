

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Starcraft {

	public class ImagesDat : MPQResource
	{
		const uint grpindex_offset = 0x00;
		const uint iscript_offset = 0x1d7e;

		const int NUM_RECORDS = 755;
		const int NUM_FIELDS = 14;

		byte[] buf;

		public ImagesDat ()
		{
		}

		void MPQResource.ReadFromStream (Stream stream)
		{
			buf = new byte [NUM_RECORDS * NUM_FIELDS * 4];
			stream.Read (buf, 0, buf.Length);
		}

		public ushort GetGrpIndex (uint index)
		{
			return Util.ReadWord (buf, (int)(grpindex_offset + index * 4));
		}

		public ushort GetIScriptIndex (uint index)
		{
			return Util.ReadWord (buf, (int)(iscript_offset + index * 4));
		}
	}

}
