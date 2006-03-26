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
	///     This program demonstrates the winding rule polygon tessellation property.  Four
	///     tessellated objects are drawn, each with very different contours.  When the w key
	///     is pressed, the objects are drawn with a different winding rule.
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
	public class RedBookTessWind
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
				return "TessWind - winding rule polygon tessellation TODO";
			}
		}

		#region Private Fields
		private static double currentWinding = Glu.GLU_TESS_WINDING_ODD;
		private static Glu.GLUtesselator tess;
		private static int list;

		private static double[][] rects = new double[12][] {
															   new double[] {50.0, 50.0, 0.0},
															   new double[] {300.0, 50.0, 0.0},
															   new double[] {300.0, 300.0, 0.0},
															   new double[] {50.0, 300.0, 0.0},
															   new double[] {100.0, 100.0, 0.0},
															   new double[] {250.0, 100.0, 0.0},
															   new double[] {250.0, 250.0, 0.0},
															   new double[] {100.0, 250.0, 0.0},
															   new double[] {150.0, 150.0, 0.0},
															   new double[] {200.0, 150.0, 0.0},
															   new double[] {200.0, 200.0, 0.0},
															   new double[] {150.0, 200.0, 0.0}
														   };
		private static double[][] spiral = new double[16][] {
																new double[] {400.0, 250.0, 0.0},
																new double[] {400.0, 50.0, 0.0},
																new double[] {50.0, 50.0, 0.0},
																new double[] {50.0, 400.0, 0.0},
																new double[] {350.0, 400.0, 0.0},
																new double[] {350.0, 100.0, 0.0},
																new double[] {100.0, 100.0, 0.0},
																new double[] {100.0, 350.0, 0.0},
																new double[] {300.0, 350.0, 0.0},
																new double[] {300.0, 150.0, 0.0},
																new double[] {150.0, 150.0, 0.0},
																new double[] {150.0, 300.0, 0.0},
																new double[] {250.0, 300.0, 0.0},
																new double[] {250.0, 200.0, 0.0},
																new double[] {200.0, 200.0, 0.0},
																new double[] {200.0, 250.0, 0.0}
															};
		private static double[][] quad1 = new double[4][] {
															  new double[] {50.0, 150.0, 0.0},
															  new double[] {350.0, 150.0, 0.0},
															  new double[] {350.0, 200.0, 0.0},
															  new double[] {50.0, 200.0, 0.0}
														  };
		private static double[][] quad2 = new double[4][] {
															  new double[] {100.0, 100.0, 0.0},
															  new double[] {300.0, 100.0, 0.0},
															  new double[] {300.0, 350.0, 0.0},
															  new double[] {100.0, 350.0, 0.0}
														  };
		private static double[][] tri = new double[3][] {
															new double[] {200.0, 50.0, 0.0},
															new double[] {250.0, 300.0, 0.0},
															new double[] {150.0, 300.0, 0.0}
														};
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookTessWind()
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
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glShadeModel(Gl.GL_FLAT);
			tess = Glu.gluNewTess();
			Glu.gluTessCallback(tess, Glu.GLU_TESS_VERTEX, new Glu.TessVertexCallback1(Gl.glVertex3dv));
			Glu.gluTessCallback(tess, Glu.GLU_TESS_BEGIN, new Glu.TessBeginCallback(Begin));
			Glu.gluTessCallback(tess, Glu.GLU_TESS_END, new Glu.TessEndCallback(End));
			Glu.gluTessCallback(tess, Glu.GLU_TESS_ERROR, new Glu.TessErrorCallback(Error));
			Glu.gluTessCallback(tess, Glu.GLU_TESS_COMBINE, new Glu.TessCombineCallback1(Combine));

			list = Gl.glGenLists(4);
			MakeNewLists();
		}
		#endregion Init()

		#region MakeNewLists()
		/// <summary>
		///     <para>
		///         Make four display lists, each with a different tessellated object.
		///     </para>
		/// </summary>
		private static void MakeNewLists() 
		{
			int i;

			Glu.gluTessProperty(tess, Glu.GLU_TESS_WINDING_RULE, currentWinding);

			Gl.glNewList(list, Gl.GL_COMPILE);
			Glu.gluTessBeginPolygon(tess, IntPtr.Zero);
			Glu.gluTessBeginContour(tess);
			for(i = 0; i < 4; i++) 
			{
				Glu.gluTessVertex(tess, rects[i], rects[i]);
			}
			Glu.gluTessEndContour(tess);
			Glu.gluTessBeginContour(tess);
			for(i = 4; i < 8; i++) 
			{
				Glu.gluTessVertex(tess, rects[i], rects[i]);
			}
			Glu.gluTessEndContour(tess);
			Glu.gluTessBeginContour(tess);
			for(i = 8; i < 12; i++) 
			{
				Glu.gluTessVertex(tess, rects[i], rects[i]);
			}
			Glu.gluTessEndContour(tess);
			Glu.gluTessEndPolygon(tess);
			Gl.glEndList();

			Gl.glNewList(list + 1, Gl.GL_COMPILE);
			Glu.gluTessBeginPolygon(tess, IntPtr.Zero);
			Glu.gluTessBeginContour(tess);
			for(i = 0; i < 4; i++) 
			{
				Glu.gluTessVertex(tess, rects[i], rects[i]);
			}
			Glu.gluTessEndContour(tess);
			Glu.gluTessBeginContour(tess);
			for(i = 7; i >= 4; i--) 
			{
				Glu.gluTessVertex(tess, rects[i], rects[i]);
			}
			Glu.gluTessEndContour(tess);
			Glu.gluTessBeginContour(tess);
			for(i = 11; i >= 8; i--) 
			{
				Glu.gluTessVertex(tess, rects[i], rects[i]);
			}
			Glu.gluTessEndContour(tess);
			Glu.gluTessEndPolygon(tess);
			Gl.glEndList();

			Gl.glNewList(list + 2, Gl.GL_COMPILE);
			Glu.gluTessBeginPolygon(tess, IntPtr.Zero);
			Glu.gluTessBeginContour(tess);
			for(i = 0; i < 16; i++) 
			{
				Glu.gluTessVertex(tess, spiral[i], spiral[i]);
			}
			Glu.gluTessEndContour(tess);
			Glu.gluTessEndPolygon(tess);
			Gl.glEndList();

			Gl.glNewList(list + 3, Gl.GL_COMPILE);
			Glu.gluTessBeginPolygon(tess, IntPtr.Zero);
			Glu.gluTessBeginContour(tess);
			for(i = 0; i < 4; i++) 
			{
				Glu.gluTessVertex(tess, quad1[i], quad1[i]);
			}
			Glu.gluTessEndContour(tess);
			Glu.gluTessBeginContour(tess);
			for(i = 0; i < 4; i++) 
			{
				Glu.gluTessVertex(tess, quad2[i], quad2[i]);
			}
			Glu.gluTessEndContour(tess);
			Glu.gluTessBeginContour(tess);
			for(i = 0; i < 3; i++) 
			{
				Glu.gluTessVertex(tess, tri[i], tri[i]);
			}
			Glu.gluTessEndContour(tess);
			Glu.gluTessEndPolygon(tess);
			Gl.glEndList();
		}
		#endregion MakeNewLists()

		// --- Callbacks ---
		#region Begin(int which)
		private static void Begin(int which) 
		{
			Gl.glBegin(which);
		}
		#endregion Begin(int which)

		#region Combine(double[] coordinates, double[][] vertexData, float[] weight, double[] dataOut)
		/// <summary>
		///     <para>
		///         The Combine callback is used to create a new vertex when edges intersect.
		///         coordinate location is trivial to calculate, but weight[4] may be used to
		///         average color, normal, or texture coordinate data.
		///     </para>
		/// </summary>
		private static void Combine(double[] coordinates, double[] vertexData, float[] weight, double[] dataOut) 
		{
			double[] vertex = new double[3];

			vertex[0] = coordinates[0];
			vertex[1] = coordinates[1];
			vertex[2] = coordinates[2];
			dataOut = vertex;
		}
		#endregion Combine(double[] coordinates, double[][] vertexData, float[] weight, double[] dataOut)

		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
			Gl.glColor3f(1.0f, 1.0f, 1.0f);
			Gl.glPushMatrix(); 
			Gl.glCallList(list);
			Gl.glTranslatef(0.0f, 500.0f, 0.0f);
			Gl.glCallList(list + 1);
			Gl.glTranslatef(500.0f, -500.0f, 0.0f);
			Gl.glCallList(list + 2);
			Gl.glTranslatef(0.0f, 500.0f, 0.0f);
			Gl.glCallList(list + 3);
			Gl.glPopMatrix();
			Gl.glFlush();
		}
		#endregion Display()

		#region End()
		private static void End() 
		{
			Gl.glEnd();
		}
		#endregion End()

		#region Error(int errorCode)
		private static void Error(int errorCode) 
		{
			Console.WriteLine("Tessellation Error: {0}", Glu.gluErrorString(errorCode));
			Environment.Exit(1);
		}
		#endregion Error(int errorCode)

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			if(w <= h) 
			{
				Glu.gluOrtho2D(0.0, 1000.0, 0.0, 1000.0 * (double) h / (double) w);
			}
			else 
			{
				Glu.gluOrtho2D(0.0, 1000.0 * (double) w / (double) h, 0.0, 1000.0);
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
				case Key.W:
					if(currentWinding == Glu.GLU_TESS_WINDING_ODD) 
					{
						currentWinding = Glu.GLU_TESS_WINDING_NONZERO;
					}
					else if(currentWinding == Glu.GLU_TESS_WINDING_NONZERO) 
					{
						currentWinding = Glu.GLU_TESS_WINDING_POSITIVE;
					}
					else if(currentWinding == Glu.GLU_TESS_WINDING_POSITIVE) 
					{
						currentWinding = Glu.GLU_TESS_WINDING_NEGATIVE;
					}
					else if(currentWinding == Glu.GLU_TESS_WINDING_NEGATIVE) 
					{
						currentWinding = Glu.GLU_TESS_WINDING_ABS_GEQ_TWO;
					}
					else if(currentWinding == Glu.GLU_TESS_WINDING_ABS_GEQ_TWO) 
					{
						currentWinding = Glu.GLU_TESS_WINDING_ODD;
					}
					MakeNewLists();
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