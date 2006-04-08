using System;
using System.IO;
using System.Threading;

using SdlDotNet;

namespace Starcraft
{
	public class GlobalResources
	{
		Mpq mpq;

		IScriptBin iscriptBin;
		ImagesDat imagesDat;
		SpritesDat spritesDat;
		SfxDataDat sfxDataDat;

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

		void ResourceLoader (object state)
		{
			try {
				Console.WriteLine ("loading images.tbl");
				imagesTbl = (Tbl)mpq.GetResource (Builtins.ImagesTbl);

				Console.WriteLine ("loading sfxdata.tbl");
				sfxDataTbl = (Tbl)mpq.GetResource (Builtins.SfxDataTbl);

				Console.WriteLine ("loading sprites.tbl");
				spritesTbl = (Tbl)mpq.GetResource (Builtins.SpritesTbl);

				Console.WriteLine ("loading gluAll.tbl");
				gluAllTbl = (Tbl)mpq.GetResource (Builtins.rez_GluAllTbl);

				Console.WriteLine ("loading images.dat");
				imagesDat = (ImagesDat)mpq.GetResource (Builtins.ImagesDat);

				Console.WriteLine ("loading sfxdata.dat");
				sfxDataDat = (SfxDataDat)mpq.GetResource (Builtins.SfxDataDat);

				Console.WriteLine ("loading sprites.dat");
				spritesDat = (SpritesDat)mpq.GetResource (Builtins.SpritesDat);

				Console.WriteLine ("loading iscript.bin");
				iscriptBin = (IScriptBin)mpq.GetResource (Builtins.IScriptBin);

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
