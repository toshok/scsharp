//
// SCSharp.UI.MovieElement
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
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class MovieElement : UIElement
	{
		public MovieElement (UIScreen screen, BinElement el, byte[] palette, string resource, bool scale)
			: base (screen, el, palette)
		{
			resource_path = resource;
			this.scale = scale;

			player = new SmackerPlayer ((Stream)Mpq.GetResource (resource_path), 1);
			player.FrameReady += PlayerFrameReady;
		}

		public void Play ()
		{
			player.Play ();
		}

		public void Stop ()
		{
			player.Stop ();
		}

		SmackerPlayer player;
		string resource_path;
		bool scale;

		Surface surf;

		protected override Surface CreateSurface ()
		{
			if (surf != null)
				surf.Dispose ();

			if (player.Surface == null)
				return null;

			surf = new Surface (player.Surface);

			if (scale &&
			    player.Width != Width
			    || player.Height != Height) {
				double horiz_zoom = (double)Width / player.Width;
				double vert_zoom = (double)Height / player.Height;
				double zoom;

				if (horiz_zoom < vert_zoom)
					zoom = horiz_zoom;
				else
					zoom = vert_zoom;

				surf.Scale (zoom);
			}

			return surf;
		}

		void PlayerFrameReady ()
		{
			Invalidate ();
		}
	}

}
