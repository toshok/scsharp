using System;
using System.IO;
using System.Threading;
using MpqReader;
using System.Reflection;

public class Brute {
	const int MAX_COUNT = 45;

	static string allowable = "()\\/._0123456789abcdefghijklmnopqrstuvwxyz";

	static StreamWriter sw;

	static int entry_count = 0;

	static object iolock = new object ();
	static object consolelock = new object ();

	static void TestStrings (MpqArchive mpq, string dot, int count, char[] test, int index) {
		if (index == count) {
			entry_count ++;
			if (entry_count % 10000 == 0) {
				lock (consolelock) {
					Console.Write (dot);
					Console.Out.Flush ();
				}
			}

			String entry = new String (test, 0, count);
			if (mpq.FileExists (entry)) {
				lock (iolock) {
					sw.WriteLine (entry);
				}
			}
			return;
		}
		for (int j = 0; j < allowable.Length; j ++) {
			test[index] = allowable[j];
			TestStrings (mpq, dot, count, test, index + 1);
		}
	}


	public static void Main (string[] args) {
		if (args.Length < 2) {
			Console.WriteLine ("usage:  brute.exe <mpq-file> <output-list.txt>");
			Environment.Exit (0);
		}

		MpqArchive mpq = new MpqArchive (args[0]);
		sw = File.CreateText (args[1]);

		Thread t = new Thread (delegate (object state) {
			for (int count = 8; count < MAX_COUNT; count ++) {
				if (count % 2 == 0)
					continue;
				char[] test = new char [count];
				lock (consolelock) {
					Console.Write ("T{0}", count);
					Console.Out.Flush();
				}
				TestStrings (mpq, "t", count, test, 0);
			}
		});

		t.Start();

		for (int count = 8; count < MAX_COUNT; count ++) {
			if (count % 2 == 1)
				continue;
			char[] test = new char [count];
			lock (consolelock) {
				Console.Write ("M{0}", count);
				Console.Out.Flush ();
			}
			TestStrings (mpq, "m", count, test, 0);
		}
	}
}
