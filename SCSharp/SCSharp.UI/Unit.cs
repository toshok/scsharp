//
// SCSharp.UI.Unit
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
using System.Collections.Generic;

using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class Unit
	{
		int unit_id;
		UnitsDat units;

		uint hitpoints;
		uint shields;

		int x;
		int y;

		Sprite sprite;

		public Unit (int unit_id)
		{
			this.unit_id = unit_id;
			units = GlobalResources.Instance.UnitsDat;

			hitpoints = units.Hitpoints [unit_id];
			shields = units.Shields [unit_id];
		}

		public Unit (UnitInfo info) : this (info.unit_id)
		{
			x = info.x;
			y = info.y;
		}

		public Sprite CreateSprite (Mpq mpq, byte[] palette)
		{
			if (sprite != null)
				throw new Exception ();

			sprite = SpriteManager.CreateSprite (mpq, SpriteId, palette, x, y);

			sprite.RunScript (AnimationType.Init);

			return sprite;
		}

		public int X {
			get { return x; }
			set { x = value; }
		}

		public int Y {
			get { return y; }
			set { y = value; }
		}

		public Sprite Sprite {
			get { return sprite; }
		}

		public int UnitId {
			get { return unit_id; }
		}

		public int FlingyId {
			get { return units.FlingyIds [unit_id]; }
		}

		public int SpriteId {
			get { return GlobalResources.Instance.FlingyDat.SpriteIds[FlingyId]; }
		}

		public uint ConstructSpriteId {
			get { return units.ConstructSpriteIds [unit_id]; }
		}

		public int AnimationLevel {
			get { return units.AnimationLevels [unit_id]; }
		}

		public uint HitPoints {
			get { return hitpoints; }
			set { hitpoints = value; }
		}

		public uint Shields {
			get { return shields; }
			set { shields = value; }
		}

		public int CreateScore {
			get { return units.CreateScores [unit_id]; }
		}

		public int DestroyScore {
			get { return units.DestroyScores [unit_id]; }
		}

		public int SelectionCircle {
			get { return GlobalResources.Instance.SpritesDat.SelectionCircles [SpriteId]; }
		}

		public int SelectionCircleOffset {
			get { return GlobalResources.Instance.SpritesDat.SelectionCircleOffsets [SpriteId]; }
		}

		public string Portrait {
			get { return GlobalResources.Instance.PortDataTbl[(int)GlobalResources.Instance.PortDataDat.PortraitIndexes [(int)units.Portraits [unit_id]]]; }
		}
	}
}
