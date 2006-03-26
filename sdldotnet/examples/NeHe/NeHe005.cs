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
	/// Lesson 05: 3D Shapes
	/// </summary>
	public class NeHe005 : NeHe001
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 05: 3D Shapes";
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
			// Red
			Gl.glColor3f(1, 0, 0); 
			// Top Of Triangle (Front)
			Gl.glVertex3f(0, 1, 0);
			// Green
			Gl.glColor3f(0, 1, 0); 
			// Left Of Triangle (Front)
			Gl.glVertex3f(-1, -1, 1);
			// Blue
			Gl.glColor3f(0, 0, 1); 
			// Right Of Triangle (Front)
			Gl.glVertex3f(1, -1, 1);
			// Red
			Gl.glColor3f(1, 0, 0); 
			// Top Of Triangle (Right)
			Gl.glVertex3f(0, 1, 0);
			// Blue
			Gl.glColor3f(0, 0, 1); 
			// Left Of Triangle (Right)
			Gl.glVertex3f(1, -1, 1);
			// Green
			Gl.glColor3f(0, 1, 0); 
			// Right Of Triangle (Right)
			Gl.glVertex3f(1, -1, -1);
			// Red
			Gl.glColor3f(1, 0, 0); 
			// Top Of Triangle (Back)
			Gl.glVertex3f(0, 1, 0);
			// Green
			Gl.glColor3f(0, 1, 0); 
			// Left Of Triangle (Back)
			Gl.glVertex3f(1, -1, -1);
			// Blue
			Gl.glColor3f(0, 0, 1); 
			// Right Of Triangle (Back)
			Gl.glVertex3f(-1, -1, -1);   
			// Red
			Gl.glColor3f(1, 0, 0); 
			// Top Of Triangle (Left)
			Gl.glVertex3f(0, 1, 0);
			// Blue
			Gl.glColor3f(0, 0, 1); 
			// Left Of Triangle (Left)
			Gl.glVertex3f(-1, -1, -1);   
			// Green
			Gl.glColor3f(0, 1, 0); 
			// Right Of Triangle (Left)
			Gl.glVertex3f(-1, -1, 1);
			// Finished Drawing The Triangle
			Gl.glEnd();
			// Reset The Current Modelview Matrix
			Gl.glLoadIdentity();   
			// Move Right 1.5 Units And Into The Screen 7.0
			Gl.glTranslatef(1.5f, 0, -7);
			// Rotate The Quad On The X, Y, and Z Axis ( NEW )
			Gl.glRotatef(rquad, 1, 1, 1);
			// Set The Color To Blue One Time Only
			Gl.glColor3f(0.5f, 0.5f, 1);
			// Draw A Quad
			Gl.glBegin(Gl.GL_QUADS);   
			// Set The Color To Green
			Gl.glColor3f(0, 1, 0); 
			// Top Right Of The Quad (Top)
			Gl.glVertex3f(1, 1, -1);
			// Top Left Of The Quad (Top)
			Gl.glVertex3f(-1, 1, -1);
			// Bottom Left Of The Quad (Top)
			Gl.glVertex3f(-1, 1, 1);
			// Bottom Right Of The Quad (Top)
			Gl.glVertex3f(1, 1, 1);
			// Set The Color To Orange
			Gl.glColor3f(1, 0.5f, 0);
			// Top Right Of The Quad (Bottom)
			Gl.glVertex3f(1, -1, 1);
			// Top Left Of The Quad (Bottom)
			Gl.glVertex3f(-1, -1, 1);
			// Bottom Left Of The Quad (Bottom)
			Gl.glVertex3f(-1, -1, -1);   
			// Bottom Right Of The Quad (Bottom)
			Gl.glVertex3f(1, -1, -1);
			// Set The Color To Red
			Gl.glColor3f(1, 0, 0); 
			// Top Right Of The Quad (Front)
			Gl.glVertex3f(1, 1, 1);
			// Top Left Of The Quad (Front)
			Gl.glVertex3f(-1, 1, 1);
			// Bottom Left Of The Quad (Front)
			Gl.glVertex3f(-1, -1, 1);
			// Bottom Right Of The Quad (Front)
			Gl.glVertex3f(1, -1, 1);
			// Set The Color To Yellow
			Gl.glColor3f(1, 1, 0); 
			// Top Right Of The Quad (Back)
			Gl.glVertex3f(1, -1, -1);
			// Top Left Of The Quad (Back)
			Gl.glVertex3f(-1, -1, -1);   
			// Bottom Left Of The Quad (Back)
			Gl.glVertex3f(-1, 1, -1);
			// Bottom Right Of The Quad (Back)
			Gl.glVertex3f(1, 1, -1);
			// Set The Color To Blue
			Gl.glColor3f(0, 0, 1); 
			// Top Right Of The Quad (Left)
			Gl.glVertex3f(-1, 1, 1);
			// Top Left Of The Quad (Left)
			Gl.glVertex3f(-1, 1, -1);
			// Bottom Left Of The Quad (Left)
			Gl.glVertex3f(-1, -1, -1);   
			// Bottom Right Of The Quad (Left)
			Gl.glVertex3f(-1, -1, 1);
			
			// Set The Color To Violet
			Gl.glColor3f(1, 0, 1); 
			// Top Right Of The Quad (Right)
			Gl.glVertex3f(1, 1, -1);
			// Top Left Of The Quad (Right)
			Gl.glVertex3f(1, 1, 1);
			// Bottom Left Of The Quad (Right)
			Gl.glVertex3f(1, -1, 1);
			// Bottom Right Of The Quad (Right)
			Gl.glVertex3f(1, -1, -1);
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