using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using SdlDotNet;

namespace SCSharp
{
	/* this should probably subclass from UIElement instead...  look into that */
	public abstract class UIDialog : UIScreen
	{
		UIScreen parent;

		protected UIDialog (UIScreen parent, Mpq mpq, string prefix, string binFile)
			: base (mpq, prefix, binFile)
		{
			this.parent = parent;
			background_translucent = 254;
			background_transparent = 0;
		}

		public override void AddToPainter (Painter painter)
		{
			this.painter = painter;

			Console.WriteLine ("adding UIDialog to the painter");
			if (Background != null)
				painter.Add (Layer.DialogBackground, BackgroundPainter);

			if (UIPainter != null)
				painter.Add (Layer.DialogUI, UIPainter.Paint);
		}


		public override void RemoveFromPainter (Painter painter)
		{
			if (Background != null)
				painter.Remove (Layer.DialogBackground, BackgroundPainter);

			if (UIPainter != null)
				painter.Remove (Layer.DialogUI, UIPainter.Paint);

			this.painter = null;
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			/* figure out where we're going to be located on the screen */
			int baseX = (Game.SCREEN_RES_X - Background.Width) / 2;
			int baseY = (Game.SCREEN_RES_Y - Background.Height) / 2;

			/* and add that offset to all our elements */
			foreach (UIElement el in Elements) {
				el.X1 += (ushort)baseX;
				el.Y1 += (ushort)baseY;
			}
		}

		public UIScreen Parent {
			get { return parent; }
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
