/*
 * $RCSfile: Rectangles.cs,v $
 * Copyright (C) 2004 David Hudson (jendave@yahoo.com)
 * Copyright (C) 2003 Will Weisser (ogl@9mm.com)
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
using System.Drawing;

using SdlDotNet;

namespace SdlDotNet.Examples.Rectangles
{
	/// <summary>
	/// A simple SDL.NET Example that draws a bunch of rectangles on the screen. 
	/// Pressing Q or Escape will exit.
	/// </summary>
	public class Rectangles 
	{
		// The screen elements
		private Surface screen;
		private int width = 640;
		private int height = 480;

		// A random number generator to be used for placing the rectangles
		private Random rand = new Random();
		
		/// <summary>
		/// 
		/// </summary>
		public Rectangles() 
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public void Run() 
		{
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyboardDown);
			Events.Tick += new TickEventHandler(this.Tick);
			Events.VideoResize += new VideoResizeEventHandler (this.Resize);
			Events.Quit += new QuitEventHandler(this.Quit);
			
			Events.Fps = 50;

			screen = Video.SetVideoModeWindow(width, height, true);
			Video.WindowIcon();
			Video.WindowCaption = "SDL.NET - Rectangles Example";

			Events.Run();
		}

		private void Resize (object sender, VideoResizeEventArgs e)
		{
			screen = Video.SetVideoModeWindow(e.Width, e.Height, true);
			this.width = e.Width;
			this.height = e.Height;
		}

		private void KeyboardDown(object sender, KeyboardEventArgs e)
		{
			// Check if the key pressed was a Q or Escape
			if (e.Key == Key.Escape || e.Key == Key.Q)
			{
				Events.QuitApplication();
			}
		}

		private void Quit(object sender, QuitEventArgs e)
		{
			Events.QuitApplication();
		}

		private void Tick(object sender, TickEventArgs e)
		{
			// Draw a new random rectangle
			screen.Fill(
				new Rectangle(
				rand.Next(-300, width), rand.Next(-300, height),
				rand.Next(20, 300), rand.Next(20, 300)),
				Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255)));

			// Flip the back buffer onto the screen.
			screen.Update();
		}

		[STAThread]
		static void Main() 
		{
			Rectangles rectangles = new Rectangles();
			rectangles.Run();
		}
	}
}
