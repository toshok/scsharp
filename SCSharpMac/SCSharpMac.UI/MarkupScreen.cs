//
// SCSharpMac.UI.MarkupScreen
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

using MonoMac.AppKit;
using MonoMac.CoreAnimation;

using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
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
			List<CALayer> lineLayers;
			CALayer lineContainer;
			CALayer newBackground;
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
				newBackground = GuiUtil.LayerFromStream (background, 254, 0);
				newBackground.AnchorPoint = new PointF (0, 0);
				newBackground.Position = new PointF (0, 0);
			}

			public void AddLine (string line)
			{
				lines.Add (line);
			}

			public void CreateLineLayers ()
			{
				lineContainer = CALayer.Create ();
				lineContainer.AnchorPoint = new PointF (0, 0);
				lineContainer.Bounds = new RectangleF (0, 0, 640, 480);
				
				lineLayers = new List<CALayer> ();
				foreach (string l in lines) {
					CALayer layer;
					if (l.Trim() == "")
						layer = null;
					else
						layer = GuiUtil.ComposeText (l, fnt, pal, 640/*Painter.SCREEN_RES_X*/ - X_OFFSET * 2, -1, 4);
					lineLayers.Add (layer);
					if (layer != null)
						lineContainer.AddSublayer (layer);
				}
			}

			public CALayer Background {
				get { return newBackground; } 
			}
			
			public CALayer Lines {
				get { return lineContainer; }
			}
			
			public bool HasText {
				get { return lines != null && lines.Count > 0; }
			}

			public void Layout ()
			{
				float y;
				
				CreateLineLayers ();
				
				switch (location) {
				case PageLocation.Top:
					y = Y_OFFSET;
					foreach (CALayer l in lineLayers) {
						if (l != null) {
							l.Position = new PointF ((lineContainer.Bounds.Width - l.Bounds.Width) / 2, y);
							y += l.Bounds.Height;
						}
						else 
							y += fnt.LineSize;
					}
					break;
				case PageLocation.Bottom:
					y = lineContainer.Bounds.Height - Y_OFFSET - fnt.LineSize * lines.Count;
					foreach (CALayer l in lineLayers) {
						if (l != null) {
							l.Position = new PointF ((lineContainer.Bounds.Width - l.Bounds.Width) / 2, y);
							y += l.Bounds.Height;
						}
						else
							y += fnt.LineSize;
					}
					break;
				case PageLocation.Left:
					y = (lineContainer.Bounds.Height - fnt.LineSize * lines.Count) / 2;
					foreach (CALayer l in lineLayers) {
						if (l != null) {
							l.Position = new PointF (X_OFFSET, y);
							y += l.Bounds.Height;
						}
						else 
							y += fnt.LineSize;
					}
					break;
				case PageLocation.LowerLeft:
					y = lineContainer.Bounds.Height - Y_OFFSET - fnt.LineSize * lines.Count;
					foreach (CALayer l in lineLayers) {
						if (l != null) {
							l.Position = new PointF (X_OFFSET, y);
							y += l.Bounds.Height;
						}
						else
							y += fnt.LineSize;
					}
					break;
				case PageLocation.Right:
					y = (lineContainer.Bounds.Height - fnt.LineSize * lines.Count) / 2;
					foreach (CALayer l in lineLayers) {
						if (l != null) {
							l.Position = new PointF (lineContainer.Bounds.Width - l.Bounds.Width - X_OFFSET, y);
							y += l.Bounds.Height;
						}
						else
							y += fnt.LineSize;
					}
					break;
				case PageLocation.Center:
					y = (lineContainer.Bounds.Height - fnt.LineSize * lines.Count) / 2;
					foreach (CALayer l in lineLayers) {
						if (l != null) {
							l.Position = new PointF ((lineContainer.Bounds.Width - l.Bounds.Width) / 2, y);
							y += l.Bounds.Height;
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
				
			pal = pcx.RGBData;

			Console.WriteLine ("loading font");
			fnt = GuiUtil.GetFonts(mpq)[3];

			Console.WriteLine ("loading markup");
			LoadMarkup ();

			/* set things up so we're ready to go */
			millisDelay = 4000;
			pageEnumerator = pages.GetEnumerator();
			AdvanceToNextPage ();
		}
		
		CALayer currentBackground;
		CALayer currentLines;
		IEnumerator<MarkupPage> pageEnumerator;

		int millisDelay;
		long totalElapsed;

		public override void AddToPainter ()
		{
			base.AddToPainter ();
			
            Game.Instance.Tick += FlipPage;			
		}

		public override void RemoveFromPainter ()
		{
			Game.Instance.Tick -= FlipPage;
			base.RemoveFromPainter ();
		}

		void FlipPage (object sender, TickEventArgs e)
		{
			totalElapsed += e.MillisecondsElapsed;

			if (totalElapsed < millisDelay)
				return;

			totalElapsed = 0;
			AdvanceToNextPage ();
		}

#if notyet
		public override void KeyboardDown (NSEvent theEvent)
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

		public override void KeyboardUp (NSEvent theEvent)
		{
		}
#endif

		void AdvanceToNextPage ()
		{
			if (currentLines != null)
				currentLines.RemoveFromSuperLayer ();
			while (pageEnumerator.MoveNext ()) {
				if (pageEnumerator.Current.Background != null) {
					if (currentBackground != null)
						currentBackground.RemoveFromSuperLayer ();
					currentBackground = pageEnumerator.Current.Background;
					AddSublayer (currentBackground);
				}
				if (pageEnumerator.Current.HasText) {
					currentLines = pageEnumerator.Current.Lines;
					AddSublayer (currentLines);
					return;
				}
			}

			Console.WriteLine ("finished!");
            Game.Instance.Tick -= FlipPage;
			MarkupFinished ();
		}

		protected abstract void LoadMarkup ();
		protected abstract void MarkupFinished ();
	}
}
