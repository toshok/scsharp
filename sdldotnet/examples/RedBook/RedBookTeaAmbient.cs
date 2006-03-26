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
	///     This program renders three lighted, shaded teapots, with different ambient values.
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
	public class RedBookTeaAmbient
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
				return "TeaAmbient - teapots with different ambient values";
			}
		}

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookTeaAmbient()
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
		///         Initialize light source and lighting model.
		///     </para>
		/// </summary>
		private static void Init() 
		{
			
			float[] lightAmbient = {0.0f, 0.0f, 0.0f, 1.0f};
			float[] lightDiffuse = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] lightSpecular = {1.0f, 1.0f, 1.0f, 1.0f};
			// lightPosition is NOT default value
			float[] lightPosition = {1.0f, 0.0f, 0.0f, 0.0f};
			float[] globalAmbient = {0.75f, 0.75f, 0.75f, 1.0f};

			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, lightAmbient);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, lightDiffuse);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, lightSpecular);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPosition);

			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, globalAmbient);

			Gl.glFrontFace(Gl.GL_CW);
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_AUTO_NORMAL);
			Gl.glEnable(Gl.GL_NORMALIZE);
			Gl.glDepthFunc(Gl.GL_LESS);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
		}
		#endregion Init()

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			float[] lowAmbient = {0.1f, 0.1f, 0.1f, 1.0f};
			float[] moreAmbient = {0.4f, 0.4f, 0.4f, 1.0f};
			float[] mostAmbient = {1.0f, 1.0f, 1.0f, 1.0f};

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

			// material has small ambient reflection
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, lowAmbient);
			Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 40.0f);
			Gl.glPushMatrix();
			Gl.glTranslatef(0.0f, 2.0f, 0.0f);
			Glut.glutSolidTeapot(1.0);
			Gl.glPopMatrix();

			// material has moderate ambient reflection
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, moreAmbient);
			Gl.glPushMatrix();
			Gl.glTranslatef(0.0f, 0.0f, 0.0f);
			Glut.glutSolidTeapot(1.0);
			Gl.glPopMatrix();

			// material has large ambient reflection
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, mostAmbient);
			Gl.glPushMatrix();
			Gl.glTranslatef(0.0f, -2.0f, 0.0f);
			Glut.glutSolidTeapot(1.0);
			Gl.glPopMatrix();
			Gl.glFlush();
		}
		#endregion Display()

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			if(w <= h) 
			{
				Gl.glOrtho(-4.0, 4.0, -4.0 * (float) h / (float) w, 4.0 * (float) h / (float) w, -10.0, 10.0);
			}
			else 
			{
				Gl.glOrtho(-4.0 * (float) w / (float) h, 4.0 * (float) w / (float) h, -4.0, 4.0, -10.0, 10.0);
			}
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