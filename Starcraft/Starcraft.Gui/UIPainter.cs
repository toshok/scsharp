using System;
using System.Collections.Generic;
using Gtk;
using Gdk;

namespace Starcraft {

	public class UIPainter
	{
		BIN ui;

		public UIPainter (BIN ui)
		{
			this.ui = ui;
		}

		public void Paint (Gdk.Pixbuf pb, DateTime now)
		{
			foreach (UIElement e in ui.Elements) {
				switch (e.type) {
				case UIElementType.DialogBox:
					/* nothing to do here */
					break;
				case UIElementType.DefaultButton:
					Game.Instance.Painter.DrawText (e.x1, e.y1, e.text);
					break;
				case UIElementType.Button:
					Game.Instance.Painter.DrawText (e.x1, e.y1, e.text);
					break;
				case UIElementType.OptionButton:
					Game.Instance.Painter.DrawText (e.x1, e.y1, e.text);
					break;
				case UIElementType.CheckBox:
					Game.Instance.Painter.DrawText (e.x1, e.y1, e.text);
					break;
				case UIElementType.Image:
					Gdk.Pixbuf image = e.resolvedData as Gdk.Pixbuf;
					if (image != null) {
						int alpha;
						if ((e.flags & UIElementFlags.ApplyTranslucency) == UIElementFlags.ApplyTranslucency)
							alpha = 0x99;
						else
							alpha = 0xff;

						image.Composite (pb, e.x1, e.y1, image.Width, image.Height,
								 e.x1, e.y1, 1, 1, InterpType.Nearest, alpha);
					}
					break;
				case UIElementType.Slider:
					break;
				case UIElementType.TextBox:
					break;
				case UIElementType.LabelLeftAlign:
					Game.Instance.Painter.DrawText (e.x1, e.y1, e.text);
					break;
				case UIElementType.LabelCenterAlign:
					Game.Instance.Painter.DrawText (e.x1, e.y1, e.text);
					break;
				case UIElementType.LabelRightAlign:
					Game.Instance.Painter.DrawText (e.x1, e.y1, e.text);
					break;
				case UIElementType.ListBox:
					break;
				case UIElementType.ComboBox:
					break;
				case UIElementType.ButtonWithoutBorder:
					Game.Instance.Painter.DrawText (e.x1, e.y1, e.text);
					break;
				}
			}
		}
	}

}
