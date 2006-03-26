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
using System.Reflection;

using SdlDotNet;
using Tao.OpenGl;

namespace SdlDotNet.Examples.RedBook
{
	/// <summary>
	///     This program demonstrates some characters of a stroke (vector) font.  The
	///     characters are represented by display lists, which are given numbers which
	///     correspond to the ASCII values of the characters.  Use of Gl.glCallLists()
	///     is demonstrated.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Original Author:    Silicon Graphics, Inc.
	///         http://www.opengl.org/developers/code/examples/redbook/stroke.c
	///     </para>
	///     <para>
	///         C# Implementation:  Randy Ridge
	///         http://www.taoframework.com
	///     </para>
	///     <para>
	///			SDL.NET implementation: David Hudson
	///			http://cs-sdl.sourceforge.net
	///     </para>
	/// </remarks>
	public class RedBookStroke
	{
		//Width of screen
		int width = 440;
		//Height of screen
		int height = 120;
		
		

		/// <summary>
		/// Lesson title
		/// </summary>
		public static string Title
		{
			get
			{
				return "Stroke - stroke (vector) font";
			}
		}

		#region Private Constants
		private const int PT = 1;
		private const int STROKE = 2;
		private const int END = 3;
		#endregion Private Constants

		#region Private Fields
		private static string test1 = "A SPARE SERAPE APPEARS AS";
		private static string test2 = "APES PREPARE RARE PEPPERS";

		private static CharPoint[] Adata = {
											   new CharPoint(0, 0, PT),
											   new CharPoint(0, 9, PT),
											   new CharPoint(1, 10, PT),
											   new CharPoint(4, 10, PT),
											   new CharPoint(5, 9, PT),
											   new CharPoint(5, 0, STROKE),
											   new CharPoint(0, 5, PT),
											   new CharPoint(5, 5, END)
										   };

		private static CharPoint[] Edata = {
											   new CharPoint(5, 0, PT),
											   new CharPoint(0, 0, PT),
											   new CharPoint(0, 10, PT),
											   new CharPoint(5, 10, STROKE),
											   new CharPoint(0, 5, PT),
											   new CharPoint(4, 5, END)
										   };

		private static CharPoint[] Pdata = {
											   new CharPoint(0, 0, PT),
											   new CharPoint(0, 10, PT),
											   new CharPoint(4, 10, PT),
											   new CharPoint(5, 9, PT),
											   new CharPoint(5, 6, PT),
											   new CharPoint(4, 5, PT),
											   new CharPoint(0, 5, END)
										   };

		private static CharPoint[] Rdata = {
											   new CharPoint(0, 0, PT),
											   new CharPoint(0, 10, PT),
											   new CharPoint(4, 10, PT),
											   new CharPoint(5, 9, PT),
											   new CharPoint(5, 6, PT),
											   new CharPoint(4, 5, PT),
											   new CharPoint(0, 5, STROKE),
											   new CharPoint(3, 5, PT),
											   new CharPoint(5, 0, END)
										   };

		private static CharPoint[] Sdata = {
											   new CharPoint(0, 1, PT),
											   new CharPoint(1, 0, PT),
											   new CharPoint(4, 0, PT),
											   new CharPoint(5, 1, PT),
											   new CharPoint(5, 4, PT),
											   new CharPoint(4, 5, PT),
											   new CharPoint(1, 5, PT),
											   new CharPoint(0, 6, PT),
											   new CharPoint(0, 9, PT),
											   new CharPoint(1, 10, PT),
											   new CharPoint(4, 10, PT),
											   new CharPoint(5, 9, END)
										   };
		#endregion Private Fields

		#region Private Structs
		private struct CharPoint 
		{
			public float X;
			public float Y;
			public int Type;

			public CharPoint(float x, float y, int type) 
			{
				X = x;
				Y = y;
				Type = type;
			}
		}
		#endregion Private Structs

		#region Constructors

		/// <summary>
		/// Basic constructor
		/// </summary>
		public RedBookStroke()
		{
			Initialize();
		}

		#endregion Constructors

		#region Lesson Setup
		/// <summary>
		/// Initializes methods common to all RedBook lessons
		/// </summary>
		private void Initialize()
		{
			// Sets keyboard events
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			Keyboard.EnableKeyRepeat(150,50);
			// Sets the ticker to update OpenGL Context
			Events.Tick += new TickEventHandler(this.Tick); 
			Events.Quit += new QuitEventHandler(this.Quit);
			//			// Sets the resize window event
			//			Events.VideoResize += new VideoResizeEventHandler (this.Resize);
			// Set the Frames per second.
			Events.Fps = 60;
			// Creates SDL.NET Surface to hold an OpenGL scene
			Video.SetVideoModeWindowOpenGL(width, height, true);
			// Sets Window icon and title
			this.WindowAttributes();
		}

		/// <summary>
		/// Sets Window icon and caption
		/// </summary>
		private void WindowAttributes()
		{
			Video.WindowIcon();
			Video.WindowCaption = 
				"SDL.NET - RedBook " + 
				this.GetType().ToString().Substring(26);
		}

		#endregion Lesson Setup
		/// <summary>
		/// Resizes window
		/// </summary>
		private void Reshape()
		{
			Reshape(this.width, this.height);
		}

		// --- Application Methods ---
		#region DrawLetter(CharPoint[] letter)
		/// <summary>
		///     <para>
		///         Interprets the instructions from the array for that letter and renders
		///         the letter with line segments.
		///     </para>
		/// </summary>
		private static void DrawLetter(CharPoint[] letter) 
		{
			int i = 0;

			Gl.glBegin(Gl.GL_LINE_STRIP);
			while(i < letter.Length) 
			{
				switch(letter[i].Type) 
				{
					case PT:
						Gl.glVertex2f(letter[i].X, letter[i].Y);
						break;
					case STROKE:
						Gl.glVertex2f(letter[i].X, letter[i].Y);
						Gl.glEnd();
						Gl.glBegin(Gl.GL_LINE_STRIP);
						break;
					case END:
						Gl.glVertex2f(letter[i].X, letter[i].Y);
						Gl.glEnd();
						Gl.glTranslatef(8.0f, 0.0f, 0.0f);
						return;
				}
				i++;
			}
		}
		#endregion DrawLetter(CharPoint[] letter)

		#region Init()
		/// <summary>
		///     <para>
		///         Create a display list for each of 6 characters.
		///     </para>
		/// </summary>
		private static void Init() 
		{

			int list;

			Gl.glShadeModel(Gl.GL_FLAT);

			list = Gl.glGenLists(128);
			Gl.glListBase(list);
			Gl.glNewList(list + 'A', Gl.GL_COMPILE);
			DrawLetter(Adata);
			Gl.glEndList();
			Gl.glNewList(list + 'E', Gl.GL_COMPILE);
			DrawLetter(Edata);
			Gl.glEndList();
			Gl.glNewList(list + 'P', Gl.GL_COMPILE);
			DrawLetter(Pdata);
			Gl.glEndList();
			Gl.glNewList(list + 'R', Gl.GL_COMPILE);
			DrawLetter(Rdata);
			Gl.glEndList();
			Gl.glNewList(list + 'S', Gl.GL_COMPILE);
			DrawLetter(Sdata);
			Gl.glEndList();
			Gl.glNewList(list + ' ', Gl.GL_COMPILE);
			Gl.glTranslatef(8.0f, 0.0f, 0.0f);
			Gl.glEndList();
		}
		#endregion Init()

		#region PrintStrokedString(string text)
		private static void PrintStrokedString(string text) 
		{
			byte [] textbytes = new byte[text.Length];
			for (int i = 0; i < text.Length; i++) textbytes[i] = (byte) text[i];
			Gl.glCallLists(text.Length, Gl.GL_BYTE, textbytes);
		}
		#endregion PrintStrokedString(string text)

		// --- Callbacks ---
		#region Display()
		private static void Display() 
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
			Gl.glColor3f(1.0f, 1.0f, 1.0f);
			Gl.glPushMatrix();
			Gl.glScalef(2.0f, 2.0f, 2.0f);
			Gl.glTranslatef(10.0f, 30.0f, 0.0f);
			PrintStrokedString(test1);
			Gl.glPopMatrix();
			Gl.glPushMatrix();
			Gl.glScalef(2.0f, 2.0f, 2.0f);
			Gl.glTranslatef(10.0f, 13.0f, 0.0f);
			PrintStrokedString(test2);
			Gl.glPopMatrix();
			Gl.glFlush();
		}
		#endregion Display()

		#region Reshape(int w, int h)
		private static void Reshape(int w, int h) 
		{
			Gl.glViewport(0, 0, w, h);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluOrtho2D(0.0, (double) w, 0.0, (double) h);
		}
		#endregion Reshape(int w, int h)

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.Escape:
					// Will stop the app loop
					Events.QuitApplication();
					break;
				default:
					break;
			}
		}

		private void Tick(object sender, TickEventArgs e)
		{
			Display();
			Video.GLSwapBuffers();
		}

		private void Quit(object sender, QuitEventArgs e)
		{
			Events.QuitApplication();
		}

		//		private void Resize (object sender, VideoResizeEventArgs e)
		//		{
		//			Video.SetVideoModeWindowOpenGL(e.Width, e.Height, true);
		//			if (screen.Width != e.Width || screen.Height != e.Height)
		//			{
		//				//this.Init();
		//				this.Reshape();
		//			}
		//		}

		#endregion Event Handlers

		#region Run Loop
		/// <summary>
		/// Starts demo
		/// </summary>
		public void Run()
		{
			Reshape();
			Init();
			Events.Run();
		}

		#endregion Run Loop
	}
}