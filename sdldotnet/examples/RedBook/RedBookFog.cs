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
	///     This program draws 5 red spheres, each at a different z distance from the eye,
	///     in different types of fog.  Pressing the f key chooses between 3 types of 
	///     fog:  exponential, exponential squared, and linear.  In this program, there is
	///     a fixed density value, as well as fixed start and end values for the linear fog.
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
	public class RedBookFog
	{
		#region Fields

		//Width of screen
		int width = 500;
		//Height of screen
		int height = 500;
		
		
        private static int fogMode;

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Fog - Different types of fog";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookFog()
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
				Gl.glOrtho(-2.5, 2.5, -2.5 * (float) h / (float) w, 2.5 * (float) h / (float) w, -10.0, 10.0);
			}
			else 
			{
				Gl.glOrtho(-2.5 * (float) w / (float) h, 2.5 * (float) w / (float) h, -2.5, 2.5, -10.0, 10.0);
			}
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
		}

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		private static void Init()
		{
			
			float[] position = {0.5f, 0.5f, 3.0f, 0.0f};

			Gl.glEnable(Gl.GL_DEPTH_TEST);

			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position);
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);

			float[] mat = {0.1745f, 0.01175f, 0.01175f};
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, mat);
			mat[0] = 0.61424f;
			mat[1] = 0.04136f;
			mat[2] = 0.04136f;
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, mat);
			mat[0] = 0.727811f;
			mat[1] = 0.626959f;
			mat[2] = 0.626959f;
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, mat);
			Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 0.6f * 128.0f);

			Gl.glEnable(Gl.GL_FOG);
			float[] fogColor = {0.5f, 0.5f, 0.5f, 1.0f};

			fogMode = Gl.GL_EXP;
			Gl.glFogi(Gl.GL_FOG_MODE, fogMode);
			Gl.glFogfv(Gl.GL_FOG_COLOR, fogColor);
			Gl.glFogf(Gl.GL_FOG_DENSITY, 0.35f);
			Gl.glHint(Gl.GL_FOG_HINT, Gl.GL_DONT_CARE);
			Gl.glFogf(Gl.GL_FOG_START, 1.0f);
			Gl.glFogf(Gl.GL_FOG_END, 5.0f);

			Gl.glClearColor(0.5f, 0.5f, 0.5f, 1.0f);  // fog color
		}

		#endregion Lesson Setup

		#region void Display
		/// <summary>
		/// Renders the scene
		/// </summary>
		private static void Display()
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			RenderSphere(-2.0f, -0.5f, -1.0f);
			RenderSphere(-1.0f, -0.5f, -2.0f);
			RenderSphere(0.0f, -0.5f, -3.0f);
			RenderSphere(1.0f, -0.5f, -4.0f);
			RenderSphere(2.0f, -0.5f, -5.0f);
			Gl.glFlush();

		}
		#endregion void Display

		#region RenderSphere(float x, float y, float z)
		private static void RenderSphere(float x, float y, float z) 
		{
			Gl.glPushMatrix();
			Gl.glTranslatef(x, y, z);
			Glut.glutSolidSphere(0.4, 16, 16);
			Gl.glPopMatrix();
		}
		#endregion RenderSphere(float x, float y, float z)
		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.Escape:
					// Will stop the app loop
					Events.QuitApplication();
					break;
				case Key.F:
					if(fogMode == Gl.GL_EXP) 
					{
						fogMode = Gl.GL_EXP2;
						Console.WriteLine("Fog mode is GL_EXP2");
					}
					else if(fogMode == Gl.GL_EXP2) 
					{
						fogMode = Gl.GL_LINEAR;
						Console.WriteLine("Fog mode is GL_LINEAR");
					}
					else if(fogMode == Gl.GL_LINEAR) 
					{
						fogMode = Gl.GL_EXP;
						Console.WriteLine("Fog mode is GL_EXP");
					}
					Gl.glFogi(Gl.GL_FOG_MODE, fogMode);
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