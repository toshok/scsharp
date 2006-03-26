
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
using SdlDotNet;

namespace SdlDotNet.Examples.Triad
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class GameArea : GameObject
	{
		GameObjectCollection objectList = new GameObjectCollection();
		/// <summary>
		/// 
		/// </summary>
		public GameObjectCollection GameObjectList
		{
			get
			{
				return objectList;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public void AddObject(GameObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			objectList.Add(obj);
			obj.Parent = this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public void RemoveObject(GameObject obj)
		{
			objectList.Remove(obj);			
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Update()
		{
			foreach(GameObject obj in objectList)
			{
				obj.Update();			
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		protected void DrawGameObjects(Surface surface)
		{
			foreach(GameObject obj in objectList)
			{
				obj.Draw(surface);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public abstract void HandleSdlKeyDownEvent(KeyboardEventArgs args);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public abstract void HandleSdlKeyUpEvent(KeyboardEventArgs args);
	}
}
