using System;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace Starcraft
{
	public class OkDialog : UIDialog
	{
		string message;

		public OkDialog (UIScreen parent, Mpq mpq, string message)
			: base (parent, mpq, "glue\\PalNl", Builtins.rez_GluPOkBin)
		{
			background_path = "glue\\PalNl\\pOPopup.pcx";
			this.message = message;
		}

		const int OK_ELEMENT_INDEX = 1;
		const int MESSAGE_ELEMENT_INDEX = 2;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Elements[MESSAGE_ELEMENT_INDEX].Text = message;

			Elements[OK_ELEMENT_INDEX].Activate += 
				delegate () {
					if (Ok == null)
						Parent.DismissDialog ();
					else
						Ok ();
				};

			// notify we're ready to roll
			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
		}

		public event DialogEvent Ok;
	}
}
