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
using SdlDotNet;

namespace SdlDotNet.Examples.SimpleGame
{
	/// <summary>
	/// Derived class
	/// </summary>
	public class Entity
	{
		Sector sector;
		EventManager eventManager;
		/// <summary>
		/// constructor
		/// </summary>
		public Entity(EventManager eventManager)
		{
			if (eventManager == null)
			{
				throw new ArgumentNullException("eventManager");
			}
			eventManager.OnGameStatusEvent += new EventManager.GameStatusEventHandler(Subscribe);
			eventManager.OnEntityMoveRequestEvent += 
				new EventManager.EntityMoveRequestEventHandler(Subscribe);
			this.eventManager = eventManager;
		}

		private void Subscribe(object eventManager, GameStatusEventArgs e)
		{
			LogFile.WriteLine("Entity received a GameStatus event: " + e.GameStatus);
			if (e.GameStatus == GameStatus.Started)
			{
				Place(e.Game.Map.GetSectors()[Names.StartingSector]);
			}
		}

		private void Subscribe(object eventManager, EntityMoveRequestEventArgs e)
		{
			LogFile.WriteLine("Entity received a EntityMoveRequest event: " + e.Direction);
			Move(e.Direction);
		}

		/// <summary>
		/// 
		/// </summary>
		public Sector Sector
		{
			get
			{
				return this.sector;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="direction"></param>
		public void Move(Direction direction)
		{
			if (this.sector.MovePossible(direction))
			{
				Sector newSector = this.sector.GetNeighbors(direction);
				this.sector = newSector;
				eventManager.Publish(new EntityMoveEventArgs(this));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sector"></param>
		public void Place(Sector sector)
		{
			this.sector = sector;
			eventManager.Publish(new EntityPlaceEventArgs(this));
		}

	}
}
