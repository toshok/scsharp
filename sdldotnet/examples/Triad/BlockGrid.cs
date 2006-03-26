
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
using System.Collections;

namespace SdlDotNet.Examples.Triad
{
	/// <summary>
	/// 
	/// </summary>
	public class BlockGrid : GameArea, IDisposable
	{
		Size sizeOfGrid;
		Triad triad;
		static BlockCollection blockList = new BlockCollection();
		int delayFactor = 400;
		private float speedFactor = 1.0f;
		string data_directory = @"Data/";
		string filepath = @"../../";

		/// <summary>
		/// 
		/// </summary>
		public float SpeedFactor
		{
			get
			{
				return speedFactor;
			}
			set
			{
				if(value == 0.0)
				{
					throw new SdlException("You can not set the speed factor to zero.");
				}

				this.delayFactor = (int)(this.delayFactor / value);
				speedFactor = value;				
			}
		}

		int reductionCount;
		int lastTriadMove;
		int timeNow;
		Block[,] grid;
		BlockGridState currentState;
		bool rapidDropTriad;
		bool gameIsPaused;
		static bool reductionOccured;
		
		/// <summary>
		/// 
		/// </summary>
		public event BlocksDestroyedEventHandler BlocksDestroyed;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reductionCount"></param>
		/// <param name="blockCount"></param>
		public void OnBlocksDestroyed(int reductionCount, int blockCount)
		{
			if(BlocksDestroyed != null)
			{
				BlocksDestroyed(this,new BlocksDestroyedEventArgs(reductionCount,blockCount));
			}

		}
		/// <summary>
		/// 
		/// </summary>
		public Size GridSize 
		{
			get
			{
				return sizeOfGrid;
			}
		}
		 
		/// <summary>
		/// 
		/// </summary>
		/// <param name="location"></param>
		/// <param name="gridSize"></param>
		public BlockGrid(Point location, Size gridSize) : base()
		{
			Mixer.SetAllChannelsVolume(50);
			
			int blockGridWidth = gridSize.Width * Block.BlockWidth;
			int blockGridHeight = gridSize.Height * Block.BlockWidth;
			this.Size = new Size(blockGridWidth,blockGridHeight);
			this.Location = location;			            		
			sizeOfGrid = gridSize;		

			triad = new Triad(this);			
			AddObject(triad);
			lastTriadMove = Timer.TicksElapsed;
			lastTriadLeftRight = Timer.TicksElapsed;

			grid = new Block[gridSize.Width,gridSize.Height];

			currentState = BlockGridState.MoveTriad;
			if (File.Exists(data_directory + "move.wav"))
			{
				filepath = "";
			}

			try
			{
				loadSounds();
			}
			catch (SdlException)
			{
			}
		}

		Sound moveSound;
		Sound permuteSound;
		Sound reductionSound;
		Sound hitBottomSound;
		Sound gameOverSound;
		void loadSounds()
		{
			if(moveSound == null)
			{
				moveSound = Mixer.Sound(filepath + data_directory + "move.wav");
			}

			if(permuteSound==null)
			{
				permuteSound = Mixer.Sound(filepath + data_directory + "permute.wav");
			}

			if(reductionSound==null)
			{
				reductionSound = Mixer.Sound(filepath + data_directory + "reduction.wav");
			}

			if(hitBottomSound==null)
			{
				hitBottomSound = Mixer.Sound(filepath + data_directory + "hitBottom.wav");
			}

			if(gameOverSound == null)
			{
				gameOverSound = Mixer.Sound(filepath + data_directory + "gameOver.wav");
			}
		}
	
		void markBlocks(Triad triad)
		{
			int row = triad.Y / Block.BlockWidth;
			int col = triad.X / Block.BlockWidth;
			markBlock(row,col,triad.TopBlock);
			markBlock(row+1,col,triad.MiddleBlock);
			markBlock(row+2,col,triad.BottomBlock);

		}

		void markBlock(int row, int column, Block someBlock)
		{
			someBlock.Y = row*Block.BlockWidth;
			someBlock.X = column*Block.BlockWidth;
			someBlock.GridX = column;
			someBlock.GridY = row;
			grid[column,row] = someBlock;

			AddObject(someBlock);
		}

		int lastTriadLeftRight;
		void moveTriad()
		{
			try
			{
				if(triad == null)
				{
					return;
				}

				timeNow = Timer.TicksElapsed;
				triad.Update();

				int deltaTime = timeNow - lastTriadMove;
				int deltaTimeLeftRight = timeNow - lastTriadLeftRight;

				if((this.moveTriadLeft == true)  &&   (deltaTimeLeftRight > 250) )
				{
					triad.MoveLeft();
					this.lastTriadLeftRight = Timer.TicksElapsed;
					moveSound.Play();
				}

				if((this.moveTriadRight == true)  &&   (deltaTimeLeftRight > 250) )
				{
					triad.MoveRight();
					this.lastTriadLeftRight = Timer.TicksElapsed;
					moveSound.Play();
				}
			
				if((deltaTime > delayFactor)||((rapidDropTriad)&&(deltaTime>(delayFactor/16))))
				{	
					if(triad.CanMoveDown())
					{
						triad.MoveDown();
					}
					else
					{
						this.hitBottomSound.Play();
						markBlocks(triad);
						RemoveObject(triad);
						triad = null;
						currentState = BlockGridState.MarkBlocksToDestroy;
					}
					
					lastTriadMove = Timer.TicksElapsed;
				}
			}
			catch (SdlException)
			{
			}
		}

		bool triadHitsAnyBlock()
		{
			foreach(GameObject obj in this.GameObjectList)
			{
				if(triad.Hits(obj))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Update()
		{
			if(gameIsPaused) return;

			try
			{
				switch(currentState)
				{
					case BlockGridState.CreateTriad:
						triad=new Triad(this);
						if(triadHitsAnyBlock())
						{
							gameOverSound.Play();;
							currentState = BlockGridState.GameOver;
						}
						else
						{
							AddObject(triad);
							currentState = BlockGridState.MoveTriad;
							reductionCount = 0;
						}
						break;
				
					case BlockGridState.MoveTriad:
						moveTriad();
						break;

					case BlockGridState.MarkBlocksToDestroy:
						markBlocksToDestroy();
						currentState = BlockGridState.ShowBlocksDestroyed;
						break;

					case BlockGridState.ShowBlocksDestroyed:
						showBlocksDestroyed();
						currentState = BlockGridState.ReduceGrid;
						break;

					case BlockGridState.ReduceGrid:
						reduceGrid();
						reductionCount++;
						if(reductionOccured)
						{
							currentState = BlockGridState.MarkBlocksToDestroy;
						}
						else
						{
							currentState = BlockGridState.CreateTriad;
						}

						if(numberOfBlocksDestroyed > 0)
						{
							this.reductionSound.Play();
							OnBlocksDestroyed(reductionCount, numberOfBlocksDestroyed);
						}

						break;
					case BlockGridState.GameOver:
						break;

					default:
						throw new SdlException("BlockGridState is not handled: " + currentState);
				}
			}
			catch (SdlException)
			{	
			}
		}

		int numberOfBlocksDestroyed;
		void clearDestroyedBlocks()
		{
			int row;
			int column;
			Block currentBlock;
			for(column=0;  column<GridSize.Width;  column++)
			{		
				for(row=0;  row<GridSize.Height;  row++)
				{   
					currentBlock = grid[column,row];
					if((currentBlock != null)&&(currentBlock.Destroy))
					{
						RemoveObject(currentBlock);
						grid[column,row] = null;
						currentBlock.Dispose();
						numberOfBlocksDestroyed++;
					}   
				}
			}
		}

		void removeEmptySpacesInGrid()
		{
			int row;
			int column;
			Block currentBlock;			
			
			//Setup block list..			
			for(column=0;  column<GridSize.Width;  column++)
			{
				blockList.Clear();
				for(row=0;  row<GridSize.Height;  row++)
				{
					currentBlock = grid[column,row];
					if(currentBlock != null)
					{
						grid[column,row] = null;
						blockList.Add(currentBlock);							
					}

				}

				int k = GridSize.Height - blockList.Count;
				int i = 0;
				for(row=k;  row<GridSize.Height;  row++)
				{
					if(i < blockList.Count)
					{
						currentBlock = blockList[i];
						this.markBlock(row,column,currentBlock);
						i++;
					}
				}
			}
		}

		
		BlockCollection blocksToDestroy = new BlockCollection();
		void removeDestroyedBlocks()
		{			
			blocksToDestroy.Clear();
			foreach(GameObject go in this.GameObjectList)
			{
				if(go.GetType().Name == "Block")
				{
					Block aBlock = (Block)go;
					if(aBlock.Destroy)
					{
						blocksToDestroy.Add(aBlock);
					}
				}
			}

			foreach(Block block in blocksToDestroy)
			{
				RemoveObject(block);
				block.Dispose();
			}
		}
		
		void reduceGrid()
		{
			clearDestroyedBlocks();
			removeEmptySpacesInGrid();
			removeDestroyedBlocks();
		}

		static void showBlocksDestroyed()
		{
			int start = Timer.TicksElapsed;
			int delta = Timer.TicksElapsed - start;
			while(delta <=250)
			{
				delta = Timer.TicksElapsed - start;				
			}
		}

		static void markBlockToDestroyUsingBlockList()
		{
			//Using the blockList, determine which blocks need to be destroyed.

			Block currentBlock;
			Block previousBlock;
			int sameColorCount = 1;
			for(int i=1;  i<blockList.Count;  i++)
			{
				currentBlock = blockList[i];
				previousBlock = blockList[i-1];

				if((currentBlock is NullBlock) ||( previousBlock is NullBlock))
				{
					sameColorCount = 1;
				}
				else if(currentBlock.TypeOfBlock != previousBlock.TypeOfBlock)
				{
					sameColorCount = 1;
				}
				else
				{
					sameColorCount++;
					if(sameColorCount >= 3)
					{
						blockList[i].Destroy = true;
						blockList[i-1].Destroy = true;
						blockList[i-2].Destroy = true;
						reductionOccured = true;
					}
				}

			}

		}

		void addCurrentGridLocationToBlockList(int column, int row)
		{
			Block currentBlock = grid[column,row];
			if(currentBlock == null)
			{
				blockList.Add(new NullBlock());
			}
			else
			{
				blockList.Add(currentBlock);
			}
		}
		
		void markBlocksVertically()
		{
			for(int column=0; column<GridSize.Width;  column++)
			{
				blockList.Clear();

				//Fill the blockList with blocks for the current column..
				for(int row=0;  row<GridSize.Height;  row++)
				{
					addCurrentGridLocationToBlockList(column, row);
				}

				markBlockToDestroyUsingBlockList();				
			}
		}

		void markBlocksHorizontally()
		{			
			for(int row=0;  row<GridSize.Height;  row++)
			{
				blockList.Clear();

				//Fill the blockList with blocks for the current row..				
				for(int column=0; column<GridSize.Width;  column++)
				{
					addCurrentGridLocationToBlockList(column, row);
				}

				markBlockToDestroyUsingBlockList();				

			}
		}

		void markBlocksLeftDown()
		{
			for (int currentRow=0;  currentRow<GridSize.Height - 2;   currentRow++)
			{
				
				int row = currentRow;
				int col = GridSize.Width-1;  

				blockList.Clear();
				while (( row <  GridSize.Height)&&(  col >= 0))
				{  					 
					
					addCurrentGridLocationToBlockList(col,row);
					row++;
					col--;
				}

				markBlockToDestroyUsingBlockList();
			}
		}

		void markBlocksRightDown()
		{
			for (int currentRow=0;  currentRow<GridSize.Height - 2;   currentRow++)
			{
				
				int row = currentRow;
				int col = 0;  

				blockList.Clear();
				while (( row <  GridSize.Height)&&(  col < GridSize.Width))
				{  					 
					
					addCurrentGridLocationToBlockList(col,row);
					row++;
					col++;
				}

				markBlockToDestroyUsingBlockList();
			}
		}

		void markBlocksToDestroy()
		{
			reductionOccured = false;
			markBlocksVertically();
			markBlocksHorizontally();
			markBlocksLeftDown();
			markBlocksRightDown();
		}

		bool moveTriadLeft;
		bool moveTriadRight;
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
			System.Diagnostics.Debug.WriteLine(args.Key.ToString());
			
			try
			{
				switch(args.Key)
				{
					case Key.Space:
						if(triad!=null)
						{
							triad.Permute();
							permuteSound.Play();
						}

						break;
					case Key.LeftArrow:
						if(triad!=null)
						{
							moveTriadLeft = true;	
							triad.MoveLeft();
							this.moveSound.Play();
							this.lastTriadLeftRight = Timer.TicksElapsed;
						}

						break;
					case Key.RightArrow:
						if(triad!=null)
						{
							moveTriadRight = true;	
							triad.MoveRight();
							this.moveSound.Play();
							this.lastTriadLeftRight = Timer.TicksElapsed;
						}
						break;
					case Key.DownArrow:
						this.rapidDropTriad = true;
						break;

					case Key.P:
						gameIsPaused = !gameIsPaused;
						break;
					default:
						System.Diagnostics.Debug.WriteLine("Key not handled: " + 
							args.Key.ToString());
						break;
				}		
			}
			catch (SdlException)
			{
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void HandleSdlKeyUpEvent(KeyboardEventArgs args)
		{
			rapidDropTriad = false;
			moveTriadRight = false;
			moveTriadLeft = false;
		}

		Surface gameOverImage;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		protected override void DrawGameObject(Surface surface)
		{
			this.DrawGameObjects(surface);

			if(gameIsPaused)
			{
				drawGamePausedMessage(surface);
			}

			if(currentState == BlockGridState.GameOver)
			{
				drawGameOverMessage(surface);
			}
		}

		void drawGameOverMessage(Surface surface)
		{
			if(gameOverImage==null)
			{
				gameOverImage = new Surface(new Bitmap(filepath + data_directory + "gameOver.bmp"));
			}

			int xPosition = this.ScreenX + (int)((this.Width-gameOverImage.Width)/2); 
			surface.Blit(gameOverImage, new Rectangle(xPosition ,this.ScreenY+200,gameOverImage.Width,gameOverImage.Height));
		}

		Surface gamePausedImage;
		void drawGamePausedMessage(Surface surface)
		{
			if(gamePausedImage==null)
			{
				gamePausedImage = new Surface(new Bitmap(filepath + data_directory + "gamePause.bmp"));
			}

			int xPosition = this.ScreenX + (int)((this.Width-gamePausedImage.Width)/2); 
			surface.Blit(gamePausedImage, new Rectangle(xPosition ,this.ScreenY+200,gamePausedImage.Width,gamePausedImage.Height));
		}
		#region IDisposable Members

		bool disposed;

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
					reductionSound.Dispose();
					hitBottomSound.Dispose();
					moveSound.Dispose();
					gameOverSound.Dispose();
					gameOverImage.Dispose();
					gamePausedImage.Dispose();
					permuteSound.Dispose();
					triad.Dispose();
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
		~BlockGrid() 
		{
			Dispose(false);
		}
		#endregion

	}
}
