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
	///     This program demonstrates geometric primitives and their attributes.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
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
	public class RedBookLines
	{
		#region Fields

		//Width of screen
		int width = 400;
		//Height of screen
		int height = 150;
		
		

		private const int CHECKWIDTH = 64;
		private const int CHECKHEIGHT = 64;

		//private byte[ , , ] checkImage = new byte[CHECKWIDTH, CHECKHEIGHT, 3];
		private double zoomFactor = 1.0;

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Lines - Geometric primitives";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookLines()
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

		/// <summary>
		/// Resizes window
		/// </summary>
		private void Reshape()
		{
			Reshape(this.width, this.height);
		}

		/// <summary>
		/// Resizes window
		/// </summary>
		/// <param name="h"></param>
		/// <param name="w"></param>
		private static void Reshape(int w, int h)
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluOrtho2D(0.0, (double) w, 0.0, (double) h);
		}

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		private static void Init()
		{
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glShadeModel(Gl.GL_FLAT);
		}

		#endregion Lesson Setup

		#region DrawOneLine(float x1, float y1, float x2, float y2)
		private static void DrawOneLine(float x1, float y1, float x2, float y2) 
		{
			Gl.glBegin(Gl.GL_LINES);
			Gl.glVertex2f(x1, y1);
			Gl.glVertex2f(x2, y2);
			Gl.glEnd();
		}
		#endregion DrawOneLine(float x1, float y1, float x2, float y2)

		#region void Display
		/// <summary>
		/// Renders the scene
		/// </summary>
		private static void Display()
		{
			int i;

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

			// select white for all lines
			Gl.glColor3f(1.0f, 1.0f, 1.0f);

			// in 1st row, 3 lines, each with a different stipple
			Gl.glEnable(Gl.GL_LINE_STIPPLE);

			Gl.glLineStipple(1, 0x0101);  // dotted
			DrawOneLine(50.0f, 125.0f, 150.0f, 125.0f);
			Gl.glLineStipple(1, 0x00FF);  // dashed
			DrawOneLine(150.0f, 125.0f, 250.0f, 125.0f);
			Gl.glLineStipple(1, 0x1C47);  // dash/dot/dash
			DrawOneLine(250.0f, 125.0f, 350.0f, 125.0f);

			// in 2nd row, 3 wide lines, each with different stipple
			Gl.glLineWidth(5.0f);
			Gl.glLineStipple(1, 0x0101);  // dotted
			DrawOneLine(50.0f, 100.0f, 150.0f, 100.0f);
			Gl.glLineStipple(1, 0x00FF);  // dashed
			DrawOneLine(150.0f, 100.0f, 250.0f, 100.0f);
			Gl.glLineStipple(1, 0x1C47);  // dash/dot/dash
			DrawOneLine(250.0f, 100.0f, 350.0f, 100.0f);
			Gl.glLineWidth(1.0f);

			// in 3rd row, 6 lines, with dash/dot/dash stipple
			// as part of a single connected line strip
			Gl.glLineStipple(1, 0x1C47);  // dash/dot/dash
			Gl.glBegin(Gl.GL_LINE_STRIP);
			for(i = 0; i < 7; i++) 
			{
				Gl.glVertex2f(50.0f + ((float) i * 50.0f), 75.0f);
			}
			Gl.glEnd();

			// in 4th row, 6 independent lines with same stipple
			for(i = 0; i < 6; i++) 
			{
				DrawOneLine(50.0f + ((float) i * 50.0f), 50.0f, 50.0f + ((float)(i + 1) * 50.0f), 50.0f);
			}

			// in 5th row, 1 line, with dash/dot/dash stipple
			// and a stipple repeat factor of 5
			Gl.glLineStipple(5, 0x1C47);  // dash/dot/dash
			DrawOneLine(50.0f, 25.0f, 350.0f, 25.0f);

			Gl.glDisable(Gl.GL_LINE_STIPPLE);
			Gl.glFlush();
		}
		#endregion void Display

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.Escape:
					// Will stop the app loop
					Events.QuitApplication();
					break;
				case Key.R:
					zoomFactor = 1.0;
					Console.WriteLine("zoomFactor reset to 1.0");
					break;
				case Key.Z:
					zoomFactor += 0.5;
					if(zoomFactor >= 3.0) 
					{
						zoomFactor = 3.0;
					}
					Console.WriteLine("zoomFactor is now {0:F1}", zoomFactor);
					break;
				case Key.A:
					zoomFactor -= 0.5;
					if(zoomFactor <= 0.5) 
					{
						zoomFactor = 0.5;
					}
					Console.WriteLine("zoomFactor is now {0:F1}", zoomFactor);
					break;
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
			Init();
			Events.Run();
		}

		#endregion Run Loop
	}
}
