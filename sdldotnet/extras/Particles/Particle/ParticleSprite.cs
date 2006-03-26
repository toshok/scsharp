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

using SdlDotNet.Sprites;


namespace SdlDotNet.Particles.Particle
{
	/// <summary>
	/// A particle represented by a Sprite.
	/// </summary>
	/// <remarks>Use ParticleSpriteEmitter to emit this particle.</remarks>
	public class ParticleSprite : BaseParticle
	{
		/// <summary>
		/// Creates a particle sprite based off an existing sprite.
		/// </summary>
		/// <param name="sprite">The sprite to represent this particle.</param>
		public ParticleSprite(Sprite sprite)
		{
			m_Sprite = sprite;
		}
		/// <summary>
		/// Create a particle sprite with a set velocity.
		/// </summary>
		/// <param name="sprite">The sprite to associate with this particle.</param>
		/// <param name="velocity">The velocity of the particle.</param>
		public ParticleSprite(Sprite sprite, Vector velocity)
		{
			this.Velocity = velocity;
			m_Sprite = sprite;
		}
		/// <summary>
		/// Create a particle sprite at a position.
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		public ParticleSprite(Sprite sprite, float positionX, float positionY)
		{
			this.X = positionX;
			this.Y = positionY;
			m_Sprite = sprite;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		/// <param name="velocity"></param>
		public ParticleSprite(Sprite sprite, float positionX, float positionY, Vector velocity)
		{
			this.X = positionX;
			this.Y = positionY;
			this.Velocity = velocity;
			m_Sprite = sprite;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="velocity"></param>
		/// <param name="life"></param>
		public ParticleSprite(Sprite sprite, Vector velocity, int life)
		{
			this.Velocity = velocity;
			m_Sprite = sprite;
			this.Life = life;
			this.LifeFull = life;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="life"></param>
		public ParticleSprite(Sprite sprite, int life)
		{
			m_Sprite = sprite;
			this.Life = life;
			this.LifeFull = life;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		/// <param name="velocity"></param>
		/// <param name="life"></param>
		public ParticleSprite(Sprite sprite, float positionX, float positionY, Vector velocity, int life)
		{
			this.X = positionX;
			this.Y = positionY;
			this.Velocity = velocity;
			m_Sprite = sprite;
			this.Life = life;
			this.LifeFull = life;
		}

		private Sprite m_Sprite;
		/// <summary>
		/// The sprite representing the particle on the destination surface.
		/// </summary>
		public Sprite Sprite
		{
			get
			{
				return m_Sprite;
			}
			set
			{
				m_Sprite = value;
			}
		}
		/// <summary>
		/// Renders the sprite on the destination surface.
		/// </summary>
		/// <param name="destination">The surface to render the sprite.</param>
		public override void Render(Surface destination)
		{
			m_Sprite.Position = new Point((int)this.X, (int)this.Y);
			m_Sprite.Render(destination);
		}

		/// <summary>
		/// Gets the height of the particle's sprite.
		/// </summary>
		public override float Height
		{
			get
			{
				return m_Sprite.Height;
			}
			set
			{
				m_Sprite.Height = (int)value;
			}
		}
		/// <summary>
		/// Gets the width of the particle's sprite.
		/// </summary>
		public override float Width
		{
			get
			{
				return m_Sprite.Width;
			}
			set
			{
				m_Sprite.Width = (int)value;
			}
		}


	}
}
