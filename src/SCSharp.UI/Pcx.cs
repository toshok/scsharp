//
// SCSharp.UI.Pcx
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

namespace SCSharp.UI
{
	public class Pcx
	{
		public Pcx ()
		{
		}

		ushort xmin;
		ushort xmax;
		ushort ymin;
		ushort ymax;

		bool with_alpha;

		public void ReadFromStream (Stream stream, int translucentIndex, int transparentIndex)
		{
			with_alpha = translucentIndex != -1 || transparentIndex != -1;

			byte magic = Util.ReadByte (stream);
			if (magic != 0x0A)
				throw new Exception ("stream is not a valid .pcx file");

			/*version =*/ Util.ReadByte (stream);
			/*encoding =*/ Util.ReadByte (stream);
			ushort bpp = Util.ReadByte (stream);
			xmin = Util.ReadWord (stream);
			ymin = Util.ReadWord (stream);
			xmax = Util.ReadWord (stream);
			ymax = Util.ReadWord (stream);
			/*ushort h_dpi =*/ Util.ReadWord (stream);
			/*ushort v_dpi =*/ Util.ReadWord (stream);
			stream.Position += 48; /* skip the header palette */
			stream.Position ++;    /* skip the reserved byte */
			ushort numplanes = Util.ReadByte (stream);
			/*ushort stride =*/ Util.ReadWord (stream);
			/*headerInterp =*/ Util.ReadWord (stream);
			/*videoWidth =*/ Util.ReadWord (stream);
			/*videoHeight =*/ Util.ReadWord (stream);
			stream.Position += 54;

			if (bpp != 8 || numplanes != 1)
				throw new Exception ("unsupported .pcx image type");

			width = (ushort)(xmax - xmin + 1);
			height = (ushort)(ymax - ymin + 1);

			long imageData = stream.Position;

			stream.Position = stream.Length - 256 * 3;
			/* read the palette */
			palette = new byte[256 * 3];
			stream.Read (palette, 0, 256 * 3);

			stream.Position = imageData;

			Console.WriteLine ("imageData begins at {0}", imageData);

			/* now read the image data */
			data = new byte[width * height * 4];

			int idx = 0;
			while (idx < data.Length) {
				byte b = Util.ReadByte (stream);
				byte count;
				byte value;

				if ((b & 0xC0) == 0xC0) {
					/* it's a count byte */
					count = (byte)(b & 0x3F);
					value = Util.ReadByte (stream);
				}
				else {
					count = 1;
					value = b;
				}

				for (int i = 0; i < count; i ++) {
					if (idx + 4 > data.Length)
						return;

					/* this stuff is endian
					 * dependent... for big endian
					 * we need the "idx +"'s
					 * reversed */
					data[idx + 3] = palette [value * 3 + 0];
					data[idx + 2] = palette [value * 3 + 1];
					data[idx + 1] = palette [value * 3 + 2];
					if (with_alpha) {
						if (value == translucentIndex)
							data[idx + 0] = 0xd0;
						else if (value == transparentIndex)
							data[idx + 0] = 0x00;
						else
							data[idx + 0] = 0xff;
					}

					idx += 4;
				}
			}
		}


		byte[] data;
		byte[] palette;

		ushort width;
		ushort height;

		public byte[] RgbaData {
			get { return data; }
		}

		public byte[] RgbData {
			get {
				byte[] foo = new byte[width * height * 3];
				int i = 0;
				int j = 0;
				while (i < data.Length) {
					foo[j++] = data[i++];
					foo[j++] = data[i++];
					foo[j++] = data[i++];
					i++;
				}
				return foo;
			}
		}

		public byte[] Palette {
			get { return palette; }
		}

		public ushort Width {
			get { return width; }
		}

		public ushort Height {
			get { return height; }
		}

		public ushort Depth {
			get { return (ushort)(with_alpha ? 32 : 24); }
		}

		public ushort Stride {
			get { return (ushort)(width * (3 + (with_alpha ? 1 : 0))); }
		}
	}
}
