//
// SCSharp.Mpq.Tbl
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
	public class Tbl : MpqResource {
		Stream stream;
		int num_strings;
		string[] strings;

		public Tbl ()
		{
		}

		public void ReadFromStream (Stream stream)
		{
			this.stream = stream;
			ReadStrings();
		}

		void ReadStrings ()
		{
			int i;

			num_strings = Util.ReadWord (stream);

			int[] offsets = new int[num_strings];

			for (i = 0; i < num_strings; i ++) {
				offsets[i] = Util.ReadWord (stream);
			}

			StreamReader tr = new StreamReader (stream);
			strings = new string[num_strings];

			for (i = 0; i < num_strings; i++) {
				if (tr.BaseStream.Position != offsets[i]) {
					tr.BaseStream.Seek (offsets[i], SeekOrigin.Begin);
					tr.DiscardBufferedData ();
				}

				strings[i] = Util.ReadUntilNull (tr);
			}
		}

		public int Count {
			get { return strings.Length; }
		}

		public string this [int index] {
			get { return strings[index]; }
		}

		public string[] Strings {
			get { return strings; }
		}
	}
}
