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
	/// Lesson 08: Blending
	/// </summary>
	public class NeHe008 : NeHe007
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 08: Blending";
			}
		}

		// is blended?
		bool blend; 

		/// <summary>
		/// Is the object blended?
		/// </summary>
		protected bool Blend
		{
			get
			{
				return blend;
			}
			set
			{
				blend = value;
			}
		}

		#endregion Fields

		#region Constructor

		/// <summary>
		/// Basic constructor
		/// </summary>
		public NeHe008()
		{
			this.TextureName = new string[1];
			this.TextureName[0] = "NeHe008.bmp";
			this.Texture = new int[3];
		}

		#endregion Constructor

		#region Lesson Setup
		/// <summary>
		/// Initialize OpenGL
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
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, this.LightAmbient);
			// Setup The Diffuse Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, this.LightDiffuse);
			// Position The Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, this.LightPosition);  
			// Enable Light One
			Gl.glEnable(Gl.GL_LIGHT1); 
			// Full Brightness.  50% Alpha
			Gl.glColor4f(1, 1, 1, 0.5f);
			// Set The Blending Function For Translucency
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE); 
		}

		#endregion Lesson Setup

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.L: 
					this.Light = !this.Light;
					if(!this.Light) 
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
				case Key.B:
					// Blending Code Starts Here
					blend = !blend;
					if(blend) 
					{
						Gl.glEnable(Gl.GL_BLEND);
						// Turn Blending On
						Gl.glDisable(Gl.GL_DEPTH_TEST); 
						// Turn Depth Testing Off
					}
					else 
					{
						Gl.glDisable(Gl.GL_BLEND);  
						// Turn Blending Off
						Gl.glEnable(Gl.GL_DEPTH_TEST);  
						// Turn Depth Testing On
					}
					// Blending Code Ends Here
					break;
			}
		}

		#endregion Event Handlers
	}
}