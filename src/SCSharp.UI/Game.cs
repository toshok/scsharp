//
// SCSharp.UI.Game
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
using System.IO;
using System.Threading;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

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
		
		const int GAME_ANIMATION_TICK = 20; // number of milliseconds between animation updates

		bool isBroodWar;
		bool playingBroodWar;

		Race race;

		Mpq patchRtMpq;
		Mpq broodatMpq;
		Mpq stardatMpq;
		Mpq bwInstallExe;
		Mpq scInstallExe;

		MpqContainer installedMpq;
		MpqContainer playingMpq;

		int cached_cursor_x;
		int cached_cursor_y;

		string rootDir;

		static Game instance;
		public static Game Instance {
			get { return instance; }
		}

		public Game (string starcraftDir, string scCDDir, string bwCDDir)
		{
			instance = this;

			starcraftDir = starcraftDir.Replace ('\\', Path.DirectorySeparatorChar)
						   .Replace ('/', Path.DirectorySeparatorChar);

			scCDDir = scCDDir.Replace ('\\', Path.DirectorySeparatorChar)
					 .Replace ('/', Path.DirectorySeparatorChar);

			bwCDDir = bwCDDir.Replace ('\\', Path.DirectorySeparatorChar)
					 .Replace ('/', Path.DirectorySeparatorChar);

			screens = new UIScreen[(int)UIScreenType.ScreenCount];

			installedMpq = new MpqContainer ();
			playingMpq = new MpqContainer ();

			Mpq scMpq = null, bwMpq = null;

			if (starcraftDir != null) {
				foreach (string path in Directory.GetFileSystemEntries (starcraftDir)) {
					if (Path.GetFileName (path).ToLower() == "broodat.mpq" || Path.GetFileName (path).Equals ("Brood War Data")) {
						if (broodatMpq != null)
							throw new Exception ("You have multiple broodat.mpq files in your starcraft directory.");
						try {
							bwMpq = GetMpq (path);
							Console.WriteLine ("found BrooDat.mpq");
						}
						catch (Exception e) {
							throw new Exception (String.Format ("Could not read mpq archive {0}",
											    path),
									     e);
						}
					}
					else if (Path.GetFileName (path).ToLower() == "stardat.mpq" || Path.GetFileName (path).Equals ("Starcraft Data")) {
						if (stardatMpq != null)
							throw new Exception ("You have multiple stardat.mpq files in your starcraft directory.");
						try {
							scMpq = GetMpq (path);
							Console.WriteLine ("found StarDat.mpq");
						}
						catch (Exception e) {
							throw new Exception (String.Format ("could not read mpq archive {0}",
											    path),
									     e);
						}
					}
					else if (Path.GetFileName (path).ToLower() == "patch_rt.mpq" || Path.GetFileName (path).Equals ("Starcraft Mac Patch")) {
						if (patchRtMpq != null)
							throw new Exception ("You have multiple patch_rt.mpq files in your starcraft directory.");
						try {
							patchRtMpq = GetMpq (path);
							Console.WriteLine ("found patch_rt.mpq");
						}
						catch (Exception e) {
							throw new Exception (String.Format ("could not read mpq archive {0}",
											    path),
									     e);
						}
					}
					else if (Path.GetFileName (path).ToLower() == "starcraft.mpq") {
						try {
							scInstallExe = GetMpq (path);
							Console.WriteLine ("found starcraft.mpq");
						}
						catch (Exception e) {
							throw new Exception (String.Format ("could not read mpq archive {0}",
											    path),
									     e);
						}
					}
					else if (Path.GetFileName (path).ToLower() == "broodwar.mpq") {
						try {
							bwInstallExe = GetMpq (path);
							Console.WriteLine ("found broodwar.mpq");
						}
						catch (Exception e) {
							throw new Exception (String.Format ("could not read mpq archive {0}",
											    path),
									     e);
						}
					}
				}
			}

			if (scMpq == null) {
				throw new Exception ("unable to locate stardat.mpq, please check your StarcraftDirectory configuration setting");
			}

			if (!string.IsNullOrEmpty (scCDDir)) {
				foreach (string path in Directory.GetFileSystemEntries (scCDDir)) {
					if (Path.GetFileName (path).ToLower() == "install.exe" || Path.GetFileName (path).Equals ("Starcraft Archive")) {
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

			if (!string.IsNullOrEmpty (bwCDDir)) {
				foreach (string path in Directory.GetFileSystemEntries (bwCDDir)) {
					if (Path.GetFileName (path).ToLower() == "install.exe" || Path.GetFileName (path).Equals ("Brood War Archive")) {
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

			if (bwMpq != null) {
				if (patchRtMpq != null) {
					broodatMpq = new MpqContainer ();
					((MpqContainer)broodatMpq).Add (patchRtMpq);
					((MpqContainer)broodatMpq).Add (bwMpq);
				}
				else
					broodatMpq = bwMpq;
			}

			if (scMpq != null) {
				if (patchRtMpq != null) {
					stardatMpq = new MpqContainer ();
					((MpqContainer)stardatMpq).Add (patchRtMpq);
					((MpqContainer)stardatMpq).Add (scMpq);
				}
				else
					stardatMpq = bwMpq;
			}

			if (broodatMpq != null)
				installedMpq.Add (broodatMpq);
			if (bwInstallExe != null)
				installedMpq.Add (bwInstallExe);
			if (stardatMpq != null)
				installedMpq.Add (stardatMpq);
			if (scInstallExe != null)
				installedMpq.Add (scInstallExe);
			
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
					if (patchRtMpq != null)
						playingMpq.Add (patchRtMpq);
					playingMpq.Add (bwInstallExe);
					playingMpq.Add (broodatMpq);
					playingMpq.Add (stardatMpq);
				}
				else {
					if (scInstallExe == null)
						throw new Exception ("you need the Starcraft CD to play original games.  Please check the StarcraftCDDirectory configuration setting.");
					if (patchRtMpq != null)
						playingMpq.Add (patchRtMpq);
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

			Mouse.ShowCursor = false;

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
			screen.FirstPainted += TitleScreenReady;
			SwitchToScreen (screen);
		}

		void CreateWindow (bool fullscreen)
		{
			Video.WindowIcon ();
			Video.WindowCaption = "SCSharp";

			Painter.InitializePainter (fullscreen, GAME_ANIMATION_TICK);
		}

		void UserEvent (object sender, UserEventArgs args)
		{
			ReadyDelegate d = (ReadyDelegate)args.UserEvent;
			d ();
		}

		void PointerMotion (object o, MouseMotionEventArgs args)
		{
			cached_cursor_x = args.X;
			cached_cursor_y = args.Y;

			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);

			if (currentScreen != null)
				currentScreen.HandlePointerMotion (args);
		}

		void MouseButtonDown (object o, MouseButtonEventArgs args)
		{
			cached_cursor_x = args.X;
			cached_cursor_y = args.Y;

			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);

			if (currentScreen != null)
				currentScreen.HandleMouseButtonDown (args);
		}

		void MouseButtonUp (object o, MouseButtonEventArgs args)
		{
			cached_cursor_x = args.X;
			cached_cursor_y = args.Y;
			
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
			if ((args.Mod & ModifierKeys.LeftControl) != 0) {
				if (args.Key == Key.Q) {
					Quit ();
					return;
				}
				else if (args.Key == Key.F) {
					Painter.Fullscreen = !Painter.Fullscreen;
					return;
				}
			}
#endif
			if (currentScreen != null)
				currentScreen.HandleKeyboardDown (args);
		}

		public Race Race {
			get { return race; }
			set { race = value; }
		}

		CursorAnimator cursor;

		public CursorAnimator Cursor {
			get { return cursor; }
			set {
				if (cursor == value)
					return;

				if (cursor != null)
					cursor.RemoveFromPainter ();
				cursor = value;
				if (cursor != null) {
					cursor.AddToPainter ();
					cursor.SetPosition (cached_cursor_x, cached_cursor_y);
				}
			}
		}

		UIScreen currentScreen;

		public void SetGameScreen (UIScreen screen)
		{
			Painter.Pause ();

			if (currentScreen != null)
				currentScreen.RemoveFromPainter ();
			currentScreen = screen;
			if (currentScreen != null)
				currentScreen.AddToPainter ();

			Painter.Resume ();
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
			new GlobalResources (stardatMpq, broodatMpq);
			GlobalResources.Instance.Ready += GlobalResourcesLoaded;
			GlobalResources.Instance.Load ();
		}
	}
}
