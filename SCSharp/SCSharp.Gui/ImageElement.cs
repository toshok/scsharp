using System;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
{
	public class ImageElement : UIElement
	{
		public ImageElement (Mpq mpq, BinElement el, byte[] palette)
			: base (mpq, el, palette)
		{
		}

		protected override Surface CreateSurface ()
		{
			Surface surface;

			if ((Flags & ElementFlags.ApplyTranslucency) == ElementFlags.ApplyTranslucency)
				surface = GuiUtil.SurfaceFromStream ((Stream)Mpq.GetResource (Text),
								     254, 0);
			else
				surface = GuiUtil.SurfaceFromStream ((Stream)Mpq.GetResource (Text));

			//			surface.TransparentColor = Color.Black; /* XXX */

			return surface;
		}
	}

}
