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

using Tao.OpenGl;

namespace SdlDotNet.Examples.NeHe
{
	/// <summary>
	/// Lesson 04: Rotation
	/// </summary>
	public class NeHe004 : NeHe001
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 04: Rotation";
			}
		}

		// Angle For The Triangle ( NEW )
		float rtri;  
		// Angle For The Quad ( NEW ) 
		float rquad;

		#endregion Fields

		#region Lesson Setup

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		protected override void InitGL()
		{
			// Enable Smooth Shading
			Gl.glShadeModel(Gl.GL_SMOOTH);
			// Black Background
			Gl.glClearColor(0.0F, 0.0F, 0.0F, 0.5F);
			// Depth Buffer Setup
			Gl.glClearDepth(1.0F);
			// Enables Depth Testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			// The Type Of Depth Testing To Do
			Gl.glDepthFunc(Gl.GL_LEQUAL);
			// Really Nice Perspective Calculations
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
		}

		#endregion Lesson Setup

		#region void DrawGLScene

		/// <summary>
		/// Renders the scene
		/// </summary>
		protected override void DrawGLScene()
		{
			// Clear Screen And Depth Buffer
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			// Reset The Current Modelview Matrix
			Gl.glLoadIdentity();   
			// Move Left 1.5 Units And Into The Screen 6.0
			Gl.glTranslatef(-1.5f, 0, -6);   
			// Rotate The Triangle On The Y axis ( NEW )
			Gl.glRotatef(rtri, 0, 1, 0);
			// Drawing Using Triangles
			Gl.glBegin(Gl.GL_TRIANGLES);
			// Set The Color To Red
			Gl.glColor3f(1, 0, 0); 
			// Top
			Gl.glVertex3f(0, 1, 0);
			// Set The Color To Green
			Gl.glColor3f(0, 1, 0); 
			// Bottom Left
			Gl.glVertex3f(-1, -1, 0);
			// Set The Color To Blue
			Gl.glColor3f(0, 0, 1); 
			// Bottom Right
			Gl.glVertex3f(1, -1, 0);
			// Finished Drawing The Triangle
			Gl.glEnd();
			// Reset The Current Modelview Matrix
			Gl.glLoadIdentity();   
			// Move Right 1.5 Units And Into The Screen 6.0
			Gl.glTranslatef(1.5f, 0, -6);
			// Rotate The Quad On The X axis ( NEW )
			Gl.glRotatef(rquad, 1, 0, 0);
			// Set The Color To Blue One Time Only
			Gl.glColor3f(0.5f, 0.5f, 1);
			// Draw A Quad
			Gl.glBegin(Gl.GL_QUADS);   
			// Top Left
			Gl.glVertex3f(-1, 1, 0);
			// Top Right
			Gl.glVertex3f(1, 1, 0);
			// Bottom Right
			Gl.glVertex3f(1, -1, 0);
			// Bottom Left
			Gl.glVertex3f(-1, -1, 0);
			// Done Drawing The Quad
			Gl.glEnd();
			// Increase The Rotation Variable For The Triangle ( NEW )
			rtri += 0.2f;  
			// Decrease The Rotation Variable For The Quad ( NEW )
			rquad -= 0.15f;
		}

		#endregion void DrawGLScene
	}
}