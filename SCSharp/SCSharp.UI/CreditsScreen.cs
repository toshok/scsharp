using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class CreditsScreen : MarkupScreen
	{
		public CreditsScreen (Mpq mpq) : base (mpq)
		{
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			AddMarkup (Assembly.GetExecutingAssembly().GetManifestResourceStream ("credits.txt"));

			/* broodwar credits */
			if (Game.Instance.IsBroodWar)
				AddMarkup ((Stream)mpq.GetResource (Builtins.RezCrdtexpTxt));

			/* starcraft credits */
			AddMarkup ((Stream)mpq.GetResource (Builtins.RezCrdtlistTxt));
		}

		protected override void MarkupFinished ()
		{
			Game.Instance.SwitchToScreen (UIScreenType.MainMenu);
		}
	}
}
