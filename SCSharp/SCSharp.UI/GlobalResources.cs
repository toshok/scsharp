//
// SCSharp.UI.GlobalResources
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

namespace SCSharp.UI
{
	public class GlobalResources
	{
		Mpq mpq;

		IScriptBin iscriptBin;
		ImagesDat imagesDat;
		SpritesDat spritesDat;
		SfxDataDat sfxDataDat;
		UnitsDat unitsDat;
		FlingyDat flingyDat;

		Tbl imagesTbl;
		Tbl sfxDataTbl;
		Tbl spritesTbl;
		Tbl gluAllTbl;

		static GlobalResources instance;
		public static GlobalResources Instance {
			get { return instance; }
		}

		public GlobalResources (Mpq mpq)
		{
			if (instance != null)
				throw new Exception ("There can only be one GlobalResources");

			this.mpq = mpq;
			instance = this;
		}

		public void Load ()
		{
			ThreadPool.QueueUserWorkItem (ResourceLoader);
		}

		public void LoadSingleThreaded ()
		{
			ResourceLoader (null);
		}

		public Tbl ImagesTbl {
			get { return imagesTbl; }
		}

		public Tbl SfxDataTbl {
			get { return sfxDataTbl; }
		}

		public Tbl SpritesTbl {
			get { return spritesTbl; }
		}

		public Tbl GluAllTbl {
			get { return gluAllTbl; }
		}

		public ImagesDat ImagesDat {
			get { return imagesDat; }
		}

		public SpritesDat SpritesDat {
			get { return spritesDat; }
		}

		public SfxDataDat SfxDataDat {
			get { return sfxDataDat; }
		}

		public IScriptBin IScriptBin {
			get { return iscriptBin; }
		}

		public UnitsDat UnitsDat {
			get { return unitsDat; }
		}

		public FlingyDat FlingyDat {
			get { return flingyDat; }
		}

		void ResourceLoader (object state)
		{
			try {
				imagesTbl = (Tbl)mpq.GetResource (Builtins.ImagesTbl);

				sfxDataTbl = (Tbl)mpq.GetResource (Builtins.SfxDataTbl);

				spritesTbl = (Tbl)mpq.GetResource (Builtins.SpritesTbl);

				gluAllTbl = (Tbl)mpq.GetResource (Builtins.rez_GluAllTbl);

				imagesDat = (ImagesDat)mpq.GetResource (Builtins.ImagesDat);

				sfxDataDat = (SfxDataDat)mpq.GetResource (Builtins.SfxDataDat);

				spritesDat = (SpritesDat)mpq.GetResource (Builtins.SpritesDat);

				iscriptBin = (IScriptBin)mpq.GetResource (Builtins.IScriptBin);

				unitsDat = (UnitsDat)mpq.GetResource (Builtins.UnitsDat);

				flingyDat = (FlingyDat)mpq.GetResource (Builtins.FlingyDat);

				// notify we're ready to roll
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
			}
			catch (Exception e) {
				Console.WriteLine ("Global Resource loader failed: {0}", e);
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (Events.QuitApplication)));
			}
		}

		void FinishedLoading ()
		{
			if (Ready != null)
				Ready ();
		}

		public event ReadyDelegate Ready;
	}
}
