//
// SCSharp.Mpq.PortDataDat
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

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

/* offsets:

portrait animation index at 0x0000. DWORD
?                        at 0x02d0. BYTE
?                        at 0x0384. BYTE

*/

namespace SCSharp
{
	public class PortDataDat : MpqResource
	{
		byte[] buf;

		public PortDataDat ()
		{
		}

		public void ReadFromStream (Stream stream)
		{
			buf = new byte [stream.Length];
			stream.Read (buf, 0, buf.Length);
		}

		public uint GetIdlePortraitIndex (uint index)
		{
			return Util.ReadDWord (buf, (int)index * 4);
		}

		public uint GetTalkingPortraitIndex (uint index)
		{
			return Util.ReadDWord (buf, (int)(buf.Length / 2 + index * 4)) - 1;
		}

		public int NumIndices {
			get { return buf.Length / (2 * 4); }
		}
	}

}
