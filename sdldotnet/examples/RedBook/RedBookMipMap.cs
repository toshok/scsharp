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
	///     This program demonstrates using mipmaps for texture maps.  To overtly show the
	///     effect of mipmaps, each mipmap reduction level has a solidly colored, contrasting
	///     texture image.  Thus, the quadrilateral which is drawn is drawn with several
	///     different colors.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
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
	public class RedBookMipMap
	{
		#region Fields

		//Width of screen
		static int width = 500;
		//Height of screen
		static int height = 500;

		private static byte[ , , ] mipmapImage32 = new byte[32, 32, 4];
		private static byte[ , , ] mipmapImage16 = new byte[16, 16, 4];
		private static byte[ , , ] mipmapImage8 = new byte[8, 8, 4];
		private static byte[ , , ] mipmapImage4 = new byte[4, 4, 4];
		private static byte[ , , ] mipmapImage2 = new byte[2, 2, 4];
		private static byte[ , , ] mipmapImage1 = new byte[1, 1, 4];
		private static int texture;

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "MipMap - MipMaps for texture maps";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookMipMap()
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
		private static void Reshape()
		{
			Reshape(width, height);
		}

		/// <summary>
		/// Resizes window
		/// </summary>
		/// <param name="h"></param>
		/// <param name="w"></param>
		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluPerspective(60.0, (float) w / (float) h, 1.0, 30000.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
		}
		#endregion Reshape(int w, int h)

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		private static void Init()
		{
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glShadeModel(Gl.GL_FLAT);

			Gl.glTranslatef(0.0f, 0.0f, -3.6f);
			MakeImages();
			Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

			Gl.glGenTextures(1, out texture);
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_NEAREST);
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, 32, 32, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, mipmapImage32);
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 1, Gl.GL_RGBA, 16, 16, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, mipmapImage16);
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 2, Gl.GL_RGBA, 8, 8, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, mipmapImage8);
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 3, Gl.GL_RGBA, 4, 4, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, mipmapImage4);
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 4, Gl.GL_RGBA, 2, 2, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, mipmapImage2);
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 5, Gl.GL_RGBA, 1, 1, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, mipmapImage1);

			Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_DECAL);
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			Reshape();
		}

		#endregion Lesson Setup

		#region MakeImages()
		private static void MakeImages() 
		{
			int i, j;

			for(i = 0; i < 32; i++) 
			{
				for(j = 0; j < 32; j++) 
				{
					mipmapImage32[i, j, 0] = 255;
					mipmapImage32[i, j, 1] = 255;
					mipmapImage32[i, j, 2] = 0;
					mipmapImage32[i, j, 3] = 255;
				}
			}
			for(i = 0; i < 16; i++) 
			{
				for(j = 0; j < 16; j++) 
				{
					mipmapImage16[i, j, 0] = 255;
					mipmapImage16[i, j, 1] = 0;
					mipmapImage16[i, j, 2] = 255;
					mipmapImage16[i, j, 3] = 255;
				}
			}
			for(i = 0; i < 8; i++) 
			{
				for(j = 0; j < 8; j++) 
				{
					mipmapImage8[i, j, 0] = 255;
					mipmapImage8[i, j, 1] = 0;
					mipmapImage8[i, j, 2] = 0;
					mipmapImage8[i, j, 3] = 255;
				}
			}
			for(i = 0; i < 4; i++) 
			{
				for(j = 0; j < 4; j++) 
				{
					mipmapImage4[i, j, 0] = 0;
					mipmapImage4[i, j, 1] = 255;
					mipmapImage4[i, j, 2] = 0;
					mipmapImage4[i, j, 3] = 255;
				}
			}
			for(i = 0; i < 2; i++) 
			{
				for(j = 0; j < 2; j++) 
				{
					mipmapImage2[i, j, 0] = 0;
					mipmapImage2[i, j, 1] = 0;
					mipmapImage2[i, j, 2] = 255;
					mipmapImage2[i, j, 3] = 255;
				}
			}
			mipmapImage1[0, 0, 0] = 255;
			mipmapImage1[0, 0, 1] = 255;
			mipmapImage1[0, 0, 2] = 255;
			mipmapImage1[0, 0, 3] = 255;
		}
		#endregion MakeImages()

		#region void Display
		/*  Draw twelve spheres in 3 rows with 4 columns.  
			*   The spheres in the first row have materials with no ambient reflection.
			*   The second row has materials with significant ambient reflection.
			*   The third row has materials with colored ambient reflection.
			*
			*   The first column has materials with blue, diffuse reflection only.
			*   The second column has blue diffuse reflection, as well as specular
			*   reflection with a low shininess exponent.
			*   The third column has blue diffuse reflection, as well as specular
			*   reflection with a high shininess exponent (a more concentrated highlight).
			*   The fourth column has materials which also include an emissive component.
			*
			*   Gl.glTranslatef() is used to move spheres to their appropriate locations.
			*/
		private static void Display()
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0.0f, 0.0f);
			Gl.glVertex3f(-2.0f, -1.0f, 0.0f);
			Gl.glTexCoord2f(0.0f, 8.0f);
			Gl.glVertex3f(-2.0f, 1.0f, 0.0f);
			Gl.glTexCoord2f(8.0f, 8.0f);
			Gl.glVertex3f(2000.0f, 1.0f, -6000.0f);
			Gl.glTexCoord2f(8.0f, 0.0f);
			Gl.glVertex3f(2000.0f, -1.0f, -6000.0f);
			Gl.glEnd();
			Gl.glFlush();
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