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
	public class MyGameArea : GameArea
	{
		BlueSquare bs;
		/// <summary>
		/// 
		/// </summary>
		public MyGameArea() :base()
		{
			for(int i=0;  i<200; i++)
			{				
				AddObject(new BouncingSquare());
			}
			Size = new Size(800,600);

			bs = new BlueSquare();
			AddObject(bs);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		protected override void DrawGameObject(Surface surface)
		{
			this.DrawGameObjects(surface);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void HandleSdlKeyUpEvent(KeyboardEventArgs args)
		{
            
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void HandleSdlKeyDownEvent(KeyboardEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			switch(args.Key)
			{
				case Key.Keypad2 :
					bs.Y += 10;
					break;

				case Key.Keypad8 :
					bs.Y -= 10;
					break;

				case Key.Keypad4 :
					bs.X -= 10;
					break;

				case Key.Keypad6 :
					bs.X += 10;
					break;

				default:
					break;
			}			
		}
	}
}
