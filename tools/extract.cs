using System;
using System.IO;
using SCSharp;

public class Extract {
	public static void Main (string[] args) {
		Mpq mpq = new MpqArchive (args[0]);
		Stream stream = mpq.GetStreamForResource (args[1]);
		Stream outStream = File.OpenWrite (args[2]);

		byte[] buf = new byte[4096];
		int position = 0;

		while (position < stream.Length) {
			int read_length = stream.Read (buf, 0, buf.Length);

			outStream.Write (buf, 0, read_length);
			position += read_length;
		}

		outStream.Close ();
	}
}
