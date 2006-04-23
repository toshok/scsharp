using System;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class ImageElement : UIElement
	{
		public ImageElement (UIScreen screen, BinElement el, byte[] palette)
			: base (screen, el, palette)
		{
		}

		protected override Surface CreateSurface ()
		{
			Surface surface;

			if ((Flags & ElementFlags.Translucent) == ElementFlags.Translucent)
				surface = GuiUtil.SurfaceFromStream ((Stream)Mpq.GetResource (Text),
								     254, 0);
			else
				surface = GuiUtil.SurfaceFromStream ((Stream)Mpq.GetResource (Text));

			//			surface.TransparentColor = Color.Black; /* XXX */

			return surface;
		}
	}

}
