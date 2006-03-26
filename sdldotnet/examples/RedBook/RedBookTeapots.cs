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
	///     This program demonstrates lots of material properties.  A single light source
	///     illuminates the objects.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/teapots.c
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
	public class RedBookTeapots
	{
		//Width of screen
		int width = 500;
		//Height of screen
		int height = 600;
		
		

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Teapots - teapots lots of different material properties";
			}
		}
		#region Private Fields
		private static int teapotList;
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookTeapots()
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
		/// Initialize depth buffer, projection matrix, light source, and lighting model.
		/// Do not specify a material property here.
		/// </summary>
		private static void Init() 
		{
			
			float[] ambient = {0.0f, 0.0f, 0.0f, 1.0f};
			float[] diffuse = {1.0f, 1.0f, 1.0f, 1.0f};
			//float[] specular = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] position = {0.0f, 3.0f, 3.0f, 0.0f};

			float[] lmodel_ambient = {0.2f, 0.2f, 0.2f, 1.0f};
			float[] local_view = {0.0f};

			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambient);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, diffuse);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position);
			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);
			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_LOCAL_VIEWER, local_view);

			Gl.glFrontFace(Gl.GL_CW);
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_AUTO_NORMAL);
			Gl.glEnable(Gl.GL_NORMALIZE);
			Gl.glEnable(Gl.GL_DEPTH_TEST);

			// Be effecient, make teapot display list
			teapotList = Gl.glGenLists(1);
			Gl.glNewList(teapotList, Gl.GL_COMPILE);
			Glut.glutSolidTeapot(1.0);
			Gl.glEndList();
		}
		#endregion Init()

		#region RenderTeapot(float x, float y, float ambr, float ambg, float ambb, float difr, float difg, float difb, float specr, float specg, float specb, float shine)
		/// <summary>
		///     <para>
		///         Move object into position.  Use 3rd through 12th parameters to specify the
		///         material property.  Draw a teapot.
		///     </para>
		/// </summary>
		private static void RenderTeapot(float x, float y, float ambr, float ambg, float ambb, float difr, float difg, float difb, float specr, float specg, float specb, float shine) 
		{
			float[] mat = new float[4];

			Gl.glPushMatrix();
			Gl.glTranslatef(x, y, 0.0f);
			mat[0] = ambr;
			mat[1] = ambg;
			mat[2] = ambb;
			mat[3] = 1.0f;
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, mat);
			mat[0] = difr;
			mat[1] = difg;
			mat[2] = difb;
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, mat);
			mat[0] = specr;
			mat[1] = specg;
			mat[2] = specb;
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, mat);
			Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, shine * 128.0f);
			Gl.glCallList(teapotList);
			Gl.glPopMatrix();
		}
		#endregion RenderTeapot(float x, float y, float ambr, float ambg, float ambb, float difr, float difg, float difb, float specr, float specg, float specb, float shine)

		// --- Callbacks ---
		#region Display()
		/// <summary>
		///     <para>
		///         First column:  emerald, jade, obsidian, pearl, ruby, turquoise.
		///     </para>
		///     <para>
		///         2nd column:  brass, bronze, chrome, copper, gold, silver.
		///     </para>
		///     <para>
		///         3rd column:  black, cyan, green, red, white, yellow plastic.
		///     </para>
		///     <para>
		///         4th column:  black, cyan, green, red, white, yellow rubber.
		///     </para>
		/// </summary>
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			RenderTeapot(2.0f, 17.0f, 0.0215f, 0.1745f, 0.0215f, 0.07568f, 0.61424f, 0.07568f, 0.633f, 0.727811f, 0.633f, 0.6f);
			RenderTeapot(2.0f, 14.0f, 0.135f, 0.2225f, 0.1575f, 0.54f, 0.89f, 0.63f, 0.316228f, 0.316228f, 0.316228f, 0.1f);
			RenderTeapot(2.0f, 11.0f, 0.05375f, 0.05f, 0.06625f, 0.18275f, 0.17f, 0.22525f, 0.332741f, 0.328634f, 0.346435f, 0.3f);
			RenderTeapot(2.0f, 8.0f, 0.25f, 0.20725f, 0.20725f, 1f, 0.829f, 0.829f, 0.296648f, 0.296648f, 0.296648f, 0.088f);
			RenderTeapot(2.0f, 5.0f, 0.1745f, 0.01175f, 0.01175f, 0.61424f, 0.04136f, 0.04136f, 0.727811f, 0.626959f, 0.626959f, 0.6f);
			RenderTeapot(2.0f, 2.0f, 0.1f, 0.18725f, 0.1745f, 0.396f, 0.74151f, 0.69102f, 0.297254f, 0.30829f, 0.306678f, 0.1f);
			RenderTeapot(6.0f, 17.0f, 0.329412f, 0.223529f, 0.027451f, 0.780392f, 0.568627f, 0.113725f, 0.992157f, 0.941176f, 0.807843f, 0.21794872f);
			RenderTeapot(6.0f, 14.0f, 0.2125f, 0.1275f, 0.054f, 0.714f, 0.4284f, 0.18144f, 0.393548f, 0.271906f, 0.166721f, 0.2f);
			RenderTeapot(6.0f, 11.0f, 0.25f, 0.25f, 0.25f, 0.4f, 0.4f, 0.4f, 0.774597f, 0.774597f, 0.774597f, 0.6f);
			RenderTeapot(6.0f, 8.0f, 0.19125f, 0.0735f, 0.0225f, 0.7038f, 0.27048f, 0.0828f, 0.256777f, 0.137622f, 0.086014f, 0.1f);
			RenderTeapot(6.0f, 5.0f, 0.24725f, 0.1995f, 0.0745f, 0.75164f, 0.60648f, 0.22648f, 0.628281f, 0.555802f, 0.366065f, 0.4f);
			RenderTeapot(6.0f, 2.0f, 0.19225f, 0.19225f, 0.19225f, 0.50754f, 0.50754f, 0.50754f, 0.508273f, 0.508273f, 0.508273f, 0.4f);
			RenderTeapot(10.0f, 17.0f, 0.0f, 0.0f, 0.0f, 0.01f, 0.01f, 0.01f, 0.50f, 0.50f, 0.50f, 0.25f);
			RenderTeapot(10.0f, 14.0f, 0.0f, 0.1f, 0.06f, 0.0f, 0.50980392f, 0.50980392f, 0.50196078f, 0.50196078f, 0.50196078f, 0.25f);
			RenderTeapot(10.0f, 11.0f, 0.0f, 0.0f, 0.0f, 0.1f, 0.35f, 0.1f, 0.45f, 0.55f, 0.45f, 0.25f);
			RenderTeapot(10.0f, 8.0f, 0.0f, 0.0f, 0.0f, 0.5f, 0.0f, 0.0f, 0.7f, 0.6f, 0.6f, 0.25f);
			RenderTeapot(10.0f, 5.0f, 0.0f, 0.0f, 0.0f, 0.55f, 0.55f, 0.55f, 0.70f, 0.70f, 0.70f, 0.25f);
			RenderTeapot(10.0f, 2.0f, 0.0f, 0.0f, 0.0f, 0.5f, 0.5f, 0.0f, 0.60f, 0.60f, 0.50f, 0.25f);
			RenderTeapot(14.0f, 17.0f, 0.02f, 0.02f, 0.02f, 0.01f, 0.01f, 0.01f, 0.4f, 0.4f, 0.4f, 0.078125f);
			RenderTeapot(14.0f, 14.0f, 0.0f, 0.05f, 0.05f, 0.4f, 0.5f, 0.5f, 0.04f, 0.7f, 0.7f, 0.078125f);
			RenderTeapot(14.0f, 11.0f, 0.0f, 0.05f, 0.0f, 0.4f, 0.5f, 0.4f, 0.04f, 0.7f, 0.04f, 0.078125f);
			RenderTeapot(14.0f, 8.0f, 0.05f, 0.0f, 0.0f, 0.5f, 0.4f, 0.4f, 0.7f, 0.04f, 0.04f, 0.078125f);
			RenderTeapot(14.0f, 5.0f, 0.05f, 0.05f, 0.05f, 0.5f, 0.5f, 0.5f, 0.7f, 0.7f, 0.7f, 0.078125f);
			RenderTeapot(14.0f, 2.0f, 0.05f, 0.05f, 0.0f, 0.5f, 0.5f, 0.4f, 0.7f, 0.7f, 0.04f, 0.078125f);
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
				Gl.glOrtho(0.0, 16.0, 0.0, 16.0 * h / w, -10.0, 10.0);
			}
			else 
			{
				Gl.glOrtho(0.0, 16.0 * w / h, 0.0, 16.0, -10.0, 10.0);
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
