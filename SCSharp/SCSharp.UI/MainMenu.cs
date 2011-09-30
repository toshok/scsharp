//
// SCSharp.UI.MainMenu
//
// Authors:
//	Chris Toshok (toshok@gmail.com)
//
// Copyright 2006-2010 Chris Toshok
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
using System.Collections.Generic;
using System.IO;
using System.Threading;

using SdlDotNet.Core;
using SdlDotNet.Graphics;

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

		void ShowGameModeDialog (UIScreenType nextScreen)
		{
			GameModeDialog d = new GameModeDialog (this, mpq);
			d.Cancel += delegate () { DismissDialog (); };
			d.Activate += delegate (bool expansion) {
				DismissDialog ();
				try {
					Game.Instance.PlayingBroodWar = expansion;
					GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
					Game.Instance.SwitchToScreen (nextScreen);
				}
				catch (Exception e) {
					ShowDialog (new OkDialog (this, mpq, e.Message));
				}
			};
			ShowDialog (d);
		}

		List<UIElement> smkElements;
		UIPainter smkPainter;

		public override void AddToPainter ()
		{
			base.AddToPainter ();
			foreach (MovieElement el in smkElements)
				el.Play ();
			Painter.Add (Layer.Background, smkPainter.Paint);
		}

		public override void RemoveFromPainter ()
		{
			base.RemoveFromPainter ();
			foreach (MovieElement el in smkElements)
				el.Stop ();
			Painter.Remove (Layer.Background, smkPainter.Paint);
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}' : {3}", i, Elements[i].Type, Elements[i].Text, Elements[i].Flags);

			Elements[VERSION_ELEMENT_INDEX].Text = "v" + Consts.Version;

			Elements[SINGLEPLAYER_ELEMENT_INDEX].Flags |= ElementFlags.RightAlignText | ElementFlags.CenterTextVert;

			Elements[SINGLEPLAYER_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Game.Instance.IsBroodWar) {
						ShowGameModeDialog (UIScreenType.Login);
					}
					else {
						GuiUtil.PlaySound (mpq, Builtins.Mousedown2Wav);
						Game.Instance.SwitchToScreen (UIScreenType.Login);
					}
				};

			Elements[MULTIPLAYER_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Game.Instance.IsBroodWar) {
						ShowGameModeDialog (UIScreenType.Connection);
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
					Cinematic introScreen = new Cinematic (mpq,
									       Game.Instance.IsBroodWar
									       ? "smk\\starXIntr.smk"
									       : "smk\\starintr.smk");
					introScreen.Finished += delegate () {
						Game.Instance.SwitchToScreen (this);
					};
					Game.Instance.SwitchToScreen (introScreen);
				};

			Elements[CREDITS_ELEMENT_INDEX].Activate += 
				delegate () {
					Game.Instance.SwitchToScreen (new CreditsScreen (mpq));
				};

			Elements[EXIT_ELEMENT_INDEX].Activate += 
				delegate () {
					Game.Instance.Quit ();
				};

			smkElements = new List<UIElement>();

			AddMovieElements (SINGLEPLAYER_ELEMENT_INDEX, "glue\\mainmenu\\Single.smk", "glue\\mainmenu\\SingleOn.smk", 50, 70, false);
			AddMovieElements (MULTIPLAYER_ELEMENT_INDEX, "glue\\mainmenu\\Multi.smk", "glue\\mainmenu\\MultiOn.smk", 20, 12, true);
			AddMovieElements (CAMPAIGNEDITOR_ELEMENT_INDEX, "glue\\mainmenu\\Editor.smk", "glue\\mainmenu\\EditorOn.smk", 20, 18, true);
			AddMovieElements (EXIT_ELEMENT_INDEX, "glue\\mainmenu\\Exit.smk", "glue\\mainmenu\\ExitOn.smk", 15, 0, true);

			smkPainter = new UIPainter (smkElements);
		}

		void AddMovieElements (int elementIndex, string normalMovie, string onMovie, ushort off_x, ushort off_y, bool on_movie_on_top)
		{
			UIElement normalElement, onElement;

			normalElement = new MovieElement (this,
							  Elements[elementIndex].BinElement,
							  Elements[elementIndex].Palette,
							  normalMovie);
			onElement = new MovieElement (this,
						      Elements[elementIndex].BinElement,
						      Elements[elementIndex].Palette,
						      onMovie);

			onElement.X1 += off_x;
			onElement.Y1 += off_y;

			if (!on_movie_on_top)
				smkElements.Add (onElement);
			smkElements.Add (normalElement);
			if (on_movie_on_top)
				smkElements.Add (onElement);
			onElement.Visible = false;

			Elements[elementIndex].MouseEnterEvent += 
				delegate () {
					onElement.Visible = true;
				};
			Elements[elementIndex].MouseLeaveEvent += 
				delegate () {
					onElement.Visible = false;
				};
		}


	}
}
