/*
 * $RCSfile: Sound.cs,v $
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
using System.Runtime.InteropServices;
using Tao.Sdl;

namespace SdlDotNet 
{
	/// <summary>
	/// Triggered for a sound event.
	/// </summary>
	public delegate void SoundEventHandler(object sender, SoundEventArgs e);
	/// <summary>
	/// Represents a sound sample.
	/// </summary>
	/// <example>
	/// <code>
	/// // Load the sound
	/// Sound boing = new Sound("boing.wav");
	/// 
	/// // Play the sound
	/// boing.Play();
	/// boing.Volume = 50;
	/// 
	/// // Play the sound on the right side only
	/// boing.Play().SetPanning(0, 255);
	/// 
	/// // Fade out the sound in 500 milliseconds
	/// boing.FadeOut(500);
	/// </code>
	/// </example>
	public class Sound : BaseSdlResource 
	{
		private int channels;
		private bool disposed;
		private long size;
		/// <summary>
		/// Triggered when there was an event passed to the sound sample (ex. the sound stopped)
		/// </summary>
		public event SoundEventHandler SoundEvent;

//        /// <summary>
//        /// Internal constructor to assemble a Sound object from the handle and size.
//        /// </summary>
//        /// <param name="handle">The handle</param>
//        /// <param name="size">The size of the sound.</param>
//		internal Sound(IntPtr handle, long size) 
//		{
//			this.Handle = handle;
//			this.size = size;
//		}

		/// <summary>
		/// Loads a .wav file into memory.
		/// </summary>
		/// <param name="file">The file to load into memory.</param>
		public Sound(string file)
		{
            this.Handle = Mixer.LoadWav(file, out this.size);
		}

		/// <summary>
		/// Loads sound from a byte array.
		/// </summary>
		/// <param name="data">The sound byte information.</param>
		public Sound(byte[] data)
		{
            this.Handle = Mixer.LoadWav(data, out this.size);
		}

		/// <summary>
		/// Destroys the surface object and frees its memory
		/// </summary>
		/// <param name="disposing">
		/// If true, dispose all unmanaged resources
		/// </param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.disposed)
				{
					if (disposing)
					{
					}
					CloseHandle();
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		/// Closes sound handle
		/// </summary>
		protected override void CloseHandle() 
		{
			try
			{
				if (this.Handle != IntPtr.Zero)
				{
					SdlMixer.Mix_FreeChunk(this.Handle);
				}
			}
			catch (NullReferenceException)
			{
				this.Handle = IntPtr.Zero;
			}
			finally
			{
				this.Handle = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Gets and sets the volume of the sound. 
		/// Should be between 0 and 128 inclusive.
		/// </summary>
		public int Volume
		{
			get
			{
				int result = SdlMixer.Mix_VolumeChunk(this.Handle, -1);
				GC.KeepAlive(this);
				return result;
			}
			set
			{
				if (value >= 0 && value <= SdlMixer.MIX_MAX_VOLUME)
				{
					SdlMixer.Mix_VolumeChunk(this.Handle, value);
				}
				GC.KeepAlive(this);
			}
		}

		/// <summary>
		/// Returns sound as an array of bytes.
		/// </summary>
		public byte[] Array()
		{
				byte[] array = new byte[this.size];
				Marshal.Copy(this.Handle, array, 0, (int)this.size);
				return array;
		}

		/// <summary>
		/// Plays the sound.
		/// </summary>
		/// <returns>The channel used to play the sound.</returns>
		public Channel Play()
		{
			return this.Play(0);
		}

		/// <summary>
		/// Plays the sound for a desired number of loops.
		/// </summary>
		/// <param name="loops">The number of loops to play.</param>
		/// <returns>The channel used to play the sound.</returns>
		public Channel Play(int loops) 
		{
			return this.Play(loops, (int) SdlFlag.InfiniteLoop);
		}

		/// <summary>
		/// Plays the sound.
		/// </summary>
		/// <param name="loopIndefinitely">
		/// True to play sound indefinately.
		/// </param>
		/// <returns>The channel used to play the sound.</returns>
		public Channel Play(bool loopIndefinitely) 
		{
			if (loopIndefinitely == true)
			{
				return this.Play(-1, (int) SdlFlag.InfiniteLoop);
			}
			else
			{
				return this.Play(0);
			}
		}

		/// <summary>
		/// Plays a sound for a desired number of milliseconds or loops.
		/// </summary>
		/// <param name="loops">
		/// The maximum number of loops to play.  -1 for indefinate.
		/// </param>
		/// <param name="milliseconds">
		/// The number of milliseconds to play the sound loop.
		/// </param>
		/// <returns>The channel used to play the sound.</returns>
		public Channel Play(int loops, int milliseconds) 
		{
			int index = 
				SdlMixer.Mix_PlayChannelTimed
				(
				Mixer.FindAvailableChannel(), 
				this.Handle, 
				loops, 
				milliseconds
				);

			if (index == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
			this.channels++;
			return new Channel(index);
		}

		/// <summary>
		/// Fades in a sample once using the first available channel
		/// </summary>
		/// <param name="milliseconds">
		/// The number of milliseconds to fade in for
		/// </param>
		/// <returns>The channel used to play the sample</returns>
		public Channel FadeIn(int milliseconds) 
		{
			int index = SdlMixer.Mix_FadeInChannelTimed(Mixer.FindAvailableChannel(), this.Handle, 0, milliseconds, -1);
			if (index == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
			this.channels++;
			return new Channel(index);
		}

		/// <summary>
		/// Fades in a sample the specified number of times using 
		/// the first available channel
		/// </summary>
		/// <param name="milliseconds">
		/// The number of milliseconds to fade in for
		/// </param>
		/// <param name="loops">The number of loops.  
		/// Specify 1 to have the sample play twice</param>
		/// <returns>The channel used to play the sample</returns>
		public Channel FadeIn(int milliseconds, int loops) 
		{
			int index = SdlMixer.Mix_FadeInChannelTimed(Mixer.FindAvailableChannel(), this.Handle, loops, milliseconds, -1);
			if (index == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
			this.channels++;
			return new Channel(index);
		}

		/// <summary>
		/// Fades in a sample once using the first available channel, 
		/// stopping after the specified number of ms
		/// </summary>
		/// <param name="milliseconds">
		/// The number of milliseconds to fade in for
		/// </param>
		/// <param name="ticks">The time limit in milliseconds</param>
		/// <returns>The channel used to play the sample</returns>
		public Channel FadeInTimed(int milliseconds, int ticks) 
		{
			int index = SdlMixer.Mix_FadeInChannelTimed(Mixer.FindAvailableChannel(), this.Handle, 0, milliseconds, ticks);
			if (index == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
			this.channels++;
			return new Channel(index);
		}

		/// <summary>
		/// Fades in a sample the specified number of times using 
		/// the first available channel, stopping after the 
		/// specified number of ms
		/// </summary>
		/// <param name="milliseconds">
		/// The number of milliseconds to fade in for
		/// </param>
		/// <param name="loops">The number of loops.  
		/// Specify 1 to have the sample play twice</param>
		/// <param name="ticks">The time limit in milliseconds</param>
		/// <returns>The channel used to play the sample</returns>
		public Channel FadeInTimed(int milliseconds, int loops, int ticks) 
		{
			int index = SdlMixer.Mix_FadeInChannelTimed(Mixer.FindAvailableChannel(), this.Handle, loops, milliseconds, ticks);
			if (index == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
			this.channels++;
			return new Channel(index);
		}

		/// <summary>
		/// Gets and sets the number of channels 
		/// used to play this sound sample.
		/// </summary>
		public int NumberOfChannels
		{
			get
			{
				return this.channels;
			}
			set
			{
				this.channels = value;
			}
		}

		/// <summary>
		/// Stops the sound sample.
		/// </summary>
		public void Stop()
		{
			SoundEventArgs args = new SoundEventArgs(SoundAction.Stop);
			OnSoundEvent(args);
		}

		/// <summary>
		/// Fades out the sound sample.
		/// </summary>
		public void Fadeout(int fadeoutTime)
		{
			SoundEventArgs args = new SoundEventArgs(SoundAction.Fadeout, fadeoutTime);
			OnSoundEvent(args);
		}

		/// <summary>
		/// Raises SoundEvent
		/// </summary>
		/// <param name="e">SoundEvent Args</param>
		protected void OnSoundEvent(SoundEventArgs e)
		{
			if (SoundEvent != null)
			{
				SoundEvent(this, e);
			}
		}
	}
}
