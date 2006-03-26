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
	/// An interface describing a force that manipulates a group of particles.
	/// </summary>
	/// <example>
	/// The following makes a particle manipulator that keeps pixels from going off the left side of the screen.
	/// <code>
	/// public class ParticleBounceLeft : IParticleManipulator
	/// {
	///		public ParticleBounceLeft()
	///		{
	///		}
	///		public void Manipulate(ParticleCollection particles)
	///			foreach(BaseParticle p in particles)
	///			{
	///				if(p.X &lt; 0)
	///				{
	///					p.X = this.X;
	///					p.Velocity.X *= -1;
	///				}
	///			}
	///		}
	/// }
	/// </code>
	/// </example>
	/// <remarks>The Manipulate method should only change the velocity of the particles.</remarks>
	public interface IParticleManipulator
	{
		/// <summary>
		/// Manipulates the given group of particles by the manipulators force.
		/// </summary>
		/// <param name="particles">The collection of particles to manipulate.</param>
		/// <remarks>This should only affect the particles' velocity.</remarks>
		void Manipulate(ParticleCollection particles);
	}
}
