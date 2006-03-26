/*
 * $RCSfile$
 * Copyright (C) 2005 David Hudson (jendave@yahoo.com)
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
	/// Represents a sound sample.
	/// Create with Mixer.LoadWav().
	/// </summary>
	/// <remarks></remarks>
	public class Channel : IDisposable
	{
		private int index;
		private Sound sound;
		private Sound queuedSound;
		private Sound lastSound;
		private SdlMixer.ChannelFinishedDelegate channelFinishedDelegate;

		/// <summary>
		/// Creates a channel with the given index
		/// </summary>
		/// <param name="index">Index numberof channel</param>
		/// <remarks></remarks>
		public Channel(int index)
		{
			this.index = index;
		}

		/// <summary>
		/// Returns index of channel
		/// </summary>
		/// <remarks></remarks>
		public int Index
		{
			get
			{
				return this.index;
			}
		}


		/// <summary>
		/// Plays a sound the specified number of times on a specific channel
		/// </summary>
		/// <param name="sound">The sound to play</param>
		/// <returns>The channel used to play the sound</returns>
		/// <remarks></remarks>
		public int Play(Sound sound) 
		{
			return this.Play(sound, 0);
		}
		/// <summary>
		/// Plays a sound the specified number of times on a specific channel
		/// </summary>
		/// <param name="sound">The sound to play</param>
		/// <param name="loops">
		/// The number of loops.  Specify 1 to have the sound play twice
		/// </param>
		/// <returns>The channel used to play the sound</returns>
		/// <remarks></remarks>
		public int Play(Sound sound, int loops) 
		{
			return this.Play(sound, loops, (int) SdlFlag.InfiniteLoop);
		}

		/// <summary>
		/// Plays a sound the specified number of times on a 
		/// specific channel, stopping after the specified number of ms
		/// </summary>
		/// <param name="sound">The sound to play</param>
		/// <param name="loops">
		/// The number of loops.  Specify 1 to have the sound play twice
		/// </param>
		/// <param name="milliseconds">The time limit in milliseconds</param>
		/// <returns>The channel used to play the sound</returns>
		/// <remarks></remarks>
		public int Play(Sound sound, int loops, int milliseconds) 
		{
			if (sound == null)
			{
				throw new ArgumentNullException("sound");
			}
			int ret = SdlMixer.Mix_PlayChannelTimed(this.index, sound.Handle, loops, milliseconds);
			if (ret == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
			this.Sound = sound;
			return ret;
		}

		/// <summary>
		/// Plays a sound the specified number of times on a specific channel
		/// </summary>
		/// <param name="sound">The sound to play</param>
		/// <param name="continuous">If true, sound will be looped.</param>
		/// <returns>The channel used to play the sound</returns>
		/// <remarks></remarks>
		public int Play(Sound sound, bool continuous) 
		{
			if (continuous == true)
			{
				return this.Play(sound, -1, (int) SdlFlag.InfiniteLoop);
			}
			else
			{
				return this.Play(sound);
			}
		}

		/// <summary>
		/// Plays a sound the specified number of times on a 
		/// specific channel, stopping after the specified number of ms
		/// </summary>
		/// <param name="sound">The sound to play</param>
		/// <param name="milliseconds">The time limit in milliseconds</param>
		/// <returns>The channel used to play the sound</returns>
		/// <remarks></remarks>
		public int PlayTimed(Sound sound, int milliseconds) 
		{
			if (sound == null)
			{
				throw new ArgumentNullException("sound");
			}
			int ret = SdlMixer.Mix_PlayChannelTimed(this.index, sound.Handle, -1, milliseconds);
			if (ret == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
			this.Sound = sound;
			return ret;
		}

		/// <summary>
		/// Sets the volume for a channel
		/// </summary>
		/// <returns>Channel volume</returns>
		/// <remarks></remarks>
		public int Volume
		{
			get
			{
				return SdlMixer.Mix_Volume(this.index, -1);
			}
			set
			{
				SdlMixer.Mix_Volume(this.index, value);
			}
		}

		/// <summary>
		/// Returns the Sound object that is attached to this channel
		/// </summary>
		/// <remarks></remarks>
		public Sound Sound
		{
			get
			{
				return this.sound;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (this.sound != null)
				{
					this.lastSound = this.sound;
					this.lastSound.NumberOfChannels--;
				}
				this.sound = value;
				this.sound.SoundEvent += new SoundEventHandler(ProcessSoundEvent);
				this.sound.NumberOfChannels++;
			}
		}

		/// <summary>
		/// Returns sound previously played on this channel
		/// </summary>
		/// <remarks></remarks>
		public Sound LastSound
		{
			get
			{
				return this.lastSound;
			}
		}

		/// <summary>
		/// Returns queued sound on this channel
		/// </summary>
		/// <remarks></remarks>
		public Sound QueuedSound
		{
			get
			{
				return this.queuedSound;
			}
			set
			{
				this.queuedSound = value;
			}
		}

		/// <summary>
		/// Fades in a sound the specified number of times on a 
		/// specific channel
		/// </summary>
		/// <param name="sound">The sound to play</param>
		/// <param name="ms">
		/// The number of milliseconds to fade in for
		/// </param>
		/// <param name="loops">
		/// The number of loops.  
		/// Specify 1 to have the sound play twice
		/// </param>
		/// <returns>The channel used to play the sound</returns>
		/// <remarks></remarks>
		public int FadeIn(Sound sound, int ms, int loops) 
		{
			if (sound == null)
			{
				throw new ArgumentNullException("sound");
			}
			int ret = SdlMixer.Mix_FadeInChannelTimed(this.index, sound.Handle, loops, ms, -1);
			if (ret == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
			return ret;
		}

		/// <summary>
		/// Fades in a sound the specified number of times on 
		/// a specific channel, stopping after the specified number of ms
		/// </summary>
		/// <param name="sound">The sound to play</param>
		/// <param name="ms">The number of milliseconds to fade in for
		/// </param>
		/// <param name="loops">The number of loops.  
		/// Specify 1 to have the sound play twice</param>
		/// <param name="ticks">The time limit in milliseconds</param>
		/// <returns>The channel used to play the sound</returns>
		/// <remarks></remarks>
		public int FadeIn(Sound sound, int ms, int loops, int ticks) 
		{
			if (sound == null)
			{
				throw new ArgumentNullException("sound");
			}
			int ret = SdlMixer.Mix_FadeInChannelTimed(this.index, sound.Handle, loops, ms, ticks);
			if (ret == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
			return ret;
		}

		/// <summary>
		/// Pauses playing on a specific channel
		/// </summary>
		/// <remarks></remarks>
		public void Pause() 
		{
			SdlMixer.Mix_Pause(this.index);
		}

		/// <summary>
		/// Resumes playing on a paused channel
		/// </summary>
		/// <remarks></remarks>
		public void Resume() 
		{
			SdlMixer.Mix_Resume(this.index);
		}

		/// <summary>
		/// Stop playing on a specific channel
		/// </summary>
		/// <remarks></remarks>
		public void Stop() 
		{
			SdlMixer.Mix_HaltChannel(this.index);
			this.sound = null;
		}
		/// <summary>
		/// Stop playing a channel after a specified time interval
		/// </summary>
		/// <param name="ms">
		/// The number of milliseconds to stop playing after
		/// </param>
		/// <remarks></remarks>
		public void Expire(int ms) 
		{
			SdlMixer.Mix_ExpireChannel(this.index, ms);
		}
		/// <summary>
		/// Fades out a channel.
		/// </summary>
		/// <param name="ms">
		/// The number of milliseconds to fade out for
		/// </param>
		/// <returns>The number of channels fading out</returns>
		/// <remarks></remarks>
		public int Fadeout(int ms) 
		{
			return SdlMixer.Mix_FadeOutChannel(this.index, ms);
		}

		/// <summary>
		/// Returns a flag indicating whether or not a channel is playing
		/// </summary>
		/// <returns>True if the channel is playing, otherwise False</returns>
		/// <remarks></remarks>
		public bool IsPlaying() 
		{
			return (SdlMixer.Mix_Playing(this.index) != 0);
		}
		/// <summary>
		/// Returns a flag indicating whether or not a channel is paused
		/// </summary>
		/// <returns>True if the channel is paused, otherwise False</returns>
		/// <remarks></remarks>
		public bool IsPaused() 
		{
			return (SdlMixer.Mix_Paused(this.index) != 0);
		}
		/// <summary>
		/// Returns the current fading status of a channel
		/// </summary>
		/// <returns>The current fading status of the channel</returns>
		/// <remarks></remarks>
		public  FadingStatus FadingStatus() 
		{
			return (FadingStatus)SdlMixer.Mix_FadingChannel(this.index);
		}
		/// <summary>
		/// Sets the panning (stereo attenuation) for a specific channel
		/// </summary>
		/// <param name="left">A left speaker value from 0-255 inclusive</param>
		/// <param name="right">A right speaker value from 0-255 inclusive</param>
		/// <remarks></remarks>
		public  void SetPanning(int left, int right) 
		{
			if (SdlMixer.Mix_SetPanning(this.index, (byte)left, (byte)right) == 0)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Sets the distance (attenuate sounds based on distance 
		/// from listener) for a specific channel
		/// </summary>
		/// <param name="distanceValue">
		/// Distance value from 0-255 inclusive
		/// </param>
		/// <remarks></remarks>
		public void Distance(byte distanceValue)
		{
			if (SdlMixer.Mix_SetDistance(this.index, distanceValue) == 0)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Sets the "position" of a sound (approximate '3D' audio)
		///  for a specific channel
		/// </summary>
		/// <param name="angle">The angle of the sound, between 0 and 359,
		///  0 = directly in front</param>
		/// <param name="distance">The distance of the sound from 0-255
		///  inclusive</param>
		///  <remarks></remarks>
		public void SetPosition(int angle, int distance) 
		{
			if (SdlMixer.Mix_SetPosition(this.index, (short)angle, (byte)distance) == 0)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Flips the left and right stereo for the channel
		/// </summary>
		/// <param name="flip">True to flip, False to reset to normal</param>
		/// <remarks></remarks>
		public void ReverseStereo(bool flip) 
		{
			if (SdlMixer.Mix_SetReverseStereo(this.index, flip?1:0) == 0)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Enables the callback for this channel
		/// </summary>
		/// <remarks>
		/// When the sound stops playing, the delegate will be called.
		/// </remarks>
		public void EnableChannelFinishedCallback() 
		{
			channelFinishedDelegate = new SdlMixer.ChannelFinishedDelegate(ChannelFinished);
			SdlMixer.Mix_ChannelFinished(channelFinishedDelegate);
			Events.ChannelFinished +=new ChannelFinishedEventHandler(Events_ChannelFinished);
		}
		private void ChannelFinished(int channel) 
		{
			Events.NotifyChannelFinished(this.index);
		}

		private void Events_ChannelFinished(object sender, ChannelFinishedEventArgs e)
		{
			if (this.queuedSound != null)
			{
				this.sound = this.queuedSound;
				this.queuedSound = null;
				this.Play(this.sound);
			}
		}

		private void ProcessSoundEvent(object sender, SoundEventArgs e)
		{
			if (e.Action == SoundAction.Stop)
			{
				this.Stop();
			}
			else if (e.Action == SoundAction.Fadeout)
			{
				this.Fadeout(e.FadeoutTime);
			}
			else
			{
				throw new SdlException();
			}
		}

		#region IDisposable Members

		private bool disposed;

		/// <summary>
		/// Destroy sprite
		/// </summary>
		/// <param name="disposing">If true, remove all unamanged resources</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.sound != null)
					{
						this.sound.Dispose();
						this.sound = null;
					}
					if (this.queuedSound != null)
					{
						this.queuedSound.Dispose();
						this.queuedSound = null;
					}
					if (this.lastSound != null)
					{
						this.lastSound.Dispose();
						this.lastSound = null;
					}
				}
				this.disposed = true;
			}
		}
		/// <summary>
		/// Destroy object
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Destroy object
		/// </summary>
		public void Close() 
		{
			Dispose();
		}

		/// <summary>
		/// Destroy object
		/// </summary>
		~Channel() 
		{
			Dispose(false);
		}

		#endregion
	}
}
