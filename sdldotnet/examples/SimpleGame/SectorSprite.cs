// Copyright 2005 David Hudson (jendave@yahoo.com)
// This file is part of SimpleGame.
//
// SimpleGame is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// SimpleGame is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SimpleGame; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections;
using System.Drawing;

using SdlDotNet;
using SdlDotNet.Sprites;

namespace SdlDotNet.Examples.SimpleGame
{
	/// <summary>
	/// Summary description for Sector.
	/// </summary>
	public class SectorSprite : Sprite
	{
		Sector sector;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="screen"></param>
		/// <param name="sector"></param>
		public SectorSprite(Surface screen, Sector sector) : base()
		{
			if (screen == null)
			{
				throw new ArgumentNullException("screen");
			}
			this.sector = sector;
			base.Surface = screen.CreateCompatibleSurface(128, 128);
			base.Surface.Fill(Color.FromArgb(0, 255, 128));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="screen"></param>
		/// <param name="sector"></param>
		/// <param name="rect"></param>
		public SectorSprite(Surface screen, Sector sector, Rectangle rect) :base()
		{
			if (screen == null)
			{
				throw new ArgumentNullException("screen");
			}
			this.sector = sector;
			base.Surface = screen.CreateCompatibleSurface(128, 128);
			base.Surface.Fill(Color.FromArgb(0, 255, 128));
			base.Rectangle = rect;
		}

		/// <summary>
		/// 
		/// </summary>
		public Sector Sector
		{
			get
			{
				return this.sector;
			}
		}

		#region IDisposable
		private bool disposed;

		/// <summary>
		/// Destroys the surface object and frees its memory
		/// </summary>
		/// <param name="disposing">If ture, dispose unmanaged resources</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.disposed)
				{
					if (disposing)
					{
					}
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		#endregion IDisposable
	}
}
