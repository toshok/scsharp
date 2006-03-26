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
	///     This program demonstrates use of the accumulation buffer to create an
	///     out-of-focus depth-of-field effect.  The teapots are drawn several times into the
	///     accumulation buffer.  The viewing volume is jittered, except at the focal point,
	///     where the viewing volume is at the same position, each time.  In this case, the
	///     gold teapot remains in focus.
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
	public class RedBookDof
	{
		#region Fields

		//Width of screen
		int width = 400;
		//Height of screen
		int height = 400;
		
		
		
		
        private static int teapotList;

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Dof - Depth of Field";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookDof()
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
		}

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		private static void Init()
		{
			
			float[] ambient = {0.0f, 0.0f, 0.0f, 1.0f};
			float[] diffuse = {1.0f, 1.0f, 1.0f, 1.0f};
			//float[] specular = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] position = {0.0f, 3.0f, 3.0f, 0.0f};

			float[] modelAmbient = {0.2f, 0.2f, 0.2f, 1.0f};
			float[] localView = {0.0f};

			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambient);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, diffuse);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position);

			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, modelAmbient);
			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_LOCAL_VIEWER, localView);

			Gl.glFrontFace(Gl.GL_CW);
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_AUTO_NORMAL);
			Gl.glEnable(Gl.GL_NORMALIZE);
			Gl.glEnable(Gl.GL_DEPTH_TEST);

			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glClearAccum(0.0f, 0.0f, 0.0f, 0.0f); 
            
			// make teapot display list
			teapotList = Gl.glGenLists(1);
			Gl.glNewList(teapotList, Gl.GL_COMPILE);
			Glut.glutSolidTeapot(0.5);
			Gl.glEndList();
		}

		#region AccFrustum(double left, double right, double bottom, double top, double near, double far, double pixdx, double pixdy, double eyedx, double eyedy, double focus)
		/// <summary>
		///     <para>
		///          * The first 6 arguments are identical to the Gl.glFrustum() call.
		///     </para>
		///     <para>
		///         pixdx and pixdy are anti-alias jitter in pixels.  Set both equal to 0.0 for
		///         no anti-alias jitter.  eyedx and eyedy are depth-of field jitter in pixels.
		///         Set both equal to 0.0 for no depth of field effects.
		///     </para>
		///     <para>
		///         focus is distance from eye to plane in focus.  focus must be greater than,
		///         but not equal to 0.0.
		///     </para>
		///     <para>
		///         Note that AccFrustum() calls Gl.glTranslatef().  You will probably want to
		///         insure that your ModelView matrix has been initialized to identity before
		///         calling AccFrustum().
		///     </para>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		/// <param name="top"></param>
		/// <param name="near"></param>
		/// <param name="far"></param>
		/// <param name="pixdx"></param>
		/// <param name="pixdy"></param>
		/// <param name="eyedx"></param>
		/// <param name="eyedy"></param>
		/// <param name="focus"></param>
		private static void AccFrustum(double left, double right, double bottom, double top, double near, double far, double pixdx, double pixdy, double eyedx, double eyedy, double focus) 
		{
			double xwsize, ywsize;
			double dx, dy;
			int[] viewport = new int[4];

			Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

			xwsize = right - left;
			ywsize = top - bottom;

			dx = -(pixdx * xwsize / (double) viewport[2] + eyedx * near / focus);
			dy = -(pixdy * ywsize / (double) viewport[3] + eyedy * near / focus);

			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Gl.glFrustum(left + dx, right + dx, bottom + dy, top + dy, near, far);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Gl.glTranslatef((float) -eyedx, (float) -eyedy, 0.0f);
		}
		#endregion AccFrustum(double left, double right, double bottom, double top, double near, double far, double pixdx, double pixdy, double eyedx, double eyedy, double focus)

		#region AccPerspective(double fovy, double aspect, double near, double far, double pixdx, double pixdy, double eyedx, double eyedy, double focus)
		/// <summary>
		///     <para>
		///         Note that AccPerspective() calls AccFrustum().
		///     </para>
		/// </summary>
		private static void AccPerspective(double fovy, double aspect, double near, double far, double pixdx, double pixdy, double eyedx, double eyedy, double focus) 
		{
			double fov2, left, right, bottom, top;

			fov2 = ((fovy * System.Math.PI) / 180.0) / 2.0;

			top = near / (Math.Cos(fov2) / Math.Sin(fov2));
			bottom = -top;

			right = top * aspect;
			left = -right;

			AccFrustum(left, right, bottom, top, near, far, pixdx, pixdy, eyedx, eyedy, focus);
		}
		#endregion AccPerspective(double fovy, double aspect, double near, double far, double pixdx, double pixdy, double eyedx, double eyedy, double focus)

		#endregion Lesson Setup

		#region void Display
		/// <summary>
		/// Renders the scene
		/// </summary>
		private static void Display()
		{
			int jitter;
			int[] viewport = new int[4];

			Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);
			Gl.glClear(Gl.GL_ACCUM_BUFFER_BIT);

			for(jitter = 0; jitter < 8; jitter++) 
			{
				Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
				AccPerspective(45.0, (double) viewport[2] / (double) viewport[3], 1.0, 15.0, 0.0, 0.0, 0.33 * Jitter.J8[jitter].X, 0.33 * Jitter.J8[jitter].Y, 5.0);

				// ruby, gold, silver, emerald, and cyan teapots
				RenderTeapot(-1.1f, -0.5f, -4.5f, 0.1745f, 0.01175f, 0.01175f, 0.61424f, 0.04136f, 0.04136f, 0.727811f, 0.626959f, 0.626959f, 0.6f);
				RenderTeapot(-0.5f, -0.5f, -5.0f, 0.24725f, 0.1995f, 0.0745f, 0.75164f, 0.60648f, 0.22648f, 0.628281f, 0.555802f, 0.366065f, 0.4f);
				RenderTeapot(0.2f, -0.5f, -5.5f, 0.19225f, 0.19225f, 0.19225f, 0.50754f, 0.50754f, 0.50754f, 0.508273f, 0.508273f, 0.508273f, 0.4f);
				RenderTeapot(1.0f, -0.5f, -6.0f, 0.0215f, 0.1745f, 0.0215f, 0.07568f, 0.61424f, 0.07568f, 0.633f, 0.727811f, 0.633f, 0.6f);
				RenderTeapot(1.8f, -0.5f, -6.5f, 0.0f, 0.1f, 0.06f, 0.0f, 0.50980392f, 0.50980392f, 0.50196078f, 0.50196078f, 0.50196078f, 0.25f);
				Gl.glAccum(Gl.GL_ACCUM, 0.125f);
			}
			Gl.glAccum(Gl.GL_RETURN, 1.0f);
			Gl.glFlush();
		}
		#endregion void Display

		#region RenderTeapot(float x, float y, float z, float ambr, float ambg, float ambb, float difr, float difg, float difb, float specr, float specg, float specb, float shine)
		private static void RenderTeapot(float x, float y, float z, float ambr, float ambg, float ambb, float difr, float difg, float difb, float specr, float specg, float specb, float shine) 
		{
			float[] mat = new float[4];

			Gl.glPushMatrix();
			Gl.glTranslatef(x, y, z);
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
		#endregion RenderTeapot(float x, float y, float z, float ambr, float ambg, float ambb, float difr, float difg, float difb, float specr, float specg, float specb, float shine)

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
