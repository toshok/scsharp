/*
 * $RCSfile$
 * Copyright (C) 2005 David Hudson (jendave@yahoo.com)
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
using Tao.Sdl;

namespace SdlDotNet
{
	/// <summary>
	/// The Tranformation class holds variables for rotating and zooming a Surface
	/// </summary>
	public class Transformation
	{
		double scaleX = 1.0;
		double scaleY = 1.0;
		double zoom = 1.0;
		bool antiAlias = true;
		int degreesOfRotation;

		/// <summary>
		/// Basic constructor
		/// </summary>
		/// <param name="scaleX">X-axis scaling factor</param>
		/// <param name="scaleY">Y-axis scaling factor</param>
		/// <param name="zoom">scaling in both X and Y axes</param>
		/// <param name="antiAlias">Antialias</param>
		/// <param name="degreesOfRotation">Rotate surface by given degrees</param>
		public Transformation(double scaleX, double scaleY, double zoom, bool antiAlias, int degreesOfRotation)
		{
			this.scaleX = scaleX;
			this.scaleY = scaleY;
			this.zoom = zoom;
			this.antiAlias = antiAlias;
			this.degreesOfRotation = degreesOfRotation;
		}

		/// <summary>
		/// Transform with Anti-aliasing is on
		/// </summary>
		/// <param name="scaleX">X-axis scaling factor
		/// </param>
		/// <param name="scaleY">Y-axis scaling factor
		/// </param>
		/// <param name="zoom">scaling in both X and Y axes
		/// </param>
		/// <param name="degreesOfRotation">Rotate surface by given degrees
		/// </param>
		public Transformation(double scaleX, double scaleY, double zoom, int degreesOfRotation) : this(scaleX, scaleY, zoom, true, degreesOfRotation)
		{
		}

		/// <summary>
		/// Transform with Anti-aliasing on and zoom off.
		/// </summary>
		/// <param name="scaleX">X-axis scaling factor
		/// </param>
		/// <param name="scaleY">Y-axis scaling factor
		/// </param>
		/// <param name="degreesOfRotation">
		/// Rotate surface by given degrees
		/// </param>
		public Transformation(double scaleX, double scaleY, int degreesOfRotation) : this(scaleX, scaleY, 1.0, true, degreesOfRotation)
		{
		}

		/// <summary>
		/// Transform with Anti-aliasing on and scaling off.
		/// </summary>
		/// <param name="zoom">Zoom factor</param>
		/// <param name="degreesOfRotation">Degrees to rotate surface</param>
		public Transformation(double zoom, int degreesOfRotation) : this(1.0, 1.0, zoom, true, degreesOfRotation)
		{
		}

		/// <summary>
		/// Rotate surface
		/// </summary>
		/// <param name="degreesOfRotation">Degrees to rotate surface</param>
		public Transformation(int degreesOfRotation) : this(1.0, 1.0, 1.0, true, degreesOfRotation)
		{
		}

		/// <summary>
		/// Stretch surface in X-axis by this amount
		/// </summary>
		public double ScaleX
		{
			get
			{
				return scaleX;
			}
			set
			{
				scaleX = value;
			}
		}

		/// <summary>
		/// Stretch surface in Y-axis by this amount
		/// </summary>
		public double ScaleY
		{
			get
			{
				return scaleY;
			}
			set
			{
				scaleY = value;
			}
		}

		/// <summary>
		/// Stretch surface in X-axis and Y-axis by this amount
		/// </summary>
		public double Zoom
		{
			get
			{
				return zoom;
			}
			set
			{
				this.zoom = value;
			}
		}

		/// <summary>
		/// Rotate surface by given degrees.
		/// </summary>
		public int DegreesOfRotation
		{
			get
			{
				return degreesOfRotation;
			}
			set
			{
				degreesOfRotation = value;
			}
		}

		/// <summary>
		/// Use antialiasing on the Surface.
		/// </summary>
		public bool AntiAlias
		{
			get
			{
				return antiAlias;
			}
			set
			{
				antiAlias = value;
			}
		}
	}
}
