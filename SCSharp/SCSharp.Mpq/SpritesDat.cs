//
// SCSharp.Mpq.SpritesDat
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

	public class SpritesDat : MpqResource
	{
		const int NUM_SPRITES = 517;
		const int NUM_DOODADS = 130;

		const int IMAGESDAT_FIELD = 0;
		const int BARLENGTH_FIELD = 1;
		const int SELECTIONCIRCLE_FIELD = 4;
		const int SELECTIONCIRCLE_OFFSET_FIELD = 5;

		const int NUM_FIELDS = 6;

		int[] size_of_variable = { 2, 1, 1, 1, 1, 1 };
		int[] size_of_variable_block = {
			NUM_SPRITES * 2,
			NUM_SPRITES - NUM_DOODADS,
			NUM_SPRITES,
			NUM_SPRITES,
			NUM_SPRITES - NUM_DOODADS,
			NUM_SPRITES - NUM_DOODADS };

		int[] offset_to_variable_block;

		byte[] buf;

		public SpritesDat ()
		{
			offset_to_variable_block = new int [NUM_FIELDS];
			offset_to_variable_block[0] = 0;
			for (int i = 1; i < NUM_FIELDS; i ++)
				offset_to_variable_block [i] = offset_to_variable_block[i-1] + size_of_variable_block[i-1];
		}

		public void ReadFromStream (Stream stream)
		{
			int size = 0;
			for (int i = 0; i < NUM_FIELDS; i ++)
				size += size_of_variable_block [i];
			buf = new byte [size];
			stream.Read (buf, 0, size);
		}

		int GetIndexLocation (int field, int index)
		{
			return offset_to_variable_block[field] + index * size_of_variable[field];
		}

		public ushort GetImagesDatEntry (int index)
		{
			return Util.ReadWord (buf, GetIndexLocation (IMAGESDAT_FIELD, index));
		}
	}

}
