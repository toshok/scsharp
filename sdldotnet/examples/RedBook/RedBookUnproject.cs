#region License
/*
MIT License
Copyright ©2003-2005 Tao Framework Team
http://www.taoframework.com
All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.Reflection;

using SdlDotNet;
using Tao.OpenGl;

namespace SdlDotNet.Examples.RedBook
{
	/// <summary>
	///     When the left mouse button is pressed, this program reads the mouse position and
	///     determines two 3D points from which it was transformed.  Very little is
	///     displayed.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/stroke.c
	///     </para>
	///     <para>
	///         C# Implementation:  Randy Ridge
	///         http://www.taoframework.com
	///     </para>
	///     <para>
	///			SDL.NET implementation: David Hudson
	///			http://cs-sdl.sourceforge.net
	///     </para>
	/// </remarks>
	public class RedBookUnproject
	{
		//Width of screen
		int width = 500;
		//Height of screen
		int height = 500;
		
		

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Unproject - transforms two 3D points";
			}
		}

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookUnproject()
		{
			Initialize();
		}

		#endregion Constructors

		#region Lesson Setup
		/// <summary>
		/// Initializes methods common to all RedBook lessons
		/// </summary>
		private void Initialize()
		{
			// Sets keyboard events
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			Keyboard.EnableKeyRepeat(150,50);
			Events.MouseMotion +=new MouseMotionEventHandler(this.MouseMotion);
			// Sets the ticker to update OpenGL Context
			Events.Tick += new TickEventHandler(this.Tick); 
			Events.Quit += new QuitEventHandler(this.Quit);
			//			// Sets the resize window event
			//			Events.VideoResize += new VideoResizeEventHandler (this.Resize);
			// Set the Frames per second.
			Events.Fps = 60;
			// Creates SDL.NET Surface to hold an OpenGL scene
			Video.SetVideoModeWindowOpenGL(width, height, true);
			// Sets Window icon and title
			this.WindowAttributes();
		}

		/// <summary>
		/// Sets Window icon and caption
		/// </summary>
		private void WindowAttributes()
		{
			Video.WindowIcon();
			Video.WindowCaption = 
				"SDL.NET - RedBook " + 
				this.GetType().ToString().Substring(26);
		}

		#endregion Lesson Setup
		/// <summary>
		/// Resizes window
		/// </summary>
		private void Reshape()
		{
			Reshape(this.width, this.height);
		}

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
			Gl.glFlush();
		}
		#endregion Display()

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluPerspective(45.0, (float) w / (float) h, 1.0, 100.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
		}
		#endregion Reshape(int w, int h)

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.Escape:
					// Will stop the app loop
					Events.QuitApplication();
					break;
				case Key.X:
					Gl.glRotatef(30.0f, 1.0f, 0.0f, 0.0f);
					break;
				case Key.Y:
					Gl.glRotatef(30.0f, 0.0f, 1.0f, 0.0f);
					break;
				case Key.I:
					Gl.glLoadIdentity();
					Glu.gluLookAt(0, 0, 10, 0, 0, 0, 0, 1, 0);
					break;
				default:
					break;
			}
		}

		private void MouseMotion(object sender, MouseMotionEventArgs e)
		{
			int[] viewport = new int[4];
			double[] modelviewMatrix = new double[16];
			double[] projectionMatrix = new double[16];
			int realY;  // OpenGL y coordinate position
			double worldX, worldY, worldZ;  // returned world x, y, z coords

			if(e.ButtonPressed == true) 
			{
				Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);
				Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelviewMatrix);
				Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projectionMatrix);
				// note viewport[3] is height of window in pixels
				realY = viewport[3] - (int) e.Y - 1;
				Console.WriteLine("Coordinates at cursor are ({0:F6}, {1:F6})", e.X, realY);
				Glu.gluUnProject((double) e.X, (double) realY, 0.0, modelviewMatrix, projectionMatrix, viewport, out worldX, out worldY, out worldZ); 
				Console.WriteLine("World coords at z = 0.0 are ({0:F6}, {1:F6}, {2:F6})", worldX, worldY, worldZ);
				Glu.gluUnProject((double) e.X, (double) realY, 1.0, modelviewMatrix, projectionMatrix, viewport, out worldX, out worldY, out worldZ); 
				Console.WriteLine("World coords at z = 1.0 are ({0:F6}, {1:F6}, {2:F6})", worldX, worldY, worldZ);
			}
		}

		private void Tick(object sender, TickEventArgs e)
		{
			Display();
			Video.GLSwapBuffers();
		}

		private void Quit(object sender, QuitEventArgs e)
		{
			Events.QuitApplication();
		}

		//		private void Resize (object sender, VideoResizeEventArgs e)
		//		{
		//			Video.SetVideoModeWindowOpenGL(e.Width, e.Height, true);
		//			if (screen.Width != e.Width || screen.Height != e.Height)
		//			{
		//				//this.Init();
		//				this.Reshape();
		//			}
		//		}

		#endregion Event Handlers

		#region Run Loop
		/// <summary>
		/// Starts demo
		/// </summary>
		public void Run()
		{
			Reshape();
			Events.Run();
		}

		#endregion Run Loop
	}
}