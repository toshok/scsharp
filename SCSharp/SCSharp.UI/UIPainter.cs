//
// SCSharp.UI.UIPainter
//
// Authors:
//	Chris Toshok (toshok@hungry.com)
//
// (C) 2006 The Hungry Programmers (http://www.hungry.com/)
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class UIPainter
	{
		static bool showBorders;

		static UIPainter()
		{
#if DEBUG
			string sb = ConfigurationManager.AppSettings ["ShowElementBorders"];
			if (sb != null) {
				showBorders = Boolean.Parse (sb);
			}
#endif
		}

		List<UIElement> elements;

		public UIPainter (List<UIElement> elements)
		{
			this.elements = elements;
		}

		public void Paint (DateTime now)
		{
			for (int i = 0; i < elements.Count; i ++) {
				UIElement e = elements[i];

				if (e == null)
					continue;

				e.Paint (now);

				if (showBorders) {
					Painter.DrawBox (new Rectangle (new Point (e.X1,e.Y1), new Size (e.Width - 1, e.Height - 1)),
							 e.Visible ? Color.Green : Color.Yellow);
					if (e.Text == "")
						e.Text = i.ToString();
				}
			}
		}
	}

}
