//*****************************************************************************
//	This program is free software; you can redistribute it and/or
//	modify it under the terms of the GNU General Public License
//	as published by the Free Software Foundation; either version 2
//	of the License, or (at your option) any later version.
//	This program is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//	GNU General Public License for more details.
//	You should have received a copy of the GNU General Public License
//	along with this program; if not, write to the Free Software
//	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//	
//	Created by Michael Rosario
//	July 29th,2003
//	Contact me at mrosario@scrypt.net	
//*****************************************************************************



using System;
using NUnit.Framework;

namespace SdlDotNet.Examples.Triad
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class TriadTests
	{
		/// <summary>
		/// 
		/// </summary>
		public TriadTests()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GameObjectTests()
		{
			BouncingSquare square = new BouncingSquare();

			//Try to draw an object with an null surface...
			try
			{
				square.Draw(null);
				Assert.Fail("Surface is null.");
			}
			catch(GameException){}

			//Try to make the square hit a null object...
			square.Hits(null);


			//Try to set bad dimensions on game object...
			try
			{
				square.Width = -1;
				Assert.Fail("Width is negative!");
			}
			catch(GameException){}

			//Try to set bad dimensions on game object...
			try
			{
				square.Width = 0;
				Assert.Fail("Width is zero!");
			}
			catch(GameException){}

			//Try to set bad dimensions on game object...
			try
			{
				square.Height = -1;
				Assert.Fail("Height is negative!");
			}
			catch(GameException){}
			
			//Try to set bad dimensions on game object...
			try
			{
				square.Height = 0;
				Assert.Fail("Height is negative!");
			}
			catch(GameException){}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void TestGameObjectCollection()
		{
			GameObjectCollection objectList = new GameObjectCollection();

			//Try to set size to null  ...
			try
			{
				objectList.Add(null);
				Assert.Fail("Input object is null.");
			}
			catch(GameException){}

			//Try to add null objects ...
			GameObject[] nullList = {null,null,null};
			try
			{
				objectList.AddRange(nullList);
				Assert.Fail("Input object list is null.");
			}
			catch(GameException){}

			//Try an empty list...
			GameObject[] emptyList = {};
			objectList.AddRange(emptyList);

			//Test constructor...						
			//try
			//{
			//GameObjectCollection objectList2 
			// = new GameObjectCollection(nullList);
			//Assert.Fail("Input object list is null.");
			//}
			//catch(GameException){}

			objectList.IndexOf(null);			
			try
			{
				objectList.Insert(0,null);
				Assert.Fail("Input object is null.");
			}
			catch(GameException){}

			try
			{
				objectList.Remove(null);
				Assert.Fail("Input object is null.");
			}
			catch(GameException){}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void TestGameArea()
		{
			MyGameArea area = new MyGameArea();
			try
			{
				area.AddObject(null);
				Assert.Fail("Input object is null.");
			}
			catch(GameException){}

			try
			{
				area.RemoveObject(null);
				Assert.Fail("Input object is null.");
			}
			catch(GameException){}
		}
	}
}
