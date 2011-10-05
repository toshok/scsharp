using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.ObjCRuntime;

using SCSharp;
using SCSharpMac.UI;

namespace SCSharpMac
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		Game game;
		
		public AppDelegate ()
		{
		}
		
		public override void FinishedLaunching (NSObject notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
				
			string sc_dir = "/Users/toshok/src/scsharp/starcraft-data/starcraft";
			string bw_cd_dir = "/Users/toshok/src/scsharp/starcraft-data/bw-cd";
			string sc_cd_dir = "/Users/toshok/src/scsharp/starcraft-data/sc-cd";
			
            //string sc_cd_dir = ConfigurationManager.AppSettings["StarcraftCDDirectory"];
            //string bw_cd_dir = ConfigurationManager.AppSettings["BroodwarCDDirectory"];

			/* catch this pathological condition where someone has set the cd directories to the same location. */
            if (sc_cd_dir != null && bw_cd_dir != null && bw_cd_dir == sc_cd_dir) {
				Console.WriteLine ("The StarcraftCDDirectory and BroodwarCDDirectory configuration settings must have unique values.");
                return;
			}
			
			game = new Game (sc_dir /*ConfigurationManager.AppSettings["StarcraftDirectory"]*/,
				 			 sc_cd_dir, bw_cd_dir);
			
			mainWindowController.Window.ContentView = game;
			mainWindowController.Window.MakeFirstResponder (game);

			game.Startup();                                                                                                                                                       			
		}
	}
}

