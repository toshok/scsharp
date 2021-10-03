//
// SCSharp.Mpq.Fnt
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
using System.Collections.Generic;
using System.Text;

namespace SCSharp
{
	public class Glyph
	{
		internal Glyph (int width, int height, int xoffset, int yoffset,
				byte[,] bitmap)
		{
			this.bitmap = bitmap;
			this.width = width;
			this.height = height;
			this.xoffset = xoffset;
			this.yoffset = yoffset;
		}

		int width;
		int height;
		int xoffset;
		int yoffset;
		byte[,] bitmap;

		public byte[,] Bitmap {
			get { return bitmap; }
		}

		public int Width {
			get { return width; }
		}

		public int Height {
			get { return height; }
		}

		public int XOffset {
			get { return xoffset; }
		}

		public int YOffset {
			get { return yoffset; }
		}
	}

	public class Fnt : MpqResource {
		Stream stream;

		public Fnt ()
		{
		}

		public void ReadFromStream (Stream stream)
		{
			this.stream = stream;
			ReadFontHeader ();
			ReadGlyphOffsets ();

			glyphs = new Dictionary<int,Glyph> ();
		}

		void ReadFontHeader ()
		{
			/*uint name =*/ Util.ReadDWord (stream);
			
			lowIndex = Util.ReadByte (stream);
			highIndex = Util.ReadByte (stream);

			if (lowIndex > highIndex) {
				byte tmp = lowIndex;
				lowIndex = highIndex;
				highIndex = tmp;
			}
			maxWidth = Util.ReadByte (stream);
			maxHeight = Util.ReadByte (stream);
			/*uint unknown =*/ Util.ReadDWord (stream);
		}

		Dictionary<uint,uint> offsets;

		void ReadGlyphOffsets ()
		{
			offsets = new Dictionary<uint,uint> ();
			for (uint c = lowIndex; c < highIndex; c++)
				offsets.Add (c, Util.ReadDWord (stream));
		}

		Glyph GetGlyph (int c)
		{
			if (glyphs.ContainsKey (c))
				return glyphs[c];

			stream.Position = offsets[(uint)c];

			byte letterWidth = Util.ReadByte (stream);
			byte letterHeight = Util.ReadByte (stream);
			byte letterXOffset = Util.ReadByte (stream);
			byte letterYOffset = Util.ReadByte (stream);

			byte[,] bitmap = new byte[letterHeight, letterWidth];

			int x, y;
			x = letterWidth - 1;
			y = letterHeight - 1;
			while (true) {
				byte b = Util.ReadByte (stream);
				int count = (b & 0xF8) >> 3;
				byte cmap_entry = (byte)(b & 0x07);

				for (int i = 0; i < count; i ++) {
					bitmap[y,x] = 0;
					x--;
					if (x < 0) {
						x = letterWidth - 1;
						y--;
						if (y < 0)
							goto done;
					}
				}

				bitmap[y,x] = (byte)cmap_entry;
				x--;
				if (x < 0) {
					x = letterWidth - 1;
					y--;
					if (y < 0)
						goto done;
				}
			}
		done:
			glyphs.Add (c,
				    new Glyph (letterWidth,
					       letterHeight,
					       letterXOffset,
					       letterYOffset,
					       bitmap));

			return glyphs[c];
		}

		public Glyph this [int index] {
			get {
				if (index < lowIndex || index > highIndex)
					throw new ArgumentOutOfRangeException ("index",
							       String.Format ("value of {0} out of range of {1}-{2}", index, lowIndex, highIndex));

				return GetGlyph (index);
			}
		}

		public int SpaceSize {
			get { return this[109-1].Width; /* 109 = ascii for 'm' */ }
		}

		public int LineSize {
			get { return maxHeight; }
		}

		public int MaxWidth {
			get { return maxWidth; }
		}

		public int MaxHeight {
			get { return maxHeight; }
		}
		
		public int LowIndex {
			get { return lowIndex; }
		}
		
		public int HighIndex {
			get { return highIndex; }
		}
		
		Dictionary<int,Glyph> glyphs;
		byte highIndex;
		byte lowIndex;
		byte maxWidth;
		byte maxHeight;

		public void DumpGlyphs()
		{
			for (int c = lowIndex; c < highIndex; c++) {
				Console.WriteLine ("Letter: {0}", c);
				DumpGlyph (c);
			}
		}

		public void DumpGlyph (int c)
		{
			Glyph g = GetGlyph (c);
			byte[,] bitmap = g.Bitmap;
			for (int y = g.Height - 1; y >= 0 ; y --) {
				for (int x = g.Width - 1; x >= 0; x --) {
					if (bitmap[y,x] == 0)
						Console.Write (" ");
					else
						Console.Write ("#");
				}
				Console.WriteLine ();
			}
			Console.WriteLine ();
		}
	}
}
