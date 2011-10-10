//
// SCSharp.UI.Unit
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
using System.Collections.Generic;

using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.CoreAnimation;
using System.Drawing;

using SCSharp;

namespace SCSharpMac.UI
{
	public class Unit
	{
		int unit_id;
		UnitsDat units;

		uint hitpoints;
		uint shields;

		int x;
		int y;
		ushort width, height;

		Sprite sprite;

		public Unit (int unit_id)
		{
			this.unit_id = unit_id;
			units = GlobalResources.Instance.UnitsDat;

			hitpoints = units.Hitpoints [unit_id];
			shields = units.Shields [unit_id];
			width = units.Widths [unit_id];
			height = units.Heights [unit_id];

			Console.WriteLine ("compaiidle for unit {0} = {1}", unit_id , units.CompAIIdles[unit_id]);
		}

		public Unit (UnitInfo info) : this (info.unit_id)
		{
			x = info.x;
			y = info.y;
		}

		public void Move (MapRenderer mapRenderer, int goal_minitile_x, int goal_minitile_y)
		{
			if (false /*flying unit*/) {
				// easier case, take the direct route
			}
			else {
				// Console.WriteLine ("FindPath from {0},{1} -> {2},{3}",
				// 		   x >> 2, y >> 2,
				// 		   goal_minitile_x, goal_minitile_y);

				navigateDestination = new MapPoint (goal_minitile_x, goal_minitile_y);

				AStarSolver astar = new AStarSolver (mapRenderer);

				navigatePath = astar.FindPath (new MapPoint (x >> 2, y >> 2),
							       navigateDestination);

				sprite.Debug = true;

				if (navigatePath != null)
					NavigateAlongPath ();
			}
		}

		int totalElapsed;
		int millisDelay = 150;
		List<MapPoint> navigatePath;
		MapPoint navigateDestination;

		MapPoint startCurrentSegment;
		MapPoint endCurrentSegment;


		int dest_pixel_x, dest_pixel_y;
		int pixel_x, pixel_y;
		int delta_x, delta_y;

		int ClassifyDirection (MapPoint startCurrentSegment, MapPoint endCurrentSegment)
		{
			if (startCurrentSegment.X < endCurrentSegment.X) {
				if (startCurrentSegment.Y < endCurrentSegment.Y) {
					Console.WriteLine ("1 startCurrentSegment.Y < endCurrentSegment.Y");
					return 5;
				}
				else if (startCurrentSegment.Y == endCurrentSegment.Y) {
					Console.WriteLine ("face = 12");
					return 12;
				}
				else {
					Console.WriteLine ("1 startCurrentSegment.Y > endCurrentSegment.Y");
					return 2;
				}
			}
			else if (startCurrentSegment.X == endCurrentSegment.X) {
				if (startCurrentSegment.Y < endCurrentSegment.Y) {
					Console.WriteLine ("2 startCurrentSegment.Y < endCurrentSegment.Y");
					return 7;
				}
				else if (startCurrentSegment.Y > endCurrentSegment.Y) {
					Console.WriteLine ("2 startCurrentSegment.Y > endCurrentSegment.Y");
					return 0;
				}
				else {
					Console.WriteLine ("@#(*@!#&( shouldn't happen");
					return 0;
				}
			}
			else {
				if (startCurrentSegment.Y < endCurrentSegment.Y) {
					Console.WriteLine ("3 startCurrentSegment.Y < endCurrentSegment.Y");
					return 9;
				}
				else if (startCurrentSegment.Y == endCurrentSegment.Y) {
					Console.WriteLine ("face = 4");
					return 4;
				}
				else {
					Console.WriteLine ("3 startCurrentSegment.Y > endCurrentSegment.Y");
					return 0;
				}
			}
		}

		void NavigateTick (object sender, TickEventArgs e)
		{
//			totalElapsed += e.TicksElapsed;

			if (totalElapsed < millisDelay)
				return;

			sprite.Invalidate ();
			pixel_x += delta_x;
			pixel_y += delta_y;
			if (dest_pixel_x - pixel_x < 2)
				pixel_x = dest_pixel_x;
			if (dest_pixel_y - pixel_y < 2)
				pixel_y = dest_pixel_y;
			sprite.SetPosition (pixel_x, pixel_y);
			sprite.Invalidate ();

			x = pixel_x << 2;
			y = pixel_y << 2;

			if (pixel_x == dest_pixel_x && pixel_y == dest_pixel_y) {
				startCurrentSegment = endCurrentSegment;
				navigatePath.RemoveAt (0);

				// if we're at the destination, remove the tick handler
				if (navigatePath.Count == 0) {
					sprite.RunScript (AnimationType.WalkingToIdle);
					Game.Instance.Tick -= NavigateTick;
				}
				else {
					endCurrentSegment = navigatePath[0];

					sprite.Face (ClassifyDirection (startCurrentSegment, endCurrentSegment));

					dest_pixel_x = endCurrentSegment.X * 4 + 4;
					dest_pixel_y = endCurrentSegment.Y * 4 + 4;

					delta_x = dest_pixel_x - pixel_x;
					delta_y = dest_pixel_y - pixel_y;
				}
			}
		}

		void NavigateAlongPath ()
		{
			int sprite_x, sprite_y;

			sprite.GetPosition (out sprite_x, out sprite_y);

			Console.WriteLine ("starting pixel position = {0},{1}", sprite_x, sprite_y);

			startCurrentSegment = navigatePath[0];
			navigatePath.RemoveAt (0);
			endCurrentSegment = navigatePath[0];

			sprite.Face (ClassifyDirection (startCurrentSegment, endCurrentSegment));

			int start_pixel_x = X * 4 + 4;
			int start_pixel_y = X * 4 + 4;

			dest_pixel_x = endCurrentSegment.X * 4 + 4;
			dest_pixel_y = endCurrentSegment.Y * 4 + 4;

			delta_x = dest_pixel_x - start_pixel_x;
			delta_y = dest_pixel_y - start_pixel_y;

			pixel_x = start_pixel_x;
			pixel_y = start_pixel_y;

			sprite.RunScript (AnimationType.Walking);

			Game.Instance.Tick += NavigateTick;
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
			get {
				if (unit_id >= units.Portraits.Count)
					return null;
				int idx = (int)units.Portraits [unit_id];
				if (idx >= GlobalResources.Instance.PortDataDat.PortraitIndexes.Count)
					return null;
				int portidx = (int)GlobalResources.Instance.PortDataDat.PortraitIndexes [idx];
				if (portidx >= GlobalResources.Instance.PortDataTbl.Count)
					return null;
				return GlobalResources.Instance.PortDataTbl[portidx];
			}
		}

		public string YesSound {
			get {
				if (unit_id >= units.YesSoundStarts.Count || unit_id >= units.YesSoundEnds.Count)
					return null;

				int start_idx = (int)units.YesSoundStarts [unit_id];
				int end_idx = (int)units.YesSoundEnds [unit_id];

				Console.WriteLine ("start_idx = {0}, end_idx = {1}", start_idx, end_idx);

				if (end_idx >= GlobalResources.Instance.SfxDataDat.FileIndexes.Count)
					return null;
				int sfxidx = (int)GlobalResources.Instance.SfxDataDat.FileIndexes[end_idx-1];
				if (sfxidx >= GlobalResources.Instance.SfxDataTbl.Count)
					return null;
				return GlobalResources.Instance.SfxDataTbl[sfxidx];

			}
		}

		public string WhatSound {
			get {
				if (unit_id >= units.WhatSoundStarts.Count)
					return null;

				int idx = (int)units.WhatSoundStarts [unit_id];
				if (idx >= GlobalResources.Instance.SfxDataDat.FileIndexes.Count)
					return null;
				int sfxidx = (int)GlobalResources.Instance.SfxDataDat.FileIndexes[idx];
				if (sfxidx >= GlobalResources.Instance.SfxDataTbl.Count)
					return null;
				return GlobalResources.Instance.SfxDataTbl[sfxidx];

			}
		}

		public int Width {
			get { return units.Widths [unit_id]; }
		}

		public int Height {
			get { return units.Heights [unit_id]; }
		}
	}
}
