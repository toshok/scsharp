
using System;
using System.IO;

using Starcraft;

public class DumpTileset {

	public static void Main (string[] args) {
		string tileset = args[0];

		Console.WriteLine ("tileset name {0}", tileset);

		FileStream vx4_fs = File.OpenRead (tileset + ".vx4");
		FileStream vr4_fs = File.OpenRead (tileset + ".vr4");

		int tile_number = 1;

		Console.WriteLine ("dumping tile {0}", tile_number);

		int tile_x, tile_y;
		byte[,] tile = new byte[32,32];

		vx4_fs.Position = tile_number * 32;

		for (tile_y = 0; tile_y < 4; tile_y ++) {
			for (tile_x = 0; tile_x < 4; tile_x ++) {
				ushort minitile_id = Util.ReadWord (vx4_fs);
				bool flipped = (minitile_id & 0x01) == 0x01;
				minitile_id >>= 1;
				vr4_fs.Position = minitile_id * 64;

				Console.WriteLine ("mini-tile {0},{1} = {2}", tile_x, tile_y, minitile_id);

				int minitile_x, minitile_y;
				if (flipped) {
					Console.WriteLine ("flipped = true!");
					for (minitile_y = 0; minitile_y < 8; minitile_y++)
						for (minitile_x = 0; minitile_x < 8; minitile_x ++) {
							tile[(tile_x + 1) * 8 - minitile_x - 1, tile_y * 8 + minitile_y] = Util.ReadByte (vr4_fs);
						}
				}
				else {
					for (minitile_y = 0; minitile_y < 8; minitile_y++) {
						Console.Write ("   [ ");
						for (minitile_x = 0; minitile_x < 8; minitile_x ++) {
							tile[tile_x * 8 + minitile_x, tile_y * 8 + minitile_y] = Util.ReadByte (vr4_fs);
							Console.Write ("{0} ", tile[tile_x * 8 + minitile_x, tile_y * 8 + minitile_y]);
						}
						Console.WriteLine (" ]");
					}
				}
			}
		}

		BMP.WriteBMP (String.Format ("tile{0:0000}.bmp", tile_number),
			      tile, 32, 32,
			      Palette.grayscale_palette);
	}
}
