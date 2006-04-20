using System;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
{
	public class EntryDialog : UIDialog
	{
		string title;

		public EntryDialog (UIScreen parent, Mpq mpq, string title)
			: base (parent, mpq, "glue\\PalNl", Builtins.rez_GluPEditBin)
		{
			background_path = "glue\\PalNl\\pEPopup.pcx";
			this.title = title;
		}

		const int OK_ELEMENT_INDEX = 1;
		const int TITLE_ELEMENT_INDEX = 2;
		const int CANCEL_ELEMENT_INDEX = 3;
		const int ENTRY_ELEMENT_INDEX = 4;

		TextBoxElement entry;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Console.WriteLine ("entry element is {0}", Elements[ENTRY_ELEMENT_INDEX].Type);

			Elements[TITLE_ELEMENT_INDEX].Text = title;

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

			entry = (TextBoxElement)Elements[ENTRY_ELEMENT_INDEX];

			Elements[OK_ELEMENT_INDEX].Sensitive = false;
		}

		public override void KeyboardDown (KeyboardEventArgs args)
		{
			if ((args.Mod & (ModifierKeys.LeftAlt | ModifierKeys.RightAlt)) != 0
			    || args.Key == Key.Return
			    || args.Key == Key.Escape) {

				base.KeyboardDown (args);
				return;
			}

			entry.KeyboardDown (args);

			Elements[OK_ELEMENT_INDEX].Sensitive = (entry.Value.Length > 0);
		}

		public string Value {
			get { return entry.Value; }
		}

		public event DialogEvent Cancel;
		public event DialogEvent Ok;
	}
}
