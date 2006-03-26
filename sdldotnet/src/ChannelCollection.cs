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
using System.Drawing;
using System.Collections;
using SdlDotNet;
using Tao.Sdl;

namespace SdlDotNet 
{
	/// <summary>
	/// Encapsulates the collection of Channel objects in the Mixer system.
	/// </summary>
	/// <remarks></remarks>
	public class ChannelCollection : CollectionBase, ICollection
	{
		/// <summary>
		/// Create a Collection to hold channels
		/// </summary>
		/// <remarks></remarks>
		public ChannelCollection() : base()
		{
		}

		/// <summary>
		/// Indexer for the Items in the Collection
		/// </summary>
		/// <remarks></remarks>
		public Channel this[int index] 
		{
			get 
			{ 
				return (Channel)List[index];	
			}
		}

		/// <summary>
		/// Adds the specified Channel to the end of the ChannelCollection.
		/// </summary>
		/// <param name="channel">
		/// The Channel to be added to the end of the ChannelCollection.
		/// </param>
		/// <returns>
		/// The index at which the Channel has been added.
		/// </returns>
		/// <remarks></remarks>
		public int Add(Channel channel)
		{
			return List.Add(channel);
		} 

		/// <summary>
		/// Load a new Channel with the specified index and add 
		/// it to the end of the ChannelCollection.
		/// </summary>
		/// <param name="channelIndex">Index of Channel object
		/// </param>
		/// <returns>
		/// The index at which the Channel has been added.
		/// </returns>
		/// <remarks></remarks>
		public int Add(int channelIndex)
		{
			return List.Add(new Channel(channelIndex));
		} 			

		/// <summary>
		/// Adds the specified Channel to the ChannelCollection.
		/// </summary>
		/// <param name="collectionIndex">index of collection</param>
		/// <param name="channel">channel object</param>
		/// <remarks></remarks>
		public void Insert(int collectionIndex, Channel channel)
		{
			List.Insert(collectionIndex,channel);
		} 

		/// <summary>
		/// Removes a specified Channel from the list.
		/// </summary>
		/// <param name="channel">
		/// The Channel to remove from the ChannelCollection.
		/// </param>
		/// <remarks></remarks>
		public void Remove(Channel channel)
		{
			List.Remove(channel);
		} 

		/// <summary>
		/// Returns the index of a specified Channel in the list.
		/// </summary>
		/// <param name="channel">The channel object</param>
		/// <returns>The index of specified channel in the list</returns>
		/// <remarks></remarks>
		public int IndexOf(Channel channel)
		{
			return List.IndexOf(channel);
		} 

		/// <summary>
		/// Indicates whether a specified Channel is contained in the list.
		/// </summary>
		/// <param name="channel">The Channel to find in the list.</param>
		/// <returns>
		/// true if the Channel is found in the list; otherwise, false.
		/// </returns>
		/// <remarks></remarks>
		public bool Contains(Channel channel)
		{
			return List.Contains(channel);
		} 

		// Provide the explicit interface member for ICollection.
		void ICollection.CopyTo(Array array, int index)
		{
			this.List.CopyTo(array, index);
		}

		/// <summary>
		/// Provide the strongly typed member for ICollection.
		/// </summary>
		/// <param name="array">Array to copy collection to.</param>
		/// <param name="index">Index to copy to.</param>
		/// <remarks>Provide the strongly typed member for ICollection.</remarks> 
		public void CopyTo(Channel[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}
	}	
}
