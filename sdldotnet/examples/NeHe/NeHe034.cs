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
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

using SdlDotNet;
using Tao.OpenGl;

namespace SdlDotNet.Examples.NeHe
{
	/// <summary>
	/// Lesson 34: Beautiful Landscapes By Means Of Height Mapping
	/// </summary>
	public class NeHe034 : NeHe025
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 34: Beautiful Landscapes By Means Of Height Mapping";
			}
		}

		// Size Of Our .RAW Height Map (NEW)
		const int MAP_SIZE = 1024;
		// Width And Height Of Each Quad (NEW)
		const int STEP_SIZE = 16;
		// Ratio That The Y Is Scaled According To The X And Z (NEW)
		const float HEIGHT_RATIO = 1.5f;
		// Polygon Flag Set To TRUE By Default (NEW)
		bool bRender = true;
		// Holds The Height Map Data (NEW)
		byte[] heightMap = new byte[MAP_SIZE * MAP_SIZE];
		float scaleValue = 0.15f;
		
		#endregion Fields

		#region Constructor

		/// <summary>
		/// Basic constructor
		/// </summary>
		public NeHe034()
		{
		}

		#endregion Constructor

		#region Lesson Setup

		#region void InitGL()
		/// <summary>
		/// All setup for OpenGL goes here.
		/// </summary>
		protected override void InitGL() 
		{   
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			Events.MouseButtonDown += new MouseButtonEventHandler(this.MouseButtonDown);
			Keyboard.EnableKeyRepeat(150,50);
   
			// Enable Smooth Shading
			Gl.glShadeModel(Gl.GL_SMOOTH);
			// Black Background
			Gl.glClearColor(0, 0, 0, 0.5f);
			// Depth Buffer Setup
			Gl.glClearDepth(1);
			// Enables Depth Testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			// The Type Of Depth Testing To Do
			Gl.glDepthFunc(Gl.GL_LEQUAL);
			// Really Nice Perspective Calculations
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
			
			LoadRawFile("NeHe034.Terrain.raw", MAP_SIZE * MAP_SIZE, 
				ref heightMap); 
		}
		#endregion void InitGL()

		#region void LoadRawFile(string name, int size, ref byte[] heightMap)
		/// <summary>
		/// Read data from file.
		/// </summary>
		/// <param name="name">
		/// Name of file where data resides.
		/// </param>
		/// <param name="size">
		/// Size of file to be read.
		/// </param>
		/// <param name="heightMap">
		/// Where data is put when read.
		/// </param>
		private void LoadRawFile(string name, int size, ref byte[] heightMap) 
		{
			if(name == null || name.Length == 0) 
			{
				return;
			}

			// Look For Data\Filename
			string fileName1 = string.Format(CultureInfo.CurrentCulture,"Data{0}{1}",
				
				Path.DirectorySeparatorChar, name);
			// Look For ..\..\Data\Filename
			string fileName2 = string.Format(CultureInfo.CurrentCulture,"{0}{1}{0}{1}Data{1}{2}",
				
				"..", Path.DirectorySeparatorChar, name);

			// Make Sure The File Exists In One Of The Usual Directories
			if(!File.Exists(name) && !File.Exists(fileName1) && 
				!File.Exists(fileName2)) 
			{
				MessageBox.Show("Can't Find The Height Map!", 
					"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
			}

			// Does The File Exist Here?
			if(File.Exists(fileName1)) 
			{
				// Set To Correct File Path
				name = fileName1;
			}
			else if(File.Exists(fileName2)) 
			{
				// Set To Correct File Path
				name = fileName2;
			}

			// Open The File In Read / Binary Mode
			using(FileStream fs = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read)) 
			{
				BinaryReader r = new BinaryReader(fs);
				heightMap = r.ReadBytes(size);
			}
		}
		#endregion void LoadRawFile(string name, int size, ref byte[] heightMap)

		#endregion Lesson Setup

		#region Render

		#region bool DrawGLScene()
		/// <summary>
		/// Renders the scene
		/// </summary>
		protected override void DrawGLScene() 
		{
			// Clear The Screen And The Depth Buffer
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			// Reset The Matrix
			Gl.glLoadIdentity();

			// This Determines Where The Camera's Position And View Is
			Glu.gluLookAt(212, 60, 194, 186, 55, 171, 0, 1, 0);
			Gl.glScalef(scaleValue, scaleValue * HEIGHT_RATIO, scaleValue);
			RenderHeightMap(heightMap);
		}
		#endregion void DrawGLScene()

		#region int GetHeight(byte[] heightMap, int x, int y)
		/// <summary>
		/// Returns the height from a height map index.
		/// </summary>
		/// <param name="heightMap">
		/// Height map data.
		/// </param>
		/// <param name="x">
		/// X coordinate value.
		/// </param>
		/// <param name="y">
		/// Y coordinate value.
		/// </param>
		/// <returns>
		/// Returns int with height data.
		/// </returns>
		private static int GetHeight(byte[] heightMap, int x, int y) 
		{  
			// Error Check Our x Value
			x = x % MAP_SIZE;
			// Error Check Our y Value
			y = y % MAP_SIZE;  

			// Index Into Our Height Array And Return The Height
			return heightMap[x + (y * MAP_SIZE)];
		}
		#endregion int GetHeight(byte[] heightMap, int x, int y)

		#region void RenderHeightMap(byte[] heightMap)
		/// <summary>
		/// This renders the height map as quads.
		/// </summary>
		/// <param name="heightMap">
		/// Height map data.
		/// </param>
		private void RenderHeightMap(byte[] heightMap) 
		{
			// Create Some Variables To Walk The Array With.
			int X;
			int Y;
			// Create Some Variables For Readability
			int x;
			int y;
			int z;

			// What We Want To Render
			if(bRender) 
			{
				// Render Polygons
				Gl.glBegin(Gl.GL_QUADS);
			}
			else 
			{
				// Render Lines Instead
				Gl.glBegin(Gl.GL_LINES);
			}

			for(X = 0; X < (MAP_SIZE - STEP_SIZE); X += STEP_SIZE) 
			{
				for (Y = 0; Y < (MAP_SIZE-STEP_SIZE); Y += STEP_SIZE) 
				{
					// Get The (X, Y, Z) Value For The Bottom Left Vertex
					x = X;
					y = GetHeight(heightMap, X, Y);
					z = Y;

					// Set The Color Value Of The Current Vertex
					SetVertexColor(heightMap, x, z);
					// Send This Vertex To OpenGL To Be Rendered 
					// (Integer Points Are Faster)
					Gl.glVertex3i(x, y, z);

					// Get The (X, Y, Z) Value For The Top Left Vertex
					x = X;
					y = GetHeight(heightMap, X, Y + STEP_SIZE);
					z = Y + STEP_SIZE;

					// Set The Color Value Of The Current Vertex
					SetVertexColor(heightMap, x, z);
					// Send This Vertex To OpenGL To Be Rendered
					Gl.glVertex3i(x, y, z);

					// Get The (X, Y, Z) Value For The Top Right Vertex
					x = X + STEP_SIZE;
					y = GetHeight(heightMap, X + STEP_SIZE, Y + STEP_SIZE);
					z = Y + STEP_SIZE;

					// Set The Color Value Of The Current Vertex
					SetVertexColor(heightMap, x, z);
					// Send This Vertex To OpenGL To Be Rendered
					Gl.glVertex3i(x, y, z);

					// Get The (X, Y, Z) Value For The Bottom Right Vertex
					x = X + STEP_SIZE;
					y = GetHeight(heightMap, X + STEP_SIZE, Y);
					z = Y;

					// Set The Color Value Of The Current Vertex
					SetVertexColor(heightMap, x, z);
					// Send This Vertex To OpenGL To Be Rendered
					Gl.glVertex3i(x, y, z);
				}
			}
			Gl.glEnd();
			// Reset The Color
			Gl.glColor4f(1, 1, 1, 1);
		}
		#endregion void RenderHeightMap(byte[] heightMap)

		#region Reshape()
		/// <summary>
		/// Resizes and initializes the GL window.
		/// </summary>
		protected override void Reshape() 
		{
			base.Reshape(500.0F);
		}
		#endregion ReSizeGLScene(int width, int height)

		#region SetVertexColor(byte[] heightMap, int x, int y)
		/// <summary>
		/// Sets the color value for a particular index, 
		/// depending on the height index.
		/// </summary>
		/// <param name="heightMap">
		/// Height map data.
		/// </param>
		/// <param name="x">
		/// X coordinate value.
		/// </param>
		/// <param name="y">
		/// Y coordinate value.
		/// </param>
		private void SetVertexColor(byte[] heightMap, int x, int y) 
		{
			float fColor = -0.15f + (GetHeight(heightMap, x, y ) / 256.0f);
			// Assign This Blue Shade To The Current Vertex
			Gl.glColor3f(0, 0, fColor);
		}
		#endregion SetVertexColor(byte[] heightMap, int x, int y)

		#endregion Render

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.UpArrow: 
					// Increase the scale value to zoom in
					scaleValue += 0.001f;
					break;
				case Key.DownArrow:
					// Increase the scale value to zoom in
					scaleValue -= 0.001f;
					break;				
			}
		}

		private void MouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			bRender = !bRender;
		}

		#endregion Event Handlers
	}
}