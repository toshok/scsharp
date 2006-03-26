

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
using System.Drawing;

namespace SdlDotNet.Examples.Triad
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class  GameObject
	{
		private GameObject parent;
		/// <summary>
		/// 
		/// </summary>
		public GameObject Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;				
			}
		}
	
		private int  x;
		/// <summary>
		/// 
		/// </summary>
		public int  X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int ScreenX
		{
			get
			{
				int x = this.x;
				if(this.parent != null)
				{
					x += this.parent.ScreenX;
				}
				return x;
			}
		}
	
		/// <summary>
		/// 
		/// </summary>
		public int ScreenY
		{
			get
			{
				int y = this.y;
				if(this.parent != null)
				{
					y += this.parent.ScreenY;
				}
				return y;
			}
		}
	
		private int  y;
		/// <summary>
		/// 
		/// </summary>
		public int  Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;				
			}
		}

		private int  width;
		/// <summary>
		/// 
		/// </summary>
		public int  Width
		{
			get
			{
				return width;
			}
			set
			{
				if(width <= 0)
				{
					throw new GameException("Width is set to zero or negative value.");
				}

				width = value;				
			}
		}

		private int  height;
		/// <summary>
		/// 
		/// </summary>
		public int  Height
		{
			get
			{
				return height;
			}
			set
			{
				if(value <= 0)
				{
					throw new GameException("Height is set to zero or negative value.");
				}

				height = value;				
			}
		}
	
		/// <summary>
		/// 
		/// </summary>
		public int  X2
		{
			get
			{
				return x + width;
			}
		}
	
		/// <summary>
		/// 
		/// </summary>
		public int  Y2
		{
			get
			{
				return y + height;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int ScreenX2
		{
			get
			{
				int offSetX = 0;
				if(this.parent != null)
				{
					offSetX = this.parent.ScreenX;
				}
				return this.X2 + offSetX;
			}
		}
	
		/// <summary>
		/// 
		/// </summary>
		public int ScreenY2
		{
			get
			{
				int offSetY = 0;
				if (this.parent != null)
				{
					offSetY = this.parent.ScreenY;
				}
				return y + height + offSetY;
			}
		}
		
		int previousWidth;
		int previousHeight;
		Size currentSize;
		/// <summary>
		/// 
		/// </summary>
		public Size Size
		{
			get
			{
				if((previousWidth != width )||(previousHeight != height))
				{
					currentSize = new Size(width, height);					
				}

				previousWidth = width;
				previousHeight = height;

				return currentSize;
			}
			set
			{
				this.width = value.Width;
				this.height = value.Height;
			}
		}

		System.Drawing.Rectangle currentRectangle;
		Point previousLocation;
		/// <summary>
		/// 
		/// </summary>
		public Rectangle Rectangle
		{
			get
			{
				if(previousLocation != Location)
				{
					currentRectangle = new System.Drawing.Rectangle(Location,this.Size);					
				}

				previousLocation = Location;
				return currentRectangle;
			}
		}	

		Point previousScreenLocation;
		System.Drawing.Rectangle currentScreenRectangle;
		/// <summary>
		/// 
		/// </summary>
		public Rectangle ScreenRectangle
		{
			get
			{
				if(previousScreenLocation != ScreenLocation)
				{
					currentScreenRectangle = new System.Drawing.Rectangle(ScreenLocation,this.Size);
				}

				previousScreenLocation = ScreenLocation;
				return currentScreenRectangle;
			}
		}	

		/// <summary>
		/// 
		/// </summary>
		public Point Location
		{
			get
			{
				return new Point(x, y);
			}
			set
			{	
				this.x = value.X;
				this.y = value.Y;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Point ScreenLocation
		{
			get
			{
				return new Point(this.ScreenX, this.ScreenY);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static Point BottomRightCorner
		{
			get
			{
				return new Point(0,0);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public abstract void Update();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		protected abstract void DrawGameObject(Surface surface);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		public void Draw(Surface surface)
		{
			if(surface == null)
			{
				throw new GameException("Input surface is NullReferenceException");
			}

			DrawGameObject(surface);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public bool Contains(Point point)
		{
			bool inSideX = (ScreenX <= point.X)&&(point.X<=ScreenX2);
			bool inSideY = (ScreenY <= point.Y)&&(point.Y<=ScreenY2);
			return inSideX&&inSideY;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool Hits(GameObject obj)
		{		
			if(obj == null)
			{
				return false;
			}

			return Contains(obj.Location) || Contains(GameObject.BottomRightCorner) ;
		}
	}
}
