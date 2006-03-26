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
	///     Use the accumulation buffer to do full-scene antialiasing on a scene with
	///     orthographic parallel projection.
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
	public class RedBookAccAnti
	{
		#region Fields

		//Width of screen
		int width = 250;
		//Height of screen
		int height = 250;
		
		
		
		
		private const int ACSIZE = 8;

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "AccAnti - Accumulation Buffer";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookAccAnti()
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
				Gl.glOrtho(-2.25, 2.25, -2.25 * h / (float)w, 2.25 * h / (float)w, -10.0, 10.0);
			}
			else 
			{
				Gl.glOrtho(-2.25 * w / (float)h, 2.25 * w / (float)h, -2.25, 2.25, -10.0, 10.0);
			}
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
		}

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		private static void Init()
		{
			
			float[] materialAmbient = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] materialSpecular = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] lightPosition = {0.0f, 0.0f, 10.0f, 1.0f};
			float[] lightModelAmbient = {0.2f, 0.2f, 0.2f, 1.0f};
            
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbient);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
			Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 50.0f);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPosition);
			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, lightModelAmbient);

			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glShadeModel(Gl.GL_FLAT);

			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glClearAccum(0.0f, 0.0f, 0.0f, 0.0f);
		}

		#endregion Lesson Setup

		#region DisplayObjects()
		private static void DisplayObjects() 
		{
			float[] torusDiffuse = {0.7f, 0.7f, 0.0f, 1.0f};
			float[] cubeDiffuse = {0.0f, 0.7f, 0.7f, 1.0f};
			float[] sphereDiffuse = {0.7f, 0.0f, 0.7f, 1.0f};
			float[] octaDiffuse = {0.7f, 0.4f, 0.4f, 1.0f};

			Gl.glPushMatrix();
			Gl.glRotatef(30.0f, 1.0f, 0.0f, 0.0f);

			Gl.glPushMatrix();
			Gl.glTranslatef(-0.80f, 0.35f, 0.0f);
			Gl.glRotatef(100.0f, 1.0f, 0.0f, 0.0f);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, torusDiffuse);
			Glut.glutSolidTorus(0.275f, 0.85f, 16, 16);
			Gl.glPopMatrix();

			Gl.glPushMatrix();
			Gl.glTranslatef(-0.75f, -0.50f, 0.0f);
			Gl.glRotatef(45.0f, 0.0f, 0.0f, 1.0f);
			Gl.glRotatef(45.0f, 1.0f, 0.0f, 0.0f);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, cubeDiffuse);
			Glut.glutSolidCube(1.5f);
			Gl.glPopMatrix();

			Gl.glPushMatrix();
			Gl.glTranslatef(0.75f, 0.60f, 0.0f);
			Gl.glRotatef(30.0f, 1.0f, 0.0f, 0.0f);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, sphereDiffuse);
			Glut.glutSolidSphere(1.0f, 16, 16);
			Gl.glPopMatrix();

			Gl.glPushMatrix();
			Gl.glTranslatef(0.70f, -0.90f, 0.25f);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, octaDiffuse);
			Glut.glutSolidOctahedron();
			Gl.glPopMatrix();
			Gl.glPopMatrix();
		}
		#endregion DisplayObjects()

		#region void Display
		/// <summary>
		/// Renders the scene
		/// </summary>
		private static void Display()
		{
			int[] viewport = new int[4];

			Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

			Gl.glClear(Gl.GL_ACCUM_BUFFER_BIT);
			for(int jitter = 0; jitter < ACSIZE; jitter++) 
			{
				Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
				Gl.glPushMatrix();
				// Note that 4.5 is the distance in world space between left and right and bottom
				// and top.  This formula converts fractional pixel movement to world coordinates.
				Gl.glTranslatef((Jitter.J8[jitter].X * 4.5f) / viewport[2], (Jitter.J8[jitter].Y * 4.5f) / viewport[3], 0.0f);
				DisplayObjects();
				Gl.glPopMatrix();
				Gl.glAccum(Gl.GL_ACCUM, 1.0f / ACSIZE);
			}
			Gl.glAccum(Gl.GL_RETURN, 1.0f);
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