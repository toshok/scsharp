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
using System.Globalization;
using System.Drawing;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace SdlDotNet.Examples.NeHe 
{
	/// <summary>
	/// Lesson 14: Outline Fonts
	/// </summary>
	public class NeHe014 : NeHe013 
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 14: Outline Fonts";
			}
		}

		// Storage For Information About Our Outline Font Characters
		Gdi.GLYPHMETRICSFLOAT[] gmf = new Gdi.GLYPHMETRICSFLOAT[256];

		float rot;

		/// <summary>
		/// Used To Rotate The Text
		/// </summary>
		public float Rotation
		{
			get
			{
				return rot;
			}
			set
			{
				rot = value;
			}
		}

		#endregion Fields

		#region Constructor

		/// <summary>
		/// Basic Constructor
		/// </summary>
		public NeHe014() 
		{
			Events.Quit += new QuitEventHandler(this.Quit);
		}

		#endregion Constructor

		#region Lesson Setup

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
				Gdi.ANSI_CHARSET,  
				// Output Precision
				Gdi.OUT_TT_PRECIS, 
				// Clipping Precision
				Gdi.CLIP_DEFAULT_PRECIS,
				// Output Quality
				Gdi.ANTIALIASED_QUALITY,
				// Family And Pitch
				Gdi.FF_DONTCARE | Gdi.DEFAULT_PITCH, 
				// Font Name
				"Comic Sans MS");  

			// Selects The Font We Created
			Gdi.SelectObject(this.Hdc, font);
			// Select The Current DC	
			Wgl.wglUseFontOutlines(
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
			// Move One Unit Into The Screen
			Gl.glTranslatef(0, 0, -10);
			// Rotate On The X Axis
			Gl.glRotatef(rot, 1, 0, 0);
			// Rotate On The Y Axis
			Gl.glRotatef(rot * 1.5f, 0, 1, 0);   
			// Rotate On The Z Axis
			Gl.glRotatef(rot * 1.4f, 0, 0, 1);   
			// Pulsing Colors Based On The Rotation
			Gl.glColor3f(1.0f * ((float) (Math.Cos(rot / 20.0f))), 1.0f * ((float) (Math.Sin(rot / 25.0f))), 1.0f - 0.5f * ((float) (Math.Cos(rot / 17.0f))));
			// Print GL Text To The Screen
			GlPrint(string.Format(CultureInfo.CurrentCulture,"NeHe - {0:0.00}", rot / 50));
			// Increase The Rotation Variable
			rot += 0.5f;   
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
			// If There's No Text
			if(text == null || text.Length == 0) 
			{   
				// Do Nothing
				return;
			}

			// Used To Find The Length Of The Text
			float length = 0; 
			// Holds Our String
			char[] chars = text.ToCharArray();   

			// Loop To Find Text Length
			for(int loop = 0; loop < text.Length; loop++) 
			{
				// Increase Length By Each Characters Width
				length += gmf[chars[loop]].gmfCellIncX; 
			}

			// Center Our Text On The Screen
			Gl.glTranslatef(-length / 2, 0, 0);  
			// Pushes The Display List Bits
			Gl.glPushAttrib(Gl.GL_LIST_BIT);
			// Sets The Base Character to 0
			Gl.glListBase(this.FontBase);
			// .NET - can't call text, it's a string!
			byte [] textbytes = new byte[text.Length];
			for (int i = 0; i < text.Length; i++) textbytes[i] = (byte) text[i];
			// Draws The Display List Text
			Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_BYTE, textbytes); 
			// Pops The Display List Bits
			Gl.glPopAttrib();  
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
