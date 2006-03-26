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
	///     This program uses evaluators to generate a curved surface and automatically
	///     generated texture coordinates.
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
	public class RedBookTextureSurf
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
				return "TextureSurf - curved surface and textures";
			}
		}

		#region Private Constants
		private const int IMAGEWIDTH = 64;
		private const int IMAGEHEIGHT = 64;
		#endregion Private Constants

		#region Private Fields
		private static byte[] image = new byte[IMAGEWIDTH * IMAGEHEIGHT * 3];

		private static float[ , , ] controlPoints = {
			{
				{-1.5f, -1.5f, 4.0f},
				{-0.5f, -1.5f, 2.0f},
				{0.5f, -1.5f, -1.0f},
				{1.5f, -1.5f, 2.0f}
			},
			{
				{-1.5f, -0.5f, 1.0f},
				{-0.5f, -0.5f, 3.0f},
				{0.5f, -0.5f, 0.0f},
				{1.5f, -0.5f, -1.0f}
			},
			{
				{-1.5f, 0.5f, 4.0f},
				{-0.5f, 0.5f, 0.0f},
				{0.5f, 0.5f, 3.0f},
				{1.5f, 0.5f, 4.0f}
			},
			{
				{-1.5f, 1.5f, -2.0f},
				{-0.5f, 1.5f, -2.0f},
				{0.5f, 1.5f, 0.0f},
				{1.5f, 1.5f, -1.0f}
			}
													};

		private static float[ , , ] texturePoints = {
			{
				{0.0f, 0.0f},
				{0.0f, 1.0f}
			},
			{
				{1.0f, 0.0f},
				{1.0f, 1.0f}
			}
													};
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookTextureSurf()
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
			Gl.glMap2f(Gl.GL_MAP2_VERTEX_3, 0, 1, 3, 4, 0, 1, 12, 4, ref controlPoints[0,0,0]);
			Gl.glMap2f(Gl.GL_MAP2_TEXTURE_COORD_2, 0, 1, 2, 2, 0, 1, 4, 2, ref texturePoints[0,0,0]);
			Gl.glEnable(Gl.GL_MAP2_TEXTURE_COORD_2);
			Gl.glEnable(Gl.GL_MAP2_VERTEX_3);
			Gl.glMapGrid2f(20, 0, 1, 20, 0, 1);
			MakeImage();
			Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_DECAL);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, IMAGEWIDTH, IMAGEHEIGHT, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, image);
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glShadeModel(Gl.GL_FLAT);
		}
		#endregion Init()

		#region MakeImage()
		private static void MakeImage() 
		{
			int i, j;
			float ti, tj;
   
			for(i = 0; i < IMAGEWIDTH; i++) 
			{
				ti = 2.0f * 3.14159265f * i / IMAGEWIDTH;
				for(j = 0; j < IMAGEHEIGHT; j++) 
				{
					tj = 2.0f * 3.14159265f * j / IMAGEHEIGHT;
					image[3 * (IMAGEHEIGHT * i + j)] = (byte) (127 * (1.0 + Math.Sin(ti)));
					image[3 * (IMAGEHEIGHT * i + j) + 1] = (byte) (127 * (1.0 + Math.Cos(2 * tj)));
					image[3 * (IMAGEHEIGHT * i + j) + 2] = (byte) (127 * (1.0 + Math.Cos(ti + tj)));
				}
			}
		}
		#endregion MakeImage()

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glColor3f(1.0f, 1.0f, 1.0f);
			Gl.glEvalMesh2(Gl.GL_FILL, 0, 20, 0, 20);
			Gl.glFlush();
		}
		#endregion Display()

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			if(w <= h) 
			{
				Gl.glOrtho(-4.0, 4.0, -4.0 * (float) h / (float) w, 4.0 * (float) h / (float) w, -4.0, 4.0);
			}
			else 
			{
				Gl.glOrtho(-4.0 * (float) w / (float) h, 4.0 * (float) w / (float) h, -4.0, 4.0, -4.0, 4.0);
			}
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Gl.glRotatef(85.0f, 1.0f, 1.0f, 1.0f);
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