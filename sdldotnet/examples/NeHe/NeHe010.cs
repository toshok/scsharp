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
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Globalization;

using SdlDotNet;
using Tao.OpenGl;

namespace SdlDotNet.Examples.NeHe
{
	/// <summary>
	/// Lesson 10: Loading and Moving through a 3D World
	/// </summary>
	public class NeHe010 : NeHe008
	{
		#region Fields
		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 10: Loading and Moving through a 3D World";
			}
		}
		float xpos;
		float zpos;
		float heading;
		float walkbias;
		float walkbiasangle;
		float lookupdown;
		const float piover180 = 0.0174532925f;
		Sector sector;

		/// <summary>
		/// 
		/// </summary>
		protected float XPos
		{
			get
			{
				return xpos;
			}
			set
			{
				xpos = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected float ZPos
		{
			get
			{
				return zpos;
			}
			set
			{
				zpos = value;
			}
		}

		#endregion Fields

		#region Structs

		private struct Vertex 
		{
			public float x, y, z;
			public float u, v;
		}

		private struct Triangle 
		{
			public Vertex[] vertex;
		}

		private struct Sector 
		{
			public int numtriangles;
			public Triangle[] triangle;
		};

		#endregion Structs

		#region Constructor

		/// <summary>
		/// Basic Constructor
		/// </summary>
		public NeHe010()
		{
			this.TextureName = new string[1];
			this.TextureName[0] = "NeHe010.bmp";
			this.Texture = new int[3];
			this.DepthZ = 0;
		}

		#endregion Constructor

		#region Lesson Setup

		/// <summary>
		/// Initialize OpenGL
		/// </summary>
		protected override void InitGL()
		{
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			Keyboard.EnableKeyRepeat(75,25);
			this.LoadGLFilteredTextures();
			// Enable Texture Mapping
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			// Set The Blending Function For Translucency
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE);
			// This Will Clear The Background Color To Black
			Gl.glClearColor(0, 0, 0, 0);
			// Enables Clearing Of The Depth Buffer
			Gl.glClearDepth(1);
			// The Type Of Depth Test To Do
			Gl.glDepthFunc(Gl.GL_LESS);
			// Enables Depth Testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			// Enables Smooth Color Shading
			Gl.glShadeModel(Gl.GL_SMOOTH);
			// Really Nice Perspective Calculations
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
			this.SetupWorld();
		}

		#region SetupWorld()

		/// <summary>
		/// Loads and parses the world.
		/// </summary>
		/// <returns>
		/// <c>true</c> on successful load, otherwise <c>false</c>.
		/// </returns>
		private void SetupWorld() 
		{
			// This Method Is Pretty Ugly.  The .Net Framework Doesn't 
			// Have An Analogous Implementation
			// Of C/C++'s sscanf().  As Such You Have To Manually 
			// Parse A File, You Can Either Do So
			// Procedurally Like I'm Doing Here, Or Use Some RegEx's.  
			// To Make It A Bit Easier I Modified
			// The World.txt File To Remove Comments, Empty Lines And 
			// Excess Spaces.  Sorry For The
			// Ugliness, I'm Too Lazy To Clean It Up.
			float x, y, z, u, v;
   
			// Local Vertex Information
			int numtriangles;
  
			// Local Number Of Triangles
			string oneline = "";
   
			// The Line We've Read
			string[] splitter;
 
			// Array For Split Values
			StreamReader reader = null;

			// Our StreamReader
			ASCIIEncoding encoding = new ASCIIEncoding();
   
			// ASCII Encoding
			string fileName = @"NeHe010.World.txt";   
			// The File To Load
			string fileName1 = string.Format(CultureInfo.CurrentCulture,"Data{0}{1}",  
				// Look For Data\Filename
				Path.DirectorySeparatorChar, fileName);

			string fileName2 = string.Format(CultureInfo.CurrentCulture,"{0}{1}{0}{1}Data{1}{2}",  
				// Look For ..\..\Data\Filename
				"..", Path.DirectorySeparatorChar, fileName);


			// Make Sure The File Exists In One Of The Usual Directories
			if(!File.Exists(fileName) && !File.Exists(fileName1) && !File.Exists(fileName2)) 
			{
				throw new FileNotFoundException(); 

			}

			if(File.Exists(fileName)) 
			{
				// Does The File Exist Here?
				//fileName = fileName;   
				// Do Nothing
			}
			else if(File.Exists(fileName1)) 
			{
				// Does The File Exist Here?
				fileName = fileName1;
  
				// Swap Filename
			}
			else if(File.Exists(fileName2)) 
			{
				// Does The File Exist Here?
				fileName = fileName2;
  
				// Swap Filename
			}

			// Open The File As ASCII Text
			reader = new StreamReader(fileName, encoding);
  
			// Read The First Line
			oneline = reader.ReadLine();

			// Split The Line On Spaces
			splitter = oneline.Split();

			// The First Item In The Array Will Contain The String "NUMPOLLIES",
			// Which We Will Ignore

			// Save The Number Of Triangles As An int
			numtriangles = Convert.ToInt32(splitter[1],CultureInfo.CurrentCulture);

			// Initialize The Triangles And Save To sector
			sector.triangle = new Triangle[numtriangles];
   
			// Save The Number Of Triangles In sector
			sector.numtriangles = numtriangles;
		
			// For Every Triangle
			for(int triloop = 0; triloop < numtriangles; triloop++) 
			{   
				// Initialize The Vertices In sector
				sector.triangle[triloop].vertex = new Vertex[3];

				// For Every Vertex
				for(int vertloop = 0; vertloop < 3; vertloop++) 
				{   
					// Read A Line
					oneline = reader.ReadLine();
 
					// If The Line Isn't null
					if(oneline != null) 
					{   
						// Split The Line On Spaces
						splitter = oneline.Split();
  
						// Save x As A float
						x = Single.Parse(splitter[0],CultureInfo.CurrentCulture);
  
						// Save y As A float
						y = Single.Parse(splitter[1],CultureInfo.CurrentCulture);
  
						// Save z As A float
						z = Single.Parse(splitter[2],CultureInfo.CurrentCulture);
  
						// Save u As A float
						u = Single.Parse(splitter[3],CultureInfo.CurrentCulture);
  
						// Save v As A float
						v = Single.Parse(splitter[4],CultureInfo.CurrentCulture);
  
						// Save x To sector's Current triangle's vertex x
						sector.triangle[triloop].vertex[vertloop].x = x;

						// Save y To sector's Current triangle's vertex y
						sector.triangle[triloop].vertex[vertloop].y = y;

						// Save z To sector's Current triangle's vertex z
						sector.triangle[triloop].vertex[vertloop].z = z;

						// Save u To sector's Current triangle's vertex u
						sector.triangle[triloop].vertex[vertloop].u = u;

						// Save v To sector's Current triangle's vertex v
						sector.triangle[triloop].vertex[vertloop].v = v;
					}
				}
			}
			if(reader != null) 
			{
				// Close The StreamReader
				reader.Close();
			}
		}
		#endregion SetupWorld()

		#endregion Lesson Setup

		#region Rendering

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
	
			float x_m, y_m, z_m, u_m, v_m;
			float xtrans = -xpos;
			float ztrans = -zpos;
			float ytrans = -walkbias - 0.25f;
			float sceneroty = 360 - this.RotationY;
			int numtriangles;

			Gl.glRotatef(lookupdown, 1, 0, 0);
			Gl.glRotatef(sceneroty, 0, 1, 0);
			Gl.glTranslatef(xtrans, ytrans, ztrans);
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[this.Filter]);
			numtriangles = sector.numtriangles;
			// Process Each Triangle
			for(int loop_m = 0; loop_m < numtriangles; loop_m++) 
			{
				Gl.glBegin(Gl.GL_TRIANGLES);
				Gl.glNormal3f(0, 0, 1);
				x_m = sector.triangle[loop_m].vertex[0].x;
				y_m = sector.triangle[loop_m].vertex[0].y;
				z_m = sector.triangle[loop_m].vertex[0].z;
				u_m = sector.triangle[loop_m].vertex[0].u;
				v_m = sector.triangle[loop_m].vertex[0].v;
				Gl.glTexCoord2f(u_m, v_m); Gl.glVertex3f(x_m, y_m, z_m);

				x_m = sector.triangle[loop_m].vertex[1].x;
				y_m = sector.triangle[loop_m].vertex[1].y;
				z_m = sector.triangle[loop_m].vertex[1].z;
				u_m = sector.triangle[loop_m].vertex[1].u;
				v_m = sector.triangle[loop_m].vertex[1].v;
				Gl.glTexCoord2f(u_m, v_m); Gl.glVertex3f(x_m, y_m, z_m);

				x_m = sector.triangle[loop_m].vertex[2].x;
				y_m = sector.triangle[loop_m].vertex[2].y;
				z_m = sector.triangle[loop_m].vertex[2].z;
				u_m = sector.triangle[loop_m].vertex[2].u;
				v_m = sector.triangle[loop_m].vertex[2].v;
				Gl.glTexCoord2f(u_m, v_m); Gl.glVertex3f(x_m, y_m, z_m);
				Gl.glEnd();
			}
		}
		#endregion bool DrawGLScene()

		#endregion Rendering

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.F:
					this.Filter += 1;
					if(this.Filter > 2) 
					{
						this.Filter = 0;
					}
					break;
				case Key.PageUp:
					this.DepthZ -= 0.02f;
					this.lookupdown -= 1.0f;
					break;
				case Key.PageDown:
					this.DepthZ += 0.02f;
					this.lookupdown+= 1.0f;
					break;
				case Key.UpArrow: 
					xpos -= (float) Math.Sin(heading * piover180) * 0.05f;
					zpos -= (float) Math.Cos(heading * piover180) * 0.05f;
					if(walkbiasangle >= 359) 
					{
						walkbiasangle = 0;
					}
					else 
					{
						walkbiasangle += 10;
					}
					walkbias = (float) Math.Sin(walkbiasangle * piover180) / 20;
					break;
				case Key.DownArrow:
					xpos += (float) Math.Sin(heading * piover180) * 0.05f;
					zpos += (float) Math.Cos(heading * piover180) * 0.05f;
					if(walkbiasangle <= 1) 
					{
						walkbiasangle = 359;
					}
					else 
					{
						walkbiasangle -= 10;
					}
					walkbias = (float) Math.Sin(walkbiasangle * piover180) / 20;
					break;
				case Key.RightArrow:
					heading -= 1;
					this.RotationY = heading;
					break;
				case Key.LeftArrow:
					heading += 1;
					this.RotationY = heading;
					break;
				case Key.B:
					this.Blend = !this.Blend;
					if(this.Blend) 
					{
						// Turn Blending On
						Gl.glEnable(Gl.GL_BLEND);
						// Turn Depth Testing Off
						Gl.glDisable(Gl.GL_DEPTH_TEST);
					}
					else 
					{
						// Turn Blending Off
						Gl.glDisable(Gl.GL_BLEND);
						// Turn Depth Testing On
						Gl.glEnable(Gl.GL_DEPTH_TEST);
					}
					break;
			}
		}

		#endregion Event Handlers
	}
}
