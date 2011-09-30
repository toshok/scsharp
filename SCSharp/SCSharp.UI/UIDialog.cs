//
// SCSharp.UI.UIDialog
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
using System.Drawing;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using SdlDotNet.Core;
using SdlDotNet.Graphics;


namespace SCSharp.UI
{
	/* this should probably subclass from UIElement instead...  look into that */
	public abstract class UIDialog : UIScreen
	{
		protected UIScreen parent;
		bool dimScreen;
		Surface dimScreenSurface;

		protected UIDialog (UIScreen parent, Mpq mpq, string prefix, string binFile)
			: base (mpq, prefix, binFile)
		{
			this.parent = parent;
			background_translucent = 254;
			background_transparent = 0;

			dimScreen = true;

			dimScreenSurface = new Surface (Painter.SCREEN_RES_X, Painter.SCREEN_RES_Y);
			dimScreenSurface.Alpha = 100;
			dimScreenSurface.AlphaBlending = true;
		}

		public override void AddToPainter ()
		{
			if (Background != null)
				Painter.Add (Layer.DialogBackground, BackgroundPainter);

			if (UIPainter != null)
				Painter.Add (Layer.DialogUI, UIPainter.Paint);

			if (dimScreen)
				Painter.Add (Layer.DialogDimScreenHack, DimScreenPainter);

			Painter.Invalidate ();
		}


		public override void RemoveFromPainter ()
		{
			if (Background != null)
				Painter.Remove (Layer.DialogBackground, BackgroundPainter);

			if (UIPainter != null)
				Painter.Remove (Layer.DialogUI, UIPainter.Paint);

			if (dimScreen)
				Painter.Remove (Layer.DialogDimScreenHack, DimScreenPainter);

			Painter.Invalidate ();
		}

		void DimScreenPainter (DateTime dt)
		{
			Painter.Blit (dimScreenSurface, Painter.Dirty, Painter.Dirty);
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			/* figure out where we're going to be located on the screen */
			int baseX, baseY;
			int si;

			if (Background != null) {
				baseX = (Painter.SCREEN_RES_X - Background.Width) / 2;
				baseY = (Painter.SCREEN_RES_Y - Background.Height) / 2;
				si = 0;
			}
			else {
				baseX = Elements[0].X1;
				baseY = Elements[0].Y1;
				si = 1;
			}

			/* and add that offset to all our elements */
			for (int i = si; i < Elements.Count; i ++) {
				Elements[i].X1 += (ushort)baseX;
				Elements[i].Y1 += (ushort)baseY;
			}
		}

		public override bool UseTiles {
			get { return true; }
		}

		public UIScreen Parent {
			get { return parent; }
		}

		public bool DimScreen {
			get { return dimScreen; }
			set { dimScreen = value; }
		}

		public override void ShowDialog (UIDialog dialog)
		{
			Console.WriteLine ("showing {0}", dialog);

			if (this.dialog != null)
				throw new Exception ("only one active dialog is allowed");
			this.dialog = dialog;

			dialog.Load ();
			dialog.Ready += delegate () { 
				dialog.AddToPainter ();
				RemoveFromPainter ();
			};
		}

		public override void DismissDialog ()
		{
			if (dialog == null)
				return;

			dialog.RemoveFromPainter ();
			dialog = null;
			AddToPainter ();
		}
	}

	public delegate void DialogEvent ();
}
