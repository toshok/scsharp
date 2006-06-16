//
// SCSharp.Mpq.UnitsDat
//
// Authors:
//	Chris Toshok (toshok@hungry.com)
//
// (C) 2006 The Hungry Programmers (http://www.hungry.com/)
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

/*
227 units

general flingy   at 0x0000.  BYTE   - index into flingy.dat
overlay          at 0x00e4.  WORD
      ?          at 0x02ab.
construct sprite at 0x0534.  DWORD  - index into images.dat
shields          at 0x0a8c.  WORD 
hitpoints        at 0x0c54.  DWORD
animation level  at 0x0fe4.  BYTE
movement flags   at 0x10c8.  BYTE
ground weapon?   at 0x11ac.  BYTE?

subunit range    at 0x1d40.  BYTE
sight range      at 0x1e24.  BYTE
      ?          at 0x1f08.  BYTE

portraits        at 0x367c?  WORD

mineral cost at 0x3844. WORD
gas cost at 0x3a0c. WORD
build time at 0x3bd4.  WORD

Restricts at 0x3d9c. WORD
? at 0x3f64.  BYTE

unit width at 0x2f5c.  WORD
unit height at 0x30ac.  WORD

space required at 0x4120.  BYTE


score for producing at 0x43d8. WORD
score for destroying at 0x45a0. WORD

*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp
{
	public class UnitsDat : MpqResource
	{
		public UnitsDat ()
		{
		}

		byte[] buf;

		public void ReadFromStream (Stream stream)
		{
			buf = new byte[(int)stream.Length];

			stream.Read (buf, 0, buf.Length);
		}

		/* offsets from the stardat.mpq version */

		const int flingy_offset = 0x0000;
		const int overlay_offset = 0x00e3;
		const int construct_sprite_offset = 0x0534;
		const int animation_level_offset = 0x0fe4;
		const int create_score_offset = 0x43d8;
		const int destroy_score_offset = 0x45a0;
		const int shield_offset = 0x0a8c;
		const int hitpoint_offset = 0x0c54;
		const int portrait_offset = 0x367c;

		public byte GetFlingyId (int unit_id)
		{
			return buf[flingy_offset + unit_id];
		}

		public uint GetConstructSpriteId (int unit_id)
		{
			return Util.ReadDWord (buf, construct_sprite_offset + unit_id * 4);
		}

		public byte GetAnimationLevel (int unit_id)
		{
			return buf[animation_level_offset + unit_id];
		}

		public ushort GetCreateScore (int unit_id)
		{
			return Util.ReadWord (buf, create_score_offset + unit_id * 2);
		}

		public ushort GetDestroyScore (int unit_id)
		{
			return Util.ReadWord (buf, destroy_score_offset + unit_id * 2);
		}

		public ushort GetShields (int unit_id)
		{
			return Util.ReadWord (buf, shield_offset + unit_id * 2);
		}

		public uint GetHitpoints (int unit_id)
		{
			return Util.ReadWord (buf, hitpoint_offset + unit_id * 2);
		}

		public uint GetPortraitId (int unit_id)
		{
			return Util.ReadWord (buf, portrait_offset + unit_id * 2);
		}

	}
}
