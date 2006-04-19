
using System;
using System.IO;

using SCSharp;

public class DumpGRP {

	public static void Main (string[] args) {
		string filename = args[0];
		string palettename = args[1];

		Console.WriteLine ("grp file {0}", filename);
		Console.WriteLine ("palette file {0}", palettename);

		FileStream fs = File.OpenRead (filename);

		Grp grp = new Grp ();

		((MpqResource)grp).ReadFromStream (fs);
		Pcx pal = new Pcx ();
		pal.ReadFromStream (File.OpenRead(palettename), -1, -1);

		for (int i = 0; i < grp.FrameCount; i ++) {
			BMP.WriteBMP (String.Format ("output{0:0000}.bmp", i),
				      grp.GetFrame (i),
				      grp.Width, grp.Height,
				      pal.Palette);
		}
	}
}
