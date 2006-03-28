using System;
using System.IO;
using System.Collections.Generic;
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

		static SdlDotNet.Font[] fonts;

		static GuiUtil()
		{
			/* XXX this should be done at game
			 * initialization time.  also, it should not
			 * use a .ttf but the bitmap representation
			 * that's included in the .mpq. */

			fonts = new SdlDotNet.Font[3];
			fonts[0] = new SdlDotNet.Font ("FreeSans.ttf", 9);
			fonts[1] = new SdlDotNet.Font ("FreeSans.ttf", 12);
			fonts[2] = new SdlDotNet.Font ("FreeSans.ttf", 14);
		}

		public static SdlDotNet.Font LargeFont {
			get { return fonts[2]; }
		}

		public static SdlDotNet.Font MediumFont {
			get { return fonts[1]; }
		}

		public static SdlDotNet.Font SmallFont {
			get { return fonts[0]; }
		}

		public static Surface ComposeText (string text, int fontSize, Color foreground)
		{
			StringBuilder run;
			int i;
			List<Surface> runSurfaces = new List<Surface> ();
			int width = 0;
			int maxHeight = 0;
			Color c = foreground;
			SdlDotNet.Font font = fonts[fontSize];

			font.Bold = true;
			maxHeight = font.SizeText (text).Height;
			font.Bold = false;

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
		}

		public static byte[] GetBitmapData (byte[,] grid, ushort width, ushort height, byte[] palette, bool with_alpha)
		{
			byte[] buf = new byte[width * height * (3 + (with_alpha ? 1 : 0))];
			int i = 0;
			int x, y;

			for (y = height - 1; y >= 0; y --) {
				for (x = width - 1; x >= 0; x--) {
					buf[i++] = palette[ grid[y,x] * 3 ];
					buf[i++] = palette[ grid[y,x] * 3 + 1];
					buf[i++] = palette[ grid[y,x] * 3 + 2];
					if (with_alpha) {
						if (buf[i - 3] == 0
						    && buf[i - 2] == 0
						    && buf[i - 1] == 0) {
							buf[i++] = 0;
						}
						else
							buf[i++] = 255;
					}
				}
			}

			return buf;
		}

		public static Surface CreateSurfaceFromBitmap (byte[,] grid, ushort width, ushort height, byte[] palette, bool with_alpha)
		{
			/* beware, kind of a gross hack below */
			byte[] buf = GetBitmapData (grid, width, height, palette, with_alpha);

			Surface surf;

			unsafe {
				fixed (void *p = &buf[0]) {
 
					IntPtr handle = Sdl.SDL_CreateRGBSurfaceFrom ((IntPtr)p,
										      width, height, 24,
										      width * (3 + (with_alpha ? 1 : 0)),
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
				}
			}

			return surf;
		}

		public static byte[] ReadStream (Stream stream)
		{
			byte[] buf = new byte [stream.Length];
			stream.Read (buf, 0, buf.Length);
			return buf;
		}

		public static Surface SurfaceFromStream (Stream stream)
		{
			byte[] buf = ReadStream (stream);
			return new Surface (buf);
		}

		public static void PlaySound (Mpq mpq, string resourcePath)
		{
			byte[] buf = GuiUtil.ReadStream ((Stream)mpq.GetResource (resourcePath));
			Sound s = Mixer.Sound (buf);
			s.Play();
		}
	}

}
