//
// SCSharp.UI.GuiUtils
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
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using SdlDotNet.Audio;


/* for the surface creation hack below */
using System.Reflection;
using Tao.Sdl;

namespace SCSharp.UI
{

	public delegate void ReadyDelegate ();

	public static class GuiUtil {

		static Fnt[] fonts;

		static string[] BroodwarFonts = {
			"files\\font\\font8.fnt",
			"files\\font\\font10.fnt",
			"files\\font\\font14.fnt",
			"files\\font\\font16.fnt",
			"files\\font\\font16x.fnt"
		};

		public static Fnt[] GetFonts (Mpq mpq) {
			if (fonts == null) {
				string[] font_list;
				font_list = BroodwarFonts;

				fonts = new Fnt[font_list.Length];

				for (int i = 0; i < fonts.Length; i ++) {
					fonts[i] = (Fnt)mpq.GetResource (font_list[i]);
					Console.WriteLine ("fonts[{0}] = {1}", i, fonts[i] == null ? "null" : "not null");
				}
			}
			return fonts;
		}


		public static Surface RenderGlyph (Fnt font, Glyph g, byte[] palette, int offset)
		{
			byte[] buf = new byte[g.Width * g.Height * 4];
			int i = 0;

			for (int y = g.Height - 1; y >= 0; y--) {
				for (int x = g.Width - 1; x >= 0; x--) {
					if (g.Bitmap[y,x] == 0)
						buf [i + 0] = 0;
					else if (g.Bitmap[y,x] == 1)
						buf [i + 0] = 255;
					else
						buf [i + 0] = 128;

					buf[i + 1] = palette[ (g.Bitmap[y,x] + offset) * 3 + 2];
					buf[i + 2] = palette[ (g.Bitmap[y,x] + offset) * 3 + 1];
					buf[i + 3] = palette[ (g.Bitmap[y,x] + offset) * 3 ];

					if (buf[i+1] == 252 && buf[i+2] == 0 && buf[i+3] == 252)
						buf[i + 0] = 0;

					i += 4;
				}
			}

			return CreateSurfaceFromRGBAData (buf, (ushort)g.Width, (ushort)g.Height, 32, g.Width * 4);
		}

		public static Surface ComposeText (string text, Fnt font, byte[] palette)
		{
			return ComposeText (text, font, palette, -1, -1, 4);
		}

		public static Surface ComposeText (string text, Fnt font, byte[] palette, int offset)
		{
			return ComposeText (text, font, palette, -1, -1, offset);
		}

		public static Surface ComposeText (string text, Fnt font, byte[] palette, int width, int height,
						   int offset)
		{
			if (font == null)
				Console.WriteLine ("aiiiieeee");

			int i;
			/* create a run of text, for now ignoring any control codes in the string */
			StringBuilder run = new StringBuilder ();
			for (i = 0; i < text.Length; i ++)
				if (text[i] == 0x0a /* allow newlines */||
				    !Char.IsControl (text[i]))
					run.Append (text[i]);

			string rs = run.ToString ();
			byte[] r = Encoding.ASCII.GetBytes (rs);

			int x, y;
			int text_height, text_width;

			/* measure the text first, optionally wrapping at width */
			text_width = text_height = 0;
			x = y = 0;

			for (i = 0; i < r.Length; i ++) {
				int glyph_width = 0;

				if (r[i] != 0x0a) /* newline */ {
					if (r[i] == 0x20) /* space */
						glyph_width = font.SpaceSize;
					else {
						Glyph g = font[r[i]-1];

						glyph_width = g.Width + g.XOffset;
					}
				}

				if (r[i] == 0x0a ||
				    (width != -1 && x + glyph_width > width)) {
					if (x > text_width)
						text_width = x;
					x = 0;
					text_height += font.LineSize;
				}
					
				x += glyph_width;
			}

			if (x > text_width)
				text_width = x;
			text_height += font.LineSize;

			Surface surf = new Surface (text_width, text_height);
			surf.TransparentColor = Color.Black;

			/* the draw it */
			x = y = 0;
			for (i = 0; i < r.Length; i ++) {
				int glyph_width = 0;
				Glyph g = null;

				if (r[i] != 0x0a) /* newline */ {
					if (r[i] == 0x20)  /* space */{
						glyph_width = font.SpaceSize;
					}
					else {
						g = font[r[i]-1];
						glyph_width = g.Width + g.XOffset;

						Surface gs = RenderGlyph (font, g, palette, offset);
						surf.Blit (gs, new Point (x, y + g.YOffset));
					}
				}

				if (r[i] == 0x0a ||
				    x + glyph_width > text_width) {
					x = 0;
					y += font.LineSize;
				}
					
				x += glyph_width;
			}

			return surf;
		}

		public static Surface CreateSurface (byte[] data, ushort width, ushort height, int depth, int stride,
						     int rmask, int gmask, int bmask, int amask)
		{
					      
			/* beware, kind of a gross hack below */
			Surface surf;

			IntPtr blob = Marshal.AllocCoTaskMem (data.Length);
			Marshal.Copy (data, 0, blob, data.Length);

			IntPtr handle = Sdl.SDL_CreateRGBSurfaceFrom (blob,
								      width, height, depth,
								      stride,
								      rmask, gmask, bmask, amask);

			surf = (Surface)Activator.CreateInstance (typeof (Surface),
								  BindingFlags.NonPublic | BindingFlags.Instance,
								  null,
								  new object[] {handle},
								  null);

			return surf;
		}

		public static Surface CreateSurfaceFromRGBAData (byte[] data, ushort width, ushort height, int depth, int stride)
		{
			return CreateSurface (data, width, height, depth, stride,
					      /* XXX this needs addressing in Tao.Sdl - these arguments should be uints */
					      unchecked ((int)0xff000000),
					      (int)0x00ff0000,
					      (int)0x0000ff00,
					      (int)0x000000ff);
		}

		public static Surface CreateSurfaceFromRGBData (byte[] data, ushort width, ushort height, int depth, int stride)
		{
			return CreateSurface (data, width, height, depth, stride,
					      (int)0x00ff0000,
					      (int)0x0000ff00,
					      (int)0x000000ff,
					      (int)0x00000000);
		}

		public static byte[] GetBitmapData (byte[,] grid, ushort width, ushort height, byte[] palette, bool with_alpha)
		{
			byte[] buf = new byte[width * height * (3 + (with_alpha ? 1 : 0))];
			int i = 0;
			int x, y;

			for (y = height - 1; y >= 0; y --) {
				for (x = width - 1; x >= 0; x--) {
					if (with_alpha)
						i++;
					buf[i++] = palette[ grid[y,x] * 3 + 2];
					buf[i++] = palette[ grid[y,x] * 3 + 1];
					buf[i++] = palette[ grid[y,x] * 3 ];
					if (with_alpha) {
						if (buf[i - 3] == 0
						    && buf[i - 2] == 0
						    && buf[i - 1] == 0) {
							buf[i-4] = 0x00;
						}
						else if (buf[i - 3] == 59
							 && buf[i - 2] == 39
							 && buf[i - 1] == 39) {
							buf[i - 3] = buf[i - 2] = buf[i - 1] = 0;
							//							Console.WriteLine ("translucent shadow pixel, palette index = {0}", palette [grid[y,x]] * 3);
							buf[i - 4] = 0xaa;
						}
						else {
							//							Console.WriteLine ("pixel data RGB = {0},{1},{2}, palette index = {3}",
							//									   buf[i-3],buf[i-2],buf[i-1], palette [grid[y,x]] * 3);
							buf[i-4] = 0xff;
						}
					}
				}
			}

			return buf;
		}

		public static byte[] GetBitmapData (byte[,] grid, ushort width, ushort height, byte[] palette, int translucent_index, int transparent_index)
		{
			byte[] buf = new byte[width * height * 4];
			int i = 0;
			int x, y;

			for (y = height - 1; y >= 0; y --) {
				for (x = width - 1; x >= 0; x--) {
					if (grid[y,x] == translucent_index)
						buf[i+0] = 0x05; /* keep this in sync with Pcx.cs */
					else if (grid[y,x] == transparent_index)
						buf[i+0] = 0x00;
					else
						buf[i+0] = 0xff;
					buf[i+1] = palette[ grid[y,x] * 3 + 2];
					buf[i+2] = palette[ grid[y,x] * 3 + 1];
					buf[i+3] = palette[ grid[y,x] * 3 + 0];
					i+= 4;
				}
			}

			return buf;
		}

		public static Surface CreateSurfaceFromBitmap (byte[,] grid, ushort width, ushort height, byte[] palette, bool with_alpha)
		{
			byte[] buf = GetBitmapData (grid, width, height, palette, with_alpha);

			return CreateSurfaceFromRGBAData (buf, width, height, with_alpha ? 32 : 24, width * (3 + (with_alpha ? 1 : 0)));
		}

		public static Surface CreateSurfaceFromBitmap (byte[,] grid, ushort width, ushort height, byte[] palette, int translucent_index, int transparent_index)
		{
			byte[] buf = GetBitmapData (grid, width, height, palette, translucent_index, transparent_index);

			return CreateSurfaceFromRGBAData (buf, width, height, 32, width * 4);
		}

		public static byte[] ReadStream (Stream stream)
		{
			if (stream is MemoryStream) {
				return ((MemoryStream)stream).ToArray();
			}
			else {
				byte[] buf = new byte [stream.Length];
				stream.Read (buf, 0, buf.Length);
				return buf;
			}
		}


		public static Surface SurfaceFromPcx (Pcx pcx)
		{
			return CreateSurfaceFromRGBAData (pcx.RgbaData, pcx.Width, pcx.Height, pcx.Depth, pcx.Stride);
		}

		public static Surface SurfaceFromStream (Stream stream, int translucentIndex, int transparentIndex)
		{
			Pcx pcx = new Pcx();
			pcx.ReadFromStream (stream, translucentIndex, transparentIndex);
			return SurfaceFromPcx (pcx);
		}

		public static Surface SurfaceFromStream (Stream stream)
		{
			return GuiUtil.SurfaceFromStream (stream, -1, -1);
		}

		public static Sound SoundFromStream (Stream stream)
		{
			byte[] buf = GuiUtil.ReadStream (stream);
			return Mixer.Sound (buf);
		}

		public static void PlaySound (Mpq mpq, string resourcePath)
		{
			Stream stream = (Stream)mpq.GetResource (resourcePath);
			if (stream == null)
				return;
			Sound s = GuiUtil.SoundFromStream (stream);
			s.Play();
		}

		public static void PlayMusic (Mpq mpq, string resourcePath, int numLoops)
		{
			Stream stream = (Stream)mpq.GetResource (resourcePath);
			if (stream == null)
				return;
			Sound s = GuiUtil.SoundFromStream (stream);
			s.Play (true);
		}
	}
}
