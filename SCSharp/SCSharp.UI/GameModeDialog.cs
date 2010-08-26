//
// SCSharp.UI.GameModeDialog
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
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class GameModeDialog : UIDialog
	{
		public GameModeDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_GluGameModeBin)
		{
			background_path = "glue\\Palmm\\retail_ex.pcx";
			background_translucent = 42;
			background_transparent = 0;
		}

		const int ORIGINAL_ELEMENT_INDEX = 1;
		const int TITLE_ELEMENT_INDEX = 2;
		const int EXPANSION_ELEMENT_INDEX = 3;
		const int CANCEL_ELEMENT_INDEX = 4;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++) {
				Console.WriteLine ("{0}: {1}", i, Elements[i].Text);
			}

			Elements[TITLE_ELEMENT_INDEX].Text = GlobalResources.Instance.BrooDat.GluAllTbl.Strings[172];

			Elements[ORIGINAL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Activate != null)
						Activate (false);
				};

			Elements[EXPANSION_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Activate != null)
						Activate (true);
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Cancel != null)
						Cancel ();
				};
		}

		public event DialogEvent Cancel;
		public event GameModeActivateDelegate Activate;
	}

	public delegate void GameModeActivateDelegate (bool expansion);
}
