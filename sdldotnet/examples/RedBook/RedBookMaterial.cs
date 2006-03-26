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
	///     This program demonstrates the use of the GL lighting model.  Several objects are
	///     drawn using different material characteristics.  A single light source
	///     illuminates the objects.
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
	public class RedBookMaterial
	{
		#region Fields

		//Width of screen
		int width = 650;
		//Height of screen
		int height = 450;
		
		

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Material - Material Characteristics";
			}
		}

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookMaterial()
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
		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			if(w <= h * 2) 
			{
				Gl.glOrtho(-6.0, 6.0, -3.0 * ((float) h * 2) / (float) w, 3.0 * ((float) h * 2) / (float) w, -10.0, 10.0);
			}
			else 
			{
				Gl.glOrtho(-6.0 * (float) w / ((float) h * 2), 6.0 * (float) w / ((float) h * 2), -3.0, 3.0, -10.0, 10.0);
			}
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();

		}
		#endregion Reshape(int w, int h)

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		private static void Init()
		{
			
			float[] ambient = {0.0f, 0.0f, 0.0f, 1.0f};
			float[] diffuse = {1.0f, 1.0f, 1.0f, 1.0f};
			//float[] specular = {1.0f, 1.0f, 1.0f, 1.0f};
			float[] position = {0.0f, 3.0f, 2.0f, 0.0f};
			float[] modelAmbient = {0.4f, 0.4f, 0.4f, 1.0f};
			float[] localView = {0.0f};

			Gl.glClearColor(0.0f, 0.1f, 0.1f, 0.0f);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glShadeModel(Gl.GL_SMOOTH);

			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambient);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, diffuse);
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position);
			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, modelAmbient);
			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_LOCAL_VIEWER, localView);

			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
		}

		#endregion Lesson Setup

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
		float[] materialNone = {0.0f, 0.0f, 0.0f, 1.0f};
		float[] materialAmbient = {0.7f, 0.7f, 0.7f, 1.0f};
		float[] materialAmbientColor = {0.8f, 0.8f, 0.2f, 1.0f};
		float[] materialDiffuse = {0.1f, 0.5f, 0.8f, 1.0f};
		float[] materialSpecular = {1.0f, 1.0f, 1.0f, 1.0f};
		float[] materialEmission = {0.3f, 0.2f, 0.2f, 0.0f};
		float[] shininessNone = {0.0f};
		float[] shininessLow = {5.0f};
		float[] shininessHigh = {100.0f};

		Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

		// draw sphere in first row, first column diffuse reflection only; no ambient or
		// specular
		Gl.glPushMatrix();
		Gl.glTranslatef(-3.75f, 3.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialNone);
		Glut.glutSolidSphere(1.0f, 16, 16);
		Gl.glPopMatrix();

		// draw sphere in first row, second column diffuse and specular reflection; low
		// shininess; no ambient
		Gl.glPushMatrix();
		Gl.glTranslatef(-1.25f, 3.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessLow);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialNone);
		Glut.glutSolidSphere(1.0f, 16, 16);
		Gl.glPopMatrix();

		// draw sphere in first row, third column diffuse and specular reflection; high
		// shininess; no ambient
		Gl.glPushMatrix();
		Gl.glTranslatef(1.25f, 3.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessHigh);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialNone);
		Glut.glutSolidSphere(1.0f, 16, 16);
		Gl.glPopMatrix();

		// draw sphere in first row, fourth column diffuse reflection; emission; no ambient
		// or specular reflection
		Gl.glPushMatrix();
		Gl.glTranslatef(3.75f, 3.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialEmission);
		Glut.glutSolidSphere(1.0f, 16, 16);
		Gl.glPopMatrix();

		// draw sphere in second row, first column ambient and diffuse reflection; no
		// specular
		Gl.glPushMatrix();
		Gl.glTranslatef(-3.75f, 0.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbient);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialNone);
		Glut.glutSolidSphere(1.0f, 16, 16);
		Gl.glPopMatrix();

		// draw sphere in second row, second column ambient, diffuse and specular reflection;
		// low shininess
		Gl.glPushMatrix();
		Gl.glTranslatef(-1.25f, 0.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbient);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessLow);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialNone);
		Glut.glutSolidSphere(1.0f, 16, 16);
		Gl.glPopMatrix();

		// draw sphere in second row, third column ambient, diffuse and specular reflection;
		// high shininess
		Gl.glPushMatrix();
		Gl.glTranslatef(1.25f, 0.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbient);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessHigh);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialNone);
		Glut.glutSolidSphere(1.0f, 16, 16);
		Gl.glPopMatrix();

		// draw sphere in second row, fourth column ambient and diffuse reflection; emission;
		// no specular
		Gl.glPushMatrix();
		Gl.glTranslatef(3.75f, 0.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbient);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialEmission);
		Glut.glutSolidSphere(1.0f, 16, 16);
		Gl.glPopMatrix();

		// draw sphere in third row, first column colored ambient and diffuse reflection; no
		// specular
		Gl.glPushMatrix();
		Gl.glTranslatef(-3.75f, -3.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbientColor);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialNone);
		Glut.glutSolidSphere(1.0f, 16, 16);
		Gl.glPopMatrix();

		// draw sphere in third row, second column colored ambient, diffuse and specular
		// reflection; low shininess
		Gl.glPushMatrix();
		Gl.glTranslatef(-1.25f, -3.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbientColor);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessLow);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialNone);
		Glut.glutSolidSphere(1.0f, 16, 16);
		Gl.glPopMatrix();

		// draw sphere in third row, third column colored ambient, diffuse and specular
		// reflection; high shininess
		Gl.glPushMatrix();
		Gl.glTranslatef(1.25f, -3.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbientColor);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessHigh);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialNone);
		Glut.glutSolidSphere(1.0f, 16, 16);
		Gl.glPopMatrix();

		// draw sphere in third row, fourth column colored ambient and diffuse reflection;
		// emission; no specular
		Gl.glPushMatrix();
		Gl.glTranslatef(3.75f, -3.0f, 0.0f);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, materialAmbientColor);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, materialDiffuse);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, materialNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, shininessNone);
		Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, materialEmission);
		Glut.glutSolidSphere(1.0f, 16, 16);
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
