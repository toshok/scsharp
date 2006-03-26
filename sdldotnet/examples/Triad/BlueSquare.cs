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
using System.Drawing;
using SdlDotNet;

namespace SdlDotNet.Examples.Triad
{
	/// <summary>
	/// 
	/// </summary>
	public class BlueSquare : GameObject
	{
		/// <summary>
		/// 
		/// </summary>
		public BlueSquare()
		{
			this.Size = new Size(30,30);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Update()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		protected override void DrawGameObject(Surface surface)
		{			
			Rectangle t1 = this.Rectangle;
			surface.Fill(t1,Color.Blue);			
		}
	}
}
