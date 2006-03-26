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
	///     This program demonstrates use of OpenGL feedback.  First,a lighting environment
	///     is set up and a few lines are drawn.  Then feedback mode is entered, and the
	///     same lines are drawn.  The results in the feedback buffer are printed.
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
	public class RedBookFeedback
	{
		#region Fields

		//Width of screen
		int width = 100;
		//Height of screen
		int height = 100;	
		
		

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Feedback - Feedback mode";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookFeedback()
		{
			Initialize();
		}

		#endregion Constructors

		#region DrawGeometry(int mode)
		/// <summary>
		///     <para>
		///         Draw a few lines and two points, one of which will be clipped.  If in
		///         feedback mode, a passthrough token is issued between the each primitive.
		///     </para>
		/// </summary>
		private static void DrawGeometry(int mode) 
		{
			Gl.glBegin(Gl.GL_LINE_STRIP);
			Gl.glNormal3f(0.0f, 0.0f, 1.0f);
			Gl.glVertex3f(30.0f, 30.0f, 0.0f);
			Gl.glVertex3f(50.0f, 60.0f, 0.0f);
			Gl.glVertex3f(70.0f, 40.0f, 0.0f);
			Gl.glEnd();

			if(mode == Gl.GL_FEEDBACK) 
			{
				Gl.glPassThrough(1.0f);
			}

			Gl.glBegin(Gl.GL_POINTS);
			Gl.glVertex3f(-100.0f, -100.0f, -100.0f);  //  will be clipped
			Gl.glEnd();
            
			if(mode == Gl.GL_FEEDBACK) 
			{
				Gl.glPassThrough(2.0f);
			}

			Gl.glBegin(Gl.GL_POINTS);
			Gl.glNormal3f(0.0f, 0.0f, 1.0f);
			Gl.glVertex3f(50.0f, 50.0f, 0.0f);
			Gl.glEnd();
		}
		#endregion DrawGeometry(int mode)

		#region Print3dColorVertex(int size, int count, float[] buffer)
		/// <summary>
		///     <para>
		///         Write contents of one vertex to console.
		///     </para>
		/// </summary>
		private static void Print3dColorVertex(int size, int count, float[] buffer) 
		{
			Console.Write("  ");
			for(int i = 0; i < 7; i++) 
			{
				Console.Write("{0:F2} ", buffer[size - count]);
				count = count - 1;
			}
			Console.Write("\n");
		}
		#endregion Print3dColorVertex(int size, int count, float[] buffer)

		#region PrintBuffer(int size, float[] buffer)
		/// <summary>
		///     <para>
		///         Write the contents of the entire buffer.  (Parse tokens!)
		///     </para>
		/// </summary>
		private static void PrintBuffer(int size, float[] buffer) 
		{
			int count;
			float token;

			count = size;
			while(count != 0) 
			{
				token = buffer[size - count];
				count--;
				if(token == Gl.GL_PASS_THROUGH_TOKEN) 
				{
					Console.WriteLine("GL_PASS_THROUGH_TOKEN");
					Console.WriteLine("  {0:F2}", buffer[size - count]);
					count--;
				}
				else if(token == Gl.GL_POINT_TOKEN) 
				{
					Console.WriteLine("GL_POINT_TOKEN");
					Print3dColorVertex(size, count, buffer);
				}
				else if(token == Gl.GL_LINE_TOKEN) 
				{
					Console.WriteLine("GL_LINE_TOKEN");
					Print3dColorVertex(size, count, buffer);
					Print3dColorVertex(size, count, buffer);
				}
				else if(token == Gl.GL_LINE_RESET_TOKEN) 
				{
					Console.WriteLine("GL_LINE_RESET_TOKEN");
					Print3dColorVertex(size, count, buffer);
					Print3dColorVertex(size, count, buffer);
				}
			}
		}
		#endregion PrintBuffer(int size, float[] buffer)

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
			Gl.glOrtho(0, w, 0, h, -1.0, 1.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
		}

		/// <summary>
		///     <para>
		///         Initialize antialiasing for RGBA mode, including alpha blending, hint, and
		///         line width.  Print out implementation specific info on line width granularity
		///         and width.
		///     </para>
		/// </summary>
		private static void Init()
		{
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
		}

		#endregion Lesson Setup

		#region void Display
		/// <summary>
		/// Renders the scene
		/// </summary>
		private static void Display()
		{
			float[] feedBuffer = new float[1024];
			int size;

			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Gl.glOrtho(0.0, 100.0, 0.0, 100.0, 0.0, 1.0);

			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
			DrawGeometry(Gl.GL_RENDER);

			Gl.glFeedbackBuffer(1024, Gl.GL_3D_COLOR, feedBuffer);
			Gl.glRenderMode(Gl.GL_FEEDBACK);
			DrawGeometry(Gl.GL_FEEDBACK);

			size = Gl.glRenderMode(Gl.GL_RENDER);
			PrintBuffer(size, feedBuffer);
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