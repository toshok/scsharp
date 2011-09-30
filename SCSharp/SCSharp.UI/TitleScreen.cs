//
// SCSharp.UI.TitleScreen
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

using SdlDotNet.Core;
using SdlDotNet.Graphics;

using System.Drawing;

namespace SCSharp.UI
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

			Elements[COPYRIGHT1_ELEMENT_INDEX].Text = "Game code Copyright © 2006-2010 Chris Toshok.  All rights reserved.";
			Elements[COPYRIGHT2_ELEMENT_INDEX].Text = "Game assets Copyright © 1998 Blizzard Entertainment. All rights reserved.";
			Elements[COPYRIGHT3_ELEMENT_INDEX].Text = "";

		}

		public override void AddToPainter ()
		{
			base.AddToPainter ();
			Events.Tick += LoadingFlasher;
		}
		
		public override void RemoveFromPainter ()
		{
			base.RemoveFromPainter ();
			Events.Tick -= LoadingFlasher;
		}

		const int COPYRIGHT1_ELEMENT_INDEX = 1;
		const int COPYRIGHT2_ELEMENT_INDEX = 2;
		const int COPYRIGHT3_ELEMENT_INDEX = 3;
		const int LOADING_ELEMENT_INDEX = 4;

		const int FLASH_ON_DURATION = 1000;
		const int FLASH_OFF_DURATION = 500;
		int totalElapsed;

		void LoadingFlasher (object sender, TickEventArgs e)
		{
			totalElapsed += e.TicksElapsed;

			if ((Elements[LOADING_ELEMENT_INDEX].Visible && (totalElapsed < FLASH_ON_DURATION)) ||
			    (!Elements[LOADING_ELEMENT_INDEX].Visible && (totalElapsed < FLASH_OFF_DURATION)) )
				return;

			Console.WriteLine ("Flashing");

			Elements[LOADING_ELEMENT_INDEX].Visible = !Elements[LOADING_ELEMENT_INDEX].Visible;

			totalElapsed = 0;
		}
	}
}
