
using System;
using System.IO;

using Starcraft;

public class DumpGRP {

	public static void Main (string[] args) {
		string filename = args[0];

		Console.WriteLine ("grp file {0}", filename);

		FileStream fs = File.OpenRead (filename);

		Grp grp = new Grp ();

		((MpqResource)grp).ReadFromStream (fs);

		for (int i = 0; i < grp.FrameCount; i ++) {
			BMP.WriteBMP (String.Format ("output{0:0000}.bmp", i),
				      grp.GetFrame (i),
				      grp.Width, grp.Height,
				      Palette.default_palette);
		}
	}
}
