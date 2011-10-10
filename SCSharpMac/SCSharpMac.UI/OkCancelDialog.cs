//
// SCSharp.UI.OkCancelDialog
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
using System.IO;
using System.Text;
using System.Threading;

using MonoMac.CoreGraphics;
using MonoMac.CoreAnimation;

using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
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
		}

		public event DialogEvent Ok;
		public event DialogEvent Cancel;
	}
}
