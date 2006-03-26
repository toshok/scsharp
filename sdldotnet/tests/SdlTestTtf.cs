using System;
using System.Threading;
using NUnit.Framework;
using Tao.Sdl;
using System.Runtime.InteropServices;

namespace SdlDotNet.Tests
{
	#region SDL_ttf.h
	/// <summary>
	/// SDL Tests.
	/// </summary>
	[TestFixture]
	public class SdlTestTtf
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
			SdlTtf.TTF_Quit();
			Tao.Sdl.Sdl.SDL_Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[SetUp]
		public void Init()
		{
			this.Quit();
			SdlTtf.TTF_Init();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
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
			this.Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO);
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
		public void LinkedVersion()
		{
			Sdl.SDL_version version = SdlTtf.TTF_Linked_Version();
			//Console.WriteLine("Ttf version: " + version.ToString());
			Assert.AreEqual("2.0.7", version.major.ToString() 
				+ "." + version.minor.ToString() 
				+ "." + version.patch.ToString());
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void TTF_Init()
		{
			this.Quit();
			Assert.AreEqual( 0, Tao.Sdl.SdlTtf.TTF_Init());
			Assert.IsTrue(Tao.Sdl.SdlTtf.TTF_WasInit()!= 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void TTF_Quit()
		{
			this.Quit();
			Assert.AreEqual(SdlTtf.TTF_WasInit(), 0);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void OpenFont()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 10);
			Assert.IsFalse(fontPtr == IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void OpenFontIndex()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFontIndex("../../FreeSans.ttf", 10, 0);
			Assert.IsFalse(fontPtr == IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void OpenFontRW()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFontRW(Sdl.SDL_RWFromFile("../../FreeSans.ttf", "rb"), 1, 12);
			Assert.IsFalse(fontPtr == IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void OpenFontIndexRW()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFontIndexRW(Sdl.SDL_RWFromFile("../../FreeSans.ttf", "rb"), 1, 12, 0);
			Assert.IsFalse(fontPtr == IntPtr.Zero);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void CloseFont()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFontIndexRW(Sdl.SDL_RWFromFile("../../FreeSans.ttf", "rb"), 1, 12, 0);
			SdlTtf.TTF_CloseFont(fontPtr);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SetGetFontStyle()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 10);
			SdlTtf.TTF_SetFontStyle(fontPtr, SdlTtf.TTF_STYLE_BOLD|SdlTtf.TTF_STYLE_ITALIC);
			Assert.AreEqual(SdlTtf.TTF_STYLE_BOLD|SdlTtf.TTF_STYLE_ITALIC, SdlTtf.TTF_GetFontStyle(fontPtr));
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("For some reason, the FontHeight returns back 3pt higher that what was passed by OpenFont")]
		public void FontHeight()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 9);
			Assert.AreEqual(SdlTtf.TTF_FontHeight(fontPtr), 12);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FontAscent()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 10);
			Assert.AreEqual(12, SdlTtf.TTF_FontAscent(fontPtr));
			//Console.WriteLine("FontAscent:" + result.ToString());
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FontDescent()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 10);
			Assert.AreEqual(-4, SdlTtf.TTF_FontDescent(fontPtr));
			//Console.WriteLine("FontDescent:" + SdlTtf.TTF_FontDescent(fontPtr).ToString());
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FontLineSkip()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 10);
			Assert.AreEqual(17, SdlTtf.TTF_FontLineSkip(fontPtr));
			//Console.WriteLine("FontLineSkip:" + SdlTtf.TTF_FontLineSkip(fontPtr).ToString());
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FontFaces()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 10);
			Assert.AreEqual(SdlTtf.TTF_FontFaces(fontPtr), 4294967297);
			//Console.WriteLine("FontFaces:" + SdlTtf.TTF_FontFaces(fontPtr).ToString());
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FontFaceIsFixedWidth()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 12);
			Assert.AreEqual(SdlTtf.TTF_FontFaceIsFixedWidth(fontPtr), 0);
			IntPtr fontPtrMono = SdlTtf.TTF_OpenFont("../../FreeMono.ttf", 12);
			Assert.IsTrue(SdlTtf.TTF_FontFaceIsFixedWidth(fontPtrMono) != 0);
			//Console.WriteLine("FontFaceIsFixedWidth:" + 
			//	SdlTtf.TTF_FontFaceIsFixedWidth(fontPtrMono).ToString());
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Works fine when run alone. When run as part of the suite is messes up several other tests.")]
		public void FontFaceFamilyName()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 10);
			//Console.WriteLine("FontFaceFamily:" + SdlTtf.TTF_FontFaceFamilyName(fontPtr).ToString());
			Assert.AreEqual(SdlTtf.TTF_FontFaceFamilyName(fontPtr).ToString(), "FreeSans");
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Works fine when run alone. When run as part of the suite is messes up several other tests.")]
		public void FontFaceStyleName()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 10);
			//Console.WriteLine("FontFaceStyleName:" + SdlTtf.TTF_FontFaceStyleName(fontPtr).ToString());
			Assert.AreEqual(SdlTtf.TTF_FontFaceStyleName(fontPtr).ToString(), "Roman");
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GlyphMetrics()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 12);
			int minx;
			int miny;
			int maxx;
			int maxy;
			int advance;
			int result;

			result = SdlTtf.TTF_GlyphMetrics(fontPtr, 1 , out minx, out maxx,out  miny, out maxy, out advance);
			Assert.AreEqual(-1, minx);
			Assert.AreEqual(4, maxx);
			Assert.AreEqual(0, miny);
			Assert.AreEqual(8, maxy);
			Assert.AreEqual(5, advance);
//			Console.WriteLine("minx: " + minx.ToString());
//			Console.WriteLine("maxx: " + maxx.ToString());
//			Console.WriteLine("miny: " + miny.ToString());
//			Console.WriteLine("maxy: " + maxy.ToString());
//			Console.WriteLine("advance: " + advance.ToString());
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SizeText()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 10);
			int w; 
			int h;
			int result = SdlTtf.TTF_SizeText(fontPtr, "hello", out w, out h);
//			Console.WriteLine("w: " + w.ToString());
//			Console.WriteLine("h: " + h.ToString());
			Assert.AreEqual(w, 6);
			Assert.AreEqual(17, h);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SizeUTF8()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 10);
			int w; 
			int h;
			int result = SdlTtf.TTF_SizeUTF8(fontPtr, "hello", out w, out h);
						Console.WriteLine("w: " + w.ToString());
						Console.WriteLine("h: " + h.ToString());
			Assert.AreEqual(w, 6);
			Assert.AreEqual(17, h);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SizeUNICODE()
		{
			this.Quit();
			this.Init();
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 10);
			int w; 
			int h;
			int result = SdlTtf.TTF_SizeUNICODE(fontPtr, "hello", out w, out h);
						Console.WriteLine("w: " + w.ToString());
						Console.WriteLine("h: " + h.ToString());
			Assert.AreEqual(w, 22);
			Assert.AreEqual(17, h);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderText_Solid()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 24);
			Sdl.SDL_Color color = new Sdl.SDL_Color(254, 0, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderText_Solid(fontPtr, "hello", color);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderUTF8_Solid()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 24);
			Sdl.SDL_Color color = new Sdl.SDL_Color(254, 0, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderUTF8_Solid(fontPtr, "hello", color);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderUNICODE_Solid()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 24);
			Sdl.SDL_Color color = new Sdl.SDL_Color(254, 0, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderUNICODE_Solid(fontPtr, "hello", color);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderGlyph_Solid()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 24);
			Sdl.SDL_Color color = new Sdl.SDL_Color(254, 0, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderGlyph_Solid(fontPtr, 1000, color);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderText_Shaded()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 24);
			Sdl.SDL_Color colorfg = new Sdl.SDL_Color(254, 0, 0);
			Sdl.SDL_Color colorbg = new Sdl.SDL_Color(0, 254, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderText_Shaded(fontPtr, "hello", colorfg, colorbg);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderUTF8_Shaded()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 24);
			Sdl.SDL_Color colorfg = new Sdl.SDL_Color(254, 0, 0);
			Sdl.SDL_Color colorbg = new Sdl.SDL_Color(0, 254, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderUTF8_Shaded(fontPtr, "hello", colorfg, colorbg);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderUNICODE_Shaded()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 24);
			Sdl.SDL_Color colorfg = new Sdl.SDL_Color(254, 0, 0);
			Sdl.SDL_Color colorbg = new Sdl.SDL_Color(0, 254, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderUNICODE_Shaded(fontPtr, "hello", colorfg, colorbg);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderGlyph_Shaded()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 24);
			Sdl.SDL_Color colorfg = new Sdl.SDL_Color(254, 0, 0);
			Sdl.SDL_Color colorbg = new Sdl.SDL_Color(0, 254, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderGlyph_Shaded(fontPtr, 1000, colorfg, colorbg);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderText_Blended()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 24);
			Sdl.SDL_Color colorfg = new Sdl.SDL_Color(254, 0, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderText_Blended(fontPtr, "hello", colorfg);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderUTF8_Blended()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 24);
			Sdl.SDL_Color colorfg = new Sdl.SDL_Color(254, 0, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderUTF8_Blended(fontPtr, "hello", colorfg);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderUNICODE_Blended()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 24);
			Sdl.SDL_Color colorfg = new Sdl.SDL_Color(254, 0, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderUNICODE_Blended(fontPtr, "hello", colorfg);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RenderGlyph_Blended()
		{
			this.Quit();
			this.Init();
			IntPtr surfacePtr = VideoSetup();
			Sdl.SDL_Rect rect1 = new Sdl.SDL_Rect(0,0,400,400);
			Sdl.SDL_Rect rect2 = new Sdl.SDL_Rect(0,0,400,400);
			IntPtr fontPtr = SdlTtf.TTF_OpenFont("../../FreeSans.ttf", 12);
			Sdl.SDL_Color colorfg = new Sdl.SDL_Color(254, 0, 0);
			IntPtr fontSurfacePtr = SdlTtf.TTF_RenderGlyph_Blended(fontPtr, 1000, colorfg);
			Assert.IsFalse(fontSurfacePtr == IntPtr.Zero);
			int result = Sdl.SDL_BlitSurface(fontSurfacePtr, ref rect1, surfacePtr, ref rect2);
			Assert.AreEqual(result, 0);
			Sdl.SDL_UpdateRect(surfacePtr, 0,0,400,400);
			Thread.Sleep(sleepTime);
			this.Quit();
		}
	}
	#endregion SDL_ttf.h
}
