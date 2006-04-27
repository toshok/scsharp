//
// SCSharp.UI.MainMenu
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
	public class MainMenu : UIScreen
	{
		public MainMenu (Mpq mpq) : base (mpq, "glue\\Palmm", Builtins.rez_GluMainBin)
		{
		}

		const int EXIT_ELEMENT_INDEX = 2;
		const int SINGLEPLAYER_ELEMENT_INDEX = 3;
		const int MULTIPLAYER_ELEMENT_INDEX = 4;
		const int CAMPAIGNEDITOR_ELEMENT_INDEX = 5;
		const int INTRO_ELEMENT_INDEX = 8;
		const int CREDITS_ELEMENT_INDEX = 9;
		const int VERSION_ELEMENT_INDEX = 10;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Elements[VERSION_ELEMENT_INDEX].Text = "v0.0000004";

			Elements[SINGLEPLAYER_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Game.Instance.IsBroodWar) {
						GameModeDialog d = new GameModeDialog (this, mpq);
						d.Cancel += delegate () { DismissDialog (); };
						d.Activate += delegate (bool expansion) {
							DismissDialog ();
							Game.Instance.IsBroodWar = expansion;
							GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
							Game.Instance.SwitchToScreen (UIScreenType.Login);
						};
						ShowDialog (d);
					}
					else {
						GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
						Game.Instance.SwitchToScreen (UIScreenType.Login);
					}
				};

			Elements[MULTIPLAYER_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Game.Instance.IsBroodWar) {
						GameModeDialog d = new GameModeDialog (this, mpq);
						d.Cancel += delegate () { DismissDialog (); };
						d.Activate += delegate (bool expansion) {
							DismissDialog ();
							GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
							Game.Instance.SwitchToScreen (UIScreenType.Connection);
						};
						ShowDialog (d);
					}
					else {
						GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
						Game.Instance.SwitchToScreen (UIScreenType.Connection);
					}
				};

			Elements[CAMPAIGNEDITOR_ELEMENT_INDEX].Activate +=
				delegate () {
					OkDialog d = new OkDialog (this, mpq,
								   "The campaign editor functionality is not available in SCSharp");
					ShowDialog (d);
				};

			Elements[INTRO_ELEMENT_INDEX].Activate +=
				delegate () {
					OkDialog d = new OkDialog (this,
								   mpq,
								   "Cinematics are not available (yet) in SCSharp");
					ShowDialog (d);
				};

			Elements[CREDITS_ELEMENT_INDEX].Activate += 
				delegate () {
					Game.Instance.SwitchToScreen (new CreditsScreen (mpq));
				};

			Elements[EXIT_ELEMENT_INDEX].Activate += 
				delegate () {
					Game.Instance.Quit ();
				};
		}
	}
}
