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
	///     This program demonstrates polygon offset to draw a shaded polygon and its
	///     wireframe counterpart without ugly visual artifacts ("stitching").
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/planet.c
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
	public class RedBookPolyOff
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
				return "PolyOff - polygon offset";
			}
		}

		#region Private Fields
		private static int list;
		private static int spinX = 0;
		private static int spinY = 0;
		private static float distance = 0.0f;
		private static float polyFactor = 1.0f;
		private static float polyUnits = 1.0f;
		#endregion Private Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookPolyOff()
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
			Events.MouseButtonDown += new MouseButtonEventHandler(this.MouseButtonDown);
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
			
			float[] lightAmbient = {0.0f, 0.0f, 0.0f, 1.0f};
			float[] lightDiffuse = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] lightSpecular = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] lightPosition = {1.0f, 1.0f, 1.0f, 0.0f};
			float[] globalAmbient = {0.2f, 0.2f, 0.2f, 1.0f};

			Gl.glClearColor(0.0f, 0.0f, 0.0f, 1.0f);

			list = Gl.glGenLists(1);
			Gl.glNewList(list, Gl.GL_COMPILE);
			Glut.glutSolidSphere(1.0, 20, 12);
			Gl.glEndList();

			Gl.glEnable(Gl.GL_DEPTH_TEST);

			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, lightAmbient);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, lightDiffuse);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, lightSpecular);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPosition);
			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, globalAmbient);
		}
		#endregion Init()

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			//float[] materialAmbient = {0.8f, 0.8f, 0.8f, 1.0f};
			//float[] materialDiffuse = {1.0f, 0.0f, 0.5f, 1.0f};
			//float[] materialSpecular = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] gray = {0.8f, 0.8f, 0.8f, 1.0f};
			float[] black = {0.0f, 0.0f, 0.0f, 1.0f};

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glPushMatrix();
			Gl.glTranslatef(0.0f, 0.0f, distance);
			Gl.glRotatef((float) spinX, 1.0f, 0.0f, 0.0f);
			Gl.glRotatef((float) spinY, 0.0f, 1.0f, 0.0f);

			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE, gray);
			Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, black);
			Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 0.0f);
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL);
			Gl.glPolygonOffset(polyFactor, polyUnits);
			Gl.glCallList(list);
			Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL);

			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glDisable(Gl.GL_LIGHT0);
			Gl.glColor3f(1.0f, 1.0f, 1.0f);
			Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
			Gl.glCallList(list);
			Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
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
			Glu.gluPerspective(45.0, (float) w / (float) h, 1.0, 10.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Glu.gluLookAt(0.0, 0.0, 5.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
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
				case Key.T:
					if(distance < 4.0f) 
					{
						distance = (distance + 0.5f);
					}
					break;
				case Key.R:
					if(distance > -5.0f) 
					{
						distance = (distance - 0.5f);
					}
					break;
				case Key.F:
					polyFactor = polyFactor + 0.1f;
					Console.WriteLine("polyFactor is {0}", polyFactor);
					break;
				case Key.D:
					polyFactor = polyFactor - 0.1f;
					Console.WriteLine("polyFactor is {0}", polyFactor);
					break;
				case Key.U:
					polyUnits = polyUnits + 1.0f;
					Console.WriteLine("polyUnits is {0}", polyUnits);
					break;
				case Key.Y:
					polyUnits = polyUnits - 1.0f;
					Console.WriteLine("polyUnits is {0}", polyUnits);
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

		private void MouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			switch(e.Button) 
			{
				case MouseButton.PrimaryButton:
					spinX = (spinX + 5) % 360; 
					break;
				case MouseButton.SecondaryButton:
					spinY = (spinY + 5) % 360; 
					break;
				default:
					break;
			}
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
