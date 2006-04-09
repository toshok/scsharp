using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using SdlDotNet;

namespace Starcraft
{
	/* this should probably subclass from UIElement instead...  look into that */
	public abstract class UIDialog : UIScreen
	{
		protected UIDialog (Mpq mpq, string prefix, string binFile) : base (mpq, prefix, binFile)
		{
		}

		public override void AddToPainter (Painter painter)
		{
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
	}

	public delegate void DialogEvent ();
}
