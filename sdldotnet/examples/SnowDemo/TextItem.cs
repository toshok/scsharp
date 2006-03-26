/* This file is part of SnowDemo
 * Text.cs, (c) 2003 Sijmen Mulder
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 */

using System;
using System.Drawing;
using System.Globalization;

using SdlDotNet;
using SdlDotNet.Sprites;

namespace SdlDotNet.Examples.SnowDemo
{
	enum TextFadeState
	{
		BeforeFadeIn,
		FadeIn,
		BeforeFadeOut,
		FadeOut,
		Finished
	}
	/// <summary>
	/// 
	/// </summary>
	public class TextItem : TextSprite
	{
		const float inSpeed = 40;
		const float outSpeed = 40;
		float time;
		float startTime;
		TextFadeState state = TextFadeState.BeforeFadeIn;
		float alpha;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="font"></param>
		/// <param name="positionY"></param>
		/// <param name="phrase"></param>
		/// <param name="startTime"></param>
		public TextItem(string phrase, Font font, int positionY, float startTime) : 
			base(phrase, font, false, new Point(25, positionY))
		{
			base.Surface.Alpha = 0;
			base.Surface.AlphaBlending = true;
			this.startTime = startTime;
		}

		/// <summary>
		/// 
		/// </summary>
		public float StartTime
		{
			get
			{
				return startTime;
			}
			set
			{
				startTime = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public float EndTime
		{
			get
			{
				return startTime + 4.5f;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public override void Update(TickEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			float seconds = args.SecondsElapsed;
			time = Timer.SecondsElapsed;

			switch(state)
			{
				case TextFadeState.BeforeFadeIn:
					if(time >= startTime)
					{
						state = TextFadeState.FadeIn;
					}
					break;

				case TextFadeState.FadeIn:
					if (seconds <= (float.MaxValue / inSpeed) - alpha)
					{
						alpha += seconds * inSpeed;
					}
					else
					{
						alpha = float.MaxValue;
					}

					if(alpha >= 255)
					{
						alpha = 255;
						state = TextFadeState.BeforeFadeOut;
					}
					this.Surface.Alpha = (byte)alpha;
					break;

				case TextFadeState.BeforeFadeOut:
					if(time >= this.EndTime)
					{
						state = TextFadeState.FadeOut;
					}
					break;

				case TextFadeState.FadeOut:
					alpha -= seconds * outSpeed;

					if(alpha <= 0)
					{
						alpha = 0;
						state = TextFadeState.Finished;
					}
					this.Surface.Alpha = (byte)alpha;
					break;
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
