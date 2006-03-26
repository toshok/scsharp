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

namespace SdlDotNet.Particles.Emitters
{
	/// <summary>
	/// A particle emitter that emits particle rectangles.
	/// </summary>
	/// <example>
	/// <code>
	/// ParticleRectangleEmitter emitter = new ParticlePixelEmitter(new Size(10,10), new Size(50,50));
	/// particleSystem.Add(emitter);
	/// </code>
	/// </example>
	public class ParticleRectangleEmitter : ParticlePixelEmitter
	{
		/// <summary>
		/// Creates a new rectangle particle.
		/// </summary>
		public ParticleRectangleEmitter()
		{
		}
		/// <summary>
		/// Creates a new rectangle particle within a system.
		/// </summary>
		/// <param name="system">The parent system of the emitter.</param>
		public ParticleRectangleEmitter(ParticleSystem system) : base(system)
		{
		}

		/// <summary>
		/// Creates a new rectangle particle with a max size and min size.
		/// </summary>
		/// <param name="minSize">The minimum size of rectangle particles.</param>
		/// <param name="maxSize">The maximum size of rectangle particles.</param>
		public ParticleRectangleEmitter(SizeF minSize, SizeF maxSize)
		{
			this.MinSize = minSize;
			this.MaxSize = maxSize;
		}
		/// <summary>
		/// Creates a new rectangle particle within a system with a max and min size.
		/// </summary>
		/// <param name="system">The parent system of the emitter.</param>
		/// <param name="minSize">The minimum size of rectangle particles.</param>
		/// <param name="maxSize">The maximum size of rectangle particles.</param>
		public ParticleRectangleEmitter(ParticleSystem system, SizeF minSize, SizeF maxSize) : base(system)
		{
			this.MinSize = minSize;
			this.MaxSize = maxSize;
		}

		private SizeF m_MinSize = new SizeF(1,1);
		private SizeF m_MaxSize = new SizeF(10,10);
		/// <summary>
		/// Gets and sets the minimum height of particles.
		/// </summary>
		public float MinHeight
		{
			get
			{
				return m_MinSize.Height;
			}
			set
			{
				m_MinSize.Height = value;
			}
		}
		/// <summary>
		/// Gets and sets the maximum height of particles.
		/// </summary>
		public float MaxHeight
		{
			get
			{
				return m_MaxSize.Height;
			}
			set
			{
				m_MaxSize.Height = value;
			}
		}
		/// <summary>
		/// Gets and sets the maximum width of particles.
		/// </summary>
		public float MaxWidth
		{
			get
			{
				return m_MaxSize.Width;
			}
			set
			{
				m_MaxSize.Width = value;
			}
		}
		/// <summary>
		/// Gets and sets the minimum width of particles.
		/// </summary>
		public float MinWidth
		{
			get
			{
				return m_MinSize.Width;
			}
			set
			{
				m_MinSize.Width = value;
			}
		}
		/// <summary>
		/// Gets and sets the minimum size of particles.
		/// </summary>
		public SizeF MinSize
		{
			get
			{
				return m_MinSize;
			}
			set
			{
				m_MinSize = value;
			}
		}
		/// <summary>
		/// Gets and sets the maximum size of particles.
		/// </summary>
		public SizeF MaxSize
		{
			get
			{
				return m_MaxSize;
			}
			set
			{
				m_MaxSize = value;
			}
		}


		/// <summary>
		/// Creates a new ParticleRectangle with the set attributes.
		/// </summary>
		/// <returns>The new particle rectangle represented by a BaseParticle.</returns>
		protected override SdlDotNet.Particles.Particle.BaseParticle CreateParticle()
		{
			ParticleRectangle p = new ParticleRectangle();
			p.Color = Color.FromArgb(
				Random.Next(this.MinR, this.MaxR),
				Random.Next(this.MinG, this.MaxG),
				Random.Next(this.MinB, this.MaxB));
			p.Width = GetRange(m_MinSize.Width, m_MaxSize.Width);
			p.Height = GetRange(m_MinSize.Height, m_MaxSize.Height);
			return p;
		}

	}
}
