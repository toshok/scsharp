/*
 * $RCSfile: Animation.cs,v $
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

namespace SdlDotNet.Sprites
{
	/// <summary>
	/// Animation.
	/// </summary>
	public class Animation : SurfaceCollection
	{

		#region Constructors
		/// <summary>
		/// Creates a new empty Animation.
		/// </summary>
		public Animation() : base()
		{
		}

		/// <summary>
		/// Creates a new Animation with a SurfaceCollection representing 
		/// the animation.
		/// </summary>
		/// <param name="frames">
		/// The collection of surfaces in the animation.
		/// </param>
		public Animation(SurfaceCollection frames) : base(frames)
		{
		}

		/// <summary>
		/// Creates an Animation with one surface to start off the animation.
		/// </summary>
		/// <param name="firstFrame">
		/// The surface representing the animation.
		/// </param>
		public Animation(Surface firstFrame) : base(firstFrame)
		{
		}

		/// <summary>
		/// Creates a new Animation with a SurfaceCollection representing the 
		/// animation.
		/// </summary>
		/// <param name="frames">
		/// The collection of surfaces in the animation.
		/// </param>
		/// <param name="delay">
		/// The amount of delay to be had between each frame.
		/// </param>
		/// <param name="loop">
		/// Whether or not the animation is 
		/// to loop when reached the end. Defaults to true.
		/// </param>
		public Animation(SurfaceCollection frames, double delay, bool loop) : base(frames)
		{
			this.m_Delay = delay;
			this.m_Loop = loop;
		}

		/// <summary>
		/// Creates a new Animation with a SurfaceCollection 
		/// representing the animation.
		/// </summary>
		/// <param name="frames">
		/// The collection of 
		/// surfaces in the animation.
		/// </param>
		/// <param name="delay">
		/// The amount of delay to be 
		/// had between each frame. Defaults to 30.
		/// </param>
		public Animation(SurfaceCollection frames, double delay) : base(frames)
		{
			this.m_Delay = delay;
		}
		#endregion Constructors

		#region Properties

		double m_Delay = 30;

		/// <summary>
		/// Gets and sets the amount of time delay that 
		/// should be had before moving onto the next frame.
		/// </summary>
		public double Delay
		{
			get
			{
				return m_Delay;
			}
			set
			{
				m_Delay = value;
			}
		}

		/// <summary>
		/// Gets and sets how long the animation should take to finish.
		/// </summary>
		public double AnimationTime
		{
			get
			{
				return m_Delay * this.Count;
			}
			set 
			{
				m_Delay = this.Count / value;
			}
		}

		bool m_Loop = true;

		/// <summary>
		/// Gets and sets whether or not the animation should loop.
		/// </summary>
		public bool Loop
		{
			get
			{ 
				return m_Loop;
			}
			set
			{ 
				m_Loop = value; 
			}
		}

		int m_FrameIncrement = 1;

		/// <summary>
		/// Gets and sets the number of frames to go forward 
		/// when moving onto the next frame.
		/// </summary>
		/// <remarks>
		/// Making this one would result in the 
		/// animation going forwards one frame. 
		/// -1 would mean that the animation would go backwards. 
		/// Cannot be 0.
		/// </remarks>
		public int FrameIncrement
		{
			get
			{
				return m_FrameIncrement;
			}
			set
			{
				if (value == 0)
				{
					m_FrameIncrement = 1;
				}
				else
				{
					m_FrameIncrement = value;
				}
			}
		}

		/// <summary>
		/// Gets and sets whether the animation goes forwards or backwards.
		/// </summary>
		public bool AnimateForward
		{
			get
			{
				return m_FrameIncrement >= 0;
			}
			set
			{
				if (value)
				{
					// Make positive
					if (m_FrameIncrement < 0)
					{
						m_FrameIncrement *= -1;
					}
				}
				else
				{
					// Make negative
					if (m_FrameIncrement > 0)
					{
						m_FrameIncrement *= -1;
					}
				}

			}
		}

		#endregion Properties
	}
}
