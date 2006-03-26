/*
 * $RCSfile: MoviePlayer.cs,v $
 * Copyright (C) 2004 David Hudson (jendave@yahoo.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using SdlDotNet;

namespace SdlDotNet.Examples.MoviePlayer
{
	#region Class Documentation
	/// <summary>
	/// Simple Tao.Sdl Example
	/// </summary>
	/// <remarks>
	/// Just plays a short movie.
	/// To quit, you can close the window, 
	/// press the Escape key or press the 'q' key
	/// <p>Written by David Hudson (jendave@yahoo.com)</p>
	/// </remarks>
	#endregion Class Documentation
	public class MoviePlayer : IDisposable
	{		
		Movie movie;
		Surface screen;

		#region Run()
		/// <summary>
		/// 
		/// </summary>
		public void Run() 
		{
			string data_directory = @"Data/";
			string filepath = @"../../";
			if (File.Exists(data_directory + "test.mpg"))
			{
				filepath = "";
			}

			int width = 352;
			int height = 240;

			Events.KeyboardDown += 
				new KeyboardEventHandler(this.KeyboardDown); 
			Events.Tick += new TickEventHandler(this.Tick);
			Events.Quit += new QuitEventHandler(this.Quit);

			Video.WindowIcon();
			Video.WindowCaption = "SDL.NET - Movie Player";
			screen = Video.SetVideoModeWindow(width, height); 
			Mixer.Close();
			movie = new Movie(filepath + data_directory + "test.mpg");
			Console.WriteLine("Time: " + movie.Length);
			Console.WriteLine("Width: " + movie.Size.Width);
			Console.WriteLine("Height: " + movie.Size.Height);
			Console.WriteLine("HasAudio: " + movie.HasAudio);
			Console.WriteLine("HasVideo: " + movie.HasVideo);
			movie.Display(screen);
			movie.Play();
			Events.Run();
		} 
		#endregion Run()

		#region Main()
		[STAThread]
		static void Main() 
		{
			MoviePlayer player = new MoviePlayer();
			player.Run();
		}
		#endregion Main()

		private void KeyboardDown(object sender, KeyboardEventArgs e)
		{
			// Check if the key pressed was a Q or Escape
			if (e.Key == Key.Escape || e.Key == Key.Q)
			{
				movie.Stop();
				movie.Close();
				Events.QuitApplication();
			}
		}

		private void Quit(object sender, QuitEventArgs e)
		{
			Events.QuitApplication();
		}

		private void Tick(object sender, TickEventArgs e)
		{
			if (movie.IsPlaying)
			{
				return;
			}
			else
			{
				movie.Stop();
				movie.Close();
				Events.QuitApplication();
			}
		}
		#region IDisposable Members

		private bool disposed;

		/// <summary>
		/// Closes and destroys this object
		/// </summary>
		/// <remarks>Destroys managed and unmanaged objects</remarks>
		public void Dispose() 
		{
			Dispose(true);
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
		~MoviePlayer() 
		{
			Dispose(false);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.movie != null)
					{
						this.movie.Dispose();
						this.movie = null;
					}
				}
				this.disposed = true;
			}
		}

		#endregion
	}
}
