using System;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class ImageElement : UIElement
	{
		public ImageElement (Mpq mpq, BinElement el, byte[] palette)
			: base (mpq, el, palette)
		{
		}

		protected override Surface CreateSurface ()
		{
			Surface surface = GuiUtil.SurfaceFromStream ((Stream)Mpq.GetResource (Text),
						     (Flags & ElementFlags.ApplyTranslucency) == ElementFlags.ApplyTranslucency);
			surface.TransparentColor = Color.Black; /* XXX */

			return surface;
		}
	}

}
