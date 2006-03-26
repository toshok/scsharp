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
	///     The brief program illustrates use of texture proxies.  This program only prints
	///     out some messages about whether certain size textures are supported and then exits.
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
	public class RedBookTexProx
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
				return "TexProx - texture proxies. TODO";
			}
		}

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookTexProx()
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
			int[] proxyComponents = new int[1];
			byte[] nullImage = null;

			Console.WriteLine();

			Gl.glTexImage2D(Gl.GL_PROXY_TEXTURE_2D, 0, Gl.GL_RGBA8, 64, 64, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, nullImage);
			Gl.glGetTexLevelParameteriv(Gl.GL_PROXY_TEXTURE_2D, 0, Gl.GL_TEXTURE_INTERNAL_FORMAT, proxyComponents);
			Console.WriteLine("Proxying 64x64 level 0 RGBA8 texture (level 0)");
			if(proxyComponents[0] == Gl.GL_RGBA8) 
			{
				Console.WriteLine("proxy allocation succeeded");
			}
			else 
			{
				Console.WriteLine("proxy allocation failed");
			}
			Console.WriteLine();

			Gl.glTexImage2D(Gl.GL_PROXY_TEXTURE_2D, 0, Gl.GL_RGBA16, 2048, 2048, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_SHORT, nullImage);
			Gl.glGetTexLevelParameteriv(Gl.GL_PROXY_TEXTURE_2D, 0, Gl.GL_TEXTURE_INTERNAL_FORMAT, proxyComponents);
			Console.WriteLine("Proxying 2048x2048 level 0 RGBA16 texture (big so unlikely to be supported)");
			if(proxyComponents[0] == Gl.GL_RGBA16) 
			{
				Console.WriteLine("proxy allocation succeeded");
			}
			else 
			{
				Console.WriteLine("proxy allocation failed");
			}
			Console.WriteLine();

			Console.WriteLine("Press Enter to exit...");
			Console.ReadLine();
		}
		#endregion Init()

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Environment.Exit(0);
		}
		#endregion Display()

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
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