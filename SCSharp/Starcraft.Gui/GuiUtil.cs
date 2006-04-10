using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using SdlDotNet;

/* for the surface creation hack below */
using System.Reflection;
using Tao.Sdl;

namespace Starcraft {

	public delegate void ReadyDelegate ();

	public static class GuiUtil {

		static Fnt largeFont;
		static Fnt mediumFont;
		static Fnt smallFont;

		public static Fnt GetLargeFont (Mpq mpq)
		{
			if (largeFont == null)
				largeFont = (Fnt)mpq.GetResource ("files\\font\\font16x.fnt");
			return largeFont;
		}

		public static Fnt GetMediumFont (Mpq mpq)
		{
			if (mediumFont == null)
				mediumFont = (Fnt)mpq.GetResource ("files\\font\\font14.fnt");
			return mediumFont;
		}

		public static Fnt GetSmallFont (Mpq mpq)
		{
			if (smallFont == null)
				smallFont = (Fnt)mpq.GetResource ("files\\font\\font8.fnt");
			return smallFont;
		}

		public static Surface RenderGlyph (Fnt font, Glyph g, byte[] palette, int offset)
		{
			byte[,] bm2 = new byte[g.Height,g.Width];
			for (int y = 0; y < g.Height; y++) {
				for (int x = 0; x < g.Width; x++)
					bm2[y,x] = (byte)(g.Bitmap[y,x] + offset);
			}

			return CreateSurfaceFromBitmap (bm2, (ushort)g.Width, (ushort)g.Height,
							palette, true);
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
				if (!Char.IsControl (text[i]))
					run.Append (text[i]);

			string rs = run.ToString ();
			byte[] r = Encoding.ASCII.GetBytes (rs);

			int x, y;
			int text_height, text_width;

			if (width == -1 && height == -1) {
				text_width = font.SizeText (rs);
				text_height = font.LineSize;
			}
			else {
				/* measure the text first, wrapping at width */
				text_width = text_height = 0;
				x = y = 0;

				for (i = 0; i < r.Length; i ++) {
					int glyph_width;

					if (r[i] == 32) /* space */
						glyph_width = font.SpaceSize;
					else
						glyph_width = font[r[i]-1].Width;

					if (x + glyph_width > width) {
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
			}

			Surface surf = new Surface (text_width, text_height);
			surf.TransparentColor = Color.Black;

			/* the draw it */
			x = y = 0;
			for (i = 0; i < r.Length; i ++) {
				int glyph_width;
				Glyph g = null;

				if (r[i] == 32) {
					glyph_width = font.SpaceSize;
				}
				else {
					g = font[r[i]-1];
					glyph_width = g.Width;

					Surface gs = RenderGlyph (font, g, palette, offset);
					surf.Blit (gs, new Point (x, y + g.YOffset));
				}

				if (x + glyph_width > text_width) {
					x = 0;
					y += font.LineSize;
				}
					
				x += glyph_width;
			}

			return surf;
#if false
			StringBuilder run;
			int i;
			List<Surface> runSurfaces = new List<Surface> ();
			int width = 0;
			int maxHeight = 0;
			Color c = foreground;
			SdlDotNet.Font font = fonts[fontSize];

			//			font.Bold = true;
			maxHeight = font.Maxeight; /* XXX should be just the height of the string? */
			//			font.Bold = false;

			i = 0;
			run = new StringBuilder ();
			while (i < text.Length) {
				if (!Char.IsControl (text[i]))
					run.Append (text[i]);

				if (Char.IsControl(text[i]) || i == text.Length - 1) {
					/* blit the current run, reset the font's state,
					   and start a new run */
					if (run.Length > 0) {
						string runString = run.ToString ();
						Surface runSurf = font.Render (runString, true, foreground);
						runSurf.AlphaBlending = true;
						width += runSurf.Width;
						runSurfaces.Add (runSurf);
					}
					if (text[i] == 0x01) {
						c = foreground;
						font.Normal = true;
					}
					else if (text[i] == 0x04) {
						c = Color.FromArgb (0, foreground.R + 20, foreground.G + 50, foreground.B + 20);
						font.Bold = true;
					}
					run = new StringBuilder ();
				}

				i++;
			}

			Surface composedSurf = new Surface (width, maxHeight);
			int x = 0;
			foreach (Surface surf in runSurfaces) {
				composedSurf.Blit (surf, new Point (x, maxHeight - surf.Height));
				x += surf.Width;
			}

			return composedSurf;
#endif
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
					buf[i++] = palette[ grid[y,x] * 3 ];
					buf[i++] = palette[ grid[y,x] * 3 + 1];
					buf[i++] = palette[ grid[y,x] * 3 + 2];
					if (with_alpha) {
						if (buf[i - 3] == 0
						    && buf[i - 2] == 0
						    && buf[i - 1] == 0) {
							buf[i-4] = 0x00;
						}
						else
							buf[i-4] = 0xff;
					}
				}
			}

			return buf;
		}

		public static Surface CreateSurfaceFromRGBAData (byte[] data, ushort width, ushort height, int depth, int stride)
		{
			/* beware, kind of a gross hack below */
			Surface surf;

			IntPtr blob = Marshal.AllocCoTaskMem (data.Length);
			Marshal.Copy (data, 0, blob, data.Length);

			IntPtr handle = Sdl.SDL_CreateRGBSurfaceFrom (blob,
								      width, height, depth,
								      stride,
								      /* XXX this needs addressing in Tao.Sdl - these arguments should be uints */
								      unchecked ((int)0xff000000),
								      (int)0x00ff0000,
								      (int)0x0000ff00,
								      (int)0x000000ff);

			surf = (Surface)Activator.CreateInstance (typeof (Surface),
								  BindingFlags.NonPublic | BindingFlags.Instance,
								  null,
								  new object[] {handle},
								  null);

			return surf;
		}

		public static Surface CreateSurfaceFromBitmap (byte[,] grid, ushort width, ushort height, byte[] palette, bool with_alpha)
		{
			byte[] buf = GetBitmapData (grid, width, height, palette, with_alpha);

			return CreateSurfaceFromRGBAData (buf, width, height, with_alpha ? 32 : 24, width * (3 + (with_alpha ? 1 : 0)));
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


		public static Surface SurfaceFromStream (Stream stream, int translucentIndex, int transparentIndex)
		{
			Pcx pcx = new Pcx();
			pcx.ReadFromStream (stream, translucentIndex, transparentIndex);
			return CreateSurfaceFromRGBAData (pcx.RgbaData, pcx.Width, pcx.Height, pcx.Depth, pcx.Stride);
		}

		public static Surface SurfaceFromStream (Stream stream, bool applyTrans)
		{
			return SurfaceFromStream (stream, applyTrans ? 252 : -1, applyTrans ? 0 : -1);
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
