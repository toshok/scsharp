using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using SdlDotNet;

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

			dimScreenSurface = new Surface (Game.SCREEN_RES_X, Game.SCREEN_RES_Y);
			dimScreenSurface.Alpha = 100;
			dimScreenSurface.AlphaBlending = true;
		}

		public override void AddToPainter (Painter painter)
		{
			this.painter = painter;

			if (Background != null)
				painter.Add (Layer.DialogBackground, BackgroundPainter);

			if (UIPainter != null)
				painter.Add (Layer.DialogUI, UIPainter.Paint);

			if (dimScreen)
				painter.Add (Layer.DialogDimScreenHack, DimScreenPainter);
		}


		public override void RemoveFromPainter (Painter painter)
		{
			if (Background != null)
				painter.Remove (Layer.DialogBackground, BackgroundPainter);

			if (UIPainter != null)
				painter.Remove (Layer.DialogUI, UIPainter.Paint);

			if (dimScreen)
				painter.Remove (Layer.DialogDimScreenHack, DimScreenPainter);

			this.painter = null;
		}

		void DimScreenPainter (Surface surf, DateTime dt)
		{
			surf.Blit (dimScreenSurface);
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			/* figure out where we're going to be located on the screen */
			int baseX, baseY;
			int si;

			if (Background != null) {
				baseX = (Game.SCREEN_RES_X - Background.Width) / 2;
				baseY = (Game.SCREEN_RES_Y - Background.Height) / 2;
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

		public override Painter Painter {
			get { return painter; }
		}

		public UIScreen Parent {
			get { return parent; }
		}

		public bool DimScreen {
			get { return dimScreen; }
			set { dimScreen = value; }
		}

		Painter rememberedPainter;
		public override void ShowDialog (UIDialog dialog)
		{
			Console.WriteLine ("showing {0}", dialog);

			if (this.dialog != null)
				throw new Exception ("only one active dialog is allowed");
			this.dialog = dialog;

			dialog.Load ();
			dialog.Ready += delegate () { 
				dialog.AddToPainter (painter);
				rememberedPainter = painter;
				RemoveFromPainter (painter);
			};
		}

		public override void DismissDialog ()
		{
			if (dialog == null)
				return;

			dialog.RemoveFromPainter (rememberedPainter);
			dialog = null;
			AddToPainter (rememberedPainter);
		}
	}

	public delegate void DialogEvent ();
}
