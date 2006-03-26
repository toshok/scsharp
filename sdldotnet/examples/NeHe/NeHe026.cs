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
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

using SdlDotNet;
using Tao.OpenGl;

namespace SdlDotNet.Examples.NeHe
{
	/// <summary>
	/// Lesson 26: Clipping &amp; Reflections Using The Stencil Buffer
	/// </summary>
	public class NeHe026 : NeHe025
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 26: Clipping & Reflections Using The Stencil Buffer";
			}
		}
		
		float ballHeight;
		
		#endregion Fields

		#region Constructor

		/// <summary>
		/// Basic constructor
		/// </summary>
		public NeHe026()
		{
			this.ballHeight = 2;
			this.DepthZ = -7;
			this.Texture = new int[3];
			this.TextureName = new string[3];
			this.TextureName[0] = "NeHe026.EnvWall.bmp";
			this.TextureName[1] = "NeHe026.Ball.bmp";
			this.TextureName[2] = "NeHe026.EnvRoll.bmp";
			this.LightAmbient[0] = 0.7f;
			this.LightAmbient[1] = 0.7f;
			this.LightAmbient[2] = 0.7f;
			this.LightAmbient[3] = 1.0f;
			this.LightDiffuse[0] = 1.0f;
			this.LightDiffuse[1] = 1.0f;
			this.LightDiffuse[2] = 1.0f;
			this.LightDiffuse[3] = 1.0f;
			this.LightPosition[0] = 4.0f;
			this.LightPosition[1] = 4.0f;
			this.LightPosition[2] = 6.0f;
			this.LightPosition[3] = 1.0f;
		}
		
		#endregion Constructor

		#region Lesson Setup

		#region void InitGL()
		/// <summary>
		/// All setup for OpenGL goes here.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> on success, otherwise <c>false</c>.
		/// </returns>
		protected override void InitGL() 
		{
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			Keyboard.EnableKeyRepeat(150,50);

			// All Setup For OpenGL Goes Here
			LoadGLTextures();
			// Enable Smooth Shading
			Gl.glShadeModel(Gl.GL_SMOOTH);
			// Background   
			Gl.glClearColor(0.2f, 0.5f, 1, 1); 
			// Depth Buffer Setup
			Gl.glClearDepth(1);
			// Clear The Stencil Buffer To 0
			Gl.glClearStencil(0);  
			// Enables Depth Testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);   
			// The Type Of Depth Testing To Do
			Gl.glDepthFunc(Gl.GL_LEQUAL);
			// Really Nice Perspective Calculations
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST); 
			// Enable 2D Texture Mapping
			Gl.glEnable(Gl.GL_TEXTURE_2D);   
			// Set The Ambient Lighting For Light0
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, this.LightAmbient);
			// Set The Diffuse Lighting For Light0
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, this.LightDiffuse);
			// Set The Position For Light0
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, this.LightPosition);   
			// Enable Light 0
			Gl.glEnable(Gl.GL_LIGHT0);
			// Enable Lighting
			Gl.glEnable(Gl.GL_LIGHTING);
			// Create A New Quadratic
			this.Quadratic = Glu.gluNewQuadric();
			// Generate Smooth Normals For The Quad
			Glu.gluQuadricNormals(this.Quadratic, Gl.GL_SMOOTH);
			// Enable Texture Coords For The Quad
			Glu.gluQuadricTexture(this.Quadratic, Gl.GL_TRUE);
			// Set Up Sphere Mapping
			Gl.glTexGeni(Gl.GL_S, Gl.GL_TEXTURE_GEN_MODE, Gl.GL_SPHERE_MAP);
			// Set Up Sphere Mapping
			Gl.glTexGeni(Gl.GL_T, Gl.GL_TEXTURE_GEN_MODE, Gl.GL_SPHERE_MAP);

		}
		#endregion void InitGL()

		#endregion Lesson Setup

		#region Render

		#region void DrawGLScene()
		/// <summary>
		/// Renders the scene
		/// </summary>
		protected override void DrawGLScene() 
		{
			// Clear Screen, Depth Buffer & Stencil Buffer
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT | Gl.GL_STENCIL_BUFFER_BIT);

			// Clip Plane Equations
			// Plane Equation To Use For The Reflected Objects
			double[] eqr = {0,-1, 0, 0};

			// Reset The Modelview Matrix
			Gl.glLoadIdentity();
			// Zoom And Raise Camera Above The Floor (Up 0.6 Units)
			Gl.glTranslatef(0, -0.6f, this.DepthZ);
			// Set Color Mask
			Gl.glColorMask(0, 0, 0, 0);
			// Enable Stencil Buffer For "marking" The Floor
			Gl.glEnable(Gl.GL_STENCIL_TEST);
			// Always Passes, 1 Bit Plane, 1 As Mask
			Gl.glStencilFunc(Gl.GL_ALWAYS, 1, 1);
			// We Set The Stencil Buffer To 1 Where We Draw Any Polygon
			// Keep If Test Fails, Keep If Test Passes But Buffer Test Fails
			// Replace If Test Passes
			Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_KEEP, Gl.GL_REPLACE);
			// Disable Depth Testing
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			// Draw The Floor (Draws To The Stencil Buffer)
			// We Only Want To Mark It In The Stencil Buffer
			DrawFloor();
			// Enable Depth Testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			// Set Color Mask to TRUE, TRUE, TRUE, TRUE
			Gl.glColorMask(1, 1, 1, 1);
			// We Draw Only Where The Stencil Is 1
			// (I.E. Where The Floor Was Drawn)
			Gl.glStencilFunc(Gl.GL_EQUAL, 1, 1);
			// Don't Change The Stencil Buffer
			Gl.glStencilOp(Gl.GL_KEEP, Gl.GL_KEEP, Gl.GL_KEEP);
			// Enable Clip Plane For Removing Artifacts
			// (When The Object Crosses The Floor)
			Gl.glEnable(Gl.GL_CLIP_PLANE0);
			// Equation For Reflected Objects
			Gl.glClipPlane(Gl.GL_CLIP_PLANE0, eqr);
			// Push The Matrix Onto The Stack
			Gl.glPushMatrix();
			// Mirror Y Axis
			Gl.glScalef(1, -1, 1);
			// Set Up Light0
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, this.LightPosition);
			// Position The Object
			Gl.glTranslatef(0, this.Height, 0);
			// Rotate Local Coordinate System On X Axis
			Gl.glRotatef(this.RotationX, 1, 0, 0);
			// Rotate Local Coordinate System On Y Axis
			Gl.glRotatef(this.RotationY, 0, 1, 0);
			// Draw The Sphere (Reflection)
			DrawObject();
			// Pop The Matrix Off The Stack
			Gl.glPopMatrix();
			// Disable Clip Plane For Drawing The Floor
			Gl.glDisable(Gl.GL_CLIP_PLANE0);
			// We Don't Need The Stencil Buffer Any More (Disable)
			Gl.glDisable(Gl.GL_STENCIL_TEST);
			// Set Up Light0 Position
			Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, this.LightPosition);
			// Enable Blending (Otherwise The Reflected Object Wont Show)
			Gl.glEnable(Gl.GL_BLEND);
			// Since We Use Blending, We Disable Lighting
			Gl.glDisable(Gl.GL_LIGHTING);
			// Set Color To White With 80% Alpha
			Gl.glColor4f(1, 1, 1, 0.8f);
			// Blending Based On Source Alpha And 1 Minus Dest Alpha
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
			// Draw The Floor To The Screen
			DrawFloor();
			// Enable Lighting
			Gl.glEnable(Gl.GL_LIGHTING);
			// Disable Blending
			Gl.glDisable(Gl.GL_BLEND);
			// Position The Ball At Proper Height
			Gl.glTranslatef(0, this.ballHeight, 0); 
			// Rotate On The X Axis
			Gl.glRotatef(this.RotationX, 1, 0, 0);
			// Rotate On The Y Axis
			Gl.glRotatef(this.RotationY, 0, 1, 0);
			// Draw The Ball
			DrawObject();
			// Update X Rotation Angle By xrotspeed
			this.RotationX += this.XSpeed; 
			// Update Y Rotation Angle By yrotspeed
			this.RotationY += this.YSpeed;
			// Flush The GL Pipeline
			Gl.glFlush();
		}
		#endregion void DrawGLScene()

		#region DrawFloor()
		/// <summary>
		/// Draws the floor.
		/// </summary>
		private void DrawFloor() 
		{
			// Select Texture 1 (0)
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[0]);
			// Begin Drawing A Quad
			Gl.glBegin(Gl.GL_QUADS);
			// Normal Pointing Up
			Gl.glNormal3f(0, 1, 0);
			// Bottom Left Of Texture
			Gl.glTexCoord2f(0, 1);
			// Bottom Left Corner Of Floor
			Gl.glVertex3f(-2, 0, 2);
			// Top Left Of Texture
			Gl.glTexCoord2f(0, 0);
			// Top Left Corner Of Floor
			Gl.glVertex3f(-2, 0, -2);
			// Top Right Of Texture
			Gl.glTexCoord2f(1, 0);
			// Top Right Corner Of Floor
			Gl.glVertex3f(2, 0,-2);
			// Bottom Right Of Texture
			Gl.glTexCoord2f(1, 1);
			// Bottom Right Corner Of Floor
			Gl.glVertex3f(2, 0, 2);
			// Done Drawing The Quad
			Gl.glEnd();
		}
		#endregion DrawFloor()

		#region DrawObject()
		/// <summary>
		/// Draws our ball.
		/// </summary>
		private void DrawObject() 
		{
			// Set Color To White
			Gl.glColor3f(1, 1, 1); 
			// Select Texture 2 (1)
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[1]); 
			// Draw First Sphere
			Glu.gluSphere(this.Quadratic, 0.35f, 32, 16); 
			// Select Texture 3 (2)
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[2]); 
			// Set Color To White With 40% Alpha
			Gl.glColor4f(1, 1, 1, 0.4f);
			// Enable Blending
			Gl.glEnable(Gl.GL_BLEND);  
			// Set Blending Mode To Mix Based On SRC Alpha
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE); 
			// Enable Sphere Mapping
			Gl.glEnable(Gl.GL_TEXTURE_GEN_S);
			// Enable Sphere Mapping
			Gl.glEnable(Gl.GL_TEXTURE_GEN_T);
			// Draw Another Sphere Using New Texture
			Glu.gluSphere(this.Quadratic, 0.35f, 32, 16); 
			// Textures Will Mix Creating A MultiTexture Effect (Reflection)
			// Disable Sphere Mapping
			Gl.glDisable(Gl.GL_TEXTURE_GEN_S);
			// Disable Sphere Mapping
			Gl.glDisable(Gl.GL_TEXTURE_GEN_T);   
			// Disable Blending
			Gl.glDisable(Gl.GL_BLEND); 
		}
		#endregion DrawObject()

		#endregion Render

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.PageUp:
					this.ballHeight += 0.03f;
					break;
				case Key.PageDown:
					this.ballHeight -= 0.03f;
					break;
				case Key.UpArrow: 
					this.XSpeed -= 0.08f;
					break;
				case Key.DownArrow:
					this.XSpeed += 0.08f;
					break;
				case Key.RightArrow:
					this.YSpeed += 0.08f;
					break;
				case Key.LeftArrow:
					this.YSpeed -= 0.08f;
					break;
				case Key.A:
					this.DepthZ += 0.05f;
					break;
				case Key.Z:
					this.DepthZ -= 0.05f;
					break;				
			}
		}

		#endregion Event Handlers
	}
}