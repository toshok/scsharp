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
	/// Lesson 06: Texture Mapping
	/// </summary>
	public class NeHe006 : NeHe001
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 06: Texture Mapping";
			}
		}

		// X Rotation ( NEW )
		float rotationX; 
		// Y Rotation ( NEW )
		float rotationY;
		// Z Rotation ( NEW )
		float rotationZ;
		// Storage For One Texture ( NEW )
		int[] texture;
		// Directory to find the data files
		string dataDirectory = @"Data/";
		// Path to Data directory
		string filePath = @"../../";
		// Name of texture
		string[] textureName;

		/// <summary>
		/// Rotation on the X axis
		/// </summary>
		protected float RotationX
		{
			get
			{
				return this.rotationX;
			}
			set
			{
				this.rotationX = value;
			}
		}

		/// <summary>
		/// rotation on the Y axis
		/// </summary>
		protected float RotationY
		{
			get
			{
				return this.rotationY;
			}
			set
			{
				this.rotationY = value;
			}
		}

		/// <summary>
		/// rotation on the Z axis
		/// </summary>
		protected float RotationZ
		{
			get
			{
				return this.rotationZ;
			}
			set
			{
				this.rotationZ = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected string[] TextureName
		{
			get
			{
				return textureName;
			}
			set
			{
				textureName = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected int [] Texture
		{
			get
			{
				return texture;
			}
			set
			{
				texture = value;
			}
		}

		/// <summary>
		/// Directory to find the data files
		/// </summary>
		protected string DataDirectory
		{
			get
			{
				return dataDirectory;
			}
			set
			{
				dataDirectory = value;
			}
		}

		/// <summary>
		/// Path to Data directory
		/// </summary>
		protected string FilePath
		{
			get
			{
				return filePath;
			}
			set
			{
				filePath = value;
			}
		}

		#endregion Fields
		
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public NeHe006()
		{
			textureName = new string[1];
			textureName[0] = "NeHe006.bmp";
			texture = new int[1];
		}

		#endregion Constructor

		#region Lesson Setup

		/// <summary>
		/// Initializes the OpenGL system
		/// </summary>
		protected override void InitGL()
		{
			// Loads bitmaps and converts to textures
			this.LoadGLTextures();
			// Enable Texture Mapping ( NEW )
			Gl.glEnable(Gl.GL_TEXTURE_2D);
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

		#region void DrawGLScene()

		/// <summary>
		/// Renders the scene
		/// </summary>
		protected override void DrawGLScene()
		{
			// Clear The Screen And The Depth Buffer
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			// Reset The View
			Gl.glLoadIdentity();   
			Gl.glTranslatef(0, 0, -5);

			Gl.glRotatef(this.rotationX, 1, 0, 0);
			Gl.glRotatef(this.rotationY, 0, 1, 0);
			Gl.glRotatef(this.rotationZ, 0, 0, 1);

			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[0]);

			Gl.glBegin(Gl.GL_QUADS);
			// Front Face
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-1, -1, 1);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(1, -1, 1);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(1, 1, 1);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-1, 1, 1);
			// Back Face
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(-1, -1, -1);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(-1, 1, -1);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(1, 1, -1);
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(1, -1, -1);
			// Top Face
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-1, 1, -1);
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-1, 1, 1);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(1, 1, 1);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(1, 1, -1);
			// Bottom Face
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(-1, -1, -1);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(1, -1, -1);
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(1, -1, 1);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(-1, -1, 1);
			// Right face
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(1, -1, -1);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(1, 1, -1);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(1, 1, 1);
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(1, -1, 1);
			// Left Face
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-1, -1, -1);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(-1, -1, 1);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(-1, 1, 1);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-1, 1, -1);
			Gl.glEnd();

			this.rotationX += 0.3f;
			this.rotationY += 0.2f;
			this.rotationZ += 0.4f;
		}

		#endregion void DrawGLScene()

		#region void LoadGLTextures()

		/// <summary>
		/// Load bitmaps and convert to textures.
		/// </summary>
		protected virtual void LoadGLTextures() 
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
					Rectangle rectangle = 
						new Rectangle(0, 0, textureImage[i].Width, textureImage[i].Height);
					// Get The Bitmap's Pixel Data From The Locked Bitmap
					BitmapData bitmapData = 
						textureImage[i].LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

					// Typical Texture Generation Using Data From The Bitmap
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[i]);
					Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB8, textureImage[i].Width, textureImage[i].Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
					Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
					Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

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

		#endregion void LoadGLTextures()
	}
}