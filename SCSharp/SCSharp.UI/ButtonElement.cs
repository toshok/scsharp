using System;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
{
	public class ButtonElement : UIElement
	{
		public ButtonElement (UIScreen screen, BinElement el, byte[] palette)
			: base (screen, el, palette)
		{
		}

		protected override Surface CreateSurface ()
		{
			Surface surf = new Surface (Width, Height);

			surf.TransparentColor = Color.Black; /* XXX */

			Surface text_surf = GuiUtil.ComposeText (Text, Font, Palette, -1, -1,
								 Sensitive ? 4 : 24);
			
			surf.Blit (text_surf, new Point ((surf.Width - text_surf.Width) / 2,
							 (surf.Height - text_surf.Height) / 2));

			return surf;
		}

		public override void MouseButtonDown (MouseButtonEventArgs args)
		{
		}

		public override void MouseButtonUp (MouseButtonEventArgs args)
		{
			if (PointInside (args.X, args.Y))
				OnActivate ();
		}

		public override void MouseOver ()
		{
			if ((Flags & ElementFlags.RespondToMouse) == ElementFlags.RespondToMouse) {
				/* highlight the text */
				GuiUtil.PlaySound (Mpq, Builtins.MouseoverWav);
			}
		}
	}

}
