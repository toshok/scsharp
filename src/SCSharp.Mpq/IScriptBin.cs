//
// SCSharp.Mpq.IScriptBin
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
using System.Text;
using System.Collections.Generic;

namespace SCSharp
{

	public class IScriptBin : MpqResource
	{
		byte[] buf;
		Dictionary<uint,ushort> entries;

		public IScriptBin ()
		{
			entries = new Dictionary<uint,ushort>();
		}

		bool PointsToSCPE (int p)
		{
			return (buf[p] == 0x53
				&& buf[p+1] == 0x43
				&& buf[p+2] == 0x50
				&& buf[p+3] == 0x45);
		}

		public void ReadFromStream (Stream stream)
		{
			buf = new byte [stream.Length];
			stream.Read (buf, 0, buf.Length);

			int p = buf.Length - 8;
			int o = Util.ReadWord (buf, p + 2);
			Console.WriteLine ("first offset = {0:X}", o);
			while (PointsToSCPE (Util.ReadWord (buf, p + 2)))
				p -= 4;

			Console.WriteLine ("iscript entry offsets {0:x}", p);
			Console.WriteLine ("iscript.bin contains {0} entries",
					   (buf.Length - p) / 4 - 2);

			while (p < buf.Length - 4) {
				ushort images_id = Util.ReadWord (buf, p);
				ushort offset = Util.ReadWord (buf, p+2);
				entries[images_id] = offset;

				Console.WriteLine ("id: {0}   offset: {1:X}", images_id, offset);

				p += 4;
			}
		}

		public byte[] Contents {
			get { return buf; }
		}

		public ushort GetScriptEntryOffset (uint images_id) {
			if (!entries.ContainsKey (images_id))
				return 0;
			return entries[images_id];
		}
	}

}
