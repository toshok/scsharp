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

using SdlDotNet;

namespace SdlDotNet.Particles.Particle
{
	/// <summary>
	/// A particle represented by a surface.
	/// </summary>
	public class ParticleSurface : BaseParticle
	{
		private Surface m_Surface;
		/// <summary>
		/// Gets and sets the surface used to represent the particle.
		/// </summary>
		public Surface Surface
		{
			get
			{
				return m_Surface;
			}
			set
			{
				m_Surface = value;
			}
		}

		private Rectangle m_ClipRectangle;
		/// <summary>
		/// Gets and sets the clipping rectangle of the surface.
		/// </summary>
		public Rectangle ClipRectangle
		{
			get
			{
				return m_ClipRectangle;
			}
			set
			{
				m_ClipRectangle = value;
			}
		}

		/// <summary>
		/// Creates a particle surface.
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		/// <param name="velocity"></param>
		/// <param name="life"></param>
		public ParticleSurface(Surface surface, float positionX, float positionY, Vector velocity, int life)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			m_Surface = surface;
			this.X = positionX;
			this.Y = positionY;
			this.Velocity = velocity;
			this.Life = life;
			this.LifeFull = life;
			m_ClipRectangle = new Rectangle(0, 0, m_Surface.Width, m_Surface.Height);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		/// <param name="velocity"></param>
		public ParticleSurface(Surface surface, float positionX, float positionY, Vector velocity)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			m_Surface = surface;
			this.X = positionX;
			this.Y = positionY;
			this.Velocity = velocity;
			m_ClipRectangle = new Rectangle(0, 0, m_Surface.Width, m_Surface.Height);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		public ParticleSurface(Surface surface, float positionX, float positionY)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			m_Surface = surface;
			this.X = positionX;
			this.Y = positionY;
			m_ClipRectangle = new Rectangle(0, 0, m_Surface.Width, m_Surface.Height);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		/// <param name="life"></param>
		public ParticleSurface(Surface surface, float positionX, float positionY, int life)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			m_Surface = surface;
			this.X = positionX;
			this.Y = positionY;
			this.Life = life;
			m_ClipRectangle = new Rectangle(0, 0, m_Surface.Width, m_Surface.Height);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		public ParticleSurface(Surface surface)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			m_Surface = surface;
			m_ClipRectangle = new Rectangle(0, 0, m_Surface.Width, m_Surface.Height);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="clip"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		/// <param name="velocity"></param>
		/// <param name="life"></param>
		public ParticleSurface(Surface surface, Rectangle clip, float positionX, float positionY, Vector velocity, int life)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			m_ClipRectangle = clip;
			m_Surface = surface;
			this.X = positionX;
			this.Y = positionY;
			this.Velocity = velocity;
			this.Life = life;
			this.LifeFull = life;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="clip"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		/// <param name="velocity"></param>
		public ParticleSurface(Surface surface, Rectangle clip, float positionX, float positionY, Vector velocity)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			m_ClipRectangle = clip;
			m_Surface = surface;
			this.X = positionX;
			this.Y = positionY;
			this.Velocity = velocity;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="clip"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		public ParticleSurface(Surface surface, Rectangle clip, float positionX, float positionY)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			m_ClipRectangle = clip;
			m_Surface = surface;
			this.X = positionX;
			this.Y = positionY;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="clip"></param>
		/// <param name="positionX"></param>
		/// <param name="positionY"></param>
		/// <param name="life"></param>
		public ParticleSurface(Surface surface, Rectangle clip, float positionX, float positionY, int life)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			m_ClipRectangle = clip;
			m_Surface = surface;
			this.X = positionX;
			this.Y = positionY;
			this.Life = life;
			this.LifeFull = life;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="clip"></param>
		public ParticleSurface(Surface surface, Rectangle clip)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			m_ClipRectangle = clip;
			m_Surface = surface;
		}

		/// <summary>
		/// Renders the surface as the particle.
		/// </summary>
		/// <param name="destination">The surface to blit the particle.</param>
		public override void Render(Surface destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			destination.Blit(m_Surface, new Point((int)this.X, (int)this.Y), m_ClipRectangle);
		}

		/// <summary>
		/// Gets the width of the particle's surface.
		/// </summary>
		public override float Width
		{
			get
			{
				return m_Surface.Width;
			}
			set
			{
			}
		}
		/// <summary>
		/// Gets the height of the particle's surface.
		/// </summary>
		public override float Height
		{
			get
			{
				return m_Surface.Height;
			}
			set
			{
			}
		}



	}
}
