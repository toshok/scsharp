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
using System.Collections;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Forms;
using Tao.FreeGlut;

namespace SdlDotNet.Examples.RedBook
{
	/// <summary>
	/// Summary description for RedBook.
	/// </summary>
	public class RedBook : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox lstExamples;
		private System.Windows.Forms.Button startButton;
		private System.Collections.ArrayList redBookTypes = new ArrayList();
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuExit;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// 
		/// </summary>
		public RedBook()
		{
			//
			// Required for Windows Form Designer support
			//
			Glut.glutInit();
			InitializeComponent();
		}

		private bool disposed;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			try
			{
				if (!this.disposed)
				{
					if (disposing)
					{
						if(components != null)
						{
							components.Dispose();
						}
					}
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lstExamples = new System.Windows.Forms.ListBox();
			this.startButton = new System.Windows.Forms.Button();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuExit = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// lstExamples
			// 
			this.lstExamples.Location = new System.Drawing.Point(8, 8);
			this.lstExamples.Name = "lstExamples";
			this.lstExamples.Size = new System.Drawing.Size(360, 342);
			this.lstExamples.Sorted = true;
			this.lstExamples.TabIndex = 0;
			this.lstExamples.DoubleClick += new System.EventHandler(this.startButton_Click);
			// 
			// startButton
			// 
			this.startButton.Location = new System.Drawing.Point(144, 360);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 1;
			this.startButton.Text = "Start Demo";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuExit});
			this.menuItem1.Text = "File";
			// 
			// menuExit
			// 
			this.menuExit.Index = 0;
			this.menuExit.Text = "Exit";
			this.menuExit.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// RedBook
			// 
			this.AcceptButton = this.startButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(378, 405);
			this.Controls.Add(this.startButton);
			this.Controls.Add(this.lstExamples);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Menu = this.mainMenu1;
			this.Name = "RedBook";
			this.Text = "SDL.NET - RedBook OpenGL Examples";
			this.Load += new System.EventHandler(this.RedBook_Load);
			this.Closed += new System.EventHandler(this.RedBook_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		private void RedBook_Load(object sender, System.EventArgs e)
		{
			// Load app.ico as the form icon.
			Assembly asm = Assembly.GetExecutingAssembly();
			string iconName = "";
			foreach (string s in asm.GetManifestResourceNames())
			{
				if (s.EndsWith("App.ico"))
				{
					iconName = s;
					break;
				}
			}
			if (iconName.Length > 0)
			{
				this.Icon = new Icon(asm.GetManifestResourceStream(iconName));
			}

			// Get the RedBook examples.
			Type[] types = asm.GetTypes();

			foreach(Type type in types)
			{
				// RedBookXXX
				if(type.Name.StartsWith("RedBook") && type.Name.Length > 7)
				{
					try
					{
						// Get the title of the RedBook example class
						object result = type.InvokeMember("Title",
							BindingFlags.GetProperty, null, type, null);

						// Add the example to the array and display it on the listbox
						lstExamples.Items.Add((string)result);
						redBookTypes.Add(type);
					}
					catch(System.MissingMethodException)
					{
						// RedBook demo missing static Title property - do nothing
					}
				}
			}
		}

		private void RunDemo()
		{
			try
			{
				object dynObj;
				// Get the desired RedBook example type.
				Type dynClassType = (Type)redBookTypes[lstExamples.SelectedIndex];

				// Make an instance of it.
				dynObj = Activator.CreateInstance(dynClassType);
				if(dynObj != null)
				{
					// Make the SDL window appear on top of this form.
					this.SendToBack();
					MethodInfo invokedMethod = dynClassType.GetMethod("Run");
					invokedMethod.Invoke(dynObj, null);
				}
			}
			catch(System.Reflection.TargetInvocationException)
			{
				// User changed demo - do nothing
			}
			catch(System.ArgumentOutOfRangeException)
			{
			}
			catch(System.MissingMethodException)
			{
				// missing method - do nothing
			}
		}

		System.Threading.Thread thread;

		private void startButton_Click(object sender, System.EventArgs e)
		{
			SdlDotNet.Events.QuitApplication();
				
			if (thread != null)
			{
				thread.Abort();	
			}

			thread = new System.Threading.Thread(new System.Threading.ThreadStart(RunDemo));
			thread.Priority = System.Threading.ThreadPriority.Normal;
			thread.IsBackground = true;
			thread.Name = "SDL.NET - Demo Thread";
			thread.Start();
		}

		/// <summary>
		/// 
		/// </summary>
		[STAThread]
		public static void Main()
		{
			try
			{
				Application.Run(new RedBook());
			}
			catch(System.ObjectDisposedException)
			{
				Console.WriteLine("HELLO");
				Application.Exit();
			}
		}

		private void RedBook_Closed(object sender, System.EventArgs e)
		{
			// Quit SDL if it's not quit already
			SdlDotNet.Events.QuitApplication();
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
