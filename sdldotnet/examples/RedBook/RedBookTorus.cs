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
	///     This program demonstrates the creation of a display list.
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
	public class RedBookTorus
	{
		//Width of screen
		int width = 200;
		//Height of screen
		int height = 200;
		
		

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Torus - creation of a display list";
			}
		}

		#region Private Fields
		private static int torus;
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookTorus()
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

		#endregion Lesson Setup
		/// <summary>
		/// Resizes window
		/// </summary>
		private void Reshape()
		{
			Reshape(this.width, this.height);
		}

		// --- Application Methods ---
		#region Init()
		/// <summary>
		///     <para>
		///         Create display list with torus and initialize state.
		///     </para>
		/// </summary>
		private static void Init() 
		{
			torus = Gl.glGenLists(1);
			Gl.glNewList(torus, Gl.GL_COMPILE);
			DrawTorus(8, 25);
			Gl.glEndList();

			Gl.glShadeModel(Gl.GL_FLAT);
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
		}
		#endregion Init()

		#region DrawTorus(int c, int t)
		/// <summary>
		///     <para>
		///         Draw a torus.
		///     </para>
		/// </summary>
		private static void DrawTorus(int numc, int numt) 
		{
			int i, j, k;
			double s, t, x, y, z, twoPi;

			twoPi = 2 * System.Math.PI;
			for(i = 0; i < numc; i++) 
			{
				Gl.glBegin(Gl.GL_QUAD_STRIP);
				for(j = 0; j <= numt; j++) 
				{
					for(k = 1; k >= 0; k--) 
					{
						s = (i + k) % numc + 0.5;
						t = j % numt;

						x = (1 + 0.1 * Math.Cos(s * twoPi / numc)) * Math.Cos(t * twoPi / numt);
						y = (1 + 0.1 * Math.Cos(s * twoPi / numc)) * Math.Sin(t * twoPi / numt);
						z = 0.1 * Math.Sin(s * twoPi / numc);
						Gl.glVertex3f((float) x, (float) y, (float) z);
					}
				}
				Gl.glEnd();
			}
		}
		#endregion DrawTorus(int c, int t)

		// --- Callbacks ---
		#region Display()
		/// <summary>
		///     <para>
		///         Clear window and draw torus.
		///     </para>
		/// </summary>
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
			Gl.glColor3f(1.0f, 1.0f, 1.0f);
			Gl.glCallList(torus);
			Gl.glFlush();
		}
		#endregion Display()

		#region Reshape(int w, int h)
		/// <summary>
		///     <para>
		///         Handle window resize.
		///     </para>
		/// </summary>
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluPerspective(30, (float) w / (float) h, 1.0, 100.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Glu.gluLookAt(0, 0, 10, 0, 0, 0, 0, 1, 0);
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