using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class LoginScreen : UIScreen
	{
		public LoginScreen (Mpq mpq) : base (mpq)
		{
		}

		CursorAnimator cursor;
		Surface background;
		Bin loginBin;
		UIPainter loginPainter;

		protected override void ResourceLoader (object state)
		{
			try {
				cursor = new CursorAnimator ((Grp)mpq.GetResource (Builtins.PalNl_ArrowGrp));
				cursor.SetHotSpot (64, 64);
				Console.WriteLine ("loading login screen background");
				background = GuiUtil.SurfaceFromStream ((Stream)mpq.GetResource (Builtins.PalNl_BackgroundPcx));
				Console.WriteLine ("loading login screen ui elements");
				loginBin = (Bin)mpq.GetResource (Builtins.rez_GluLoginBin);

				loginPainter = new UIPainter (loginBin, mpq);

				// notify we're ready to roll
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
			}
			catch (Exception e) {
				Console.WriteLine ("Login screen resource loader failed: {0}", e);
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (Events.QuitApplication)));
			}
		}

		protected override void FinishedLoading ()
		{
			Background = background;
			Cursor = cursor;
			UI = loginBin;
			UIPainter = loginPainter;

			base.FinishedLoading ();
		}

		public override void ActivateElement (UIElement e)
		{
			switch ((Key)e.hotkey)
			{
			case Key.C:
				Game.Instance.SwitchToScreen (UIScreenType.MainMenu);
				break;
			default:
				break;
			}
		}
	}
}
