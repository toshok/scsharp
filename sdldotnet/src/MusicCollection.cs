/*
 * $RCSfile$
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
using System.Collections;
using System.IO;

namespace SdlDotNet
{
	/// <summary>
	/// Encapsulates the collection of Music objects.
	/// </summary>
	public class MusicCollection : CollectionBase, ICollection
	{
		/// <summary>
		/// Creates an empty MusicCollection
		/// </summary>
		public MusicCollection()
		{
		}

		/// <summary>
		/// Creates a new MusicCollection with the given music element in it.
		/// </summary>
		/// <param name="music">The music object to be held as the first element in the collection.</param>
		public MusicCollection(Music music)
		{
			Add(music);
		}

		/// <summary>
		/// Creates a copy of the given MusicCollection.
		/// </summary>
		/// <param name="music">The music collection to make a copy of.</param>
		public MusicCollection(MusicCollection music)
		{
			Add(music);
		}

		/// <summary>
		/// Creates a new MusicCollection with the given music file in it.
		/// </summary>
		/// <param name="fileName">The music file to load as the first element.</param>
		public MusicCollection(string fileName)
		{
			Add(fileName);
		}

		/// <summary>
		/// Indexer for the collection
		/// </summary>
		public virtual Music this[int index]
		{
			get
			{
				if (this.Count == 0)
				{
					return ((Music)List[index]);
				}
				else
				{
					return ((Music)List[index % this.Count]);	
				}
			}
			set
			{
				if (this.Count == 0)
				{
					List[index] = value;
				}
				else
				{
					List[index % this.Count] = value;
				}
			}
		}

		/// <summary>
		/// Adds a music object to the collection.
		/// </summary>
		/// <param name="music">The music object to add.</param>
		/// <returns>The index of the music object in the collection.</returns>
		public int Add(Music music)
		{
			return List.Add(music);
		}

		/// <summary>
		/// Adds a music object to the collection from its file path.
		/// </summary>
		/// <param name="fileName">The file to add in as music.</param>
		/// <returns>The index of the music object in the collection.</returns>
		public int Add(string fileName)
		{
			return Add(new Music(fileName));
		}

		/// <summary>
		/// Adds a music collection to the current collection.
		/// </summary>
		/// <param name="musicCollection">The collection of music to add.</param>
		/// <returns>The new amount of music entrees in the collection.</returns>
		public int Add(MusicCollection musicCollection)
		{
			if (musicCollection == null)
			{
				throw new ArgumentNullException("musicCollection");
			}
			for (int i = 0; i < musicCollection.Count; i++)
			{
				this.List.Add(musicCollection[i]);
			}
			return this.Count;
		}

		/// <summary>
		/// Adds the specified Music to the MusicCollection.
		/// </summary>
		/// <param name="index">Index at which to insert to new music</param>
		/// <param name="fileName">Filename to the music to insert</param>
		public void Insert(int index, string fileName)
		{
			List.Insert(index, new Music(fileName));
		}

		/// <summary>
		/// Adds the specified Music to the MusicCollection.
		/// </summary>
		/// <param name="index">Index at which to insert to new music</param>
		/// <param name="music">Music to insert</param>
		public void Insert(int index, Music music)
		{
			List.Insert(index, music);
		}

		/// <summary>
		/// Adds a collection of music into the specified location in the current collection.
		/// </summary>
		/// <param name="index">The location of which to add the collection.</param>
		/// <param name="musicCollection">The music collection to add to the current collection.</param>
		public void Insert(int index, MusicCollection musicCollection)
		{
			if( musicCollection == null )
			{
				throw new ArgumentNullException("musicCollection");
			}
			for(int i = 0; i < musicCollection.Count; i++)
			{
				List.Insert(index + i, musicCollection[i]);
			}
		}

		/// <summary>
		/// Removes a specified Music from the collection.
		/// </summary>
		/// <param name="music">
		/// The Music to remove from the MusicCollection.
		/// </param>
		public void Remove(Music music)
		{
			List.Remove(music);
		} 

		/// <summary>
		/// Returns the index of a specified Music in the collection.
		/// </summary>
		/// <param name="music">The music object</param>
		/// <returns>The index of specified music in the collection</returns>
		public int IndexOf(Music music)
		{
			return (List.IndexOf(music));
		} 
		
		/// <summary>
		/// Indicates whether a specified Music is contained in the collection.
		/// </summary>
		/// <param name="music">
		/// The Music to find in the collection.
		/// </param>
		/// <returns>
		/// true if the Music is found in the collection; otherwise, false.
		/// </returns>
		public bool Contains(Music music)
		{
			return (List.Contains(music));
		}
		
		/// <summary>
		/// Copy music collection to array
		/// </summary>
		/// <param name="array">Array to copy collection to</param>
		/// <param name="index">Start at this index</param>
		public virtual void CopyTo(Music[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		/// <summary>
		/// Copy array
		/// </summary>
		/// <param name="array">Array to copy collection to</param>
		/// <param name="index">Start index</param>
		public virtual void CopyTo(Array array, int index)
		{
			this.List.CopyTo(array,index);
		}

		/// <summary>
		/// Makes each music item in the collection queue the next item in the collection.
		/// </summary>
		/// <remarks>You must call <see cref="Music.EnableMusicFinishedCallback"/>() to enable queueing.</remarks>
		public void CreateQueueList()
		{
			for(int i = 0; i < List.Count; i++)
			{
				this[i].QueuedMusic = this[i + 1];
				// Note that the last element refers to the first element because of the this indexer
			}
		}

		/// <summary>
		/// Makes a music collection from the files held within the given directory.
		/// </summary>
		/// <remarks>Doesn't throw an exception when attempting to load from an invalid file format within the directory.</remarks>
		/// <param name="dir">The path to the directory to load.</param>
		/// <returns>The new MusicCollection that was created.</returns>
		public static MusicCollection FromDirectory(string dir)
		{
			DirectoryInfo directory = new DirectoryInfo(dir);
			FileInfo[] files = directory.GetFiles();
			MusicCollection collection = new MusicCollection();
			foreach(FileInfo file in files)
			{
				try
				{
					collection.Add(file.FullName);
				}
				catch (System.IO.FileNotFoundException e)
				{
					e.ToString();
					// Do nothing and move onto next file
				}
			}
			return collection;
		}
	}
}
