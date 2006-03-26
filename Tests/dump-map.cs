
using System;
using System.IO;

using Starcraft;

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

		Chk chk = (Chk)map.GetResource ("scenario.chk");

		ushort[,] mapTiles = chk.MapTiles;

		byte[] image = new byte[chk.Width * 32 * chk.Height * 32 * 3];

		int stride = chk.Width * 32;

		Stream vx4_fs = (Stream)mpq.GetResource ("tileset\\badlands.vx4");
		Stream vr4_fs = (Stream)mpq.GetResource ("tileset\\badlands.vr4");
		Stream wpe_fs = (Stream)mpq.GetResource ("tileset\\badlands.wpe");

		for (int map_y = 0; map_y < chk.Height; map_y++) {
			for (int map_x = 0; map_x < chk.Width; map_x ++) {

				int mapTile = mapTiles[map_x,map_y];

				bool odd = (mapTile & 0x10) == 0x10;

				int tile_number = (mapTiles[map_x,map_y] >> 4) & 0xffff;

				int tile_x, tile_y;

				vx4_fs.Position = tile_number * 32;

				for (tile_y = 0; tile_y < 4; tile_y ++) {
					for (tile_x = 0; tile_x < 4; tile_x ++) {
						ushort minitile_id = Util.ReadWord (vx4_fs);
						bool flipped = (minitile_id & 0x01) == 0x01;
						minitile_id >>= 1;
						vr4_fs.Position = minitile_id * 64;

						int minitile_x, minitile_y;
						if (flipped) {
							for (minitile_y = 0; minitile_y < 8; minitile_y++)
								for (minitile_x = 0; minitile_x < 8; minitile_x ++) {
									byte palette_entry = Util.ReadByte (vr4_fs);
									wpe_fs.Position = palette_entry * 4;

									image[0 + 3 * (map_x * 32 + (tile_x + 1) * 8 - minitile_x - 1 + stride * (map_y * 32 + tile_y * 8 + minitile_y))] = Util.ReadByte (wpe_fs);
									image[1 + 3 * (map_x * 32 + (tile_x + 1) * 8 - minitile_x - 1 + stride * (map_y * 32 + tile_y * 8 + minitile_y))] = Util.ReadByte (wpe_fs);
									image[2 + 3 * (map_x * 32 + (tile_x + 1) * 8 - minitile_x - 1 + stride * (map_y * 32 + tile_y * 8 + minitile_y))] = Util.ReadByte (wpe_fs);
								}
						}
						else {
							for (minitile_y = 0; minitile_y < 8; minitile_y++) {
								for (minitile_x = 0; minitile_x < 8; minitile_x ++) {
									byte palette_entry = Util.ReadByte (vr4_fs);
									wpe_fs.Position = palette_entry * 4;

									image[0 + 3 * (map_x * 32 + tile_x * 8 + minitile_x + stride * (map_y * 32 + tile_y * 8 + minitile_y))] = Util.ReadByte (wpe_fs);
									image[1 + 3 * (map_x * 32 + tile_x * 8 + minitile_x + stride * (map_y * 32 + tile_y * 8 + minitile_y))] = Util.ReadByte (wpe_fs);
									image[2 + 3 * (map_x * 32 + tile_x * 8 + minitile_x + stride * (map_y * 32 + tile_y * 8 + minitile_y))] = Util.ReadByte (wpe_fs);
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
