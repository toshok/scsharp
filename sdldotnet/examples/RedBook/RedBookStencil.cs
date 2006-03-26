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
using Tao.FreeGlut;

namespace SdlDotNet.Examples.RedBook
{
	/// <summary>
	///     This program demonstrates use of the stencil buffer for masking nonrectangular
	///     regions.  Whenever the window is redrawn, a value of 1 is drawn into a
	///     diamond-shaped region in the stencil buffer.  Elsewhere in the stencil buffer,
	///     the value is 0.  Then a blue sphere is drawn where the stencil value is 1,
	///     and yellow torii are drawn where the stencil value is not 1.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Mark J. Kilgard
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
	public class RedBookStencil
	{
		//Width of screen
		int width = 400;
		//Height of screen
		int height = 400;
		
		

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Stencil - stencil buffer";
			}
		}

		#region Private Constants
		private const int YELLOWMAT = 1;
		private const int BLUEMAT = 2;
		#endregion Private Constants

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookStencil()
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
		private static void Init() 
		{
			
			float[] yellowDiffuse = {0.7f, 0.7f, 0.0f, 1.0f};
			float[] yellowSpecular = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] blueDiffuse = {0.1f, 0.1f, 0.7f, 1.0f};
			float[] blueSpecular = {0.1f, 1.0f, 1.0f, 1.0f};
			float[] position = {1.0f, 1.0f, 1.0f, 0.0f};

			Gl.glNewList(YELLOWMAT, Gl.GL_COMPILE);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, yellowDiffuse);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, yellowSpecular);
			Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 64.0f);
			Gl.glEndList();

			Gl.glNewList(BLUEMAT, Gl.GL_COMPILE);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, blueDiffuse);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, blueSpecular);
			Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 45.0f);
			Gl.glEndList();

			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position);

			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_LIGHTING);

			Gl.glClearStencil(0x0);
			Gl.glEnable(Gl.GL_STENCIL_TEST);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Gl.glTranslatef(0.0f, 0.0f, -5.0f);
		}
		#endregion Init()

		// --- Callbacks ---
		#region Display()
		/// <summary>
		///     <para>
		///         Draw a sphere in a diamond-shaped section in the middle of a window with 2
		///         torii.
		///     </para>
		/// </summary>
		private static void Display() 
		{
			Gl.glClear(Gl.GL_STENCIL_BUFFER_BIT);

			// create a diamond shaped stencil area
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPushMatrix();
			Gl.glLoadIdentity();
			Gl.glOrtho(-3.0, 3.0, -3.0, 3.0, -1.0, 1.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glPushMatrix();
			Gl.glLoadIdentity();

			// Disable color buffer update.
			Gl.glColorMask(0, 0, 0, 0);
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			Gl.glStencilFunc(Gl.GL_ALWAYS, 0x1, 0x1);
			Gl.glStencilOp(Gl.GL_REPLACE, Gl.GL_REPLACE, Gl.GL_REPLACE);

			Gl.glBegin(Gl.GL_QUADS);
			Gl.glVertex3f(-1.0f, 0.0f, 0.0f);
			Gl.glVertex3f(0.0f, 1.0f, 0.0f);
			Gl.glVertex3f(1.0f, 0.0f, 0.0f);
			Gl.glVertex3f(0.0f, -1.0f, 0.0f);
			Gl.glEnd();
			Gl.glPopMatrix();
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPopMatrix();
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
    
			// Enable color buffer update.
			Gl.glColorMask(1, 1, 1, 1);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_KEEP, Gl.GL_KEEP);

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

			// draw blue sphere where the stencil is 1
			Gl.glStencilFunc(Gl.GL_EQUAL, 0x1, 0x1);
			Gl.glCallList(BLUEMAT);
			Glut.glutSolidSphere(0.5, 15, 15);

			// draw the tori where the stencil is not 1
			Gl.glStencilFunc(Gl.GL_NOTEQUAL, 0x1, 0x1);
			Gl.glPushMatrix();
			Gl.glRotatef(45.0f, 0.0f, 0.0f, 1.0f);
			Gl.glRotatef(45.0f, 0.0f, 1.0f, 0.0f);
			Gl.glCallList(YELLOWMAT);
			Glut.glutSolidTorus(0.275, 0.85, 15, 15);
			Gl.glPushMatrix();
			Gl.glRotatef(90.0f, 1.0f, 0.0f, 0.0f);
			Glut.glutSolidTorus(0.275, 0.85, 15, 15);
			Gl.glPopMatrix();
			Gl.glPopMatrix();

			Gl.glFlush();
		}
		#endregion Display()

		#region Reshape(int w, int h)
		/// <summary>
		///     <para>
		///         Whenever the window is reshaped, redefine the coordinate system and redraw
		///         the stencil area.
		///     </para>
		/// </summary>
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluPerspective(45.0, (float) w / (float) h, 3.0, 7.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
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