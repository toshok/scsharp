//
// SCSharp.UI.Game
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
using System.Threading;

using SdlDotNet;
using System.Drawing;

using System.Text;

namespace SCSharp.UI
{
	public enum UIScreenType
	{
		/* not including title */
		MainMenu,
		Login,
		Connection,

		ScreenCount
	}

	public class Game
	{
		UIScreen[] screens;
		
		const int GAME_ANIMATION_TICK = 50; // number of milliseconds between animation updates

		bool isBroodWar;
		bool playingBroodWar;

		Race race;

		Mpq broodatMpq;
		Mpq stardatMpq;
		Mpq bwInstallExe;
		Mpq scInstallExe;

		MpqContainer installedMpq;
		MpqContainer playingMpq;

		Painter painter;

		uint cached_cursor_x;
		uint cached_cursor_y;

		string rootDir;

		static Game instance;
		public static Game Instance {
			get { return instance; }
		}

		public Game (string starcraftDir, string scCDDir, string bwCDDir)
		{
			instance = this;

			screens = new UIScreen[(int)UIScreenType.ScreenCount];

			installedMpq = new MpqContainer ();
			playingMpq = new MpqContainer ();

			if (starcraftDir != null) {
				foreach (string path in Directory.GetFileSystemEntries (starcraftDir)) {
					if (Path.GetFileName (path).ToLower() == "broodat.mpq") {
						try {
							broodatMpq = GetMpq (path);
							Console.WriteLine ("found BrooDat.mpq");
						}
						catch (Exception e) {
							throw new Exception (String.Format ("Could not read mpq archive {0}",
											    path),
									     e);
						}
					}
					else if (Path.GetFileName (path).ToLower() == "stardat.mpq") {
						try {
							stardatMpq = GetMpq (path);
							Console.WriteLine ("found StarDat.mpq");
						}
						catch (Exception e) {
							throw new Exception (String.Format ("could not read mpq archive {0}",
											    path),
									     e);
						}
					}
				}
			}

			if (stardatMpq == null) {
				throw new Exception ("unable to locate stardat.mpq, please check your StarcraftDirectory configuration setting");
			}

			if (scCDDir != null) {
				foreach (string path in Directory.GetFileSystemEntries (scCDDir)) {
					if (Path.GetFileName (path) == "install.exe") {
						try {
							scInstallExe = GetMpq (path);
							Console.WriteLine ("found SC install.exe");
						}
						catch (Exception e) {
							throw new Exception (String.Format ("could not read mpq archive {0}",
											    path),
									     e);
						}
					}
				}
			}

			if (bwCDDir != null) {
				foreach (string path in Directory.GetFileSystemEntries (bwCDDir)) {
					if (Path.GetFileName (path).ToLower() == "install.exe") {
						try {
							bwInstallExe = GetMpq (path);
							Console.WriteLine ("found BW install.exe");
						}
						catch (Exception e) {
							throw new Exception (String.Format ("could not read mpq archive {0}",
											    path),
									     e);
						}
					}
				}
			}

			if (bwInstallExe == null)
				throw new Exception ("unable to locate broodwar cd's install.exe, please check your BroodwarCDDirectory configuration setting");

			if (broodatMpq != null)
				installedMpq.Add (broodatMpq);
			if (bwInstallExe != null)
				installedMpq.Add (bwInstallExe);
			if (stardatMpq != null)
				installedMpq.Add (stardatMpq);
			if (scInstallExe != null)
				installedMpq.Add (broodatMpq);
			
			PlayingBroodWar = isBroodWar = (broodatMpq != null);
			
			this.rootDir = starcraftDir;
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

		public string RootDirectory {
			get { return rootDir; }
		}

		public bool PlayingBroodWar {
			get { return playingBroodWar; }
			set {
				playingBroodWar = value;
				playingMpq.Clear ();
				if (playingBroodWar) {
					if (bwInstallExe == null)
						throw new Exception ("you need the Broodwar CD to play Broodwar games.  Please check the BroodwarCDDirectory configuration setting.");
					playingMpq.Add (bwInstallExe);
					playingMpq.Add (broodatMpq);
					playingMpq.Add (stardatMpq);
				}
				else {
					if (scInstallExe == null)
						throw new Exception ("you need the Starcraft CD to play original games.  Please check the StarcraftCDDirectory configuration setting.");
					playingMpq.Add (scInstallExe);
					playingMpq.Add (stardatMpq);
				}
			}

		}

		public bool IsBroodWar {
			get { return isBroodWar; }
		}

		public void Startup (bool fullscreen)
		{
			/* create our window and hook up to the events we care about */
			CreateWindow (fullscreen);

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

		public void Quit ()
		{
			Events.QuitApplication ();
		}

		void DisplayTitle ()
		{
			/* create the title screen, and make sure we
			   don't start loading anything else until
			   it's on the screen */
			UIScreen screen = new TitleScreen (installedMpq);
			screen.Ready += TitleScreenReady;
			SwitchToScreen (screen);
		}

		public const int SCREEN_RES_X = 640;
		public const int SCREEN_RES_Y = 480;

		void CreateWindow (bool fullscreen)
		{
			Video.WindowIcon ();
			Video.WindowCaption = "SCSharp";
			Surface surf;

			if (fullscreen)
				surf = Video.SetVideoMode (SCREEN_RES_X, SCREEN_RES_Y);
			else
				surf = Video.SetVideoModeWindow (SCREEN_RES_X, SCREEN_RES_Y);

			painter = new Painter (surf, GAME_ANIMATION_TICK);
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
			
			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);

			if (currentScreen != null)
				currentScreen.HandlePointerMotion (args);
		}

		void MouseButtonDown (object o, MouseButtonEventArgs args)
		{
			cached_cursor_x = (uint)args.X;
			cached_cursor_y = (uint)args.Y;
			
			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);

			if (currentScreen != null)
				currentScreen.HandleMouseButtonDown (args);
		}

		void MouseButtonUp (object o, MouseButtonEventArgs args)
		{
			cached_cursor_x = (uint)args.X;
			cached_cursor_y = (uint)args.Y;
			
			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);

			if (currentScreen != null)
				currentScreen.HandleMouseButtonUp (args);
		}

		void KeyboardUp (object o, KeyboardEventArgs args)
		{
			if (currentScreen != null)
				currentScreen.HandleKeyboardUp (args);
		}

		void KeyboardDown (object o, KeyboardEventArgs args)
		{
#if !RELEASE
			if ((args.Mod & ModifierKeys.LeftControl) != 0
			    && args.Key == Key.Q)
				Quit ();
#endif
			if (currentScreen != null)
				currentScreen.HandleKeyboardDown (args);
		}

		public Painter Painter {
			get { return painter; }
		}

		public Race Race {
			get { return race; }
			set { race = value; }
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

		UIScreen currentScreen;

		public void SetGameScreen (UIScreen screen)
		{
			if (currentScreen != null)
				currentScreen.RemoveFromPainter (painter);
			currentScreen = screen;
			if (currentScreen != null)
				currentScreen.AddToPainter (painter);
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
					screens[index] = new MainMenu (installedMpq);
					break;
				case UIScreenType.Login:
					screens[index] = new LoginScreen (playingMpq);
					break;
				case UIScreenType.Connection:
					screens[index] = new ConnectionScreen (playingMpq);
					break;
				default:
					throw new Exception ();
				}
			}

			SwitchToScreen (screens[(int)screenType]);
		}

		public Mpq PlayingMpq {
			get { return playingMpq; }
		}

		public Mpq InstalledMpq {
			get { return installedMpq; }
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
			new GlobalResources (installedMpq);
			GlobalResources.Instance.Ready += GlobalResourcesLoaded;
			GlobalResources.Instance.Load ();
		}
	}
}
