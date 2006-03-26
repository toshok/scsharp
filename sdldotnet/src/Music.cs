/*
 * $RCSfile: Music.cs,v $
 * Copyright (C) 2004, 2005 David Hudson (jendave@yahoo.com)
 * Copyright (C) 2005 Rob Loach (http://www.robloach.net)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using Tao.Sdl;

namespace SdlDotNet 
{
	/// <summary>
	/// Represents a music sample.  Music is generally longer than a sound effect sample,
	/// however it can also be compressed e.g. by Ogg Vorbis
	/// </summary>
	/// <example>
	/// <code>
	/// Music techno = new Music("techno.mp3");
	/// Music jazz = new Music("jazz.mid");
	/// 
	/// techno.Play();
	/// 
	/// if(Music.CurrentMusic == techno)
	///		jazz.Play();
	///		
	///	Music.FadeOut(1000);
	///	Music.Volume = 50;
	///	
	///	techno.FadeIn(1, 500);
	///	techno.QueuedMusic = jazz;  // Play jazz after techno finishes playing
	///	jazz.QueuedMusic = techno;  // Play techno after jazz finishes playing
	///	
	///	Music.EnableMusicFinishedCallback(); // Enable processing queues.
	/// </code>
	/// </example>
	public sealed class Music : BaseSdlResource
	{
		private static SdlMixer.MusicFinishedDelegate MusicFinishedDelegate;

		private string m_FileName = "";

		/// <summary>
		/// Gets the filename of the music sample.
		/// </summary>
		public string FileName
		{
			get
			{
				return m_FileName;
			}
		}

		static Music()
		{
			Mixer.Initialize();
		}

		private static Music m_CurrentMusic;

		/// <summary>
		/// Gets and sets the currently loaded music sample.
		/// </summary>
		public static Music CurrentMusic
		{
			get
			{
				return m_CurrentMusic;
			}
			set
			{
				m_CurrentMusic = value;
			}
		}

		private Music m_QueuedMusic;
		/// <summary>
		/// Gets and sets the next queued music sample after this one completes.
		/// </summary>
		/// <remarks>
		/// You must call Music.EnableMusicFinishedCallback before this can work.
		/// </remarks>
		public Music QueuedMusic
		{
			get
			{
				return m_QueuedMusic;
			}
			set
			{
				m_QueuedMusic = value;
			}
		}

		/// <summary>
		/// Loads a music sample from a file.
		/// </summary>
		/// <param name="fileName">The file path to load from.</param>
		public Music(string fileName)
		{
			this.Handle = Mixer.LoadMusic(fileName);
			m_FileName = fileName;
		}

		/// <summary>
		/// Closes Music handle
		/// </summary>
		protected override void CloseHandle() 
		{
			try
			{
				if (this.Handle != IntPtr.Zero)
				{
					SdlMixer.Mix_FreeMusic(this.Handle);
				}
			}
			catch (NullReferenceException)
			{
			}
			finally
			{
				this.Handle = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Plays the music sample
		/// </summary>
		public void Play() 
		{
			Play(1);
		}

		/// <summary>
		/// Plays the music sample
		/// </summary>
		public void Play(bool continuous) 
		{
			if (continuous == true)
			{
				Play(-1);
			}
			else
			{
				Play(1);
			}
		}

		/// <summary>
		/// Plays the music sample
		/// </summary>
		/// <param name="numberOfTimes">
		/// The number of times to play. 
		/// Specify 1 to play a single time, -1 to loop forever.
		/// </param>
		public void Play(int numberOfTimes) 
		{
			m_CurrentMusic = this;
			if (SdlMixer.Mix_PlayMusic(this.Handle, numberOfTimes) != 0)
				throw SdlException.Generate();
		}

		/// <summary>
		/// Plays the music sample and fades it in
		/// </summary>
		/// <param name="numberOfTimes">
		/// The number of times to play. 
		/// Specify 1 to play a single time, -1 to loop forever.
		/// </param>
		/// <param name="milliseconds">
		/// The number of milliseconds to fade in for
		/// </param>
		public void FadeIn(int numberOfTimes, int milliseconds) 
		{
			m_CurrentMusic = this;
			if (SdlMixer.Mix_FadeInMusic(this.Handle, numberOfTimes, milliseconds) != 0)
				throw SdlException.Generate();
		}
		/// <summary>
		/// Plays the music sample, starting from a specific 
		/// position and fades it in
		/// </summary>
		/// <param name="numberOfTimes">
		/// The number of times to play.
		///  Specify 1 to play a single time, -1 to loop forever.
		///  </param>
		/// <param name="milliseconds">
		/// The number of milliseconds to fade in for
		/// </param>
		/// <param name="position">
		/// A format-defined position value. 
		/// For Ogg Vorbis, this is the number of seconds from the
		///  beginning of the song
		///  </param>
		public void FadeInPosition(int numberOfTimes, int milliseconds, double position) 
		{
			m_CurrentMusic = this;
			if (SdlMixer.Mix_FadeInMusicPos(this.Handle, 
				numberOfTimes, milliseconds, position) != 0)
			{
				throw SdlException.Generate();
			}
		}
		/// <summary>
		/// Sets the music volume between 0 and 128.
		/// </summary>
		public static int Volume
		{
			get
			{
				return SdlMixer.Mix_VolumeMusic(-1);
			}
			set
			{
				SdlMixer.Mix_VolumeMusic(value);
			}
		}

		/// <summary>
		/// Pauses the music playing
		/// </summary>
		public static void Pause()
		{
			SdlMixer.Mix_PauseMusic();
		}
		/// <summary>
		/// Resumes paused music
		/// </summary>
		public static void Resume()
		{
			SdlMixer.Mix_ResumeMusic();
		}
		/// <summary>
		/// Resets the music position to the beginning of the sample
		/// </summary>
		public static void Rewind() 
		{
			SdlMixer.Mix_RewindMusic();
		}

		/// <summary>
		/// Gets the format of the music data type.
		/// </summary>
		public MusicType MusicType
		{
			get
			{
				return (MusicType) SdlMixer.Mix_GetMusicType(this.Handle);
			}
		}

		/// <summary>
		/// Sets the music position to a format-defined value.
		/// For Ogg Vorbis and mp3, this is the number of seconds 
		/// from the beginning of the song
		/// </summary>
		/// <param name="musicPosition">
		/// Number of seconds from beginning of song
		/// </param>
		public static void Position(double musicPosition) 
		{
			if (m_CurrentMusic.MusicType == MusicType.Mp3)
			{
				Rewind();
			}
			if (SdlMixer.Mix_SetMusicPosition(musicPosition) != 0)
			{
				throw SdlException.Generate();
			}
		}
		/// <summary>
		/// Stops playing music
		/// </summary>
		public static void Stop() 
		{
			SdlMixer.Mix_HaltMusic();
		}
		/// <summary>
		/// Fades out music
		/// </summary>
		/// <param name="milliseconds">
		/// The number of milliseconds to fade out for
		/// </param>
		public static void Fadeout(int milliseconds) 
		{
			if (SdlMixer.Mix_FadeOutMusic(milliseconds) != 1)
			{
				throw SdlException.Generate();
			}
		}
		/// <summary>
		/// Gets a flag indicating whether or not music is playing
		/// </summary>
		public static bool IsPlaying
		{
			get
			{
				return (SdlMixer.Mix_PlayingMusic() != 0);
			}
		}
		/// <summary>
		/// Gets a flag indicating whether or not music is paused
		/// </summary>
		public static bool IsPaused 
		{
			get
			{
				return (SdlMixer.Mix_PausedMusic() != 0);
			}
		}
		/// <summary>
		/// Gets a flag indicating whether or not music is fading
		/// </summary>
		public static bool IsFading
		{
			get
			{
				return (SdlMixer.Mix_FadingMusic() != 0);
			}
		}

		/// <summary>
		/// For performance reasons, you must call this method
		/// to enable the Events.ChannelFinished and 
		/// Events.MusicFinished events
		/// </summary>
		public static void EnableMusicFinishedCallback() 
		{
			MusicFinishedDelegate = new SdlMixer.MusicFinishedDelegate(MusicFinished);
			SdlMixer.Mix_HookMusicFinished(MusicFinishedDelegate);
			Events.MusicFinished += new MusicFinishedEventHandler(Events_MusicFinished);
		}

		/// <summary>
		/// Returns a System.String that represents the current Music object.
		/// </summary>
		/// <returns>
		/// A System.String that represents the Music object (the filename).
		/// </returns>
		public override string ToString()
		{
			return m_FileName;
		}


		/// <summary>
		/// Called upon when the music sample finishes.
		/// </summary>
		private static void MusicFinished() 
		{
			Events.NotifyMusicFinished();
		}

		/// <summary>
		/// Private method to process the next queued music file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void Events_MusicFinished(object sender, MusicFinishedEventArgs e)
		{
			if(Music.IsPlaying == false)
			{
				if(m_CurrentMusic.m_QueuedMusic != null)
				{
					m_CurrentMusic = m_CurrentMusic.m_QueuedMusic;
					m_CurrentMusic.Play();
				}
			}
		}
	}
}