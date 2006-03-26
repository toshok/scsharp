using System;
using System.IO;
using System.Threading;

using Gtk;
using Gdk;

namespace Starcraft
{
	public class Game
	{
		const uint GAME_ANIMATION_TICK = 50; // number of milliseconds between animation updates

		Race race;

		MPQ scenario;
		MPQ mpq;
		Painter painter;

		Gtk.Window window;
		Gtk.DrawingArea da;

		uint cached_cursor_x;
		uint cached_cursor_y;

		static Game instance;
		public static Game Instance {
			get { return instance; }
		}

		public Game (MPQ mpq)
		{
			this.mpq = mpq;

			instance = this;

			race = Race.Protoss;
		}

		public void Startup ()
		{
			CreateWindow ();
			DisplayTitle ();
			LoadGlobalResources ();
			LoadMainMenu ();
		}

		public void SetScenario (MPQ scenario)
		{
			this.scenario = scenario;
		}

		void CreateWindow ()
		{
			window = new Gtk.Window ("Starcraft");
			window.SetDefaultSize (640, 480);

			da = new DrawingArea ();
			window.Add (da);

			painter = new Painter (da, GAME_ANIMATION_TICK);

			da.AddEvents ((int)(EventMask.PointerMotionMask |
					    EventMask.ButtonPressMask |
					    EventMask.ButtonReleaseMask | 
					    EventMask.ButtonMotionMask));
			da.MotionNotifyEvent += PointerMotion;

			window.ShowAll ();
		}
		
		void PointerMotion (object o, MotionNotifyEventArgs args)
		{
			cached_cursor_x = (uint)args.Event.X;
			cached_cursor_y = (uint)args.Event.Y;
			
			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);
		}

		public Painter Painter {
			get { return painter; }
		}

		public Race Race {
			get { return race; }
		}

		Gdk.Pixbuf splash;
		CursorAnimator cursor;

		public CursorAnimator Cursor {
			get { return cursor; }
			set {
				if (cursor != null)
					painter.Remove (Layer.Cursor, cursor.Paint);
				cursor = value;
				if (cursor != null) {
					painter.Add (Layer.Cursor, cursor.Paint);
					cursor.SetPosition (cached_cursor_x, cached_cursor_y);
					HideSystemCursor ();
				}
			}
		}

		void DisplayTitle ()
		{
			splash = new Gdk.Pixbuf ((Stream)mpq.GetResource (Builtins.TitlePcx));
			painter.Add (Layer.Background, SplashPainter);
		}

		void HideSystemCursor()
		{
			Gdk.Pixbuf empty = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, true, 8, 1, 1);
			empty.Fill (0x00000000);
			Gdk.Cursor cempty = new Gdk.Cursor (da.GdkWindow.Display, empty, 0, 0);

			da.GdkWindow.Cursor = cempty;
		}

		void SplashPainter (Gdk.Pixbuf pb, DateTime dt)
		{
			splash.Composite (pb, 0, 0, splash.Width, splash.Height,
					  0, 0, 1, 1, InterpType.Nearest, 0xff);
		}

		UIScreen currentScreen;
		UIScreen newScreen;

		UIScreen gamescreen;
		void GameScreenReady ()
		{
			gamescreen.Ready -= GameScreenReady;
			SetGameScreen (gamescreen);
		}

		void NewScreen_DoneSwooshing ()
		{
			currentScreen.DoneSwooshing -= NewScreen_DoneSwooshing;
			Console.WriteLine ("done swooshing!");

			if (gamescreen == null) {
				gamescreen = new GameScreen (mpq, scenario);
				gamescreen.Ready += GameScreenReady;
				gamescreen.Load ();
			}
		}

		void OldScreen_DoneSwooshing ()
		{
			if (currentScreen != null) {
				currentScreen.DoneSwooshing -= OldScreen_DoneSwooshing;
				currentScreen.RemoveFromPainter (painter);
			}

			currentScreen = newScreen;
			newScreen = null;

			if (currentScreen != null) {
				currentScreen.AddToPainter (painter);
				currentScreen.DoneSwooshing += NewScreen_DoneSwooshing;
				currentScreen.SwooshIn ();
			}
		}

		public void SetGameScreen (UIScreen screen)
		{
			newScreen = screen;
			if (currentScreen != null) {
				currentScreen.DoneSwooshing += OldScreen_DoneSwooshing;
				currentScreen.SwooshOut ();
			}
			else
				OldScreen_DoneSwooshing ();
		}

		UIScreen mainMenu;
		void MainMenuReady ()
		{
			mainMenu.Ready -= MainMenuReady;

			painter.Remove (Layer.Background, SplashPainter);
			SetGameScreen (mainMenu);
			mainMenu = null;
		}

		void LoadMainMenu ()
		{
			mainMenu = new MainMenu (mpq);
			mainMenu.Ready += MainMenuReady;
			mainMenu.Load ();
		}

		void LoadGlobalResources ()
		{
			new GlobalResources (mpq);
			GlobalResources.Instance.Load ();
		}

#if notyet
		void HudPainter (Gdk.Pixbuf pb, DateTime dt)
		{
			hud.Composite (pb, 0, 0, hud.Width, hud.Height,
				       0, 0, 1, 1, InterpType.Nearest, 0xff);
		}

		void HudReady ()
		{
			painter.Add (Layer.HUD, HudPainter);
		}

		void LoadProtossHud ()
		{
			ThreadPool.QueueUserWorkItem (HudResourceLoaderThread);
		}

		void HudResourceLoaderThread (object state)
		{
			Console.WriteLine ("loading protoss hud");

			Gdk.Pixbuf foo = new Gdk.Pixbuf ((Stream)mpq.GetResource (Builtins.Game_PConsolePcx));
			hud = new Gdk.Pixbuf (Colorspace.Rgb, true, 8,
					      foo.Width, foo.Height);

			hud.Fill (0x00000000);
			hud.AddAlpha (true, 0, 0, 0);

			foo.AddAlpha (true, 0, 0, 0);
			foo.Composite (hud, 0, 0, foo.Width, foo.Height, 0, 0, 1, 1, InterpType.Nearest, 0xff);

			foo.Dispose ();

			Thread.Sleep (10000);

			(new ThreadNotify (new ReadyEvent (HudReady))).WakeupMain ();
		}
#endif
	}
}
