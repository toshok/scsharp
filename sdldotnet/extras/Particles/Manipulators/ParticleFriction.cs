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

using SdlDotNet.Particles.Particle;

namespace SdlDotNet.Particles.Manipulators
{
	/// <summary>
	/// A particle manipulator that slows down particles by a given amount of speed.
	/// </summary>
	/// <example>
	/// The following example creates a particle friction manipulator that slows particles down by 0.5f every update.
	/// <code>
	/// ParticleFriction friction = new ParticleFriction(0.5f);
	/// particleSystem.Add(friction);
	/// </code>
	/// </example>
	public class ParticleFriction : IParticleManipulator
	{
		/// <summary>
		/// Create a ParticleFriction object with the default amount of friction.
		/// </summary>
		public ParticleFriction()
		{
		}
		/// <summary>
		/// Create a ParticleFriction object.
		/// </summary>
		/// <param name="friction">The amount of friction to apply to the particles.</param>
		public ParticleFriction(float friction)
		{
			m_Friction = friction;
		}

		private float m_Friction = 0.1f;
		/// <summary>
		/// Gets and sets the amount of friction applied to the particles.
		/// </summary>
		public float Friction
		{
			get
			{
				return m_Friction;
			}
			set
			{
				m_Friction = value;
			}
		}

		#region IParticleManipulator Members

		/// <summary>
		/// Applies the friction to the given set of particles.
		/// </summary>
		/// <param name="particles">The particles to apply the friction to.</param>
		public void Manipulate(ParticleCollection particles)
		{
			if (particles == null)
			{
				throw new ArgumentNullException("particles");
			}
			foreach(BaseParticle p in particles)
			{
				p.Velocity.Length -= m_Friction;
			}
		}

		#endregion
	}
}
