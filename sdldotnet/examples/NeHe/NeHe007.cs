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
using System.Reflection;

using SdlDotNet;
using Tao.OpenGl;

namespace SdlDotNet.Examples.NeHe
{
	/// <summary>
	/// Lesson 07: Texture Filters, Lighting, and Keyboard Control
	/// </summary>
	public class NeHe007 : NeHe006
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 07: Texture Filters, Lighting, and Keyboard Control";
			}
		}

		// X Rotation Speed
		float xspeed;
		// Y Rotation Speed
		float yspeed;
		// Depth Into The Screen
		float depthZ = -5;
		//filter
		int filter;
		// Lighting ON/OFF ( NEW )
		bool light;
  
		float[] lightAmbient = {0.5f, 0.5f, 0.5f, 1};
		float[] lightDiffuse = {1, 1, 1, 1};
		float[] lightPosition = {0, 0, 2, 1};

		/// <summary>
		/// Ambient light array
		/// </summary>
		protected float[] LightAmbient
		{
			get
			{
				return lightAmbient;
			}
			set
			{
				lightAmbient = value;
			}
		}

		/// <summary>
		/// Diffure light array
		/// </summary>
		protected float[] LightDiffuse
		{
			get
			{
				return lightDiffuse;
			}
			set
			{
				lightDiffuse = value;
			}
		}

		/// <summary>
		/// Light position array
		/// </summary>
		protected float[] LightPosition
		{
			get
			{
				return lightPosition;
			}
			set
			{
				lightPosition = value;
			}
		}

		/// <summary>
		/// Is the light shining
		/// </summary>
		protected bool Light
		{
			get
			{
				return light;
			}
			set
			{
				light = value;
			}
		}

		/// <summary>
		/// Speed in X direction
		/// </summary>
		protected float XSpeed
		{
			get
			{
				return xspeed;
			}
			set
			{
				xspeed = value;
			}
		}

		/// <summary>
		/// Speed in Y direction
		/// </summary>
		protected float YSpeed
		{
			get
			{
				return yspeed;
			}
			set
			{
				yspeed = value;
			}
		}

		/// <summary>
		/// Depth
		/// </summary>
		protected float DepthZ
		{
			get
			{
				return depthZ;
			}
			set
			{
				depthZ = value;
			}
		}

		/// <summary>
		/// Filtering
		/// </summary>
		protected int Filter
		{
			get
			{
				return filter;
			}
			set
			{
				filter = value;
			}
		}

		#endregion Fields

		#region Constructor

		/// <summary>
		/// Basic constructor
		/// </summary>
		public NeHe007()
		{
			this.TextureName = new string[1];
			this.TextureName[0] = "NeHe007.bmp";
			this.Texture = new int[3];
		}

		#endregion Constructor

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
			// Enable Texture Mapping ( NEW )
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			this.LoadGLFilteredTextures();
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			Keyboard.EnableKeyRepeat(150,50);
			// Setup The Ambient Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, lightAmbient);
			// Setup The Diffuse Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, lightDiffuse);
			// Position The Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, lightPosition);  
			// Enable Light One
			Gl.glEnable(Gl.GL_LIGHT1); 
		}

		#region override void LoadGLFilteredTextures()
		/// <summary>
		/// Load bitmaps and convert to textures.
		/// </summary>
		/// <returns>
		/// <c>true</c> on success, otherwise <c>false</c>.
		/// </returns>
		protected virtual void LoadGLFilteredTextures() 
		{  
			if (File.Exists(this.DataDirectory + this.TextureName[0]))
			{		
				this.FilePath = "";
			} 
			// Status Indicator
			Bitmap[] textureImage = new Bitmap[this.TextureName.Length];   
			// Create Storage Space For The Texture

			for(int i=0; i<this.TextureName.Length; i++)
			{
				textureImage[i] = new Bitmap(this.FilePath + this.DataDirectory + this.TextureName[i]);
			}
			// Load The Bitmap
			// Check For Errors, If Bitmap's Not Found, Quit
			if(textureImage[0] != null) 
			{
				// Create The Texture
				Gl.glGenTextures(this.Texture.Length, this.Texture); 

				for(int i = 0; i < textureImage.Length; i++) 
				{
					textureImage[i].RotateFlip(RotateFlipType.RotateNoneFlipY); 
					// Flip The Bitmap Along The Y-Axis
					// Rectangle For Locking The Bitmap In Memory
					Rectangle rectangle = new Rectangle(0, 0, textureImage[i].Width, textureImage[i].Height);
					// Get The Bitmap's Pixel Data From The Locked Bitmap
					BitmapData bitmapData = textureImage[i].LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

					// Create Nearest Filtered Texture
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[i]);
					Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
					Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
					Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB8, textureImage[i].Width, textureImage[i].Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

					// Create Linear Filtered Texture
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[i+1]);
					Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
					Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
					Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB8, textureImage[i].Width, textureImage[i].Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

					// Create MipMapped Texture
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[i+2]);
					Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
					Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_NEAREST);
					Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGB8, textureImage[i].Width, textureImage[i].Height, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

					if(textureImage[i] != null) 
					{
						// If Texture Exists
						textureImage[i].UnlockBits(bitmapData); 
						// Unlock The Pixel Data From Memory
						textureImage[i].Dispose();   
						// Dispose The Bitmap
					}
				}
			}
		}
		#endregion override void LoadGLFilteredTextures()

		#endregion Lesson Setup

		#region Rendering

		/// <summary>
		/// Renders the scene
		/// </summary>
		protected override void DrawGLScene()
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			// Clear The Screen And The Depth Buffer
			Gl.glLoadIdentity();   
			// Reset The View
			Gl.glTranslatef(0, 0, depthZ);

			Gl.glRotatef(this.RotationX, 1, 0, 0);
			Gl.glRotatef(this.RotationY, 0, 1, 0);

			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[filter]);

			Gl.glBegin(Gl.GL_QUADS);
			// Front Face
			Gl.glNormal3f(0, 0, 1);
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-1, -1, 1);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(1, -1, 1);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(1, 1, 1);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-1, 1, 1);
			// Back Face
			Gl.glNormal3f(0, 0, -1);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(-1, -1, -1);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(-1, 1, -1);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(1, 1, -1);
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(1, -1, -1);
			// Top Face
			Gl.glNormal3f(0, 1, 0);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-1, 1, -1);
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-1, 1, 1);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(1, 1, 1);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(1, 1, -1);
			// Bottom Face
			Gl.glNormal3f(0, -1, 0);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(-1, -1, -1);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(1, -1, -1);
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(1, -1, 1);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(-1, -1, 1);
			// Right face
			Gl.glNormal3f(1, 0, 0);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(1, -1, -1);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(1, 1, -1);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(1, 1, 1);
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(1, -1, 1);
			// Left Face
			Gl.glNormal3f(-1, 0, 0);
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-1, -1, -1);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(-1, -1, 1);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(-1, 1, 1);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-1, 1, -1);
			Gl.glEnd();

			this.RotationX += this.XSpeed;
			this.RotationY += this.YSpeed;
		}

		#endregion Rendering

		#region Event Handlers
		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.L: 
					light = !light;
					if(!light) 
					{
						Gl.glDisable(Gl.GL_LIGHTING);
					}
					else 
					{
						Gl.glEnable(Gl.GL_LIGHTING);
					}
					break;	
				case Key.F:
					filter += 1;
					if(filter > 2) 
					{
						filter = 0;
					}
					break;
				case Key.PageUp:
					depthZ -= 0.02f;
					break;
				case Key.PageDown:
					depthZ += 0.02f;
					break;
				case Key.UpArrow: 
					xspeed -= 0.01f;
					break;
				case Key.DownArrow:
					xspeed += 0.01f;
					break;
				case Key.RightArrow:
					yspeed += 0.01f;
					break;
				case Key.LeftArrow:
					yspeed -= 0.01f;
					break;
			}
		}

		#endregion Event Handlers
	}
}