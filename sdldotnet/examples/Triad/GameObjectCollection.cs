//*****************************************************************************
//	This program is free software; you can redistribute it and/or
//	modify it under the terms of the GNU General Public License
//	as published by the Free Software Foundation; either version 2
//	of the License, or (at your option) any later version.
//	This program is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//	GNU General Public License for more details.
//	You should have received a copy of the GNU General Public License
//	along with this program; if not, write to the Free Software
//	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//	
//	Created by Michael Rosario
//	July 29th,2003
//	Contact me at mrosario@scrypt.net	
//*****************************************************************************


using System;
using System.Collections;

namespace SdlDotNet.Examples.Triad
{
	/// <summary>
	/// A collection of elements of type GameObject
	/// </summary>
	public class GameObjectCollection: CollectionBase, ICollection
	{
		/// <summary>
		/// Initializes a new empty instance of the GameObjectCollection class.
		/// </summary>
		public GameObjectCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the GameObjectCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new GameObjectCollection.
		/// </param>
		public GameObjectCollection(GameObject[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the GameObjectCollection class, containing elements
		/// copied from another instance of GameObjectCollection
		/// </summary>
		/// <param name="items">
		/// The GameObjectCollection whose elements are to be added to the new GameObjectCollection.
		/// </param>
		public GameObjectCollection(GameObjectCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this GameObjectCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this GameObjectCollection.
		/// </param>
		public void AddRange(GameObject[] items)
		{
			foreach (GameObject item in items)
			{
				if(item==null)
				{
					throw new GameException("Game object is null while adding to collection.");
				}

				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another GameObjectCollection 
		/// to the end of this GameObjectCollection.
		/// </summary>
		/// <param name="items">
		/// The GameObjectCollection whose elements are to be 
		/// added to the end of this GameObjectCollection.
		/// </param>
		public void AddRange(GameObjectCollection items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (GameObject item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type GameObject to the end of this GameObjectCollection.
		/// </summary>
		/// <param name="value">
		/// The GameObject to be added to the end of this GameObjectCollection.
		/// </param>
		public virtual void Add(GameObject value)
		{
			if(value == null)
			{
				throw new GameException("Input game object is null.");
			}

			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic GameObject value is in this GameObjectCollection.
		/// </summary>
		/// <param name="value">
		/// The GameObject value to locate in this GameObjectCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this GameObjectCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(GameObject value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this GameObjectCollection
		/// </summary>
		/// <param name="value">
		/// The GameObject value to locate in the GameObjectCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(GameObject value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the GameObjectCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the GameObject is to be inserted.
		/// </param>
		/// <param name="value">
		/// The GameObject to insert.
		/// </param>
		public virtual void Insert(int index, GameObject value)
		{
			if(value == null)
			{
				throw new GameException("GameObject is null during insert.");
			}

			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the GameObject at the given index in this GameObjectCollection.
		/// </summary>
		public virtual GameObject this[int index]
		{
			get
			{
				return (GameObject) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific GameObject from this GameObjectCollection.
		/// </summary>
		/// <param name="value">
		/// The GameObject value to remove from this GameObjectCollection.
		/// </param>
		public virtual void Remove(GameObject value)
		{
			if(value==null)
			{
				throw new GameException("You can not remove a null game object.");
			}

			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by GameObjectCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="collection"></param>
			public Enumerator(GameObjectCollection collection)
			{
				if (collection == null)
				{
					throw new ArgumentNullException("collection");
				}
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			/// <summary>
			/// 
			/// </summary>
			public GameObject Current
			{
				get
				{
					return (GameObject) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (GameObject) (this.wrapped.Current);
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			/// <summary>
			/// 
			/// </summary>
			public void Reset()
			{
				this.wrapped.Reset(); 
			}
		}

		/// <summary>
		/// Returns an enumerator that can iterate through the elements of this GameObjectCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual GameObjectCollection.Enumerator GetEnumerator()
		{
			return new GameObjectCollection.Enumerator(this);
		}
		#region ICollection Members
		/// <summary>
		/// 
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				// TODO:  Add GameObjectCollection.IsSynchronized getter implementation
				return false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(GameObject[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}
		/// <summary>
		/// 
		/// </summary>
		public object SyncRoot
		{
			get
			{
				// TODO:  Add GameObjectCollection.SyncRoot getter implementation
				return null;
			}
		}
		// Provide the explicit interface member for ICollection.
		void ICollection.CopyTo(Array array, int index)
		{
			this.List.CopyTo(array, index);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			// TODO:  Add GameObjectCollection.System.Collections.IEnumerable.GetEnumerator implementation
			return null;
		}

		#endregion
	}

}
