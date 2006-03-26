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
using SdlDotNet.Sprites;

namespace SdlDotNet.Particles.Particle
{
	/// <summary>
	/// An abstract class describing a base particle.
	/// </summary>
	/// <remarks>Some implementations of the particle class include ParticlePixel and ParticleSprite.</remarks>
	/// <example>This is an example of implementing a new kind of particle.  It creates a ParticleBox that uses blue boxes to represent particles:
	/// <code>
	/// public class ParticleBox : BaseParticle
	/// {
	///		public override void Render(Surface destination)
	///		{
	///			destination.DrawBox(new Rectangle((int)this.X, (int)this.Y, 100,100), Color.Blue);
	///		}
	/// }
	/// </code>
	/// </example>
	public abstract class BaseParticle
	{
		private int m_Life = -1;
		/// <summary>
		/// The current life of the particle. -1 means infinate life.
		/// </summary>
		/// <remarks>This is decreased when the Update method is called.</remarks>
		public int Life
		{
			get
			{
				return m_Life;
				//Surface a = new Surface();
			}
			set
			{
				m_Life = value;
			}
		}

		private int m_LifeFull = 100;
		/// <summary>
		/// Gets and sets the value representing the full life of the particle.
		/// </summary>
		/// <remarks>This is usually used when distinguishing when the particle should start dying out with alpha transparency.</remarks>
		public int LifeFull
		{
			get
			{
				return m_LifeFull;
			}
			set
			{
				m_LifeFull = value;
			}
		}

		private bool m_Static;
		/// <summary>
		/// Gets and sets whether the particle's velocity will change its location.
		/// </summary>
		/// <remarks>If set to true, the particle will not move.  If set to false, the particle will move with manipulators.</remarks>
		public bool Static
		{
			get
			{
				return m_Static;
			}
			set
			{
				m_Static = value;
			}
		}

		private float m_X;
		/// <summary>
		/// The X coordinate of the particle.
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
		private float m_Y;
		/// <summary>
		/// The Y coordinate of the particle.
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

		private Vector m_Velocity = new Vector(0,0);
		/// <summary>
		/// The speed and direction the particle is going.
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
		/// Draws the particle onto the destination.
		/// </summary>
		/// <param name="destination">The destination surface of the particle.</param>
		public abstract void Render(Surface destination);

		/// <summary>
		/// Updates the location and life of the particle.
		/// </summary>
		/// <returns>True if the particle is still alive, false if the particle is to be destroyed.</returns>
		public virtual bool Update()
		{
			if(m_Life == 0)
			{
				return false;
			}
			if(!m_Static)
			{
				m_X += m_Velocity.X;
				m_Y += m_Velocity.Y;
			}
			if(m_Life != -1) // -1 is alife forever.
			{
				if(--m_Life == 0)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Gets and sets the width of the particle.
		/// </summary>
		public abstract float Width
		{
			get;
			set;
		}

		/// <summary>
		/// Gets and sets the height of the particle.
		/// </summary>
		public abstract float Height
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the y-coordinate of the bottom edge of the particle.
		/// </summary>
		public virtual float Bottom
		{
			get
			{
				return m_Y + Height;
			}
		}

		/// <summary>
		/// Gets the x-coordinate of the right edge of the particle.
		/// </summary>
		public virtual float Right
		{
			get
			{
				return m_X + Width;
			}
		}

		/// <summary>
		/// Gets the x-coordinate of the left edge of the particle.
		/// </summary>
		public virtual float Left
		{
			get
			{
				return m_X;
			}
		}

		/// <summary>
		/// Gets the y-coordinate of the top edge of the particle.
		/// </summary>
		public virtual float Top
		{
			get
			{
				return m_Y;
			}
		}
	}
}
