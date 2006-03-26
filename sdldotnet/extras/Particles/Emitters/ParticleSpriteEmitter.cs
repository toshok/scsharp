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
using SdlDotNet.Sprites;
using SdlDotNet.Particles.Particle;

namespace SdlDotNet.Particles.Emitters
{
	/// <summary>
	/// A particle emitter that shoots out random sprites from a sprite collection.
	/// </summary>
	public class ParticleSpriteEmitter : ParticleEmitter
	{
		private SpriteCollection m_Sprites;
		/// <summary>
		/// Gets and sets the collection of sprites assosiated with this particle emitter.
		/// </summary>
		public SpriteCollection Sprites
		{
			get
			{
				return m_Sprites;
			}
//			set
//			{
//				m_Sprites = value;
//			}
		}
		/// <summary>
		/// Creates a new particle emitter that emits sprite objects.
		/// </summary>
		/// <param name="system">The particle system to add this particle emitter.</param>
		/// <param name="sprite">The sprite to emit.</param>
		public ParticleSpriteEmitter(ParticleSystem system, Sprite sprite) : base(system)
		{
			m_Sprites = new SpriteCollection(sprite);
		}
		/// <summary>
		/// Creates a new particle emitter that emits sprite objects.
		/// </summary>
		/// <param name="system">The particle system to add this particle emitter.</param>
		/// <param name="sprites">The sprite collection to choose sprites from when emitting.</param>
		public ParticleSpriteEmitter(ParticleSystem system, SpriteCollection sprites) : base(system)
		{
			m_Sprites = sprites;
		}
		/// <summary>
		/// Creates a new particle emitter that emits sprite objects.
		/// </summary>
		/// <param name="sprite">The sprite to emit.</param>
		public ParticleSpriteEmitter(Sprite sprite)
		{
			m_Sprites = new SpriteCollection(sprite);
		}
		/// <summary>
		/// Creates a new particle emitter that emits sprite objects.
		/// </summary>
		/// <param name="sprites">The sprite collection to choose sprites from when emitting.</param>
		public ParticleSpriteEmitter(SpriteCollection sprites)
		{
			m_Sprites = sprites;
		}
		/// <summary>
		/// Creates a particle based on the sprite parameters.
		/// </summary>
		/// <returns>The created particle.</returns>
		protected override BaseParticle CreateParticle()
		{
			if(m_Sprites.Count == 0)
			{
				return null;
			}
			return new ParticleSprite(
					m_Sprites[Random.Next(0,m_Sprites.Count-1)]
				);
		}
	}
}
