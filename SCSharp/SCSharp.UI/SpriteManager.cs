//
// SCSharp.UI.SpriteManager
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

using SdlDotNet;

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp.UI
{
	public static class SpriteManager
	{
		public static List<Sprite> sprites = new List<Sprite>();

		static Mpq our_mpq;

		public static int X;
		public static int Y;

		static bool in_tick;
		static List<Sprite> pendingAdds = new List<Sprite>();
		static List<Sprite> pendingRemoves = new List<Sprite>();
		
		public static Sprite CreateSprite (Mpq mpq, int sprite_number, byte[] palette, int x, int y)
		{
			our_mpq = mpq;
			return CreateSprite (sprite_number, palette, x, y);
		}

		public static Sprite CreateSprite (Sprite parentSprite, ushort images_number, byte[] palette)
		{
			Sprite sprite = new Sprite (parentSprite, images_number, palette);

			if (in_tick)
				pendingAdds.Add (sprite);
			else
				AddSprite (sprite);

			return sprite;
		}

		public static void AddSprite (Sprite sprite)
		{
			sprites.Add (sprite);
			sprite.AddToPainter ();
		}

		public static Sprite CreateSprite (int sprite_number, byte[] palette, int x, int y)
		{
			Sprite sprite = new Sprite (our_mpq, sprite_number, palette, x, y);

			if (in_tick)
				pendingAdds.Add (sprite);
			else
				AddSprite (sprite);

			return sprite;
		}

		public static void RemoveSprite (Sprite sprite)
		{
			if (in_tick)
				pendingRemoves.Add (sprite);
			else {
				sprite.RemoveFromPainter ();

				sprites.Remove (sprite);
			}
		}

		static void SpriteManagerPainterTick (DateTime now)
		{
			in_tick = true;

			IEnumerator<Sprite> e = sprites.GetEnumerator();
			
			while (e.MoveNext ()) {
				if (e.Current.Tick (now) == false) {
					Console.WriteLine ("removing sprite!!!!");
					RemoveSprite (e.Current);
				}
			}

			in_tick = false;

			foreach (Sprite s in pendingAdds)
				AddSprite (s);
			pendingAdds.Clear ();

			foreach (Sprite s in pendingRemoves)
				RemoveSprite (s);
			pendingRemoves.Clear ();
		}

		public static void AddToPainter ()
		{
			Painter.Instance.Add (Layer.Background, SpriteManagerPainterTick);

			foreach (Sprite s in sprites)
				s.AddToPainter ();
		}

		public static void RemoveFromPainter ()
		{
			Painter.Instance.Remove (Layer.Background, SpriteManagerPainterTick);

			foreach (Sprite s in sprites)
				s.RemoveFromPainter ();
		}

		public static void SetUpperLeft (int x, int y)
		{
			X = x;
			Y = y;
		}
	}

}
