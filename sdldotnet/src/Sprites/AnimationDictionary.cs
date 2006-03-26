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

namespace SdlDotNet.Sprites
{
	/// <summary>
	/// Summary description for Animation.
	/// </summary>
	public class AnimationDictionary : DictionaryBase
    {
        #region Constructors
        /// <summary>
		/// Creates an empty AnimationDictionary.
		/// </summary>
		public AnimationDictionary() : base()
		{
		}

        /// <summary>
        /// Creates an AnimationDictionary with one animation with the key "Default".
        /// </summary>
        /// <param name="animation"></param>
        public AnimationDictionary(Animation animation)
        {
            this.Add("Default", animation);
        }

		/// <summary>
		/// Creates an AnimationDictionary with one element within it.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="animation"></param>
		public AnimationDictionary(string key, Animation animation)
		{
			this.Add(key, animation);
		}

        /// <summary>
        /// Creates an AnimationDictionary with a "Default" animation of surfaces.
        /// </summary>
        /// <param name="surfaces"></param>
        public AnimationDictionary(SurfaceCollection surfaces)
        {
            this.Add("Default", surfaces);
        }


		/// <summary>
		/// Creates a new AnimationDictionary with the contents of an existing AnimationDictionary.
		/// </summary>
		/// <param name="animationDictionary">The existing music Dictionary to add.</param>
		public AnimationDictionary(AnimationDictionary animationDictionary)
		{
			this.Add(animationDictionary);
        }
        #endregion Constructors

        #region Properties
        /// <summary>
		/// Gets and sets an animation object within the 
		/// Dictionary using the animation's key.
		/// </summary>
		public Animation this[string key]
		{
			get 
			{
				return((Animation)Dictionary[key]);
			}
			set
			{
				Dictionary[key] = value;
			}
		}
		
		/// <summary>
		/// Gets all the Keys in the Dictionary.
		/// </summary>
		public ICollection Keys  
		{
			get  
			{
				return Dictionary.Keys;
			}
		}
        
		/// <summary>
		/// Gets all the Values in the Dictionary.
		/// </summary>
		public ICollection Values  
		{
			get  
			{
				return Dictionary.Values;
			}
        }

        /// <summary>
        /// Gets the average delay of all animations in the Dictionary, 
        /// sets the delay of every animation in the Dictionary.
        /// </summary>
        public double Delay
        {
            get
            {
                double average = 0;
                IDictionaryEnumerator dict = Dictionary.GetEnumerator();
				while (dict.MoveNext())
				{
					average += ((Animation)dict.Value).Delay;
				}
                return average / this.Count;
            }
            set
            {
                IDictionaryEnumerator dict = Dictionary.GetEnumerator();
				while (dict.MoveNext())
				{
					((Animation)dict.Value).Delay = value;
				}
            }
        }

        /// <summary>
        /// Gets the average FrameIncrement for each Animation.  
        /// Sets the FrameIncrement of each Animation in the Dictionary.
        /// </summary>
        public int FrameIncrement
        {
            get
            {
                int average = 0;
                IDictionaryEnumerator dict = Dictionary.GetEnumerator();
				while (dict.MoveNext())
				{
					average += ((Animation)dict.Value).FrameIncrement;
				}
                return average / this.Count;
            }
            set
            {
                IDictionaryEnumerator dict = Dictionary.GetEnumerator();
				while (dict.MoveNext())
				{
					((Animation)dict.Value).FrameIncrement = value;
				}
            }
        }


        /// <summary>
        /// Gets whether all animations animate forward and 
        /// sets whether all animations in the Dictionary 
        /// are to animate forward.
        /// </summary>
        public bool AnimateForward
        {
            get
            {
                IDictionaryEnumerator dict = Dictionary.GetEnumerator();
                while (dict.MoveNext())
                {
                    if (!((Animation)dict.Value).AnimateForward)
                    {
                        return false;
                    }
                }
                return true;
            }
            set
            {
                IDictionaryEnumerator dict = Dictionary.GetEnumerator();
				while (dict.MoveNext())
				{
					((Animation)dict.Value).AnimateForward = value;
				}
            }
        }

        /// <summary>
        /// Gets whether the first animation is looping, sets whether every animation in the Dictionary is to be looped.
        /// </summary>
        public bool Loop
        {
            get
            {
                IDictionaryEnumerator dict = Dictionary.GetEnumerator();
				while (dict.MoveNext())
				{
					return ((Animation)dict.Value).Loop;
				}
                return true;
            }
            set
            {
                IDictionaryEnumerator dict = Dictionary.GetEnumerator();
				while (dict.MoveNext())
				{
					((Animation)dict.Value).Loop = value;
				}
            }
        }
        #endregion Properties
        
        #region Functions
        /// <summary>
		/// Adds an animation to the Dictionary.
		/// </summary>
		/// <param name="key">The name of the animation.</param>
		/// <param name="animation">The animation object.</param>
		/// <returns>The final number of elements within the Dictionary.</returns>
		public int Add(string key, Animation animation) 
		{
			Dictionary.Add(key, animation);
			return Dictionary.Count;
		}

        /// <summary>
        /// Adds a surface Dictionary to the Dictionary as an animation.
        /// </summary>
        /// <param name="key">The name of the animation.</param>
        /// <param name="surfaces">The SurfaceDictionary that represents the animation.</param>
        /// <returns>The final number of elements within the Dictionary.</returns>
        public int Add(string key, SurfaceCollection surfaces)
        {
            Dictionary.Add(key, new Animation(surfaces));
            return Dictionary.Count;
        }
		
		/// <summary>
		/// Adds a Dictionary of music to the current music Dictionary.
		/// </summary>
		/// <param name="animationDictionary">
		/// The Dictionary of 
		/// music samples to add.
		/// </param>
		/// <returns>
		/// The total number of elements within 
		/// the Dictionary after adding the sample.
		/// </returns>
		public int Add(AnimationDictionary animationDictionary)
		{
			if (animationDictionary == null)
			{
				throw new ArgumentNullException("animationDictionary");
			}
			IDictionaryEnumerator dict = animationDictionary.GetEnumerator();
			while(dict.MoveNext())
			{
				this.Add((string)dict.Key, (Animation)dict.Value);
			}
			return Dictionary.Count;
        }

        /// <summary>
        /// Returns true if the Dictionary contains the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return Dictionary.Contains(key);
        }
        
		/// <summary>
		/// Removes an element from the Dictionary.
		/// </summary>
		/// <param name="key">The element's key to remove.</param>
		public void Remove(string key)
		{
			Dictionary.Remove(key);
		}
        #endregion Functions

		#region IDictionary Members

		/// <summary>
		/// Provide the explicit interface member for IDictionary.
		/// </summary>
		/// <param name="array">Array to copy Dictionary to</param>
		/// <param name="index">Index at which to insert the Dictionary items</param>
		public virtual void CopyTo(Animation[] array, int index)
		{
			((IDictionary)this).CopyTo(array, index);
		}

		/// <summary>
		/// Insert a item into the Dictionary
		/// </summary>
		/// <param name="index">Index at which to insert the item</param>
		/// <param name="animation">item to insert</param>
		public virtual void Insert(int index, Animation animation)
		{
			this.Insert(index, animation);
		}

		/// <summary>
		/// Gets the index of the given item in the Dictionary.
		/// </summary>
		/// <param name="animation">The item to search for.</param>
		/// <returns>The index of the given sprite.</returns>
		public virtual int IndexOf(Animation animation)
		{
			return this.IndexOf(animation);
		} 

		#endregion
	}
}
