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
	///     This program texture maps a checkerboard image onto two rectangles.  This
	///     program clamps the texture, if the texture coordinates fall outside 0.0 and 1.0.
	///     If the s key is pressed, a texture subimage is used to alter the original
	///     texture.  If the r key is pressed, the original texture is restored.
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
	public class RedBookTexSub
	{
		//Width of screen
		int width = 250;
		//Height of screen
		int height = 250;
		
		

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "TexSub - texture subimage.";
			}
		}

		#region Private Constants
		private const int CHECKWIDTH = 64;
		private const int CHECKHEIGHT = 64;
		private const int SUBWIDTH = 16;
		private const int SUBHEIGHT = 16;
		#endregion Private Constants

		#region Private Fields
		private static byte[ , , ] checkImage = new byte[CHECKHEIGHT, CHECKWIDTH, 4];
		private static byte[ , , ] subImage = new byte[SUBHEIGHT, SUBWIDTH, 4];
		private static int texture;
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookTexSub()
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
			Gl.glEnable(Gl.GL_DEPTH_TEST);

			MakeCheckImages();
			Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

			Gl.glGenTextures(1, out texture);
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);

			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, CHECKWIDTH, CHECKHEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, checkImage);
		}
		#endregion Init()

		#region MakeCheckImages()
		private static void MakeCheckImages() 
		{
			int i, j, c;
    
			for(i = 0; i < CHECKHEIGHT; i++) 
			{
				for(j = 0; j < CHECKWIDTH; j++) 
				{
					if(((i & 0x8) == 0) ^ ((j & 0x8) == 0)) 
					{
						c = 255;
					}
					else 
					{
						c = 0;
					}
					checkImage[i, j, 0] = (byte) c;
					checkImage[i, j, 1] = (byte) c;
					checkImage[i, j, 2] = (byte) c;
					checkImage[i, j, 3] = (byte) 255;
				}
			}

			for(i = 0; i < SUBHEIGHT; i++) 
			{
				for(j = 0; j < SUBWIDTH; j++) 
				{
					if(((i & 0x4) == 0) ^ ((j & 0x4) == 0)) 
					{
						c = 255;
					}
					else 
					{
						c = 0;
					}
					subImage[i, j, 0] = (byte) c;
					subImage[i, j, 1] = (byte) 0;
					subImage[i, j, 2] = (byte) 0;
					subImage[i, j, 3] = (byte) 255;
				}
			}
		}
		#endregion MakeCheckImages()

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_DECAL);
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0.0f, 0.0f);
			Gl.glVertex3f(-2.0f, -1.0f, 0.0f);
			Gl.glTexCoord2f(0.0f, 1.0f);
			Gl.glVertex3f(-2.0f, 1.0f, 0.0f);
			Gl.glTexCoord2f(1.0f, 1.0f);
			Gl.glVertex3f(0.0f, 1.0f, 0.0f);
			Gl.glTexCoord2f(1.0f, 0.0f);
			Gl.glVertex3f(0.0f, -1.0f, 0.0f);

			Gl.glTexCoord2f(0.0f, 0.0f);
			Gl.glVertex3f(1.0f, -1.0f, 0.0f);
			Gl.glTexCoord2f(0.0f, 1.0f);
			Gl.glVertex3f(1.0f, 1.0f, 0.0f);
			Gl.glTexCoord2f(1.0f, 1.0f);
			Gl.glVertex3f(2.41421f, 1.0f, -1.41421f);
			Gl.glTexCoord2f(1.0f, 0.0f);
			Gl.glVertex3f(2.41421f, -1.0f, -1.41421f);
			Gl.glEnd();
			Gl.glFlush();
			Gl.glDisable(Gl.GL_TEXTURE_2D);
		}
		#endregion Display()

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluPerspective(60.0, (float) w / (float) h, 1.0, 30.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Gl.glTranslatef(0.0f, 0.0f, -3.6f);
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
				case Key.S:
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);
					Gl.glTexSubImage2D(Gl.GL_TEXTURE_2D, 0, 12, 44, SUBWIDTH, SUBHEIGHT, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, subImage);
					break;
				case Key.R:
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);
					Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, CHECKWIDTH, CHECKHEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, checkImage);
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