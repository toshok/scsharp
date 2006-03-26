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
using System.IO;

using SdlDotNet;
using Tao.Sdl;

namespace SdlDotNet 
{
	/// <summary>
	/// Encapsulates the collection of Surface objects.
	/// </summary>
	public class SurfaceCollection : CollectionBase, ICollection
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SurfaceCollection() : base()
		{
		}

		/// <summary>
		/// Constructor to make a new surface collection based off of an existing one.
		/// </summary>
		/// <param name="surfaces">The surface collection to copy.</param>
		public SurfaceCollection(SurfaceCollection surfaces)
		{
			this.Add(surfaces);
		}

		/// <summary>
		/// Create collection with image as the first surface
		/// </summary>
		/// <param name="fileName">
		/// filename of surface to put into collection
		/// </param>
		public SurfaceCollection(string fileName)
		{
			this.List.Add(new Surface(fileName));
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.List.CopyTo(array, index);
		}

		/// <summary>
		/// Create a collection with one surface element.
		/// </summary>
		/// <param name="surface">The surface to add to the new collection.</param>
		public SurfaceCollection(Surface surface)
		{
			Add(surface);
		}

		/// <summary>
		/// Load in multiple files as surfaces
		/// </summary>
		/// <param name="baseName">Base name of files</param>
		/// <param name="extension">file extension of images</param>
		public SurfaceCollection(string baseName, string extension)
		{
			// Save the fields
			//this.filename = baseName + "-*" + extension;
      
			// Load the images into memory
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
				this.List.Add(new Surface(fn));
				i++;
			}
		}

		/// <summary>
		/// Loads a collection of tiled surfaces from one larger surface. 
		/// </summary>
		/// <param name="fileName">
		/// The filename of the surface which contains all the tiles.
		/// </param>
		/// <param name="tileSize">
		/// The size of each tile.
		/// </param>
		public SurfaceCollection(string fileName, Size tileSize) : 
			this(new Surface(fileName), tileSize)
		{
		}

		/// <summary>
		/// Loads a row of tiled surfaces from one larger surface.
		/// </summary>
		/// <param name="fileName">
		/// The filename of the large surface.
		/// </param>
		/// <param name="tileSize">
		/// The size of each tile.
		/// </param>
		/// <param name="rowNumber">
		/// The row number of which to load the surface collection.
		/// </param>
		public SurfaceCollection(string fileName, Size tileSize, int rowNumber) :
			this(new Surface(fileName), tileSize, rowNumber)
		{
		}

		/// <summary> 
		/// Loads a collection of tiled surfaces from one larger surface. 
		/// </summary> 
		/// <param name="fullImage">
		/// The larger surface which contains all the tiles.
		/// </param> 
		/// <param name="tileSize">
		/// The size of each tile.
		/// </param> 
		public SurfaceCollection(
			Surface fullImage, 
			Size tileSize) 
		{ 
			if (fullImage == null)
			{
				throw new ArgumentNullException("fullImage");
			}
			fullImage.Alpha = 255;
			
			for(int tileY = 0; tileY * tileSize.Height < fullImage.Height; tileY++) 
			{ 
				for(int tileX = 0; tileX * tileSize.Width < fullImage.Width; tileX++) 
				{ 
					Surface tile = fullImage.CreateCompatibleSurface(tileSize.Width, tileSize.Height);

					//tile.ClearTransparentColor();
					//fullImage.ClearTransparentColor();
					//Surface tile = new Surface(tileSize.Width, tileSize.Height);
					//tile.TransparentColor = Color.Black;
					
					tile.Blit(
						fullImage, 
						new Point(0,0), 
						new Rectangle(tileX * tileSize.Width, 
						tileY * tileSize.Height, tileSize.Width, tileSize.Height));
					this.List.Add(tile); 
				} 
			} 
		}
		
		/// <summary>
		/// Loads only one row of tiled surfaces from a larger surface.
		/// </summary>
		/// <param name="fullImage">
		/// The larger surface which contains all the tiles.
		/// </param> 
		/// <param name="tileSize">
		/// The size of each tile.
		/// </param>
		/// <param name="rowNumber">
		/// The row to be loaded.
		/// </param>
		public SurfaceCollection(Surface fullImage, Size tileSize, int rowNumber)
		{
			if (fullImage == null)
			{
				throw new ArgumentNullException("fullImage");
			}
			fullImage.Alpha = 0;
			for(int tileX = 0; tileX * tileSize.Width < fullImage.Width; tileX++)
			{
				Surface tile = fullImage.CreateCompatibleSurface(tileSize.Width, tileSize.Height);
				//tile.Fill(fullImage.TransparentColor);
				tile.Blit(
					fullImage, 
					new Point(0,0), 
					new Rectangle(tileX * tileSize.Width, 
					rowNumber * tileSize.Height, tileSize.Width, tileSize.Height)); 
				//tile.TransparentColor = fullImage.TransparentColor;
				this.List.Add(tile); 
			}
		}

		/// <summary>
		/// Indexer for the Items in the Collection
		/// </summary>
		public virtual Surface this[int index] 
		{
			get 
			{ 
				if (this.Count == 0)
				{
					return ((Surface)List[index]);
				}
				else
				{
					return ((Surface)List[index % this.Count]);	
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
		/// Adds the specified Surface to the end of the SurfaceCollection.
		/// </summary>
		/// <param name="surface">
		/// The Surface to be added to the end of the SurfaceCollection.
		/// </param>
		/// <returns>
		/// The index at which the Surface has been added.
		/// </returns>
		public int Add(Surface surface)
		{
			return (List.Add(surface));
		} 

		/// <summary>
		/// Adds the specified Surface to the end of the SurfaceCollection.
		/// </summary>
		/// <param name="surfaceCollection">
		/// The Surface to be added to the end of the SurfaceCollection.
		/// </param>
		/// <returns>
		/// The index at which the Surface has been added.
		/// </returns>
		public int Add(SurfaceCollection surfaceCollection)
		{
			if (surfaceCollection == null)
			{
				throw new ArgumentNullException("surfaceCollection");
			}
			for (int i = 0; i < surfaceCollection.Count; i++)
			{
				this.List.Add(surfaceCollection[i]);
			}
			return this.Count;
		} 

		/// <summary>
		/// Load a Surface with the specified filename and add 
		/// it to the end of the SurfaceCollection.
		/// </summary>
		/// <param name="fileName">
		/// The filename of the Surface 
		/// to be added to the end of the SurfaceCollection.
		/// </param>
		/// <returns>
		/// The index at which the Surface has been added.
		/// </returns>
		public int Add(string fileName)
		{
			return (this.Add(new Surface(fileName)));
		} 


		/// <summary>
		/// Create a Surface from a byte array and add it to the end 
		/// of the SurfaceCollection.
		/// </summary>
		/// <param name="array">
		/// The array of byte to create the Image 
		/// from.
		/// </param>
		/// <returns>
		/// The index at which the Surface has been added.
		/// </returns>
		public int Add(byte[] array)
		{
			return (List.Add(new Surface(array)));
		} 

		/// <summary>
		/// Create a Surface from a System.Drawing.Bitmap and add it 
		/// to the end of the SurfaceCollection.
		/// </summary>
		/// <param name="bitmap">
		/// The System.Drawing.Bitmap to create the 
		/// Image from.
		/// </param>
		/// <returns>
		/// The index at which the Surface has been added.
		/// </returns>
		public int Add(Bitmap bitmap)
		{
			return (List.Add(new Surface(bitmap)));
		} 			

		/// <summary>
		/// Adds the specified Surface to the SurfaceCollection.
		/// </summary>
		/// <param name="index">Index at which to insert to new surface</param>
		/// <param name="surface">Surface to insert</param>
		public void Insert(int index, Surface surface)
		{
			List.Insert(index, surface);
		} 

		/// <summary>
		/// Removes a specified Surface from the collection.
		/// </summary>
		/// <param name="surface">
		/// The Surface to remove from the SurfaceCollection.
		/// </param>
		public void Remove(Surface surface)
		{
			List.Remove(surface);
		} 

		/// <summary>
		/// Returns the index of a specified Surface in the collection.
		/// </summary>
		/// <param name="surface">The surface object</param>
		/// <returns>The index of specified surface in the collection</returns>
		public int IndexOf(Surface surface)
		{
			return (List.IndexOf(surface));
		} 

		/// <summary>
		/// Indicates whether a specified Surface is contained in the collection.
		/// </summary>
		/// <param name="surface">
		/// The Surface to find in the collection.
		/// </param>
		/// <returns>
		/// true if the Surface is found in the collection; otherwise, false.
		/// </returns>
		public bool Contains(Surface surface)
		{
			return (List.Contains(surface));
		}

		/// <summary>
		/// Size of first surface
		/// </summary>
		public virtual Size Size
		{
			get 
			{ 
				return new Size(this[0].Width, this[0].Height); 
			}
		}

		/// <summary>
		/// Copy surface collection to array
		/// </summary>
		/// <param name="array">Array to copy collection to</param>
		/// <param name="index">Start at this index</param>
		public virtual void CopyTo(Surface[] array, int index)
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
		/// Sets the alpha property of every surface in the collection.
		/// Gets the alpha of the first surface.
		/// </summary>
		public byte Alpha
		{
			get
			{
				return this[0].Alpha;
			}
			set
			{
				foreach(Surface surface in this.List)
				{
					surface.Alpha = value;
				}
			}
		}

		/// <summary>
		/// Gets the transparent color of the first surface.
		/// Sets the transparent color of every surface in the collection.
		/// </summary>
		public Color TransparentColor
		{
			get
			{
				return this[0].TransparentColor;
			}
			set
			{
				foreach(Surface surface in this.List)
				{
					surface.TransparentColor = value;
				}
			}
		}
	}	
}
