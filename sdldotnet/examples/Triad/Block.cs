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
using System.IO;
using System.Drawing;
using SdlDotNet;

namespace SdlDotNet.Examples.Triad
{
	/// <summary>
	/// 
	/// </summary>
	public class Block : GameObject, System.IDisposable
	{
		static string data_directory = @"Data/";
		static string filepath = @"../../";

		/// <summary>
		/// 
		/// </summary>
		public const int BlockWidth = 36;
		/// <summary>
		/// 
		/// </summary>
		public readonly static Size BlockSize = 
			new System.Drawing.Size(BlockWidth,BlockWidth);

		BlockType blockType;
		/// <summary>
		/// 
		/// </summary>
		public BlockType TypeOfBlock
		{
			set
			{
				blockType = value;
			}

			get
			{
				return blockType;
			}

		}

		static Random random;

		/// <summary>
		/// 
		/// </summary>
		public Block()
		{
			this.Size = BlockSize;

			if (random==null)
			{
				random = new Random(DateTime.Now.Millisecond);
			}
			
			blockType = (BlockType)random.Next(5);
		}

		private int gridX;
		/// <summary>
		/// 
		/// </summary>
		public int GridX
		{
			get
			{
				return gridX;
			}
			set
			{
				gridX = value;				
			}
		}
	
		private int gridY;
		/// <summary>
		/// 
		/// </summary>
		public int GridY
		{
			get
			{
				return gridY;
			}
			set
			{
				gridY = value;				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Point GridLocation
		{
			get
			{
				return new Point(gridX,gridY);
			}
			set
			{
				gridX = value.X;
				gridY = value.Y;
			}

		}

		private bool destroy;

		/// <summary>
		/// 
		/// </summary>
		public bool Destroy
		{
			get
			{
				return destroy;
			}
			set
			{
				destroy = value;
			}
		}
	
		static Surface redBlock;

		static Surface getRedBlock()
		{
			if (redBlock == null)
			{
				Bitmap bmp = new System.Drawing.Bitmap(filepath + data_directory + "redBlock.bmp");
				redBlock = new Surface(bmp);
			}
			return redBlock;
		}

		static Surface whiteBlock;

		static Surface getWhiteBlock()
		{
			if (whiteBlock == null)
			{
				Bitmap bmp = new System.Drawing.Bitmap(filepath + data_directory + "whiteBlock.bmp");
				whiteBlock = new Surface(bmp);
			}
			return whiteBlock;
		}
		
		static Surface yellowBlock;

		static Surface getYellowBlock()
		{
			if (yellowBlock == null)
			{
				Bitmap bmp = new System.Drawing.Bitmap(filepath + data_directory + "yellowBlock.bmp");
				yellowBlock = new Surface(bmp);
			}
			return yellowBlock;
		}

		static Surface purpleBlock;

		static Surface getPurpleBlock()
		{
			if (purpleBlock == null)
			{
				Bitmap bmp = new System.Drawing.Bitmap(filepath + data_directory + "purpleBlock.bmp");
				purpleBlock = new Surface(bmp);
			}
			return purpleBlock;
		}

		static Surface blueBlock;
		static Surface getBlueBlock()
		{
			if (blueBlock == null)
			{
				Bitmap bmp = new System.Drawing.Bitmap(filepath + data_directory + "blueBlock.bmp");
				blueBlock = new Surface(bmp);
			}
			return blueBlock;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		protected override void DrawGameObject(Surface surface)
		{
			Surface image;
			if (File.Exists(data_directory + "blueBlock.bmp"))
			{
				filepath = "";
			}
			
			switch(blockType)
			{
				case BlockType.Purple:	image = Block.getPurpleBlock();	break;
				case BlockType.Red:		image = Block.getRedBlock();		break;
				case BlockType.White:	image = Block.getWhiteBlock();	break;
				case BlockType.Yellow:	image = Block.getYellowBlock();	break;
				case BlockType.Blue:	image = Block.getBlueBlock();	break;
				default: image = Block.getBlueBlock(); break;
			}

			if (!this.Destroy)
			{
				if(image != null)
				{
					surface.Blit(image,this.ScreenRectangle);	
				}
			}
			else
			{
				surface.Fill(this.ScreenRectangle,Color.SlateGray);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Update()
		{
		}

		#region IDisposable Members

		private bool disposed;

		/// <summary>
		/// Destroy sprite
		/// </summary>
		/// <param name="disposing">If true, remove all unamanged resources</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.Parent = null;	
					GC.SuppressFinalize(this);
				}
				this.disposed = true;
			}
		}
		/// <summary>
		/// Destroy object
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>
		/// Destroy object
		/// </summary>
		public void Close() 
		{
			Dispose();
		}

		/// <summary>
		/// Destroy object
		/// </summary>
		~Block() 
		{
			Dispose(false);
		}
		#endregion

	}
}
