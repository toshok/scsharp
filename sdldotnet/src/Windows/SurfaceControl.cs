#region License
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
#endregion License

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SdlDotNet;

namespace SdlDotNet.Windows 
{
	#region Class Documentation
	/// <summary>
	///     Provides a simple Sdl Surface control allowing 
	///     quick development of Windows Forms-based
	///     Sdl Surface applications.
	/// </summary>
	#endregion Class Documentation
	[DefaultProperty("Image")]
	[ToolboxBitmap(typeof(Bitmap),"SurfaceControl.bmp")]
	public class SurfaceControl : System.Windows.Forms.PictureBox
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SurfaceControl()
		{
		}

		/// <summary>
		/// Copies surface to this surface
		/// </summary>
		/// <param name="surface">surface to copy onto control</param>
		public void Blit(Surface surface)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			this.Image = surface.Bitmap;
		}
		
		/// <summary>
		/// Raises the OnResize event
		/// </summary>
		/// <param name="e">Contains the event data</param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
            if (!this.DesignMode)
            {
                SdlDotNet.Events.Add(new VideoResizeEventArgs(this.Width, this.Height));
            }
		}

//		protected override void OnPaint(PaintEventArgs e)
//		{
//			e.Graphics.DrawImage(this.Image,new Point(0,0));
//			//base.OnPaint (e);
//		}

		/// <summary>
		/// Raises the SizeChanged event
		/// </summary>
		/// <param name="e">Contains the event data</param>
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged (e);
            if (!this.DesignMode)
            {
                SdlDotNet.Events.Add(new VideoResizeEventArgs(this.Width, this.Height));
            }
		}
		
		/// <summary>
		/// Raises the MouseDown event
		/// </summary>
		/// <param name="e">Contains the event data</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);
            if (!this.DesignMode)
            {
                SdlDotNet.Events.Add(new MouseButtonEventArgs(SurfaceControl.ConvertMouseButtons(e), true, (short)e.X, (short)e.Y));
            }
		}

		/// <summary>
		/// Raises the MouseUp event
		/// </summary>
		/// <param name="e">Contains the event data</param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
            if (!this.DesignMode)
            {
                SdlDotNet.Events.Add(new MouseButtonEventArgs(SurfaceControl.ConvertMouseButtons(e), false, (short)e.X, (short)e.Y));
            }
		}
		
		int lastX;
		int lastY;

		/// <summary>
		/// Raises the MouseMove event
		/// </summary>
		/// <param name="e">Contains the event data</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove (e);

            if (!this.DesignMode)
            {
                if (e.Button != MouseButtons.None)
                {
                    SdlDotNet.Events.Add(new MouseMotionEventArgs(true, SurfaceControl.ConvertMouseButtons(e), (short)e.X, (short)e.Y, (short)(e.X - lastX), (short)(e.Y - lastY)));
                }
                lastX = e.X;
                lastY = e.Y;
            }
		}

		private static MouseButton ConvertMouseButtons(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				return MouseButton.PrimaryButton;
			}
			else if (e.Button == MouseButtons.Right)
			{
				return MouseButton.SecondaryButton;
			}
			else if (e.Button == MouseButtons.Middle)
			{
				return MouseButton.MiddleButton;
			}
			else
			{
				return MouseButton.None;
			}			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public void KeyPressed(KeyEventArgs e)
		{
			this.OnKeyDown(e);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public void KeyReleased(KeyEventArgs e)
		{
			this.OnKeyUp(e);
		}

		/// <summary>
		/// Raises the 
		/// <see cref="E:System.Windows.Forms.Control.KeyDown"/> event.
		/// </summary>
		/// <param name="e">A 
		/// <see cref="T:System.Windows.Forms.KeyEventArgs"/> 
		/// that contains the event data.</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown (e);
            if (!this.DesignMode)
            {
                SdlDotNet.Events.Add(new KeyboardEventArgs((SdlDotNet.Key)Enum.Parse(typeof(SdlDotNet.Key), e.KeyCode.ToString()), (ModifierKeys)e.Modifiers, true));
            }
		}

		/// <summary>
		/// Raises the 
		/// <see cref="E:System.Windows.Forms.Control.KeyUp"/> 
		/// event.
		/// </summary>
		/// <param name="e">A 
		/// <see cref="T:System.Windows.Forms.KeyEventArgs"/> 
		/// that contains the event data.</param>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp (e);
            if (!this.DesignMode)
            {
                SdlDotNet.Events.Add(new KeyboardEventArgs((SdlDotNet.Key)Enum.Parse(typeof(SdlDotNet.Key), e.KeyCode.ToString()), (ModifierKeys)e.Modifiers, false));
            }
		}

		#region Disposing
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
		#endregion Disposing
	}
}
