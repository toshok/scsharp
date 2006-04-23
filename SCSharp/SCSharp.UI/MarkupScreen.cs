using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public abstract class MarkupScreen : UIScreen
	{
		Fnt fnt;
		byte[] pal;

		public MarkupScreen (Mpq mpq) : base (mpq)
		{
			pages = new List<MarkupPage> ();
		}

		enum PageLocation {
			Center,
			Top,
			Bottom,
			Left,
			Right
		}

		class MarkupPage
		{
			PageLocation location;
			List<string> lines;
			List<Surface> lineSurfaces;
			Surface newBackground;
			Fnt fnt;
			byte[] pal;

			public MarkupPage (PageLocation loc, Fnt font, byte[] palette)
			{
				location = loc;
				lines = new List<string> ();
				fnt = font;
				pal = palette;
			}

			public MarkupPage (Stream background)
			{
				newBackground = GuiUtil.SurfaceFromStream (background, 254, 0);
			}

			public void AddLine (string line)
			{
				lines.Add (line);
			}

			public void Layout ()
			{
				lineSurfaces = new List<Surface> ();
				foreach (string l in lines) {
					if (l.Trim() == "")
						lineSurfaces.Add (null);
					else
						lineSurfaces.Add (GuiUtil.ComposeText (l, fnt, pal));
				}
			}

			public Surface Background {
				get { return newBackground; } 
			}

			public bool HasText {
				get { return lines != null && lines.Count > 0; }
			}

			public void Paint (Surface surf)
			{
				int y;

				switch (location) {
				case PageLocation.Top:
					y = 10;
					foreach (Surface s in lineSurfaces) {
						if (s != null)
							surf.Blit (s, new Point ((surf.Width - s.Width) / 2, y));
						y += fnt.LineSize;
					}
					break;
				case PageLocation.Bottom:
					y = surf.Height - 10 - fnt.LineSize * lines.Count;
					foreach (Surface s in lineSurfaces) {
						if (s != null)
							surf.Blit (s, new Point ((surf.Width - s.Width) / 2, y));
						y += fnt.LineSize;
					}
					break;
				case PageLocation.Left:
					y = (surf.Height - fnt.LineSize * lines.Count) / 2;
					foreach (Surface s in lineSurfaces) {
						if (s != null)
							surf.Blit (s, new Point (60, y));
						y += fnt.LineSize;
					}
					break;
				case PageLocation.Right:
					y = (surf.Height - fnt.LineSize * lines.Count) / 2;
					foreach (Surface s in lineSurfaces) {
						if (s != null)
							surf.Blit (s, new Point (surf.Width - s.Width - 60, y));
						y += fnt.LineSize;
					}
					break;
				case PageLocation.Center:
					y = (surf.Height - fnt.LineSize * lines.Count) / 2;
					foreach (Surface s in lineSurfaces) {
						if (s != null)
							surf.Blit (s, new Point ((surf.Width - s.Width) / 2, y));
						y += fnt.LineSize;
					}
					break;
				}
			}
		}

		List<MarkupPage> pages;

		protected void AddMarkup (Stream s)
		{
			string l;
			MarkupPage currentPage = null;

			StreamReader sr = new StreamReader (s);

			while ((l = sr.ReadLine ()) != null) {
				if (l.StartsWith ("</")) {
					if (l.StartsWith ("</PAGE>")) {
						currentPage.Layout ();
						pages.Add (currentPage);
						currentPage = null;
					}
					else if (l.StartsWith ("</SCREENCENTER>")) {
						currentPage = new MarkupPage (PageLocation.Center, fnt, pal);
					}
					else if (l.StartsWith ("</SCREENLEFT>")) {
						currentPage = new MarkupPage (PageLocation.Left, fnt, pal);
					}
					else if (l.StartsWith ("</SCREENRIGHT>")) {
						currentPage = new MarkupPage (PageLocation.Right, fnt, pal);
					}
					else if (l.StartsWith ("</SCREENTOP>")) {
						currentPage = new MarkupPage (PageLocation.Top, fnt, pal);
					}
					else if (l.StartsWith ("</SCREENBOTTOM>")) {
						currentPage = new MarkupPage (PageLocation.Bottom, fnt, pal);
					}
					else if (l.StartsWith ("</BACKGROUND ")) {
						string bg = l.Substring ("</BACKGROUND ".Length);
						bg = bg.Substring (0, bg.Length - 1);
						pages.Add (new MarkupPage ((Stream)mpq.GetResource (bg)));
					}
					/* skip everything else */
#if false
					else if (l.StartsWith ("</FONTCOLOR")
						 || l.StartsWith ("</COMMENT")
						 || l.StartsWith ("</FADESPEED")) {
					}
#endif

				}
				else if (currentPage != null)
					currentPage.AddLine(l);
			}
		}

		protected override void ResourceLoader ()
		{
			Console.WriteLine ("loading font palette");
			Stream palStream = (Stream)mpq.GetResource ("glue\\Palmm\\tFont.pcx");
			Pcx pcx = new Pcx ();
			pcx.ReadFromStream (palStream, -1, -1);
				
			pal = pcx.RgbData;

			Console.WriteLine ("loading font");
			fnt = GuiUtil.GetFonts(mpq)[3];

			/* set things up so we're ready to go */
			millisDelay = 4000;
			pageEnumerator = pages.GetEnumerator();
			AdvanceToNextPage ();
		}

		// painting
		Surface currentBackground;
		IEnumerator<MarkupPage> pageEnumerator;

		int millisDelay;
		int totalElapsed;

		Painter p;

		public override void AddToPainter (Painter painter)
		{
			p = painter;
			/* we add a special entry to the background
			 * painter layer just so we can find out when
			 * we're first painted */
			painter.Add (Layer.Background, FirstPainted);

			painter.Add (Layer.Background, PaintBackground);
			painter.Add (Layer.UI, PaintMarkup);
		}

		public override void RemoveFromPainter (Painter painter)
		{
			painter.Remove (Layer.Background, FirstPainted);

			painter.Remove (Layer.Background, PaintBackground);
			painter.Remove (Layer.UI, PaintMarkup);
		}

		void FirstPainted (Surface surf, DateTime now)
		{
			p.Remove (Layer.Background, FirstPainted);
			p = null;

			/* set ourselves up to invalidate at a regular interval*/
                        Events.Tick += FlipPage;
		}

		void PaintBackground (Surface surf, DateTime now)
		{
			if (currentBackground != null)
				surf.Blit (currentBackground);
		}

		void PaintMarkup (Surface surf, DateTime now)
		{
			pageEnumerator.Current.Paint (surf);
		}

		void FlipPage (object sender, TickEventArgs e)
		{
			totalElapsed += e.TicksElapsed;

			if (totalElapsed < millisDelay)
				return;

			totalElapsed = 0;
			AdvanceToNextPage ();
		}

		public override void KeyboardDown (KeyboardEventArgs args)
		{
			switch (args.Key)
			{
			case Key.Escape:
				Events.Tick -= FlipPage;
				MarkupFinished ();
				break;
			case Key.Space:
			case Key.Return:
				totalElapsed = 0;
				AdvanceToNextPage ();
				break;
			}
		}

		public override void KeyboardUp (KeyboardEventArgs args)
		{
		}

		void AdvanceToNextPage ()
		{
			while (pageEnumerator.MoveNext ()) {
				if (pageEnumerator.Current.Background != null)
					currentBackground = pageEnumerator.Current.Background;
				if (pageEnumerator.Current.HasText)
					return;
			}

                        Events.Tick -= FlipPage;
			MarkupFinished ();
		}

		protected abstract void MarkupFinished ();
	}
}
