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
	///     <para>
	///         This program demonstrates when to issue lighting and transformation commands to
	///         render a model with a light which is moved by a modeling transformation (rotate
	///         or translate).  The light position is reset after the modeling transformation is
	///         called.  The eye position does not change.
	///     </para>
	///     <para>
	///         A sphere is drawn using a grey material characteristic.  A single light source
	///         illuminates the object.
	///     </para>
	///     <para>
	///         Interaction:  pressing the left mouse button alters the modeling transformation
	///         (x rotation) by 30 degrees.  The scene is then redrawn with the light in a new
	///         position.
	///     </para>
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
	public class RedBookMoveLight
	{
		#region Fields

		//Width of screen
		int width = 500;
		//Height of screen
		int height = 500;
		
		

		private static int spin = 0;

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "MoveLight - Render a model with a light";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookMoveLight()
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
			Events.MouseButtonDown += new MouseButtonEventHandler(this.MouseButtonDown);
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
		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluPerspective(40.0, (float) w / (float) h, 1.0, 20.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
		}
		#endregion Reshape(int w, int h)

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		private static void Init()
		{
			
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glShadeModel(Gl.GL_SMOOTH);
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
		}

		#endregion Lesson Setup

		#region void Display
		/*  Draw twelve spheres in 3 rows with 4 columns.  
			*   The spheres in the first row have materials with no ambient reflection.
			*   The second row has materials with significant ambient reflection.
			*   The third row has materials with colored ambient reflection.
			*
			*   The first column has materials with blue, diffuse reflection only.
			*   The second column has blue diffuse reflection, as well as specular
			*   reflection with a low shininess exponent.
			*   The third column has blue diffuse reflection, as well as specular
			*   reflection with a high shininess exponent (a more concentrated highlight).
			*   The fourth column has materials which also include an emissive component.
			*
			*   Gl.glTranslatef() is used to move spheres to their appropriate locations.
			*/
		private static void Display()
		{
			float[] position = {0.0f, 0.0f, 1.5f, 1.0f};

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glPushMatrix();
			Glu.gluLookAt(0.0, 0.0, 5.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);

			Gl.glPushMatrix();
			Gl.glRotated((double) spin, 1.0, 0.0, 0.0);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position);

			Gl.glTranslated(0.0, 0.0, 1.5);
			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glColor3f(0.0f, 1.0f, 1.0f);
			Glut.glutWireCube(0.1);
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glPopMatrix();

			Glut.glutSolidTorus(0.275, 0.85, 8, 15);
			Gl.glPopMatrix();
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

		private void MouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ButtonPressed)
			{
				spin = (spin + 30) % 360;
			}

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