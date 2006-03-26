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
	///     This program demonstrates how to intermix opaque and alpha blended polygons in
	///     the same scene, by using glDepthMask.  Press the 'a' key to animate moving the
	///     transparent object through the opaque object.  Press the 'r' key to reset the
	///     scene.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/accanti.c
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
	public class RedBookAlpha3D
	{
		#region Fields

		//Width of screen
		int width = 500;
		//Height of screen
		int height = 500;
		
		
		
		
		private const float MAXZ = 8.0f;
		private const float MINZ = -8.0f;
		private const float ZINC = 0.4f;
		private static float solidZ = MAXZ;
		private static float transparentZ = MINZ;
		private static int sphereList, cubeList;

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Alpha3D - Alpha Blending for 3D";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookAlpha3D()
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
		/// <param name="h"></param>
		/// <param name="w"></param>
		private static void Reshape(int w, int h)
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			if(w <= h) 
			{
				Gl.glOrtho(-1.5, 1.5, -1.5 * h / (float)w, 1.5 * h / (float)w, -10.0, 10.0);
			}
			else 
			{
				Gl.glOrtho(-1.5 * w / (float)h, 1.5 * w / (float)h, -1.5, 1.5, -10.0, 10.0);
			}
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
		}

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		private static void Init()
		{
			
			float[] materialSpecular = {1.0f, 1.0f, 1.0f, 0.15f};
			float[] materialShininess = {100.0f};
			float[] position = {0.5f, 0.5f, 1.0f, 0.0f};

			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, materialShininess);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position);

			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_DEPTH_TEST);

			sphereList = Gl.glGenLists(1);
			Gl.glNewList(sphereList, Gl.GL_COMPILE);
			Glut.glutSolidSphere(0.4f, 16, 16);
			Gl.glEndList();

			cubeList = Gl.glGenLists(1);
			Gl.glNewList(cubeList, Gl.GL_COMPILE);
			Glut.glutSolidCube(0.6f);
			Gl.glEndList();
		}

		#endregion Lesson Setup

		#region void Display
		/// <summary>
		/// Renders the scene
		/// </summary>
		private static void Display()
		{
			float[] materialSolid = {0.75f, 0.75f, 0.0f, 1.0f};
			float[] materialZero = {0.0f, 0.0f, 0.0f, 1.0f};
			float[] materialTransparent = {0.0f, 0.8f, 0.8f, 0.6f};
			float[] materialEmission = {0.0f, 0.3f, 0.3f, 0.6f};

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

			Gl.glPushMatrix();
			Gl.glTranslatef(-0.15f, -0.15f, solidZ);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialZero);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialSolid);
			Gl.glCallList(sphereList);
			Gl.glPopMatrix();

			Gl.glPushMatrix();
			Gl.glTranslatef(0.15f, 0.15f, transparentZ);
			Gl.glRotatef(15.0f, 1.0f, 1.0f, 0.0f);
			Gl.glRotatef(30.0f, 0.0f, 1.0f, 0.0f);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialEmission);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialTransparent);
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glDepthMask((byte) Gl.GL_FALSE);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE);
			Gl.glCallList(cubeList);
			Gl.glDepthMask((byte) Gl.GL_TRUE);
			Gl.glDisable(Gl.GL_BLEND);
			Gl.glPopMatrix();

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
				case Key.A:
					solidZ = MINZ;
					transparentZ = MAXZ;
					break;
				case Key.R:
					solidZ = MAXZ;
					transparentZ = MINZ;
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