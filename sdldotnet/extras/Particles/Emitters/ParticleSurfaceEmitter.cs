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

namespace SdlDotNet.Particles.Emitters
{
	/// <summary>
	/// A particle emitter that emits surfaces from a surface collection to represent particles..
	/// </summary>
	public class ParticleSurfaceEmitter : ParticleEmitter
	{
		private SurfaceCollection m_Surfaces;
		/// <summary>
		/// Gets and sets the collection of surfaces assosiated with this particle emitter.
		/// </summary>
		public SurfaceCollection Surfaces
		{
			get
			{
				return m_Surfaces;
			}
//			set
//			{
//				m_Surfaces = value;
//			}
		}
		/// <summary>
		/// Creates a new particle emitter that emits surface objects.
		/// </summary>
		/// <param name="system">The particle system to add this particle emitter.</param>
		/// <param name="surface">The surface to emit.</param>
		public ParticleSurfaceEmitter(ParticleSystem system, Surface surface) : base(system)
		{
			m_Surfaces = new SurfaceCollection(surface);
		}
		/// <summary>
		/// Creates a new particle emitter that emits surface objects.
		/// </summary>
		/// <param name="system">The particle system to add this particle emitter.</param>
		/// <param name="surfaces">The surface collection to choose surfaces from when emitting.</param>
		public ParticleSurfaceEmitter(ParticleSystem system, SurfaceCollection surfaces) : base(system)
		{
			m_Surfaces = surfaces;
		}
		/// <summary>
		/// Creates a new particle emitter that emits surface objects.
		/// </summary>
		/// <param name="surface">The surface to emit.</param>
		public ParticleSurfaceEmitter(Surface surface)
		{
			m_Surfaces = new SurfaceCollection(surface);
		}
		/// <summary>
		/// Creates a new particle emitter that emits surface objects.
		/// </summary>
		/// <param name="surfaces">The surface collection to choose surfaces from when emitting.</param>
		public ParticleSurfaceEmitter(SurfaceCollection surfaces)
		{
			m_Surfaces = surfaces;
		}
		/// <summary>
		/// Creates a particle from a surface in the surface collection.
		/// </summary>
		/// <returns>The created particle.</returns>
		protected override BaseParticle CreateParticle()
		{
			if(m_Surfaces.Count == 0)
			{
				return null;
			}
			return new ParticleSurface(
				m_Surfaces[Random.Next(0,m_Surfaces.Count-1)]
				);
		}
	}
}
