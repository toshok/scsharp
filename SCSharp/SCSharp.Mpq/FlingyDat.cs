/*
  sprite id      at 0x0000. WORD.
  speed          at 0x0170. DWORD.
  turn style     at 0x0450. WORD
  acceleration   at 0x05c0. DWORD.
  turning radius at 0x08a0. BYTE.
  move controls  at ?.. ?.
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp {
	public class FlingyDat : MpqResource
	{
		public FlingyDat ()
		{
		}

		byte[] buf;

		public void ReadFromStream (Stream stream)
		{
			buf = new byte[(int)stream.Length];

			stream.Read (buf, 0, buf.Length);
		}

		/* offsets from the stardat.mpq version */

		const int sprite_offset = 0x0000;
		const int speed_offset = 0x0170;
		const int turnstyle_offset = 0x0450;
		const int accel_offset = 0x05c0;
		const int turnradius_offset = 0x08a0;

		public int GetSpriteId (int index)
		{
			return Util.ReadWord (buf, sprite_offset + index * 2);
		}
	}
}
