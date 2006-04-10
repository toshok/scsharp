using System;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
{
	public class OkCancelDialog : UIDialog
	{
		string message;

		public OkCancelDialog (UIScreen parent, Mpq mpq, string message)
			: base (parent, mpq, "glue\\PalNl", Builtins.rez_GluPOkCancelBin)
		{
			background_path = "glue\\PalNl\\pDPopup.pcx";
			this.message = message;
		}

		const int OK_ELEMENT_INDEX = 1;
		const int MESSAGE_ELEMENT_INDEX = 2;
		const int CANCEL_ELEMENT_INDEX = 3;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Elements[MESSAGE_ELEMENT_INDEX].Text = message;

			Elements[OK_ELEMENT_INDEX].Activate += 
				delegate () {
					if (Ok != null)
						Ok ();
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate += 
				delegate () {
					if (Cancel != null)
						Cancel ();
				};

			// notify we're ready to roll
			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
		}

		public event DialogEvent Ok;
		public event DialogEvent Cancel;
	}
}
