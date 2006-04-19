
using System;
using System.IO;
using System.Collections.Generic;

using SCSharp;

class TGA {
	public static void WriteTGA (string filename, byte[] image, uint width, uint height)
	{
		using (FileStream fs = File.OpenWrite (filename)) {
			fs.WriteByte (0);
			fs.WriteByte (0); // no colormap
			fs.WriteByte (2); // rgb

			Util.WriteWord (fs, 0); // first color map entry
			Util.WriteWord (fs, 0); // number of colors in palette
			fs.WriteByte (0); // number of bites per palette entry

			Console.WriteLine ("width = {0}, height = {1}", (ushort)width, (ushort)height);

			Util.WriteWord (fs, 0); // image x origin
			Util.WriteWord (fs, 0); // image y origin
			Util.WriteWord (fs, (ushort)width); // width
			Util.WriteWord (fs, (ushort)height); // height

			fs.WriteByte (24); // bits per pixel

			fs.WriteByte (32);

			fs.Write (image, 0, image.Length);

			fs.Close();
		}
	}
	
}

