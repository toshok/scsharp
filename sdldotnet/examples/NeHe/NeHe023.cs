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
using System.Drawing;
using System.Drawing.Imaging;

using SdlDotNet;
using Tao.OpenGl;

namespace SdlDotNet.Examples.NeHe
{
	/// <summary>
	/// Lesson 23: Sphere Mapping, Multi-Texturing and Extensions
	/// </summary>
	public class NeHe023 : NeHe020
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 23: Sphere Mapping, Multi-Texturing and Extensions";
			}
		}

		// Which Object To Draw
		int objectToDraw = 1; 
		
		#endregion Fields
		
		#region Constructor

		/// <summary>
		/// Basic constructor
		/// </summary>
		public NeHe023()
		{
			// Storage For 6 Textures
			this.Texture = new int[6];
			this.TextureName = new string[2];
			this.TextureName[0] = "NeHe023.BG.bmp";
			this.TextureName[1] = "NeHe023.Reflect.bmp";
			this.LightAmbient[0] = 0.5f;
			this.LightAmbient[1] = 0.5f;
			this.LightAmbient[2] = 0.5f;
			this.LightAmbient[3] = 1.0f;
			this.LightDiffuse[0] = 1.0f;
			this.LightDiffuse[1] = 1.0f;
			this.LightDiffuse[2] = 1.0f;
			this.LightDiffuse[3] = 1.0f;
			this.LightPosition[0] = 0.0f;
			this.LightPosition[1] = 0.0f;
			this.LightPosition[2] = 2.0f;
			this.LightPosition[3] = 1.0f;
			this.DepthZ = -10;
		}
		
		#endregion Constructor
		
		#region Lesson Setup

		/// <summary>
		/// Initialize OpenGL
		/// </summary>
		protected override void InitGL()
		{
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			Keyboard.EnableKeyRepeat(150,50);
			LoadGLFilteredTextures();

			// Enable Texture Mapping
			Gl.glEnable(Gl.GL_TEXTURE_2D);   
			// Enable Smooth Shading
			Gl.glShadeModel(Gl.GL_SMOOTH);   
			// Black Background
			Gl.glClearColor(0, 0, 0, 0.5f);  
			// Depth Buffer Setup
			Gl.glClearDepth(1);
			// Enables Depth Testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);   
			// The Type Of Depth Testing To Do
			Gl.glDepthFunc(Gl.GL_LEQUAL);
			// Really Nice Perspective Calculations
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST); 

			// Setup The Ambient Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, LightAmbient);
			// Setup The Diffuse Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, LightDiffuse);
			// Position The Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION,LightPosition);   
			// Enable Light One
			Gl.glEnable(Gl.GL_LIGHT1); 

			// Create A Pointer To The Quadric Object (Return 0 If No Memory)
			this.Quadratic = Glu.gluNewQuadric(); 
			// Create Smooth Normals 
			Glu.gluQuadricNormals(this.Quadratic, Glu.GLU_SMOOTH);   
			// Create Texture Coords 
			Glu.gluQuadricTexture(this.Quadratic, Gl.GL_TRUE);   

			// Set The Texture Generation Mode For S To Sphere Mapping (NEW)
			Gl.glTexGeni(Gl.GL_S, Gl.GL_TEXTURE_GEN_MODE, Gl.GL_SPHERE_MAP);
			// Set The Texture Generation Mode For T To Sphere Mapping (NEW)
			Gl.glTexGeni(Gl.GL_T, Gl.GL_TEXTURE_GEN_MODE, Gl.GL_SPHERE_MAP);
		}

		#endregion Lesson Setup

		#region Render

		/// <summary>
		/// Renders the scene
		/// </summary>
		protected override void DrawGLScene()
		{
			// Clear The Screen And The Depth Buffer
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			// Reset The View
			Gl.glLoadIdentity();   

			Gl.glTranslatef(0, 0, this.DepthZ);

			// Enable Texture Coord Generation For S (NEW)
			Gl.glEnable(Gl.GL_TEXTURE_GEN_S);
			// Enable Texture Coord Generation For T (NEW)
			Gl.glEnable(Gl.GL_TEXTURE_GEN_T);

			// This Will Select The Sphere Map
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[this.Filter + (this.Filter + 1)]); 
			Gl.glPushMatrix();
			Gl.glRotatef(this.RotationX, 1, 0, 0);
			Gl.glRotatef(this.RotationY, 0, 1, 0);
			switch(objectToDraw) 
			{
				case 0:
					GlDrawCube();
					break;
				case 1:
					// Center The Cylinder
					Gl.glTranslatef(0, 0, -1.5f);
					// A Cylinder With A Radius Of 0.5 And A Height Of 2
					Glu.gluCylinder(this.Quadratic, 1, 1, 3, 32, 32);
					break;
				case 2:
					// Draw A Sphere With A Radius Of 1 And 16 Longitude And 16 Latitude Segments
					Glu.gluSphere(this.Quadratic, 1.3, 32, 32);  
					break;
				case 3:
					// Center The Cone
					Gl.glTranslatef(0, 0, -1.5f);
					// A Cone With A Bottom Radius Of .5 And A Height Of 2
					Glu.gluCylinder(this.Quadratic, 1, 0, 3, 32, 32);
					break;
			};
			Gl.glPopMatrix();
			Gl.glDisable(Gl.GL_TEXTURE_GEN_S);
			Gl.glDisable(Gl.GL_TEXTURE_GEN_T);

			// This Will Select The BG Maps...
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[this.Filter * 2]);
			Gl.glPushMatrix();
			Gl.glTranslatef(0, 0, -24);
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glNormal3f(0, 0, 1);
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-13.3f, -10, 10);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f( 13.3f, -10, 10);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f( 13.3f, 10, 10);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-13.3f, 10, 10);
			Gl.glEnd();
			Gl.glPopMatrix();

			this.RotationX += this.XSpeed;
			this.RotationY += this.YSpeed;
		}
				
		#endregion Render

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch(e.Key)
			{
				case Key.L:
					this.Light = !this.Light;
					if(this.Light)
					{
						Gl.glDisable(Gl.GL_LIGHTING);
					}
					else 
					{
						Gl.glEnable(Gl.GL_LIGHTING);
					}
					break;
				case Key.F:
					this.Filter += 1;
					if(this.Filter > 2) 
					{
						this.Filter = 0;
					}
					break;
				case Key.Space:
					if(++objectToDraw > 3) 
					{
						objectToDraw = 0;
					}
					break;
				case Key.PageUp:
					this.DepthZ -= 0.02f;
					break;
				case Key.PageDown:
					this.DepthZ += 0.02f;
					break;
				case Key.UpArrow:
					this.XSpeed -= 0.01f;
					break;
				case Key.DownArrow:
					this.XSpeed += 0.01f;
					break;
				case Key.RightArrow:
					this.YSpeed += 0.01f;
					break;
				case Key.LeftArrow: 
					this.YSpeed -= 0.01f;
					break;
			}
		}

		#endregion Event Handlers
	}
}
