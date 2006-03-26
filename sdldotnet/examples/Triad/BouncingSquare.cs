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
	public class BouncingSquare : GameObject
	{
		static Random rnd;

		/// <summary>
		/// 
		/// </summary>
		public BouncingSquare()
		{
			if(rnd == null)
			{
				rnd = new Random(System.DateTime.Now.Second);
			}

			this.dx = xinc;
			this.dy = yinc;
			this.X = rnd.Next(800);
			this.Y = rnd.Next(600);
			this.Size = new Size(3,3);
			
			this.xinc = rnd.Next(5)+1;
			this.yinc = rnd.Next(5)+1;
		}

		int xinc = 5;
		int yinc = 5;
		int dx = 5;
		int dy = 5;
		/// <summary>
		/// 
		/// </summary>
		public override void Update()
		{
			this.xinc = rnd.Next(5)+1;
			this.yinc = rnd.Next(5)+1;
			
			if(this.Parent == null)
			{
				throw new GameException("Parent object is null.");
			}

			if(this.Parent.Width ==0)
			{
				throw new GameException("Parent width is zero.");
			}

			if(this.Parent.Height ==0)
			{
				throw new GameException("Parent height is zero.");
			}
	
			if(X <=0)
			{
				dx = xinc;
			}

			if(Y <=0)
			{
				dy = yinc;
			}

			if(X > Parent.Width - this.Width)
			{
				dx = -xinc;
			}

			if(Y > Parent.Height - this.Height)
			{
				dy = -yinc;
			}	

			this.X += dx;
			this.Y += dy;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		protected override void DrawGameObject(Surface surface)
		{			
			surface.Fill(this.Rectangle,Color.Red);
		}
	}
}
