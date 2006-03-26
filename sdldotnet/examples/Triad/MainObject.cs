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
	public class MainObject : IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		public MainObject() 
		{
		}
		
		BlockGrid grid;
		Scoreboard board;
		Surface screen;
		Surface surf;
		string data_directory = @"Data/";
		string filepath = @"../../";
		
		Sound levelUpSound;

		/// <summary>
		/// 
		/// </summary>
		public void Go() 
		{
			int width = 800;
			int height = 600;
			
			Video.WindowIcon();
			Video.WindowCaption = "SDL.NET - Triad";
			Events.KeyboardDown += 
				new KeyboardEventHandler(this.KeyboardDown); 
			Events.KeyboardUp += 
				new KeyboardEventHandler(this.KeyboardUp); 
			Events.Tick += new TickEventHandler(this.Tick);
			Events.Quit += new QuitEventHandler(this.Quit);

			board = new Scoreboard();
			board.X = 600;
			board.Y = 0;
			board.Size = new Size(200,400);

			try 
			{
				screen = 
					Video.SetVideoModeWindow(width, height);
				surf = 
					screen.CreateCompatibleSurface(width, height);
				surf.Fill(
					new Rectangle(new Point(0, 0), surf.Size), Color.Black); 
				grid = new BlockGrid(new Point(20,20),new Size(11,13));
				grid.BlocksDestroyed += 
					new BlocksDestroyedEventHandler(grid_BlocksDestroyed);

				if (File.Exists(data_directory + "levelup.wav"))
				{
					filepath = "";
				}

				levelUpSound = Mixer.Sound(filepath + data_directory + "levelup.wav");
				Events.Run();
			} 
			catch 
			{
				throw; 
			}
		}

		private void KeyboardDown(object sender, KeyboardEventArgs e) 
		{
			if (e.Key == Key.Escape || e.Key == Key.Q)
			{
				Events.QuitApplication();
			}
			grid.HandleSdlKeyDownEvent(e);
		}

		private void KeyboardUp(object sender, KeyboardEventArgs e) 
		{
			grid.HandleSdlKeyUpEvent(e);
		}

		private void Quit(object sender, QuitEventArgs e)
		{
			Events.QuitApplication();
		}

		[STAThread]
		static void Main() 
		{
			MainObject s = new MainObject();
			s.Go();
		}

		int blockCount;
		private void grid_BlocksDestroyed(
			object sender, BlocksDestroyedEventArgs args)
		{
			this.blockCount += args.BlocksCount;
			if(blockCount > 30)
			{
				this.blockCount = 0;
				this.grid.SpeedFactor = grid.SpeedFactor * 1.025f;
				this.board.Level += 1;
				if (this.levelUpSound != null)
				{
					this.levelUpSound.Play();
				}
			}
			
			this.board.BlocksDestroyed += args.BlocksCount;
			this.board.Score += args.BlocksCount * 100 * args.ReductionCount;
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
					grid.Dispose();
					board.Dispose();
					if (levelUpSound != null)
					{
						levelUpSound.Dispose();
					}
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
		~MainObject() 
		{
			Dispose(false);
		}

		#endregion

		private void Tick(object sender, TickEventArgs e)
		{
			//Clear and draw the space for the grid...
			surf.Fill(grid.Rectangle, Color.Black); 
			grid.Update();
			grid.Draw(surf);
				
			//Clear and draw the space for the score board...
			surf.Fill(board.Rectangle, Color.Black); 
			board.Update();
			board.Draw(surf);

			//Blit the grid and the board to the screen surface...
			screen.Blit(surf, board.Rectangle);
			screen.Blit(surf, grid.Rectangle);	
			screen.Flip();
		}
	}
}
