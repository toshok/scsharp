using System;
using System.IO;
using SCSharp;
using System.Reflection;

public class ListMpq {
	public static void Main (string[] args) {
		Mpq mpq = new MpqArchive (args[0]);

		StreamReader sr = new StreamReader (Assembly.GetExecutingAssembly().GetManifestResourceStream ("list.txt"));

		string entry;
		while ((entry = sr.ReadLine()) != null) {
			entry = entry.ToLower ();
			Stream mpq_stream = mpq.GetStreamForResource (entry);
			if (mpq_stream == null)
				continue;

			Console.WriteLine ("{0} {1}", entry, mpq_stream.Length);
			mpq_stream.Dispose();
		}
	}
}
