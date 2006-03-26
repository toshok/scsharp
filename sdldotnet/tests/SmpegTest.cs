using System;
using System.Threading;
using NUnit.Framework;
using Tao.Sdl;
using System.Runtime.InteropServices;

namespace SdlDotNet.Tests
{
	#region Smpeg
	/// <summary>
	/// SDL Tests.
	/// </summary>
	[TestFixture]
	public class SmpegTest
	{
		//int flags = (Sdl.SDL_HWSURFACE|Sdl.SDL_DOUBLEBUF|Sdl.SDL_ANYFORMAT);
		//int bpp = 16;
		//int width = 640;
		//int height = 480;
		//IntPtr surfacePtr;
		//Sdl.SDL_Rect rect2;
		//int sleepTime = 1000;
		//short[] vx = {40, 80, 130, 80, 40};
		//short[] vy = {80, 40, 80, 130, 130};
		//byte[] src1 = {1,2,3,4};
		//byte[] src2 = {2,10,20,40};
		//byte[] dest = new byte[4];
		//Smpeg.SMPEG_Info info = new Smpeg.SMPEG_Info();
		
		/// <summary>
		/// 
		/// </summary>
		[SetUp]
		public void Init()
		{
			Sdl.SDL_Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		private void InitSdl()
		{
			Sdl.SDL_Quit();
			int init = Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			this.SmpegSetup();
			
		}
		private void SmpegSetup()
		{
			//surfacePtr = Sdl.SDL_SetVideoMode(
			//	width, 
			//	height, 
			//	bpp, 
			//	flags);
		}
		/// <summary>
		/// 
		/// </summary>
		private void Quit()
		{
			Sdl.SDL_Quit();
		}

		#region smpeg.h
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SMPEG_new()
		{
			this.InitSdl();

			//IntPtr intPtr = Smpeg.SMPEG_new("test.mpg", out info, 0); 
			//IntPtr intPtr = Sdl.SDL_RWFromFile("test.mpg", "rb");
			//IntPtr intPtr = Smpeg.SMPEG_new("test.mpg", out info, 0); 
			//Console.WriteLine("Smpeg_error: " + Smpeg.SMPEG_error(intPtr));
			//Assert.IsFalse(intPtr == IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SMPEG_new_rwops()
		{
			this.InitSdl();

			//IntPtr intPtr = Smpeg.SMPEG_new("test.mpg", out info, 0); 
			//IntPtr intPtr = Sdl.SDL_RWFromFile("test.mpg", "rb");
			//IntPtr intPtr = Smpeg.SMPEG_new_rwops(Sdl.SDL_RWFromFile("test.mpg", "rb"), out info, 0); 
			//Console.WriteLine("Smpeg_error: " + Smpeg.SMPEG_error(intPtr));
			//Assert.IsFalse(intPtr == IntPtr.Zero);
			this.Quit();
		}
//		public event Smpeg.SMPEG_DisplayCallback callbackEvent;
//		callbackEvent += new Smpeg.SMPEG_DisplayCallback(
//		private void Update(IntPtr surfacePtr, int x, int y, int w, int h)
//		{
//			Sdl.SDL_Flip(surfacePtr);
//		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SMPEG_play()
		{
			this.InitSdl();

			//IntPtr intPtr = Smpeg.SMPEG_new("test.mpg", out info, 0); 
			//IntPtr intPtr = Sdl.SDL_RWFromFile("test.mpg", "rb");
			//IntPtr intPtr = Smpeg.SMPEG_new("test.mpg", out info, 0); 
			//Console.WriteLine("Smpeg_error: " + Smpeg.SMPEG_error(intPtr));
			//Assert.IsFalse(intPtr == IntPtr.Zero);
			//Smpeg.SMPEG_enableaudio(intPtr, 1);
			//Smpeg.SMPEG_enablevideo(intPtr, 1);
			//Smpeg.SMPEG_setvolume(intPtr, 100);
			//Smpeg.SMPEG_setdisplay(intPtr, surfacePtr, IntPtr.Zero, null);

			//Smpeg.SMPEG_play(intPtr);
			//while (Smpeg.SMPEG_status(intPtr) == Smpeg.SMPEG_PLAYING){}
			//Thread.Sleep(sleepTime);
			//Smpeg.SMPEG_stop(intPtr);
			//Smpeg.SMPEG_delete(intPtr);
			this.Quit();
		}
		#endregion smpeg.h

		#region MPEGfilter.h
		#endregion MPEGfilter.h
	}
	#endregion SDL_gfx.h
}
