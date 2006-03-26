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
	/// Lesson 25: Morphing and Loading Objects from a File
	/// </summary>
	public class NeHe025 : NeHe020
	{
		#region Fields

		/// <summary>
		/// Lesson Title
		/// </summary>
		public new static string Title
		{
			get
			{
				return "Lesson 25: Morphing and Loading Objects from a File";
			}
		}

		float zspeed;   
		float ypos;

		/// <summary>
		/// 
		/// </summary>
		public float YPos
		{
			get
			{
				return ypos;
			}
			set
			{
				ypos = value;
			}
		}
		Random rand = new Random();

		// Make Sure Same Morph Key Is Not Pressed
		int key = 1;
		// Step Counter
		int step;   
		// Maximum Number Of Steps
		int steps = 200;
		// Morphing?

		bool morph; 
		// Maximum Number Of Vertices
		int maxver; 
		// Our 4 Morphable Objects
		Thing morph1, morph2, morph3, morph4;   
		Thing helper, source, destination;
		
		#endregion Fields

		#region Structs

		// Structure For 3d Points
		private struct Vertex 
		{  
			// X Coordinate
			public float X;
			// Y Coordinate
			public float Y;
			// Z Coordinate
			public float Z;
		}

		// Structure For An Object
		private struct Thing 
		{ 
			// Number Of Vertices For The Object
			public int Verts; 
			// Vertices
			public Vertex[] Points;
		}

		#endregion Structs

		#region Constructor

		/// <summary>
		/// Basic constructor
		/// </summary>
		public NeHe025()
		{
			this.ZPos = -15;
		}
		
		#endregion Constructor

		#region Lesson Setup

		/// <summary>
		/// Initialize OpenGL
		/// </summary>
		protected override void InitGL()
		{
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyDown);
			Keyboard.EnableKeyRepeat(150,50);

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

			// Sets Max Vertices To 0 By Default
			maxver = 0;

			// Load The First Object Into morph1 From File sphere.txt
			LoadThing("NeHe025.Sphere.txt", ref morph1);  
			// Load The Second Object Into morph2 From File torus.txt
			LoadThing("NeHe025.Torus.txt", ref morph2);   
			// Load The Third Object Into morph3 From File tube.txt
			LoadThing("NeHe025.Tube.txt", ref morph3);

			// Manually Reserver Ram For A 4th 468 Vertice Object (morph4)
			AllocateThing(ref morph4, 486);  
			// Loop Through All 468 Vertices
			for(int i = 0; i < 486; i++) 
			{  
				// morph4 X Point Becomes A Random Float Value From -7 to 7
				morph4.Points[i].X = ((float) (rand.Next() % 14000) / 1000) - 7;
				// morph4 Y Point Becomes A Random Float Value From -7 to 7
				morph4.Points[i].Y = ((float) (rand.Next() % 14000) / 1000) - 7;
				// morph4 Z Point Becomes A Random Float Value From -7 to 7
				morph4.Points[i].Z = ((float) (rand.Next() % 14000) / 1000) - 7;
			}

			// Load sphere.txt Object Into Helper (Used As Starting Point)
			LoadThing("NeHe025.Sphere.txt", ref helper);
			// Source & Destination Are Set To Equal First Object (morph1)
			source = destination = morph1;
		}

		#region LoadThing(string filename, ref Thing k)
		/// <summary>
		/// Loads Object from a file.
		/// </summary>
		/// <param name="filename">
		/// The file to load.
		/// </param>
		/// <param name="k">
		/// The Object to save to.
		/// </param>
		private void LoadThing(string filename, ref Thing k) 
		{
			// Will Hold Vertice Count
			int ver;  
			// Hold Vertex X, Y & Z Position
			float rx, ry, rz;
			// The Line We've Read
			string oneline = "";
			// Array For Split Values
			string[] splitter;
			// Our StreamReader
			StreamReader reader = null;
			// ASCII Encoding
			ASCIIEncoding encoding = new ASCIIEncoding();
			
			try 
			{
				// Make Sure A Filename Was Given
				if(filename == null || filename.Length == 0) 
				{  
					// If Not Return
					return; 
				}

				// Look For Data\Filename
				string fileName1 = string.Format(CultureInfo.CurrentCulture,"Data{0}{1}", 
					Path.DirectorySeparatorChar, filename);
				// Look For ..\..\Data\Filename
				string fileName2 = string.Format(CultureInfo.CurrentCulture,"{0}{1}{0}{1}Data{1}{2}",
					"..", Path.DirectorySeparatorChar, filename);

				// Make Sure The File Exists In One Of The Usual Directories
				if(!File.Exists(filename) && !File.Exists(fileName1) && !File.Exists(fileName2)) 
				{
					// If Not Return Null
					return;
					
				}

				// Does The File Exist Here?
				if(File.Exists(filename)) 
				{
					// Open The File As ASCII Text
					reader = new StreamReader(filename, encoding); 
				}
					// Does The File Exist Here?
				else if(File.Exists(fileName1)) 
				{
					// Open The File As ASCII Text
					reader = new StreamReader(fileName1, encoding);
				}
					// Does The File Exist Here?
				else if(File.Exists(fileName2)) 
				{
					// Open The File As ASCII Text
					reader = new StreamReader(fileName2, encoding);
				}

				// Read The First Line
				oneline = reader.ReadLine();
				// Split The Line On Spaces
				splitter = oneline.Split();

				// The First Item In The Array 
				// Will Contain The String "Vertices:", Which We Will Ignore
				// Save The Number Of Triangles To ver As An int
				ver = Convert.ToInt32(splitter[1],CultureInfo.CurrentCulture);
				// Sets PointObjects (k) verts Variable To Equal The Value Of ver
				k.Verts = ver;
				// Jumps To Code That Allocates Ram To Hold The Object
				AllocateThing(ref k, ver); 

				// Loop Through The Vertices
				for(int vertloop = 0; vertloop < ver; vertloop++) 
				{ 
					// Reads In The Next Line Of Text
					oneline = reader.ReadLine();
					// If The Line's Not null
					if(oneline != null) 
					{
						// Split The Line On Spaces
						splitter = oneline.Split();
						// Save The X Value As A Float
						rx = float.Parse(splitter[0],CultureInfo.CurrentCulture);
						// Save The Y Value As A Float			
						ry = float.Parse(splitter[1],CultureInfo.CurrentCulture);
						// Save The Z Value As A Float
						rz = float.Parse(splitter[2],CultureInfo.CurrentCulture);
						// Sets PointObjects (k) points.x Value To rx
						k.Points[vertloop].X = rx;
						// Sets PointObjects (k) points.y Value To ry
						k.Points[vertloop].Y = ry;
						// Sets PointObjects (k) points.z Value To rz
						k.Points[vertloop].Z = rz;
					}
				}

				// If ver Is Greater Than maxver
				// maxver Keeps Track Of The Highest Number Of 
				// Vertices Used In Any Of The Objects
				if(ver > maxver) 
				{ 
					// Set maxver Equal To ver
					maxver = ver;  
				}
			}
			catch(Exception e) 
			{
				// Handle Any Exceptions While Loading Object Data, Exit App
				string errorMsg = "An Error Occurred While Loading And Parsing Object Data:\n\t" + filename + "\n" + "\n\nStack Trace:\n\t" + e.StackTrace + "\n";
				MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
				throw;
			}
			finally 
			{
				if(reader != null) 
				{
					// Close The StreamReader
					reader.Close();
				}
			}
		}
		#endregion LoadThing(string filename, ref Thing k)

		#region AllocateThing(ref Thing thing, int number)
		/// <summary>
		/// Allocate memory for each object.
		/// </summary>
		/// <param name="thing">
		/// The object.
		/// </param>
		/// <param name="number">
		/// The number of points to allocate.
		/// </param>
		private void AllocateThing(ref Thing thing, int number) 
		{
			thing.Points = new Vertex[number];
		}
		#endregion AllocateThing(ref Thing thing, int number)

		#endregion Lesson Setup

		#region Render

		#region DrawGLScene()
		/// <summary>
		///  Renders the scene
		/// </summary>
		protected override void DrawGLScene() 
		{ 
			// Clear The Screen And The Depth Buffer
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			// Reset The View
			Gl.glLoadIdentity();
			// Translate The The Current Position To Start Drawing
			Gl.glTranslatef(this.XPos, this.YPos, this.ZPos);  
			// Rotate On The X Axis By xrot
			Gl.glRotatef(this.RotationX, 1, 0, 0);
			// Rotate On The Y Axis By yrot
			Gl.glRotatef(this.RotationY, 0, 1, 0);
			// Rotate On The Z Axis By zrot
			Gl.glRotatef(this.RotationZ, 0, 0, 1);

			// Increase xrot,yrot & zrot by xspeed, yspeed & zspeed
			this.RotationX += this.XSpeed;
			this.RotationY += this.YSpeed;
			this.RotationZ += zspeed;

			// Temp X, Y & Z Variables
			float tx, ty, tz;
			// Holds Returned Calculated Values For One Vertex
			Vertex q;

			// Begin Drawing Points
			Gl.glBegin(Gl.GL_POINTS); 
			// Loop Through All The Verts Of morph1 (All Objects Have
			for(int i = 0; i < morph1.Verts; i++) 
			{
				// If morph Is True Calculate Movement Otherwise Movement=0
				if(morph) 
				{
					// The Same Amount Of Verts For Simplicity, Could Use maxver Also)
					q = Calculate(i);
				}
				else 
				{
					q.X = q.Y = q.Z = 0;
				}
				// Subtract q.x Units From helper.points[i].x (Move On X Axis)
				helper.Points[i].X -= q.X; 
				// Subtract q.y Units From helper.points[i].y (Move On Y Axis)
				helper.Points[i].Y -= q.Y;
				// Subtract q.z Units From helper.points[i].z (Move On Z Axis)
				helper.Points[i].Z -= q.Z;
				// Make Temp X Variable Equal To Helper's X Variable
				tx = helper.Points[i].X;
				// Make Temp Y Variable Equal To Helper's Y Variable
				ty = helper.Points[i].Y;
				// Make Temp Z Variable Equal To Helper's Z Variable
				tz = helper.Points[i].Z;

				// Set Color To A Bright Shade Of Off Blue
				Gl.glColor3f(0, 1, 1); 
				// Draw A Point At The Current Temp Values (Vertex)
				Gl.glVertex3f(tx, ty, tz); 
				// Darken Color A Bit
				Gl.glColor3f(0, 0.5f, 1);
				// Calculate Two Positions Ahead
				tx -= 2 * q.X;
				ty -= 2 * q.Y;
				ty -= 2 * q.Y;
				// Draw A Second Point At The Newly Calculate Position
				Gl.glVertex3f(tx, ty, tz); 
				// Set Color To A Very Dark Blue
				Gl.glColor3f(0, 0, 1);
				tx -= 2 * q.X;
				ty -= 2 * q.Y;
				ty -= 2 * q.Y;
				// Calculate Two More Positions Ahead
				Gl.glVertex3f(tx, ty, tz);
				// Draw A Third Point At The Second New Position
				// This Creates A Ghostly Tail As Points Move
			
			}
			// Done Drawing Points
			Gl.glEnd();
			
			// If We're Morphing And We Haven't 
			// Gone Through All 200 Steps Increase Our Step Counter
			// Otherwise Set Morphing To False, 
			// Make Source=Destination And Set The Step Counter Back To Zero.
			if(morph && step <= steps) 
			{
				step++;
			}
			else 
			{
				morph = false;
				source = destination;
				step = 0;
			}
		}
		#endregion DrawGLScene()

		#region Vertex Calculate(int i)
		/// <summary>
		/// Calculates movement of points during morphing.
		/// </summary>
		/// <param name="i">
		/// The number of the point to calculate.
		/// </param>
		/// <returns>
		/// A Vertex.
		/// </returns>
		private Vertex Calculate(int i) 
		{
			// This Makes Points Move At A Speed So They All Get To 
			// Their Destination At The Same Time
			// Temporary Vertex Called a
			Vertex a;
			// a.X Value Equals Source X - Destination X Divided By Steps
			a.X = (source.Points[i].X - destination.Points[i].X) / steps;
			// a.Y Value Equals Source Y - Destination Y Divided By Steps
			a.Y = (source.Points[i].Y - destination.Points[i].Y) / steps;
			// a.Z Value Equals Source Z - Destination Z Divided By Steps
			a.Z = (source.Points[i].Z - destination.Points[i].Z) / steps;
			// Return The Results
			return a; 
		}
		#endregion Vertex Calculate(int i)

		#endregion Render

		#region Event Handlers

		private void KeyDown(object sender, KeyboardEventArgs e)
		{
			switch (e.Key) 
			{
				case Key.PageUp:
					this.zspeed -= 0.01f;
					break;
				case Key.PageDown:
					this.zspeed += 0.01f;
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
				case Key.Q:
					this.ZPos -= 0.01f;
					break;
				case Key.Z:
					this.ZPos += 0.01f;
					break;				
				case Key.W:
					this.ypos += 0.01f;
					break;
				case Key.S:
					this.ypos -= 0.01f;
					break;
				case Key.D:
					this.XPos += 0.01f;
					break;
				case Key.A:
					this.XPos -= 0.01f;
					break;
				case Key.One:
					if (key != 1 && !morph)
					{
						key = 1;
						morph = true;
						destination = morph1;
					}
					break;
				case Key.Two:
					if (key != 2 && !morph)
					{
						key = 2;
						morph = true;
						destination = morph2;
					}
					break;
				case Key.Three:
					if (key != 3 && !morph)
					{
						key = 3;
						morph = true;
						destination = morph3;
					}
					break;
				case Key.Four:
					if (key != 4 && !morph)
					{
						key = 4;
						morph = true;
						destination = morph4;
					}
					break;
			}
		}

		#endregion Event Handlers
	}
}