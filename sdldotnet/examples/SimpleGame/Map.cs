// Copyright 2005 David Hudson (jendave@yahoo.com)
// This file is part of SimpleGame.
//
// SimpleGame is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// SimpleGame is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SimpleGame; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections;
using SdlDotNet;

namespace SdlDotNet.Examples.SimpleGame
{
	/// <summary>
	/// Summary description for Game.
	/// </summary>
	public class Map
	{
		Sector[] sectors;
		EventManager eventManager;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventManager"></param>
		public Map(EventManager eventManager)
		{
			this.eventManager = eventManager;
			this.sectors = new Sector[9];
		}

		/// <summary>
		/// 
		/// </summary>
		public void Build() 
		{
			for (int i = 0; i < 9; i++)
			{
				this.sectors[i] = new Sector();
			}
			this.sectors[3].SetNeighbors(Direction.Up,this.sectors[0]);
			this.sectors[4].SetNeighbors(Direction.Up,this.sectors[1]);
			this.sectors[5].SetNeighbors(Direction.Up,this.sectors[2]);
			this.sectors[6].SetNeighbors(Direction.Up,this.sectors[3]);
			this.sectors[7].SetNeighbors(Direction.Up,this.sectors[4]);
			this.sectors[8].SetNeighbors(Direction.Up,this.sectors[5]);

			this.sectors[0].SetNeighbors(Direction.Down,this.sectors[3]);
			this.sectors[1].SetNeighbors(Direction.Down,this.sectors[4]);
			this.sectors[2].SetNeighbors(Direction.Down,this.sectors[5]);
			this.sectors[3].SetNeighbors(Direction.Down,this.sectors[6]);
			this.sectors[4].SetNeighbors(Direction.Down,this.sectors[7]);
			this.sectors[5].SetNeighbors(Direction.Down,this.sectors[8]);

			this.sectors[1].SetNeighbors(Direction.Left,this.sectors[0]);
			this.sectors[2].SetNeighbors(Direction.Left,this.sectors[1]);
			this.sectors[4].SetNeighbors(Direction.Left,this.sectors[3]);
			this.sectors[5].SetNeighbors(Direction.Left,this.sectors[4]);
			this.sectors[7].SetNeighbors(Direction.Left,this.sectors[6]);
			this.sectors[8].SetNeighbors(Direction.Left,this.sectors[7]);

			this.sectors[0].SetNeighbors(Direction.Right,this.sectors[1]);
			this.sectors[1].SetNeighbors(Direction.Right,this.sectors[2]);
			this.sectors[3].SetNeighbors(Direction.Right,this.sectors[4]);
			this.sectors[4].SetNeighbors(Direction.Right,this.sectors[5]);
			this.sectors[6].SetNeighbors(Direction.Right,this.sectors[7]);
			this.sectors[7].SetNeighbors(Direction.Right,this.sectors[8]);

			eventManager.Publish(new MapBuiltEventArgs(this));
		}

		/// <summary>
		/// 
		/// </summary>
		public Sector[] GetSectors()
		{
			return this.sectors;
		}
	}
}
