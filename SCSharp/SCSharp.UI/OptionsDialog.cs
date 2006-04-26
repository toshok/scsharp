//
// SCSharp.UI.OptionsPainter
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
	public class SoundDialog : UIDialog
	{
		public SoundDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_SndDlgBin)
		{
			background_path = null;
		}

		const int OK_ELEMENT_INDEX = 1;
		const int CANCEL_ELEMENT_INDEX = 2;

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

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Cancel != null)
						Cancel ();
				};
		}

		public event DialogEvent Ok;
		public event DialogEvent Cancel;
	}

	public class SpeedDialog : UIDialog
	{
		public SpeedDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_SpdDlgBin)
		{
			background_path = null;
		}

		const int OK_ELEMENT_INDEX = 1;
		const int CANCEL_ELEMENT_INDEX = 2;

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

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Cancel != null)
						Cancel ();
				};
		}

		public event DialogEvent Ok;
		public event DialogEvent Cancel;
	}

	public class VideoDialog : UIDialog
	{
		public VideoDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_VideoBin)
		{
			background_path = null;
		}

		const int OK_ELEMENT_INDEX = 1;
		const int CANCEL_ELEMENT_INDEX = 2;

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

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Cancel != null)
						Cancel ();
				};
		}

		public event DialogEvent Ok;
		public event DialogEvent Cancel;
	}

	public class NetworkDialog : UIDialog
	{
		public NetworkDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_NetDlgBin)
		{
			background_path = null;
		}

		const int OK_ELEMENT_INDEX = 1;
		const int CANCEL_ELEMENT_INDEX = 2;

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

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Cancel != null)
						Cancel ();
				};
		}

		public event DialogEvent Ok;
		public event DialogEvent Cancel;
	}

	public class OptionsDialog : UIDialog
	{
		public OptionsDialog (UIScreen parent, Mpq mpq)
			: base (parent, mpq, "glue\\Palmm", Builtins.rez_OptionsBin)
		{
			background_path = null;
		}

		const int PREVIOUS_ELEMENT_INDEX = 1;
		const int SPEED_ELEMENT_INDEX = 2;
		const int SOUND_ELEMENT_INDEX = 3;
		const int VIDEO_ELEMENT_INDEX = 4;
		const int NETWORK_ELEMENT_INDEX = 5;

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			Elements[SOUND_ELEMENT_INDEX].Activate +=
				delegate () {
					SoundDialog d = new SoundDialog (this, mpq);
					d.Ok += delegate () { DismissDialog (); };
					d.Cancel += delegate () { DismissDialog (); };
					ShowDialog (d);
				};

			Elements[SPEED_ELEMENT_INDEX].Activate +=
				delegate () {
					SpeedDialog d = new SpeedDialog (this, mpq);
					d.Ok += delegate () { DismissDialog (); };
					d.Cancel += delegate () { DismissDialog (); };
					ShowDialog (d);
				};

			Elements[VIDEO_ELEMENT_INDEX].Activate +=
				delegate () {
					VideoDialog d = new VideoDialog (this, mpq);
					d.Ok += delegate () { DismissDialog (); };
					d.Cancel += delegate () { DismissDialog (); };
					ShowDialog (d);
				};

			Elements[NETWORK_ELEMENT_INDEX].Activate +=
				delegate () {
					NetworkDialog d = new NetworkDialog (this, mpq);
					d.Ok += delegate () { DismissDialog (); };
					d.Cancel += delegate () { DismissDialog (); };
					ShowDialog (d);
				};

			Elements[PREVIOUS_ELEMENT_INDEX].Activate +=
				delegate () {
					if (Previous != null)
						Previous ();
				};

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);
		}

		public event DialogEvent Previous;
	}
}
