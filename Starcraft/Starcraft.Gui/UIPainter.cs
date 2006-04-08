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
				e.Paint (surf, now);
#if SHOW_ELEMENT_BORDERS
				surf.DrawBox (new Rectangle (new Point (x,y), new Size (e.Width - 1, e.Height - 1)), Color.Green);
#endif
			}
		}
	}

}
