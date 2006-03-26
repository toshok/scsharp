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

using SdlDotNet.Particles.Particle;

namespace SdlDotNet.Particles.Manipulators
{
	/// <summary>
	/// Describes a collection of particle manipulators.
	/// </summary>
	public class ParticleManipulatorCollection : CollectionBase, IParticleManipulator, ICollection
	{
		/// <summary>
		/// Create an empty Particle Manipulator collection.
		/// </summary>
		public ParticleManipulatorCollection()
		{
		}
		/// <summary>
		/// Create a particle manipulator collection with one element in it.
		/// </summary>
		/// <param name="manipulator">The first manipulator in the collection.</param>
		public ParticleManipulatorCollection(IParticleManipulator manipulator)
		{
			Add(manipulator);
		}
		/// <summary>
		/// Create a particle manipulator based off an already existing collection.
		/// </summary>
		/// <param name="manipulators">The manipulators to add to the collection.</param>
		public ParticleManipulatorCollection(ParticleManipulatorCollection manipulators)
		{
			Add(manipulators);
		}

		/// <summary>
		/// Add a particle manipulator to the collection.
		/// </summary>
		/// <param name="manipulator">The manipulator to add to the collection.</param>
		public void Add(IParticleManipulator manipulator)
		{
			List.Add(manipulator);
		}

		/// <summary>
		/// Add a collection of particle manipulators to the collection.
		/// </summary>
		/// <param name="manipulators">The manipulators to add to the collection.</param>
		public void Add(ParticleManipulatorCollection manipulators)
		{
			List.Add(manipulators);
		}
		/// <summary>
		/// Indexer
		/// </summary>
		public IParticleManipulator this[int index]
		{
			get
			{
				return (IParticleManipulator)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

		#region IParticleManipulator Members

		/// <summary>
		/// Manipulate a collection of particles with the manipulators contained in the collection.
		/// </summary>
		/// <param name="particles">The particles to manipulate.</param>
		public void Manipulate(ParticleCollection particles)
		{
			foreach(IParticleManipulator manipulator in List)
			{
				manipulator.Manipulate(particles);
			}
		}

		#endregion

		#region ICollection Members

		/// <summary>
		/// 
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				return List.IsSynchronized;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return List.SyncRoot;
			}
		}

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
		public virtual void CopyTo(IParticleManipulator[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		/// <summary>
		/// Removes particleManipulator from collection
		/// </summary>
		/// <param name="particleManipulator">ParticleManipulator to remove</param>
		public virtual void Remove(IParticleManipulator particleManipulator)
		{
			List.Remove(particleManipulator);
		}

		/// <summary>
		/// Insert a ParticleManipulator into the collection
		/// </summary>
		/// <param name="index">Index at which to insert the particleManipulator</param>
		/// <param name="particleManipulator">ParticleManipulator to insert</param>
		public virtual void Insert(int index, IParticleManipulator particleManipulator)
		{
			List.Insert(index, particleManipulator);
		} 

		/// <summary>
		/// Gets the index of the given ParticleManipulator in the collection.
		/// </summary>
		/// <param name="particleManipulator">The particleManipulator to search for.</param>
		/// <returns>The index of the given ParticleManipulator.</returns>
		public virtual int IndexOf(IParticleManipulator particleManipulator)
		{
			return List.IndexOf(particleManipulator);
		} 

		/// <summary>
		/// Checks if particleManipulator is in the container
		/// </summary>
		/// <param name="particleManipulator">ParticleManipulator to query for</param>
		/// <returns>True is the ParticleManipulator is in the container.</returns>
		public bool Contains(IParticleManipulator particleManipulator)
		{
			return (List.Contains(particleManipulator));
		}

		#endregion
	}
}
