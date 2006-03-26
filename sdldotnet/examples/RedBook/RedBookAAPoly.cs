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
	///     This program draws filled polygons with antialiased edges.  The special
	///     GL_SRC_ALPHA_SATURATE blending function is used.  Pressing the 't' key turns the
	///     antialiasing on and off.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/aaindex.c
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
	public class RedBookAAPoly
	{
		#region Fields

		//Width of screen
		int width = 200;
		//Height of screen
		int height = 200;
		
		
		
		
		private const int FACES = 6;
		private static bool polySmooth = true;

		private static float[/*8*/, /*3*/] v = new float [8, 3];
		private static float[/*8*/, /*4*/] c = {
			{0.0f, 0.0f, 0.0f, 1.0f},
			{1.0f, 0.0f, 0.0f, 1.0f},
			{0.0f, 1.0f, 0.0f, 1.0f},
			{1.0f, 1.0f, 0.0f, 1.0f},
			{0.0f, 0.0f, 1.0f, 1.0f},
			{1.0f, 0.0f, 1.0f, 1.0f},
			{0.0f, 1.0f, 1.0f, 1.0f},
			{1.0f, 1.0f, 1.0f, 1.0f}
											   };

		// indices of front, top, left, bottom, right, back faces
		private static byte[/*6*/, /*4*/] indices = {
			{4, 5, 6, 7},
			{2, 3, 7, 6},
			{0, 4, 7, 3},
			{0, 1, 5, 4},
			{1, 5, 6, 2},
			{0, 3, 2, 1}
													};

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "AAPoly - Filled polygons";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookAAPoly()
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
		/// <param name="h">height of windoww</param>
		/// <param name="w">width of window</param>
		private static void Reshape(int w, int h)
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluPerspective(30.0, (float) w / (float) h, 1.0, 20.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
		}

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		private static void Init()
		{ 
			Gl.glCullFace(Gl.GL_BACK);
			Gl.glEnable(Gl.GL_CULL_FACE);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA_SATURATE, Gl.GL_ONE);
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
		}

		#region DrawCube(float x0, float x1, float y0, float y1, float z0, float z1)
		private static void DrawCube(float x0, float x1, float y0, float y1, float z0, float z1) 
		{
			v[0, 0] = v[3, 0] = v[4, 0] = v[7, 0] = x0;
			v[1, 0] = v[2, 0] = v[5, 0] = v[6, 0] = x1;
			v[0, 1] = v[1, 1] = v[4, 1] = v[5, 1] = y0;
			v[2, 1] = v[3, 1] = v[6, 1] = v[7, 1] = y1;
			v[0, 2] = v[1, 2] = v[2, 2] = v[3, 2] = z0;
			v[4, 2] = v[5, 2] = v[6, 2] = v[7, 2] = z1;

			Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
			Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);
			Gl.glVertexPointer(3, Gl.GL_FLOAT, 0, v);
			Gl.glColorPointer(4, Gl.GL_FLOAT, 0, c);
			Gl.glDrawElements(Gl.GL_QUADS, FACES * 4, Gl.GL_UNSIGNED_BYTE, indices);
			Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
			Gl.glDisableClientState(Gl.GL_COLOR_ARRAY);
		}
		#endregion DrawCube(float x0, float x1, float y0, float y1, float z0, float z1)

		#endregion Lesson Setup

		#region void Display
		/// <summary>
		/// Renders the scene
		/// </summary>
		private static void Display()
		{
			if(polySmooth) 
			{
				Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
				Gl.glEnable(Gl.GL_BLEND);
				Gl.glEnable(Gl.GL_POLYGON_SMOOTH);
				Gl.glDisable(Gl.GL_DEPTH_TEST);
			}
			else 
			{
				Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
				Gl.glDisable(Gl.GL_BLEND);
				Gl.glDisable(Gl.GL_POLYGON_SMOOTH);
				Gl.glEnable(Gl.GL_DEPTH_TEST);
			}

			Gl.glPushMatrix();
			Gl.glTranslatef(0.0f, 0.0f, -8.0f);    
			Gl.glRotatef(30.0f, 1.0f, 0.0f, 0.0f);
			Gl.glRotatef(60.0f, 0.0f, 1.0f, 0.0f); 
			DrawCube(-0.5f, 0.5f, -0.5f, 0.5f, -0.5f, 0.5f);
			Gl.glPopMatrix ();

			Gl.glFlush ();
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
				case Key.T:
					polySmooth = !polySmooth;
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