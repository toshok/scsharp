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
	///     perspective projection, using the special routines accFrustum() and
	///     accPerspective().
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
	public class RedBookAccPersp
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
				return "AccPersp - Accumulation Buffer Perspective";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookAccPersp()
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

		#region AccFrustum(double left, double right, double bottom, double top, double near, double far, double pixdx, double pixdy, double eyedx, double eyedy, double focus)
		/// <summary>
		///     <para>
		///         The first 6 arguments are identical to the glFrustum() call.
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
		///         Note that accFrustum() calls glTranslatef().  You will probably want to
		///         insure that your ModelView matrix has been initialized to identity before
		///         calling AccFrustum().
		///     </para>
		/// </summary>
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
		///         The first 4 arguments are identical to the gluPerspective() call.
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

		#region DisplayObjects()
		private static void DisplayObjects() 
		{
			float[] torusDiffuse = {0.7f, 0.7f, 0.0f, 1.0f};
			float[] cubeDiffuse = {0.0f, 0.7f, 0.7f, 1.0f};
			float[] sphereDiffuse = {0.7f, 0.0f, 0.7f, 1.0f};
			float[] octaDiffuse = {0.7f, 0.4f, 0.4f, 1.0f};

			Gl.glPushMatrix();
			Gl.glTranslatef(0.0f, 0.0f, -5.0f);
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
				AccPerspective(50.0, (double) viewport[2] / (double) viewport[3], 1.0, 15.0, Jitter.J8[jitter].X, Jitter.J8[jitter].Y, 0.0, 0.0, 1.0);
				DisplayObjects();
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