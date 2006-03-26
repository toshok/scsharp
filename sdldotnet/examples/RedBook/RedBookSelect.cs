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
	///     This is an illustration of the selection mode and name stack, which detects
	///     whether objects which collide with a viewing volume.  First, four triangles and
	///     a rectangular box representing a viewing volume are drawn (drawScene routine).
	///     The green triangle and yellow triangles appear to lie within the viewing volume,
	///     but the red triangle appears to lie outside it.  Then the selection mode is
	///     entered (SelectObjects routine).  Drawing to the screen ceases.  To see if any
	///     collisions occur, the four triangles are called.  In this example, the green
	///     triangle causes one hit with the name 1, and the yellow triangles cause one hit
	///     with the name 3.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/select.c
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
	public class RedBookSelect
	{
		//Width of screen
		int width = 200;
		//Height of screen
		int height = 200;
		
		

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Select - Selection mode";
			}
		}

		#region Private Constants
		private const int BUFSIZE = 512;
		#endregion Private Constants

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookSelect()
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

		// --- Application Methods ---
		#region Init()
		private static void Init() 
		{
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glShadeModel(Gl.GL_FLAT);
		}
		#endregion Init()

		#region DrawScene()
		/// <summary>
		///     <para>
		///         Draws 4 triangles and a wire frame which represents the viewing volume.
		///     </para>
		/// </summary>
		private static void DrawScene() 
		{
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluPerspective(40.0, 4.0 / 3.0, 1.0, 100.0);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Glu.gluLookAt(7.5, 7.5, 12.5, 2.5, 2.5, -5.0, 0.0, 1.0, 0.0);
			Gl.glColor3f(0.0f, 1.0f, 0.0f);	// green triangle
			DrawTriangle(2.0f, 2.0f, 3.0f, 2.0f, 2.5f, 3.0f, -5.0f);
			Gl.glColor3f(1.0f, 0.0f, 0.0f);	// red triangle
			DrawTriangle(2.0f, 7.0f, 3.0f, 7.0f, 2.5f, 8.0f, -5.0f);
			Gl.glColor3f(1.0f, 1.0f, 0.0f);	// yellow triangles
			DrawTriangle(2.0f, 2.0f, 3.0f, 2.0f, 2.5f, 3.0f, 0.0f);
			DrawTriangle(2.0f, 2.0f, 3.0f, 2.0f, 2.5f, 3.0f, -10.0f);
			DrawViewVolume(0.0f, 5.0f, 0.0f, 5.0f, 0.0f, 10.0f);
		}
		#endregion DrawScene()

		#region DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, float z)
		/// <summary>
		///     <para>
		///         Draw a triangle with vertices at (x1, y1), (x2, y2), and (x3, y3) at z units
		///         away from the origin
		///     </para>
		/// </summary>
		private static void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, float z) 
		{
			Gl.glBegin(Gl.GL_TRIANGLES);
			Gl.glVertex3f(x1, y1, z);
			Gl.glVertex3f(x2, y2, z);
			Gl.glVertex3f(x3, y3, z);
			Gl.glEnd();
		}
		#endregion DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, float z)

		#region DrawViewVolume(float x1, float x2, float y1, float y2, float z1, float z2)
		/// <summary>
		///     <para>
		///         Draws a rectangular box with these outer x, y, and z values.
		///     </para>
		/// </summary>
		private static void DrawViewVolume(float x1, float x2, float y1, float y2, float z1, float z2) 
		{
			Gl.glColor3f(1.0f, 1.0f, 1.0f);
			Gl.glBegin(Gl.GL_LINE_LOOP);
			Gl.glVertex3f(x1, y1, -z1);
			Gl.glVertex3f(x2, y1, -z1);
			Gl.glVertex3f(x2, y2, -z1);
			Gl.glVertex3f(x1, y2, -z1);
			Gl.glEnd();

			Gl.glBegin(Gl.GL_LINE_LOOP);
			Gl.glVertex3f(x1, y1, -z2);
			Gl.glVertex3f(x2, y1, -z2);
			Gl.glVertex3f(x2, y2, -z2);
			Gl.glVertex3f(x1, y2, -z2);
			Gl.glEnd();

			Gl.glBegin(Gl.GL_LINES); // 4 lines
			Gl.glVertex3f(x1, y1, -z1);
			Gl.glVertex3f(x1, y1, -z2);
			Gl.glVertex3f(x1, y2, -z1);
			Gl.glVertex3f(x1, y2, -z2);
			Gl.glVertex3f(x2, y1, -z1);
			Gl.glVertex3f(x2, y1, -z2);
			Gl.glVertex3f(x2, y2, -z1);
			Gl.glVertex3f(x2, y2, -z2);
			Gl.glEnd();
		}
		#endregion DrawViewVolume(float x1, float x2, float y1, float y2, float z1, float z2)

		#region ProcessHits(int hits, int[] buffer)
		/// <summary>
		///     <para>
		///         ProcessHits prints out the contents of the selection array.
		///     </para>
		/// </summary>
		private static void ProcessHits(int hits, int[] buffer) 
		{
			int i, j;
			int names;
			int[] ptr;

			Console.WriteLine("hits = {0}", hits);
			ptr = buffer;
			for(i = 0; i < hits; i++) 
			{	//  for each hit
				names = ptr[i];
				Console.WriteLine(" number of names for hit = {0}", names);
				i++;
				Console.WriteLine("  z1 is {0}", (float) ptr[i] / 0x7fffffff);
				i++;
				Console.WriteLine("  z2 is {0}", (float) ptr[i] / 0x7fffffff);
				i++;
				Console.Write("   the name is ");
				for(j = 0; j < names; j++) 
				{	// for each name
					Console.Write("{0} ", ptr[i]);
					i++;
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}
		#endregion ProcessHits(int hits, int[] buffer)

		#region SelectObjects()
		/// <summary>
		///     <para>
		///         SelectObjects "draws" the triangles in selection mode, assigning names for
		///         the triangles.  Note that the third and fourth triangles share one name, so
		///         that if either or both triangles intersects the viewing/clipping volume,
		///         only one hit will be registered.
		///     </para>
		/// </summary>
		private static void SelectObjects() 
		{
			int[] selectBuffer = new int[BUFSIZE];
			int hits;

			Gl.glSelectBuffer(BUFSIZE, selectBuffer);
			Gl.glRenderMode(Gl.GL_SELECT);

			Gl.glInitNames();
			Gl.glPushName(0);

			Gl.glPushMatrix();
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Gl.glOrtho(0.0, 5.0, 0.0, 5.0, 0.0, 10.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Gl.glLoadName(1);
			DrawTriangle(2.0f, 2.0f, 3.0f, 2.0f, 2.5f, 3.0f, -5.0f);
			Gl.glLoadName(2);
			DrawTriangle(2.0f, 7.0f, 3.0f, 7.0f, 2.5f, 8.0f, -5.0f);
			Gl.glLoadName(3);
			DrawTriangle(2.0f, 2.0f, 3.0f, 2.0f, 2.5f, 3.0f, 0.0f);
			DrawTriangle(2.0f, 2.0f, 3.0f, 2.0f, 2.5f, 3.0f, -10.0f);
			Gl.glPopMatrix();
			Gl.glFlush();

			hits = Gl.glRenderMode(Gl.GL_RENDER);
			ProcessHits(hits, selectBuffer);
		}
		#endregion SelectObjects()

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			DrawScene();
			SelectObjects();
			Gl.glFlush();
		}
		#endregion Display()

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
			Init();
			Events.Run();
		}

		#endregion Run Loop
	}
}