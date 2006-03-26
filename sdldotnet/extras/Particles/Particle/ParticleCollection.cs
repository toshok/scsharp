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

using SdlDotNet.Particles.Emitters;

namespace SdlDotNet.Particles.Particle
{
	/// <summary>
	/// A collection of particles.
	/// </summary>
	public class ParticleCollection : CollectionBase, ICollection
	{
		/// <summary>
		/// Creates a new ParticleCollection.
		/// </summary>
		public ParticleCollection()
		{
		}

		/// <summary>
		/// Creates a new ParticleCollection with one particle element.
		/// </summary>
		/// <param name="particle">The particle to start off the collection.</param>
		public ParticleCollection(BaseParticle particle)
		{
			Add(particle);
		}

		/// <summary>
		/// Creates a new ParticleCollection from an already existing collection of particles.
		/// </summary>
		/// <param name="collection">The collection to add.</param>
		public ParticleCollection(ParticleCollection collection)
		{
			Add(collection);
		}

		/// <summary>
		/// Creates a particle collection from the particles in the provided system.
		/// </summary>
		/// <param name="system">The system containing all the particles to add.</param>
		public ParticleCollection(ParticleSystem system)
		{
			Add(system);
		}

		/// <summary>
		/// Adds a particle to the collection.
		/// </summary>
		/// <param name="particle">The particle to add.</param>
		public void Add(BaseParticle particle)
		{
			List.Add(particle);
		}

		/// <summary>
		/// Adds a particle emitter to the collection.
		/// </summary>
		/// <param name="emitter">The emitter to add to the collection.</param>
		public void Add(ParticleEmitter emitter)
		{
			Add(emitter, true);
		}

		/// <summary>
		/// Adds a particle emitter to the collection.
		/// </summary>
		/// <param name="emitter">The emitter to add to the collection.</param>
		/// <param name="changeEmitterTarget">Flag to chage the emitter's target particle collection.  Defaults to true.</param>
		public void Add(ParticleEmitter emitter, bool changeEmitterTarget)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			List.Add(emitter);
			if(changeEmitterTarget)
			{
				emitter.Target = this;
			}
		}

		/// <summary>
		/// Adds a collection of particles to the collection.
		/// </summary>
		/// <param name="collection">The collection of particles to add.</param>
		public void Add(ParticleCollection collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			foreach(BaseParticle particle in collection)
			{
				List.Add(particle);
			}
		}

		/// <summary>
		/// Adds a collection of particles from a particle system.
		/// </summary>
		/// <param name="system">The system containing all the particles.</param>
		public void Add(ParticleSystem system)
		{
			if(system == null)
			{
				throw new ArgumentNullException("system");
			}
			Add(system.Particles);
		}

		/// <summary>
		/// Updates all particles in the collection.
		/// </summary>
		/// <returns>True if a particle is still alive.</returns>
		public virtual bool Update()
		{
			BaseParticle particle;
			int count = List.Count;
			for(int i = 0; i < count; i++)
			{
				particle = (BaseParticle)List[i];
				if(!particle.Update())
				{
					List.RemoveAt(i--);
					count--;
				}
			}
			return count > 0;
		}

		/// <summary>
		/// Renders all particles on a destination surface.
		/// </summary>
		/// <param name="destination">The surface to render the particles onto.</param>
		public void Render(Surface destination)
		{
			foreach(BaseParticle particle in List)
			{
				particle.Render(destination);
			}
		}

		/// <summary>
		/// Indexer.
		/// </summary>
		public BaseParticle this[int index]
		{
			get
			{
				return (BaseParticle)List[index];
			}
			set
			{
				List[index] = value;
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
		public virtual void CopyTo(BaseParticle[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		/// <summary>
		/// Removes particle from collection
		/// </summary>
		/// <param name="particle">Particle to remove</param>
		public virtual void Remove(BaseParticle particle)
		{
			List.Remove(particle);
		}

		/// <summary>
		/// Insert a Particle into the collection
		/// </summary>
		/// <param name="index">Index at which to insert the particle</param>
		/// <param name="particle">Particle to insert</param>
		public virtual void Insert(int index, BaseParticle particle)
		{
			List.Insert(index, particle);
		} 

		/// <summary>
		/// Gets the index of the given Particle in the collection.
		/// </summary>
		/// <param name="particle">The particle to search for.</param>
		/// <returns>The index of the given Particle.</returns>
		public virtual int IndexOf(BaseParticle particle)
		{
			return List.IndexOf(particle);
		} 

		/// <summary>
		/// Checks if particle is in the container
		/// </summary>
		/// <param name="particle">Particle to query for</param>
		/// <returns>True is the Particle is in the container.</returns>
		public bool Contains(BaseParticle particle)
		{
			return (List.Contains(particle));
		}
	}
}
