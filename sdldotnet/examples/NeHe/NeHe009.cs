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
	/// Lesson 09: Moving Bitmaps in 3D Space
	/// </summary>
	public class NeHe009 : NeHe006
	{
		#region Fields
		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 09: Moving Bitmaps in 3D Space";
			}
		}

		// Random Number Generator
		Random rand = new Random();
		// Twinkling Stars
		bool twinkle;   
		// Number Of Stars To Draw
		const int num = 50;

		// Need To Keep Track Of 'num' Stars
		float zoom = -15;
		// Distance Away From Stars
		float tilt = 90;
		// Tilt The View
		float spin; 
		// Spin Stars
		int loop; 
		// Array to hold stars
		Star[] stars = new Star[num]; 

		#endregion Fields
		
		#region Structs

		// Create A Structure For Star
		private struct Star 
		{						
			// Stars Color
			byte r;
			byte g;
			byte b;   
			// Stars Distance From Center
			float dist; 
			// Stars Current Angle
			float angle;

			/// <summary>
			/// 
			/// </summary>
			public byte Red
			{
				get
				{
					return r;
				}
				set
				{
					this.r = value;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public byte Blue
			{
				get
				{
					return b;
				}
				set
				{
					this.b = value;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public byte Green
			{
				get
				{
					return g;
				}
				set
				{
					this.g = value;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public float Distance
			{
				get
				{
					return dist;
				}
				set
				{
					dist = value;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public float Angle
			{
				get
				{
					return angle;
				}
				set
				{
					angle = value;
				}
			}

		}

		#endregion Structs

		#region Constructor

		/// <summary>
		/// Basic Constructor
		/// </summary>
		public NeHe009()
		{
			this.TextureName = new string[1];
			this.TextureName[0] = "NeHe009.bmp";
			this.Texture = new int[1];
		}

		#endregion Constructor

		#region Lesson Setup
		/// <summary>
		/// Initializes OpenGL
		/// </summary>
		protected override void InitGL()
		{
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			Keyboard.EnableKeyRepeat(150,50);
			this.LoadGLTextures();
			// Enable Texture Mapping
			Gl.glEnable(Gl.GL_TEXTURE_2D);   
			// Enable Smooth Shading
			Gl.glShadeModel(Gl.GL_SMOOTH);   
			// Black Background
			Gl.glClearColor(0, 0, 0, 0.5f);  
			// Depth Buffer Setup
			Gl.glClearDepth(1);
			// Really Nice Perspective Calculations
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST); 
			// Set The Blending Function For Translucency
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE); 
			Gl.glEnable(Gl.GL_BLEND);

			for(loop = 0; loop < num; loop++) 
			{
				stars[loop].Angle = 0;
				stars[loop].Distance = ((float) loop / num) * 5;
				stars[loop].Red = (byte) (rand.Next() % 256);
				stars[loop].Green = (byte) (rand.Next() % 256);
				stars[loop].Blue = (byte) (rand.Next() % 256);
			}
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
			// Select Our Texture
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[0]); 
			
			// Loop Through All The Stars		
			for(loop = 0; loop < num; loop++) 
			{  
				// Reset The View Before We Draw Each Star
				Gl.glLoadIdentity();   
				// Zoom Into The Screen (Using The Value In 'zoom')
				Gl.glTranslatef(0, 0, zoom); 
				// Tilt The View (Using The Value In 'tilt')
				Gl.glRotatef(tilt, 1, 0, 0); 
				// Rotate To The Current Stars Angle
				Gl.glRotatef(stars[loop].Angle, 0, 1, 0);   
				// Move Forward On The X Plane
				Gl.glTranslatef(stars[loop].Distance, 0, 0);
				// Cancel The Current Stars Angle
				Gl.glRotatef(-stars[loop].Angle, 0, 1, 0);  
				// Cancel The Screen Tilt
				Gl.glRotatef(-tilt, 1, 0, 0);

				if(twinkle) 
				{
					Gl.glColor4ub(stars[(num - loop) - 1].Red, stars[(num - loop) - 1].Green, stars[(num - loop) - 1].Blue, 255);
					Gl.glBegin(Gl.GL_QUADS);
					Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-1, -1, 0);
					Gl.glTexCoord2f(1, 0); Gl.glVertex3f(1, -1, 0);
					Gl.glTexCoord2f(1, 1); Gl.glVertex3f(1, 1, 0);
					Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-1, 1, 0);
					Gl.glEnd();
				}
				Gl.glRotatef(spin, 0, 0, 1);
				Gl.glColor4ub(stars[loop].Red, stars[loop].Green, stars[loop].Blue, 255);
				Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-1, -1, 0);
				Gl.glTexCoord2f(1, 0); Gl.glVertex3f(1, -1, 0);
				Gl.glTexCoord2f(1, 1); Gl.glVertex3f(1, 1, 0);
				Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-1, 1, 0);
				Gl.glEnd();
				spin += 0.01f;
				stars[loop].Angle += ((float) loop / num);
				stars[loop].Distance -= 0.01f;
				if(stars[loop].Distance < 0) 
				{
					stars[loop].Distance += 5;
					stars[loop].Red = (byte) (rand.Next() % 256);
					stars[loop].Green = (byte) (rand.Next() % 256);
					stars[loop].Blue = (byte) (rand.Next() % 256);
				}
			}
		}

		#endregion Render

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.T: 
					twinkle = !twinkle;
					break;	
				case Key.PageUp:
					zoom -= 0.2f;
					break;
				case Key.PageDown:
					zoom += 0.2f;
					break;
				case Key.UpArrow: 
					tilt -= 0.01f;
					break;
				case Key.DownArrow:
					tilt += 0.01f;
					break;
			}
		}

		#endregion Event Handlers
	}
}