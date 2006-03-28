using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class ConnectionScreen : UIScreen
	{
		public ConnectionScreen (Mpq mpq) : base (mpq)
		{
		}

		CursorAnimator cursor;
		Surface background;
		Bin connectionBin;
		UIPainter connectionPainter;

		protected override void ResourceLoader (object state)
		{
			try {
				cursor = new CursorAnimator ((Grp)mpq.GetResource (Builtins.PalNl_ArrowGrp));
				cursor.SetHotSpot (64, 64);
				Console.WriteLine ("loading login screen background");
				background = GuiUtil.SurfaceFromStream ((Stream)mpq.GetResource (Builtins.PalNl_BackgroundPcx));
				Console.WriteLine ("loading login screen ui elements");
				connectionBin = (Bin)mpq.GetResource (Builtins.rez_GluConnBin);

				connectionPainter = new UIPainter (connectionBin, mpq);

				// notify we're ready to roll
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
			}
			catch (Exception e) {
				Console.WriteLine ("Connection screen resource loader failed: {0}", e);
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (Events.QuitApplication)));
			}
		}

		protected override void FinishedLoading ()
		{
			Background = background;
			Cursor = cursor;
			UI = connectionBin;
			UIPainter = connectionPainter;

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
