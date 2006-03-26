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
	/// A collection of elements of type Block
	/// </summary>
	public class BlockCollection: CollectionBase, ICollection
	{
		/// <summary>
		/// Initializes a new empty instance of the BlockCollection class.
		/// </summary>
		public BlockCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the BlockCollection class, 
		/// containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new 
		/// BlockCollection.
		/// </param>
		public BlockCollection(Block[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the BlockCollection class, 
		/// containing elements
		/// copied from another instance of BlockCollection
		/// </summary>
		/// <param name="items">
		/// The BlockCollection whose elements are to be added to the 
		/// new BlockCollection.
		/// </param>
		public BlockCollection(BlockCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this BlockCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this
		/// BlockCollection.
		/// </param>
		public void AddRange(Block[] items)
		{
			foreach (Block item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another BlockCollection to the 
		/// end of this BlockCollection.
		/// </summary>
		/// <param name="items">
		/// The BlockCollection whose elements are to be added to 
		/// the end of this BlockCollection.
		/// </param>
		public void AddRange(BlockCollection items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (Block item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type Block to the end of this BlockCollection.
		/// </summary>
		/// <param name="value">
		/// The Block to be added to the end of this BlockCollection.
		/// </param>
		public virtual void Add(Block value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic Block value is in this 
		/// BlockCollection.
		/// </summary>
		/// <param name="value">
		/// The Block value to locate in this BlockCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this BlockCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(Block value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of 
		/// a specific value
		/// in this BlockCollection
		/// </summary>
		/// <param name="value">
		/// The Block value to locate in the BlockCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the 
		/// _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(Block value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the BlockCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the Block is to be inserted.
		/// </param>
		/// <param name="value">
		/// The Block to insert.
		/// </param>
		public virtual void Insert(int index, Block value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the Block at the given index in this BlockCollection.
		/// </summary>
		public virtual Block this[int index]
		{
			get
			{
				return (Block) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific 
		/// Block from this BlockCollection.
		/// </summary>
		/// <param name="value">
		/// The Block value to remove from this BlockCollection.
		/// </param>
		public virtual void Remove(Block value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by BlockCollection.
		/// GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="collection"></param>
			public Enumerator(BlockCollection collection)
			{
				if (collection == null)
				{
					throw new ArgumentNullException("collection");
				}
				this.wrapped = 
					((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			/// <summary>
			/// 
			/// </summary>
			public Block Current
			{
				get
				{
					return (Block) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (Block) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the 
		/// elements of this BlockCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual BlockCollection.Enumerator GetEnumerator()
		{
			return new BlockCollection.Enumerator(this);
		}
		#region ICollection Members
		/// <summary>
		/// 
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				// TODO:  Add BlockCollection.IsSynchronized getter implementation
				return false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Block[] array, int index)
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
				// TODO:  Add BlockCollection.SyncRoot getter implementation
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
			// TODO:  Add BlockCollection.System.Collections.IEnumerable.GetEnumerator implementation
			return null;
		}

		#endregion
	}


}
