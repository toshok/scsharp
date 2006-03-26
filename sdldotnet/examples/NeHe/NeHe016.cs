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

using SdlDotNet;
using Tao.OpenGl;

namespace SdlDotNet.Examples.NeHe
{
	/// <summary>
	/// Lesson 16: Cool Looking Fog
	/// </summary>
	public class NeHe016 : NeHe010
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 16: Cool Looking Fog";
			}
		}

		// Storage For Three Types Of Fog
		int[] fogMode = {Gl.GL_EXP, Gl.GL_EXP2, Gl.GL_LINEAR};	
		// Which Fog Mode To Use 
		int fogfilter;	
		// Fog Color
		float[] fogColor = {0.5f, 0.5f, 0.5f, 1.0f};	
		
		#endregion Fields

		#region Constructor

		/// <summary>
		/// 
		/// </summary>
		public NeHe016()
		{
			this.DepthZ = -5.0f;
			// Depth Into The Screen
			this.Texture = new int[3];
			this.TextureName = new string[1];
			this.TextureName[0] = "NeHe016.bmp";
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
		}

		#endregion Constructor

		#region Lesson Setup

		/// <summary>
		/// Initialize OpenGL
		/// </summary>
		protected override void InitGL()
		{
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			Keyboard.EnableKeyRepeat(30,30);
			LoadGLFilteredTextures();

			// Enable Texture Mapping
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			// Enable Smooth Shading
			Gl.glShadeModel(Gl.GL_SMOOTH);
			// Black Background
			Gl.glClearColor(0.5f, 0.5f, 0.5f, 1.0f);
			// Depth Buffer Setup
			Gl.glClearDepth(1.0f);		
			// Enables Depth Testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			// The Type Of Depth Testing To Do
			Gl.glDepthFunc(Gl.GL_LEQUAL);
			// Really Nice Perspective Calculations					
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);		

			// Setup The Ambient Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT,  this.LightAmbient);	
			// Setup The Diffuse Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE,  this.LightDiffuse);	
			// Position The Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, this.LightPosition);	
			// Enable Light One
			Gl.glEnable(Gl.GL_LIGHT1);	

			// Fog Mode
			Gl.glFogi(Gl.GL_FOG_MODE, (int)this.fogMode[this.fogfilter]);	
			// Set Fog Color
			Gl.glFogfv(Gl.GL_FOG_COLOR, this.fogColor);
			// How Dense Will The Fog Be
			Gl.glFogf(Gl.GL_FOG_DENSITY, 0.35f);	
			// Fog Hint Value
			Gl.glHint(Gl.GL_FOG_HINT, Gl.GL_DONT_CARE);
			// Fog Start Depth
			Gl.glFogf(Gl.GL_FOG_START, 1.0f);		
			// Fog End Depth
			Gl.glFogf(Gl.GL_FOG_END, 5.0f);	
			// Enables GL_FOG					
			Gl.glEnable(Gl.GL_FOG);		
			
			if (this.Light)	
			{		
				// If lighting, enable it to start
				Gl.glEnable(Gl.GL_LIGHTING);
			}
		}
		
		#endregion Lesson Setup

		#region Render

		/// <summary>
		/// Renders the scene
		/// </summary>
		protected override void DrawGLScene()
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glLoadIdentity();
			Gl.glTranslatef(0.0f, 0.0f, this.DepthZ);

			Gl.glRotatef(this.RotationX, 1.0f, 0.0f, 0.0f);
			Gl.glRotatef(this.RotationY, 0.0f, 1.0f, 0.0f);

			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[this.Filter]);

			Gl.glBegin(Gl.GL_QUADS);
			// Front Face
			Gl.glNormal3f( 0.0f, 0.0f, 1.0f);
			Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3f(-1.0f, -1.0f,  1.0f);
			Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3f( 1.0f, -1.0f,  1.0f);
			Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3f( 1.0f,  1.0f,  1.0f);
			Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3f(-1.0f,  1.0f,  1.0f);
			// Back Face
			Gl.glNormal3f( 0.0f, 0.0f,-1.0f);
			Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3f(-1.0f, -1.0f, -1.0f);
			Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3f(-1.0f,  1.0f, -1.0f);
			Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3f( 1.0f,  1.0f, -1.0f);
			Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3f( 1.0f, -1.0f, -1.0f);
			// Top Face
			Gl.glNormal3f( 0.0f, 1.0f, 0.0f);
			Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3f(-1.0f,  1.0f, -1.0f);
			Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3f(-1.0f,  1.0f,  1.0f);
			Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3f( 1.0f,  1.0f,  1.0f);
			Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3f( 1.0f,  1.0f, -1.0f);
			// Bottom Face
			Gl.glNormal3f( 0.0f,-1.0f, 0.0f);
			Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3f(-1.0f, -1.0f, -1.0f);
			Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3f( 1.0f, -1.0f, -1.0f);
			Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3f( 1.0f, -1.0f,  1.0f);
			Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3f(-1.0f, -1.0f,  1.0f);
			// Right face
			Gl.glNormal3f( 1.0f, 0.0f, 0.0f);
			Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3f( 1.0f, -1.0f, -1.0f);
			Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3f( 1.0f,  1.0f, -1.0f);
			Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3f( 1.0f,  1.0f,  1.0f);
			Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3f( 1.0f, -1.0f,  1.0f);
			// Left Face
			Gl.glNormal3f(-1.0f, 0.0f, 0.0f);
			Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3f(-1.0f, -1.0f, -1.0f);
			Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3f(-1.0f, -1.0f,  1.0f);
			Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3f(-1.0f,  1.0f,  1.0f);
			Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3f(-1.0f,  1.0f, -1.0f);
			Gl.glEnd();

			this.RotationX += this.XSpeed;
			this.RotationY += this.YSpeed;
		}

		#endregion Render

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch(e.Key)
			{
				// L, F and G.
				case Key.L:
					this.Light = !this.Light;
					if (this.Light)
					{
						Gl.glEnable(Gl.GL_LIGHTING);
					}
					else
					{
						Gl.glDisable(Gl.GL_LIGHTING);
					}
					break;
				case Key.F:
					this.Filter += 1;
					if(this.Filter > 2) 
					{
						this.Filter = 0;
					}
					break;
				case Key.G:
					this.fogfilter += 1;
					if(this.fogfilter > 2) 
					{
						this.fogfilter = 0;
					}
					Gl.glFogi(Gl.GL_FOG_MODE, (int)this.fogMode[this.fogfilter]);	
					break;

					// Zoom in cube with Page Up/Down
				case Key.PageUp:
					this.DepthZ -= 0.02f;
					break;
				case Key.PageDown:
					this.DepthZ += 0.02f;
					break;

					// Rotate cube with arrows.
				case Key.UpArrow:
					this.XSpeed -= 0.1f;
					break;
				case Key.DownArrow:
					this.XSpeed += 0.1f;
					break;
				case Key.LeftArrow:
					this.YSpeed -= 0.1f;
					break;
				case Key.RightArrow:
					this.YSpeed += 0.1f;
					break;
			}
		}
				
		#endregion Event Handlers
	}
}
