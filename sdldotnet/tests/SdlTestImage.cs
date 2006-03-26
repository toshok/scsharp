using System;
using System.Threading;
using NUnit.Framework;
using Tao.Sdl;
using System.Runtime.InteropServices;

namespace SdlDotNet.Tests
{
	#region SDL_image.h
	/// <summary>
	/// SDL Tests.
	/// </summary>
	[TestFixture]
	public class SdlTestImage
	{
		//int init;
		int flags;
		int bpp;
		int width;
		int height;
		//IntPtr surfacePtr;
		int sleepTime;

		private void Quit()
		{
			Tao.Sdl.Sdl.SDL_Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[SetUp]
		public void Init()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO);
			flags = (Sdl.SDL_HWSURFACE|Sdl.SDL_DOUBLEBUF|Sdl.SDL_ANYFORMAT);
			bpp = 16;
			width = 640;
			height = 480;
			sleepTime = 500;
			//surfacePtr = IntPtr.Zero;
			//Sdl.SDL_FreeSurfaceInternal(surfacePtr);
		}
		/// <summary>
		/// 
		/// </summary>
		private IntPtr VideoSetup()
		{
			this.Init();
			IntPtr surfacePtr;
			//Assert.IsNotNull(surfacePtr);
			//Sdl.SDL_FreeSurface(surfacePtr);
			surfacePtr = Sdl.SDL_SetVideoMode(
				width, 
				height, 
				bpp, 
				flags);
			Assert.IsNotNull(surfacePtr);
			return surfacePtr;
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void isBMP()
		{
			string file = "test.bmp";
			Assert.IsFalse(SdlImage.IMG_isBMP(Sdl.SDL_RWFromFile(file, "rb")) == IntPtr.Zero);
			Assert.AreEqual(SdlImage.IMG_isBMP(Sdl.SDL_RWFromFile("test.jpg", "rb")), IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void isJPG()
		{
			string file = "test.jpg";
			Assert.IsFalse(SdlImage.IMG_isJPG(Sdl.SDL_RWFromFile(file, "rb")) == IntPtr.Zero);
			Assert.AreEqual(SdlImage.IMG_isJPG(Sdl.SDL_RWFromFile("test.bmp", "rb")), IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void isGIF()
		{
			string file = "test.gif";
			Assert.IsFalse(SdlImage.IMG_isGIF(Sdl.SDL_RWFromFile(file, "rb")) == IntPtr.Zero);
			Assert.AreEqual(SdlImage.IMG_isGIF(Sdl.SDL_RWFromFile("test.bmp", "rb")), IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void isPNG()
		{
			string file = "test.png";
			Assert.IsFalse(SdlImage.IMG_isPNG(Sdl.SDL_RWFromFile(file, "rb")) == IntPtr.Zero);
			Assert.AreEqual(SdlImage.IMG_isPNG(Sdl.SDL_RWFromFile("test.bmp", "rb")), IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void isPNM()
		{
			string file = "test.pnm";
			Assert.IsFalse(SdlImage.IMG_isPNM(Sdl.SDL_RWFromFile(file, "rb")) == IntPtr.Zero);
			Assert.AreEqual(SdlImage.IMG_isPNM(Sdl.SDL_RWFromFile("test.bmp", "rb")), IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void isPCX()
		{
			string file = "test.pcx";
			Assert.IsFalse(SdlImage.IMG_isPCX(Sdl.SDL_RWFromFile(file, "rb")) == IntPtr.Zero);
			Assert.AreEqual(SdlImage.IMG_isPCX(Sdl.SDL_RWFromFile("test.bmp", "rb")), IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void isXPM()
		{
			string file = "test.xpm";
			Assert.IsFalse(SdlImage.IMG_isXPM(Sdl.SDL_RWFromFile(file, "rb")) == IntPtr.Zero);
			Assert.AreEqual(SdlImage.IMG_isXPM(Sdl.SDL_RWFromFile("test.bmp", "rb")), IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Have not created XCF test file.")]
		public void isXCF()
		{
			string file = "test.xcf";
			Assert.IsFalse(SdlImage.IMG_isXCF(Sdl.SDL_RWFromFile(file, "rb")) == IntPtr.Zero);
			Assert.AreEqual(SdlImage.IMG_isXCF(Sdl.SDL_RWFromFile("test.bmp", "rb")), IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void isTIF()
		{
			string file = "test.tif";
			Assert.IsFalse(SdlImage.IMG_isTIF(Sdl.SDL_RWFromFile(file, "rb")) == IntPtr.Zero);
			Assert.AreEqual(SdlImage.IMG_isTIF(Sdl.SDL_RWFromFile("test.bmp", "rb")), IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadGIF()
		{
			string file = "test.gif";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_LoadGIF_RW(Sdl.SDL_RWFromFile(file, "rb"));
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadJPG()
		{
			string file = "test.jpg";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_LoadJPG_RW(Sdl.SDL_RWFromFile(file, "rb"));
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadPNG()
		{
			string file = "test.png";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_LoadPNG_RW(Sdl.SDL_RWFromFile(file, "rb"));
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadPCX()
		{
			string file = "test.pcx";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_LoadPCX_RW(Sdl.SDL_RWFromFile(file, "rb"));
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadTGA()
		{
			string file = "test.tga";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_LoadTGA_RW(Sdl.SDL_RWFromFile(file, "rb"));
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadPNM()
		{
			string file = "test.pnm";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_LoadPNM_RW(Sdl.SDL_RWFromFile(file, "rb"));
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadBMP()
		{
			string file = "test.bmp";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_LoadBMP_RW(Sdl.SDL_RWFromFile(file, "rb"));
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadXPM()
		{
			string file = "test.xpm";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_LoadXPM_RW(Sdl.SDL_RWFromFile(file, "rb"));
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Have not created XCF test file.")]
		public void LoadXCF()
		{
			string file = "test.xcf";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_LoadXCF_RW(Sdl.SDL_RWFromFile(file, "rb"));
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadTIF()
		{
			string file = "test.tif";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_LoadTIF_RW(Sdl.SDL_RWFromFile(file, "rb"));
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("TODO.")]
		public void ReadXPMFromArray()
		{
			//string file = "test.xpm";
			IntPtr surfacePtr = VideoSetup();
			//IntPtr imagePtr = SdlImage.IMG_ReadXPMFromArray();
			//Assert.IsFalse(imagePtr == IntPtr.Zero);
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SetGetError()
		{
			string error = "Hi there";
			SdlImage.IMG_SetError(error);
			Assert.AreEqual(SdlImage.IMG_GetError(), error);
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Load()
		{
			string file = "test.bmp";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_Load(file);
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Load_RW()
		{
			string file = "test.bmp";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_Load_RW(Sdl.SDL_RWFromFile(file, "rb"),1 );
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			file = "test.jpg";
			surfacePtr = VideoSetup();
			imagePtr = SdlImage.IMG_Load_RW(Sdl.SDL_RWFromFile(file, "rb"), 1);
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadTyped_RW()
		{
			string file = "test.gif";
			IntPtr surfacePtr = VideoSetup();
			IntPtr imagePtr = SdlImage.IMG_LoadTyped_RW(Sdl.SDL_RWFromFile(file, "rb"),1, "gif" );
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,200,200);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,200,200);
			int result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			file = "test.png";
			surfacePtr = VideoSetup();
			imagePtr = SdlImage.IMG_LoadTyped_RW(Sdl.SDL_RWFromFile(file, "rb"), 1, "png");
			Assert.IsFalse(imagePtr == IntPtr.Zero);
			result = Sdl.SDL_BlitSurface(imagePtr, ref rect1, surfacePtr, ref rect2);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,200,200);
			Thread.Sleep(sleepTime);
			Assert.AreEqual(result, 0);
			this.Quit();
		}
	}
	#endregion SDL_image.h
}
