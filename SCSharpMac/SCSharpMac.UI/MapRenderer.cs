//
// SCSharp.UI.MapRenderer
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using MonoMac.CoreGraphics;
using MonoMac.CoreAnimation;

using SCSharp;

namespace SCSharpMac.UI
{
	public class MapRenderer
	{
		Mpq mpq;
		Chk chk;

		byte[] cv5;
		byte[] vx4;
		byte[] vr4;
		byte[] vf4;
		byte[] wpe;

		Dictionary <int,CGImage> tileCache;

		int pixel_width;
		int pixel_height;

		public MapRenderer (Mpq mpq, Chk chk, int width, int height)
			: this (mpq, chk)
		{
			this.width = width;
			this.height = height;
		}

		public MapRenderer (Mpq mpq, Chk chk)
		{
			this.mpq = mpq;
			this.chk = chk;

			pixel_width = (ushort)(chk.Width * 32);
			pixel_height = (ushort)(chk.Height * 32);

			Stream cv5_fs = (Stream)mpq.GetResource (String.Format ("tileset\\{0}.cv5", Util.TilesetNames[(int)chk.Tileset]));
			cv5 = new byte [cv5_fs.Length];
			cv5_fs.Read (cv5, 0, (int)cv5_fs.Length);
			cv5_fs.Close ();

			Stream vx4_fs = (Stream)mpq.GetResource (String.Format ("tileset\\{0}.vx4", Util.TilesetNames[(int)chk.Tileset]));
			vx4 = new byte [vx4_fs.Length];
			vx4_fs.Read (vx4, 0, (int)vx4_fs.Length);
			vx4_fs.Close ();

			Stream vr4_fs = (Stream)mpq.GetResource (String.Format ("tileset\\{0}.vr4", Util.TilesetNames[(int)chk.Tileset]));
			vr4 = new byte [vr4_fs.Length];
			vr4_fs.Read (vr4, 0, (int)vr4_fs.Length);
			vr4_fs.Close ();

			Stream vf4_fs = (Stream)mpq.GetResource (String.Format ("tileset\\{0}.vf4", Util.TilesetNames[(int)chk.Tileset]));
			vf4 = new byte [vf4_fs.Length];
			vf4_fs.Read (vf4, 0, (int)vf4_fs.Length);
			vf4_fs.Close ();

			Stream wpe_fs = (Stream)mpq.GetResource (String.Format ("tileset\\{0}.wpe", Util.TilesetNames[(int)chk.Tileset]));
			wpe = new byte [wpe_fs.Length];
			wpe_fs.Read (wpe, 0, (int)wpe_fs.Length);
			wpe_fs.Close ();
			
			mapLayer = (CATiledLayer)CATiledLayer.Create ();
			mapLayer.TileSize = new SizeF (32, 32);
			mapLayer.Bounds = new RectangleF (0, 0, pixel_width, pixel_height);
			mapLayer.AnchorPoint = new PointF (0, 0);
			mapLayerDelegate = new MapLayerDelegate (this);
			mapLayer.Delegate = mapLayerDelegate;
			mapLayer.SetNeedsDisplay ();
		}

		public int MapWidth {
			get { return pixel_width; }
		}

		public int MapHeight {
			get { return pixel_height; }
		}
		
		public CALayer MapLayer {
			get { return mapLayer; }
		}
		
		class MapLayerDelegate : CALayerDelegate {
			MapRenderer renderer;
			
			public MapLayerDelegate (MapRenderer renderer) {
				this.renderer = renderer;
			}
			
			public override void DrawLayer (CALayer layer, CGContext context)
			{
				RectangleF box = context.GetClipBoundingBox ();
				int tile_x = (int)(box.X / 32);
				int tile_y = (int)((renderer.pixel_height - box.Y) / 32);
				
				try {
					ushort[,] mapTiles = renderer.Chk.MapTiles;
				
					CGImage image = renderer.GetTile (mapTiles[tile_x, tile_y]);
					context.DrawImage (box, image);
				}
				catch (Exception e) {
					Console.WriteLine (e);
				}
			}
		}
		
		int width;
		int height;
		CATiledLayer mapLayer;
		MapLayerDelegate mapLayerDelegate;

		public void SetUpperLeft (int x, int y)
		{
#if notyet
			/* XXX be naive for now, and redraw the entire section we need */
			if (mapSurface == null)
				mapSurface = new Surface (width, height);

			int tl_x = x / 32;
			int tl_y = y / 32;

			for (int map_y = tl_y; map_y <= tl_y + width / 32 && map_y < chk.Height; map_y ++) {
				for (int map_x = tl_x; map_x <= tl_x + width / 32 && map_x < chk.Width; map_x ++) {
					Surface tile = GetTile(chk.MapTiles[map_x,map_y]);
					mapSurface.Blit (tile, new Point (map_x * 32 - x, map_y * 32 - y));
				}
			}

			Painter.Invalidate (new Rectangle (new Point (0,0),
							   new Size (width, height)));
#else
			mapLayer.Position = new PointF (x-pixel_width, 480+y-pixel_height);
#endif
		}

		byte[] image;
		CGImage GetTile (int mapTile)
		{
			if (tileCache == null)
				tileCache = new Dictionary<int,CGImage>();

			//					bool odd = (mapTile & 0x10) == 0x10;

			int tile_group = mapTile >> 4; /* the tile's group in the cv5 file */
			int tile_number = mapTile & 0x0F;    /* the megatile within the tile group */

			int megatile_id = Util.ReadWord (cv5, (tile_group * 26 + 10 + tile_number) * 2);

			if (tileCache.ContainsKey (megatile_id))
				return tileCache[megatile_id];

			if (image == null)
				image = new byte[32 * 32 * 4];

			int minitile_x, minitile_y;

			for (minitile_y = 0; minitile_y < 4; minitile_y ++) {
				for (minitile_x = 0; minitile_x < 4; minitile_x ++) {
					ushort minitile_id = Util.ReadWord (vx4, megatile_id * 32 + minitile_y * 8 + minitile_x * 2);
					ushort minitile_flags = Util.ReadWord (vf4, megatile_id * 32 + minitile_y * 8 + minitile_x * 2);
					bool flipped = (minitile_id & 0x01) == 0x01;
					minitile_id >>= 1;

					int pixel_x, pixel_y;
					if (flipped) {
						for (pixel_y = 0; pixel_y < 8; pixel_y++)
							for (pixel_x = 0; pixel_x < 8; pixel_x ++) {
								int x = (minitile_x + 1) * 8 - pixel_x - 1;
								int y = (minitile_y * 8) * 32 + pixel_y * 32;

								byte palette_entry = vr4[minitile_id * 64 + pixel_y * 8 + pixel_x];

								image[0 + 4 * (x + y)] = (byte)(255 - wpe[palette_entry * 4 + 3]);
								image[1 + 4 * (x + y)] = wpe[palette_entry * 4 + 2];
								image[2 + 4 * (x + y)] = wpe[palette_entry * 4 + 1];
								image[3 + 4 * (x + y)] = wpe[palette_entry * 4 + 0];
							}
					}
					else {
						for (pixel_y = 0; pixel_y < 8; pixel_y++) {
							for (pixel_x = 0; pixel_x < 8; pixel_x ++) {
								int x = minitile_x * 8 + pixel_x;
								int y = (minitile_y * 8) * 32 + pixel_y * 32;

								byte palette_entry = vr4[minitile_id * 64 + pixel_y * 8 + pixel_x];

								image[0 + 4 * (x + y)] = (byte)(255 - wpe[palette_entry * 4 + 3]);
								image[1 + 4 * (x + y)] = wpe[palette_entry * 4 + 2];
								image[2 + 4 * (x + y)] = wpe[palette_entry * 4 + 1];
								image[3 + 4 * (x + y)] = wpe[palette_entry * 4 + 0];
							}
						}
					}
				}
			}

			CGImage tile = GuiUtil.CreateImage (image, 32, 32, 32, 32 * 4);
			tileCache [megatile_id] = tile;

			return tile;
		}

		public bool Navigable (MapPoint point)
		{
			// this assumes that point corresponds to a mini tile location
			
			// first calculate the megatile
			int megatile_x, megatile_y;
			megatile_x = point.X >> 3;
			megatile_y = point.Y >> 3;

			if ((megatile_x >= chk.Width || megatile_x < 0) ||
			    (megatile_y >= chk.Height || megatile_y < 0))
				return false;

			int mapTile = chk.MapTiles[megatile_x,megatile_y];

			int tile_group = mapTile >> 4; /* the tile's group in the cv5 file */
			int tile_number = mapTile & 0x0F;    /* the megatile within the tile group */

			int megatile_id = Util.ReadWord (cv5, (tile_group * 26 + 10 + tile_number) * 2);

			ushort minitile_flags = Util.ReadWord (vf4, megatile_id * 32 + point.Y * 8 + point.X * 2);

			// Console.WriteLine ("minitile {0},{1} is navigable?  {2}", point.X, point.Y, (minitile_flags & 0x0001) == 0x0001);
			return (minitile_flags & 0x0001) == 0x0001;
		}
		
		// TODO: give this layer a delegate and render the tiles we need to at runtime
		public CALayer RenderToLayer ()
		{
			byte[] bitmap = RenderToBitmap (mpq, chk);
			
			return GuiUtil.CreateLayerFromRGBAData (bitmap, (ushort)pixel_width, (ushort)pixel_height, 32, (ushort)(pixel_width * 4));
		}
		
		// TODO: make this code use a paletted CGImage instead of doing the expansion ourselves (can CGImage do rgba paletted images?)
		public byte[] RenderToBitmap (Mpq mpq, Chk chk)
		{
			ushort[,] mapTiles = chk.MapTiles;

			byte[] image = new byte[pixel_width * pixel_height * 4];

			for (int map_y = 0; map_y < chk.Height; map_y++) {
				for (int map_x = 0; map_x < chk.Width; map_x ++) {
					int mapTile = mapTiles[map_x,map_y];

					//					bool odd = (mapTile & 0x10) == 0x10;

					int tile_group = mapTile >> 4; /* the tile's group in the cv5 file */
					int tile_number = mapTile & 0x0F;    /* the megatile within the tile group */

					int megatile_id = Util.ReadWord (cv5, (tile_group * 26 + 10 + tile_number) * 2);

					int minitile_x, minitile_y;

					for (minitile_y = 0; minitile_y < 4; minitile_y ++) {
						for (minitile_x = 0; minitile_x < 4; minitile_x ++) {
							ushort minitile_id = Util.ReadWord (vx4, megatile_id * 32 + minitile_y * 8 + minitile_x * 2);
							bool flipped = (minitile_id & 0x01) == 0x01;
							minitile_id >>= 1;

							int pixel_x, pixel_y;
							if (flipped) {
								for (pixel_y = 0; pixel_y < 8; pixel_y++)
									for (pixel_x = 0; pixel_x < 8; pixel_x ++) {
										int x = map_x * 32 + (minitile_x + 1) * 8 - pixel_x - 1;
										int y = (map_y * 32 + minitile_y * 8) * pixel_width + pixel_y * pixel_width;

										byte palette_entry = vr4[minitile_id * 64 + pixel_y * 8 + pixel_x];

										image[0 + 4 * (x + y)] = wpe[palette_entry * 4 + 2];
										image[1 + 4 * (x + y)] = wpe[palette_entry * 4 + 1];
										image[2 + 4 * (x + y)] = wpe[palette_entry * 4 + 0];
										image[3 + 4 * (x + y)] = (byte)(255 - wpe[palette_entry * 4 + 3]);
									}
							}
							else {
								for (pixel_y = 0; pixel_y < 8; pixel_y++) {
									for (pixel_x = 0; pixel_x < 8; pixel_x ++) {
										int x = map_x * 32 + minitile_x * 8 + pixel_x;
										int y = (map_y * 32 + minitile_y * 8) * pixel_width + pixel_y * pixel_width;

										byte palette_entry = vr4[minitile_id * 64 + pixel_y * 8 + pixel_x];

										image[0 + 4 * (x + y)] = wpe[palette_entry * 4 + 2];
										image[1 + 4 * (x + y)] = wpe[palette_entry * 4 + 1];
										image[2 + 4 * (x + y)] = wpe[palette_entry * 4 + 0];
										image[3 + 4 * (x + y)] = (byte)(255 - wpe[palette_entry * 4 + 3]);
									}
								}
							}
						}
					}
				}
			}

			return image;
		}

		public Chk Chk {
			get { return chk; }
		}
	}
}
