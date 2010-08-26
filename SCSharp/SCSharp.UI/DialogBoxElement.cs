//
// SCSharp.UI.DialogBoxElement
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
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class DialogBoxElement : UIElement
	{
		public DialogBoxElement (UIScreen screen, BinElement el, byte[] palette)
			: base (screen, el, palette)
		{
			tileGrp = (Grp)Mpq.GetResource ("dlgs\\tile.grp");
		}

		Grp tileGrp;

		const int TILE_TR = 2;
		const int TILE_T = 1;
		const int TILE_TL = 0;
		const int TILE_R = 5;
		const int TILE_C = 4;
		const int TILE_L = 3;
		const int TILE_BR = 8;
		const int TILE_B = 7;
		const int TILE_BL = 6;

		void TileRow (Surface surf, Grp grp, byte[] pal, int l, int c, int r, int y)
		{
			Surface lsurf = GuiUtil.CreateSurfaceFromBitmap (grp.GetFrame (l),
									 grp.Width, grp.Height,
									 pal,
									 41, 0);

			Surface csurf = GuiUtil.CreateSurfaceFromBitmap (grp.GetFrame (c),
									 grp.Width, grp.Height,
									 pal,
									 41, 0);


			Surface rsurf = GuiUtil.CreateSurfaceFromBitmap (grp.GetFrame (r),
									 grp.Width, grp.Height,
									 pal,
									 41, 0);


			surf.Blit (lsurf, new Point (0,y));
			for (int x = grp.Width; x < surf.Width - grp.Width; x += grp.Width)
				surf.Blit (csurf, new Point (x, y));
			surf.Blit (rsurf, new Point (surf.Width - grp.Width,y));
		}

		protected override Surface CreateSurface ()
		{
			if (ParentScreen.Background == null && ParentScreen.UseTiles) {
				Surface surf = new Surface (Width, Height);
				surf.Fill (new Rectangle (new Point (0,0), new Size (Width, Height)),
					   Color.FromArgb (0,0,0,0));
				surf.TransparentColor = Color.Black; /* XXX */

				Pcx pal = new Pcx ();
				pal.ReadFromStream ((Stream)Mpq.GetResource ("unit\\cmdbtns\\ticon.pcx"),
						    -1, -1);

				/* tile the top border */
				TileRow (surf, tileGrp, pal.Palette, TILE_TL, TILE_T, TILE_TR, 0);

				/* tile everything down to the bottom border */
				for (int y = tileGrp.Height - 2; y < surf.Height - tileGrp.Height; y += tileGrp.Height - 2)
					TileRow (surf, tileGrp, pal.Palette, TILE_L, TILE_C, TILE_R, y);

				/* tile the bottom row */
				TileRow (surf, tileGrp, pal.Palette, TILE_BL, TILE_B, TILE_BR, surf.Height - tileGrp.Height);
				return surf;
			}
			else
				return null;
		}
	}

}
