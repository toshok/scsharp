using System;
using System.Drawing;
using System.Collections;

namespace SdlDotNet
{
	/// <summary>
	/// Provides a collection of Rectangles
	/// </summary>
	public class RectangleCollection : CollectionBase, ICollection
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public RectangleCollection( ) : base( )
		{
		}

		/// <summary>
		/// Allows the accessing of items in the collection via the index
		/// </summary>
		public Rectangle this[int index]  
		{
			get  
			{
				return ((Rectangle)List[index]);
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Add a rectangle to the collection
		/// </summary>
		/// <param name="item">Rectangles to add</param>
		/// <returns>The index at which the rectangle was added</returns>
		public int Add(Rectangle item)  
		{
			return (List.Add(item));
		}

		/// <summary>
		/// Retrieve the index value of the rectangle in the collection
		/// </summary>
		/// <param name="item">Rectnalge to search for</param>
		/// <returns>the index value of the rectangle</returns>
		public int IndexOf(Rectangle item)  
		{
			return(List.IndexOf(item));
		}

		/// <summary>
		/// Insert rectangle at certain index in collection
		/// </summary>
		/// <param name="index">Index of collection to insert rectangle
		/// </param>
		/// <param name="item">Rectangle to insert</param>
		public void Insert(int index, Rectangle item)  
		{
			List.Insert(index, item);
		}

		/// <summary>
		/// Remove rectangle from collection
		/// </summary>
		/// <param name="item">Rectangle to remove</param>
		public void Remove(Rectangle item)  
		{
			List.Remove(item);
		}

		/// <summary>
		/// checks to see if rectangle is in the collection
		/// </summary>
		/// <param name="item">Rectangle to check for</param>
		/// <returns>true, if rectangle is in collection</returns>
		public bool Contains(Rectangle item)
		{
			return(List.Contains(item));
		}
		#region ICollection Members

		/// <summary>
		/// Provide the explicit interface member for ICollection.
		/// </summary>
		/// <param name="array">Array to copy collection to</param>
		/// <param name="index">Index at which to insert the collection items</param>
		void ICollection.CopyTo(Array array, int index)
		{
			this.List.CopyTo(array, index);
		}

		/// <summary>
		/// Provide the explicit interface member for ICollection.
		/// </summary>
		/// <param name="array">Array to copy collection to</param>
		/// <param name="index">Index at which to insert the collection items</param>
		public virtual void CopyTo(Rectangle[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		#endregion
	}

}
