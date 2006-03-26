/*
 * $RCSfile$
 * Copyright (C) 2004, 2005 David Hudson (jendave@yahoo.com)
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
using System.Threading;
using System.IO;

using SdlDotNet;
using Tao.Sdl;

namespace SdlDotNet
{
	/// <summary>
	/// Provides methods to access the sound system.
	/// You can obtain an instance of this class by accessing the 
	/// Mixer property of the main Sdl object.
	/// </summary>
	/// <remarks>
	/// Before instantiating an instance of Movie,
	/// you must call Mixer.Close() to turn off the default mixer.
	/// If you do not do this, any movie will play very slowly. 
	/// Smpeg uses a custom mixer for audio playback. 
	/// </remarks>
	public sealed class Mixer
	{
		//static private SdlMixer.ChannelFinishedDelegate ChannelFinishedDelegate;
		private const int DEFAULT_CHUNK_SIZE = 1024;
		private const int DEFAULT_NUMBER_OF_CHANNELS = 8;
		//private static ChannelList channelList = null;
		//private Music music = Music.Instance;
		
		static private byte distance;

		Mixer()
		{}

		static Mixer()
		{
            Initialize();
		}

		/// <summary>
		/// Closes and destroys this object
		/// </summary>
		public static void Close() 
		{
			Events.CloseMixer();
		}

		/// <summary>
		/// Start Mixer subsystem
		/// </summary>
		public static void Initialize()
		{
			if ((Sdl.SDL_WasInit(Sdl.SDL_INIT_AUDIO)) 
				== (int) SdlFlag.FalseValue)
			{
				if (Sdl.SDL_Init(Sdl.SDL_INIT_AUDIO) != (int) SdlFlag.Success)
				{
					throw SdlException.Generate();
				}	
			}
			Mixer.PrivateOpen();
		}

//		/// <summary>
//		/// Queries if the Mixer subsystem has been intialized.
//		/// </summary>
//		/// <remarks>
//		/// </remarks>
//		/// <returns>True if Mixer subsystem has been initialized, false if it has not.</returns>
//		public static bool IsInitialized
//		{
//			get
//			{
//				if ((Sdl.SDL_WasInit(Sdl.SDL_INIT_AUDIO) & Sdl.SDL_INIT_AUDIO) 
//					!= (int) SdlFlag.FalseValue)
//				{
//					return true;
//				}
//				else 
//				{
//					return false;
//				}
//			}
//		}

		/// <summary>
		/// Re-opens the sound system with default values.  
		/// You do not have to call this method
		/// in order to start using the Mixer object.
		/// </summary>
		public static void Open() 
		{
			Close();
			PrivateOpen();
		}

		/// <summary>
		/// Re-opens the sound-system. You do not have to call this method
		/// in order to start using the Mixer object.
		/// </summary>
		/// <param name="frequency">The frequency to mix at</param>
		/// <param name="format">The audio format to use</param>
		/// <param name="channels">
		/// The number of channels to allocate.  
		/// You will not be able to mix more than this number of samples.
		/// </param>
		/// <param name="chunkSize">The chunk size for samples</param>
		public static void Open(int frequency, AudioFormat format, int channels, int chunkSize) 
		{
			Close();
			PrivateOpen(frequency, format, channels, chunkSize);
		}

		private static void PrivateOpen() 
		{
			SdlMixer.Mix_OpenAudio(SdlMixer.MIX_DEFAULT_FREQUENCY, 
				unchecked((short)AudioFormat.Default), 
				(int) SoundChannel.Stereo, 
				DEFAULT_CHUNK_SIZE);
			ChannelsAllocated = DEFAULT_NUMBER_OF_CHANNELS;
		}
		private static void PrivateOpen(
			int frequency, AudioFormat format, int channels, int chunksize) 
		{
			SdlMixer.Mix_OpenAudio(frequency, (short)format, channels, chunksize);
		}

		/// <summary>
		/// Creates sound channel
		/// </summary>
		/// <param name="index">Index of new channel</param>
		/// <returns>new Channel</returns>
		public static Channel CreateChannel(int index)
		{
			if (index < 0 || index >= Mixer.ChannelsAllocated)
			{
				throw new SdlException();
			}
			else
			{
				return new Channel(index);
			}
		}

		/// <summary>
		/// Loads a .wav file into memory
		/// </summary>
		/// <param name="file">sound file name</param>
		/// <returns>Sound object</returns>
		public static Sound Sound(string file)
		{
			return new Sound(file);
		}

		/// <summary>
		/// Loads a .wav file into memory
		/// </summary>
		/// <param name="file">The filename to load</param>
        /// <param name="size">Output long variable for the size of the sound object.</param>
		/// <returns>A new Sound object</returns>
		internal static IntPtr LoadWav(string file, out long size) 
		{
			IntPtr p = SdlMixer.Mix_LoadWAV_RW(Sdl.SDL_RWFromFile(file, "rb"), 1);
			if (p == IntPtr.Zero)
			{
				throw SdlException.Generate();
			}
            size = new FileInfo(file).Length;
			return p;
		}

		/// <summary>
		/// Loads a .wav file from a byte array
		/// </summary>
		/// <param name="data">The data to load</param>
		/// <returns>A new Sound object</returns>
		public static Sound Sound(byte[] data)
		{
			return new Sound(data);
		}

		/// <summary>
		/// Loads a .wav file from a byte array
		/// </summary>
		/// <param name="data">The data to load</param>
        /// <param name="size">Output variable for the size of the sound object.</param>
		/// <returns>A new Sound object</returns>
        internal static IntPtr LoadWav(byte[] data, out long size) 
		{
			IntPtr p = SdlMixer.Mix_LoadWAV_RW(Sdl.SDL_RWFromMem(data, data.Length), 1);
			if (p == IntPtr.Zero)
			{
				throw SdlException.Generate();
			}
            size = data.Length;
			return p;
		}

		/// <summary>
		/// Loads a music sample from a filename returning the pointer to the sample.
		/// </summary>
		/// <param name="filename">The file path to load.</param>
		/// <returns>The IntPtr handle to the music sample in memory.</returns>
		/// <exception cref="SdlException">Thrown if an error occurs when loading the sample.</exception>
		internal static IntPtr LoadMusic(string filename)
		{
			IntPtr handle = SdlMixer.Mix_LoadMUS(filename);
			if (handle == IntPtr.Zero)
			{
				throw SdlException.Generate();
			}
			return handle;
		}

		/// <summary>
		/// Changes the number of channels allocated for mixing
		/// </summary>
		/// <returns>The number of channels allocated</returns>
		public static int ChannelsAllocated
		{
			get
			{
				return SdlMixer.Mix_AllocateChannels(-1);
			}
			set
			{
				SdlMixer.Mix_AllocateChannels(value);
			}
		}

		/// <summary>
		/// These channels will be resrved
		/// </summary>
		/// <param name="numberOfChannels">number of channels to reserve</param>
		/// <returns>
		/// Number of channels actually reserved. This may be fewer than the number requested.
		/// </returns>
		public static int ReserveChannels(int numberOfChannels)
		{
			return SdlMixer.Mix_ReserveChannels(numberOfChannels);
		}

		/// <summary>
		/// Stop reserving any channels.
		/// </summary>
		public static void CancelReserveChannels()
		{
			SdlMixer.Mix_ReserveChannels(0);
		}

		/// <summary>
		/// Returns the index of an available channel
		/// </summary>
		/// <returns>Index of available channel</returns>
		public static int FindAvailableChannel()
		{
			return SdlMixer.Mix_GroupAvailable(-1);
		}

		/// <summary>
		/// Sets the volume for all channels
		/// </summary>
		/// <param name="volume">A new volume value, between 0 and 128 inclusive</param>
		/// <returns>New average channel volume</returns>
		public static int SetAllChannelsVolume(int volume) 
		{
			return SdlMixer.Mix_Volume(-1, volume);
		}
	
		/// <summary>
		/// Pauses playing on all channels
		/// </summary>
		public static void Pause() 
		{
			SdlMixer.Mix_Pause(-1);
		}
	
		/// <summary>
		/// Resumes playing on all paused channels
		/// </summary>
		public static void Resume() 
		{
			SdlMixer.Mix_Resume(-1);
		}

		
		/// <summary>
		/// Stop playing on all channels
		/// </summary>
		public static void Stop() 
		{
			SdlMixer.Mix_HaltChannel(-1);
		}

		/// <summary>
		/// Stop playing on all channels after a specified time interval
		/// </summary>
		/// <param name="milliseconds">
		/// The number of milliseconds to stop playing after
		/// </param>
		public static void Expire(int milliseconds) 
		{
			SdlMixer.Mix_ExpireChannel(-1, milliseconds);
		}

		/// <summary>
		/// Fades out all channels
		/// </summary>
		/// <param name="milliseconds">
		/// The number of milliseconds to fade out for
		/// </param>
		/// <returns>The number of channels fading out</returns>
		public static int Fadeout(int milliseconds) 
		{
			return SdlMixer.Mix_FadeOutChannel(-1, milliseconds);
		}

		/// <summary>
		/// Returns the number of currently playing channels
		/// </summary>
		/// <returns>The number of channels playing</returns>
		public static int NumberOfChannelsPlaying() 
		{
			return SdlMixer.Mix_Playing(-1);
		}

		/// <summary>
		/// Returns the number of paused channels
		/// </summary>
		/// <remarks>
		/// Number of channels paused.
		/// </remarks>
		/// <returns>The number of channels paused</returns>
		public static int NumberOfChannelsPaused() 
		{
			return SdlMixer.Mix_Paused(-1);
		}

		/// <summary>
		/// Sets the panning (stereo attenuation) for all channels
		/// </summary>
		/// <param name="left">
		/// A left speaker value from 0-255 inclusive
		/// </param>
		/// <param name="right">
		/// A right speaker value from 0-255 inclusive
		/// </param>
		public static void SetPanning(int left, int right) 
		{
			if (SdlMixer.Mix_SetPanning(-1, (byte)left, (byte)right) == 0)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Sets the distance (attenuate sounds based on distance 
		/// from listener) for all channels
		/// </summary>
		public static byte Distance 
		{
			set
			{
				if (SdlMixer.Mix_SetDistance(-1, value) == 0)
				{
					throw SdlException.Generate();
				}
				distance = value;
			}
			get
			{
				return distance;
			}
		}

		/// <summary>
		/// Sets the "position" of a sound (approximate '3D' audio) 
		/// for all channels
		/// </summary>
		/// <param name="angle">The angle of the sound, between 0 and 359,
		///  0 = directly in front</param>
		/// <param name="distance">
		/// The distance of the sound from 0-255 inclusive
		/// </param>
		public static void SetPosition(int angle, int distance) 
		{
			if (SdlMixer.Mix_SetPosition(-1, (short)angle, (byte)distance) == 0)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Flips the left and right stereo for all channels
		/// </summary>
		/// <param name="flip">True to flip, False to reset to normal</param>
		public static void ReverseStereo(bool flip) 
		{
			if (SdlMixer.Mix_SetReverseStereo(-1, flip?1:0) == 0)
			{
				throw SdlException.Generate();
			}
		}
	}
}
