
using System;
using System.IO;

using SCSharp;

public class DumpCHK {

	static Mpq GetMPQ (string path)
	{
		if (Directory.Exists (path))
			return new MpqDirectory (path);
		else if (File.Exists (path))
			return new MpqArchive (path);
		else
			throw new Exception (); // XX
	}

	public static void Main (string[] args) {
		string mpqpath = args[0];
		string mappath = args[1];

		Console.WriteLine ("Map name {0}", mappath);

		Mpq mpq = GetMPQ (mpqpath);
		Mpq map = GetMPQ (mappath);

		Chk chk = (Chk)map.GetResource ("staredit\\scenario.chk");

		ushort[,] mapTiles = chk.MapTiles;

		byte[] image = new byte[chk.Width * 32 * chk.Height * 32 * 3];

		/*
		  00 - Badlands
		  01 - Space Platform
		  02 - Installation
		  03 - Ashworld
		  04 - Jungle World
		  05 - Desert World
		  06 - Arctic World
		  07 - Twilight World
		*/

		string[] tileset_names = new string[] {
			"badlands",
			"platform",
			"install",
			"ashworld",
			"jungle",
			"desert",
			"ice",
			"twilight"
		};

		Stream cv5_fs = (Stream)mpq.GetResource (String.Format ("tileset\\{0}.cv5", tileset_names[chk.TileSet]));
		Stream vx4_fs = (Stream)mpq.GetResource (String.Format ("tileset\\{0}.vx4", tileset_names[chk.TileSet]));
		Stream vr4_fs = (Stream)mpq.GetResource (String.Format ("tileset\\{0}.vr4", tileset_names[chk.TileSet]));
		Stream wpe_fs = (Stream)mpq.GetResource (String.Format ("tileset\\{0}.wpe", tileset_names[chk.TileSet]));

		byte[] cv5 = new byte [cv5_fs.Length];
		cv5_fs.Read (cv5, 0, (int)cv5_fs.Length);

		byte[] vx4 = new byte [vx4_fs.Length];
		vx4_fs.Read (vx4, 0, (int)vx4_fs.Length);

		byte[] vr4 = new byte [vr4_fs.Length];
		vr4_fs.Read (vr4, 0, (int)vr4_fs.Length);

		byte[] wpe = new byte [wpe_fs.Length];
		wpe_fs.Read (wpe, 0, (int)wpe_fs.Length);

		for (int map_y = 0; map_y < chk.Height; map_y++) {
			for (int map_x = 0; map_x < chk.Width; map_x ++) {
				int mapTile = mapTiles[map_x,map_y];

				bool odd = (mapTile & 0x10) == 0x10;

				int tile_group = mapTile >> 4; /* the tile's group in the cv5 file */
				int tile_number = mapTile & 0x0F;    /* the megatile within the tile group */

				int megatile_id = Util.ReadWord (cv5, (tile_group * 26 + 10 + tile_number) * 2);

				if (map_y == 0) {
					Console.Write ("[{0}.{1}:0x{2:x}]", tile_group, tile_number, megatile_id);
					if (map_x == chk.Width - 1)
						Console.WriteLine ();
				}

				int minitile_x, minitile_y;

				//				Console.WriteLine ("[{0},{1}] = {2} ({3:x} = {4:x}:{5:x})", map_x, map_y, megatile_id, mapTile, tile_group, tile_number);

				for (minitile_y = 0; minitile_y < 4; minitile_y ++) {
					for (minitile_x = 0; minitile_x < 4; minitile_x ++) {
						ushort minitile_id = Util.ReadWord (vx4, megatile_id * 32 + minitile_y * 8 + minitile_x * 2);
						bool flipped = (minitile_id & 0x01) == 0x01;
						minitile_id >>= 1;

						//						Console.WriteLine ("minitile [{0},{1}] = {2}", minitile_x, minitile_y, minitile_id);

						int pixel_x, pixel_y;
						if (flipped) {
							for (pixel_y = 0; pixel_y < 8; pixel_y++)
								for (pixel_x = 0; pixel_x < 8; pixel_x ++) {
									int x = map_x * 32 + (minitile_x + 1) * 8 - pixel_x - 1;
									int y = (map_y * 32 + minitile_y * 8) * chk.Width * 32 + pixel_y * chk.Width * 32;

									byte palette_entry = vr4[minitile_id * 64 + pixel_y * 8 + pixel_x];

									image[0 + 3 * (x + y)] = wpe[palette_entry * 4 + 2];
									image[1 + 3 * (x + y)] = wpe[palette_entry * 4 + 1];
									image[2 + 3 * (x + y)] = wpe[palette_entry * 4 + 0];
								}
						}
						else {
							for (pixel_y = 0; pixel_y < 8; pixel_y++) {
								for (pixel_x = 0; pixel_x < 8; pixel_x ++) {
									int x = map_x * 32 + minitile_x * 8 + pixel_x;
									int y = (map_y * 32 + minitile_y * 8) * chk.Width * 32 + pixel_y * chk.Width * 32;

									byte palette_entry = vr4[minitile_id * 64 + pixel_y * 8 + pixel_x];

									image[0 + 3 * (x + y)] = wpe[palette_entry * 4 + 2];
									image[1 + 3 * (x + y)] = wpe[palette_entry * 4 + 1];
									image[2 + 3 * (x + y)] = wpe[palette_entry * 4 + 0];
								}
							}
						}
					}
				}
			}
		}

		TGA.WriteTGA ("map.tga",
			      image, (uint)chk.Width * 32, (uint)chk.Height * 32);
	}
}
