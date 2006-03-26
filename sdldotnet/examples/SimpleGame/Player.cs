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
	/// Derived class
	/// </summary>
	public class Player
	{
		ArrayList entityList;
		/// <summary>
		/// constructor
		/// </summary>
		public Player(EventManager eventManager)
		{
			this.entityList = new ArrayList();
			this.entityList.Add(new Entity(eventManager));
		}
	}
}
