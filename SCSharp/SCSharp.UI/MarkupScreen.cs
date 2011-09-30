//
// SCSharp.UI.MarkupScreen
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

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
			Right,
			LowerLeft
		}

		class MarkupPage
		{
			const int X_OFFSET = 60;
			const int Y_OFFSET = 10;

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
						lineSurfaces.Add (GuiUtil.ComposeText (l, fnt, pal, Painter.SCREEN_RES_X - X_OFFSET * 2, -1, 4));
				}
			}

			public Surface Background {
				get { return newBackground; } 
			}

			public bool HasText {
				get { return lines != null && lines.Count > 0; }
			}

			public void Paint ()
			{
				int y;

				switch (location) {
				case PageLocation.Top:
					y = Y_OFFSET;
					foreach (Surface s in lineSurfaces) {
						if (s != null) {
							Painter.Blit (s, new Point ((Painter.Width - s.Width) / 2, y));
							y += s.Height;
						}
						else 
							y += fnt.LineSize;
					}
					break;
				case PageLocation.Bottom:
					y = Painter.Height - Y_OFFSET - fnt.LineSize * lines.Count;
					foreach (Surface s in lineSurfaces) {
						if (s != null) {
							Painter.Blit (s, new Point ((Painter.Width - s.Width) / 2, y));
							y += s.Height;
						}
						else
							y += fnt.LineSize;
					}
					break;
				case PageLocation.Left:
					y = (Painter.Height - fnt.LineSize * lines.Count) / 2;
					foreach (Surface s in lineSurfaces) {
						if (s != null) {
							Painter.Blit (s, new Point (X_OFFSET, y));
							y += s.Height;
						}
						else 
							y += fnt.LineSize;
					}
					break;
				case PageLocation.LowerLeft:
					y = Painter.Height - Y_OFFSET - fnt.LineSize * lines.Count;
					foreach (Surface s in lineSurfaces) {
						if (s != null) {
							Painter.Blit (s, new Point (X_OFFSET, y));
							y += s.Height;
						}
						else
							y += fnt.LineSize;
					}
					break;
				case PageLocation.Right:
					y = (Painter.Height - fnt.LineSize * lines.Count) / 2;
					foreach (Surface s in lineSurfaces) {
						if (s != null) {
							Painter.Blit (s, new Point (Painter.Width - s.Width - X_OFFSET, y));
							y += s.Height;
						}
						else
							y += fnt.LineSize;
					}
					break;
				case PageLocation.Center:
					y = (Painter.Height - fnt.LineSize * lines.Count) / 2;
					foreach (Surface s in lineSurfaces) {
						if (s != null) {
							Painter.Blit (s, new Point ((Painter.Width - s.Width) / 2, y));
							y += s.Height;
						}
						else
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
					else if (l.StartsWith ("</SCREENLOWERLEFT>")) {
						currentPage = new MarkupPage (PageLocation.LowerLeft, fnt, pal);
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

			Console.WriteLine ("loading markup");
			LoadMarkup ();

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

		public override void AddToPainter ()
		{
			base.AddToPainter ();
			Painter.Add (Layer.Background, PaintBackground);
			Painter.Add (Layer.UI, PaintMarkup);
		}

		public override void RemoveFromPainter ()
		{
			base.RemoveFromPainter ();
			Painter.Remove (Layer.Background, PaintBackground);
			Painter.Remove (Layer.UI, PaintMarkup);
		}

		protected override void FirstPaint (object sender, EventArgs args)
		{
			base.FirstPaint (sender, args);

			/* set ourselves up to invalidate at a regular interval*/
                        Events.Tick += FlipPage;
		}

		void PaintBackground (DateTime now)
		{
			if (currentBackground != null)
				Painter.Blit (currentBackground);
		}

		void PaintMarkup (DateTime now)
		{
			pageEnumerator.Current.Paint ();
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
			Painter.Invalidate ();
			while (pageEnumerator.MoveNext ()) {
				if (pageEnumerator.Current.Background != null)
					currentBackground = pageEnumerator.Current.Background;
				if (pageEnumerator.Current.HasText)
					return;
			}

			Console.WriteLine ("finished!");
                        Events.Tick -= FlipPage;
			MarkupFinished ();
		}

		protected abstract void LoadMarkup ();
		protected abstract void MarkupFinished ();
	}
}
