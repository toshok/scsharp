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
	/// Represents a collection of music samples 
	/// held together by a dictionary key-value base.
	/// </summary>
	/// <example>
	/// <code>
	/// MusicCollection tunes = new MusicCollection();
	/// 
	/// tunes.Add("techno", "techno.mid");
	/// tunes.Add("jazz.mid");
	/// 
	/// tunes["techo"].Play();
	/// tunes["jazz.mid"].Play();
	/// </code>
	/// </example>
	public class MusicDictionary : DictionaryBase, IDictionary
	{
		/// <summary>
		/// Creates a new empty MusicCollection.
		/// </summary>
		public MusicDictionary() : base()
		{
		}

		/// <summary>
		/// Creates a new MusicCollection with one element in it.
		/// </summary>
		/// <param name="key">
		/// The key you would like to refer to the music sample as.
		/// </param>
		/// <param name="music">
		/// The sample object itself.
		/// </param>
		public MusicDictionary(string key, Music music)
		{
			this.Add(key, music);
		}

		/// <summary>
		/// Creates a new MusicCollection with one element in it.
		/// </summary>
		/// <param name="fileName">
		/// The filename and key of the single Music object to load.
		/// </param>
		public MusicDictionary(string fileName)
		{    
			this.Add(fileName);
		}

		/// <summary>
		/// Creates a new MusicCollection with one element in it.
		/// </summary>
		/// <param name="music">
		/// The single music sample to start off the collection.
		/// </param>
		public MusicDictionary(Music music)
		{
			this.Add(music);
		}

		/// <summary>
		/// Loads multiple files from a directory into the collection.
		/// </summary>
		/// <param name="baseName">
		/// The name held before the file index.
		/// </param>
		/// <param name="extension">
		/// The extension of the files (.mp3)
		/// </param>
		public MusicDictionary(string baseName, string extension)
		{
			// Save the fields
			//this.filename = baseName + "-*" + extension;
			int i = 0;
			while (true)
			{
				string fn = null;
				if (i < 10)
				{
					fn = baseName + "-0" + i + extension;
				}
				else
				{
					fn = baseName + "-" + i + extension;
				}
                
				if (!File.Exists(fn))
				{
					break;
				}
                
				// Load it
				this.Dictionary.Add(fn, new Music(fn));
				i++;
			}
		}

		/// <summary>
		/// Creates a new MusicCollection with the contents 
		/// of an existing MusicCollection.
		/// </summary>
		/// <param name="musicDictionary">
		/// The existing music collection to add.
		/// </param>
		public MusicDictionary(MusicDictionary musicDictionary)
		{
			if (musicDictionary == null)
			{
				throw new ArgumentNullException("musicDictionary");
			}
			IDictionaryEnumerator enumer = musicDictionary.GetEnumerator();
			while(enumer.MoveNext())
			{
				this.Add((string)enumer.Key, (Music)enumer.Value);
			}
		}

		/// <summary>
		/// Gets and sets a music object within the collection.
		/// </summary>
		public Music this[string key]
		{
			get 
			{
				return((Music)Dictionary[key]);
			}
			set
			{
				Dictionary[key] = value;
			}
		}
		
		/// <summary>
		/// Gets all the Keys in the Collection.
		/// </summary>
		public ICollection Keys  
		{
			get  
			{
				return Dictionary.Keys;
			}
		}
        
		/// <summary>
		/// Gets all the Values in the Collection.
		/// </summary>
		public ICollection Values  
		{
			get  
			{
				return Dictionary.Values;
			}
		}

		/// <summary>
		/// Adds a music sample to the collection.
		/// </summary>
		/// <param name="key">
		/// The key to use as reference to the music object.
		/// </param>
		/// <param name="music">
		/// The sample to add.
		/// </param>
		/// <returns>
		/// The total number of elements within the 
		/// collection after adding the sample.
		/// </returns>
		public int Add(string key, Music music) 
		{
			Dictionary.Add(key, music);
			return Dictionary.Count;
		}

		/// <summary>
		/// Adds a music sample to the collection 
		/// using the filename as the key.
		/// </summary>
		/// <param name="fileName">
		/// The music filename to load as well 
		/// as the key to use as the reference.
		/// </param>
		/// <returns>The total number of elements 
		/// within the collection after adding the sample.
		/// </returns>
		public int Add(string fileName)
		{
			Dictionary.Add(fileName, new Music(fileName));
			return Dictionary.Count;
		}

		/// <summary>
		/// Adds a music sample to the collection using 
		/// the Music's ToString method as the key.
		/// </summary>
		/// <param name="music">
		/// The music object to add.
		/// </param>
		/// <returns>
		/// The total number of elements within the collection 
		/// after adding the sample.
		/// </returns>
		public int Add(Music music)
		{
			if (music == null)
			{
				throw new ArgumentNullException("music");
			}
			Dictionary.Add(music.ToString(), music);
			return Dictionary.Count;
		}
        
		/// <summary>
		/// Returns true if the collection contains the given key.
		/// </summary>
		/// <param name="key">key for item in collection</param>
		/// <returns>Returns true if collection contains the key.</returns>
		public bool Contains(string key)
		{
			return Dictionary.Contains(key);
		}
		
		/// <summary>
		/// Adds a collection of music to the current music collection.
		/// </summary>
		/// <param name="musicDictionary">
		/// The collection of music samples to add.
		/// </param>
		/// <returns>
		/// The total number of elements within the collection after 
		/// adding the sample.
		/// </returns>
		public int Add(MusicDictionary musicDictionary)
		{
			if (musicDictionary == null)
			{
				throw new ArgumentNullException("musicDictionary");
			}
			IDictionaryEnumerator dict = musicDictionary.GetEnumerator();
			while(dict.MoveNext())
			{
				this.Add((string)dict.Key, (Music)dict.Value);
			}
			return Dictionary.Count;
		}

		/// <summary>
		/// Adds a music sample to the collection.
		/// </summary>
		/// <param name="key">
		/// The reference value for the music sample.
		/// </param>
		/// <param name="fileName">
		/// The filename of the music sample to load.
		/// </param>
		/// <returns>
		/// The total number of elements within the collection 
		/// after adding the sample.
		/// </returns>
		public int Add(string key, string fileName)
		{
			Dictionary.Add(key, new Music(fileName));
			return Dictionary.Count;
		}
        
		/// <summary>
		/// Removes an element from the collection.
		/// </summary>
		/// <param name="key">The element's key to remove.</param>
		public void Remove(string key)
		{
			Dictionary.Remove(key);
		}

        /// <summary>
        /// Makes all items in the collection 
        /// queued to each other for a playlist effect.
        /// </summary>
        public void CreateQueueList()
        {
            IDictionaryEnumerator enumer = Dictionary.GetEnumerator();
            Music currItem = null;
            Music prevItem = null;
            while(enumer.MoveNext())
            {
                currItem = (Music)enumer.Value;
				if (prevItem != null)
				{
					prevItem.QueuedMusic = currItem;
				}
                prevItem = currItem;
            }
        }

		/// <summary>
		/// Provide the explicit interface member for ICollection.
		/// </summary>
		/// <param name="array">Array to copy collection to</param>
		/// <param name="index">Index at which to insert the collection items</param>
		void ICollection.CopyTo(Array array, int index)
		{
			this.CopyTo(array, index);
		}

		/// <summary>
		/// Provide the explicit interface member for ICollection.
		/// </summary>
		/// <param name="array">Array to copy collection to</param>
		/// <param name="index">Index at which to insert the collection items</param>
		public virtual void CopyTo(Music[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		/// <summary>
		/// Insert a item into the collection
		/// </summary>
		/// <param name="index">Index at which to insert the item</param>
		/// <param name="music">item to insert</param>
		public virtual void Insert(int index, Music music)
		{
			this.Insert(index, music);
		} 

		/// <summary>
		/// Gets the index of the given item in the collection.
		/// </summary>
		/// <param name="music">The item to search for.</param>
		/// <returns>The index of the given sprite.</returns>
		public virtual int IndexOf(Music music)
		{
			return this.IndexOf(music);
		} 
	}
}
