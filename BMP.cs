
using System;
using System.IO;
using System.Collections.Generic;

using Starcraft;

class BMP {
	public static void WriteBMP (string filename, byte[,] grid, uint width, uint height, byte[] palette)
	{
		using (FileStream fs = File.OpenWrite (filename)) {
			fs.WriteByte ((byte)'B');
			fs.WriteByte ((byte)'M');

			Util.WriteDWord (fs, 0); // size of the file (we seek back and patch it at the end)

			Util.WriteWord (fs, 0);
			Util.WriteWord (fs, 0);

			Util.WriteDWord (fs, 1078);

			// info header
			Util.WriteDWord (fs, 40);
			Util.WriteDWord (fs, width);
			Util.WriteDWord (fs, height);
			Util.WriteWord (fs, 0);
			Util.WriteWord (fs, 8);
			Util.WriteDWord (fs, 0);
			Util.WriteDWord (fs, 0);
			Util.WriteDWord (fs, 0);
			Util.WriteDWord (fs, 0);
			Util.WriteDWord (fs, 0);
			Util.WriteDWord (fs, 0);

			int i;
			// grayscale colormap
			for (i = 0; i < 256*3; i += 3) {
				fs.WriteByte (palette[i]);
				fs.WriteByte (palette[i+1]);
				fs.WriteByte (palette[i+2]);
				fs.WriteByte (0);
			}

			// pixel data
			uint padding = width % 4;
			for (int y = 0; y < height; y ++) {
				for (int x = 0; x < width; x ++)
					fs.WriteByte (grid[y,x]);
				for (i = 0; i < padding; i ++)
					fs.WriteByte (0);
			}

			uint size = (uint)fs.Position;
			fs.Seek (2, SeekOrigin.Begin);
			Util.WriteDWord (fs, size);
			fs.Close();
		}
	}
	
}

