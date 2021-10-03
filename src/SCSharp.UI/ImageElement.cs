//
// SCSharp.UI.ImageElement
//
// Authors:
//	Chris Toshok (toshok@gmail.com)
//
// Copyright 2006-2010 Chris Toshok
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
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

using System.Drawing;

namespace SCSharp.UI
{
	public class ImageElement : UIElement
	{
		int translucent_index;
		Pcx pcx;

		public ImageElement (UIScreen screen, BinElement el, byte[] palette, int translucent_index)
			: base (screen, el, palette)
		{
			this.translucent_index = translucent_index;
		}

		public ImageElement (UIScreen screen, ushort x1, ushort y1, ushort width, ushort height, int translucent_index)
			: base (screen, x1, y1, width, height)
		{
			this.translucent_index = translucent_index;
		}

		protected override Surface CreateSurface ()
		{
			return GuiUtil.SurfaceFromPcx (Pcx);
		}

		public Pcx Pcx {
			get {
				if (pcx == null) {
					pcx = new Pcx ();
					pcx.ReadFromStream ((Stream)Mpq.GetResource (Text), translucent_index, 0);
				}
				return pcx;
			}
		}

		public override ElementType Type {
			get { return ElementType.Image; }
		}
	}

}
