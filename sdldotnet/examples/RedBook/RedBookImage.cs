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
	///     This program demonstrates drawing pixels and shows the effect of
	///     Gl.glDrawPixels(), Gl.glCopyPixels(), and Gl.glPixelZoom().  Interaction:
	///     moving the mouse while pressing the mouse button will copy the image in the
	///     lower-left corner of the window to the mouse position, using the current pixel
	///     zoom factors.  There is no attempt to prevent you from drawing over the original
	///     image.  If you press the 'r' key, the original image and zoom factors are reset.
	///     If you press the 'z' or 'Z' keys, you change the zoom factors.
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
	public class RedBookImage
	{
		#region Fields

		//Width of screen
		int width = 250;
		//Height of screen
		int height = 250;		
		
		

		private const int CHECKWIDTH = 64;
		private const int CHECKHEIGHT = 64;

		private static byte[ , , ] checkImage = new byte[CHECKWIDTH, CHECKHEIGHT, 3];
		private static double zoomFactor = 1.0;

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Image - moving an image. TODO";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookImage()
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
			Events.MouseMotion += new MouseMotionEventHandler(this.MouseMotion);
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
			//height = h;
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluOrtho2D(0.0, (double) w, 0.0, (double) h);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity ();
		}

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		private static void Init()
		{
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glShadeModel(Gl.GL_FLAT);
			MakeCheckImage();
			Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
		}

		#endregion Lesson Setup

		#region void Display
		/// <summary>
		/// Renders the scene
		/// </summary>
		private static void Display()
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
			Gl.glRasterPos2i(0, 0);
			Gl.glDrawPixels(CHECKWIDTH, CHECKHEIGHT, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, checkImage);
			Gl.glFlush();
		}
		#endregion void Display

		#region MakeCheckImage()
		private static void MakeCheckImage() 
		{
			int i, j, c;

			for(i = 0; i < CHECKWIDTH; i++) 
			{
				for(j = 0; j < CHECKHEIGHT; j++) 
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
				}
			}
		}
		#endregion MakeCheckImage()

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.Escape:
					// Will stop the app loop
					Events.QuitApplication();
					break;
				case Key.R:
					zoomFactor = 1.0;
					Console.WriteLine("zoomFactor reset to 1.0");
					break;
				case Key.Z:
					zoomFactor += 0.5;
					if(zoomFactor >= 3.0) 
					{
						zoomFactor = 3.0;
					}
					Console.WriteLine("zoomFactor is now {0:F1}", zoomFactor);
					break;
				case Key.A:
					zoomFactor -= 0.5;
					if(zoomFactor <= 0.5) 
					{
						zoomFactor = 0.5;
					}
					Console.WriteLine("zoomFactor is now {0:F1}", zoomFactor);
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

		private void MouseMotion(object sender, MouseMotionEventArgs e)
		{
			if (e.ButtonPressed)
			{
				Gl.glRasterPos2i(100, 100);
				Gl.glPixelZoom((float) zoomFactor, (float) zoomFactor);
				Gl.glCopyPixels(0, 0, CHECKWIDTH, CHECKHEIGHT, Gl.GL_COLOR);
				Gl.glPixelZoom(1.0f, 1.0f);
				Gl.glFlush();
			}
		}

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