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
	///     <para>
	///         This program demonstrates polygon tessellation.  Two tesselated objects are
	///         drawn.  The first is a rectangle with a triangular hole.  The second is a
	///         smooth shaded, self-intersecting star.
	///     </para>
	///     <para>
	///         Note the exterior rectangle is drawn with its vertices in counter-clockwise
	///         order, but its interior clockwise.  Note the Combine callback is needed for
	///         the self-intersecting star.  Also note that removing the TessProperty for
	///         the star will make the interior unshaded (WINDING_ODD).
	///     </para>
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
	public class RedBookTess
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
				return "Tess - tessellation TODO";
			}
		}

		#region Private Fields
		private static int startList;
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookTess()
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
			Glu.GLUtesselator tess;
			double[][] rect = new double[4][] {
												  new double[] {50.0, 50.0, 0.0},
												  new double[] {200.0, 50.0, 0.0},
												  new double[] {200.0, 200.0, 0.0},
												  new double[] {50.0, 200.0, 0.0}
											  };
			double[][] tri = new double[3][] {
												 new double[] {75.0, 75.0, 0.0},
												 new double[] {125.0, 175.0, 0.0},
												 new double[] {175.0, 75.0, 0.0}
											 };
			double[][] star = new double[5][] {
												  new double[] {250.0, 50.0, 0.0, 1.0, 0.0, 1.0},
												  new double[] {325.0, 200.0, 0.0, 1.0, 1.0, 0.0},
												  new double[] {400.0, 50.0, 0.0, 0.0, 1.0, 1.0},
												  new double[] {250.0, 150.0, 0.0, 1.0, 0.0, 0.0},
												  new double[] {400.0, 150.0, 0.0, 0.0, 1.0, 0.0}
											  };

			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);

			startList = Gl.glGenLists(2);

			tess = Glu.gluNewTess();
			Glu.gluTessCallback(tess, Glu.GLU_TESS_VERTEX, new Glu.TessVertexCallback1(Gl.glVertex3dv));
			Glu.gluTessCallback(tess, Glu.GLU_TESS_BEGIN, new Glu.TessBeginCallback(Begin));
			Glu.gluTessCallback(tess, Glu.GLU_TESS_END, new Glu.TessEndCallback(End));
			Glu.gluTessCallback(tess, Glu.GLU_TESS_ERROR, new Glu.TessErrorCallback(Error));

			// rectangle with triangular hole inside
			Gl.glNewList(startList, Gl.GL_COMPILE);
			Gl.glShadeModel(Gl.GL_FLAT);    
			Glu.gluTessBeginPolygon(tess, IntPtr.Zero);
			Glu.gluTessBeginContour(tess);
			Glu.gluTessVertex(tess, rect[0], rect[0]);
			Glu.gluTessVertex(tess, rect[1], rect[1]);
			Glu.gluTessVertex(tess, rect[2], rect[2]);
			Glu.gluTessVertex(tess, rect[3], rect[3]);
			Glu.gluTessEndContour(tess);
			Glu.gluTessBeginContour(tess);
			Glu.gluTessVertex(tess, tri[0], tri[0]);
			Glu.gluTessVertex(tess, tri[1], tri[1]);
			Glu.gluTessVertex(tess, tri[2], tri[2]);
			Glu.gluTessEndContour(tess);
			Glu.gluTessEndPolygon(tess);
			Gl.glEndList();

			Glu.gluTessCallback(tess, Glu.GLU_TESS_VERTEX, new Glu.TessVertexCallback1(Vertex));
			Glu.gluTessCallback(tess, Glu.GLU_TESS_BEGIN, new Glu.TessBeginCallback(Begin));
			Glu.gluTessCallback(tess, Glu.GLU_TESS_END, new Glu.TessEndCallback(End));
			Glu.gluTessCallback(tess, Glu.GLU_TESS_ERROR, new Glu.TessErrorCallback(Error));
			Glu.gluTessCallback(tess, Glu.GLU_TESS_COMBINE, new Glu.TessCombineCallback1(Combine));

			// smooth shaded, self-intersecting star
			Gl.glNewList(startList + 1, Gl.GL_COMPILE);
			Gl.glShadeModel(Gl.GL_SMOOTH);    
			Glu.gluTessProperty(tess, Glu.GLU_TESS_WINDING_RULE, Glu.GLU_TESS_WINDING_POSITIVE);
			Glu.gluTessBeginPolygon(tess, IntPtr.Zero);
			Glu.gluTessBeginContour(tess);
			Glu.gluTessVertex(tess, star[0], star[0]);
			Glu.gluTessVertex(tess, star[1], star[1]);
			Glu.gluTessVertex(tess, star[2], star[2]);
			Glu.gluTessVertex(tess, star[3], star[3]);
			Glu.gluTessVertex(tess, star[4], star[4]);
			Glu.gluTessEndContour(tess);
			Glu.gluTessEndPolygon(tess);
			Gl.glEndList();
			Glu.gluDeleteTess(tess);
		}
		#endregion Init()

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
		///         average color, normal, or texture coordinate data.  In this program, color
		///         is weighted.
		///     </para>
		/// </summary>
		private static void Combine(double[] coordinates, double[] vertexData, float[] weight, double[] dataOut) 
		{
			double[] vertex = new double[6];
			int i;

			vertex[0] = coordinates[0];
			vertex[1] = coordinates[1];
			vertex[2] = coordinates[2];

			for(i = 3; i < 6; i++) 
			{
				vertex[i] = weight[0] * vertexData[i] + weight[1] * vertexData[i] + weight[2] * vertexData[i] + weight[3] * vertexData[i];
			}

			dataOut = vertex;
		}
		#endregion Combine(double[] coordinates, double[][] vertexData, float[] weight, double[] dataOut)

		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
			Gl.glColor3f(1.0f, 1.0f, 1.0f);
			Gl.glCallList(startList);
			Gl.glCallList(startList + 1);
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
			Glu.gluOrtho2D(0.0, (double) w, 0.0, (double) h);
		}
		#endregion Reshape(int w, int h)

		#region Vertex(double[] vertex)
		private static void Vertex(double[] vertex) 
		{
			//double[] pointer;

			//pointer = vertex;
			//Gl.glColor3dv(pointer);
			Gl.glColor3f(1.0f, 1.0f, 1.0f);
			Gl.glVertex3dv(vertex);
		}
		#endregion Vertex(double[] vertex)

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
