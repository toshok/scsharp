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
	///     Use of multiple names and picking are demonstrated.  A 3x3 grid of squares is
	///     drawn.  When the left mouse button is pressed, all squares under the cursor
	///     position have their color changed.
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
	public class RedBookPickSquare
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
				return "PickSquare - Pick squares with mouse";
			}
		}

		#region Private Constants
		private const int BUFSIZE = 512;
		#endregion Private Constants

		#region Private Fields
		private static int[ , ] board = new int[3, 3];
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookPickSquare()
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

		#region DrawSquares(int mode)
		/// <summary>
		///     <para>
		///         The nine squares are drawn.  In selection mode, each square is given two
		///         names:  one for the row and the other for the column on the grid.  The color
		///         of each square is determined by its position on the grid, and the value in
		///         the board[][] array.
		///     </para>
		/// </summary>
		private static void DrawSquares(int mode) 
		{
			int i, j;
			for(i = 0; i < 3; i++) 
			{
				if(mode == Gl.GL_SELECT) 
				{
					Gl.glLoadName(i);
				}
				for(j = 0; j < 3; j ++) 
				{
					if(mode == Gl.GL_SELECT) 
					{
						Gl.glPushName(j);
					}
					Gl.glColor3f((float) i / 3.0f, (float) j / 3.0f, (float) board[i, j] / 3.0f);
					Gl.glRecti(i, j, i + 1, j + 1);
					if(mode == Gl.GL_SELECT) 
					{
						Gl.glPopName();
					}
				}
			}
		}
		#endregion DrawSquares(int mode)

		#region Init()
		/// <summary>
		///     <para>
		///         Clear color value for every square on the board.
		///     </para>
		/// </summary>
		private static void Init() 
		{
			int i, j;
			for(i = 0; i < 3; i++) 
			{
				for(j = 0; j < 3; j ++) 
				{
					board[i, j] = 0;
				}
			}
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
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
			int ii = 0;
			int jj = 0;
			int names;
			int[] ptr;

			Console.WriteLine("hits = {0}", hits);
			ptr = buffer;
			for(i = 0; i < hits; i++) 
			{  // for each hit
				names = ptr[i];
				Console.WriteLine(" number of names for this hit = {0}", names);
				i++;;
				Console.WriteLine("  z1 is {0}", (float) ptr[i] / 0x7fffffff);
				i++;
				Console.WriteLine("  z2 is {0}", (float) ptr[i] / 0x7fffffff);
				i++;
				Console.Write("   names are ");
				for(j = 0; j < names; j++) 
				{  // for each name
					Console.Write("{0} ", ptr[i]);
					if(j == 0) 
					{  // set row and column
						ii = ptr[i];
					}
					else if(j == 1) 
					{
						jj = ptr[i];
					}
					i++;
				}
				Console.WriteLine();
				board[ii, jj] = (board[ii, jj] + 1) % 3;
			}
			Console.WriteLine();
		}
		#endregion ProcessHits(int hits, int[] buffer)

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
			DrawSquares(Gl.GL_RENDER);
			Gl.glFlush();
		}
		#endregion Display()

		#region Mouse(int button, int state, int x, int y)
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

			if(e.Button != MouseButton.SecondaryButton || e.ButtonPressed == false) 
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
			Glu.gluOrtho2D (0.0, 3.0, 0.0, 3.0);
			DrawSquares(Gl.GL_SELECT);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPopMatrix();
			Gl.glFlush();

			hits = Gl.glRenderMode(Gl.GL_RENDER);
			ProcessHits(hits, selectBuffer);
		}
		#endregion Mouse(int button, int state, int x, int y)

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluOrtho2D(0.0, 3.0, 0.0, 3.0);
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
	}
}