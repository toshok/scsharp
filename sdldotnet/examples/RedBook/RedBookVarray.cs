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
	///     This program demonstrates vertex arrays.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/stroke.c
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
	public class RedBookVarray
	{
		//Width of screen
		int width = 350;
		//Height of screen
		int height = 350;
		
		

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Varray - vertex arrays";
			}
		}

		#region Private Constants
		private const int POINTER = 1;
		private const int INTERLEAVED = 2;
		private const int DRAWARRAY = 1;
		private const int ARRAYELEMENT = 2;
		private const int DRAWELEMENTS = 3;
		#endregion Private Constants

		#region Private Fields
		private static int setupMethod = POINTER;
		private static int derefMethod = DRAWARRAY;

		private static float[] intertwined = {
										  1.0f, 0.2f, 1.0f, 100.0f, 100.0f, 0.0f,
										  1.0f, 0.2f, 0.2f, 0.0f, 200.0f, 0.0f,
										  1.0f, 1.0f, 0.2f, 100.0f, 300.0f, 0.0f,
										  0.2f, 1.0f, 0.2f, 200.0f, 300.0f, 0.0f,
										  0.2f, 1.0f, 1.0f, 300.0f, 200.0f, 0.0f,
										  0.2f, 0.2f, 1.0f, 200.0f, 100.0f, 0.0f
									  };

		private static int[] vertices = {
									 25, 25,
									 100, 325,
									 175, 25,
									 175, 325,
									 250, 25,
									 325, 325
								 };

		private static float[] colors = {
									 1.0f, 0.2f, 0.2f,
									 0.2f, 0.2f, 1.0f,
									 0.8f, 1.0f, 0.2f,
									 0.75f, 0.75f, 0.75f,
									 0.35f, 0.35f, 0.35f,
									 0.5f, 0.5f, 0.5f
								 };
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookVarray()
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
		#region Init()
		private static void Init() 
		{
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glShadeModel(Gl.GL_SMOOTH);
			SetupPointers();
		}
		#endregion Init()

		#region SetupInterleave()
		private static void SetupInterleave() 
		{
			Gl.glInterleavedArrays(Gl.GL_C3F_V3F, 0, intertwined);
		}
		#endregion SetupInterleave()

		#region SetupPointers()
		private static void SetupPointers() 
		{
			Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
			Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);

			Gl.glVertexPointer(2, Gl.GL_INT, 0, vertices);
			Gl.glColorPointer(3, Gl.GL_FLOAT, 0, colors);
		}
		#endregion SetupPointers()

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

			if(derefMethod == DRAWARRAY) 
			{
				Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
			}
			else if(derefMethod == ARRAYELEMENT) 
			{
				Gl.glBegin(Gl.GL_TRIANGLES);
				Gl.glArrayElement(2);
				Gl.glArrayElement(3);
				Gl.glArrayElement(5);
				Gl.glEnd();
			}
			else if(derefMethod == DRAWELEMENTS) 
			{
				int[] indices = {0, 1, 3, 4};

				Gl.glDrawElements(Gl.GL_POLYGON, 4, Gl.GL_UNSIGNED_INT, indices);
			}
			Gl.glFlush();
		}
		#endregion Display()

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluOrtho2D(0.0, (double) w, 0.0, (double) h);
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

		private void MouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			switch(e.Button) 
			{
				case MouseButton.PrimaryButton:
					if(setupMethod == POINTER) 
					{
						setupMethod = INTERLEAVED;
						SetupInterleave();
					}
					else if(setupMethod == INTERLEAVED) 
					{
						setupMethod = POINTER;
						SetupPointers();
					}
					break;
				case MouseButton.SecondaryButton:
					if(derefMethod == DRAWARRAY) 
					{
						derefMethod = ARRAYELEMENT;
					}
					else if(derefMethod == ARRAYELEMENT) 
					{
						derefMethod = DRAWELEMENTS;
					}
					else if (derefMethod == DRAWELEMENTS) 
					{
						derefMethod = DRAWARRAY;
					}
					break;
				default:
					break;

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