using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class MainMenu : UIScreen
	{
		public MainMenu (Mpq mpq) : base (mpq)
		{
		}

		CursorAnimator cursor;
		Surface background;
		Bin mainBin;
		UIPainter mainPainter;

		const int VERSION_ELEMENT_INDEX = 10;

		protected override void ResourceLoader (object state)
		{
			try {
				Console.WriteLine ("loading arrow cursor");
				cursor = new CursorAnimator ((Grp)mpq.GetResource (Builtins.Palmm_ArrowGrp));
				cursor.SetHotSpot (64, 64);
				Console.WriteLine ("loading main menu background");
				background = GuiUtil.SurfaceFromStream ((Stream)mpq.GetResource (Builtins.Palmm_BackgroundPcx));
				Console.WriteLine ("loading main menu ui elements");
				mainBin = (Bin)mpq.GetResource (Builtins.rez_GluMainBin);

				mainBin.Elements[VERSION_ELEMENT_INDEX].text = "v0.0000000001";

				mainPainter = new UIPainter (mainBin, mpq);

				// notify we're ready to roll
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
			}
			catch (Exception e) {
				Console.WriteLine ("Main menu resource loader failed: {0}", e);
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (Events.QuitApplication)));
			}
		}

		protected override void FinishedLoading ()
		{
			Background = background;
			Cursor = cursor;
			UI = mainBin;
			UIPainter = mainPainter;

			base.FinishedLoading ();
		}

		public override void ActivateElement (UIElement e)
		{
			switch ((Key)e.hotkey) {
			case Key.S:
				GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
				Game.Instance.SwitchToScreen (UIScreenType.Login);
				break;
			case Key.M:
				GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
				Game.Instance.SwitchToScreen (UIScreenType.Connection);
				break;
			case Key.R:
				Game.Instance.SwitchToScreen (new CreditsScreen (mpq));
				break;
			case Key.X:
				Events.QuitApplication();
				break;
			default:
				break;
			}
		}
	}
}
