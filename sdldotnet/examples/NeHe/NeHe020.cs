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
	/// Lesson 20: Masking
	/// </summary>
	public class NeHe020 : NeHe019
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 20: Masking";
			}
		}

		// Which Scene To Draw
		bool scene;				
		bool masking = true;
		// Rolling Texture
		float roll;				

		#endregion Fields

		#region Constructor

		/// <summary>
		/// Basic constructor
		/// </summary>
		public NeHe020()
		{
			this.Texture = new int[5];
			this.TextureName = new string[5];
			this.TextureName[0] = "NeHe020.Logo.bmp";
			this.TextureName[1] = "NeHe020.Mask1.bmp";
			this.TextureName[2] = "NeHe020.Image1.bmp";
			this.TextureName[3] = "NeHe020.Mask2.bmp";
			this.TextureName[4] = "NeHe020.Image2.bmp";
		}
		
		#endregion Constructor

		#region Lesson Setup

		/// <summary>
		/// Initialize OpenGL
		/// </summary>
		protected override void InitGL()
		{
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			Keyboard.EnableKeyRepeat(0,0);
			LoadGLTextures();

			// Enable Texture Mapping
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			// Enable Smooth Shading
			Gl.glShadeModel(Gl.GL_SMOOTH);
			// Black Background
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			// Depth Buffer Setup
			Gl.glClearDepth(1.0f);		
			Gl.glEnable(Gl.GL_DEPTH_TEST);
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
			
			// Move Into The Screen 5 Units
			Gl.glTranslatef(0.0f, 0.0f, -2.0f);

			// Select Our Logo Texture
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[0]);			
			// Start Drawing A Textured Quad
			Gl.glBegin(Gl.GL_QUADS);
			// Bottom Left
			Gl.glTexCoord2f(0.0f, -this.roll+0.0f); Gl.glVertex3f(-1.1f, -1.1f,  0.0f);	
			// Bottom Right
			Gl.glTexCoord2f(3.0f, -this.roll+0.0f); Gl.glVertex3f( 1.1f, -1.1f,  0.0f);	
			// Top Right
			Gl.glTexCoord2f(3.0f, -this.roll+3.0f); Gl.glVertex3f( 1.1f,  1.1f,  0.0f);	
			// Top Left
			Gl.glTexCoord2f(0.0f, -this.roll+3.0f); Gl.glVertex3f(-1.1f,  1.1f,  0.0f);	
			// Done Drawing The Quad
			Gl.glEnd();		

			// Enable Blending
			Gl.glEnable(Gl.GL_BLEND);
			// Disable Depth Testing
			Gl.glDisable(Gl.GL_DEPTH_TEST);	

			// Is Masking Enabled?
			if (this.masking)	
			{
				// Blend Screen Color With Zero (Black)
				Gl.glBlendFunc(Gl.GL_DST_COLOR, Gl.GL_ZERO);
			}
			
			// Are We Drawing The Second Scene?
			if (this.scene)		
			{
				// Translate Into The Screen One Unit
				Gl.glTranslatef(0.0f, 0.0f, -1.0f);					
				// Rotate On The Z Axis 360 Degrees.
				Gl.glRotatef(this.roll*360, 0.0f, 0.0f, 1.0f);				
				// Is Masking On?
				if (this.masking)
				{
					// Select The Second Mask Texture
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[3]);	
					// Start Drawing A Textured Quad
					Gl.glBegin(Gl.GL_QUADS);	
					// Bottom Left
					Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3f(-1.1f, -1.1f,  0.0f);	
					// Bottom Right
					Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3f( 1.1f, -1.1f,  0.0f);	
					// Top Right
					Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3f( 1.1f,  1.1f,  0.0f);	
					// Top Left
					Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3f(-1.1f,  1.1f,  0.0f);	
					// Done Drawing The Quad
					Gl.glEnd();
				}

				// Copy Image 2 Color To The Screen
				Gl.glBlendFunc(Gl.GL_ONE, Gl.GL_ONE);					
				// Select The Second Image Texture
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[4]);		
				// Start Drawing A Textured Quad
				Gl.glBegin(Gl.GL_QUADS);		
				// Bottom Left
				Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3f(-1.1f, -1.1f,  0.0f);	
				// Bottom Right
				Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3f( 1.1f, -1.1f,  0.0f);	
				// Top Right
				Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3f( 1.1f,  1.1f,  0.0f);	
				// Top Left
				Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3f(-1.1f,  1.1f,  0.0f);	
				// Done Drawing The Quad
				Gl.glEnd();	
			}
			else			
			{
				// Is Masking On?
				if (this.masking)
				{
					// Select The First Mask Texture
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[1]);	
					// Start Drawing A Textured Quad
					Gl.glBegin(Gl.GL_QUADS);	
					// Bottom Left
					Gl.glTexCoord2f(this.roll+0.0f, 0.0f); Gl.glVertex3f(-1.1f, -1.1f,  0.0f);	
					// Bottom Right
					Gl.glTexCoord2f(this.roll+4.0f, 0.0f); Gl.glVertex3f( 1.1f, -1.1f,  0.0f);	
					// Top Right
					Gl.glTexCoord2f(this.roll+4.0f, 4.0f); Gl.glVertex3f( 1.1f,  1.1f,  0.0f);	
					// Top Left
					Gl.glTexCoord2f(this.roll+0.0f, 4.0f); Gl.glVertex3f(-1.1f,  1.1f,  0.0f);	
					// Done Drawing The Quad
					Gl.glEnd();
				}

				// Copy Image 1 Color To The Screen
				Gl.glBlendFunc(Gl.GL_ONE, Gl.GL_ONE);					
				// Select The First Image Texture
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[2]);		
				// Start Drawing A Textured Quad
				Gl.glBegin(Gl.GL_QUADS);		
				// Bottom Left
				Gl.glTexCoord2f(roll+0.0f, 0.0f); Gl.glVertex3f(-1.1f, -1.1f,  0.0f);	
				// Bottom Right
				Gl.glTexCoord2f(roll+4.0f, 0.0f); Gl.glVertex3f( 1.1f, -1.1f,  0.0f);	
				// Top Right
				Gl.glTexCoord2f(roll+4.0f, 4.0f); Gl.glVertex3f( 1.1f,  1.1f,  0.0f);	
				// Top Left
				Gl.glTexCoord2f(roll+0.0f, 4.0f); Gl.glVertex3f(-1.1f,  1.1f,  0.0f);	
				// Done Drawing The Quad
				Gl.glEnd();	
			}

			// Enable Depth Testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);	
			// Disable Blending
			Gl.glDisable(Gl.GL_BLEND);		

			// Increase Our Texture Roll Variable
			this.roll += 0.002f;	
			// Is Roll Greater Than One
			if (this.roll > 1.0f)
			{
				this.roll -= 1.0f;
			}
		}
				
		#endregion Render

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch(e.Key)
			{
				case Key.Space:
					this.scene = !this.scene;
					break;
				case Key.M:
					this.masking = !this.masking;
					break;
			}
		}

		#endregion Event Handlers
	}
}
