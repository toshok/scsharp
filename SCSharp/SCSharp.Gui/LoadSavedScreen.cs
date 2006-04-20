using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
{
	public class LoadSavedScreen : UIScreen
	{
		public LoadSavedScreen (Mpq mpq) : base (mpq, "glue\\PalNl", Builtins.rez_GluLoadBin)
		{
		}

		const int SAVEDGAME_LISTBOX_ELEMENT_INDEX = 5;
		const int OK_ELEMENT_INDEX = 6;
		const int CANCEL_ELEMENT_INDEX = 7;
		const int DELETE_ELEMENT_INDEX = 8;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (UIScreenType.RaceSelection);
				};
		}
	}
}
