
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp {
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

		string ReadUntilNull (StreamReader r)
		{
			StringBuilder sb = new StringBuilder();

			char c;
			do {
				c = (char)r.Read();
				if (c != 0)
					sb.Append (c);
			} while (c != 0);

			return sb.ToString();
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

				strings[i] = ReadUntilNull (tr);
			}
		}

		public string this [int index] {
			get { return strings[index]; }
		}

		public string[] Strings {
			get { return strings; }
		}
	}
}