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
using System.Globalization;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace SdlDotNet.Examples.NeHe 
{
	/// <summary>
	/// Lesson 13: Bitmap Fonts
	/// </summary>
	public class NeHe013 : NeHe006, IDisposable
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 13: Bitmap Fonts";
			}
		}
		
		IntPtr hDC;

		/// <summary>
		/// Window Handle
		/// </summary>
		public IntPtr Hdc
		{
			get
			{
				return hDC;
			}
			set
			{
				hDC = value;
			}
		}

		int fontBase; 

		/// <summary>
		/// Base Display List For The Font Set
		/// </summary>
		public int FontBase
		{
			get
			{
				return fontBase;
			}
			set
			{
				fontBase = value;
			}
		}

		float cnt1;
		
		/// <summary>
		/// 1st Counter Used To Move Text &amp; For Coloring
		/// </summary>
		public float Cnt1
		{
			get
			{
				return cnt1;
			}
			set
			{
				cnt1 = value;
			}
		}

		float cnt2;

		/// <summary>
		/// 2nd Counter Used To Move Text &amp; For Coloring
		/// </summary>
		public float Cnt2
		{
			get
			{
				return cnt2;
			}
			set
			{
				cnt2 = value;
			}
		}
	
		#endregion Fields

		#region Constructor

		/// <summary>
		/// Basic Constructor
		/// </summary>
		public NeHe013() 
		{
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
			hDC = User.GetDC(Video.WindowHandle);

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
			// Really Nice Perspective Calculations
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
			BuildFont(); 
		}

		/// <summary>
		/// Create Font
		/// </summary>
		protected virtual void BuildFont() 
		{
			GC.KeepAlive(this);
			fontBase = Gl.glGenLists(96);

			System.Drawing.Font font = new System.Drawing.Font(
				"Courier New", 
				17,
				FontStyle.Bold);
			
			IntPtr oldfont = Gdi.SelectObject(hDC, font.ToHfont());
			// Selects The Font We Want
			Wgl.wglUseFontBitmaps(hDC, 32, 96, fontBase);
			// Builds 96 Characters Starting At Character 32
			Gdi.SelectObject(hDC, oldfont);
			// Selects The Font We Want
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
			Gl.glTranslatef(0, 0, -1);
			// Pulsing Colors Based On Text Position
			Gl.glColor3f(1.0f * ((float) (Math.Cos(Cnt1))), 1.0f * ((float) (Math.Sin(cnt2))), 1.0f - 0.5f* ((float) (Math.Cos(Cnt1 + cnt2))));
			// Position The Text On The Screen
			Gl.glRasterPos2f(-0.45f + 0.05f * ((float) (Math.Cos(Cnt1))), 0.32f * ((float) (Math.Sin(cnt2))));
			// Print GL Text To The Screen
			GlPrint(string.Format(CultureInfo.CurrentCulture,"Active OpenGL Text With NeHe - {0:0.00}", Cnt1));
			// Increase The First Counter
			Cnt1 += 0.051f;
			// Increase The First Counter
			cnt2 += 0.005f;
		}

		/// <summary>
		/// Print font to screen
		/// </summary>
		/// <param name="text"></param>
		protected virtual void GlPrint(string text) 
		{
			// If There's No Text
			if(text == null || text.Length == 0) 
			{
				// Do Nothing
				return;
				
			}
			// Pushes The Display List Bits
			Gl.glPushAttrib(Gl.GL_LIST_BIT);
			// Sets The Base Character to 32
			Gl.glListBase(fontBase - 32);
			// .NET -- we can't just pass text, we need to convert
			byte [] textbytes = new byte[text.Length];

			for (int i = 0; i < text.Length; i++)
			{
				textbytes[i] = (byte) text[i];
			}

			// Draws The Display List Text
			Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_BYTE, textbytes);
			// Pops The Display List Bits
			Gl.glPopAttrib();
		}

		#endregion Render

		#region Close Lesson

		/// <summary>
		/// Close font
		/// </summary>
		protected virtual void KillFont() 
		{
			// Delete All 96 Characters
			Gl.glDeleteLists(fontBase, 96);
		}

		#endregion Close Lesson

		#region Event Handlers

		private void Quit(object sender, QuitEventArgs e)
		{
			this.KillFont();
		}

		#endregion Event Handlers

		#region IDisposable Members

		private bool disposed;

		/// <summary>
		/// Destroy sprite
		/// </summary>
		/// <param name="disposing">If true, remove all unamanged resources</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					GC.SuppressFinalize(this);
				}
				this.disposed = true;
			}
		}
		/// <summary>
		/// Destroy object
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>
		/// Destroy object
		/// </summary>
		public void Close() 
		{
			Dispose();
		}

		/// <summary>
		/// Destroy object
		/// </summary>
		~NeHe013() 
		{
			Dispose(false);
		}

		#endregion
	}
}
