//
// SCSharpMac.UI.Cinematic
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
using System.Threading;

using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
{
	public class Cinematic : UIScreen
	{
		SmackerPlayer player;
		string resourcePath;

		public Cinematic (Mpq mpq, string resourcePath)
			: base (mpq, null, null)
		{
			this.resourcePath = resourcePath;
		}

		public override void AddToPainter ()
		{
			base.AddToPainter ();

			player = new SmackerPlayer ((Stream)mpq.GetResource (resourcePath));

			player.Finished += PlayerFinished;
			player.Play ();

			if (player.Width != 800/*Painter.Width*/
		    	|| player.Height != 600/*Painter.Height*/) {

				float horiz_zoom = (float)800/*Painter.Width*/ / player.Width;
				float vert_zoom = (float)600/*Painter.Height*/ / player.Height;
				float zoom;

				if (horiz_zoom < vert_zoom)
					zoom = horiz_zoom;
				else
					zoom = vert_zoom;
				
				AffineTransform = CGAffineTransform.MakeScale (zoom, zoom);
			}

			player.Layer.AnchorPoint = new PointF (0, 0);
			AddSublayer (player.Layer);
		}

		public override void RemoveFromPainter ()
		{
			player.Stop ();
			player = null;

			base.RemoveFromPainter ();
		}

		public event PlayerEvent Finished;

		void PlayerFinished ()
		{
			var h = Finished;
			if (h != null)
				h ();
		}

#if notyet
		public override void KeyboardDown (KeyboardEventArgs args)
		{
			if (args.Key == Key.Escape
			    || args.Key == Key.Return
			    || args.Key == Key.Space) {
				player.Stop ();
				PlayerFinished ();
			}
		}
#endif
	}
}
