//
// SCSharpMac.UI.Game
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
using System.Drawing;
using System.Text;
using System.Diagnostics;

using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreVideo;
using MonoMac.CoreGraphics;

using SCSharp;

namespace SCSharpMac.UI
{
	public enum UIScreenType
	{
		/* not including title */
		MainMenu,
		Login,
		Connection,

		ScreenCount
	}

	public delegate void TickEventHandler (object sender, TickEventArgs args);
	
	
	public class TickEventArgs : EventArgs {
		public TickEventArgs (long millis)
		{
			elapsedMillis = millis;
		}
		
		public long MillisecondsElapsed {
			get { return elapsedMillis; }
		}
		
		public float SecondsElapsed {
			get { return (float)elapsedMillis / 1000.0f; }
		}

		long elapsedMillis;
	}
			

	public class Game : NSView
	{
		UIScreen[] screens;
		
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
			
			Layer = CALayer.Create();
			Layer.BackgroundColor = new CGColor (0, 0, 0, 1);
			
#if USE_TRACKING_RECTS
			AddTrackingRect (new Rectangle (0, 0, 640, 480), this, IntPtr.Zero, false);
#endif
			
			WantsLayer = true;
			
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
		
		
		long lastVideoMillis;
		void EmitTick ()
		{			
			if (lastVideoMillis == 0L) {
				// we don't actually emit on this, we just prepare for the next video update
				lastVideoMillis = tickStopWatch.ElapsedMilliseconds;
				return;
			}
			
			var h = Tick;
			if (h != null)
				h (this, new TickEventArgs (tickStopWatch.ElapsedMilliseconds - lastVideoMillis));
			
			lastVideoMillis = tickStopWatch.ElapsedMilliseconds;
		}
		
		public event TickEventHandler Tick;

		
		public override void MouseDown (NSEvent theEvent)
		{
			Console.WriteLine ("MouseDown");
			
			cached_cursor_x = (int)theEvent.LocationInWindow.X;
			cached_cursor_y = (int)theEvent.LocationInWindow.Y;

			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);

			if (currentScreen != null)
				currentScreen.HandleMouseButtonDown (theEvent);
		}
		
		public override void MouseUp (NSEvent theEvent)
		{
			Console.WriteLine ("MouseUp at {0} {1}", theEvent.LocationInWindow.X, theEvent.LocationInWindow.Y);
			cached_cursor_x = (int)theEvent.LocationInWindow.X;
			cached_cursor_y = (int)theEvent.LocationInWindow.Y;
			
			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);

			if (currentScreen != null)
				currentScreen.HandleMouseButtonUp (theEvent);
		}
		
		public override void MouseDragged (NSEvent theEvent)
		{
			Console.WriteLine ("MouseDragged");
			base.MouseDragged (theEvent);
		}
		
		public override void MouseMoved (NSEvent theEvent)
		{
			Console.WriteLine ("MouseMoved");
			
			cached_cursor_x = (int)theEvent.LocationInWindow.X;
			cached_cursor_y = (int)theEvent.LocationInWindow.Y;
			
			if (cursor != null)
				cursor.SetPosition (cached_cursor_x, cached_cursor_y);

			if (currentScreen != null)
				currentScreen.HandlePointerMotion (theEvent);
		}
		
#if USE_TRACKING_RECTS
		public override void MouseEntered (NSEvent theEvent)
		{
			Console.WriteLine ("MouseEntered");
			base.MouseEntered (theEvent);
		}
		
		public override void MouseExited (NSEvent theEvent)
		{
			Console.WriteLine ("MouseExited");
			base.MouseExited (theEvent);
		}
#endif
		
		public override void KeyDown (NSEvent theEvent)
		{
//#if !RELEASE
#if notyet
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
				currentScreen.HandleKeyboardDown (theEvent);
		}
		
		public override void KeyUp (NSEvent theEvent)
		{
			if (currentScreen != null)
				currentScreen.HandleKeyboardUp (theEvent);
		}
		
		Stopwatch tickStopWatch;
		
		public void Startup ()
		{
			DisplayTitle ();
			
			displayLink = new CVDisplayLink();
			displayLink.SetOutputCallback (DisplayLinkOutputCallback);
		
			tickStopWatch = new Stopwatch ();
			tickStopWatch.Start ();
			
			displayLink.Start ();
		}
		
		CVDisplayLink displayLink;
		CVReturn DisplayLinkOutputCallback (CVDisplayLink displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut)
		{
			Game.Instance.BeginInvokeOnMainThread ( () => Game.Instance.EmitTick () );
			return CVReturn.Success;
		}
		
		public void Quit ()
		{
			Environment.Exit (0);
		}

		void DisplayTitle ()
		{
			/* create the title screen, and make sure we
			   don't start loading anything else until
			   it's on the screen */
			UIScreen screen = new TitleScreen (installedMpq);
#if notyet
			screen.FirstPainted += TitleScreenReady;
			SwitchToScreen (screen);
#else
			SwitchToScreen (screen);
			TitleScreenReady ();
#endif
		}

		void CreateWindow (bool fullscreen)
		{
#if notyet
			Video.WindowIcon ();
			Video.WindowCaption = "SCSharp";

			Painter.InitializePainter (fullscreen, GAME_ANIMATION_TICK);
#endif
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

				if (cursor != null) {
					cursor.RemoveFromPainter ();
					cursor.RemoveFromSuperLayer ();
				}
				cursor = value;
				if (cursor != null) {
					cursor.AddToPainter ();
					cursor.SetPosition (cached_cursor_x, cached_cursor_y);
					Layer.AddSublayer (cursor);
				}
			}
		}

		UIScreen currentScreen;

		public void SetGameScreen (UIScreen screen)
		{
			if (currentScreen != null) {
				currentScreen.RemoveFromPainter ();
				currentScreen.RemoveFromSuperLayer ();
			}
			
			currentScreen = screen;
			
			if (currentScreen != null) {
				Layer.AddSublayer (currentScreen);
				currentScreen.AddToPainter ();
			}
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
#if notyet 
					screens[index] = new ConnectionScreen (playingMpq);
#endif
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
			Console.WriteLine ("GlobalResourcesLoaded");
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

