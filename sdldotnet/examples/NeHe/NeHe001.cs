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

namespace SdlDotNet.Examples.NeHe
{
	/// <summary>
	/// Lesson 01: Setting Up An OpenGL Window
	/// </summary>
	public class NeHe001
	{
		#region Fields

		//Width of screen
		int width = 640;
		//Height of screen
		int height = 480;
		// Bits per pixel of screen
		int bpp = 16;
		// Surface to render on
		Surface screen;

		/// <summary>
		/// Width of window
		/// </summary>
		protected int Width
		{
			get
			{
				return width;
			}
		}

		/// <summary>
		/// Height of window
		/// </summary>
		protected int Height
		{
			get
			{
				return height;
			}
		}

		/// <summary>
		/// Bits per pixel of surface
		/// </summary>
		protected int BitsPerPixel
		{
			get
			{
				return this.bpp;
			}
		}

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Lesson 01: Setting Up An OpenGL Window";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public NeHe001()
		{
			Initialize();
		}

		#endregion Constructors

		#region Lesson Setup
		/// <summary>
		/// Initializes methods common to all NeHe lessons
		/// </summary>
		protected void Initialize()
		{
			// Sets keyboard events
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			// Sets the ticker to update OpenGL Context
			Events.Tick += new TickEventHandler(this.Tick);
			Events.Quit += new QuitEventHandler(this.Quit);
//			// Sets the resize window event
//			Events.VideoResize += new VideoResizeEventHandler (this.Resize);
			// Set the Frames per second.
			Events.Fps = 60;
			// Creates SDL.NET Surface to hold an OpenGL scene
			screen = Video.SetVideoModeWindowOpenGL(width, height, true);
			// Sets Window icon and title
			this.WindowAttributes();
		}

		/// <summary>
		/// Sets Window icon and caption
		/// </summary>
		protected void WindowAttributes()
		{
			Video.WindowIcon();
			Video.WindowCaption = 
				"SDL.NET - NeHe Lesson " + 
				this.GetType().ToString().Substring(
				this.GetType().ToString().Length-3);
		}

		/// <summary>
		/// Resizes window
		/// </summary>
		protected virtual void Reshape()
		{
			this.Reshape(100.0F);
		}

		/// <summary>
		/// Resizes window
		/// </summary>
		/// <param name="distance"></param>
		protected virtual void Reshape(float distance)
		{
			// Reset The Current Viewport
			Gl.glViewport(0, 0, width, height);
			// Select The Projection Matrix
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			// Reset The Projection Matrix
			Gl.glLoadIdentity();
			// Calculate The Aspect Ratio Of The Window
			Glu.gluPerspective(45.0F, (width / (float)height), 0.1F, distance);
			// Select The Modelview Matrix
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			// Reset The Modelview Matrix
			Gl.glLoadIdentity();
		}

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		protected virtual void InitGL()
		{ 
			// Enable Smooth Shading
			Gl.glShadeModel(Gl.GL_SMOOTH);
			// Black Background
			Gl.glClearColor(0.0F, 0.0F, 0.0F, 0.5F);
			// Depth Buffer Setup
			Gl.glClearDepth(1.0F);
			// Enables Depth Testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			// The Type Of Depth Testing To Do
			Gl.glDepthFunc(Gl.GL_LEQUAL);
			// Really Nice Perspective Calculations
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
		}

		#endregion Lesson Setup

		#region void DrawGLScene
		/// <summary>
		/// Renders the scene
		/// </summary>
		protected virtual void DrawGLScene()
		{
			// Clear Screen And Depth Buffer
			Gl.glClear((Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT));
			// Reset The Current Modelview Matrix
			Gl.glLoadIdentity();
		}
		#endregion void DrawGLScene

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.Escape:
					// Will stop the app loop
					Events.QuitApplication();
					break;
				case Key.F1:
					// Toggle fullscreen
					if ((screen.FullScreen)) 
					{
						screen = Video.SetVideoModeWindowOpenGL(width, height, true);
						this.WindowAttributes();
					}
					else 
					{
						screen = Video.SetVideoModeOpenGL(width, height, bpp);
					}
					Reshape();
					break;
			}
		}

		private void Tick(object sender, TickEventArgs e)
		{
			DrawGLScene();
			Video.GLSwapBuffers();
		}

		private void Quit(object sender, QuitEventArgs e)
		{
			Events.QuitApplication();
		}

//		private void Resize (object sender, VideoResizeEventArgs e)
//		{
//			screen = Video.SetVideoModeWindowOpenGL(e.Width, e.Height, true);
//			if (screen.Width != e.Width || screen.Height != e.Height)
//			{
//				//this.InitGL();
//				this.Reshape();
//			}
//		}

		#endregion Event Handlers

		#region Run Loop
		/// <summary>
		/// Starts lesson
		/// </summary>
		public void Run()
		{
			Reshape();
			InitGL();
			Events.Run();
		}

		#endregion Run Loop
	}
}