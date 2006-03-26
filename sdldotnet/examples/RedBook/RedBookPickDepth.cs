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
	///     Picking is demonstrated in this program.  In rendering mode, three overlapping
	///     rectangles are drawn.  When the left mouse button is pressed, selection mode is
	///     entered with the picking matrix.  Rectangles which are drawn under the cursor
	///     position are "picked."  Pay special attention to the depth value range, which is
	///     returned.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/pickdepth.c
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
	public class RedBookPickDepth
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
				return "PickDepth - Pick Rectangles with mouse";
			}
		}

		#region Private Constants
		private const int BUFSIZE = 512;
		#endregion Private Constants

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookPickDepth()
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
			Events.MouseButtonDown += new MouseButtonEventHandler(this.MouseButtonDown);
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
		#region DrawRectangles(int mode)
		private static void DrawRectangles(int mode) 
		{
			if(mode == Gl.GL_SELECT) 
			{
				Gl.glLoadName(1);
			}
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glColor3f(1.0f, 1.0f, 0.0f);
			Gl.glVertex3i(2, 0, 0);
			Gl.glVertex3i(2, 6, 0);
			Gl.glVertex3i(6, 6, 0);
			Gl.glVertex3i(6, 0, 0);
			Gl.glEnd();

			if(mode == Gl.GL_SELECT) 
			{
				Gl.glLoadName(2);
			}
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glColor3f(0.0f, 1.0f, 1.0f);
			Gl.glVertex3i(3, 2, -1);
			Gl.glVertex3i(3, 8, -1);
			Gl.glVertex3i(8, 8, -1);
			Gl.glVertex3i(8, 2, -1);
			Gl.glEnd();

			if(mode == Gl.GL_SELECT) 
			{
				Gl.glLoadName(3);
			}
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glColor3f(1.0f, 0.0f, 1.0f);
			Gl.glVertex3i(0, 2, -2);
			Gl.glVertex3i(0, 7, -2);
			Gl.glVertex3i(5, 7, -2);
			Gl.glVertex3i(5, 2, -2);
			Gl.glEnd();
		}
		#endregion DrawRectangles(int mode)

		#region Init()
		private static void Init() 
		{
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glShadeModel(Gl.GL_FLAT);
			Gl.glDepthRange(0.0, 1.0);  // The default z mapping
		}
		#endregion Init()

		#region ProcessHits(int hits, int[] buffer)
		/// <summary>
		///     <para>
		///         Prints out the contents of the selection array.
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
			{  // for each hit
				names = ptr[i];
				Console.WriteLine(" number of names for hit = {0}", names);
				i++;;
				Console.WriteLine("  z1 is {0}", (float) ptr[i] / 0x7fffffff);
				i++;
				Console.WriteLine("  z2 is {0}", (float) ptr[i] / 0x7fffffff);
				i++;
				Console.Write("   the name is ");
				for(j = 0; j < names; j++) 
				{  // for each name
					Console.Write("{0} ", ptr[i]);
					i++;
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}
		#endregion ProcessHits(int hits, int[] buffer)

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			DrawRectangles(Gl.GL_RENDER);
			Gl.glFlush();
		}
		#endregion Display()

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Gl.glOrtho(0.0, 8.0, 0.0, 8.0, -0.5, 2.5);
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

		/// <summary>
		///     <para>
		///         Sets up selection mode, name stack, and projection matrix for picking.  Then
		///         the objects are drawn.
		///     </para>
		/// </summary>
		private void MouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			int[] selectBuffer = new int[BUFSIZE];
			int hits;
			int[] viewport = new int[4];

			if(e.Button != MouseButton.SecondaryButton) 
			{
				return;
			}

			Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

			Gl.glSelectBuffer(BUFSIZE, selectBuffer);
			Gl.glRenderMode(Gl.GL_SELECT);

			Gl.glInitNames();
			Gl.glPushName(0);

			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPushMatrix();
			Gl.glLoadIdentity();
			// create 5x5 pixel picking region near cursor location
			Glu.gluPickMatrix((double) e.X, (double) (viewport[3] - e.Y), 5.0, 5.0, viewport);
			Gl.glOrtho(0.0, 8.0, 0.0, 8.0, -0.5, 2.5);
			DrawRectangles(Gl.GL_SELECT);
			Gl.glPopMatrix();
			Gl.glFlush();

			hits = Gl.glRenderMode(Gl.GL_RENDER);
			ProcessHits(hits, selectBuffer);
		}
	}
}