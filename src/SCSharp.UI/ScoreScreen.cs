//
// SCSharp.UI.ScoreScreen
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

namespace SCSharp.UI
{
	public class ScoreScreen : UIScreen
	{
#if false
		SwooshPainter swoosher;
#endif
		bool victorious;

		public ScoreScreen (Mpq mpq, bool victorious)
			: base  (mpq, String.Format ("glue\\Pal{0}{1}", Util.RaceChar[(int)Game.Instance.Race], victorious ? 'v' : 'd'), Builtins.rez_GluScoreBin)
		{
			this.mpq = mpq;
			this.victorious = victorious;
		}

#if false
		void DoneSwooshingIn ()
		{
			Console.WriteLine ("Done Swooshing In");
			Game.Instance.Cursor = cursor;

			painter.Remove (Layer.UI, swoosher.Paint);
			painter.Add (Layer.UI, UIPainter.Paint);
			RaiseDoneSwooshing();
		}

		void pMainPainter (Gdk.Pixbuf pb, DateTime now)
		{
			if (translucent)
				Console.WriteLine ("translucent!");
			pMainPb.Composite (pb, 0, 0, pMainPb.Width, pMainPb.Height,
					   0, 0, 1, 1, InterpType.Nearest, 0xff);
		}

		public override void SwooshIn ()
		{
			swoosher = new SwooshPainter (SwooshPainter.Direction.FromLeft,
						      pMainPainter,
						      0, 0, translucent ? (int)(0xff * 0.75) : 0xff);
			swoosher.DoneSwooshing += DoneSwooshingIn;
			painter.Add (Layer.UI, swoosher.Paint);
		}

		public override void SwooshOut ()
		{
			base.SwooshOut();
		}
#endif

		const int MAIN_IMAGE_INDEX = 1;
		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Elements[MAIN_IMAGE_INDEX].Text = String.Format (victorious ? Builtins.Scorev_pMainPcx : Builtins.Scored_pMainPcx,
									 Util.RaceChar[(int)Game.Instance.Race]);
			Console.WriteLine ("screen path = {0}", Elements[MAIN_IMAGE_INDEX].Text);
		}
	}
}
