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
//using SdlDotNet

namespace SdlDotNet.Particles.Manipulators
{
	/// <summary>
	/// A particle manipulator the keeps particles within a boundary.
	/// </summary>
	/// <example>
	/// The following example will keep all particles in the particleSystem in a rectangle of 0,0,100,100.
	/// <code>
	/// ParticleBoundry bounds = new ParticleBoundry(0,0,100,100);
	/// particleSystem.Add(bounds);
	/// </code></example>
	public class ParticleBoundary : IParticleManipulator
	{
		/// <summary>
		/// Create a ParticleBoundary with an empty boundary.
		/// </summary>
		public ParticleBoundary()
		{
			m_Boundary = new RectangleF(0,0,0,0);
		}
		/// <summary>
		/// Create a ParticleBoundary from a given size.
		/// </summary>
		/// <param name="size"></param>
		public ParticleBoundary(SizeF size)
		{
			m_Boundary = new RectangleF(0,0,size.Width,size.Height);
		}
		/// <summary>
		/// Create a ParticleBoundary from a given size.
		/// </summary>
		/// <param name="size">The width and height of the boundary.</param>
		public ParticleBoundary(Size size)
		{
			m_Boundary = new RectangleF(0,0,size.Width,size.Height);
		}
		/// <summary>
		/// Create a ParticleBoundary in the given rectangle boundary.
		/// </summary>
		/// <param name="rect">The rectangle representing the boundary.</param>
		public ParticleBoundary(Rectangle rect)
		{
			m_Boundary = rect;
		}
		/// <summary>
		/// Create a ParticleBoundary from a given rectangle boundary.
		/// </summary>
		/// <param name="rect">The rectangle representing the boundary.</param>
		public ParticleBoundary(RectangleF rect)
		{
			m_Boundary = rect;
		}
		/// <summary>
		/// Create a ParticleBoundary from a given bounds.
		/// </summary>
		/// <param name="positionX">The x-coordinate of the upper-left corner of the rectangle.</param>
		/// <param name="positionY">The y-coordinate of the upper-left corner of the rectangle.</param>
		/// <param name="width">The width of the boundary.</param>
		/// <param name="height">The height of the boundary.</param>
		public ParticleBoundary(float positionX, float positionY, float width, float height)
		{
			m_Boundary = new RectangleF(positionX, positionY,width,height);
		}
		/// <summary>
		/// Create a ParticleBoundary from a given size.
		/// </summary>
		/// <param name="width">The width of the boundary.</param>
		/// <param name="height">The height of the boundary.</param>
		public ParticleBoundary(float width, float height) : this(0,0,width,height)
		{
		}

		private RectangleF m_Boundary;
		/// <summary>
		/// Gets and sets the boundary rectangle.
		/// </summary>
		public RectangleF Boundary
		{
			get
			{
				return m_Boundary;
			}
			set
			{
				m_Boundary = value;
			}
		}

		/// <summary>
		/// Gets and set the x-coordinate 
		/// of the upper-left corner of the rectangle.
		/// </summary>
		public float X
		{
			get
			{
				return m_Boundary.X;
			}
			set
			{
				m_Boundary.X = value;
			}
		}

		/// <summary>
		/// Gets and sets the y-coordinate of 
		/// the upper-left corner of the rectangle.
		/// </summary>
		public float Y
		{
			get
			{
				return m_Boundary.Y;
			}
			set
			{
				m_Boundary.Y = value;
			}
		}

		/// <summary>
		/// Gets the x-coordinate of the left edge of the boundry.
		/// </summary>
		public float Left
		{
			get
			{
				return m_Boundary.Left;
			}
		}

		/// <summary>
		/// Gets the x-coordinate of the right edge of the boundry.
		/// </summary>
		public float Right
		{
			get
			{
				return m_Boundary.Right;
			}
		}

		/// <summary>
		/// Gets the y-coordinate of the top edge of the boundry.
		/// </summary>
		public float Top
		{
			get
			{
				return m_Boundary.Top;
			}
		}

		/// <summary>
		/// Gets the y-coordinate of the bottom edge of the boundry.
		/// </summary>
		public float Bottom
		{
			get
			{
				return m_Boundary.Bottom;
			}
		}

		/// <summary>
		/// Gets and sets the width of the boundry.
		/// </summary>
		public float Width
		{
			get
			{
				return m_Boundary.Width;
			}
			set
			{
				m_Boundary.Width = value;
			}
		}

		/// <summary>
		/// Gets and sets the height of the boundry.
		/// </summary>
		public float Height
		{
			get
			{
				return m_Boundary.Height;
			}
			set
			{
				m_Boundary.Height = value;
			}
		}

		/// <summary>
		/// Gets and sets the size of the boundry (width and height).
		/// </summary>
		public SizeF Size
		{
			get
			{
				return m_Boundary.Size;
			}
			set
			{
				m_Boundary.Size = value;
			}
		}

		/// <summary>
		/// Gets and sets the location of the boundry.
		/// </summary>
		public PointF Location
		{
			get
			{
				return m_Boundary.Location;
			}
			set
			{
				m_Boundary.Location = value;
			}
		}


		#region IParticleManipulator Members

		/// <summary>
		/// Makes sure that every particle is within the given boundary.
		/// </summary>
		/// <param name="particles">The particle collection to set inside the bounds.</param>
		/// <remarks>Particles that reach the outside the rectangle are bounced back into bounds.</remarks>
		public void Manipulate(ParticleCollection particles)
		{
			if (particles == null)
			{
				throw new ArgumentNullException("particles");
			}
			foreach(BaseParticle p in particles)
			{
				if(p.Left < this.Left)
				{
					p.X = this.Left;
					p.Velocity.X*=-1;
				}
				else if(p.Right > this.Right)
				{
					p.X = this.Right - p.Width;
					p.Velocity.X*=-1;
				}
				else if(p.Top < this.Top)
				{
					p.Y = this.Top;
					p.Velocity.Y*=-1;
				}
				else if(p.Bottom > this.Bottom)
				{
					p.Y = this.Bottom - p.Height;
					p.Velocity.Y*=-1;
				}
			}
		}

		#endregion


	}
}
