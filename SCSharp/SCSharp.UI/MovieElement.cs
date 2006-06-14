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
		public MovieElement (UIScreen screen, BinElement el, byte[] palette, string resource)
			: base (screen, el, palette)
		{
			resource_path = resource;

			Sensitive = false;

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

		public Size MovieSize {
			get { return new Size (player.Width, player.Height); }
		}

		public void Dim (byte dimness)
		{
			dim = dimness;
			Invalidate ();
		}

		SmackerPlayer player;
		string resource_path;
		byte dim = 0;

		protected override Surface CreateSurface ()
		{
			if (player.Surface == null)
				return null;

			Surface surf;
			if (dim == 0) {
				surf = player.Surface;
			}
			else {
				surf = new Surface (player.Surface.Size);
				surf.Alpha = dim;
				surf.AlphaBlending = true;
				surf.Blit (player.Surface);
			}

			return surf;
		}

		void PlayerFrameReady ()
		{
			Invalidate ();
		}
	}

}
