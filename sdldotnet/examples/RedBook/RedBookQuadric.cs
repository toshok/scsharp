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
	///     This program demonstrates the use of some of the Glu.gluQuadric* routines.
	///     Quadric objects are created with some quadric properties and the callback routine
	///     to handle errors.  Note that the cylinder has no top or bottom and the circle
	///     has a hole in it.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/planet.c
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
	public class RedBookQuadric
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
				return "Quadric - Quadric objects";
			}
		}

		#region Private Fields
		private static int startList;
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookQuadric()
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
			Glu.GLUquadric quadric;
			float[] materialAmbient = {0.5f, 0.5f, 0.5f, 1.0f};
			float[] materialSpecular = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] materialShininess = {50.0f};
			float[] lightPosition = {1.0f, 1.0f, 1.0f, 0.0f};
			float[] modelAmbient = {0.5f, 0.5f, 0.5f, 1.0f};

			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);

			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbient);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, materialShininess);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPosition);
			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, modelAmbient);

			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_DEPTH_TEST);

			//  Create 4 display lists, each with a different quadric object.  Different drawing
			//  styles and surface normal specifications are demonstrated.
			startList = Gl.glGenLists(4);
			quadric = Glu.gluNewQuadric();
			Glu.gluQuadricCallback(quadric, Glu.GLU_ERROR, new Glu.QuadricErrorCallback(Error));

			Glu.gluQuadricDrawStyle(quadric, Glu.GLU_FILL); // smooth shaded
			Glu.gluQuadricNormals(quadric, Glu.GLU_SMOOTH);
			Gl.glNewList(startList, Gl.GL_COMPILE);
			Glu.gluSphere(quadric, 0.75, 15, 10);
			Gl.glEndList();

			Glu.gluQuadricDrawStyle(quadric, Glu.GLU_FILL); // flat shaded
			Glu.gluQuadricNormals(quadric, Glu.GLU_FLAT);
			Gl.glNewList(startList + 1, Gl.GL_COMPILE);
			Glu.gluCylinder(quadric, 0.5, 0.3, 1.0, 15, 5);
			Gl.glEndList();

			Glu.gluQuadricDrawStyle(quadric, Glu.GLU_LINE); // all polygons wireframe
			Glu.gluQuadricNormals(quadric, Glu.GLU_NONE);
			Gl.glNewList(startList + 2, Gl.GL_COMPILE);
			Glu.gluDisk(quadric, 0.25, 1.0, 20, 4);
			Gl.glEndList();

			Glu.gluQuadricDrawStyle(quadric, Glu.GLU_SILHOUETTE); // boundary only
			Glu.gluQuadricNormals(quadric, Glu.GLU_NONE);
			Gl.glNewList(startList + 3, Gl.GL_COMPILE);
			Glu.gluPartialDisk(quadric, 0.0, 1.0, 20, 4, 0.0, 225.0);
			Gl.glEndList();
		}
		#endregion Init()

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glPushMatrix();
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glShadeModel(Gl.GL_SMOOTH);
			Gl.glTranslatef(-1.0f, -1.0f, 0.0f);
			Gl.glCallList(startList);

			Gl.glShadeModel(Gl.GL_FLAT);
			Gl.glTranslatef(0.0f, 2.0f, 0.0f);
			Gl.glPushMatrix();
			Gl.glRotatef(300.0f, 1.0f, 0.0f, 0.0f);
			Gl.glCallList(startList + 1);
			Gl.glPopMatrix();

			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glColor3f(0.0f, 1.0f, 1.0f);
			Gl.glTranslatef(2.0f, -2.0f, 0.0f);
			Gl.glCallList(startList + 2);

			Gl.glColor3f(1.0f, 1.0f, 0.0f);
			Gl.glTranslatef(0.0f, 2.0f, 0.0f);
			Gl.glCallList(startList + 3);
			Gl.glPopMatrix();
			Gl.glFlush();
		}
		#endregion Display()

		#region void Error(int errorCode)
		private static void Error(int errorCode) 
		{
			string errorString;
			errorString = Glu.gluErrorString(errorCode);
			Console.WriteLine("Quadric Error: {0}", errorString);
			Environment.Exit(1);
		}
		#endregion void Error(int errorCode)

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			if(w <= h) 
			{
				Gl.glOrtho(-2.5, 2.5, -2.5 * (float) h / (float) w, 2.5 * (float) h / (float) w, -10.0, 10.0);
			}
			else 
			{
				Gl.glOrtho(-2.5 * (float) w / (float) h, 2.5 * (float) w / (float) h, -2.5, 2.5, -10.0, 10.0);
			}
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