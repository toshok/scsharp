//#define SHOW_ELEMENT_BORDERS

using System;
using System.Collections.Generic;
using System.IO;

using SdlDotNet;
using System.Drawing;

namespace Starcraft {

	public class UIPainter
	{
		List<UIElement> elements;
		bool resolved;

		public UIPainter (List<UIElement> elements)
		{
			this.elements = elements;
		}

		public void Paint (Surface surf, DateTime now)
		{
			foreach (UIElement e in elements) {
				Surface elementSurface = e.Surface;

				if (elementSurface == null)
					continue;

				int x, y;
				x = e.X1;
				y = e.Y1;

				if (e.Type == ElementType.LabelRightAlign)
					x += e.Width - elementSurface.Width;
				else if (e.Type == ElementType.LabelCenterAlign)
					x += (e.Width - elementSurface.Width) / 2;

				surf.Blit (elementSurface, new Point (x, y));
				
#if SHOW_ELEMENT_BORDERS
				surf.DrawBox (new Rectangle (new Point (x,y), new Size (e.Width - 1, e.Height - 1)), Color.Green);
#endif
			}
		}
	}

}
