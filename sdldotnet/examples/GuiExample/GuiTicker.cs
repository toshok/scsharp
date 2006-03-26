/*
 * $RCSfile$
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
using System.Collections;
using System.Drawing;

namespace SdlDotNet.Examples.GuiExample
{
	/// <summary>
	/// A rather simple component, the ticker scrolls text left or right
	/// across the screen according to the given rate. It may have an
	/// option to hide itself if there is nothing in the ticker,
	/// otherwise it stays on the screen.
	/// </summary>
	public class GuiTicker : GuiComponent
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gui"></param>
		/// <param name="point"></param>
		/// <param name="height"></param>
		public GuiTicker(GuiManager gui, Point point, int height)
			: base(gui, new Rectangle(point.X, point.Y, Video.Screen.Width, height))
		{			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gui"></param>
		/// <param name="coordinates"></param>
		/// <param name="height"></param>
		public GuiTicker(GuiManager gui, Vector coordinates, int height)
			: base(gui, new Rectangle((int)coordinates.X , (int)coordinates.Y, Video.Screen.Width, height), 100)
		{
		}

		#region Sprites
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sprite"></param>
		public void Add(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			sprite.Position = new Point(this.Size.Width, this.Position.Y);
			base.Sprites.Add(sprite);
		}
		#endregion

		#region Events
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void Update(TickEventArgs args)
		{
			Sprite s;
			// Figure out the rates. 
			// The min and max start on opposite sides
			// of the ticker.
			//int offset = args.RatePerSecond(Delta);

			if (this.Sprites.Count != 0)
			{
				// Go through all the displayed sprites
				for (int i = 0; i < this.Sprites.Count; i++)
				{
					s = this.Sprites[i];
					// Tick the sprite and wrap it in a translator
					s.Update(args);
	  
					// Move the sprite along
					if (i > 1 && this.Sprites[i-1] != null && s.IntersectsWith(this.Sprites[i-1].Rectangle))
					{
					}
					else
					{
						s.X += 10;
					}
					s.Y = 0;
				}
			}
		}
		#endregion

		#region Properties
		private int delta = -10;
		//private int minSpace = 10;
		private bool isAutoHide;

		/// <summary>
		/// 
		/// </summary>
		public bool IsAutoHide
		{
			get 
			{ 
				return isAutoHide; 
			}
			set 
			{ 
				isAutoHide = value; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsHidden
		{
			get 
			{ 
				return (isAutoHide && this.Sprites.Count == 0); 
			}
		}

		/// <summary>
		/// Delta is the number of pixels that the ticker should move per
		/// second. This should be independant of actual frame rate.
		/// </summary>
		public int Delta
		{
			get 
			{ 
				return delta; 
			}
			set 
			{ 
				delta = value; 
			}
		}
		#endregion
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
	}
}
