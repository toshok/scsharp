//
// SCSharp.UI.GlobalResources
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
using System.Threading;

using SdlDotNet.Core;
using SdlDotNet.Graphics;


namespace SCSharp.UI
{
	public class Resources {
		public IScriptBin IscriptBin;
		public ImagesDat ImagesDat;
		public SpritesDat SpritesDat;
		public SfxDataDat SfxDataDat;
		public UnitsDat UnitsDat;
		public FlingyDat FlingyDat;
		public MapDataDat MapDataDat;
		public PortDataDat PortDataDat;

		public Tbl ImagesTbl;
		public Tbl SfxDataTbl;
		public Tbl SpritesTbl;
		public Tbl GluAllTbl;
		public Tbl MapDataTbl;
		public Tbl PortDataTbl;
	}

	public class GlobalResources
	{
		Mpq stardatMpq;
		Mpq broodatMpq;

		Resources starcraftResources;
		Resources broodwarResources;

		static GlobalResources instance;
		public static GlobalResources Instance {
			get { return instance; }
		}

		public GlobalResources (Mpq stardatMpq, Mpq broodatMpq)
		{
			if (instance != null)
				throw new Exception ("There can only be one GlobalResources");

			this.stardatMpq = stardatMpq;
			this.broodatMpq = broodatMpq;

			starcraftResources = new Resources();
			broodwarResources = new Resources();

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

		Resources Resources {
			get { return Game.Instance.PlayingBroodWar ? broodwarResources : starcraftResources; }
		}

		public Tbl ImagesTbl {
			get { return Resources.ImagesTbl; }
		}

		public Tbl SfxDataTbl {
			get { return Resources.SfxDataTbl; }
		}

		public Tbl SpritesTbl {
			get { return Resources.SpritesTbl; }
		}

		public Tbl GluAllTbl {
			get { return Resources.GluAllTbl; }
		}

		public ImagesDat ImagesDat {
			get { return Resources.ImagesDat; }
		}

		public SpritesDat SpritesDat {
			get { return Resources.SpritesDat; }
		}

		public SfxDataDat SfxDataDat {
			get { return Resources.SfxDataDat; }
		}

		public IScriptBin IScriptBin {
			get { return Resources.IscriptBin; }
		}

		public UnitsDat UnitsDat {
			get { return Resources.UnitsDat; }
		}

		public FlingyDat FlingyDat {
			get { return Resources.FlingyDat; }
		}

		public MapDataDat MapDataDat {
			get { return Resources.MapDataDat; }
		}

		public PortDataDat PortDataDat {
			get { return Resources.PortDataDat; }
		}

		public Tbl MapDataTbl {
			get { return Resources.MapDataTbl; }
		}

		public Tbl PortDataTbl {
			get { return Resources.PortDataTbl; }
		}

		public Resources StarDat {
			get { return starcraftResources; }
		}

		public Resources BrooDat {
			get { return broodwarResources; }
		}

		void ResourceLoader (object state)
		{
			try {
				starcraftResources.ImagesTbl = (Tbl)stardatMpq.GetResource (Builtins.ImagesTbl);
				starcraftResources.SfxDataTbl = (Tbl)stardatMpq.GetResource (Builtins.SfxDataTbl);
				starcraftResources.SpritesTbl = (Tbl)stardatMpq.GetResource (Builtins.SpritesTbl);
				starcraftResources.GluAllTbl = (Tbl)stardatMpq.GetResource (Builtins.rez_GluAllTbl);
				starcraftResources.MapDataTbl = (Tbl)stardatMpq.GetResource (Builtins.MapDataTbl);
				starcraftResources.PortDataTbl = (Tbl)stardatMpq.GetResource (Builtins.PortDataTbl);
				starcraftResources.ImagesDat = (ImagesDat)stardatMpq.GetResource (Builtins.ImagesDat);
				starcraftResources.SfxDataDat = (SfxDataDat)stardatMpq.GetResource (Builtins.SfxDataDat);
				starcraftResources.SpritesDat = (SpritesDat)stardatMpq.GetResource (Builtins.SpritesDat);
				starcraftResources.IscriptBin = (IScriptBin)stardatMpq.GetResource (Builtins.IScriptBin);
				starcraftResources.UnitsDat = (UnitsDat)stardatMpq.GetResource (Builtins.UnitsDat);
				starcraftResources.FlingyDat = (FlingyDat)stardatMpq.GetResource (Builtins.FlingyDat);
				starcraftResources.MapDataDat = (MapDataDat)stardatMpq.GetResource (Builtins.MapDataDat);
				starcraftResources.PortDataDat = (PortDataDat)stardatMpq.GetResource (Builtins.PortDataDat);

				if (broodatMpq != null) {
					broodwarResources.ImagesTbl = (Tbl)broodatMpq.GetResource (Builtins.ImagesTbl);
					broodwarResources.SfxDataTbl = (Tbl)broodatMpq.GetResource (Builtins.SfxDataTbl);
					broodwarResources.SpritesTbl = (Tbl)broodatMpq.GetResource (Builtins.SpritesTbl);
					broodwarResources.GluAllTbl = (Tbl)broodatMpq.GetResource (Builtins.rez_GluAllTbl);
					broodwarResources.MapDataTbl = (Tbl)broodatMpq.GetResource (Builtins.MapDataTbl);
					broodwarResources.PortDataTbl = (Tbl)broodatMpq.GetResource (Builtins.PortDataTbl);
					broodwarResources.ImagesDat = (ImagesDat)broodatMpq.GetResource (Builtins.ImagesDat);
					broodwarResources.SfxDataDat = (SfxDataDat)broodatMpq.GetResource (Builtins.SfxDataDat);
					broodwarResources.SpritesDat = (SpritesDat)broodatMpq.GetResource (Builtins.SpritesDat);
					broodwarResources.IscriptBin = (IScriptBin)broodatMpq.GetResource (Builtins.IScriptBin);
					broodwarResources.UnitsDat = (UnitsDat)broodatMpq.GetResource (Builtins.UnitsDat);
					broodwarResources.FlingyDat = (FlingyDat)broodatMpq.GetResource (Builtins.FlingyDat);
					broodwarResources.MapDataDat = (MapDataDat)broodatMpq.GetResource (Builtins.MapDataDat);
					broodwarResources.PortDataDat = (PortDataDat)broodatMpq.GetResource (Builtins.PortDataDat);
				}

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
