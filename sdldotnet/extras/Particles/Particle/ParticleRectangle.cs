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
	/// Uses a rectangle to represent a particle on the destination surface.
	/// </summary>
	/// <remarks>Use ParticleRectangleEmitter to emit this kind of particle.</remarks>
	public class ParticleRectangle : ParticlePixel
	{
		/// <summary>
		/// Creates a new particle rectangle.
		/// </summary>
		public ParticleRectangle()
		{
		}
		/// <summary>
		/// Creates a new particle rectangle with a color.
		/// </summary>
		/// <param name="color">The color of the rectangle.</param>
		public ParticleRectangle(Color color)
		{
			base.Color = color;
		}
		/// <summary>
		/// Creates a new particle rectangle with a color and a size.
		/// </summary>
		/// <param name="size"></param>
		/// <param name="color"></param>
		public ParticleRectangle(SizeF size, Color color)
		{
			base.Color = color;
			this.m_Width = size.Width;
			this.m_Height = size.Height;
		}

		/// <summary>
		/// Creates a new particle rectangle with a size.
		/// </summary>
		/// <param name="size">The size of the rectangle.</param>
		public ParticleRectangle(SizeF size)
		{
			this.m_Width = size.Width;
			this.m_Height = size.Height;
		}
		/// <summary>
		/// Creates a new particle rectangle with a position and color.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="color"></param>
		public ParticleRectangle(PointF pos, Color color)
		{
			base.Color = color;
			base.X = pos.X;
			base.Y = pos.Y;
		}

		/// <summary>
		/// Creates a new particle rectangle with a position.
		/// </summary>
		/// <param name="pos"></param>
		public ParticleRectangle(PointF pos)
		{
			this.X = pos.X;
			this.Y = pos.Y;
		}


		/// <summary>
		/// Creates a new particle rectangle with a position, size and color.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="color"></param>
		public ParticleRectangle(RectangleF rect, Color color)
		{
			base.Color = color;
			base.X = rect.X;
			base.Y = rect.Y;
			this.m_Width = rect.Width;
			this.m_Height = rect.Height;
		}

		/// <summary>
		/// Creates a new particle rectangle with a position and size.
		/// </summary>
		/// <param name="rect"></param>
		public ParticleRectangle(RectangleF rect)
		{
			base.X = rect.X;
			base.Y = rect.Y;
			this.m_Width = rect.Width;
			this.m_Height = rect.Height;
		}

		private float m_Width = 10;
		/// <summary>
		/// Gets and sets the width of the rectangle.
		/// </summary>
		public override float Width
		{
			get
			{
				return m_Width;
			}
			set
			{
				m_Width = value;
			}
		}

		private float m_Height = 10;
		/// <summary>
		/// Gets and sets the height of the rectangle.
		/// </summary>
		public override float Height
		{
			get
			{
				return m_Height;
			}
			set
			{
				m_Height = value;
			}
		}

		private bool m_Filled = true;
		/// <summary>
		/// Gets and sets whether or not the rectangle is to be a filled rectangle when drawn on the destination surface.
		/// </summary>
		public bool Filled
		{
			get
			{
				return m_Filled;
			}
			set
			{
				m_Filled = value;
			}
		}


		/// <summary>
		/// Draws the particle on the destination surface.
		/// </summary>
		/// <param name="destination">The surface to draw the particle on.</param>
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

				if(m_Filled)
				{
					destination.DrawFilledBox(
						new System.Drawing.Rectangle(
						(int)this.X, (int)this.Y,
						(int)m_Width, (int)m_Height),
						Color.FromArgb((int)alpha,this.Color));
				}
				else
				{
					destination.DrawBox(
						new System.Drawing.Rectangle(
						(int)this.X, (int)this.Y,
						(int)m_Width, (int)m_Height),
						Color.FromArgb((int)alpha,this.Color));
				}
			}
			else
			{
				if(m_Filled)
				{
					destination.DrawFilledBox(
						new System.Drawing.Rectangle(
						(int)this.X, (int)this.Y,
						(int)m_Width, (int)m_Height),
						this.Color);
				}
				else
				{
					destination.DrawBox(
						new System.Drawing.Rectangle(
						(int)this.X, (int)this.Y,
						(int)m_Width, (int)m_Height),
						this.Color);
				}
			}
		}
	}
}
