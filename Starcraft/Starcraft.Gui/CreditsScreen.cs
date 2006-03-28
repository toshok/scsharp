using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class CreditsScreen : UIScreen
	{
		public CreditsScreen (Mpq mpq) : base (mpq)
		{
		}

		enum PageLocation {
			Center,
			Top,
			Bottom,
			Left,
			Right
		}

		class CreditsPage
		{
			PageLocation location;
			List<string> lines;
			Surface surface;
			Surface newBackground;

			int width;
			int height;

			bool delay; /* should we stop processing for some amount of time after this page */

			public CreditsPage (PageLocation loc)
			{
				location = loc;
				lines = new List<string> ();
				delay = true;
			}

			public CreditsPage (Stream background)
			{
				newBackground = GuiUtil.SurfaceFromStream (background);
				delay = false;
			}

			public void AddLine (string line)
			{
				lines.Add (line);
			}

			int MeasureWidth ()
			{
				SdlDotNet.Font font = GuiUtil.LargeFont;
				int width = 0;

				foreach (string l in lines) {
					Size s = font.SizeText (l);
					if (width < s.Width)
						width = s.Width;
				}

				return width;
			}

			int MeasureHeight ()
			{
				SdlDotNet.Font font = GuiUtil.LargeFont;
				return font.LineSize * lines.Count;
			}

			public void Layout ()
			{
				width = MeasureWidth ();
				height = MeasureHeight ();
			}

			public Surface Background {
				get { return newBackground; } 
			}

			public bool HasText {
				get { return lines != null && lines.Count > 0; }
			}

			public void Paint (Surface surf)
			{
				SdlDotNet.Font font = GuiUtil.LargeFont;
				int x, y;

				Color c = Color.FromArgb (0, 0, 255, 0);

				switch (location) {
				case PageLocation.Top:
					y = 10;
					foreach (string l in lines) {
						Surface s = font.Render (l, c);
						surf.Blit (s, new Point ((surf.Width - s.Width) / 2, y));
						y += font.LineSize;
					}
					break;
				case PageLocation.Bottom:
					y = surf.Height - 10 - font.LineSize * lines.Count;
					foreach (string l in lines) {
						Surface s = font.Render (l, c);
						surf.Blit (s, new Point ((surf.Width - s.Width) / 2, y));
						y += font.LineSize;
					}
					break;
				case PageLocation.Left:
					y = (surf.Height - font.LineSize * lines.Count) / 2;
					foreach (string l in lines) {
						Surface s = font.Render (l, c);
						surf.Blit (s, new Point (60, y));
						y += font.LineSize;
					}
					break;
				case PageLocation.Right:
					y = (surf.Height - font.LineSize * lines.Count) / 2;
					foreach (string l in lines) {
						Surface s = font.Render (l, c);
						surf.Blit (s, new Point (surf.Width - s.Width - 60, y));
						y += font.LineSize;
					}
					break;
				case PageLocation.Center:
					y = (surf.Height - font.LineSize * lines.Count) / 2;
					foreach (string l in lines) {
						Surface s = font.Render (l, c);
						surf.Blit (s, new Point ((surf.Width - s.Width) / 2, y));
						y += font.LineSize;
					}
					break;
				}
			}
		}

		List<CreditsPage> pages;

		protected override void ResourceLoader (object state)
		{
			pages = new List<CreditsPage> ();

			try {
				StreamReader sr;

				if (Game.Instance.IsBroodWar) {
					/* broodwar credits */
					sr = new StreamReader ((Stream)mpq.GetResource (Builtins.RezCrdtexpTxt));
					Parse (sr);
				}

				/* starcraft credits */
				sr = new StreamReader ((Stream)mpq.GetResource (Builtins.RezCrdtlistTxt));
				Parse (sr);

				// notify we're ready to roll
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
			}
			catch (Exception e) {
				Console.WriteLine ("Login screen resource loader failed: {0}", e);
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (Events.QuitApplication)));
			}
		}

		void AdvanceToNextPage ()
		{
			while (pageEnumerator.MoveNext ()) {
				if (pageEnumerator.Current.Background != null)
					currentBackground = pageEnumerator.Current.Background;
				if (pageEnumerator.Current.HasText)
					return;
			}

			ReturnToMainMenu ();
		}

		protected override void FinishedLoading ()
		{
			StartUp ();
			base.FinishedLoading ();
		}

		void StartUp ()
		{
			millisDelay = 4000;
			pageEnumerator = pages.GetEnumerator();
			AdvanceToNextPage ();
		}

		// painting
		Surface currentBackground;
		IEnumerator<CreditsPage> pageEnumerator;

		int millisDelay;
		int totalElapsed;

		Painter painter;

		public override void AddToPainter (Painter painter)
		{
			this.painter = painter;
			/* we add a special entry to the background
			 * painter layer just so we can find out when
			 * we're first painted */
			painter.Add (Layer.Background, FirstPainted);

			painter.Add (Layer.Background, PaintBackground);
			painter.Add (Layer.UI, PaintCredits);
		}

		public override void RemoveFromPainter (Painter painter)
		{
			painter.Remove (Layer.Background, FirstPainted);

			painter.Remove (Layer.Background, PaintBackground);
			painter.Remove (Layer.UI, PaintCredits);
		}

		void FirstPainted (Surface surf, DateTime now)
		{
			painter.Remove (Layer.Background, FirstPainted);
			painter = null;

			/* set ourselves up to invalidate at a regular interval*/
                        Events.Tick += FlipPage;
		}

		void PaintBackground (Surface surf, DateTime now)
		{
			if (currentBackground != null)
				surf.Blit (currentBackground);
		}

		void PaintCredits (Surface surf, DateTime now)
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
				Game.Instance.SwitchToScreen (UIScreenType.MainMenu);
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

		void ReturnToMainMenu ()
		{
                        Events.Tick -= FlipPage;
			Game.Instance.SwitchToScreen (UIScreenType.MainMenu);
		}

		void Parse (StreamReader sr)
		{
			string l;
			CreditsPage currentPage = null;

			while ((l = sr.ReadLine ()) != null) {
				if (l.StartsWith ("</")) {
					if (l.StartsWith ("</PAGE>")) {
						currentPage.Layout ();
						pages.Add (currentPage);
						currentPage = null;
					}
					else if (l.StartsWith ("</SCREENCENTER>")) {
						currentPage = new CreditsPage (PageLocation.Center);
					}
					else if (l.StartsWith ("</SCREENLEFT>")) {
						currentPage = new CreditsPage (PageLocation.Left);
					}
					else if (l.StartsWith ("</SCREENRIGHT>")) {
						currentPage = new CreditsPage (PageLocation.Right);
					}
					else if (l.StartsWith ("</SCREENTOP>")) {
						currentPage = new CreditsPage (PageLocation.Top);
					}
					else if (l.StartsWith ("</SCREENBOTTOM>")) {
						currentPage = new CreditsPage (PageLocation.Bottom);
					}
					else if (l.StartsWith ("</BACKGROUND ")) {
						string bg = l.Substring ("</BACKGROUND ".Length);
						bg = bg.Substring (0, bg.Length - 1);
						pages.Add (new CreditsPage ((Stream)mpq.GetResource (bg)));
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
	}
}
