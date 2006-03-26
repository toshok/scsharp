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
using System.Drawing;

namespace SdlDotNet.Examples.GuiExample
{
	/// <summary>
	/// This class manages and controls the various GUI elements inside
	/// the SDL GUI system. It controls both the appearance (theme) of
	/// the control and also some basic functionality. Almost all of the
	/// GUI elements take this as a constructor. This class is intended
	/// to be extended, the default implement uses just rectangles and
	/// no graphics for its rendering.
	/// </summary>
	public class GuiManager
	{
		/// <summary>
		/// Constructs the GUI manager with the minimum required to keep
		/// the entire system running properly. The sprite manager is used
		/// to control the actual window elements while the baseFont is
		/// used for any requests for fonts. Specific fonts may assigned,
		/// the base system will always fall back to the baseFont.
		/// </summary>
		public GuiManager(SpriteCollection spriteManager, SdlDotNet.Font baseFont,
			Size size)
		{
			this.spriteManager = spriteManager;
			this.baseFont = baseFont;
			this.size = size;
		}

		#region Fonts
		/// <summary>
		/// Contains the fall-back font for the system
		/// </summary>
		private SdlDotNet.Font baseFont;

		// Contains the font for any window titles
		private SdlDotNet.Font titleFont;

		// Contains the font for menus
		private SdlDotNet.Font menuFont;

		/// <summary>
		/// 
		/// </summary>
		public SdlDotNet.Font BaseFont
		{
			get 
			{ 
				return this.baseFont; 
			}
			set
			{
				if (value == null)
				{
					throw new GuiException("Cannot assign a null font to the GUI");
				}

				this.baseFont = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public SdlDotNet.Font MenuFont
		{
			get
			{
				if (this.menuFont == null)
				{
					return this.baseFont;
				}
				else
				{
					return menuFont; 
				}
			}
			set 
			{ 
				menuFont = value; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public SdlDotNet.Font TitleFont
		{
			get
			{
				if (titleFont == null)
				{
					return baseFont;
				}
				else
				{
					return titleFont; 
				}
			}
			set 
			{ 
				titleFont = value; 
			}
		}

		/// <summary>
		/// Renders a given text with the given font and returns the size
		/// of the surface rendered.
		/// </summary>
		public static Size GetTextSize(SdlDotNet.Font font, string textItem)
		{
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}
			// Render the text
			Surface ts = font.Render(textItem, Color.FromArgb(255, 255, 255));
      
			return new Size(ts.Width, ts.Height);
		}
		#endregion

		#region Components
		/// <summary>
		/// 
		/// </summary>
		public static Padding TickerPadding
		{
			get 
			{ 
				return new Padding(2); 
			}
		}

//		/// <summary>
//		/// 
//		/// </summary>
//		public void Render(Surface surface, GuiTicker ticker)
//		{
//			// Draw a frame
//			surface.Fill(ticker.Rectangle, backgroundColor);
//			DrawRect(surface, ticker.Rectangle, frameColor);
//		}
		#endregion

		#region Menus
		/// <summary>
		/// 
		/// </summary>
		public static Padding MenuBarPadding
		{
			get 
			{ 
				return new Padding(10, 2); 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static Padding MenuItemPadding
		{
			get 
			{ 
				return new Padding(10, 2); 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static Padding MenuItemInnerPadding
		{
			get 
			{ 
				return new Padding(0); 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static Padding MenuPopupPadding
		{
			get 
			{ 
				return new Padding(2, 2, 1, 1); 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static Padding MenuSpacerPadding
		{
			get 
			{ 
				return new Padding(10, 1, 10, 2);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static Padding MenuTitlePadding
		{
			get 
			{ 
				return new Padding(10, 2); 
			}
		}
		#endregion

		#region Windows
		private int windowPad = 1;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="window"></param>
		/// <returns></returns>
		public Padding GetPadding(GuiWindow window)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window");
			}
			// Create a new padding
			Padding pad = new Padding(windowPad);

			// Check for title
			if (window.Title != null)
			{
				pad.Top += TitleFont.Height + windowPad;
			}

			return pad;
		}
		#endregion

		#region Drawing Routines
		/// <summary>
		/// This is one of the core drawing functions to draw a rectangle
		/// in a specified color on the given args.Surface.
		/// </summary>
		public static void DrawRect(Surface surface, Rectangle bounds, Color color)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			// FIX: Very sloppy code.

			// Ignore blanks
			if (surface.Width == 0 || surface.Height == 0)
			{
				return;
			}

			// Draw the lines
			int l = bounds.Left;
			int r = bounds.Right;
			int t = bounds.Top;
			int b = bounds.Bottom;

			// Draw the pixels
			for (int i = l; i <= r; i++)
			{
				if (i >= 0 && i <= surface.Width)
				{
					try 
					{
						if (t >= 0)
						{
							surface.DrawPixel(i, t, color);
						}
					} 
					catch (GuiException e)
					{ 
						Console.WriteLine(e);
						//throw;
					}
	  
					try 
					{
						if (b <= surface.Height)
						{
							surface.DrawPixel(i, b, color);
						}
					} 
					catch (GuiException e)
					{ 
						Console.WriteLine(e);
						//throw;
					}
				}
			}

			for (int i = t; i < b; i++)
			{
				if (i >= 0 && i <= surface.Height)
				{
					try 
					{
						if (l >= 0)
						{
							surface.DrawPixel(l, i, color);
						}
					} 
					catch (SdlException e)
					{ 
						Console.WriteLine(e);
					}
	  
					try 
					{
						if (r <= surface.Width)
						{
							surface.DrawPixel(r, i, color);
						}
					} 
					catch (GuiException e)
					{ 
						Console.WriteLine(e);
						//throw e;
					}
				}
			}
		}
		#endregion

		#region Colors
		private Color backgroundColor = Color.FromArgb(200, 20, 20, 50);
		private Color frameColor = Color.FromArgb(50, 50, 50);
		//private Color selectedColor = Color.FromArgb(25, 25, 50);
		private Color traceBoundsColor = Color.FromArgb(200, 25, 25);
		private Color traceOuterColor = Color.FromArgb(100, 0, 0);
		private Color traceInnerColor = Color.FromArgb(255, 50, 50);
		private Color traceColor = Color.CornflowerBlue;

		/// <summary>
		/// 
		/// </summary>
		public Color FrameColor
		{
			get
			{
				return frameColor;
			}
			set
			{
				frameColor = value;
			}
		}
			/// <summary>
			/// 
			/// </summary>
			public Color TraceColor
		{
			get 
			{ 
				return traceColor; 
			}
			set 
			{ 
				traceColor = value; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Color BackgroundColor
		{
			get 
			{ 
				return backgroundColor; 
			}
			set 
			{ 
				backgroundColor = value; 
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public Color BoundsTraceColor
		{
			get 
			{ 
				return traceBoundsColor; 
			}
			set 
			{ 
				traceBoundsColor = value; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Color OuterBoundsTraceColor
		{
			get 
			{ 
				return traceOuterColor; 
			}
			set 
			{ 
				traceOuterColor = value; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Color InnerBoundsTraceColor
		{
			get 
			{ 
				return traceInnerColor; 
			}
			set 
			{ 
				traceInnerColor = value; 
			}
		}
		#endregion

		/*************************************************************/

		#region Information
		/// <summary>
		/// 
		/// </summary>
		/// <param name="title"></param>
		/// <returns></returns>
		public int GetTitleHeight(string title)
		{
			if (title == null || titleFont == null)
			{
				return 0;
			}
			else
			{
				return titleFont.Render(title, titleColor).Height;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="title"></param>
		/// <returns></returns>
		public int GetTitleWidth(string title)
		{
			if (title == null || titleFont == null)
			{
				return 0;
			}
			else
			{
				return titleFont.Render(title, titleColor).Width
					+ 2 * titleHorzPad;
			}
		}
		#endregion

		#region Properties
		private SpriteCollection spriteManager;
		private Size size = Size.Empty;

		private Color titleColor = Color.FromArgb(250, 250, 250);
		/// <summary>
		/// 
		/// </summary>
		public Color TitleColor
		{
			get
			{
				return titleColor;
			}
			set
			{
				titleColor = value;
			}
		}
		private int titleHorzPad = 3;
		private int dragZOrder = 10000;

		/// <summary>
		/// 
		/// </summary>
		public int DragZOrder
		{
			get 
			{ 
				return dragZOrder; 
			}
			set 
			{ 
				dragZOrder = value; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Size Size
		{
			get 
			{ 
				return size; 
			}
			set 
			{ 
				size = value; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public SpriteCollection SpriteManager
		{
			get 
			{ 
				return spriteManager; 
			}
//			set
//			{
//				if (value == null)
//				{
//					throw new SdlException("Cannot assign null sprite manager to gui");
//				}
//
//				spriteManager = value;
//			}
		}
		#endregion
	}
}
