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

using SdlDotNet;
using System;
using System.Drawing;
using System.Globalization;

namespace SdlDotNet.Sprites
{
	/// <summary>
	/// Implements a basic font that is given a font and a string and
	/// generates an appropriate surface from that font.
	/// </summary>
	public class TextSprite : Sprite
	{
		#region Constructors
		/// <summary>
		/// Creates a new TextSprite given the font.
		/// </summary>
		/// <param name="font">The font to use when rendering.</param>
		public TextSprite(SdlDotNet.Font font) : this(" ", font)
		{
		}

		/// <summary>
		/// Creates a new TextSprite with given the text and font.
		/// </summary>
		/// <param name="textItem">Text to display</param>
		/// <param name="font">The font to use when rendering.</param>
		public TextSprite(
			string textItem, 
			SdlDotNet.Font font) : base(font.Render(textItem, Color.White))
		{
			this.textItem = textItem;
			this.font = font;
			this.RenderInternal();
		}

		/// <summary>
		/// Creates a new TextSprite given the text, font and color.
		/// </summary>
		/// <param name="textItem">Text to display</param>
		/// <param name="font">The font to use when rendering.</param>
		/// <param name="color">color of Text</param>
		public TextSprite(
			string textItem, 
			SdlDotNet.Font font, 
			Color color) : base(font.Render(textItem, color))
		{
			this.textItem = textItem;
			this.font = font;
			this.color = color;
			this.RenderInternal();
		}

		/// <summary>
		/// Creates a new TextSprite given the text, font, color and anti-aliasing flag.
		/// </summary>
		/// <param name="textItem">Text to display</param>
		/// <param name="font">The font to use when rendering.</param>
		/// <param name="color">Color of Text</param>
		/// <param name="antiAlias">A flag determining if it's to 
		/// use anti-aliasing when rendering. Defaults to true.</param>
		public TextSprite(
			string textItem, 
			SdlDotNet.Font font, 
			Color color, bool antiAlias) : base(font.Render(textItem, color))
		{
			this.textItem = textItem;
			this.font = font;
			this.color = color;
			this.antiAlias = antiAlias;
			this.RenderInternal();
		}
		/// <summary>
		/// Creates a new TextSprite given the text, font, color and background color.
		/// </summary>
		/// <param name="textItem">Text to display</param>
		/// <param name="font">The font to use when rendering.</param>
		/// <param name="textColor">Text Color</param>
		/// <param name="backgroundColor">Background color</param>
		public TextSprite(
			string textItem, 
			SdlDotNet.Font font, 
			Color textColor,
			Color backgroundColor)
			: this(textItem, font, textColor)
		{
			this.backgroundColor = backgroundColor;
			this.RenderInternal();
		}

		/// <summary>
		/// Creates a new TextSprite given the text, font and position.
		/// </summary>
		/// <param name="textItem"></param>
		/// <param name="font">The font to use when rendering.</param>
		/// <param name="position"></param>
		public TextSprite(
			string textItem, 
			SdlDotNet.Font font, 
			Point position) : this(textItem, font)
		{
			base.Position = position;
			this.RenderInternal();
		}

		/// <summary>
		/// Creates a new TextSprite given the text, font, anti-aliasing flag and position.
		/// </summary>
		/// <param name="textItem">Text to display</param>
		/// <param name="font">The font to use when rendering.</param>
		/// <param name="antiAlias">A flag determining if it's to use 
		/// anti-aliasing when rendering. Defaults to true.</param>
		/// <param name="position">Position of sprite</param>
		public TextSprite(
			string textItem, 
			SdlDotNet.Font font,
			bool antiAlias,
			Point position) : this(textItem, font)
		{
			base.Position = position;
			this.antiAlias = antiAlias;
			this.RenderInternal();
		}

		/// <summary>
		/// Creates a new TextSprite given the text, font, color and position.
		/// </summary>
		/// <param name="textItem">Text to display</param>
		/// <param name="font">The font to use when rendering.</param>
		/// <param name="color">Color of Text</param>
		/// <param name="position">Position of Sprite</param>
		public TextSprite(
			string textItem, 
			SdlDotNet.Font font, 
			Color color,
			Point position)
			: this(textItem, font, position)
		{
			this.color = color;
			this.RenderInternal();
		}

		/// <summary>
		/// Creates a new TextSprite given the text, font, color, anti-aliasing flag and position.
		/// </summary>
		/// <param name="textItem">Text to display</param>
		/// <param name="font">The font to use when rendering.</param>
		/// <param name="color">Color of Text</param>
		/// <param name="antiAlias">A flag determining if it's to use anti-aliasing when rendering. Defaults to true.</param>
		/// <param name="position">Position of Sprite</param>
		public TextSprite(
			string textItem, 
			SdlDotNet.Font font, 
			Color color,
			bool antiAlias,
			Point position)
			: this(textItem, font, position)
		{
			this.color = color;
			this.antiAlias = antiAlias;
			this.RenderInternal();
		}
		#endregion Constructors

		#region Drawing
		/// <summary>
		/// Renders the font, if both the text and color and font are
		/// set. It stores the render in memory until it is used.
		/// </summary>
		/// <returns>The new renderation surface of the text.</returns>
		private void RenderInternal()
		{
			if (textItem == null)
			{
				textItem = " ";
			}

			// Render it (Solid or Blended)
			try
			{
				if (backgroundColor.IsEmpty)
				{
					base.Surface = font.Render(textItem, antiAlias, color);
				}
				else
				{
					base.Surface = font.Render(textItem, color, backgroundColor);
				}
				base.Size = new Size(base.Surface.Width, base.Surface.Height);
			}
			catch (SpriteException e)
			{
				base.Surface = null;
				throw new SdlException(e.ToString());
			}
		}
		#endregion

		#region Font Rendering

		private SdlDotNet.Font font;
		private bool antiAlias = true;

		private string textItem;

		private Color color = Color.White;
		private Color backgroundColor;

		/// <summary>
		/// Gets and sets the color to be used with the text.
		/// </summary>
		public Color Color
		{
			get 
			{ 
				return color; 
			}
			set 
			{ 
				color = value;
				this.RenderInternal();
			}
		}

		/// <summary>
		/// Gets and sets the background color to be used with the text.
		/// </summary>
		/// <remarks>Defaults as Color.Transparent.</remarks>
		public Color BackgroundColor
		{
			get 
			{ 
				return backgroundColor; 
			}
			set 
			{ 
				backgroundColor = value;
				this.RenderInternal();
			}
		}

		/// <summary>
		/// Gets and sets the font to be used with the text.
		/// </summary>
		public SdlDotNet.Font Font
		{
			get 
			{ 
				return font;
			}
			set 
			{ 
				if (value == null)
				{
					throw new SdlException("Cannot assign a null Font");
				}
				font = value;
				this.RenderInternal();
			}
		}

		/// <summary>
		/// Gets and sets the text to be rendered.
		/// </summary>
		public string Text
		{
			get 
			{ 
				return textItem; 
			}
			set 
			{ 
				textItem = value;
				this.RenderInternal();
			}
		}
		#endregion

		#region Operators
		/// <summary>
		/// Converts the text sprite to a string.
		/// </summary>
		/// <returns>Returns string representation of object</returns>
		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "(text \"{0}\",{1})", textItem, base.ToString());
		}
		#endregion

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
						if (this.font != null)
						{
							this.font.Dispose();
							this.font = null;
						}
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