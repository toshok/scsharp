/*
 * $RCSfile: BounceMode.cs,v $
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

using SdlDotNet.Sprites;
using SdlDotNet;
using System.Drawing;
using System;
using System.Threading;

namespace SdlDotNet.Examples.SpriteGuiDemos
{
	/// <summary>
	/// 
	/// </summary>
	public class BounceMode : DemoMode
	{
		static Random rand = new Random();
		/// <summary>
		/// Constructs the internal sprites needed for our demo.
		/// </summary>
		public BounceMode()
		{
			// Create the fragment marbles
			Rectangle rect = new Rectangle(new Point(0, 0), SdlDemo.Size);
			for (int i = 0; i < 50; i++)
			{
				Thread.Sleep(10);
				SurfaceCollection d = LoadRandomMarble();
				BounceSprite bounceSprite = 
					new BounceSprite(d,
					new Rectangle(new Point(0,0), SdlDemo.Size), 
					new Point(rand.Next(rect.Left, rect.Right),
					rand.Next(rect.Top, rect.Bottom)));
				Sprites.Add(bounceSprite);
			}
			Sprites.EnableTickEvent();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString() { return "Bounce"; }
	}
}
