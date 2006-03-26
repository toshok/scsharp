using System;
using System.Threading;
using NUnit.Framework;
using Tao.Sdl;
using System.Runtime.InteropServices;

namespace SdlDotNet.Tests
{
	#region SDL_mixer.h
	/// <summary>
	/// SDL Tests.
	/// </summary>
	[TestFixture]
	public class SdlTestMixer
	{
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
		[Test]
		public void LinkedVersion()
		{
			Sdl.SDL_version version = SdlMixer.Mix_Linked_Version();
			//Console.WriteLine("Mixer version: " + version.ToString());
			Assert.AreEqual(version.major.ToString() 
				+ "." + version.minor.ToString() 
				+ "." + version.patch.ToString(), "1.2.5");
		}
		/// <summary>
		/// 
		/// </summary>
		private void InitAudio()
		{
			QuitAudio();
			Sdl.SDL_Init(Sdl.SDL_INIT_AUDIO);
			int results = SdlMixer.Mix_OpenAudio(
				SdlMixer.MIX_DEFAULT_FREQUENCY, 
				(short) SdlMixer.MIX_DEFAULT_FORMAT, 
				2, 
				1024);
			Assert.AreEqual(results,0);
		}
		/// <summary>
		/// 
		/// </summary>
		private void QuitAudio()
		{
			SdlMixer.Mix_CloseAudio();
			Sdl.SDL_Quit();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void OpenAudio()
		{
			Sdl.SDL_Quit();
			Sdl.SDL_Init(Sdl.SDL_INIT_AUDIO);
			int results = SdlMixer.Mix_OpenAudio(
				SdlMixer.MIX_DEFAULT_FREQUENCY, 
				(short) SdlMixer.MIX_DEFAULT_FORMAT, 
				2, 
				1024);
			Assert.AreEqual(results,0);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void AllocateChannels()
		{
			InitAudio();	
			int results = SdlMixer.Mix_AllocateChannels(16);
			Assert.AreEqual(results, 16);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void QuerySpec()
		{
			InitAudio();
			int frequency;
			short format;
			int channels;
			int results = SdlMixer.Mix_QuerySpec(out frequency, out format, out channels);
//			Console.WriteLine("freq: " + frequency.ToString());
//			Console.WriteLine("format: " + format.ToString());
//			Console.WriteLine("chan: " + channels.ToString());
//			Console.WriteLine("results: " + results.ToString());
			Assert.AreEqual(frequency, SdlMixer.MIX_DEFAULT_FREQUENCY);
			Assert.AreEqual(format, (short) SdlMixer.MIX_DEFAULT_FORMAT);
			Assert.AreEqual(channels, 2);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadWav_RW()
		{
			InitAudio();		
			IntPtr resultPtr = SdlMixer.Mix_LoadWAV_RW(Sdl.SDL_RWFromFile("test.wav", "rb"), 1);
			Assert.IsFalse(resultPtr == IntPtr.Zero);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadWav()
		{
			InitAudio();		
			IntPtr resultPtr = SdlMixer.Mix_LoadWAV("test.wav");
			Assert.IsFalse(resultPtr == IntPtr.Zero);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadMusWav()
		{
			InitAudio();		
			IntPtr resultPtr = SdlMixer.Mix_LoadMUS("test.wav");
			Assert.IsFalse(resultPtr == IntPtr.Zero);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadMusMp3()
		{
			InitAudio();		
			IntPtr resultPtr = SdlMixer.Mix_LoadMUS("test.mp3");
			Assert.IsFalse(resultPtr == IntPtr.Zero);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void LoadMusOgg()
		{
			InitAudio();		
			IntPtr resultPtr = SdlMixer.Mix_LoadMUS("test.ogg");
			Assert.IsFalse(resultPtr == IntPtr.Zero);
			QuitAudio();
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SetGetError()
		{
			string error = "Hi there";
			SdlMixer.Mix_SetError(error);
			Assert.AreEqual(SdlMixer.Mix_GetError(), error);
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void QuickLoad_Wav()
		{
			InitAudio();		
			IntPtr resultPtr = SdlMixer.Mix_QuickLoad_WAV(Sdl.SDL_RWFromFile("test.wav", "rb"));
			Assert.IsFalse(resultPtr == IntPtr.Zero);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void QuickLoad_Raw()
		{
			InitAudio();		
			IntPtr resultPtr = SdlMixer.Mix_QuickLoad_RAW(Sdl.SDL_RWFromFile("test.wav", "rb"), 1000);
			Assert.IsFalse(resultPtr == IntPtr.Zero);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FreeChunk()
		{
			InitAudio();		
			IntPtr wavPtr = Sdl.SDL_RWFromFile("test.wav", "rb");
			SdlMixer.Mix_FreeChunk(wavPtr);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FreeMusic()
		{
			InitAudio();		
			IntPtr wavPtr = Sdl.SDL_RWFromFile("test.wav", "rb");
			SdlMixer.Mix_FreeMusic(wavPtr);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GetMusicType()
		{
			InitAudio();	
			IntPtr resultPtr = SdlMixer.Mix_LoadMUS("test.wav");
			int musicType = SdlMixer.Mix_GetMusicType(resultPtr);
			Console.WriteLine("musictype:" + musicType);
			//Assert.IsFalse(resultPtr == IntPtr.Zero);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void SetPostMix()
		{

		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void HookMusic()
		{

		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void HookMusicFinished()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void GetMusicHookData()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void ChannelFinished()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void RegisterEffect()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void UnregisterEffect()
		{

		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void UnregisterAllEffects()
		{

		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SetPanning()
		{
			InitAudio();	
			int result = SdlMixer.Mix_SetPanning(1, 255,127);
			Assert.IsTrue(result != 0);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SetPosition()
		{
			InitAudio();	
			int result = SdlMixer.Mix_SetPosition(1, 90, 100);
			Assert.IsTrue(result != 0);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SetDistance()
		{
			InitAudio();	
			int result = SdlMixer.Mix_SetDistance(1, 140);
			Assert.IsTrue(result != 0);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void SetReverseStereo()
		{
			InitAudio();	
			int result = SdlMixer.Mix_SetReverseStereo(SdlMixer.MIX_CHANNEL_POST, 1);
			Assert.IsTrue(result != 0);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ReserveChannels()
		{
			InitAudio();	
			int result = SdlMixer.Mix_ReserveChannels(1);
			//Console.WriteLine("ReserveChannels: " + result.ToString());
			Assert.IsTrue(result == 1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GroupChannel()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannel(1, 1);
			//Console.WriteLine("ReserveChannels: " + result.ToString());
			Assert.IsTrue(result == 1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GroupChannels()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			//Console.WriteLine("ReserveChannels: " + result.ToString());
			Assert.IsTrue(result == 8);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GroupAvailable()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannel(1, 1);
			result = SdlMixer.Mix_GroupAvailable(1);
			//Console.WriteLine("ReserveChannels: " + result.ToString());
			Assert.IsTrue(result != -1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GroupCount()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannel(1, 1);
			result = SdlMixer.Mix_GroupCount(1);
			//Console.WriteLine("ReserveChannels: " + result.ToString());
			Assert.IsTrue(result == 1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GroupOldest()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			result = SdlMixer.Mix_GroupOldest(1);
			//Console.WriteLine("GroupOldest: " + result.ToString());
			Assert.IsTrue(result == -1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void GroupNewer()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			result = SdlMixer.Mix_GroupOldest(1);
			//Console.WriteLine("GroupOldest: " + result.ToString());
			Assert.IsTrue(result == -1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void PlayChannelTimed()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			IntPtr chunkPtr = SdlMixer.Mix_LoadWAV("test.wav");
			result = SdlMixer.Mix_PlayChannelTimed(-1, chunkPtr, -1, 500);
			Thread.Sleep(500);
			Console.WriteLine("PlayChannelTimed: " + result.ToString());
			Assert.IsTrue(result != -1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void PlayChannel()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			IntPtr chunkPtr = SdlMixer.Mix_LoadWAV("test.wav");
			result = SdlMixer.Mix_PlayChannel(-1, chunkPtr, -1);
			Thread.Sleep(500);
			Console.WriteLine("PlayChannel: " + result.ToString());
			Assert.IsTrue(result != -1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void PlayMusic()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_PlayMusic( chunkPtr, -1);
			Console.WriteLine("PlayMusic: " + result.ToString());
			Assert.IsTrue(result != -1);
			Thread.Sleep(1000);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FadeInMusic()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_FadeInMusic( chunkPtr, -1, 2);
			Console.WriteLine("PlayMusic: " + result.ToString());
			Assert.IsTrue(result != -1);
			Thread.Sleep(5000);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void FadeInMusicPos()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_FadeInMusicPos( chunkPtr, -1, 2, 1);
			Console.WriteLine("PlayMusic: " + result.ToString());
			Assert.IsTrue(result != -1);
			Thread.Sleep(5000);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void FadeInChannelTimed()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			IntPtr chunkPtr = SdlMixer.Mix_LoadWAV("test.wav");
			result = SdlMixer.Mix_FadeInChannelTimed(1, chunkPtr, -1, 0,-1);
			Thread.Sleep(500);
			Console.WriteLine("PlayChannel: " + result.ToString());
			Assert.IsTrue(result != -1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void FadeInChannel()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			IntPtr chunkPtr = SdlMixer.Mix_LoadWAV("test.wav");
			result = SdlMixer.Mix_FadeInChannel(1, chunkPtr, -1, 0);
			Thread.Sleep(500);
			Console.WriteLine("PlayChannel: " + result.ToString());
			Assert.IsTrue(result != -1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Volume()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			IntPtr chunkPtr = SdlMixer.Mix_LoadWAV("test.wav");
			result = SdlMixer.Mix_Volume(1, SdlMixer.MIX_MAX_VOLUME);
			Console.WriteLine("Volume: " + result.ToString());
			Assert.IsTrue(result == SdlMixer.MIX_MAX_VOLUME);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void VolumeChunk()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			IntPtr chunkPtr = SdlMixer.Mix_LoadWAV("test.wav");
			result = SdlMixer.Mix_VolumeChunk(chunkPtr, SdlMixer.MIX_MAX_VOLUME);
			Console.WriteLine("Volume: " + result.ToString());
			Assert.IsTrue(result == SdlMixer.MIX_MAX_VOLUME);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void VolumeMusic()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_VolumeMusic(SdlMixer.MIX_MAX_VOLUME);
			Console.WriteLine("Volume: " + result.ToString());
			Assert.IsTrue(result == SdlMixer.MIX_MAX_VOLUME);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void HaltChannel()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			result = SdlMixer.Mix_HaltChannel(1);
			//Console.WriteLine("HaltChannel: " + result.ToString());
			Assert.IsTrue(result == 0);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void HaltGroup()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			result = SdlMixer.Mix_HaltGroup(1);
			//Console.WriteLine("HaltChannel: " + result.ToString());
			Assert.IsTrue(result == 0);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void HaltMusic()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_HaltMusic();
			Assert.IsTrue(result == 0);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ExpireChannel()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			result = SdlMixer.Mix_ExpireChannel(1, 100);
			//Console.WriteLine("HaltChannel: " + result.ToString());
			Assert.IsTrue(result == 1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void FadeOutChannel()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			IntPtr chunkPtr = SdlMixer.Mix_LoadWAV("test.wav");
			result = SdlMixer.Mix_FadeOutChannel(1, 100);
			Thread.Sleep(500);
			Console.WriteLine("PlayChannel: " + result.ToString());
			Assert.IsTrue(result != -1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void  FadeOutGroup()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannel(1, 1);
			result = SdlMixer.Mix_FadeOutGroup(1, 100);
			Thread.Sleep(100);
			//Console.WriteLine("ReserveChannels: " + result.ToString());
			Assert.IsTrue(result == 1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void FadeOutMusic()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_PlayMusic( chunkPtr, -1);
			result = SdlMixer.Mix_FadeOutMusic(1000);
			Thread.Sleep(2000);
			Console.WriteLine("PlayMusic: " + result.ToString());
			Assert.IsTrue(result == 1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Works fine on its own, but something wrong when it runs as a test suite")]
		public void FadingMusic()
		{
			InitAudio();	
			int result;
			int resultFading;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_PlayMusic( chunkPtr, -1);
			resultFading = SdlMixer.Mix_FadingMusic();
			//Console.WriteLine("FadingMusic1: " + resultFading.ToString());
			Assert.AreEqual(resultFading, SdlMixer.MIX_NO_FADING);
			result = SdlMixer.Mix_FadeOutMusic(1000);
			resultFading = SdlMixer.Mix_FadingMusic();
			Assert.AreEqual(resultFading, SdlMixer.MIX_FADING_OUT);
			//Console.WriteLine("FadingMusic2: " + resultFading.ToString());
			Thread.Sleep(2000);
			resultFading = SdlMixer.Mix_FadingMusic();
			Assert.AreEqual(resultFading, SdlMixer.MIX_NO_FADING);
			//Console.WriteLine("FadingMusic: " + resultFading.ToString());
			Assert.IsTrue(result == 1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Something wrong.")]
		public void FadingChannel()
		{
			InitAudio();	
			int result;
			int resultFading;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_PlayChannel(1, chunkPtr, -1);
			resultFading = SdlMixer.Mix_FadingChannel(1);
			//Console.WriteLine("FadingMusic1: " + resultFading.ToString());
			Assert.AreEqual(resultFading, SdlMixer.MIX_NO_FADING);
			result = SdlMixer.Mix_FadeOutChannel(1, 1000);
			resultFading = SdlMixer.Mix_FadingChannel(1);
			Assert.AreEqual(resultFading, SdlMixer.MIX_FADING_OUT);
			//Console.WriteLine("FadingMusic2: " + resultFading.ToString());
			Thread.Sleep(2000);
			resultFading = SdlMixer.Mix_FadingChannel(1);
			Assert.AreEqual(resultFading, SdlMixer.MIX_NO_FADING);
			//Console.WriteLine("FadingMusic: " + resultFading.ToString());
			Assert.IsTrue(result == 1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Pause()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_PlayChannel(1, chunkPtr, -1);
			SdlMixer.Mix_Pause(-1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Resume()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_PlayChannel(1, chunkPtr, -1);
			SdlMixer.Mix_Pause(-1);
			SdlMixer.Mix_Resume(-1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not Finished")]
		public void Paused()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_PlayChannel(1, chunkPtr, -1);
			SdlMixer.Mix_Pause(1);
			result = SdlMixer.Mix_Paused(-1);
			Assert.AreEqual(result, 1);
			result = SdlMixer.Mix_Paused(-1);
			Assert.AreEqual(result, 1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void PauseMusic()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_PlayMusic( chunkPtr, -1);
			SdlMixer.Mix_PauseMusic();
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ResumeMusic()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_PlayMusic( chunkPtr, -1);
			SdlMixer.Mix_PauseMusic();
			SdlMixer.Mix_ResumeMusic();
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RewindMusic()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.ogg");
			result = SdlMixer.Mix_PlayMusic( chunkPtr, -1);
			SdlMixer.Mix_RewindMusic();
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void PausedMusic()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_PlayMusic( chunkPtr, -1);
			SdlMixer.Mix_PauseMusic();
			result = SdlMixer.Mix_PausedMusic();
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void SetMusicPosition()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_SetMusicPosition(1000);
			//Console.WriteLine("PlayMusic: " + result.ToString());
			Assert.IsTrue(result != -1);
			Thread.Sleep(5000);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not finished")]
		public void Playing()
		{
			InitAudio();	
			int result = SdlMixer.Mix_GroupChannels(0, 7, 1);
			IntPtr chunkPtr = SdlMixer.Mix_LoadWAV("test.wav");
			result = SdlMixer.Mix_PlayChannel(-1, chunkPtr, -1);
			Thread.Sleep(500);
			//Console.WriteLine("PlayChannel: " + result.ToString());
			result = SdlMixer.Mix_Playing(1);
			Assert.IsTrue(result != -1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void PlayingMusic()
		{
			InitAudio();	
			int result;
			IntPtr chunkPtr = SdlMixer.Mix_LoadMUS("test.wav");
			result = SdlMixer.Mix_PlayMusic( chunkPtr, -1);
			Console.WriteLine("PlayMusic: " + result.ToString());
			result = SdlMixer.Mix_PlayingMusic();
			Assert.IsTrue(result == 1);
			Thread.Sleep(1000);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not Finished")]
		public void SetPlayingCMD()
		{
			InitAudio();	
			int result;
			//result = SdlMixer.Mix_PlayMusic( chunkPtr, -1);
			result = SdlMixer.Mix_SetMusicCMD("test");
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not Finished")]
		public void GetChunk()
		{
			InitAudio();	
			IntPtr resultPtr;
			//result = SdlMixer.Mix_PlayMusic( chunkPtr, -1);
			resultPtr= SdlMixer.Mix_GetChunk(1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not Finished")]
		public void SetSynchroValue()
		{
			InitAudio();	
			int result;
			result = SdlMixer.Mix_SetSynchroValue(1);
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Ignore("Not Finished")]
		public void GetSynchroValue()
		{
			InitAudio();	
			int result;
			result = SdlMixer.Mix_GetSynchroValue();
			QuitAudio();
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void CloseAudio()
		{
			InitAudio();	
			SdlMixer.Mix_CloseAudio();
			Sdl.SDL_Quit();
		}
	}
	#endregion SDL_mixer.h
}