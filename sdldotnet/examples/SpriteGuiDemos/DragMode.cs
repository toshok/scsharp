/*
 * $RCSfile: DragMode.cs,v $
 * Copyright (C) 2004 D. R. E. Moonfire (d.moonfire@mfgames.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */


using SdlDotNet;
using SdlDotNet.Sprites;
using System.Collections;
using System.Drawing;
using System.Threading;

namespace SdlDotNet.Examples.SpriteGuiDemos
{
	/// <summary>
	/// 
	/// </summary>
	public class DragMode : DemoMode
	{
		/// <summary>
		/// Constructs the internal sprites needed for our demo.
		/// </summary>
		public DragMode()
		{
			// Create the fragment marbles
			int rows = 5;
			int cols = 5;
			int sx = (800 - cols * 50) / 2;
			int sy = (600 - rows * 50) / 2;
			SurfaceCollection m1 = LoadMarble("marble1");
			SurfaceCollection m2 = LoadMarble("marble2");
			Animation anim1 = new Animation(m1);
						
			Animation anim2 = new Animation(m2);

			Hashtable frames = new Hashtable();
			frames.Add("marble1", m1);
			frames.Add("marble2", m2);

			DragSprite dragSprite;
			for (int i = 0; i < cols; i++)
			{
				Thread.Sleep(10);
				for (int j = 0; j < rows; j++)
				{
					dragSprite = new DragSprite(frames, "marble1",
						new Point(sx + i * 50, sy + j * 50),
						new Rectangle(new Point(0, 0), SdlDemo.Size));
					dragSprite.Animations.Add("marble1", anim1);
					dragSprite.Animations.Add("marble2", anim2);

					Thread.Sleep(10);
					Sprites.Add(dragSprite);
				}
			}
			Sprites.EnableMouseButtonEvent();
			Sprites.EnableMouseMotionEvent();
			Sprites.EnableTickEvent();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString() { return "Drag"; }
	}
}
