using System;
using System.Threading;
using NUnit.Framework;
using Tao.Sdl;
using System.Runtime.InteropServices;

namespace SdlDotNet.Tests
{
	/// <summary>
	/// SDL Tests.
	/// </summary>
	[TestFixture]
	public class SdlTest
	{
		#region SDL.h
		private void InitSdl()
		{
			Tao.Sdl.Sdl.SDL_Quit();
		}
		private void Quit()
		{
			Tao.Sdl.Sdl.SDL_Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitVideo()
		{
			Video.Close();
			//Assert.IsFalse(Video.IsInitialized);
			Video.Initialize();
			//Assert.IsTrue(Video.IsInitialized);
			Video.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitMixer()
		{
			Mixer.Close();
			//Assert.IsFalse(Mixer.IsInitialized);
			Mixer.Initialize();
			//Assert.IsTrue(Mixer.IsInitialized);
			Mixer.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitTimer()
		{
			Timer.Close();
			//Assert.IsFalse(Timer.IsInitialized);
			Timer.Initialize();
			//Assert.IsTrue(Timer.IsInitialized);
			Timer.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitCDRom()
		{
			CDRom.Close();
			//Assert.IsFalse(CDRom.IsInitialized);
			CDRom.Initialize();
			//Assert.IsTrue(CDRom.IsInitialized);
			CDRom.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitJoystick()
		{
			Joysticks.Close();
			//Assert.IsFalse(Joysticks.IsInitialized);
			Joysticks.Initialize();
			//Assert.IsTrue(Joysticks.IsInitialized);
			Joysticks.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitEverything()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Assert.AreEqual( 0, Tao.Sdl.Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING));
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_EVERYTHING)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitSubSystemVideo()
		{
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_VIDEO) == 0);
			Assert.AreEqual( 0, Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_AUDIO));
			Assert.AreEqual( 0, Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_VIDEO));
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_VIDEO)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitSubSystemAudio()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Assert.AreEqual( 0, Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_AUDIO));
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_AUDIO)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitSubSystemTimer()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Assert.AreEqual( 0, Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_TIMER));
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_TIMER)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitSubSystemCDRom()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Assert.AreEqual( 0, Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_CDROM));
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_CDROM)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitSubSystemJoystick()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Assert.AreEqual( 0, Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_JOYSTICK));
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_JOYSTICK)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void InitSubSystemEverything()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Assert.AreEqual( 0, Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_EVERYTHING));
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_EVERYTHING)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SdlQuit()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_EVERYTHING)== 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void QuitSubSystemVideo()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_VIDEO);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_VIDEO)!= 0);
			Tao.Sdl.Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_VIDEO);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_VIDEO)== 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void QuitSubSystemAudio()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_AUDIO);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_AUDIO)!= 0);
			Tao.Sdl.Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_AUDIO);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_AUDIO)== 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void QuitSubSystemTimer()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_TIMER);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_TIMER)!= 0);
			Tao.Sdl.Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_TIMER);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_TIMER)== 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void QuitSubSystemCDRom()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_CDROM);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_CDROM)!= 0);
			Tao.Sdl.Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_CDROM);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_CDROM)== 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void QuitSubSystemJoystick()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_JOYSTICK);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_JOYSTICK)!= 0);
			Tao.Sdl.Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_JOYSTICK);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_JOYSTICK)== 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void QuitSubSystemEverything()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_EVERYTHING);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_EVERYTHING)!= 0);
			Tao.Sdl.Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_EVERYTHING);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_EVERYTHING)== 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void WasInitEverything()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_EVERYTHING)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void WasInitVideo()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_VIDEO)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void WasInitAudio()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_Init(Sdl.SDL_INIT_AUDIO);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_AUDIO)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void WasInitCDRom()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_Init(Sdl.SDL_INIT_CDROM);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_CDROM)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void WasInitJoystick()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_Init(Sdl.SDL_INIT_JOYSTICK);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_JOYSTICK)!= 0);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void WasInitTimer()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_Init(Sdl.SDL_INIT_TIMER);
			Assert.IsTrue(Tao.Sdl.Sdl.SDL_WasInit(Sdl.SDL_INIT_TIMER)!= 0);
			this.Quit();
		}
		#endregion SDL.h

		#region SDL_active.h
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GetAppState()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			byte state = Sdl.SDL_GetAppState();
			Console.WriteLine("SDL_GetAppState(): " + state.ToString());
			Assert.IsTrue(state == 7);
			this.Quit();
		}
		#endregion SDL_active.h

		#region SDL_audio.h

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void AudioDriverName()
		{
			Sdl.SDL_Init(Sdl.SDL_INIT_AUDIO);
			string driver= "";
			string result = Sdl.SDL_AudioDriverName(driver,20);
			Assert.IsNotNull(result);
			
			//Console.WriteLine("audio driver: " + result);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void OpenAudio()
		{
			Sdl.SDL_Init(Sdl.SDL_INIT_AUDIO);
			//Console.WriteLine("audio driver: " + result);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Causes problems with Mixer tests")]
		public void LoadWAV()
		{
			Sdl.SDL_Init(Sdl.SDL_INIT_AUDIO);
			IntPtr spec;

			IntPtr audio_buf;

			int audio_len;
			
			IntPtr pointer = Sdl.SDL_LoadWAV("test.wav", out spec, out audio_buf, out audio_len);
				Console.WriteLine("Error: " + Sdl.SDL_GetError());
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Causes problems with Mixer tests")]
		public void LoadWAV_RW()
		{
			Sdl.SDL_Init(Sdl.SDL_INIT_AUDIO);
			IntPtr spec;

			IntPtr audio_buf;

			int audio_len;
			
			IntPtr result = Sdl.SDL_LoadWAV_RW(Sdl.SDL_RWFromFile("test.wav", "rb"), 1,  out spec,  out audio_buf, out audio_len);
			Console.WriteLine("Error: " + Sdl.SDL_GetError());
			this.Quit();
		}
		#endregion SDL_audio.h

		#region SDL_byteorder.h

		/// <summary>
		/// Endian Test
		/// </summary>
		[Test]
		public void Endian()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
#if WIN32
			Assert.IsTrue(Sdl.SDL_BYTEORDER == Sdl.SDL_LIL_ENDIAN);
#endif

			this.Quit();
		}
		#endregion SDL_byteorder.h

		#region SDL_cdrom.h
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void FRAMES_TO_MSF()
		{

			int frames = 10000;
			int M;
			int S;
			int F;

			Sdl.FRAMES_TO_MSF(frames, out M, out S, out F);
			Assert.AreEqual(M, 2);
			Assert.AreEqual(S, 13);
			Assert.AreEqual(F, 25);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void MSF_TO_FRAMES()
		{
			int frames = 10000;
			int M = 2;
			int S = 13;
			int F = 25;

			int result = Sdl.MSF_TO_FRAMES(M, S, F);
			Assert.AreEqual(result, frames);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CDNumDrives()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			//Console.WriteLine("CDNumDrives: " + Sdl.SDL_CDNumDrives());
			Assert.AreEqual(Sdl.SDL_CDNumDrives(), 1);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CDName()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			Assert.AreEqual(Sdl.SDL_CDName(0), "D:\\");
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CDOpen()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			IntPtr resultPtr = Sdl.SDL_CDOpen(0);
			Sdl.SDL_CD cd = 
				(Sdl.SDL_CD)Marshal.PtrToStructure(resultPtr, typeof(Sdl.SDL_CD));
			//Console.WriteLine("CDName: " + Sdl.SDL_CDName(0));
			Assert.AreEqual(cd.id, 0 );
			Sdl.SDL_CDClose(resultPtr);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CDStatus()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			IntPtr cd = Sdl.SDL_CDOpen(0);
			Console.WriteLine("CDStatus: " + Sdl.SDL_CDStatus(cd));
			//Assert.AreEqual(Sdl.SDL_CDName(0), "D:\\");
			Sdl.SDL_CDClose(cd);
			this.Quit();
		}
		/// <summary>
		/// There is a problem with the SDL C-function. 
		/// Many cds will not start with a low 'start' setting. 
		/// I will only use SDL_CDPlayTracks.
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CDPlay()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			IntPtr cd = Sdl.SDL_CDOpen(0);
			Assert.IsFalse(cd == IntPtr.Zero);
			if ( Sdl.CD_INDRIVE(Sdl.SDL_CDStatus(cd)) == 1 )
			{
				Console.WriteLine("CDStatus: " + Sdl.SDL_CDStatus(cd));
				int result = Sdl.SDL_CDPlay(cd, 300, 1000);
				if (result == -1)
				{
					Console.WriteLine("Error: " + Sdl.SDL_GetError());
				}
				Assert.AreEqual(0, result);
			}
			Sdl.SDL_CDClose(cd);
		this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CDPlayTracks()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			IntPtr cd = Sdl.SDL_CDOpen(0);
			Assert.IsFalse(cd == IntPtr.Zero);
			//Console.WriteLine("CDStatus: " + Sdl.SDL_CDStatus(cd));
			if ( Sdl.CD_INDRIVE(Sdl.SDL_CDStatus(cd)) == 1 )
			{
				int result = Sdl.SDL_CDPlayTracks(cd, 0, 0, 0, Sdl.CD_FPS*20);
				if (result == -1)
				{
					Console.WriteLine("Error: " + Sdl.SDL_GetError());
				}
				Assert.AreEqual(0, result);
			}
			Sdl.SDL_CDClose(cd);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CDPause()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			IntPtr cd = Sdl.SDL_CDOpen(0);
			//Console.WriteLine("CDStatus: " + Sdl.SDL_CDStatus(cd));
			if ( Sdl.CD_INDRIVE(Sdl.SDL_CDStatus(cd)) == 1 )
			{
				int result = Sdl.SDL_CDPlayTracks(cd, 0, 0, 0, Sdl.CD_FPS*20);
				Assert.AreEqual(result, 0);
				if (result == -1)
				{
					Console.WriteLine("Error: " + Sdl.SDL_GetError());
				}
				result = Sdl.SDL_CDPause(cd);
				Assert.AreEqual(0, result);
			}
			Sdl.SDL_CDClose(cd);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CDResume()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			IntPtr cd = Sdl.SDL_CDOpen(0);
			//Console.WriteLine("CDStatus: " + Sdl.SDL_CDStatus(cd));
			if ( Sdl.CD_INDRIVE(Sdl.SDL_CDStatus(cd)) == 1 )
			{
				int result = Sdl.SDL_CDPlayTracks(cd, 0, 0, 0, Sdl.CD_FPS*20);
				Assert.AreEqual(result, 0);
				if (result == -1)
				{
					Console.WriteLine("Error: " + Sdl.SDL_GetError());
				}
				result = Sdl.SDL_CDPause(cd);
				Assert.AreEqual(0, result);
				result = Sdl.SDL_CDResume(cd);
				Assert.AreEqual(result, 0);
			}
			Sdl.SDL_CDClose(cd);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CDStop()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			IntPtr cd = Sdl.SDL_CDOpen(0);
			//Console.WriteLine("CDStatus: " + Sdl.SDL_CDStatus(cd));
			if ( Sdl.CD_INDRIVE(Sdl.SDL_CDStatus(cd)) == 1 )
			{
				int result = Sdl.SDL_CDPlayTracks(cd, 0, 0, 0, Sdl.CD_FPS*20);
				Assert.AreEqual(result, 0);
				if (result == -1)
				{
					Console.WriteLine("Error: " + Sdl.SDL_GetError());
				}
				result = Sdl.SDL_CDStop(cd);
				Assert.AreEqual(result, 0);
			}
			Sdl.SDL_CDClose(cd);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CD_INDRIVE()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			IntPtr cd = Sdl.SDL_CDOpen(0);
			//Console.WriteLine("CD_INDRIVE: " + Sdl.CD_INDRIVE(Sdl.SDL_CDStatus(cd)));
			//Make sure there is a CD in the drive
			Assert.AreEqual(Sdl.CD_INDRIVE(Sdl.SDL_CDStatus(cd)), 1);
			Sdl.SDL_CDClose(cd);
			Sdl.SDL_Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void TrackAudio()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			IntPtr intPtr = Sdl.SDL_CDOpen(0);
			Console.WriteLine("CD_INDRIVE: " + Sdl.CD_INDRIVE(Sdl.SDL_CDStatus(intPtr)));
			//Make sure there is a CD in the drive
			Assert.AreEqual(Sdl.CD_INDRIVE(Sdl.SDL_CDStatus(intPtr)), 1);
			
			Sdl.SDL_CD cd = 
				(Sdl.SDL_CD)Marshal.PtrToStructure(
				intPtr, typeof(Sdl.SDL_CD));
			//Sdl.SDL_CDtrack[] cdTrack = new Sdl.SDL_CDtrack[cd.numtracks];
			//IntPtr current;
			Console.WriteLine(cd.numtracks);
			Console.WriteLine(cd.cur_frame);
			Console.WriteLine(cd.cur_track);
			Console.WriteLine(cd.id);
			Console.WriteLine(cd.status);
			int minutes;
			int seconds;
			int frames;
			
			for (int i = 0; i < cd.numtracks; i++ )
			{
				Console.WriteLine("Type: " + cd.track[i].type);
				Sdl.FRAMES_TO_MSF(cd.track[i].length, out minutes, out seconds, out frames);
				Console.WriteLine("Length: " + minutes + ":" + seconds);
				Console.WriteLine("Id: " + cd.track[i].id);
				//Console.WriteLine(cd.track.ToInt32());
				//cdTrack[ i ] = (Sdl.SDL_CDtrack)Marshal.PtrToStructure( cd.track, typeof(Sdl.SDL_CDtrack));
				
//				
//         
//				//Marshal.FreeCoTaskMem( (IntPtr)Marshal.ReadInt32( current ));
//				//Marshal.DestroyStructure( current, typeof(Sdl.SDL_CDtrack) );
//				current = (IntPtr)((int)current + 
					//Marshal.SizeOf( cdTrack[ i ] ));
         
				//current = Marshal.ReadIntPtr(intPtr, Marshal.SizeOf(typeof(Sdl.SDL_CDtrack)));
				//current = new IntPtr(intPtr.ToInt32() + i * Marshal.SizeOf(typeof(Sdl.SDL_CDtrack)));
				//Marshal.Copy(current, j, 0, Marshal.SizeOf(typeof(Sdl.SDL_CDtrack)));
				//Marshal.Copy(j, 0, structPtr, Marshal.SizeOf(typeof(Sdl.SDL_CDtrack)));
				//Marshal.WriteIntPtr(intPtr, structPtr[i]);

				//Console.WriteLine("SDL_CDtrack: " + Marshal.SizeOf(typeof(Sdl.SDL_CDtrack)));
				//Assert.IsFalse(intPtr == IntPtr.Zero);
				//Assert.IsFalse(current == IntPtr.Zero);
				//Assert.IsFalse(structPtr[i] == IntPtr.Zero);
				//Console.WriteLine(intPtr.ToString());
				//Console.WriteLine(current.ToString());
				//Console.WriteLine(structPtr[i].ToString());
				//cdTrack[ i ] = (Sdl.SDL_CDtrack)Marshal.PtrToStructure( structPtr[i], typeof(Sdl.SDL_CDtrack));

				//Console.WriteLine( "Track Id: " + cdTrack[ i ].id );
				//Console.WriteLine( "Track Type: " + cdTrack[i].type );

				//Assert.AreEqual(cdTrack[i].type,(byte) Sdl.SDL_AUDIO_TRACK);
			}
			Sdl.SDL_CDClose(intPtr);
			Sdl.SDL_Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CDEject()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			IntPtr cd = Sdl.SDL_CDOpen(0);
			int result = Sdl.SDL_CDEject(cd);
			Sdl.SDL_CDClose(cd);
			this.Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CDROM")]
		public void CDClose()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			IntPtr cd = Sdl.SDL_CDOpen(0);
			Sdl.SDL_CDClose(cd);

			this.Quit();
		}
		#endregion SDL_cdrom.h
		
		#region SDL_cpuinfo.h
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CPUInfo")]
		public void HasMMX()
		{

			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			Assert.IsTrue(Sdl.SDL_HasMMX() == Sdl.SDL_TRUE);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CPUInfo")]
		public void HasMMXExt()
		{

			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			Assert.IsFalse(Sdl.SDL_HasMMXExt() == Sdl.SDL_TRUE);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CPUInfo")]
		public void Has3DNow()
		{

			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			Assert.IsFalse(Sdl.SDL_Has3DNow() == Sdl.SDL_TRUE);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CPUInfo")]
		public void HasAltiVec()
		{

			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			Assert.IsFalse(Sdl.SDL_HasAltiVec() == Sdl.SDL_TRUE);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CPUInfo")]
		public void HasRDTSC()
		{

			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			Assert.IsTrue(Sdl.SDL_HasRDTSC() == Sdl.SDL_TRUE);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CPUInfo")]
		public void HasSSE()
		{

			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			Assert.IsTrue(Sdl.SDL_HasSSE() == Sdl.SDL_TRUE);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Category("CPUInfo")]
		public void HasSSE2()
		{

			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			Assert.IsTrue(Sdl.SDL_HasSSE2() == Sdl.SDL_TRUE);
			this.Quit();
		}
		#endregion SDL_cpuinfo.h
		
		#region SDL_error.h
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SetGetError()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_SetError("Nunit test");
			Assert.AreEqual("Nunit test", Tao.Sdl.Sdl.SDL_GetError());
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ClearError()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_SetError("Nunit test");
			Assert.AreEqual("Nunit test", Tao.Sdl.Sdl.SDL_GetError());
			Tao.Sdl.Sdl.SDL_ClearError();
			Assert.AreEqual("", Tao.Sdl.Sdl.SDL_GetError());
			this.Quit();
		}

		#endregion SDL_error.h

		// SDL_events.h -- TODO

		#region SDL_getenv.h
		/// <summary>
		/// 
		/// </summary>
		[Test]
#if WIN32
		[ExpectedException(typeof(System.EntryPointNotFoundException))]
#endif
		public void PutEnv()
		{
			Assert.AreEqual(0, Tao.Sdl.Sdl.SDL_putenv("SDLTest"));
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
#if WIN32
		[ExpectedException(typeof(System.EntryPointNotFoundException))]
#endif
		public void GetEnv()
		{
			Tao.Sdl.Sdl.SDL_getenv("HOME");
			this.Quit();
		}
		#endregion SDL_getenv.h

		// SDL_joystick.h
		// SDL_keyboard.h
		// SDL_mouse.h
		// SDL_rwops.h

		#region SDL_timer.h
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GetTicks()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			int beforeGetTicks = Tao.Sdl.Sdl.SDL_GetTicks();
			Thread.Sleep(100);
			int afterGetTicks = Tao.Sdl.Sdl.SDL_GetTicks();
			//Console.WriteLine("GetTicks(): " + Tao.Sdl.Sdl.SDL_GetTicks().ToString());
			Assert.IsTrue(afterGetTicks - beforeGetTicks >= 100);
			this.Quit();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Delay()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			int beforeDelay = Sdl.SDL_GetTicks();
			//Console.WriteLine("Before Delay(): " + beforeDelay.ToString());
			Sdl.SDL_Delay(100);
			int afterDelay = Sdl.SDL_GetTicks();
			//Console.WriteLine("After Delay(): " + afterDelay.ToString());
			Assert.IsTrue(afterDelay - beforeDelay >= 100);
		}

		private int PrintTimerInterval(int interval)
		{
			int currentSetTimer = Tao.Sdl.Sdl.SDL_GetTicks();
			Console.WriteLine("Current SetTimer(): " + currentSetTimer.ToString());
			return interval;
		}


		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("This test crashes the nunit gui. The problem is with the delay")]
		public void SetTimer()
		{
			Tao.Sdl.Sdl.SDL_Quit();
			Tao.Sdl.Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_TIMER);
			int interval = 10;
			Tao.Sdl.Sdl.SDL_TimerCallback testDelegate;
			testDelegate = new Tao.Sdl.Sdl.SDL_TimerCallback(PrintTimerInterval);
			int beforeSetTimer = Tao.Sdl.Sdl.SDL_GetTicks();
			Console.WriteLine("Before SetTimer(): " + beforeSetTimer.ToString());
			Tao.Sdl.Sdl.SDL_SetTimer(interval, testDelegate);
			//Assert.IsTrue(interval < testDelegate(interval));
			//Thread.Sleep(9);
			//Tao.Sdl.Sdl.SDL_Delay(20);
			int afterSetTimer = Tao.Sdl.Sdl.SDL_GetTicks();
			Console.WriteLine("After SetTimer(): " + afterSetTimer.ToString());
			while (afterSetTimer - beforeSetTimer < 20)
			{
				afterSetTimer = Tao.Sdl.Sdl.SDL_GetTicks();
			}
			this.Quit();

		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Must finish SetTimer test")]
		public void SetTimerCancel()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_TIMER);
			Sdl.SDL_TimerCallback testDelegate;
			testDelegate = new Sdl.SDL_TimerCallback(PrintTimerInterval);
			int interval = 10;
			Sdl.SDL_SetTimer(interval, testDelegate);
			Sdl.SDL_SetTimer(0, null);
			this.Quit();

		}
		#endregion SDL_timer.h

		#region SDL_version.h
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LinkedVersion()
		{
			Sdl.SDL_version version = Sdl.SDL_Linked_Version();
			Assert.AreEqual(version.major.ToString() 
				+ "." + version.minor.ToString() 
				+ "." + version.patch.ToString(), "1.2.7");
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void CompiledVersion()
		{
			Assert.AreEqual(Sdl.SDL_COMPILEDVERSION, 1207);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void VersionAtLeast127()
		{
			Assert.IsTrue(Sdl.SDL_VERSION_ATLEAST(1,2,7));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void VersionAtLeast128()
		{
			Assert.IsFalse(Sdl.SDL_VERSION_ATLEAST(1,2,8));
		}
		#endregion SDL_version.h
	}
}
