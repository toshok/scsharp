using System;
using System.IO;
using SCSharp;
using SCSharp.UI;

using SdlDotNet;
using System.Drawing;

public class FontFoo {
	static string str1 = "abcdefghijklmnopqrstuvwxyz";
	static string str2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	static string str3 = "1234567890!@#$%^&*()`~-_=+[]{}\\|;:'\",.<>/?";

	static string str4 = "Kyll";

	public static void Main (string[] args)
	{
		MpqContainer mpq = new MpqContainer ();
		mpq.Add (new MpqArchive ("/home/toshok/src/starcraft/sc-cd/install.exe"));
		mpq.Add (new MpqArchive ("/home/toshok/src/starcraft/starcraft/StarDat.mpq"));

		Fnt fnt = (Fnt)mpq.GetResource ("files\\font\\font16.fnt");
		Console.WriteLine ("loading font palette");
		Stream palStream = (Stream)mpq.GetResource ("glue\\Palmm\\tFont.pcx");
		Pcx pcx1 = new Pcx ();
		pcx1.ReadFromStream (palStream, -1, -1);

		Painter.InitializePainter (false, 300);

		Surface textSurf1 = GuiUtil.ComposeText (str1, fnt, pcx1.Palette);
		Surface textSurf2 = GuiUtil.ComposeText (str2, fnt, pcx1.Palette);
		Surface textSurf3 = GuiUtil.ComposeText (str3, fnt, pcx1.Palette);
		Surface textSurf4 = GuiUtil.ComposeText (str4, fnt, pcx1.Palette);

		Surface backgroundSurface = new Surface (Painter.SCREEN_RES_X, Painter.SCREEN_RES_Y);
		backgroundSurface.Fill (new Rectangle (new Point (0, 0), backgroundSurface.Size), Color.Red);

		Painter.Add (Layer.UI,
			     delegate (DateTime now) {
				int y = 0;
				Painter.Blit (textSurf1, new Point (0, y)); y += textSurf1.Height;
				Painter.Blit (textSurf2, new Point (0, y)); y += textSurf2.Height;
				Painter.Blit (textSurf3, new Point (0, y)); y += textSurf3.Height;
				Painter.Blit (textSurf4, new Point (0, y)); y += textSurf4.Height;
			     });

		Painter.Add (Layer.Background,
			     delegate (DateTime now) {
				     Painter.Blit (backgroundSurface);
			     });

		Events.KeyboardUp += delegate (object o, KeyboardEventArgs keyargs) {
			if (keyargs.Key == Key.Escape)
				Events.QuitApplication();
		};

		Events.Run ();
	}
}
