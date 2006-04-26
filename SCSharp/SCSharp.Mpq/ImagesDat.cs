//
// SCSharp.Mpq.ImagesDat
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

namespace SCSharp
{

	public class ImagesDat : MpqResource
	{
		const uint grpindex_offset = 0x00;
		const uint iscript_offset = 0x1d7e;

		const int NUM_RECORDS = 755;
		const int NUM_FIELDS = 14;

		byte[] buf;

		public ImagesDat ()
		{
		}

		public void ReadFromStream (Stream stream)
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
