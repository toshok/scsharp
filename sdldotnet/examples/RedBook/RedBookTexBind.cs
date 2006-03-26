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
	///     This program demonstrates using Gl.glBindTexture() by creating and managing two
	///     textures.
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
	public class RedBookTexBind
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
				return "TexBind - creating and managing two textures.";
			}
		}

		#region Private Constants
		private const int CHECKWIDTH = 64;
		private const int CHECKHEIGHT = 64;
		#endregion Private Constants

		#region Private Fields
		private static byte[ , , ] checkImage = new byte[CHECKHEIGHT, CHECKWIDTH, 4];
		private static byte[ , , ] otherImage = new byte[CHECKHEIGHT, CHECKWIDTH, 4];
		private static int[] textures = new int[2];
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookTexBind()
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

			Gl.glGenTextures(2, textures);
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[0]);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, CHECKWIDTH, CHECKHEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, checkImage);

			Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[1]);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
			Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_DECAL);
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, CHECKWIDTH, CHECKHEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, otherImage);
			Gl.glEnable(Gl.GL_TEXTURE_2D);
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

					if(((i & 0x10) == 0) ^ ((j & 0x10) == 0)) 
					{
						c = 255;
					}
					else 
					{
						c = 0;
					}
					otherImage[i, j, 0] = (byte) c;
					otherImage[i, j, 1] = (byte) 0;
					otherImage[i, j, 2] = (byte) 0;
					otherImage[i, j, 3] = (byte) 255;
				}
			}
		}
		#endregion MakeCheckImages()

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[0]);
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0, 0);
			Gl.glVertex3f(-2, -1, 0);
			Gl.glTexCoord2f(0, 1);
			Gl.glVertex3f(-2, 1, 0);
			Gl.glTexCoord2f(1, 1);
			Gl.glVertex3f(0, 1, 0);
			Gl.glTexCoord2f(1, 0);
			Gl.glVertex3f(0, -1, 0);
			Gl.glEnd();
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[1]);
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2f(0, 0);
			Gl.glVertex3f(1, -1, 0);
			Gl.glTexCoord2f(0, 1);
			Gl.glVertex3f(1, 1, 0);
			Gl.glTexCoord2f(1, 1);
			Gl.glVertex3f(2.41421f, 1, -1.41421f);
			Gl.glTexCoord2f(1, 0);
			Gl.glVertex3f(2.41421f, -1, -1.41421f);
			Gl.glEnd();
			Gl.glFlush();
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