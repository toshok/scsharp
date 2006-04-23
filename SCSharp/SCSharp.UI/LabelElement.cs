using System;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class LabelElement : UIElement
	{
		public LabelElement (UIScreen screen, BinElement el, byte[] palette)
			: base (screen, el, palette)
		{
		}

		protected override Surface CreateSurface ()
		{
			/* this is wrong */
			Surface surf = new Surface (Width, Height);

			Surface textSurf = GuiUtil.ComposeText (Text, Font, Palette, Width, Height,
								Sensitive ? 4 : 24);;

			int x = 0;
			if (Type == ElementType.LabelRightAlign)
				x += Width - textSurf.Width;
			else if (Type == ElementType.LabelCenterAlign)
				x += (Width - textSurf.Width) / 2;

			surf.Blit (textSurf, new Point (x, 0));

			surf.TransparentColor = Color.Black /* XXX */;
			return surf;
		}
	}

}
