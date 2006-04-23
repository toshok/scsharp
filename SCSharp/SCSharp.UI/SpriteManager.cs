
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
		static Painter painter;

		static Mpq our_mpq;

		public static int X;
		public static int Y;

		public static Sprite CreateSprite (Mpq mpq, int sprite_number, byte[] palette, int x, int y)
		{
			our_mpq = mpq;
			return CreateSprite (sprite_number, palette, x, y);
		}

		public static Sprite CreateSprite (int sprite_number, byte[] palette, int x, int y)
		{
			Sprite sprite = new Sprite (our_mpq, sprite_number, palette, x, y);
			sprites.Add (sprite);
			if (painter != null)
				sprite.AddToPainter (painter);

			return sprite;
		}

		public static void RemoveSprite (Sprite sprite)
		{
			if (painter != null)
				sprite.RemoveFromPainter (painter);
			sprites.Remove (sprite);
		}

		static void SpriteManagerPainterTick (Surface surf, DateTime now)
		{
			IEnumerator<Sprite> e = sprites.GetEnumerator();
			
			while (e.MoveNext ()) {
				if (e.Current.Tick (surf, now) == false) {
					Console.WriteLine ("removing sprite!!!!");
					RemoveSprite (e.Current);
				}
			}
		}

		public static void AddToPainter (Painter p)
		{
			p.Add (Layer.Background, SpriteManagerPainterTick);

			painter = p;
			foreach (Sprite s in sprites)
				s.AddToPainter (painter);
		}

		public static void RemoveFromPainter (Painter p)
		{
			p.Remove (Layer.Background, SpriteManagerPainterTick);

			foreach (Sprite s in sprites)
				s.RemoveFromPainter (p);
			painter = null;
		}

		public static void SetUpperLeft (int x, int y)
		{
			X = x;
			Y = y;
		}
	}

}
