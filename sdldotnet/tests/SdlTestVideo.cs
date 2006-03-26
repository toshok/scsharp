using System;
using System.Threading;
using NUnit.Framework;
using Tao.Sdl;
using SdlDotNet;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SdlDotNet.Tests
{
	#region SDL_video.h
	/// <summary>
	/// SDL Tests.
	/// </summary>
	[TestFixture]
	public class SdlTestVideo
	{
		//int init;
		int bpp;
		int width;
		int height;

		/// <summary>
		/// 
		/// </summary>
		[SetUp]
		public void Init()
		{
			Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO);
			//flags = (Sdl.SDL_HWSURFACE|Sdl.SDL_DOUBLEBUF|Sdl.SDL_ANYFORMAT);
			bpp = 16;
			width = 640;
			height = 480;
			//surfacePtr = IntPtr.Zero;
			//Sdl.SDL_FreeSurfaceInternal(surfacePtr);
		}
		private void VideoSetup()
		{
			this.VideoSetup(this.bpp);
		}
		/// <summary>
		/// 
		/// </summary>
		private void VideoSetup(int bpp)
		{
			Video.Close();
			Video.SetVideoModeWindow(this.width, this.height, bpp); 
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void DriverName()
		{
			string buffer="";
			string driverName = Sdl.SDL_VideoDriverName(buffer, 100);
			//Console.WriteLine("Video: "+ driverName);
			Assert.IsNotNull(driverName);
		}

//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void CreateRGBSurfaceAndFree()
//		{
//			int rmask = 0x00000000;
//			int gmask = 0x00ff0000;
//			int bmask = 0x0000ff00;
//			int amask = 0x000000ff;
//			IntPtr surfacePtr = VideoSetup();
//			IntPtr rgbSurfacePtr = Sdl.SDL_CreateRGBSurface(
//				flags, 
//				width, 
//				height, 
//				bpp, 
//				rmask,
//				gmask, 
//				bmask, 
//				amask);
//			Sdl.SDL_Surface rgbSurface = 
//				(Sdl.SDL_Surface)Marshal.PtrToStructure(rgbSurfacePtr, typeof(Sdl.SDL_Surface));
//			Assert.IsTrue(rgbSurface.w == width);
//			Assert.IsTrue(rgbSurface.h == height);
//			//Sdl.SDL_FreeSurface(surfacePtr);
//			Sdl.SDL_FreeSurface(rgbSurfacePtr);
//			//Sdl.SDL_FreeSurfaceInternal(rgbSurfacePtr);
//			Assert.AreEqual(IntPtr.Zero, rgbSurfacePtr);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}

//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void FillRectAndFlip()
//		{
//			IntPtr surfacePtr = VideoSetup();
//			Assert.IsNotNull(surfacePtr);
//			Sdl.SDL_Rect rect = new Sdl.SDL_Rect(100,100,100,100);
//			int result = Sdl.SDL_FillRect(surfacePtr, ref rect, 10000);
//			int resultFlip = Sdl.SDL_Flip(surfacePtr);
//			Thread.Sleep(sleepTime);
//
//			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(150,150,150,150);
//			int result2 = Sdl.SDL_FillRect(surfacePtr, ref rect2, 1000);
//			Assert.AreEqual(result2, 0); 
//			resultFlip = Sdl.SDL_Flip(surfacePtr);
//			Assert.AreEqual(resultFlip, 0); 
//			Thread.Sleep(sleepTime);
//
//			int result3 = Sdl.SDL_FillRect(surfacePtr, ref rect, 5000);
//			Assert.AreEqual(result3, 0); 
//			resultFlip = Sdl.SDL_Flip(surfacePtr);
//			
//			Assert.AreEqual(resultFlip, 0); 
//			Thread.Sleep(sleepTime);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void MustLock()
//		{
//			IntPtr surfacePtr = VideoSetup();
//			int status = Sdl.SDL_MUSTLOCK(surfacePtr);
//			//Console.WriteLine("MUSTLOCK: " + status.ToString());
//			Assert.IsTrue(status == 0);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void GetVideoSurface()
//		{
//			IntPtr surfacePtr = VideoSetup();
//			IntPtr videoPtr = Sdl.SDL_GetVideoSurface();
//			Assert.IsNotNull(videoPtr);
//			Sdl.SDL_FreeSurface(videoPtr);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		 }
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GetVideoInfo()
		{
			IntPtr videoInfoPtr = Sdl.SDL_GetVideoInfo();
			Assert.IsNotNull(videoInfoPtr);

			Sdl.SDL_VideoInfo videoInfo = (Sdl.SDL_VideoInfo)
				Marshal.PtrToStructure(videoInfoPtr, 
				typeof(Sdl.SDL_VideoInfo));

			Sdl.SDL_FreeSurface(videoInfoPtr);
		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void UpdateRect()
//		{
//			//TODO: Must figure out a real test for this method.
//			IntPtr surfacePtr = VideoSetup();
//			Sdl.SDL_UpdateRect(surfacePtr, 0, 0, 0, 0);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void UpdateRects()
//		{
//			//TODO: Must figure out a real test for this method.
//			IntPtr surfacePtr = VideoSetup();
//			Sdl.SDL_Rect rect = new Sdl.SDL_Rect(100,100,100,100);
//			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(150,150,150,150);
//			Sdl.SDL_Rect[] rects = {
//									   new Sdl.SDL_Rect(100,100,100,100), 
//									   new Sdl.SDL_Rect(150,150,150,150)
//								   };
//			Sdl.SDL_UpdateRects(surfacePtr, rects.Length, rects);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void SetGamma()
//		{
//			IntPtr surfacePtr = VideoSetup();
//			int resultSetGamma = Sdl.SDL_SetGamma(2,2,2);
//			Assert.AreEqual(resultSetGamma, 0); 
//			Thread.Sleep(sleepTime);
//			resultSetGamma = Sdl.SDL_SetGamma(1,1,1);
//			Assert.AreEqual(resultSetGamma, 0); 
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void WM_SetGetCaption()
//		{
//			IntPtr surfacePtr = VideoSetup();
//			string title;
//			string iconText;
//			Sdl.SDL_WM_SetCaption("Hi There", "Hello");
//			Sdl.SDL_WM_GetCaption(out title, out iconText);
//			Assert.AreEqual(title, "Hi There");
//			Assert.AreEqual(iconText, "Hello");
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void WM_IconifyWindow()
//		{
//			IntPtr surfacePtr = VideoSetup();
//			int result = Sdl.SDL_WM_IconifyWindow();
//			Assert.AreEqual(result, 1);
//			Thread.Sleep(sleepTime);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void WM_ToggleFullScreen()
//		{
//			IntPtr surfacePtr = VideoSetup();
//			int result = Sdl.SDL_WM_ToggleFullScreen(surfacePtr);
//#if WIN32
//			Assert.AreEqual(result, 0);
//#else
//			Assert.AreEqual(result, 1);
//#endif
//			Thread.Sleep(sleepTime);
//			result = Sdl.SDL_WM_ToggleFullScreen(surfacePtr);
//#if WIN32
//			Assert.AreEqual(result, 0);
//#else
//			Assert.AreEqual(result, 1);
//#endif
//			Sdl.SDL_FreeSurface(surfacePtr);
//			
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void GetGammaRamp()
//		{
//			IntPtr surfacePtr = VideoSetup();
//			short[] red = new short[256];
//			short[] blue = new short[256];
//			short[] green = new short[256];
//			int resultGetGammaRamp= Sdl.SDL_GetGammaRamp( red, green, blue);
//			Assert.AreEqual(resultGetGammaRamp, 0); 
//		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SetGammaRamp()
		{
			int resultSetGammaRamp= Sdl.SDL_SetGammaRamp( null, null, null);
			Assert.AreEqual(resultSetGammaRamp, 0); 
		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void SetColors()
//		{
//			Sdl.SDL_Color[] colors = new Sdl.SDL_Color[255];
//			for(byte i=0;i<=254;i++)
//			{
//				colors[i].r=i;
//				colors[i].g=i;
//				colors[i].b=i;
//			}
//			IntPtr surfacePtr = VideoSetup();
//			int resultSetColors = Sdl.SDL_SetColors(surfacePtr, colors, 0, 255);
//			Assert.AreEqual(resultSetColors, 0); 
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void SetPalette()
//		{
//			Sdl.SDL_Color[] colors = new Sdl.SDL_Color[255];
//			for(byte i=0;i<=254;i++)
//			{
//				colors[i].r=i;
//				colors[i].g=i;
//				colors[i].b=i;
//			}
//			IntPtr surfacePtr = VideoSetup();
//			int resultSetPalette = Sdl.SDL_SetPalette(surfacePtr, Sdl.SDL_LOGPAL|Sdl.SDL_PHYSPAL,  colors, 0, 255);
//			Assert.AreEqual(resultSetPalette, 0); 
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ListModes()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO);
			IntPtr format = IntPtr.Zero;
			Sdl.SDL_Rect[] rects = Sdl.SDL_ListModes(format, Sdl.SDL_FULLSCREEN|Sdl.SDL_HWSURFACE);
			Console.WriteLine("ListModes: " + rects.Length);
			for (int i=0; i<rects.Length; i++)
			{
				Console.WriteLine("Mode(" + i + "): " + rects[ i ].ToString());
			}
			Assert.IsTrue(rects.Length > 5);
		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void SDL_MapRGB()
//		{
//			IntPtr surfacePtr = VideoSetup();
//
//			Sdl.SDL_Surface surface = 
//				(Sdl.SDL_Surface)Marshal.PtrToStructure(surfacePtr, typeof(Sdl.SDL_Surface));
//			surfaceFormatPtr = surface.format;
//			Sdl.SDL_PixelFormat surfaceFormat = 
//				(Sdl.SDL_PixelFormat)Marshal.PtrToStructure(surfaceFormatPtr, typeof(Sdl.SDL_PixelFormat));
//			int result = Sdl.SDL_MapRGB(surfaceFormatPtr, 255, 255, 0);
//			Assert.AreEqual(surfaceFormat.BitsPerPixel, 16);
//			Assert.AreEqual(result, 65504);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void SDL_MapRGBA()
//		{
//			IntPtr surfacePtr = VideoSetup();
//
//			Sdl.SDL_Surface surface = 
//				(Sdl.SDL_Surface)Marshal.PtrToStructure(surfacePtr, typeof(Sdl.SDL_Surface));
//			surfaceFormatPtr = surface.format;
//			Sdl.SDL_PixelFormat surfaceFormat = 
//				(Sdl.SDL_PixelFormat)Marshal.PtrToStructure(surfaceFormatPtr, typeof(Sdl.SDL_PixelFormat));
//			int result = Sdl.SDL_MapRGBA(surfaceFormatPtr, 255, 255, 0, 0);
//			Assert.AreEqual(surfaceFormat.BitsPerPixel, 16);
//			Assert.AreEqual(result, 65504);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void SDL_GetRGB()
//		{
//			int pixel = 65504;
//			IntPtr surfacePtr = VideoSetup();
//			byte r;
//			byte g;
//			byte b;
//
//			Sdl.SDL_Surface surface = 
//				(Sdl.SDL_Surface)Marshal.PtrToStructure(surfacePtr, typeof(Sdl.SDL_Surface));
//			surfaceFormatPtr = surface.format;
//			Sdl.SDL_PixelFormat surfaceFormat = 
//				(Sdl.SDL_PixelFormat)Marshal.PtrToStructure(surfaceFormatPtr, typeof(Sdl.SDL_PixelFormat));
//			Sdl.SDL_GetRGB(pixel, surfaceFormatPtr, out r, out g, out b);
//			Assert.AreEqual(r, 255);
//			Assert.AreEqual(g, 255);
//			Assert.AreEqual(b, 0);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void SDL_GetRGBA()
//		{
//			int pixel = 65504;
//			IntPtr surfacePtr = VideoSetup();
//			byte r;
//			byte g;
//			byte b;
//			byte a;
//
//			Sdl.SDL_Surface surface = 
//				(Sdl.SDL_Surface)Marshal.PtrToStructure(surfacePtr, typeof(Sdl.SDL_Surface));
//			surfaceFormatPtr = surface.format;
//			Sdl.SDL_PixelFormat surfaceFormat = 
//				(Sdl.SDL_PixelFormat)Marshal.PtrToStructure(surfaceFormatPtr, typeof(Sdl.SDL_PixelFormat));
//			Sdl.SDL_GetRGBA(pixel, surfaceFormatPtr, out r, out g, out b, out a);
//			Assert.AreEqual(r, 255);
//			Assert.AreEqual(g, 255);
//			Assert.AreEqual(b, 0);
//			Assert.AreEqual(a, 255);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GetColor8()
		{
			Color testColor = Color.White;
			VideoSetup(8);
			Surface surf = new Surface(this.width, this.height);
			Color color = surf.GetColor(testColor.ToArgb());
			Assert.AreEqual(testColor.ToArgb(), color.ToArgb());
			Events.Close();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GetColor16()
		{
			Color testColor = Color.White;
			VideoSetup(16);
			Surface surf = new Surface(this.width, this.height);
			Color color = surf.GetColor(testColor.ToArgb());
			Assert.AreEqual(testColor.ToArgb(), color.ToArgb());
			Events.Close();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GetColor24()
		{
			Color testColor = Color.White;
			VideoSetup(24);
			Surface surf = new Surface(this.width, this.height);
			Color color = surf.GetColor(testColor.ToArgb());
			Assert.AreEqual(testColor.ToArgb(), color.ToArgb());
			Events.Close();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GetColor32()
		{
			Color testColor = Color.White;
			VideoSetup(32);
			Surface surf = new Surface(this.width, this.height);
			Color color = surf.GetColor(testColor.ToArgb());
			Assert.AreEqual(testColor.ToArgb(), color.ToArgb());
			Events.Close();
		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void SDL_GrabInput()
//		{
//			IntPtr surfacePtr = VideoSetup();
//			int result = Sdl.SDL_WM_GrabInput(Sdl.SDL_GRAB_ON);
//			Assert.AreEqual(result, (int)Sdl.SDL_GRAB_ON);
//			result = Sdl.SDL_WM_GrabInput(Sdl.SDL_GRAB_QUERY);
//			Assert.AreEqual(result, (int)Sdl.SDL_GRAB_ON);
//			result = Sdl.SDL_WM_GrabInput(Sdl.SDL_GRAB_OFF);
//			Assert.AreEqual(result, (int)Sdl.SDL_GRAB_OFF);
//			result = Sdl.SDL_WM_GrabInput(Sdl.SDL_GRAB_QUERY);
//			Assert.AreEqual(result, (int)Sdl.SDL_GRAB_OFF);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void CreateRGBSurfaceFrom()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void LockUnlockSurface()
		{
		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void LoadBMPSaveBMPBlit()
//		{
//			IntPtr surfacePtr = VideoSetup();
//			Assert.IsNotNull(surfacePtr);
//			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
//			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
//			IntPtr bmpImagePtr = Sdl.SDL_LoadBMP("test.bmp");
//			Assert.IsNotNull(bmpImagePtr);
//			Assert.IsFalse(bmpImagePtr==IntPtr.Zero);
//			int result = Sdl.SDL_BlitSurface(bmpImagePtr, ref rect1, surfacePtr, ref rect2);
//			Assert.AreEqual(result, 0);
//			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
//			Thread.Sleep(sleepTime);
//			result = Sdl.SDL_SaveBMP(surfacePtr, "testScreen.bmp");
//			Assert.AreEqual(result, 0);
//			Sdl.SDL_FreeSurface(surfacePtr);
//		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void SetColorKey()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void SetAlpha()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void SetClipRect()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void ConvertSurface()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void DisplayFormat()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void DisplayFormatAlpha()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void SDL_CreateLockUnlockFreeYUVOverlay()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void SDL_DisplayYUVOverlay()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void SDL_GL()
		{
		}
//		/// <summary>
//		/// 
//		/// </summary>
//		[Test]
//		public void WM_SetIcon()
//		{
//			IntPtr surfacePtr = VideoSetup();
//			Sdl.SDL_WM_SetIcon(Sdl.SDL_LoadBMP("testicon.bmp"), null);
//			Thread.Sleep(sleepTime);
//		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SurfaceStretchWidth()
		{
			Rectangle sourceRectangle = new Rectangle(0,0,100, 33);
			Rectangle destinationRectangle = new Rectangle(0, 0, 200, 132);
			Surface sourceSurf = new Surface(sourceRectangle);
			Surface surface = sourceSurf.Stretch(sourceRectangle, destinationRectangle);
			Assert.AreEqual(destinationRectangle.Width, surface.Width);
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SurfaceStretchHeight()
		{
			Rectangle sourceRectangle = new Rectangle(0,0,100, 33);
			Rectangle destinationRectangle = new Rectangle(0, 0, 200, 132);
			Surface sourceSurf = new Surface(sourceRectangle);
			Surface surface = sourceSurf.Stretch(sourceRectangle, destinationRectangle);
			Assert.AreEqual(destinationRectangle.Height, surface.Height);
		}
	}
	#endregion SDL_video.h
}
