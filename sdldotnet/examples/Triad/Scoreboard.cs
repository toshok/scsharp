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
	public class Scoreboard : GameArea, IDisposable
	{
		SdlDotNet.Font font;
		/// <summary>
		/// 
		/// </summary>
		public Scoreboard()
		{
			string FontName = "FreeSans.ttf";
			string data_directory = @"Data/";
			string filepath = @"../../";

			if (File.Exists(data_directory + FontName))
			{
				filepath = "";
			}
			font = new SdlDotNet.Font(filepath + data_directory + FontName,18);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		protected override void DrawGameObject(Surface surface)
		{
			int currentY = 0;

			Surface fontSurface = font.Render("Score: " + this._Score, Color.FromArgb(255,255,255));
			surface.Blit(fontSurface, new System.Drawing.Point(this.ScreenX, this.ScreenY + currentY));

			currentY+=20;
			fontSurface = font.Render("Blocks Destroyed: " + this._BlocksDestroyed, Color.FromArgb(255,255,255));
			surface.Blit(fontSurface, new System.Drawing.Point(this.ScreenX, this.ScreenY + currentY));

			currentY+=20;

			fontSurface = font.Render("Level: " + this._Level, Color.FromArgb(255,255,255));
			surface.Blit(fontSurface, new System.Drawing.Point(this.ScreenX, this.ScreenY + currentY));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void HandleSdlKeyDownEvent(KeyboardEventArgs args)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void HandleSdlKeyUpEvent(KeyboardEventArgs args)
		{
		}

		private int _Score;
		/// <summary>
		/// 
		/// </summary>
		public int Score
		{
			get
			{
				return _Score;
			}
			set
			{
				_Score = value;				
			}
		}
	
		private int _Level;
		/// <summary>
		/// 
		/// </summary>
		public int Level
		{
			get
			{
				return _Level;
			}
			set
			{
				_Level = value;				
			}
		}

		private int _BlocksDestroyed;
		/// <summary>
		/// 
		/// </summary>
		public int BlocksDestroyed
		{
			get
			{
				return _BlocksDestroyed;
			}
			set
			{
				_BlocksDestroyed = value;				
			}
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
					font.Dispose();
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
		~Scoreboard() 
		{
			Dispose(false);
		}
		#endregion

	}
}
