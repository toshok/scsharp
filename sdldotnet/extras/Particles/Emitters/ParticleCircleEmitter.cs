/*
 * $RCSfile: ParticlePixelEmitter.cs,v $
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

namespace SdlDotNet.Particles.Emitters
{
	/// <summary>
	/// A particle emitter that emits circle-based particles.
	/// </summary>
	/// <example><code>
	/// ParticleCircleEmitter emitter = new ParticleCircleEmitter();
	/// emitter.ColorMin = Color.Black;
	/// emitter.ColorMax = Color.White;
	/// emitter.RadiusMin = 1;
	/// emitter.RadiusMax = 3;
	/// particleSystem.Add(emitter);
	/// </code></example>
	public class ParticleCircleEmitter : ParticlePixelEmitter
	{
		/// <summary>
		/// Creates a new pixel particle emitter.
		/// </summary>
		public ParticleCircleEmitter() : base()
		{
		}

		/// <summary>
		/// Creates a new pixel particle emitter inside a particle system.
		/// </summary>
		/// <param name="system">The system that the particle emitter should be added to.</param>
		public ParticleCircleEmitter(ParticleSystem system) : base(system)
		{
		}

		/// <summary>
		/// Creates a particle emitter that emits particels inside the given system with min and max color values.
		/// </summary>
		/// <param name="system">The system that the particle emitter should be added to.</param>
		/// <param name="minColor">The minimum color values of emitted particles.</param>
		/// <param name="maxColor">The maximum color values of emitted particles.</param>
		/// <param name="radiusMin">The minimum radius of a particle.</param>
		/// <param name="radiusMax">The maximum radius of a particle.</param>
		public ParticleCircleEmitter(ParticleSystem system, Color minColor, Color maxColor, short radiusMin, short radiusMax) : base(system)
		{
			ColorMin = minColor;
			ColorMax = maxColor;
			m_RadiusMin = radiusMin;
			m_RadiusMax = radiusMax;
		}

		/// <summary>
		/// Creates a particle emitter with min and max color values.
		/// </summary>
		/// <param name="minColor">The minimum color values of emitted particles.</param>
		/// <param name="maxColor">The maximum color values of emitted particles.</param>
		/// <param name="radiusMin">The minimum radius of a particle.</param>
		/// <param name="radiusMax">The maximum radius of a particle.</param>
		public ParticleCircleEmitter(Color minColor, Color maxColor, short radiusMin, short radiusMax) : base()
		{
			ColorMin = minColor;
			ColorMax = maxColor;
			m_RadiusMin = radiusMin;
			m_RadiusMax = radiusMax;
		}

		/// <summary>
		/// Creates a particle emitter with min and max color values.
		/// </summary>
		/// <param name="minColor">The minimum color values of emitted particles.</param>
		/// <param name="maxColor">The maximum color values of emitted particles.</param>
		public ParticleCircleEmitter(Color minColor, Color maxColor) : base()
		{
			ColorMin = minColor;
			ColorMax = maxColor;
		}

		/// <summary>
		/// Creates a particle emitter with radius values.
		/// </summary>
		/// <param name="radiusMin">The minimum radius of a particle.</param>
		/// <param name="radiusMax">The maximum radius of a particle.</param>
		public ParticleCircleEmitter(short radiusMin, short radiusMax) : base()
		{
			m_RadiusMin = radiusMin;
			m_RadiusMax = radiusMax;
		}

		private short m_RadiusMin = 1;
		private short m_RadiusMax = 10;
		/// <summary>
		/// Gets and sets the maximum radius of particles.
		/// </summary>
		public short RadiusMax
		{
			get
			{
				return m_RadiusMax;
			}
			set
			{
				m_RadiusMax = value;
			}
		}
		/// <summary>
		/// Gets and sets the minimum radius for particles.
		/// </summary>
		public short RadiusMin
		{
			get
			{
				return m_RadiusMin;
			}
			set
			{
				m_RadiusMin = value;
			}
		}

		/// <summary>
		/// A protected method to return a new particle pixel with the randomized color attributes.
		/// </summary>
		/// <returns>A new particle pixel with the new color values.</returns>
		protected override SdlDotNet.Particles.Particle.BaseParticle CreateParticle()
		{
			ParticleCircle p = new ParticleCircle();
			p.Color = Color.FromArgb(
				Random.Next(this.MinR, this.MaxR),
				Random.Next(this.MinG, this.MaxG),
				Random.Next(this.MinB, this.MaxB));
			p.Radius = (short)Random.Next((int)m_RadiusMin, (int)m_RadiusMax);
			return p;
		}
	}
}
