//
// SCSharpMac.UI.UIDialog
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

using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

using SCSharp;

namespace SCSharpMac.UI
{
	/* this should probably subclass from UIElement instead...  look into that */
	public abstract class UIDialog : UIScreen
	{
		protected UIScreen parent;
		bool dimScreen;
		CALayer dimLayer;

		protected UIDialog (UIScreen parent, Mpq mpq, string prefix, string binFile)
			: base (mpq, prefix, binFile)
		{
			this.parent = parent;
			background_translucent = 254;
			background_transparent = 0;
			
			dimScreen = true;
			
			dimLayer = CALayer.Create ();
			dimLayer.Bounds = parent.Bounds;
			dimLayer.AnchorPoint = new PointF (0, 0);
			dimLayer.BackgroundColor = new CGColor (0, 0, 0, .7f);
		}

		public override void AddToPainter ()
		{
			if (dimScreen)
				parent.AddSublayer (dimLayer);
			parent.AddSublayer (this);
		}


		public override void RemoveFromPainter ()
		{
			RemoveFromSuperLayer ();
			if (dimScreen)
				dimLayer.RemoveFromSuperLayer ();
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			/* figure out where we're going to be located on the screen */
			int baseX, baseY;
			int si;

			if (Background != null) {
				Bounds = Background.Bounds;
				Position = new PointF ((parent.Bounds.Width - Background.Bounds.Width) / 2,
								       parent.Bounds.Height - (parent.Bounds.Height - Background.Bounds.Height) / 2 - Background.Bounds.Height);
			}
			else {
				Position = new PointF (Elements[0].X1, parent.Bounds.Height - Elements[0].Y1);
				for (int i = 1; i < Elements.Count; i ++) {
					var ui_el = Elements[i];
					ui_el.X1 -= Elements[0].X1;
					ui_el.Y1 -= Elements[0].Y1;
				}
				
				Elements[0].X1 = 0;
				Elements[0].Y1 = 0;	
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
