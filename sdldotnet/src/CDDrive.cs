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
using System.Runtime.InteropServices;
//using System.Globalization;
using System.Reflection;
using System.Resources;

using Tao.Sdl;

namespace SdlDotNet 
{
	/// <summary>
	/// Represents a CDROM drive on the system
	/// </summary>
	/// <remarks></remarks>
	public class CDDrive : BaseSdlResource
	{
		private bool disposed;
		private int index;
//		ResourceManager stringManager;

		/// <summary>
		/// Represents a CDROM drive on the system
		/// </summary>
		/// <param name="handle">Handle to CDDrive</param>
		/// <param name="index">Index number of drive</param>
		/// <remarks>used internally</remarks>
		internal CDDrive(IntPtr handle, int index) 
		{
//			stringManager = 
//				new ResourceManager("en-US", Assembly.GetExecutingAssembly());
			if ((handle == IntPtr.Zero) || !CDRom.IsValidDriveNumber(index))
			{
				throw SdlException.Generate();
			}
			else
			{
				this.Handle = handle;
				this.index = index;
			}
		}

		/// <summary>
		/// Represents a CDROM drive on the system
		/// </summary>
		/// <param name="index">Index number of drive</param>
		/// <remarks>Initializes drive</remarks>
		/// <exception cref="SdlException">
		/// An exception will be thrown if the drive number 
		/// is below zero or above the number of drive on the system.
		/// </exception>
		public CDDrive(int index) : this(Sdl.SDL_CDOpen(index), index)
		{
		}

		/// <summary>
		/// The drive number
		/// </summary>
		/// <remarks>Returns the drive number.</remarks>
		public int Index 
		{ 
			get 
			{ 
				return this.index; 
			} 
		}
		/// <summary>
		/// Returns a platform-specific name for a CD-ROM drive
		/// </summary>
		/// <remarks></remarks>
		/// <returns>A platform-specific name, i.e. "D:\"</returns>
		public string DriveName() 
		{
			if (!CDRom.IsValidDriveNumber(this.index))
			{
//				throw new SdlException(stringManager.GetString("Device " + this.index + "out of range. Drive name not available.", CultureInfo.CurrentUICulture));
				throw new SdlException("Device " + this.index + "out of range. Drive name not available.");
			}
			return Sdl.SDL_CDName(this.index);
		}

		/// <summary>
		/// Disposes object.
		/// </summary>
		/// <remarks>Destroys unmanaged resources</remarks>
		/// <param name="disposing">
		/// If true, it disposes the handle to the drive
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
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		/// Closes CDDrive handle
		/// </summary>
		/// <remarks>Closes handle to unmanaged SDL resource</remarks>
		protected override void CloseHandle() 
		{
			try
			{
				if (this.Handle != IntPtr.Zero)
				{
					Sdl.SDL_CDClose(this.Handle);
				}
			}
			finally
			{
				this.Handle = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Gets the current drive status
		/// </summary>
		/// <remarks>
		/// used to determine if the drive is busy, stopped, empty.
		/// </remarks>
		public CDStatus Status 
		{
			get 
			{ 
				CDStatus status = (CDStatus) Sdl.SDL_CDStatus(this.Handle);
				return (status); 
			}
		}

		/// <summary>
		/// Checks to see if the CD is currently playing.
		/// </summary>
		/// <remarks>
		/// Returns true if the drive is in use.
		/// </remarks>
		public bool IsBusy 
		{
			get 
			{ 
				CDStatus status = (CDStatus) Sdl.SDL_CDStatus(this.Handle);
				if (status == CDStatus.Playing)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Checks to see if the CD drive is currently empty.
		/// </summary>
		/// <remarks>
		/// Returns true if the drive is empty.
		/// </remarks>
		public bool IsEmpty
		{
			get 
			{ 
				CDStatus status = (CDStatus) Sdl.SDL_CDStatus(this.Handle);
				if (status == CDStatus.TrayEmpty)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Checks to see if the CD drive is currently paused.
		/// </summary>
		/// <remarks>Returns true if the drive has been paused.</remarks>
		public bool IsPaused
		{
			get 
			{ 
				CDStatus status = (CDStatus) Sdl.SDL_CDStatus(this.Handle);
				if (status == CDStatus.Paused)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Checks to see if the track has audio data.
		/// </summary>
		/// <remarks></remarks>
		/// <param name="trackNumber">
		/// Returns true if the track is an audio track
		/// </param>
		public bool IsAudioTrack(int trackNumber)
		{
				int result = Sdl.CD_INDRIVE((int)this.Status);
				GC.KeepAlive(this);
				if (result == 1)
				{
					Sdl.SDL_CD cd = 
						(Sdl.SDL_CD)Marshal.PtrToStructure(
						this.Handle, typeof(Sdl.SDL_CD));

					if (cd.track[trackNumber].type == (byte) CDTrackType.Audio)
					{
						return true;
					}
					else 
					{
						return false;
					}
				}
				else 
				{ 
					return false;
				}
		}

		/// <summary>
		/// Checks to see if the track is a data track.
		/// </summary>
		/// <remarks></remarks>
		/// <param name="trackNumber">
		/// Returns true if the track is a data track
		/// </param>
		public bool IsDataTrack(int trackNumber)
		{
				int result = Sdl.CD_INDRIVE((int)this.Status);
				GC.KeepAlive(this);
				if (result == 1)
				{
					Sdl.SDL_CD cd = 
						(Sdl.SDL_CD)Marshal.PtrToStructure(
						this.Handle, typeof(Sdl.SDL_CD));

					if (cd.track[trackNumber].type == (byte) CDTrackType.Data)
					{
						return true;
					}
					else 
					{
						return false;
					}
				}
				else 
				{ 
					return false;
				}
		}

		/// <summary>
		/// Returns the length of an audio track in seconds.
		/// </summary>
		/// <remarks></remarks>
		/// <param name="trackNumber">Track to query</param>
		public int TrackLength(int trackNumber)
		{
				int result = Sdl.CD_INDRIVE((int)this.Status);
				GC.KeepAlive(this);
				if (result == 1)
				{
					Sdl.SDL_CD cd = 
						(Sdl.SDL_CD)Marshal.PtrToStructure(
						this.Handle, typeof(Sdl.SDL_CD));
					return (int)Timer.FramesToSeconds(cd.track[trackNumber].length);
				}
				else 
				{ 
					return 0;
				}
		}

		/// <summary>
		/// Returns the number of seconds before the audio track starts on the cd.
		/// </summary>
		/// <remarks></remarks>
		/// <param name="trackNumber">Track to query</param>
		public int TrackStart(int trackNumber)
		{
				int result = Sdl.CD_INDRIVE((int)this.Status);
				GC.KeepAlive(this);
				if (result == 1)
				{
					Sdl.SDL_CD cd = 
						(Sdl.SDL_CD)Marshal.PtrToStructure(
						this.Handle, typeof(Sdl.SDL_CD));
					return (int)Timer.FramesToSeconds(cd.track[trackNumber].offset);
				}
				else 
				{ 
					return 0;
				}
		}

		/// <summary>
		/// Returns the end time of the track in seconds.
		/// </summary>
		/// <remarks></remarks>
		/// <param name="trackNumber">Track to query</param>
		public int TrackEnd(int trackNumber)
		{
			int result = Sdl.CD_INDRIVE((int)this.Status);
			GC.KeepAlive(this);
			if (result == 1)
			{
				return this.TrackStart(trackNumber) + this.TrackLength(trackNumber);
			}
			else 
			{ 
				return 0;
			}
		}

		/// <summary>
		/// Plays the tracks on a CD in the drive
		/// </summary>
		/// <remarks></remarks>
		/// <param name="startTrack">
		/// The starting track to play (numbered 0-99)
		/// </param>
		/// <param name="numberOfTracks">
		/// The number of tracks to play
		/// </param>
		public void PlayTracks(int startTrack, int numberOfTracks) 
		{
			int result = Sdl.SDL_CDPlayTracks(
				this.Handle, startTrack, 0, numberOfTracks, 0);
			GC.KeepAlive(this);
			if (result == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Plays the tracks on a CD in the drive
		/// </summary>
		/// <param name="startTrack">
		/// The starting track to play 
		/// (numbered 0-99)
		/// </param>
		/// <param name="startFrame">
		/// The frame (75th of a second increment) offset from the 
		/// starting track to play from
		/// </param>
		/// <param name="numberOfTracks">
		/// The number of tracks to play
		/// </param>
		/// <param name="numberOfFrames">
		/// The frame (75th of a second increment) offset after the last 
		/// track to stop playing after
		/// </param>
		/// <remarks></remarks>
		public void PlayTracks(
			int startTrack, int startFrame, 
			int numberOfTracks, int numberOfFrames) 
		{
			int result = Sdl.SDL_CDPlayTracks(
				this.Handle, startTrack, startFrame, 
				numberOfTracks, numberOfFrames);
			GC.KeepAlive(this);
			if (result == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Play CD from a given track
		/// </summary>
		/// <remarks></remarks>
		/// <param name="startTrack">Track to start from</param>
		public void PlayTracks(int startTrack)
		{
			int result = Sdl.SDL_CDPlayTracks(
				this.Handle, startTrack, 0, 0, 0);
			GC.KeepAlive(this);
			if (result == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Play CD from the first track.
		/// </summary>
		/// <remarks></remarks>
		public void Play()
		{
			int result = Sdl.SDL_CDPlayTracks(
				this.Handle, 0, 0, 0, 0);
			GC.KeepAlive(this);
			if (result == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Pauses the CD in this drive
		/// </summary>
		/// <remarks></remarks>
		public void Pause() 
		{
			int result = Sdl.SDL_CDPause(this.Handle);
			GC.KeepAlive(this);
			if (result == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
		}
		/// <summary>
		/// Resumes a previously paused CD in this drive
		/// </summary>
		/// <remarks></remarks>
		public void Resume() 
		{
			int result = Sdl.SDL_CDResume(this.Handle);
			GC.KeepAlive(this);
			if (result == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
		}
		/// <summary>
		/// Stops playing the CD in this drive
		/// </summary>
		/// <remarks></remarks>
		public void Stop() 
		{
			int result = Sdl.SDL_CDStop(this.Handle);
			GC.KeepAlive(this);
			if ( result == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}

		}

		/// <summary>
		/// Ejects this drive
		/// </summary>
		/// <remarks></remarks>
		public void Eject() 
		{
			int result = Sdl.SDL_CDEject(this.Handle);
			GC.KeepAlive(this);
			if ( result == (int) SdlFlag.Error)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Gets the number of tracks in the currently inserted CD
		/// </summary>
		/// <remarks></remarks>
		public int NumberOfTracks 
		{
			get 
			{
				int result = Sdl.CD_INDRIVE((int)this.Status);
				GC.KeepAlive(this);
				if (result == 1)
				{
					Sdl.SDL_CD cd = 
						(Sdl.SDL_CD)Marshal.PtrToStructure(
						this.Handle, typeof(Sdl.SDL_CD));
					return cd.numtracks;
				}
				else 
				{ 
					return 0;
				}
			}
		}
		/// <summary>
		/// Gets the currently playing track number
		/// </summary>
		/// <remarks></remarks>
		public int CurrentTrack 
		{
			get 
			{
				Sdl.SDL_CDStatus(this.Handle);
				GC.KeepAlive(this);
				Sdl.SDL_CD cd = 
					(Sdl.SDL_CD)Marshal.PtrToStructure(
					this.Handle, typeof(Sdl.SDL_CD));
				
				return cd.cur_track;
			}
		}
		/// <summary>
		/// Gets the currently playing frame of the current track.
		/// </summary>
		/// <remarks></remarks>
		public int CurrentTrackFrame 
		{
			get 
			{
				Sdl.SDL_CDStatus(this.Handle);
				GC.KeepAlive(this);
				Sdl.SDL_CD cd = 
					(Sdl.SDL_CD)Marshal.PtrToStructure(
					this.Handle, typeof(Sdl.SDL_CD));
				return cd.cur_frame;
			}
		}

		/// <summary>
		/// Gets the number of seconds into the current track.
		/// </summary>
		/// <remarks></remarks>
		public double CurrentTrackSeconds
		{
			get 
			{
				return Timer.FramesToSeconds(this.CurrentTrackFrame);
			}
		}
	}
}
