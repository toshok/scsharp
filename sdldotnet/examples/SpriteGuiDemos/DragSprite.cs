/*
 * $RCSfile: DragSprite.cs,v $
 * Copyright (C) 2004 D. R. E. Moonfire (d.moonfire@mfgames.com)
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


using SdlDotNet.Sprites;
using SdlDotNet;
using System;
using System.Drawing;
using System.Collections;
using System.Globalization;

namespace SdlDotNet.Examples.SpriteGuiDemos
{
	/// <summary>
	/// 
	/// </summary>
	public class DragSprite : BoundedSprite
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="frames"></param>
		/// <param name="key"></param>
		/// <param name="coordinates"></param>
		/// <param name="bounds"></param>
		public DragSprite(Hashtable frames, string key, Point coordinates,
			Rectangle bounds)
			: base((SurfaceCollection)frames[key], bounds, coordinates)
		{
			this.Size = ((SurfaceCollection)frames[key]).Size;
			this.AllowDrag = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "(drag {0} {1})", this.BeingDragged, base.ToString());
		}

		#region Events
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void Update(MouseButtonEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (this.IntersectsWith(new Point(args.X, args.Y)))
			{
				// If we are being held down, pick up the marble
				// Change the Z-order
				if (args.ButtonPressed)
				{
					this.Z += 100;
					this.BeingDragged = true;
					this.CurrentAnimation = "marble2";
				}
				else
				{
					this.Z -= 100;
					this.BeingDragged = false;
					this.CurrentAnimation = "marble1";
				}
			}
		}

		/// <summary>
		/// If the sprite is picked up, this moved the sprite to follow
		/// the mouse.
		/// </summary>
		public override void Update(MouseMotionEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (!AllowDrag)
			{
				return;
			}

			// Move the window as appropriate
			if (this.BeingDragged)
			{
				this.X += args.RelativeX;
				this.Y += args.RelativeY;
			}
		}
		#endregion

		private bool disposed;

		/// <summary>
		/// Destroys the surface object and frees its memory
		/// </summary>
		/// <param name="disposing"></param>
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
	}
}
