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
	/// A particle manipulator that pulls all particles by a common gravity.
	/// </summary>
	/// <example>
	/// The following example puts a vertical gravity of 0.4f in the particleSystem.
	/// <code>
	/// ParticleGravity grav = new ParticleGravity(0.4f);
	/// particleSystem.Add(grav);
	/// </code>
	/// </example>
	public class ParticleGravity : IParticleManipulator
	{
		private Vector m_Velocity = new Vector(0f,0.2f);
		/// <summary>
		/// Creates a new ParticleSystem with a common gravity.
		/// </summary>
		/// <param name="velocity">The velocity (horizontal and vertical) of the particle system.</param>
		public ParticleGravity(Vector velocity)
		{
			m_Velocity = velocity;
		}

		/// <summary>
		/// Creates a new ParticleSystem using gravity and wind.
		/// </summary>
		/// <param name="gravity">The vertical gravity of the system.</param>
		/// <param name="wind">The horizontal gravity of the system.  This is commonly refered to as wind.</param>
		public ParticleGravity(float gravity, float wind) : this(new Vector(wind, gravity))
		{
		}

		/// <summary>
		/// Creates a new ParticleSystem with a vertical gravity.
		/// </summary>
		/// <param name="gravity">The vertical gravity of the system.</param>
		public ParticleGravity(float gravity) : this(new Vector(0,gravity))
		{
		}

		/// <summary>
		/// Creates a new ParticleSystem.
		/// </summary>
		/// <remarks>The gravity defaults as "new Vector()".</remarks>
		public ParticleGravity() : this(new Vector())
		{
		}

		/// <summary>
		/// Gets and sets the velocity (direction and speed) of the gravity as a vector.
		/// </summary>
		public Vector Velocity
		{
			get
			{
				return m_Velocity;
			}
			set
			{
				m_Velocity = value;
			}
		}

		/// <summary>
		/// Gets and sets the vertical pull of the gravity.
		/// </summary>
		public float Gravity
		{
			get
			{
				return m_Velocity.Y;
			}
			set
			{
				m_Velocity.Y = value;
			}
		}

		/// <summary>
		/// Gets and sets the horizontal push of the gravity.
		/// </summary>
		public float Wind
		{
			get
			{
				return m_Velocity.X;
			}
			set
			{
				m_Velocity.X = value;
			}
		}


		#region IParticleManipulator Members

		/// <summary>
		/// Manipulate particles by the gravity.
		/// </summary>
		/// <param name="particles">The particles to pull by the gravity.</param>
		public void Manipulate(ParticleCollection particles)
		{
			if (particles == null)
			{
				throw new ArgumentNullException("particles");
			}
			foreach(BaseParticle p in particles)
			{
				p.Velocity += m_Velocity;
			}
		}

		#endregion
	}
}
