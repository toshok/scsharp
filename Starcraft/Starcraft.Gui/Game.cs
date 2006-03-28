using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public enum UIScreenType {
		/* not including title */
		MainMenu,
		Login,
		Connection,

		ScreenCount
	}

	public class Game
	{
		UIScreenType currentScreenType;
		UIScreen[] screens;
		
		const int GAME_ANIMATION_TICK = 50; // number of milliseconds between animation updates

		bool isBroodWar;

		Race race;

		Mpq scenario;
		Mpq mpq;
		Painter painter;

		uint cached_cursor_x;
		uint cached_cursor_y;

		static Game instance;
		public static Game Instance {
			get { return instance; }
		}

		public Game (string starcraftDir)
		{
			instance = this;

			race = Race.Protoss;

			screens = new UIScreen[(int)UIScreenType.ScreenCount];

			mpq = new MpqContainer ();

			Mpq broodatMpq = null, stardatMpq = null;

			foreach (string path in Directory.GetFileSystemEntries (starcraftDir, "*.mpq")) {
				if (path.ToLower().EndsWith ("broodat.mpq")) {
					Console.WriteLine (path);
					broodatMpq = GetMpq (path);
				}
				else if (path.ToLower().EndsWith ("stardat.mpq")) {
					Console.WriteLine (path);
					stardatMpq = GetMpq (path);
				}
			}

			if (broodatMpq == null && stardatMpq == null) {
				throw new Exception ("Could not locate broodat.mpq and/or stardat.mpq.");
			}

			if (broodatMpq != null) {
				isBroodWar = true;
				((MpqContainer)mpq).Add (broodatMpq);
			}

			if (stardatMpq != null)
				((MpqContainer)mpq).Add (stardatMpq);

		}

		Mpq GetMpq (string path)
		{
			if (Directory.Exists (path))
				return new MpqDirectory (path);
			else if (File.Exists (path))
				return new MpqArchive (path);
			else
				return null;
		}

		public bool IsBroodWar {
			get { return isBroodWar; }
		}

		public void Startup ()
		{
			/* create our window and hook up to the events we care about */
			CreateWindow ();

			Events.UserEvent += UserEvent;
			Events.MouseMotion += PointerMotion;
			Events.MouseButtonDown += MouseButtonDown;
			Events.MouseButtonUp += MouseButtonUp;
			Events.KeyboardUp += KeyboardUp;
			Events.KeyboardDown += KeyboardDown;

			DisplayTitle ();

			/* start everything up */
			Events.Run ();
		}

		void DisplayTitle ()
		{
			/* create the title screen, and make sure we
			   don't start loading anything else until
			   it's on the screen */
			UIScreen screen = new TitleScreen (mpq);
			screen.Ready += TitleScreenReady;
			SwitchToScreen (screen);
		}

		void CreateWindow ()
		{
			Video.WindowIcon ();
			Video.WindowCaption = "Starcraft";
			Video.SetVideoModeWindow (640, 480);

			painter = new Painter (GAME_ANIMATION_TICK);

		}

		void UserEvent (object sender, UserEventArgs args)
		{
			ReadyDelegate d = (ReadyDelegate)args.UserEvent;
			d ();
		}

		void PointerMotion (object o, MouseMotionEventArgs args)
		{
			cached_cursor_x = (uint)args.X;
			cached_cursor_y = (uint)args.Y;
			
			if (swooshing)
				return;

			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);

			if (currentScreen != null)
				currentScreen.PointerMotion (args);
		}

		void MouseButtonDown (object o, MouseButtonEventArgs args)
		{
			cached_cursor_x = (uint)args.X;
			cached_cursor_y = (uint)args.Y;
			
			if (swooshing)
				return;

			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);

			if (currentScreen != null)
				currentScreen.MouseButtonDown (args);
		}

		void MouseButtonUp (object o, MouseButtonEventArgs args)
		{
			cached_cursor_x = (uint)args.X;
			cached_cursor_y = (uint)args.Y;
			
			if (swooshing)
				return;

			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);

			if (currentScreen != null)
				currentScreen.MouseButtonUp (args);
		}

		void KeyboardUp (object o, KeyboardEventArgs args)
		{
			if (swooshing)
				return;

			if (currentScreen != null)
				currentScreen.KeyboardUp (args);
		}

		void KeyboardDown (object o, KeyboardEventArgs args)
		{
			if (swooshing)
				return;

			if (currentScreen != null)
				currentScreen.KeyboardDown (args);
		}

		public Painter Painter {
			get { return painter; }
		}

		public Race Race {
			get { return race; }
		}

		CursorAnimator cursor;

		public CursorAnimator Cursor {
			get { return cursor; }
			set {
				if (cursor != null)
					painter.Remove (Layer.Cursor, cursor.Paint);
				cursor = value;
				if (cursor == null) {
					Mouse.ShowCursor = true;
				}
				else {
					painter.Add (Layer.Cursor, cursor.Paint);
					cursor.SetPosition (cached_cursor_x, cached_cursor_y);
					Mouse.ShowCursor = false;
				}
			}
		}

		bool swooshing;
		UIScreen currentScreen;
		UIScreen newScreen;

		void NewScreen_DoneSwooshing ()
		{
			currentScreen.DoneSwooshing -= NewScreen_DoneSwooshing;
			Console.WriteLine ("done swooshing in");
			swooshing = false;
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
			swooshing = true;
			if (currentScreen != null) {
				currentScreen.DoneSwooshing += OldScreen_DoneSwooshing;
				currentScreen.SwooshOut ();
			}
			else
				OldScreen_DoneSwooshing ();
		}

		UIScreen screenToSwitchTo;
		public void SwitchToScreen (UIScreen screen)
		{
			screen.Ready += SwitchReady;
			screenToSwitchTo = screen;
			screenToSwitchTo.Load ();
			return;
		}

		public void SwitchToScreen (UIScreenType screenType)
		{
			int index = (int)screenType;
			if (screens[index] == null) {
				switch (screenType) {
				case UIScreenType.MainMenu:
					screens[index] = new MainMenu (mpq);
					break;
				case UIScreenType.Login:
					screens[index] = new LoginScreen (mpq);
					break;
				case UIScreenType.Connection:
					screens[index] = new ConnectionScreen (mpq);
					break;
				default:
					throw new Exception ();
				}
			}

			SwitchToScreen (screens[(int)screenType]);
		}

		void SwitchReady ()
		{
			screenToSwitchTo.Ready -= SwitchReady;
			SetGameScreen (screenToSwitchTo);
			screenToSwitchTo = null;
		}

		void GlobalResourcesLoaded ()
		{
			SwitchToScreen (UIScreenType.MainMenu);
		}

		void TitleScreenReady ()
		{
			Console.WriteLine ("Loading global resources");
			new GlobalResources (mpq);
			GlobalResources.Instance.Ready += GlobalResourcesLoaded;
			GlobalResources.Instance.Load ();
		}
	}
}
