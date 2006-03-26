using System;
using System.IO;
using System.Threading;

using Gtk;
using Gdk;

namespace Starcraft
{
	public class GlobalResources
	{
		Mpq mpq;

		IScriptBin iscriptBin;
		ImagesDat imagesDat;
		SpritesDat spritesDat;

		Tbl imagesTbl;
		Tbl spritesTbl;

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

		public ImagesDat ImagesDat {
			get { return imagesDat; }
		}

		public Tbl SpritesTbl {
			get { return spritesTbl; }
		}

		public IScriptBin IScriptBin {
			get { return iscriptBin; }
		}

		public SpritesDat SpritesDat {
			get { return spritesDat; }
		}

		void ResourceLoader (object state)
		{
			Console.WriteLine ("loading images.tbl");
			imagesTbl = (Tbl)mpq.GetResource (Builtins.ImagesTbl);
			Console.WriteLine ("loading images.dat");
			imagesDat = (ImagesDat)mpq.GetResource (Builtins.ImagesDat);

			Console.WriteLine ("loading sprites.tbl");
			spritesTbl = (Tbl)mpq.GetResource (Builtins.SpritesTbl);
			Console.WriteLine ("loading sprites.dat");
			spritesDat = (SpritesDat)mpq.GetResource (Builtins.SpritesDat);

			Console.WriteLine ("loading iscript.bin");
			iscriptBin = (IScriptBin)mpq.GetResource (Builtins.IScriptBin);

			// notify we're ready to roll
			(new ThreadNotify (new ReadyEvent (FinishedLoading))).WakeupMain ();
		}

		void FinishedLoading ()
		{
			if (Ready != null)
				Ready ();
		}

		public event ReadyDelegate Ready;
	}
}
