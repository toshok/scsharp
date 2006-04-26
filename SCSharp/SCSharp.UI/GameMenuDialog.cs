//
// SCSharp.UI.GameMenuDialog
//
// Authors:
//	Chris Toshok (toshok@hungry.com)
//
// (C) 2006 The Hungry Programmers (http://www.hungry.com/)
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Text;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class GameMenuDialog : UIDialog
	{
		public GameMenuDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_GameMenuBin)
		{
			background_path = null;
		}

		const int RETURNTOGAME_ELEMENT_INDEX = 1;
		const int SAVEGAME_ELEMENT_INDEX = 2;
		const int LOADGAME_ELEMENT_INDEX = 3;
		const int PAUSEGAME_ELEMENT_INDEX = 4;
		const int RESUMEGAME_ELEMENT_INDEX = 5;
		const int OPTIONS_ELEMENT_INDEX = 6;
		const int HELP_ELEMENT_INDEX = 7;
		const int MISSIONOBJECTIVES_ELEMENT_INDEX = 8;
		const int ENDMISSION_ELEMENT_INDEX = 9;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[RETURNTOGAME_ELEMENT_INDEX].Activate +=
				delegate () {
					if (ReturnToGame != null)
						ReturnToGame ();
				};

			Elements[MISSIONOBJECTIVES_ELEMENT_INDEX].Activate +=
				delegate () {
					ObjectivesDialog d = new ObjectivesDialog (this, mpq);
					d.Previous += delegate () { DismissDialog (); };
					ShowDialog (d);
				};

			Elements[ENDMISSION_ELEMENT_INDEX].Activate +=
				delegate () {
					EndMissionDialog d = new EndMissionDialog (this, mpq);
					d.Previous += delegate () { DismissDialog (); };
#if false
					d.Quit += delegate () { DismissDialog ();
								/* XXX hack just to get things looking right */
								parent.DismissDialog ();
								Game.Instance.SwitchToScreen (UIScreenType.MainMenu); };
#endif
					ShowDialog (d);
				};

			Elements[OPTIONS_ELEMENT_INDEX].Activate += 
				delegate () {
					OptionsDialog d = new OptionsDialog (this, mpq);
					d.Previous += delegate () { DismissDialog (); };
					ShowDialog (d);
				};

			Elements[HELP_ELEMENT_INDEX].Activate +=
				delegate () {
					HelpDialog d = new HelpDialog (this, mpq);
					d.Previous += delegate () { DismissDialog (); };
					ShowDialog (d);
				};
		}

		public event DialogEvent ReturnToGame;
	}

	public class ObjectivesDialog : UIDialog
	{
		public ObjectivesDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_ObjctDlgBin)
		{
			background_path = null;
		}

		const int PREVIOUS_ELEMENT_INDEX = 1;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[PREVIOUS_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Previous != null)
						Previous ();
				};
		}

		public event DialogEvent Previous;
	}

	public class EndMissionDialog : UIDialog
	{
		public EndMissionDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_AbrtMenuBin)
		{
			background_path = null;
		}

		const int PREVIOUS_ELEMENT_INDEX = 1;
		const int RESTARTMISSION_ELEMENT_INDEX = 2;
		const int QUITMISSION_ELEMENT_INDEX = 3;
		const int EXITPROGRAM_ELEMENT_INDEX = 4;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[QUITMISSION_ELEMENT_INDEX].Activate +=
				delegate () {
					QuitMissionDialog d = new QuitMissionDialog (this, mpq);
					d.Cancel += delegate () { DismissDialog (); };
					ShowDialog (d);
				};

			Elements[EXITPROGRAM_ELEMENT_INDEX].Activate +=
				delegate () {
					ExitConfirmationDialog d = new ExitConfirmationDialog (this, mpq);
					d.Cancel += delegate () { DismissDialog (); };
					ShowDialog (d);
				};

			Elements[RESTARTMISSION_ELEMENT_INDEX].Activate += 
				delegate () {
					RestartConfirmationDialog d = new RestartConfirmationDialog (this, mpq);
					d.Cancel += delegate () { DismissDialog (); };
					ShowDialog (d);
				};

			Elements[PREVIOUS_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Previous != null)
						Previous ();
				};
		}

		public event DialogEvent Previous;
	}

	public class RestartConfirmationDialog : UIDialog
	{
		public RestartConfirmationDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_RestartBin)
		{
			background_path = null;
		}

		const int RESTART_ELEMENT_INDEX = 1;
		const int CANCEL_ELEMENT_INDEX = 2;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Cancel != null)
						Cancel ();
				};
		}

		public event DialogEvent Cancel;
	}

	public class QuitMissionDialog : UIDialog
	{
		public QuitMissionDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_Quit2MnuBin)
		{
			background_path = null;
		}

		const int QUIT_ELEMENT_INDEX = 1;
		const int OBSERVE_ELEMENT_INDEX = 2;
		const int CANCEL_ELEMENT_INDEX = 3;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[QUIT_ELEMENT_INDEX].Activate +=
				delegate () {
					//QuitConfirmationDialog d = new QuitConfirmationDialog (this, mpq);
					//					ShowDialog (d);
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Cancel != null)
						Cancel ();
				};
		}

		public event DialogEvent Cancel;
	}

	public class ExitConfirmationDialog : UIDialog
	{
		public ExitConfirmationDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_QuitBin)
		{
			background_path = null;
		}

		const int EXIT_ELEMENT_INDEX = 1;
		const int CANCEL_ELEMENT_INDEX = 2;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[EXIT_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Exit != null)
						Exit ();
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Cancel != null)
						Cancel ();
				};
		}

		public event DialogEvent Exit;
		public event DialogEvent Cancel;
	}

	public class HelpDialog : UIDialog
	{
		public HelpDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_HelpMenuBin)
		{
			background_path = null;
		}

		const int PREVIOUS_ELEMENT_INDEX = 1;
		const int KEYSTROKE_ELEMENT_INDEX = 2;
		const int TIPS_INDEX = 3;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[PREVIOUS_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Previous != null)
						Previous ();
				};

			Elements[KEYSTROKE_ELEMENT_INDEX].Activate +=
				delegate () {
					KeystrokeDialog d = new KeystrokeDialog (this, mpq);
					d.Ok += delegate () { DismissDialog (); };
					ShowDialog (d);
				};
		}

		public event DialogEvent Previous;
	}

	public class KeystrokeDialog : UIDialog
	{
		public KeystrokeDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_HelpBin)
		{
			background_path = null;
		}

		const int OK_ELEMENT_INDEX = 1;
		const int HELPLIST_ELEMENT_INDEX = 2;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[OK_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Ok != null)
						Ok ();
				};

			ListBoxElement list = (ListBoxElement)Elements[HELPLIST_ELEMENT_INDEX];
			Tbl help_txt = (Tbl)mpq.GetResource (Builtins.rez_HelpTxtTbl);

			for (int i = 0; i < help_txt.Strings.Length; i++) {
				list.AddItem (help_txt.Strings[i]);
			}
		}

		public event DialogEvent Ok;
	}

}
