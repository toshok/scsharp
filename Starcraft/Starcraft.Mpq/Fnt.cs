
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Starcraft {
	public class Glyph {
		internal Glyph (int width, int height, int xoffset, int yoffset,
				byte[,] bitmap)
		{
			this.bitmap = bitmap;
			this.width = width;
			this.height = height;
			this.xoffset = xoffset;
			this.yoffset = yoffset;

#if false
			Console.WriteLine ("======");
			Console.WriteLine (" + width = {0}", width);
			Console.WriteLine (" + height = {0}", height);
			Console.WriteLine (" + xoffset = {0}", xoffset);
			Console.WriteLine (" + yoffset = {0}", yoffset);
			Console.WriteLine ("======");
#endif
		}

		int width;
		int height;
		int xoffset;
		int yoffset;
		byte[,] bitmap;
		object userData;

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

		public object UserData {
			get { return userData; }
			set { userData = value; }
		}
	}

	public class Fnt : MpqResource {
		Stream stream;
		int num_strings;
		string[] strings;

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
			uint name = Util.ReadDWord (stream);
			
			lowIndex = Util.ReadByte (stream);
			highIndex = Util.ReadByte (stream);
			maxWidth = Util.ReadByte (stream);
			maxHeight = Util.ReadByte (stream);
			uint unknown = Util.ReadDWord (stream);

			Console.WriteLine ("font runs from {0} to {1} ({2} glyphs)", lowIndex, highIndex, highIndex - lowIndex);
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
			//			Console.WriteLine ("Getting Glyph {0}", c);

			if (glyphs.ContainsKey (c))
				return glyphs[c];

			stream.Position = offsets[(uint)c];

			byte letterWidth = Util.ReadByte (stream);
			byte letterHeight = Util.ReadByte (stream);
			byte letterXOffset = Util.ReadByte (stream);
			byte letterYOffset = Util.ReadByte (stream);

			byte[,] bitmap = new byte[letterHeight + letterYOffset, letterWidth];

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

				bitmap[y+letterYOffset,x] = (byte)(cmap_entry + 21); // 14 = magic number
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
					throw new ArgumentOutOfRangeException ("index");

				return GetGlyph (index);
			}
		}

		public int SizeText (string s) {
			/* XXX ascii only */
			int w = 0;

			foreach (byte b in Encoding.ASCII.GetBytes (s))
				w += this[b].Width;
			return w;
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
