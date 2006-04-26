//
// SCSharp.UI.LoginScreen
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
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class LoginScreen : UIScreen
	{
		public LoginScreen (Mpq mpq) : base (mpq, "glue\\PalNl", Builtins.rez_GluLoginBin)
		{
		}

		const int OK_ELEMENT_INDEX = 4;
		const int CANCEL_ELEMENT_INDEX = 5;
		const int NEW_ELEMENT_INDEX = 6;
		const int DELETE_ELEMENT_INDEX = 7;
		const int LISTBOX_ELEMENT_INDEX = 8;

		ListBoxElement listbox;

		string spcdir;
		string[] files;

		void PopulateUIFromDir ()
		{
			files = Directory.GetFiles (spcdir, "*.spc");
		
			for (int i = 0; i < files.Length; i ++)
				listbox.AddItem (Path.GetFileNameWithoutExtension (files[i]));

			listbox.SelectedIndex = 0;
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[OK_ELEMENT_INDEX].Activate +=
				delegate () {
					if (listbox.SelectedIndex == -1)
						return;

					Game.Instance.SwitchToScreen (UIScreenType.RaceSelection);
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (UIScreenType.MainMenu);
				};

			Elements[NEW_ELEMENT_INDEX].Activate +=
				delegate () {
					EntryDialog d = new EntryDialog (this, mpq,
									 GlobalResources.Instance.GluAllTbl.Strings[22]);
					d.Cancel += delegate () { DismissDialog (); };
					d.Ok += delegate () {
						if (listbox.Contains (d.Value)) {
							NameAlreadyExists (d);
						}
						else {
							DismissDialog ();
							listbox.AddItem (d.Value);
						}
					};
					ShowDialog (d);
				};

			Elements[DELETE_ELEMENT_INDEX].Activate +=
				delegate () {
					OkCancelDialog okd = new OkCancelDialog (this, mpq,
										 GlobalResources.Instance.GluAllTbl.Strings[23]);
					okd.Cancel += delegate () { DismissDialog (); };
					okd.Ok += delegate () {
						DismissDialog ();
						/* actually delete the file */
						listbox.RemoveAt (listbox.SelectedIndex);
					};
					ShowDialog (okd);
				};

			listbox = (ListBoxElement)Elements[LISTBOX_ELEMENT_INDEX];

			spcdir = Path.Combine (Game.Instance.RootDirectory, "characters");

			PopulateUIFromDir ();
		}

		public override void KeyboardDown (KeyboardEventArgs args)
		{
			if (args.Key == Key.DownArrow
			    || args.Key == Key.UpArrow) {
				listbox.KeyboardDown (args);
			}
			else
				base.KeyboardDown (args);
		}

		void NameAlreadyExists (EntryDialog d)
		{
			OkDialog okd = new OkDialog (d, mpq,
						     GlobalResources.Instance.GluAllTbl.Strings[24]);
			d.ShowDialog (okd);
		}
	}
}
