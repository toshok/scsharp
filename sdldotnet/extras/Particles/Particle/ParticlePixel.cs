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

namespace SdlDotNet.Particles.Particle
{
	/// <summary>
	/// A particle represented by a pixel on the destination surface.
	/// </summary>
	/// <remarks>Use ParticlePixelEmitter to emit this particle.</remarks>
	public class ParticlePixel : BaseParticle
	{
		/// <summary>
		/// Creates a new ParticlePixel.
		/// </summary>
		public ParticlePixel()
		{
		}
		/// <summary>
		/// Creates a new ParticlePixel.
		/// </summary>
		/// <param name="positionX">The X coordinate.</param>
		/// <param name="positionY">The Y coordinate.</param>
		/// <param name="color">The color of the pixel on the destination surface.</param>
		public ParticlePixel(Color color, float positionX, float positionY)
		{
			this.X = positionX;
			this.Y = positionY;
			m_Color = color;
		}
		/// <summary>
		/// Creates a new ParticlePixel.
		/// </summary>
		/// <param name="positionX">The X coordinate.</param>
		/// <param name="positionY">The Y coordinate.</param>
		/// <param name="velocity">The speed and direction of the particle.</param>
		public ParticlePixel(float positionX, float positionY, Vector velocity)
		{
			this.Velocity = velocity;
			this.X = positionX;
			this.Y = positionY;
		}
		/// <summary>
		/// Creates a new ParticlePixel.
		/// </summary>
		/// <param name="color">The color of the pixel on the destination surface.</param>
		/// <param name="positionX">The X coordinate.</param>
		/// <param name="positionY">The Y coordinate.</param>
		/// <param name="life">How long the particle is to stay alive.</param>
		public ParticlePixel(Color color, float positionX, float positionY, int life)
		{
			this.X = positionX;
			this.Y = positionY;
			m_Color = color;
			Life = life;
			LifeFull = life;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		/// <param name="velocity"></param>
		/// <param name="life"></param>
		public ParticlePixel(Color color, float positionX, float positionY, Vector velocity, int life)
		{
			this.Velocity = velocity;
			this.X = positionX;
			this.Y = positionY;
			m_Color = color;
			Life = life;
			LifeFull = life;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		/// <param name="velocity"></param>
		public ParticlePixel(Color color, float positionX, float positionY, Vector velocity)
		{
			this.Velocity = velocity;
			this.X = positionX;
			this.Y = positionY;
			m_Color = color;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		/// <param name="velocity"></param>
		public ParticlePixel(Color color, Vector velocity)
		{
			this.Velocity = velocity;
			m_Color = color;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		public ParticlePixel(float positionX, float positionY)
		{
			this.X = positionX;
			this.Y = positionY;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		public ParticlePixel(Color color)
		{
			m_Color = color;
		}
		private Color m_Color = Color.Black;
		/// <summary>
		/// The color of the particle pixel when drawn on the destination surface.
		/// </summary>
		public Color Color
		{
			get
			{
				return m_Color;
			}
			set
			{
				m_Color = value;
			}
		}

		/// <summary>
		/// Draws the particle on the destination surface represented by a pixel.
		/// </summary>
		/// <param name="destination">The destination surface where to draw the particle.</param>
		public override void Render(Surface destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if(this.Life != -1)
			{
				float alpha;
				if(this.Life >= this.LifeFull)
					alpha = 255;
				else if (this.Life <= 0)
					alpha = 0;
				else
					alpha = ((float)this.Life / this.LifeFull) * 255F;

				destination.DrawPixel((int)this.X,(int)this.Y, m_Color, (int)alpha);
			}
			else
			{
				destination.DrawPixel((int)this.X,(int)this.Y, m_Color, true);
			}
		}

		/// <summary>
		/// Gets the height of the particle.
		/// </summary>
		public override float Height
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets the width of the particle.
		/// </summary>
		public override float Width
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}


	}
}
