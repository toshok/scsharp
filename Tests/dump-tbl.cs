using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using SCSharp;

public class DumpTBL {
	public static void Main (string[] args) {
		string filename = args[0];

		FileStream fs = File.OpenRead (filename);

		Tbl tbl = new Tbl ();

		((MpqResource)tbl).ReadFromStream (fs);

		Console.WriteLine ("dumping {0}", filename);
		for (int i = 0; i < tbl.Strings.Length; i ++)
			Console.WriteLine ("strings[{0}] = {1}", i, tbl.Strings[i]);
	}
}
