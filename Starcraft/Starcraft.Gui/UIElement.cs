
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections.Generic;

using SdlDotNet;

namespace Starcraft {

	public class UIElement
	{
		BinElement el;
		Surface surface;
		Mpq mpq;
		byte[] palette;
		bool sensitive;
		Fnt fnt;

		public UIElement (Mpq mpq, BinElement el, byte[] palette)
		{
			this.mpq = mpq;
			this.el = el;
			this.palette = palette;
			this.sensitive = true;
		}

		public Mpq Mpq {
			get { return mpq; }
		}

		public string Text {
			get { return el.text; }
			set { el.text = value;
			      ClearSurface (); }
		}

		public bool Sensitive {
			get { return sensitive; }
			set { sensitive = value;
			      ClearSurface (); }
		}

		public byte[] Palette {
			get { return palette; }
			set { palette = value;
			      ClearSurface (); }
		}

		public Surface Surface {
			get {
				if (surface == null)
					surface = CreateSurface ();

				return surface;
			}
		}

		public Fnt Font {
			get { 
				if (fnt == null) {
					fnt = GuiUtil.GetMediumFont (mpq); /* XXX */
					if ((Flags & ElementFlags.FontSmall) == ElementFlags.FontSmall)
						fnt = GuiUtil.GetSmallFont (mpq);
					else if ((Flags & ElementFlags.FontMedium) == ElementFlags.FontMedium)
						fnt = GuiUtil.GetMediumFont (mpq);
					else if ((Flags & ElementFlags.FontLarge) == ElementFlags.FontLarge)
						fnt = GuiUtil.GetLargeFont (mpq);
				}
				return fnt;
			}
			set { fnt = value;
			      ClearSurface (); }
		}

		public ElementFlags Flags { get { return el.flags; } }
		public ElementType Type { get { return el.type; } }

		public ushort X1 {
			get { return el.x1; }
			set { el.x1 = value; }
		}
		public ushort Y1 {
			get { return el.y1; }
			set { el.y1 = value; }
		}
		public ushort Width { get { return el.width; } }
		public ushort Height { get { return el.height; } }

		public Key Hotkey { get { return (Key)el.hotkey; } }

		public event ElementEvent Activate;

		public void OnActivate ()
		{
			if (Activate != null)
				Activate ();
		}

		protected void ClearSurface ()
		{
			surface = null;
		}

		protected virtual Surface CreateSurface ()
		{
			switch (Type) {
			case ElementType.DefaultButton:
			case ElementType.Button:
			case ElementType.ButtonWithoutBorder:
			case ElementType.LabelLeftAlign:
			case ElementType.LabelCenterAlign:
			case ElementType.LabelRightAlign:
				return GuiUtil.ComposeText (Text, Font, palette, Width, Height,
							    sensitive ? 4 : 24);
			default:
				return null;
			}
		}

		public void Paint (Surface surf, DateTime now)
		{
			if (Surface == null)
				return;

			int x, y;
			x = X1;
			y = Y1;

			if (Type == ElementType.LabelRightAlign)
				x += Width - surface.Width;
			else if (Type == ElementType.LabelCenterAlign)
				x += (Width - surface.Width) / 2;

			surf.Blit (surface, new Point (x, y));
		}
		
	}

	public delegate void ElementEvent ();
}
