using System;

namespace SdlDotNet.Particles.Particle
{
	/// <summary>
	/// A particle represented by a circle.
	/// </summary>
	/// <remarks>Use ParticleCircleEmitter to emit this particle.</remarks>
	public class ParticleCircle : ParticlePixel
	{
		/// <summary>
		/// Creates a particle represented by a circle with the default values.
		/// </summary>
		public ParticleCircle()
		{
		}
		/// <summary>
		/// Creates a particle represented by a circle with a set radius.
		/// </summary>
		/// <param name="radius"></param>
		public ParticleCircle(short radius)
		{
			m_Radius = radius;
		}
		private short m_Radius = 1;
		/// <summary>
		/// Gets and sets the radius of the particles.
		/// </summary>
		public short Radius
		{
			get
			{
				return m_Radius;
			}
			set
			{
				m_Radius = value;
			}
		}

		/// <summary>
		/// Gets and sets the height of the circle.
		/// </summary>
		public override float Height
		{
			get
			{
				return m_Radius * 2;
			}
			set
			{
				m_Radius = (short)(value / 2);
			}
		}

		/// <summary>
		/// Gets the y-coordinate of the bottom edge of the circle.
		/// </summary>
		public override float Bottom
		{
			get
			{
				return this.Y + m_Radius;
			}
		}

		/// <summary>
		/// Gets the x-coordinate of the left edge of the circle.
		/// </summary>
		public override float Left
		{
			get
			{
				return this.X - m_Radius;
			}
		}

		/// <summary>
		/// Gets the x-coordinate of the right edge of the circle.
		/// </summary>
		public override float Right
		{
			get
			{
				return this.X + m_Radius;
			}
		}

		/// <summary>
		/// Gets the y-coordinate of the top edge of the circle.
		/// </summary>
		public override float Top
		{
			get
			{
				return this.Y - m_Radius;
			}
		}

		/// <summary>
		/// Gets and sets the width of the circle.
		/// </summary>
		public override float Width
		{
			get
			{
				return m_Radius * 2;
			}
			set
			{
				m_Radius = (short)(value / 2);
			}
		}		

		/// <summary>
		/// Draws the particle on the destination surface represented by a circle.
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

				destination.DrawFilledCircle(
					new Circle((short)this.X, (short)this.Y, m_Radius), 
					System.Drawing.Color.FromArgb((int)alpha, this.Color));
			}
			else
			{
				destination.DrawFilledCircle(
					new Circle((short)this.X, (short)this.Y, m_Radius), 
					this.Color);
			}
		}
	}
}
