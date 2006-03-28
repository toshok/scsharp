//#define SHOW_ELEMENT_BORDERS

using System;
using System.Collections.Generic;
using System.IO;

using SdlDotNet;
using System.Drawing;

namespace Starcraft {

	public class UIPainter
	{
		Bin ui;
		bool resolved;

		public UIPainter (Bin ui, Mpq mpq)
		{
			this.ui = ui;

			ResolveUI (mpq);
		}

		public void Paint (Surface surf, DateTime now)
		{
			foreach (UIElement e in ui.Elements) {
				Surface elementSurface = e.resolvedData as Surface;

				if (elementSurface == null)
					continue;

				int x, y;
				x = e.x1;
				y = e.y1;

				if (e.type == UIElementType.LabelRightAlign)
					x += e.width - elementSurface.Width;
				else if (e.type == UIElementType.LabelCenterAlign)
					x += (e.width - elementSurface.Width) / 2;

				surf.Blit (elementSurface, new Point (x, y));
				
#if SHOW_ELEMENT_BORDERS
				surf.DrawBox (new Rectangle (new Point (e.x1,e.y1), new Size (e.width - 1, e.height - 1)), Color.Green);
#endif
			}
		}

		void ResolveUI (Mpq mpq)
		{
			foreach (UIElement e in ui.Elements) {
				Surface s = null;

				switch (e.type) {
				case UIElementType.Image:
					s = GuiUtil.SurfaceFromStream ((Stream)mpq.GetResource (e.text));
					break;
				case UIElementType.DefaultButton:
				case UIElementType.Button:
				case UIElementType.ButtonWithoutBorder:
				case UIElementType.LabelLeftAlign:
				case UIElementType.LabelCenterAlign:
				case UIElementType.LabelRightAlign:
					int fontsize = 0;
					if ((e.flags & UIElementFlags.FontSmall) == UIElementFlags.FontSmall)
						fontsize = 0;
					else if ((e.flags & UIElementFlags.FontMedium) == UIElementFlags.FontMedium)
						fontsize = 1;
					else if ((e.flags & UIElementFlags.FontLarge) == UIElementFlags.FontLarge)
						fontsize = 2;
					s = GuiUtil.ComposeText (e.text, fontsize, Color.FromArgb (0, 0, 175, 0));
					break;
				default:
					break;
				}

				/* handle alpha here... */
				e.resolvedData = s;
			}
		}
	}

}
