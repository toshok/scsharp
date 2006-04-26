using System;
using System.IO;
using Starcraft;

using SdlDotNet;
using System.Drawing;

public class FontFoo {
#if false
	static string str1 = "abcdefghijklmnopqrstuvwxyz";
	static string str2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	static string str3 = "1234567890!@#$%^&*()`~-_=+[]{}\\|;:'\",.<>/?";
#endif

	static string str4 = "Kyll";

	public static void Main (string[] args)
	{
		MpqContainer mpq = new MpqContainer ();
		mpq.Add (new MpqArchive ("/home/toshok/starcraft/install.exe"));
		mpq.Add (new MpqArchive ("/home/toshok/starcraft/starcraft/StarDat.mpq"));

		Fnt fnt = (Fnt)mpq.GetResource ("files\\font\\font16.fnt");
		Console.WriteLine ("loading font palette");
		Stream palStream = (Stream)mpq.GetResource ("glue\\Palmm\\tFont.pcx");
		Pcx pcx1 = new Pcx ();
		pcx1.ReadFromStream (palStream, false);

		Painter painter = new Painter (Video.SetVideoModeWindow (600, 100), 300);

#if false
		Surface textSurf1 = GuiUtil.ComposeText (str1, fnt, pcx1.Palette);
		Surface textSurf2 = GuiUtil.ComposeText (str2, fnt, pcx1.Palette);
		Surface textSurf3 = GuiUtil.ComposeText (str3, fnt, pcx1.Palette);
#endif
		Surface textSurf4 = GuiUtil.ComposeText (str4, fnt, pcx1.Palette);

		painter.Add (Layer.UI,
			     delegate (Surface surf, DateTime now) {
#if false
				surf.Blit (textSurf1, new Point (0,0));
				surf.Blit (textSurf2, new Point (0, textSurf1.Size.Height));
				surf.Blit (textSurf3, new Point (0, textSurf1.Size.Height + textSurf2.Size.Height));
#endif
				surf.Blit (textSurf4, new Point (0, 0));
			     });

		painter.Add (Layer.Background,
			     delegate (Surface surf, DateTime now) {
				surf.Fill (new Rectangle (new Point (0, 0), surf.Size), Color.Red);
			     });

		Events.KeyboardUp += delegate (object o, KeyboardEventArgs args) {
			if (args.Key == Key.Escape)
				Events.QuitApplication();
		};

		Events.Run ();
	}
}
