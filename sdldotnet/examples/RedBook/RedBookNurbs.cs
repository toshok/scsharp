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
	///     This program shows a NURBS (Non-uniform rational B-splines) surface, shaped like
	///     a heart.
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
	public class RedBookNurbs
	{
		//Width of screen
		int width = 500;
		//Height of screen
		int height = 500;
		
		

		#region Private Constants
		private const int SPOINTS = 13;
		private const int SORDER = 3;
		private const int SKNOTS = (SPOINTS + SORDER);
		private const int TPOINTS = 3;
		private const int TORDER = 3;
		private const int TKNOTS = (TPOINTS + TORDER);
		private const float SQRT2 = 1.41421356237309504880f;
		#endregion Private Constants

		#region Private Fields
		private  float[] sknots = {
											-1.0f, -1.0f, -1.0f, 0.0f, 1.0f, 2.0f, 3.0f, 4.0f,
											4.0f,  5.0f,  6.0f, 7.0f, 8.0f, 9.0f, 9.0f, 9.0f
										};
		private  float[] tknots = {1.0f, 1.0f, 1.0f, 2.0f, 2.0f, 2.0f};
		private  float[/*S_NUMPOINTS*/, /*T_NUMPOINTS*/, /*4*/] controlPoints = {
			{
				{4.0f, 2.0f, 2.0f, 1.0f},
				{4.0f, 1.6f, 2.5f, 1.0f},
				{4.0f, 2.0f, 3.0f, 1.0f}
			},
			{
				{5.0f, 4.0f, 2.0f, 1.0f},
				{5.0f, 4.0f, 2.5f, 1.0f},
				{5.0f, 4.0f, 3.0f, 1.0f}
			},
			{
				{6.0f, 5.0f, 2.0f, 1.0f},
				{6.0f, 5.0f, 2.5f, 1.0f},
				{6.0f, 5.0f, 3.0f, 1.0f}
			},
			{
				{SQRT2 * 6.0f, SQRT2 * 6.0f, SQRT2 * 2.0f, SQRT2},
				{SQRT2 * 6.0f, SQRT2 * 6.0f, SQRT2 * 2.5f, SQRT2},
				{SQRT2 * 6.0f, SQRT2 * 6.0f, SQRT2 * 3.0f, SQRT2}
			},
			{
				{5.2f, 6.7f, 2.0f, 1.0f},
				{5.2f, 6.7f, 2.5f, 1.0f},
				{5.2f, 6.7f, 3.0f, 1.0f}
			},
			{
				{SQRT2 * 4.0f, SQRT2 * 6.0f, SQRT2 * 2.0f, SQRT2},
				{SQRT2 * 4.0f, SQRT2 * 6.0f, SQRT2 * 2.5f, SQRT2},
				{SQRT2 * 4.0f, SQRT2 * 6.0f, SQRT2 * 3.0f, SQRT2}
			},
			{
				{4.0f, 5.2f, 2.0f, 1.0f},
				{4.0f, 4.6f, 2.5f, 1.0f},
				{4.0f, 5.2f, 3.0f, 1.0f}
			},
			{
				{SQRT2 * 4.0f, SQRT2 * 6.0f, SQRT2 * 2.0f, SQRT2},
				{SQRT2 * 4.0f, SQRT2 * 6.0f, SQRT2 * 2.5f, SQRT2},
				{SQRT2 * 4.0f, SQRT2 * 6.0f, SQRT2 * 3.0f, SQRT2}
			},
			{
				{2.8f, 6.7f, 2.0f, 1.0f},
				{2.8f, 6.7f, 2.5f, 1.0f},
				{2.8f, 6.7f, 3.0f, 1.0f}
			},
			{
				{SQRT2 * 2.0f, SQRT2 * 6.0f, SQRT2 * 2.0f, SQRT2},
				{SQRT2 * 2.0f, SQRT2 * 6.0f, SQRT2 * 2.5f, SQRT2},
				{SQRT2 * 2.0f, SQRT2 * 6.0f, SQRT2 * 3.0f, SQRT2}
			},
			{
				{2.0f, 5.0f, 2.0f, 1.0f},
				{2.0f, 5.0f, 2.5f, 1.0f},
				{2.0f, 5.0f, 3.0f, 1.0f}
			},
			{
				{3.0f, 4.0f, 2.0f, 1.0f},
				{3.0f, 4.0f, 2.5f, 1.0f},
				{3.0f, 4.0f, 3.0f, 1.0f}
			},
			{
				{4.0f, 2.0f, 2.0f, 1.0f},
				{4.0f, 1.6f, 2.5f, 1.0f},
				{4.0f, 2.0f, 3.0f, 1.0f}
			}
																					  };
		private static Glu.GLUnurbs nurb;
		#endregion Private Fields

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Nurbs - Heart-shaped NURBS";
			}
		}

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookNurbs()
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
		///         Initialize depth buffer, light source, material property, and lighting model.
		///     </para>
		/// </summary>
		private static void Init() 
		{
			float[] materialAmbient = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] materialDiffuse = {1.0f, 0.2f, 1.0f, 1.0f};
			float[] materialSpecular = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] materialShininess = {50.0f};
			float[] light0Position = {1.0f, 0.1f, 1.0f, 0.0f};
			float[] light1Position = {-1.0f, 0.1f, 1.0f, 0.0f};
			float[] lightModelAmbient = {0.3f, 0.3f, 0.3f, 1.0f};

			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbient);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, materialShininess);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, light0Position);
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, light1Position);
			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, lightModelAmbient);

			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_LIGHT1);
			Gl.glDepthFunc(Gl.GL_LESS);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glEnable(Gl.GL_AUTO_NORMAL);

			nurb = Glu.gluNewNurbsRenderer();

			Glu.gluNurbsProperty(nurb, Glu.GLU_SAMPLING_TOLERANCE, 25.0f);
			Glu.gluNurbsProperty(nurb, Glu.GLU_DISPLAY_MODE, Glu.GLU_FILL);
		}
		#endregion Init()

		// --- Callbacks ---
		#region Display()
		private  void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

			Gl.glPushMatrix();
			Gl.glTranslatef(4.0f, 4.5f, 2.5f);
			Gl.glRotatef(220.0f, 1.0f, 0.0f, 0.0f);
			Gl.glRotatef(115.0f, 0.0f, 1.0f, 0.0f);
			Gl.glTranslatef(-4.0f, -4.5f, -2.5f);

			Glu.gluBeginSurface(nurb);
			Glu.gluNurbsSurface(nurb, SKNOTS, sknots, TKNOTS, tknots, 4 * TPOINTS, 4, controlPoints, SORDER, TORDER, Gl.GL_MAP2_VERTEX_4);
			Glu.gluEndSurface(nurb);
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
			Gl.glFrustum(-1.0, 1.0, -1.5, 0.5, 0.8, 10.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Glu.gluLookAt(7.0, 4.5, 4.0, 4.5, 4.5, 2.0, 6.0, -3.0, 2.0);
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