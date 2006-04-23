using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class GameModeDialog : UIDialog
	{
		public GameModeDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_GluGameModeBin)
		{
			background_path = "glue\\Palmm\\retail_ex.pcx";
			background_translucent = 42;
			background_transparent = 0;
		}

		const int ORIGINAL_ELEMENT_INDEX = 1;
		const int TITLE_ELEMENT_INDEX = 2;
		const int EXPANSION_ELEMENT_INDEX = 3;
		const int CANCEL_ELEMENT_INDEX = 4;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++) {
				Console.WriteLine ("{0}: {1}", i, Elements[i].Text);
			}

			Elements[TITLE_ELEMENT_INDEX].Text = GlobalResources.Instance.GluAllTbl.Strings[172];

			Elements[ORIGINAL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Activate != null)
						Activate (false);
				};

			Elements[EXPANSION_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Activate != null)
						Activate (true);
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Cancel != null)
						Cancel ();
				};
		}

		public event DialogEvent Cancel;
		public event GameModeActivateDelegate Activate;
	}

	public delegate void GameModeActivateDelegate (bool expansion);
}
