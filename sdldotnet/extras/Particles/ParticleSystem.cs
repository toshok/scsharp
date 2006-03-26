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

using SdlDotNet;
using SdlDotNet.Particles.Particle;
using SdlDotNet.Particles.Manipulators;
using SdlDotNet.Particles.Emitters;

namespace SdlDotNet.Particles
{
	/// <summary>
	/// A collection of particles manipulated by a number of common manipulators.
	/// </summary>
	/// <example>
	/// The following example creates a particle system with an pixel emitter and a vortex manipulator.
	/// <code>
	/// ParticleSystem system = new ParticleSystem();
	/// system.Add(new ParticleVortex(0.3f));
	/// system.Add(new ParticlePixelEmitter(Color.Black, Color.White));
	/// </code>
	/// </example>
	/// <remarks>Every tick you should call the Update method.  Every time you paint, you should call the Render function.</remarks>
	public class ParticleSystem
	{
		private ParticleManipulatorCollection m_Manipulators;
		private ParticleCollection m_Particles;

		/// <summary>
		/// Gets the collection of manipulators to manipulate the particles in the system.
		/// </summary>
		public ParticleManipulatorCollection Manipulators
		{
			get
			{
				return m_Manipulators;
			}
		}


		/// <summary>
		/// Gets the particles contained in the system.
		/// </summary>
		public ParticleCollection Particles
		{
			get
			{
				return m_Particles;
			}
		}

		/// <summary>
		/// Creates an empty particle system with no manipulators.
		/// </summary>
		public ParticleSystem()
		{
			m_Manipulators = new ParticleManipulatorCollection();
			m_Particles = new ParticleCollection();
		}

		/// <summary>
		/// Creates a particle system with a collection of particles already in it.
		/// </summary>
		/// <param name="particles">The particles to use with this system.</param>
		public ParticleSystem(ParticleCollection particles)
		{
			m_Manipulators = new ParticleManipulatorCollection();
			m_Particles = new ParticleCollection(particles);
		}

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="system">The particle system to copy.</param>
		public ParticleSystem(ParticleSystem system)
		{
			if (system == null)
			{
				throw new ArgumentNullException("system");
			}
			m_Manipulators = new ParticleManipulatorCollection(system.Manipulators);
			m_Particles = new ParticleCollection(system);
		}

		/// <summary>
		/// Creates a particle system with an already created manipulators and particles.
		/// </summary>
		/// <param name="manipulators">The manipulators to associate with this particle system.</param>
		/// <param name="particles">The particles to add to this particle system.</param>
		public ParticleSystem(ParticleManipulatorCollection manipulators, ParticleCollection particles)
		{
			m_Manipulators = manipulators;
			m_Particles = new ParticleCollection(particles);
		}

		/// <summary>
		/// Creates an empty particle system with the desired paritcle manipulators.
		/// </summary>
		/// <param name="manipulators">The manipulators to use with the contained particles.</param>
		public ParticleSystem(ParticleManipulatorCollection manipulators)
		{
			m_Manipulators = manipulators;
			m_Particles = new ParticleCollection();
		}

		/// <summary>
		/// Creates an empty particle system with one particle manipulator.
		/// </summary>
		/// <param name="manipulator">The manipulator to use with this particle system.</param>
		public ParticleSystem(IParticleManipulator manipulator)
		{
			m_Manipulators = new ParticleManipulatorCollection();
			m_Manipulators.Add(manipulator);
			m_Particles = new ParticleCollection();
		}

		/// <summary>
		/// Updates all particles within this system using the given gravity.
		/// </summary>
		/// <returns>True if the system contains particles.</returns>
		public bool Update()
		{
			m_Manipulators.Manipulate(m_Particles);
			return m_Particles.Update();
		}

		/// <summary>
		/// Adds a particle to the system.
		/// </summary>
		/// <param name="particle"></param>
		public void Add(BaseParticle particle)
		{
			m_Particles.Add(particle);
		}

		/// <summary>
		/// Add a particle emitter to the particles.
		/// </summary>
		/// <param name="emitter">The emitter to add.</param>
		public void Add(ParticleEmitter emitter)
		{
			Add(emitter, true);
		}

		/// <summary>
		/// Adds a particle emitter to the particles.
		/// </summary>
		/// <param name="emitter">The emitter to add.</param>
		/// <param name="changeEmitterTarget">Flag to change the emitters target to the particles of this system. Defaults to true.</param>
		public void Add(ParticleEmitter emitter, bool changeEmitterTarget)
		{
			if(emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			if(changeEmitterTarget)
			{
				emitter.Target = m_Particles;
			}
			m_Particles.Add(emitter);
		}

		/// <summary>
		/// Renders all particles on the surface destination.
		/// </summary>
		/// <param name="destination">The destination surface.</param>
		public void Render(Surface destination)
		{
			foreach(BaseParticle particle in m_Particles)
			{
				particle.Render(destination);
			}
		}

		/// <summary>
		/// Adds a particle manipulator to the system.
		/// </summary>
		/// <param name="manipulator"></param>
		public void Add(IParticleManipulator manipulator)
		{
			m_Manipulators.Add(manipulator);
		}
	}
}
