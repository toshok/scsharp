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
	///     This program draws several overlapping filled polygons to demonstrate the effect
	///     order has on alpha blending results.  Use the 't' key to toggle the order of
	///     drawing polygons.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/accanti.c
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
	public class RedBookAlpha
	{
		#region Fields

		//Width of screen
		int width = 200;
		//Height of screen
		int height = 200;
		
		
		
        private static bool leftFirst = true;

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Alpha - Alpha Blending";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookAlpha()
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
			if(w <= h) 
			{
				Glu.gluOrtho2D(0.0, 1.0, 0.0, 1.0 * h / w);
			}
			else 
			{
				Glu.gluOrtho2D(0.0, 1.0 * w / h, 0.0, 1.0);
			}
		}

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		private static void Init()
		{
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
			Gl.glShadeModel(Gl.GL_FLAT);
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
		}

		#endregion Lesson Setup

		#region DrawLeftTriangle()
		/// <summary>
		///     <para>
		///         Draws yellow triangle on left hand side of screen.
		///     </para>
		/// </summary>
		private static void DrawLeftTriangle() 
		{
			Gl.glBegin(Gl.GL_TRIANGLES);
			Gl.glColor4f(1.0f, 1.0f, 0.0f, 0.75f);
			Gl.glVertex3f(0.1f, 0.9f, 0.0f);
			Gl.glVertex3f(0.1f, 0.1f, 0.0f);
			Gl.glVertex3f(0.7f, 0.5f, 0.0f);
			Gl.glEnd();
		}
		#endregion DrawLeftTriangle()

		#region DrawRightTriangle()
		/// <summary>
		///     <para>
		///         Draws cyan triangle on right hand side of screen.
		///     </para>
		/// </summary>
		private static void DrawRightTriangle() 
		{
			Gl.glBegin(Gl.GL_TRIANGLES);
			Gl.glColor4f(0.0f, 1.0f, 1.0f, 0.75f);
			Gl.glVertex3f(0.9f, 0.9f, 0.0f);
			Gl.glVertex3f(0.3f, 0.5f, 0.0f);
			Gl.glVertex3f(0.9f, 0.1f, 0.0f);
			Gl.glEnd();
		}
		#endregion DrawRightTriangle()

		#region void Display
		/// <summary>
		/// Renders the scene
		/// </summary>
		private static void Display()
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
			if(leftFirst) 
			{
				DrawLeftTriangle();
				DrawRightTriangle();
			}
			else 
			{
				DrawRightTriangle();
				DrawLeftTriangle();
			}
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
				case Key.T:
					leftFirst = !leftFirst;
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