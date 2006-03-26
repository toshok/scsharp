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
    /// Encapulates a Dictionary of Sound objects in a Sound Dictionary.
    /// </summary>
    /// <remarks>
    /// Every sound object within the Dictionary is indexed by a string key.
    /// </remarks>
    /// <example>
    /// <code>
    /// SoundDictionary sounds = new SoundDictionary();
    /// sounds.Add("boom", Mixer.Sound("explosion.wav"));
    /// sounds.Add("boing.wav");
    /// sounds.Add("baseName", ".ogg");
    /// 
    /// sounds["boing.wav"].Play();
    /// sounds["boom"].Play();
    /// sounds["baseName-01.ogg"].Play(); 
    /// </code>
    /// </example>
    /// <seealso cref="Sound"/>
    public class SoundDictionary : DictionaryBase, IDictionary
    {
        /// <summary>
        /// Creates a new SoundDictionary object.
        /// </summary>
        public SoundDictionary() : base()
        {
        }
        
        /// <summary>
        /// Creates a SoundDictionary with one loaded item.
        /// </summary>
        /// <param name="key">The key of the sound item.</param>
        /// <param name="sound">The sound object.</param>
        public SoundDictionary(string key, Sound sound)
        {
            this.Add(key, sound);
        }
        
        /// <summary>
        /// Creates a SoundDictionary with one loaded item.
        /// </summary>
        /// <param name="fileName">
        /// The sound item's filename to load and set as the key.
        /// </param>
        public SoundDictionary(string fileName)
        {    
            this.Add(fileName);
        }
        
        /// <summary>
        /// Loads a number of files.
        /// </summary>
        /// <param name="baseName">
        /// The string contained before the file index number.
        /// </param>
        /// <param name="extension">The extension of each file.</param>
        public SoundDictionary(string baseName, string extension)
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
                this.Dictionary.Add(fn, Mixer.Sound(fn));
                i++;
            }
        }
        
        /// <summary>
        /// Adds the contents of an existing SoundDictionary to a new one.
        /// </summary>
        /// <param name="soundDictionary">
        /// The SoundDictionary to copy.
        /// </param>
        public SoundDictionary(SoundDictionary soundDictionary)
        {
			if (soundDictionary == null)
			{
				throw new ArgumentNullException("soundDictionary");
			}
			IDictionaryEnumerator enumer = soundDictionary.GetEnumerator();
			while(enumer.MoveNext())
			{
				this.Add((string)enumer.Key, (Sound)enumer.Value);
			}
        }
        
        /// <summary>
        /// Gets and sets a Sound value based off of its key.
        /// </summary>
        public Sound this[string key]
        {
            get 
            {
                return((Sound)Dictionary[key]);
            }
            set
            {
                Dictionary[key] = value;
            }
        }
        
        /// <summary>
        /// Gets all the Keys in the Dictionary.
        /// </summary>
        public IDictionary Keys  
        {
            get  
            {
                return this.Keys;
            }
        }
        
        /// <summary>
        /// Gets all the Values in the Dictionary.
        /// </summary>
        public IDictionary Values  
        {
            get  
            {
                return this.Values;
            }
        }
        
        /// <summary>
        /// Adds a Sound object to the Dictionary.
        /// </summary>
        /// <param name="key">
        /// The key to make reference to the object.
        /// </param>
        /// <param name="sound">The sound object to add.</param>
        /// <returns>
        /// The final number of elements within the Dictionary.
        /// </returns>
        public int Add(string key, Sound sound) 
        {
            Dictionary.Add(key, sound);
            return Dictionary.Count;
        }
        
        /// <summary>
        /// Adds a newly loaded file to the Dictionary.
        /// </summary>
        /// <param name="fileName">The filename to load.</param>
        /// <returns>
        /// The final number of elements within the Dictionary.
        /// </returns>
        public int Add(string fileName)
        {
            Dictionary.Add(fileName, Mixer.Sound(fileName));
            return Dictionary.Count;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return Dictionary.Contains(key);
        }
        
        /// <summary>
        /// Adds an existing SoundDictionary to the Dictionary.
        /// </summary>
        /// <param name="soundDictionary">
        /// The SoundDictionary to add.
        /// </param>
        /// <returns>
        /// The final number of objects within the Dictionary.
        /// </returns>
        public int Add(SoundDictionary soundDictionary)
        {
			if (soundDictionary == null)
			{
				throw new ArgumentNullException("soundDictionary");
			}
            IDictionaryEnumerator dict = soundDictionary.GetEnumerator();
			while(dict.MoveNext())
			{
				this.Add((string)dict.Key, (Sound)dict.Value);
			}
            return Dictionary.Count;
        }
        
        /// <summary>
        /// Loads and adds a new sound object to the Dictionary.
        /// </summary>
        /// <param name="key">
        /// The key to give the sound object.
        /// </param>
        /// <param name="fileName">
        /// The sound file to load.
        /// </param>
        /// <returns>
        /// The final number of elements within the Dictionary.
        /// </returns>
        public int Add(string key, string fileName)
        {
        	Dictionary.Add(key, Mixer.Sound(fileName));
        	return Dictionary.Count;
		}
        
		/// <summary>
		/// Loads and adds a new sound object to the Dictionary.
		/// </summary>
		/// <param name="sound">
		/// The sound sample to add. Uses ToString() as the key.
		/// </param>
		/// <returns>
		/// The final number of elements within the Dictionary.
		/// </returns>
		public int Add(Sound sound)
		{
			if (sound == null)
			{
				throw new ArgumentNullException("sound");
			}
			Dictionary.Add(sound.ToString(), sound);
			return Dictionary.Count;
		}		
        
        /// <summary>
        /// Removes an element from the Dictionary.
        /// </summary>
        /// <param name="key">
        /// The element's key to remove.
        /// </param>
        public void Remove(string key)
        {
            Dictionary.Remove(key);
        }

		/// <summary>
		/// Stops every sound within the Dictionary.
		/// </summary>
		public void Stop()
		{
			foreach(Sound sound in this.Dictionary.Values)
			{
				sound.Stop();
			}
		}
        
        /// <summary>
        /// Plays every sound within the Dictionary.
        /// </summary>
        public void Play()
        {
			foreach(Sound sound in this.Dictionary.Values)
			{
				sound.Play();
			}
        }
        
        /// <summary>
        /// Sets the volume of every sound object within the Dictionary. 
        /// Gets the average volume of all sound 
        /// objects within the Dictionary.
        /// </summary>
        public int Volume
        {
        	get
        	{
				if(Dictionary.Count > 0)
				{
					int total = 0;
					foreach(Sound sound in this.Dictionary.Values)
					{
						total += sound.Volume;
					}
					return total / Dictionary.Count;
				}
				else
				{
					return 0;
				}
        	}
        	set
        	{
				foreach(Sound sound in this.Dictionary.Values)
				{
					sound.Volume = value;
				}
        	}
		}
		#region IDictionary Members

		/// <summary>
		/// Provide the explicit interface member for IDictionary.
		/// </summary>
		/// <param name="array">Array to copy Dictionary to</param>
		/// <param name="index">Index at which to insert the Dictionary items</param>
		public virtual void CopyTo(Sound[] array, int index)
		{
			((IDictionary)this).CopyTo(array, index);
		}

		/// <summary>
		/// Insert a item into the Dictionary
		/// </summary>
		/// <param name="index">Index at which to insert the item</param>
		/// <param name="sound">item to insert</param>
		public virtual void Insert(int index, Sound sound)
		{
			this.Insert(index, sound);
		} 

		/// <summary>
		/// Gets the index of the given item in the Dictionary.
		/// </summary>
		/// <param name="sound">The item to search for.</param>
		/// <returns>The index of the given sprite.</returns>
		public virtual int IndexOf(Sound sound)
		{
			return this.IndexOf(sound);
		} 

		#endregion
	}
}
