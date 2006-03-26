using System;
using System.Threading;
using NUnit.Framework;
using Tao.Sdl;
using System.Runtime.InteropServices;

namespace SdlDotNet.Tests
{
	#region SDL_gfx.h
	/// <summary>
	/// SDL Tests.
	/// </summary>
	[TestFixture]
	public class SdlTestGfx
	{
		int flags = (Sdl.SDL_HWSURFACE|Sdl.SDL_DOUBLEBUF|Sdl.SDL_ANYFORMAT);
		int bpp = 16;
		int width = 640;
		int height = 480;
		IntPtr surfacePtr;
		Sdl.SDL_Rect rect2;
		int sleepTime = 1000;
		short[] vx = {40, 80, 130, 80, 40};
		short[] vy = {80, 40, 80, 130, 130};
		byte[] src1 = {1,2,3,4};
		byte[] src2 = {2,10,20,40};
		byte[] dest = new byte[4];
		
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
			this.GfxSetup();
			
		}
		private void GfxSetup()
		{
			surfacePtr = Sdl.SDL_SetVideoMode(
				width, 
				height, 
				bpp, 
				flags);
			rect2 = new Sdl.SDL_Rect(
				0,
				0,
				(short) width,
				(short) height);
			Sdl.SDL_SetClipRect(surfacePtr, ref rect2);
		}
		/// <summary>
		/// 
		/// </summary>
		private void Quit()
		{
			Sdl.SDL_Quit();
		}

		#region SDL_gfxPrimitives.h
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void pixelColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.pixelColor(surfacePtr, 100,100,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void pixelRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.pixelRGBA(surfacePtr, 100,100, 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void hlineColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.hlineColor(surfacePtr, 100, 200,100,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void hlineRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.hlineRGBA(surfacePtr, 100,200,100, 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void vlineColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.vlineColor(surfacePtr, 100, 200,100,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void vlineRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.vlineRGBA(surfacePtr, 100,200,100, 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void rectangleColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.rectangleColor(surfacePtr, 100, 200,300, 300,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void rectangleRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.rectangleRGBA(surfacePtr, 100,200,300,300, 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void boxColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.boxColor(surfacePtr, 100, 200,300, 300,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void boxRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.boxRGBA(surfacePtr, 100,200,300,300, 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void lineColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.lineColor(surfacePtr, 100, 200,300, 300,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void lineRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.lineRGBA(surfacePtr, 100,200,300,300, 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void aalineColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.aalineColor(surfacePtr, 100, 200,300, 300,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void aalineRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.aalineRGBA(surfacePtr, 100,200,300,300, 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void CircleRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.circleRGBA(surfacePtr, 100,100, 50,200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void CircleColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.circleColor(surfacePtr, 100,100, 50,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void aaCircleRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.aacircleRGBA(surfacePtr, 100,100, 50,200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void aaCircleColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.aacircleColor(surfacePtr, 100,100, 50,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void filledCircleColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.filledCircleColor(surfacePtr, 100,100, 50,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void filledCircleRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.filledCircleRGBA(surfacePtr, 100,100, 50,200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void EllipseColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.ellipseColor(surfacePtr, 200,100, 100, 50,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void EllipseRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.ellipseRGBA(surfacePtr, 200,100,100, 50,200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void aaEllipseColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.aaellipseColor(surfacePtr, 200,100, 100, 50,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void aaEllipseRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.aaellipseRGBA(surfacePtr, 200,100,100, 50,200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FilledEllipseColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.filledEllipseColor(surfacePtr, 200,100, 100, 50,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FilledEllipseRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.filledEllipseRGBA(surfacePtr, 200,100,100, 50,200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void PieColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.pieColor(surfacePtr, 200,100, 100, 50, 100,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void PieRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.pieRGBA(surfacePtr, 200,100,100, 50, 100,200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FilledPieColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.filledPieColor(surfacePtr, 200,100, 100, 50, 100,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FilledPieRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.filledPieRGBA(surfacePtr, 200,100,100, 50, 100,200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void TrigonColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.trigonColor(surfacePtr, 100,100, 250,400, 100, 300,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void TrigonPieRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.trigonRGBA(surfacePtr, 100,100, 250,400, 100, 300,200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void aaTrigonColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.aatrigonColor(surfacePtr, 100,100, 250,400, 100, 300,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void aaTrigonPieRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.aatrigonRGBA(surfacePtr, 100,100, 250,400, 100, 300,200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FilledTrigonColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.filledTrigonColor(surfacePtr, 100,100, 250,400, 100, 300,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FilledTrigonPieRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.filledTrigonRGBA(surfacePtr, 100,100, 250,400, 100, 300,200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void polygonColor()
		{
			this.InitSdl();
			Random rand = new Random();
			int result = SdlGfx.polygonColor(surfacePtr,vx, vy,vx.Length,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void polygonRGBA()
		{
			this.InitSdl();
			Random rand = new Random();
			int result = 
				SdlGfx.polygonRGBA(surfacePtr,vx, vy,vx.Length, 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void aapolygonColor()
		{
			this.InitSdl();
			Random rand = new Random();
			int result = SdlGfx.aapolygonColor(surfacePtr,vx, vy,vx.Length,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void aapolygonRGBA()
		{
			this.InitSdl();
			Random rand = new Random();
			int result = 
				SdlGfx.aapolygonRGBA(surfacePtr,vx, vy,vx.Length, 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FilledPolygonColor()
		{
			this.InitSdl();
			Random rand = new Random();
			int result = SdlGfx.filledPolygonColor(surfacePtr,vx, vy,vx.Length,7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FilledPolygonRGBA()
		{
			this.InitSdl();
			Random rand = new Random();
			int result = 
				SdlGfx.filledPolygonRGBA(surfacePtr,vx, vy,vx.Length, 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void bezierColor()
		{
			this.InitSdl();
			Random rand = new Random();
			int result = SdlGfx.bezierColor(surfacePtr,vx, vy,vx.Length,4, 7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void bezierRGBA()
		{
			this.InitSdl();
			Random rand = new Random();
			int result = 
				SdlGfx.bezierRGBA(surfacePtr,vx, vy,vx.Length, 4, 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void charColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.characterColor(
				surfacePtr, 100,100, 'X',7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void charRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.characterRGBA(
				surfacePtr, 100,100, 'X', 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void stringColor()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.stringColor(
				surfacePtr, 100,100, "SDL.NET",7777777);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void stringRGBA()
		{
			this.InitSdl();
			
			Random rand = new Random();
			int result = SdlGfx.stringRGBA(
				surfacePtr, 100,100, "SDL.NET", 200, 0,(byte)0, 254);
			result = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result,0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("not finished")]
		public void gfxPrimitivesFont()
		{
		}
		#endregion SDL_gfxPrimitives.h

		#region SDL_rotozoom.h
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void rotozoomSurface()
		{
			this.InitSdl();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr bmpImagePtr = Sdl.SDL_LoadBMP("test.bmp");
			IntPtr rotoSurfacePtr = SdlGfx.rotozoomSurface(bmpImagePtr, 90, 2, SdlGfx.SMOOTHING_OFF);
			int result = Sdl.SDL_BlitSurface(rotoSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.IsNotNull(rotoSurfacePtr);
			Assert.IsFalse(rotoSurfacePtr==IntPtr.Zero);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			
			//int results = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void rotozoomSurfaceSize()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void zoomSurface()
		{
			this.InitSdl();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr bmpImagePtr = Sdl.SDL_LoadBMP("test.bmp");
			IntPtr zoomSurfacePtr = SdlGfx.zoomSurface(bmpImagePtr, 5, 2, SdlGfx.SMOOTHING_OFF);
			int result = Sdl.SDL_BlitSurface(zoomSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.IsNotNull(zoomSurfacePtr);
			Assert.IsFalse(zoomSurfacePtr==IntPtr.Zero);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			
			//int results = Sdl.SDL_Flip(surfacePtr);
			Thread.Sleep(sleepTime);
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void zoomSurfaceSize()
		{
			int dstwidth;
			int dstheight;
			SdlGfx.zoomSurfaceSize(100, 33, 2, 4, out dstwidth, out dstheight);
			Assert.AreEqual(200, dstwidth);
			Assert.AreEqual(132, dstheight);

				
		}
		#endregion SDL_rotozoom.h

		#region SDL_imageFilter.h
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ImageFilterAdd()
		{
			int result = SdlGfx.SDL_imageFilterAdd(src1, src2, dest, src1.Length);
			//Console.WriteLine("result: " + result.ToString());
			//Console.WriteLine(
			//	"dest: " + dest[0].ToString() + 
			//	", " + dest[1].ToString() + ", " + dest[2].ToString() + 
			//	", " + dest[3].ToString());
			Assert.AreEqual(src1[0] + src2[0], dest[0]);
			Assert.AreEqual(src1[1] + src2[1], dest[1]);
			Assert.AreEqual(src1[2] + src2[2], dest[2]);
			Assert.AreEqual(src1[3] + src2[3], dest[3]);
		}
		#endregion SDL_imageFilter.h
	}
	#endregion SDL_gfx.h
}