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
	///     This program draws a NURBS surface in the shape of a symmetrical hill.
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
	public class RedBookSurfaceOld
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
				return "SurfaceOld - old version of hill-shaped NURBS surface";
			}
		}

		#region Private Fields
		private static float[ , , ] controlPoints = new float[4, 4, 3];
		private static bool showPoints = false;
		private static Glu.GLUnurbs nurb;
		private static bool down = false;
		private static int lastX = 0;
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookSurfaceOld()
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
			Events.MouseButtonDown += new MouseButtonEventHandler(this.MouseButton);
			Events.MouseButtonUp += new MouseButtonEventHandler(this.MouseButton);
			Events.MouseMotion += new MouseMotionEventHandler(this.MouseMotion);
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
		///         Initialize material property and depth buffer.
		///     </para>
		/// </summary>
		private static void Init() 
		{
			float[] materialDiffuse = {0.7f, 0.7f, 0.7f, 1.0f};
			float[] materialSpecular = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] materialShininess = {100.0f};

			Gl.glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, materialShininess);

			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glDepthFunc(Gl.GL_LESS);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glEnable(Gl.GL_AUTO_NORMAL);
			Gl.glEnable(Gl.GL_NORMALIZE);

			InitSurface();

			nurb = Glu.gluNewNurbsRenderer();
			Glu.gluNurbsProperty(nurb, Glu.GLU_SAMPLING_TOLERANCE, 25.0f);
			Glu.gluNurbsProperty(nurb, Glu.GLU_DISPLAY_MODE, Glu.GLU_FILL);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Gl.glTranslatef(0.0f, 0.0f, -5.0f);
		}
		#endregion Init()

		#region InitSurface()
		private static void InitSurface() 
		{
			int u, v;
			for(u = 0; u < 4; u++) 
			{
				for(v = 0; v < 4; v++) 
				{
					controlPoints[u, v, 0] = 2.0f * ((float)u - 1.5f);
					controlPoints[u, v, 1] = 2.0f * ((float)v - 1.5f);

					if((u == 1 || u == 2) && (v == 1 || v == 2)) 
					{
						controlPoints[u, v, 2] = 7.0f;
					}
					else 
					{
						controlPoints[u, v, 2] = -3.0f;
					}
				}
			}
		}
		#endregion InitSurface()

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			float[] knots = {0, 0, 0, 0, 1, 1, 1, 1};
			int i, j;

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

			Gl.glPushMatrix();
			Gl.glRotatef(330, 1, 0, 0);
			Gl.glScalef(0.25f, 0.25f, 0.25f);

			Glu.gluBeginSurface(nurb);
			Glu.gluNurbsSurface(nurb, 8, knots, 8, knots, 4 * 3, 3, controlPoints, 4, 4, Gl.GL_MAP2_VERTEX_3);
			Glu.gluEndSurface(nurb);

			if(showPoints) 
			{
				Gl.glPointSize(5.0f);
				Gl.glDisable(Gl.GL_LIGHTING);
				Gl.glColor3f(1, 1, 0);
				Gl.glBegin(Gl.GL_POINTS);
				for(i = 0; i < 4; i++) 
				{
					for(j = 0; j < 4; j++) 
					{
						Gl.glVertex3f(controlPoints[i, j, 0], controlPoints[i, j, 1], controlPoints[i, j, 2]);
					}
				}
				Gl.glEnd();
				Gl.glEnable(Gl.GL_LIGHTING);
			}
			Gl.glPopMatrix();
		}
		#endregion Display()

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluPerspective(45.0, (double) w / (double) h, 3.0, 8.0);
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
				case Key.C:
					showPoints = !showPoints;
					break;
				case Key.One:
					showPoints = false;
					break;
				case Key.Two:
					showPoints = true;
					break;
				case Key.Three:
					Glu.gluNurbsProperty(nurb, Glu.GLU_DISPLAY_MODE, Glu.GLU_FILL);
					break;
				case Key.Four:
					Glu.gluNurbsProperty(nurb, Glu.GLU_DISPLAY_MODE, Glu.GLU_OUTLINE_POLYGON);
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

		private void MouseButton(object sender, MouseButtonEventArgs e)
		{
			if(e.ButtonPressed == true) 
			{
				lastX = e.X;
				down = true;
			}
			else 
			{
				down = false;
			}

		}

		private void MouseMotion(object sender, MouseMotionEventArgs e)
		{
			if(down) 
			{
				Gl.glRotatef(lastX - e.X, 0, 1, 0);
				lastX = e.X;
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