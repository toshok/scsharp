
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

		public UIElement (Mpq mpq, BinElement el, byte[] palette)
		{
			this.mpq = mpq;
			this.el = el;
			this.palette = palette;
		}

		public string Text {
			get { return el.text; }
			set { el.text = value;
			      surface = null; }
		}

		public Surface Surface {
			get {
				if (surface == null)
					CreateSurface ();

				return surface;
			}
		}

		public ElementFlags Flags { get { return el.flags; } }
		public ElementType Type { get { return el.type; } }

		public ushort X1 { get { return el.x1; } }
		public ushort Y1 { get { return el.y1; } }
		public ushort Width { get { return el.width; } }
		public ushort Height { get { return el.height; } }

		public Key Hotkey { get { return (Key)el.hotkey; } }

		public event ElementEvent Activate;

		public void OnActivate ()
		{
			if (Activate != null)
				Activate ();
		}

		void CreateSurface ()
		{
			switch (Type) {
			case ElementType.Image:
				surface = GuiUtil.SurfaceFromStream ((Stream)mpq.GetResource (Text),
								     (Flags & ElementFlags.ApplyTranslucency) == ElementFlags.ApplyTranslucency);
				surface.TransparentColor = Color.Black; /* XXX */
				break;
			case ElementType.DefaultButton:
			case ElementType.Button:
			case ElementType.ButtonWithoutBorder:
			case ElementType.LabelLeftAlign:
			case ElementType.LabelCenterAlign:
			case ElementType.LabelRightAlign:
				Fnt fnt = null;
				if ((Flags & ElementFlags.FontSmall) == ElementFlags.FontSmall)
					fnt = GuiUtil.GetSmallFont (mpq);
				else if ((Flags & ElementFlags.FontMedium) == ElementFlags.FontMedium)
					fnt = GuiUtil.GetMediumFont (mpq);
				else if ((Flags & ElementFlags.FontLarge) == ElementFlags.FontLarge)
					fnt = GuiUtil.GetLargeFont (mpq);

				surface = GuiUtil.ComposeText (Text, fnt, palette, Width, Height);
				break;
			default:
				break;
			}
		}

		
	}

	public delegate void ElementEvent ();
}
