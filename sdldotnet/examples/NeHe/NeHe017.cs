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
	/// Lesson 17: 2D Texture Font
	/// </summary>
	public class NeHe017 : NeHe013
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 17: 2D Texture Font";
			}
		}

		#endregion Fields	

		#region Constructor

		/// <summary>
		/// Basic constructor
		/// </summary>
		public NeHe017()
		{
			Events.Quit += new QuitEventHandler(this.Quit);
			this.Texture = new int[2];	
			this.TextureName = new string[2];
			// Texture array
			this.TextureName[0] = "NeHe017.Font.bmp";
			this.TextureName[1] = "NeHe017.Bumps.bmp";
		}
		
		#endregion Constructor

		#region Lesson Setup

		/// <summary>
		/// Initialize OpenGL
		/// </summary>
		protected override void InitGL()
		{
			LoadGLTextures();

			// Enable Texture Mapping
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			// Enable Smooth Shading
			Gl.glShadeModel(Gl.GL_SMOOTH);
			// Black Background
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.5f);
			// Depth Buffer Setup
			Gl.glClearDepth(1.0f);		
			// Enables Depth Testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			// The Type Of Depth Testing To Do
			Gl.glDepthFunc(Gl.GL_LEQUAL);
			// Select The Type Of Blending
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE);
			// Really Nice Perspective Calculations	
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);		

			BuildFont();
		}

		/// <summary>
		/// Build Font
		/// </summary>
		protected override void BuildFont()
		{
			// Holds Our X Character Coord
			float cx;		
			// Holds Our Y Character Coord
			float cy;		
			
			// Creating 256 Display Lists
			this.FontBase = Gl.glGenLists(256);				
			// Select Our Font Texture
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[0]);
			// Loop Through All 256 Lists
			for (int loop=0; loop < 256; loop++)			
			{
				// X Position Of Current Character
				cx = (float)(loop % 16) / 16.0f;
				// Y Position Of Current Character
				cy = (float)(loop / 16) / 16.0f;

				// Start Building A List
				Gl.glNewList((uint)(this.FontBase+loop), Gl.GL_COMPILE);
				// Use A Quad For Each Character
				Gl.glBegin(Gl.GL_QUADS);
				// Texture Coord (Bottom Left)
				Gl.glTexCoord2f(cx, 1 - cy - 0.0625f);			
				// Vertex Coord (Bottom Left)
				Gl.glVertex2i(0, 0);	
				// Texture Coord (Bottom Right)
				Gl.glTexCoord2f(cx + 0.0625f, 1 - cy - 0.0625f);
				// Vertex Coord (Bottom Right)
				Gl.glVertex2i(16, 0);	
				// Texture Coord (Top Right)
				Gl.glTexCoord2f(cx + 0.0625f, 1 - cy);			
				// Vertex Coord (Top Right)
				Gl.glVertex2i(16, 16);	
				// Texture Coord (Top Left)
				Gl.glTexCoord2f(cx, 1 - cy);					
				// Vertex Coord (Top Left)
				Gl.glVertex2i(0, 16);	
				// Done Building Our Quad (Character)
				Gl.glEnd();	
				// Move To The Right Of The Character
				Gl.glTranslated(10, 0, 0);
				// Done Building The Display List
				Gl.glEndList();
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
			// Select Our Second Texture
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[1]);	
			// Move Into The Screen 5 Units
			Gl.glTranslatef(0.0f, 0.0f, -5.0f);
			// Rotate On The Z Axis 45 Degrees (Clockwise)
			Gl.glRotatef(45.0f, 0.0f, 0.0f, 1.0f);					
			// Rotate On The X & Y Axis By Cnt1 (Left To Right)
			Gl.glRotatef(this.Cnt1 * 30.0f, 1.0f, 1.0f, 0.0f);		
			// Disable Blending Before We Draw In 3D
			Gl.glDisable(Gl.GL_BLEND);		
			// Bright White
			Gl.glColor3f(1.0f, 1.0f, 1.0f);	
			// Draw Our First Texture Mapped Quad
			Gl.glBegin(Gl.GL_QUADS);		
			// First Texture Coord
			Gl.glTexCoord2d(0.0f, 0.0f);
			// First Vertex
			Gl.glVertex2f(-1.0f, 1.0f);	
			// Second Texture Coord
			Gl.glTexCoord2d(1.0f, 0.0f);
			// Second Vertex
			Gl.glVertex2f( 1.0f, 1.0f);	
			// Third Texture Coord
			Gl.glTexCoord2d(1.0f, 1.0f);
			// Third Vertex
			Gl.glVertex2f( 1.0f, -1.0f);
			// Fourth Texture Coord
			Gl.glTexCoord2d(0.0f, 1.0f);
			// Fourth Vertex
			Gl.glVertex2f(-1.0f, -1.0f);
			// Done Drawing The First Quad
			Gl.glEnd();			
			// Rotate On The X & Y Axis By 90 Degrees (Left To Right)
			Gl.glRotatef(90.0f, 1.0f, 1.0f, 0.0f);					
			// Draw Our Second Texture Mapped Quad
			Gl.glBegin(Gl.GL_QUADS);
			// First Texture Coord
			Gl.glTexCoord2d(0.0f, 0.0f);
			// First Vertex
			Gl.glVertex2f(-1.0f, 1.0f);	
			// Second Texture Coord
			Gl.glTexCoord2d(1.0f, 0.0f);
			// Second Vertex
			Gl.glVertex2f( 1.0f, 1.0f);	
			// Third Texture Coord
			Gl.glTexCoord2d(1.0f, 1.0f);
			// Third Vertex
			Gl.glVertex2f( 1.0f, -1.0f);
			// Fourth Texture Coord
			Gl.glTexCoord2d(0.0f, 1.0f);
			// Fourth Vertex
			Gl.glVertex2f(-1.0f, -1.0f);
			// Done Drawing Our Second Quad
			Gl.glEnd();			
			// Enable Blending
			Gl.glEnable(Gl.GL_BLEND);
			// Reset The View
			Gl.glLoadIdentity();		
			// Pulsing Colors Based On Text Position
			Gl.glColor3f(1.0f*(float)Math.Cos(this.Cnt1), 1.0f*(float)Math.Sin(this.Cnt2), 1.0f-0.5f*(float)Math.Cos(this.Cnt1+this.Cnt2));
			// Pr(int) Gl.GL Text To The Screen
			GlPrint((int)(280+250*Math.Cos(this.Cnt1)), (int)(235+200*Math.Sin(this.Cnt2)), "NeHe", 0);

			Gl.glColor3f(1.0f*(float)(Math.Sin(this.Cnt2)), 1.0f-0.5f*(float)(Math.Cos(this.Cnt1+this.Cnt2)), 1.0f*(float)(Math.Cos(this.Cnt1)));
			// Pr(int) Gl.GL Text To The Screen
			GlPrint((int)(280+230*Math.Cos(this.Cnt2)), (int)(235+200*Math.Sin(this.Cnt1)), "OpenGL", 0);
			// Set Color To Blue
			Gl.glColor3f(0.0f, 0.0f, 1.0f);
			GlPrint((int)(240+200*Math.Cos((this.Cnt2+this.Cnt1)/5)), 2, "Giuseppe D'Agata", 0);
			// Set Color To White
			Gl.glColor3f(1.0f, 1.0f, 1.0f);
			GlPrint((int)(242+200*Math.Cos((this.Cnt2+this.Cnt1)/5)), 2, "Giuseppe D'Agata", 0);
			// Increase The First Counter
			this.Cnt1 += 0.01f;
			this.Cnt2 += 0.0081f;	
		}

		/// <summary>
		/// Print to screen
		/// </summary>
		/// <param name="displayText">Text to display</param>
		/// <param name="positionX">X position to display the text</param>
		/// <param name="positionY">Y position to display the text</param>
		/// <param name="characterSet"></param>
		public void GlPrint(int positionX, int positionY, string displayText, int characterSet)	
		{
			if (displayText == null || displayText.Length == 0)
			{
				displayText = " ";
			}

			if (characterSet > 1)
			{
				characterSet = 1;
			}
			// Select Our Font Texture
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, Texture[0]);			
			// Disables Depth Testing
			Gl.glDisable(Gl.GL_DEPTH_TEST);	
			// Select The Projection Matrix
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			// Store The Projection Matrix
			Gl.glPushMatrix();	
			// Reset The Projection Matrix
			Gl.glLoadIdentity();
			// Set Up An Ortho Screen
			Gl.glOrtho(0, 640, 0, 480, -1, 1);
			// Select The Modelview Matrix
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			// Store The Modelview Matrix
			Gl.glPushMatrix();	
			// Reset The Modelview Matrix
			Gl.glLoadIdentity();
			// Position The Text (0,0 - Bottom Left)
			Gl.glTranslated(positionX, positionY,0);		
			// Choose The Font Set (0 or 1)
			Gl.glListBase(this.FontBase - 32 + (128 * characterSet));	
			// Write The Text To The Screen
			Gl.glCallLists(displayText.Length, Gl.GL_UNSIGNED_BYTE, displayText);	
			// Select The Projection Matrix
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			// Restore The Old Projection Matrix
			Gl.glPopMatrix();	
			// Select The Modelview Matrix
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			// Restore The Old Projection Matrix
			Gl.glPopMatrix();		
			// Enables Depth Testing					
			Gl.glEnable(Gl.GL_DEPTH_TEST);	
		}

		#endregion Render

		#region Event Handlers

		private void Quit(object sender, QuitEventArgs e)
		{
			KillFont();
		}

		#endregion Event Handlers
	}
}
