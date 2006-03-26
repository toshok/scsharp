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
using System.IO;

using Tao.PhysFs;
using SdlDotNet;

namespace SdlDotNet.Examples.PhysFsTest
{
	/// <summary>
	/// Summary description for PhysFsTest.
	/// </summary>
	public class PhysFsTest : IDisposable
	{
		private Surface surf;
		string data_directory = @"Data/";
		string filepath = @"../../";


		/// <summary>
		/// 
		/// </summary>
		public PhysFsTest()
		{
			// setup SDL.NET
			Video.WindowIcon();
			Video.WindowCaption = "SDL.NET - PhysFsTest";
			Video.SetVideoModeWindow(400, 300);
			Events.KeyboardDown += new KeyboardEventHandler(this.KeyboardDown);
			Events.Tick += new TickEventHandler(this.Tick);
			Events.Quit += new QuitEventHandler(this.Quit);
			Events.Fps = 30;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Run()
		{
			if (File.Exists(data_directory + "data.zip"))
			{
				filepath = "";
			}

			// Initiate PhysFS
			Fs.PHYSFS_init("init");

			// Allow PhysFS to look in data.zip for files
			Fs.PHYSFS_addToSearchPath(filepath + data_directory + "data.zip", 1);

			// Open surface from zip
			IntPtr imageFile = Fs.PHYSFS_openRead("sdldotnet_full.png");

			// Read it into a byte array
			byte[] imageBytes;
			Fs.PHYSFS_read(imageFile, out imageBytes, 1, (uint)Fs.PHYSFS_fileLength(imageFile));
			surf = new Surface(imageBytes);

			// close the file
			Fs.PHYSFS_close(imageFile);

			Events.Run();
		}

		/// <summary>
		/// 
		/// </summary>
		public static void Main()
		{
			PhysFsTest app = new PhysFsTest();
			app.Run();
		}

		private void KeyboardDown(object sender, KeyboardEventArgs e)
		{
			if(e.Key == Key.Escape)
			{
				Fs.PHYSFS_deinit();
				Events.QuitApplication();
			}
		}

		private void Tick(object sender, TickEventArgs e)
		{
			Video.Screen.Fill(Color.White);
         
			// render the image
			Point location = new Point(Video.Screen.Width / 2 - surf.Width / 2, Video.Screen.Height / 2 - surf.Height / 2);
			Video.Screen.Blit(surf, location);

			Video.Screen.Flip();
		}

		private void Quit(object sender, QuitEventArgs e)
		{
			Events.QuitApplication();
		}
		#region IDisposable Members

		private bool disposed;

		/// <summary>
		/// Closes and destroys this object
		/// </summary>
		/// <remarks>Destroys managed and unmanaged objects</remarks>
		public void Dispose() 
		{
			Dispose(true);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					surf.Dispose();
				}
				this.disposed = true;
			}
		}

		#endregion
	}
} 

