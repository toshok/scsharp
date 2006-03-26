/*
 * $RCSfile$
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

using NUnit.Framework;
using SdlDotNet.Sprites;

namespace SdlDotNet.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture] 
	public class DrawableTest
	{
		/// <summary>
		/// 
		/// </summary>
		[Test] 
		public void TestImageLoad()
		{
			// Load the image
			SurfaceCollection id = new SurfaceCollection("../../../examples/SpriteGuiDemos/Data/marble1.png");
      
			// Make sure the height and width match
			Assert.AreEqual(384, id.Size.Width);
			Assert.AreEqual(384, id.Size.Height);
		}
	}
}
