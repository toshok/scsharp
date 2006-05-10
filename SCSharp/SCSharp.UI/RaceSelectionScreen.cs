//
// SCSharp.UI.RaceSelectionScreen
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
	public class RaceSelectionScreen : UIScreen
	{
		public RaceSelectionScreen (Mpq mpq) : base (mpq, "glue\\PalNl", Builtins.rez_GluExpcmpgnBin)
		{
			background_path = null;
		}

		const int LOADSAVED_ELEMENT_INDEX = 3;
		const int ZERG_ELEMENT_INDEX = 4;
		const int PROTOSS_ELEMENT_INDEX = 5;
		const int TERRAN_ELEMENT_INDEX = 6;
		const int CANCEL_ELEMENT_INDEX = 7;
		const int PLAYCUSTOM_ELEMENT_INDEX = 8;
		const int TERRAN_PROTOSS_INCOMPLETE_INDEX = 9;
		const int ZERG_PROTOSS_INCOMPLETE_INDEX = 10;
		const int ZERG_TERRAN_INCOMPLETE_INDEX = 11;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[ZERG_ELEMENT_INDEX].MouseEnterEvent += 
				delegate () {
					if (true /* XXX this should come from the player's file */) {
						Elements[ZERG_PROTOSS_INCOMPLETE_INDEX].Visible = true;
					}
				};

			Elements[ZERG_ELEMENT_INDEX].MouseLeaveEvent += 
				delegate () {
					if (true /* XXX this should come from the player's file */) {
						Elements[ZERG_PROTOSS_INCOMPLETE_INDEX].Visible = false;
					}
				};

			Elements[TERRAN_ELEMENT_INDEX].MouseEnterEvent += 
				delegate () {
					if (true /* XXX this should come from the player's file */) {
						Elements[TERRAN_PROTOSS_INCOMPLETE_INDEX].Visible = true;
					}
				};

			Elements[TERRAN_ELEMENT_INDEX].MouseLeaveEvent += 
				delegate () {
					if (true /* XXX this should come from the player's file */) {
						Elements[TERRAN_PROTOSS_INCOMPLETE_INDEX].Visible = false;
					}
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (UIScreenType.Login);
				};

			Elements[LOADSAVED_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (new LoadSavedScreen (mpq));
				};

			Elements[PLAYCUSTOM_ELEMENT_INDEX].Activate += 
				delegate () {
					Game.Instance.SwitchToScreen (new PlayCustomScreen (mpq));
				};
		}
	}
}
