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
	///     This program draws a texture mapped teapot with automatically generated texture
	///     coordinates.  The texture is rendered as stripes on the teapot.  Initially, the
	///     object is drawn with texture coordinates based upon the object coordinates of the
	///     vertex and distance from the plane x = 0.  Pressing the 'e' key changes the
	///     coordinate generation to eye coordinates of the vertex.  Pressing the 'o' key
	///     switches it back to the object coordinates.  Pressing the 's' key changes the
	///     plane to a slanted one (x + y + z = 0).  Pressing the 'x' key switches it back to
	///     x = 0.
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
	public class RedBookTexGen
	{
		//Width of screen
		int width = 256;
		//Height of screen
		int height = 256;
		
		

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "TexGen - generated texture coordinates.";
			}
		}

		#region Private Constants
		private const int STRIPEWIDTH = 32;
		#endregion Private Constants

		#region Private Fields
		private static byte[] stripeImage = new byte[STRIPEWIDTH * 4];
		private static int texture;

		// planes for texture coordinate generation
		private static float[] xequalzero = {1.0f, 0.0f, 0.0f, 0.0f};
		private static float[] slanted = {1.0f, 1.0f, 1.0f, 0.0f};
		private static float[] currentCoeff;
		private static int currentPlane;
		private static int currentGenMode;
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookTexGen()
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
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glShadeModel(Gl.GL_SMOOTH);

			MakeStripeImage();
			Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

			Gl.glGenTextures(1, out texture);
			Gl.glBindTexture(Gl.GL_TEXTURE_1D, texture);
			Gl.glTexParameteri(Gl.GL_TEXTURE_1D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
			Gl.glTexParameteri(Gl.GL_TEXTURE_1D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
			Gl.glTexParameteri(Gl.GL_TEXTURE_1D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
			Gl.glTexImage1D(Gl.GL_TEXTURE_1D, 0, Gl.GL_RGBA, STRIPEWIDTH, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, stripeImage);

			Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
			currentCoeff = xequalzero;
			currentGenMode = Gl.GL_OBJECT_LINEAR;
			currentPlane = Gl.GL_OBJECT_PLANE;
			Gl.glTexGeni(Gl.GL_S, Gl.GL_TEXTURE_GEN_MODE, currentGenMode);
			Gl.glTexGenfv(Gl.GL_S, currentPlane, currentCoeff);

			Gl.glEnable(Gl.GL_TEXTURE_GEN_S);
			Gl.glEnable(Gl.GL_TEXTURE_1D);
			Gl.glEnable(Gl.GL_CULL_FACE);
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_AUTO_NORMAL);
			Gl.glEnable(Gl.GL_NORMALIZE);
			Gl.glFrontFace(Gl.GL_CW);
			Gl.glCullFace(Gl.GL_BACK);
			Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 64.0f);
		}
		#endregion Init()

		#region MakeStripeImage()
		private static void MakeStripeImage() 
		{
			for(int j = 0; j < STRIPEWIDTH; j++) 
			{
				stripeImage[4 * j] = (byte) ((j <= 4) ? 255 : 0);
				stripeImage[4 * j + 1] = (byte) ((j > 4) ? 255 : 0);
				stripeImage[4 * j + 2] = (byte) 0;
				stripeImage[4 * j + 3] = (byte) 255;
			}
		}
		#endregion MakeStripeImage()

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glPushMatrix();
			Gl.glRotatef(45.0f, 0.0f, 0.0f, 1.0f);
			Gl.glBindTexture(Gl.GL_TEXTURE_1D, texture);
			Glut.glutSolidTeapot(2.0);
			Gl.glPopMatrix();
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
				Gl.glOrtho(-3.5, 3.5, -3.5 * (float) h / (float) w, 3.5 * (float) h / (float) w, -3.5, 3.5);
			}
			else 
			{
				Gl.glOrtho(-3.5 * (float) w / (float) h, 3.5 * (float) w / (float) h, -3.5, 3.5, -3.5, 3.5);
			}
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
				case Key.E:
					currentGenMode = Gl.GL_EYE_LINEAR;
					currentPlane = Gl.GL_EYE_PLANE;
					Gl.glTexGeni(Gl.GL_S, Gl.GL_TEXTURE_GEN_MODE, currentGenMode);
					Gl.glTexGenfv(Gl.GL_S, currentPlane, currentCoeff);
					break;
				case Key.O:
					currentGenMode = Gl.GL_OBJECT_LINEAR;
					currentPlane = Gl.GL_OBJECT_PLANE;
					Gl.glTexGeni(Gl.GL_S, Gl.GL_TEXTURE_GEN_MODE, currentGenMode);
					Gl.glTexGenfv(Gl.GL_S, currentPlane, currentCoeff);
					break;
				case Key.S:
					currentCoeff = slanted;
					Gl.glTexGenfv(Gl.GL_S, currentPlane, currentCoeff);
					break;
				case Key.X:
					currentCoeff = xequalzero;
					Gl.glTexGenfv(Gl.GL_S, currentPlane, currentCoeff);
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