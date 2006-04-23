using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
{
	public class TitleScreen : UIScreen
	{
		public TitleScreen (Mpq mpq) : base (mpq, "glue\\Palmm", Builtins.rez_TitleDlgBin)
		{
			background_path = Builtins.TitlePcx;
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();
			Cursor = null; /* clear out the cursor */

			Elements[1].Text = "Copyright © 2006 Chris Toshok.  All rights reserved.";
			Elements[2].Text = "Game assets Copyright © 1998 Blizzard Entertainment. All rights reserved.";
			Elements[3].Text = "";

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}.Text = {1}", i, Elements[i].Text);
		}
	}
}
