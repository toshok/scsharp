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
using System.Drawing;

using SdlDotNet.Particles.Particle;

namespace SdlDotNet.Particles.Manipulators
{
	/// <summary>
	/// A particle manipulator that pulls particles towards a point.
	/// </summary>
	/// <remarks>If the radius is set to -1, the pull (strength) on all particles is constant.</remarks>
	/// <example>
	/// The following example creates a particle vortex that pulls particles towards the middle of the screen.
	/// <code>
	/// ParticleVortex vortex = new ParticleVortex(0.3f);
	/// vortex.X = (float)(Video.Screen.Width / 2);
	/// vortex.Y = (float)(Video.Screen.Height / 2);
	/// particleSystem.Add(vortex);
	/// </code>
	/// </example>
	public class ParticleVortex : IParticleManipulator
	{
		/// <summary>
		/// Creates a particle vortex manipulator with the default values.
		/// </summary>
		public ParticleVortex()
		{
		}
		/// <summary>
		/// Creates a particle vortex manipulator from just the location.
		/// </summary>
		/// <param name="position">The position of the vortex.</param>
		public ParticleVortex(PointF position)
		{
			m_X = position.X;
			m_Y = position.Y;
		}
		/// <summary>
		/// Creates a particle vortex manipulator.
		/// </summary>
		/// <param name="position">The position of the vortex.</param>
		/// <param name="strength">The amount of pull applied to the particles.</param>
		/// <param name="radius">The size of the vortex. -1 is infinate size.</param>
		public ParticleVortex(PointF position, float strength, float radius)
		{
			m_X = position.X;
			m_Y = position.Y;
			m_Strength = strength;
			m_Radius = radius;
		}
		/// <summary>
		/// Creates a particle vortex manipulator with an infinate size.
		/// </summary>
		/// <param name="posX">The X coordinate of the vortex.</param>
		/// <param name="posY">The Y coordinate of the vortex.</param>
		/// <param name="strength">The amount of pull applied to the particles.</param>
		public ParticleVortex(float posX, float posY, float strength)
		{
			m_X = posX;
			m_Y = posY;
			m_Strength = strength;
		}
		/// <summary>
		/// Creates a particle vortex manipulator with an infinate size.
		/// </summary>
		/// <param name="posX">The X coordinate of the vortex.</param>
		/// <param name="posY">The Y coordinate of the vortex.</param>
		/// <param name="strength">The amount of pull applied to the particles.</param>
		/// <param name="radius">The size of the vortex. -1 is infinate size.</param>
		public ParticleVortex(float posX, float posY, float strength, float radius)
		{
			m_X = posX;
			m_Y = posY;
			m_Strength = strength;
			m_Radius = radius;
		}
		/// <summary>
		/// Creates a particle vortex manipulator from just the strength.
		/// </summary>
		/// <param name="strength">The amount of pull applied to the particles.</param>
		public ParticleVortex(float strength)
		{
			m_Strength = strength;
		}
		/// <summary>
		/// Creates a particle vortex manipulator from just the strength and radius.
		/// </summary>
		/// <param name="strength">The amount of pull applied to the particles.</param>
		/// <param name="radius">The size of the vortex. -1 is infinate size.</param>
		public ParticleVortex(float strength, float radius)
		{
			m_Strength = strength;
			m_Radius = radius;
		}

		private float m_X;
		private float m_Y;
		/// <summary>
		/// Gets and sets the Y coordinate of the vortex.
		/// </summary>
		public float Y
		{
			get
			{
				return m_Y;
			}
			set
			{
				m_Y = value;
			}
		}
		/// <summary>
		/// Gets and sets the X coordinate of the vortex.
		/// </summary>
		public float X
		{
			get
			{
				return m_X;
			}
			set
			{
				m_X = value;
			}
		}
		private float m_Radius = -1f;
		/// <summary>
		/// Gets and sets the size, the radius, of the vortex.  If set to -1 the pull will be constant on each particle.
		/// </summary>
		public float Radius
		{
			get
			{
				return m_Radius;
			}
			set
			{
				m_Radius = value;
			}
		}
		private float m_Strength = 0.2f;
		/// <summary>
		/// Gets and sets the amount of pull put onto each particle in the vortex's radius.
		/// </summary>
		public float Strength
		{
			get
			{
				return m_Strength;
			}
			set
			{
				m_Strength = value;
			}
		}
		#region IParticleManipulator Members

		/// <summary>
		/// Applies the vortex force on each particle in the vortex's radius.
		/// </summary>
		/// <param name="particles">The collection of particles to manipulate.</param>
		public void Manipulate(ParticleCollection particles)
		{
			if (particles == null)
			{
				throw new ArgumentNullException("particles");
			}
			foreach(BaseParticle p in particles)
			{
				Vector v = new Vector(p.X,p.Y,m_X,m_Y);
				if(m_Radius == -1f)
				{
					v.Length = m_Strength;
				}
				else
				{
					if(v.Length > m_Radius)
						continue;
					v.Length = (1f - v.Length / m_Radius) * m_Strength;
				}
				p.Velocity += v;
			}
		}

		#endregion
	}
}
