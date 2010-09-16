//
// Driver
//
// Authors:
//	Chris Toshok (toshok@gmail.com)
//
// Copyright 2006-2010 Chris Toshok
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Configuration;

using SCSharp.UI;

using SdlDotNet;

public class Driver
{
	public static void Main (string[] args)
	{
		bool fullscreen = false;

		string sc_cd_dir = ConfigurationManager.AppSettings["StarcraftCDDirectory"];
		string bw_cd_dir = ConfigurationManager.AppSettings["BroodwarCDDirectory"];

		/* catch this pathological condition where someone has set the cd directories to the same location. */
		if (!string.IsNullOrEmpty (sc_cd_dir) && !string.IsNullOrEmpty (bw_cd_dir) && bw_cd_dir == sc_cd_dir) {
			Console.WriteLine ("The StarcraftCDDirectory and BroodwarCDDirectory configuration settings must have unique values.");
			return;
		}

		Game g = new Game (ConfigurationManager.AppSettings["StarcraftDirectory"],
				   sc_cd_dir, bw_cd_dir);

		if (args.Length > 0)
			if (args[0] == "/fullscreen")
				fullscreen = true;

		g.Startup(fullscreen);
	}
}
