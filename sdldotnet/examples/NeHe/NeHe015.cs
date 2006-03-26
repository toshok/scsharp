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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace SdlDotNet.Examples.NeHe 
{
	/// <summary>
	/// Lesson 15: Texture Mapped Outline Fonts
	/// </summary>
	public class NeHe015 : NeHe014
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 15: Texture Mapped Outline Fonts";
			}
		}

		// Storage For Information About Our Outline Font Characters
		private Gdi.GLYPHMETRICSFLOAT[] gmf = new Gdi.GLYPHMETRICSFLOAT[256];

		#endregion Fields

		#region Constructor

		/// <summary>
		/// Basic Constructor
		/// </summary>
		public NeHe015() 
		{
			// One Texture Map
			this.Texture = new int[1];
			this.TextureName = new string[1];
			this.TextureName[0] = "NeHe015.bmp";
			Events.Quit += new QuitEventHandler(this.Quit);
		}

		#endregion Constructor

		#region Lesson Setup

		/// <summary>
		/// Initialize OpenGL
		/// </summary>
		protected override void InitGL()
		{
			GC.KeepAlive(this);
			this.Hdc = User.GetDC(Video.WindowHandle);

			this.LoadGLTextures();
			this.BuildFont();
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
			// Quick And Dirty Lighting (Assumes Light0 Is Set Up)
			Gl.glEnable(Gl.GL_LIGHT0);
			// Enable Lighting
			Gl.glEnable(Gl.GL_LIGHTING);
			// Really Nice Perspective Calculations
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
			// Enable Texture Mapping ( NEW )
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			// Select The Texture
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[0]);
		}

		/// <summary>
		/// Build Font
		/// </summary>
		protected override void BuildFont() 
		{
			// Windows Font ID
			IntPtr font;   
			// Storage For 256 Characters
			this.FontBase = Gl.glGenLists(256);   

			// Create The Font
			font = Gdi.CreateFont( 
				// Height Of Font
				-12,   
				// Width Of Font
				0, 
				// Angle Of Escapement
				0, 
				// Orientation Angle
				0, 
				// Font Weight
				Gdi.FW_BOLD,   
				// Italic
				false, 
				// Underline
				false, 
				// Strikeout
				false, 
				// Character Set Identifier
				Gdi.SYMBOL_CHARSET,   
				// Output Precision
				Gdi.OUT_TT_PRECIS, 
				// Clipping Precision
				Gdi.CLIP_DEFAULT_PRECIS,
				// Output Quality
				Gdi.ANTIALIASED_QUALITY,
				// Family And Pitch
				Gdi.FF_DONTCARE | Gdi.DEFAULT_PITCH, 
				// Font Name
				"Wingdings");  

			// Selects The Font We Created
			Gdi.SelectObject(this.Hdc, font);
			Wgl.wglUseFontOutlines(
				// Select The Current DC
				this.Hdc,
				// Starting Character
				0,
				// Number Of Display Lists To Build
				255,
				// Starting Display Lists
				this.FontBase,
				// Deviation From The True Outlines
				0,
				// Font Thickness In The Z Direction
				0.2f,
				// Use Polygons, Not Lines
				Wgl.WGL_FONT_POLYGONS,
				// Address Of Buffer To Recieve Data
				gmf);  
		}

		/// <summary>
		/// Load textures
		/// </summary>
		protected override void LoadGLTextures()
		{
			if (File.Exists(this.DataDirectory + this.TextureName))
			{		
				this.FilePath = "";
			} 
			// Status Indicator
			Bitmap[] textureImage = new Bitmap[this.TextureName.Length];   
			// Create Storage Space For The Texture

			textureImage[0] = new Bitmap(this.FilePath + this.DataDirectory + this.TextureName[0]);
			// Load The Bitmap
			// Check For Errors, If Bitmap's Not Found, Quit
			if(textureImage[0] != null) 
			{
				Gl.glGenTextures(this.Texture.Length, this.Texture);

				textureImage[0].RotateFlip(RotateFlipType.RotateNoneFlipY); 
				// Flip The Bitmap Along The Y-Axis
				// Rectangle For Locking The Bitmap In Memory
				Rectangle rectangle = new Rectangle(0, 0, textureImage[0].Width, textureImage[0].Height);
				// Get The Bitmap's Pixel Data From The Locked Bitmap
				BitmapData bitmapData = textureImage[0].LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

				// Create Linear Filtered Texture
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[0]);
				Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGB8, textureImage[0].Width, textureImage[0].Height, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_NEAREST);
				Gl.glTexGeni(Gl.GL_S, Gl.GL_TEXTURE_GEN_MODE, Gl.GL_OBJECT_LINEAR);
				Gl.glTexGeni(Gl.GL_T, Gl.GL_TEXTURE_GEN_MODE, Gl.GL_OBJECT_LINEAR);
				Gl.glEnable(Gl.GL_TEXTURE_GEN_S);
				Gl.glEnable(Gl.GL_TEXTURE_GEN_T);

				// If Texture Exists
				if(textureImage[0] != null) 
				{
					// Unlock The Pixel Data From Memory
					textureImage[0].UnlockBits(bitmapData); 
					// Dispose The Bitmap
					textureImage[0].Dispose();   
				}
			}
		}

		#endregion Lesson Setup

		#region Render

		/// <summary>
		/// Renders the scene
		/// </summary>
		protected override void DrawGLScene() 
		{
			// Clear Screen And Depth Buffer
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			// Reset The Current Modelview Matrix
			Gl.glLoadIdentity();   
			Gl.glTranslatef(1.1f * ((float) (Math.Cos(this.Rotation / 16.0f))), 0.8f * ((float) (Math.Sin(this.Rotation / 20.0f))), -3.0f);
			// Rotate On The X Axis
			Gl.glRotatef(this.Rotation, 1, 0, 0);
			// Rotate On The Y Axis
			Gl.glRotatef(this.Rotation * 1.2f, 0, 1, 0);   
			// Rotate On The Z Axis
			Gl.glRotatef(this.Rotation * 1.4f, 0, 0, 1);   
			// Center On X, Y, Z Axis
			Gl.glTranslatef(-0.35f, -0.35f, 0.1f);   
			// Draw A Skull And Crossbones Symbol
			GlPrint("N");  
			// Increase The Rotation Variable
			this.Rotation += 0.1f;   
		}

		#region GlPrint(string text)
		/// <summary>
		/// Custom GL "print" routine.
		/// </summary>
		/// <param name="text">
		/// The text to print.
		/// </param>
		protected override void GlPrint(string text) 
		{
			if(text == null || text.Length == 0) 
			{   
				// If There's No Text
				return;
				// Do Nothing
			}
			float length = 0;  
			// Used To Find The Length Of The Text
			char[] chars = text.ToCharArray();   
			// Holds Our String

			for(int loop = 0; loop < text.Length; loop++) 
			{ // Loop To Find Text Length
				length += gmf[chars[loop]].gmfCellIncX; 
				// Increase Length By Each Characters Width
			}

			Gl.glTranslatef(-length / 2, 0, 0);  
			// Center Our Text On The Screen
			Gl.glPushAttrib(Gl.GL_LIST_BIT); 
			// Pushes The Display List Bits
			Gl.glListBase(this.FontBase);
			// Sets The Base Character to 0
			// .NET - can't call text, it's a string!
			byte [] textbytes = new byte[text.Length];
			for (int i = 0; i < text.Length; i++) textbytes[i] = (byte) text[i];
			Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_BYTE, textbytes); 
			// Draws The Display List Text
			Gl.glPopAttrib();  
			// Pops The Display List Bits
		}
		#endregion GlPrint(string text)

		#endregion Render

		#region Event Handlers

		private void Quit(object sender, QuitEventArgs e)
		{
			KillFont();
		}

		#endregion Event Handlers
	}
}
