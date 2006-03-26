/*
 * $RCSfile: Video.cs,v $
 * Copyright (C) 2004, 2005 David Hudson (jendave@yahoo.com)
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
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using Tao.Sdl;

namespace SdlDotNet 
{
	/// <summary>
	/// Provides methods to set the video mode, create video surfaces, 
	/// hide and show the mouse cursor,
	/// and interact with OpenGL
	/// </summary>
	public sealed class Video
	{
		static Video()
		{
			Initialize();
		}

		Video()
		{
		}

		/// <summary>
		/// Closes and destroys this object
		/// </summary>
		public static void Close() 
		{
			Events.CloseVideo();
		}

		/// <summary>
		/// Initializes Video subsystem.
		/// </summary>
		public static void Initialize()
		{
			if ((Sdl.SDL_WasInit(Sdl.SDL_INIT_VIDEO)) 
				== (int) SdlFlag.FalseValue)
			{
				if (Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO) != (int) SdlFlag.Success)
				{
					throw SdlException.Generate();
				}
			}
		}

//		/// <summary>
//		/// Queries if the Video subsystem has been intialized.
//		/// </summary>
//		/// <remarks>
//		/// </remarks>
//		/// <returns>True if Video subsystem has been initialized, false if it has not.</returns>
//		public static bool IsInitialized
//		{
//			get
//			{
//				if ((Sdl.SDL_WasInit(Sdl.SDL_INIT_VIDEO) & Sdl.SDL_INIT_VIDEO) 
//					!= (int) SdlFlag.FalseValue)
//				{
//					return true;
//				}
//				else 
//				{
//					return false;
//				}
//			}
//		}

		/// <summary>
		/// Checks if the requested video mode is supported
		/// </summary>
		/// <param name="width">Width of mode</param>
		/// <param name="height">Height of mode</param>
		/// <param name="fullscreen">Fullscreen or not</param>
		/// <param name="bitsPerPixel">
		/// Bits per pixel. Typically 8, 16, 24 or 32
		/// </param>
		/// <remarks></remarks>
		/// <returns>
		/// True is mode is supported, false if it is not.
		/// </returns>
		public static bool IsVideoModeOk(int width, int height, bool fullscreen, int bitsPerPixel)
		{
			VideoModes flags = VideoModes.None;
			if (fullscreen)
			{
				flags = VideoModes.Fullscreen;
			}
			int result = Sdl.SDL_VideoModeOK(
				width, 
				height, 
				bitsPerPixel, 
				(int)flags);
			if (result == bitsPerPixel)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Checks if the application is active
		/// </summary>
		/// <returns>True is applications is active</returns>
		public static bool IsActive
		{
			get
			{
				return (Sdl.SDL_GetAppState() & (int)Focus.Application) !=0;
			}
		}
		/// <summary>
		/// Returns the highest bitsperpixel supported 
		/// for the given width and height
		/// </summary>
		/// <param name="width">Width of mode</param>
		/// <param name="height">Height of mode</param>
		/// <param name="fullscreen">Fullscreen mode</param>
		public static int BestBitsPerPixel(int width, int height, bool fullscreen)
		{
			VideoModes flags = VideoModes.None;
			if (fullscreen)
			{
				flags = VideoModes.Fullscreen;
			}
			return Sdl.SDL_VideoModeOK(
				width, 
				height, 
				VideoInfo.BitsPerPixel, 
				(int)flags);
		}


		/// <summary>
		/// Returns array of modes supported
		/// </summary>
		/// <param name="fullscreen">Fullscreen mode</param>
		/// <returns>Array of Size structs</returns>
		public static Size[] ListModes(bool fullscreen)
		{
			int flags = (int)VideoModes.None;
			if (fullscreen)
			{
				flags = (int)VideoModes.Fullscreen;
			}
			IntPtr format = IntPtr.Zero;
			Sdl.SDL_Rect[] rects = Sdl.SDL_ListModes(format, flags);
			Size[] size = new Size[rects.Length];
			for (int i=0; i<rects.Length; i++)
			{
				size[ i ].Width = rects[ i ].w;
				size[ i ].Height = rects[ i ].h; 
			}
			return size;
		}
		/// <summary>
		/// Sets the video mode of a fullscreen application
		/// </summary>
		/// <param name="width">width</param>
		/// <param name="height">height</param>
		/// <returns>a surface to draw to</returns>
		public static Surface SetVideoMode(int width, int height) 
		{
			return SetVideoMode(width, height, 0,
				(VideoModes.Fullscreen));
		}
		/// <summary>
		/// Sets the video mode of a fullscreen application
		/// </summary>
		/// <param name="width">screen width</param>
		/// <param name="height">screen height</param>
		/// <param name="bitsPerPixel">bits per pixel</param>
		/// <returns>a surface to draw to</returns>
		public static Surface SetVideoMode(int width, int height, int bitsPerPixel) 
		{
			return SetVideoMode(width, height, bitsPerPixel,
				(VideoModes.Fullscreen));
		}

		/// <summary>
		/// Sets the windowed video mode using current screen bpp
		/// </summary>
		/// <param name="width">The width of the window</param>
		/// <param name="height">The height of the window</param>
		/// <remarks>It puts a frame around the window</remarks>
		/// <returns>a surface to draw to</returns>
		public static Surface SetVideoModeWindow(int width, int height) 
		{
			return Video.SetVideoModeWindow(width, height, false);
		}

		/// <summary>
		/// Sets the windowed video mode using a given bpp
		/// </summary>
		/// <param name="width">The width of the window</param>
		/// <param name="height">The height of the window</param>
		/// <param name="bitsPerPixel">bits per pixel</param>
		/// <remarks>It puts a frame around the window</remarks>
		/// <returns>a surface to draw to</returns>
		public static Surface SetVideoModeWindow(int width, int height, int bitsPerPixel) 
		{
			return Video.SetVideoModeWindow(width, height, bitsPerPixel, false);
		}

		/// <summary>
		/// Sets the windowed video mode using current screen bpp
		/// </summary>
		/// <param name="width">The width of the window</param>
		/// <param name="height">The height of the window</param>
		/// <param name="resizable">
		/// If true, the window will be resizable
		/// </param>
		/// <returns>a surface to draw to</returns>
		public static Surface SetVideoModeWindow(int width, int height, bool resizable) 
		{
			VideoModes flags = VideoModes.None;
			if (resizable)
			{
				flags |= VideoModes.Resizable;
			}
			return SetVideoMode(width, height, 0, flags);
		}

		/// <summary>
		/// Sets the windowed video mode using current screen bpp
		/// </summary>
		/// <param name="width">The width of the window</param>
		/// <param name="height">The height of the window</param>
		/// <param name="resizable">
		/// If true, the window will be resizable
		/// </param>
		/// <param name="frame">
		/// if true, the window will have a frame around it
		/// </param>
		/// <returns>a surface to draw to</returns>
		public static Surface SetVideoModeWindow(int width, int height, bool resizable, bool frame) 
		{
			VideoModes flags = VideoModes.None;
			if (resizable)
			{
				flags |= VideoModes.Resizable;
			}
			if (!frame)
			{
				flags |= VideoModes.NoFrame;
			}
			return SetVideoMode(width, height, 0, flags);
		}

		/// <summary>
		/// Sets the windowed video mode
		/// </summary>
		/// <param name="width">The width of the window</param>
		/// <param name="height">The height of the window</param>
		/// <param name="resizable">
		/// If true, the window will be resizable
		/// </param>
		/// <param name="bitsPerPixel">bits per pixel</param>
		/// <returns>a surface to draw to</returns>
		public static Surface SetVideoModeWindow(
			int width, 
			int height, 
			int bitsPerPixel, 
			bool resizable) 
		{
			VideoModes flags = VideoModes.None;
			if (resizable)
			{
				flags |= VideoModes.Resizable;
			}
			return SetVideoMode(width, height, bitsPerPixel, flags);
		}

		/// <summary>
		/// Sets the windowed video mode
		/// </summary>
		/// <param name="width">The width of the window</param>
		/// <param name="height">The height of the window</param>
		/// <param name="resizable">
		/// If true, the window will be resizable
		/// </param>
		/// <param name="frame">
		/// if true, the window will have a frame around it
		/// </param>
		/// <param name="bitsPerPixel">bits per pixel</param>
		/// <returns>a surface to draw to</returns>
		public static Surface SetVideoModeWindow(
			int width, 
			int height, 
			int bitsPerPixel,
			bool resizable,
			bool frame) 
		{
			VideoModes flags = VideoModes.None;
			if (resizable)
			{
				flags |= VideoModes.Resizable;
			}
			if (!frame)
			{
				flags |= VideoModes.NoFrame;
			}
			return SetVideoMode(width, height, bitsPerPixel, flags);
		}
		
		/// <summary>
		/// Sets a full-screen video mode suitable for drawing with OpenGL
		/// </summary>
		/// <param name="width">the horizontal resolution</param>
		/// <param name="height">the vertical resolution</param>
		/// <param name="bitsPerPixel">bits per pixel</param>
		/// <returns>A Surface representing the screen</returns>
		public static Surface SetVideoModeOpenGL(int width, int height, int bitsPerPixel) 
		{
			return SetVideoMode(width, height, bitsPerPixel,
				(VideoModes.OpenGL|VideoModes.Fullscreen));
		}

		/// <summary>
		/// Sets a full-screen video mode suitable for drawing with OpenGL
		/// </summary>
		/// <param name="width">the horizontal resolution</param>
		/// <param name="height">the vertical resolution</param>
		/// <returns>A Surface representing the screen</returns>
		public static Surface SetVideoModeOpenGL(int width, int height) 
		{
			return SetVideoMode(width, height, 0,
				(VideoModes.OpenGL|VideoModes.Fullscreen));
		}
		/// <summary>
		/// Sets a windowed video mode suitable for drawing with OpenGL
		/// </summary>
		/// <param name="width">The width of the window</param>
		/// <param name="height">The height of the window</param>
		/// <param name="resizable">
		/// If true, the window will be resizable
		/// </param>
		/// <returns>A Surface representing the window</returns>
		public static Surface SetVideoModeWindowOpenGL(
			int width, 
			int height, 
			bool resizable) 
		{
			VideoModes flags = VideoModes.OpenGL;
			if (resizable)
			{
				flags |= VideoModes.Resizable;
			}
			return SetVideoMode(width, height, 0, flags);
		}
		/// <summary>
		/// Sets a windowed video mode suitable for drawing with OpenGL
		/// </summary>
		/// <param name="width">The width of the window</param>
		/// <param name="height">The height of the window</param>
		/// <param name="resizable">
		/// If true, the window will be resizable
		/// </param>
		/// <param name="frame">
		/// If true, the window will have a frame around it
		/// </param>
		/// <returns>A Surface representing the window</returns>
		public static Surface SetVideoModeWindowOpenGL(
			int width, 
			int height, 
			bool resizable,
			bool frame) 
		{
			VideoModes flags = (VideoModes.OpenGL);
			if (resizable)
			{
				flags |= VideoModes.Resizable;
			}
			if (!frame)
			{
				flags |= VideoModes.NoFrame;
			}
			return SetVideoMode(width, height, 0, flags);
		}

		/// <summary>
		/// Sets a windowed video mode suitable for drawing with OpenGL
		/// </summary>
		/// <param name="width">The width of the window</param>
		/// <param name="height">The height of the window</param>
		/// <returns>A Surface representing the window</returns>
		public static Surface SetVideoModeWindowOpenGL(
			int width, 
			int height)
		{
			return SetVideoModeWindowOpenGL(width, height, false);

		}
		/// <summary>
		/// Sets the video mode of a fullscreen application
		/// </summary>
		/// <param name="width">screen width</param>
		/// <param name="height">screen height</param>
		/// <param name="bitsPerPixel">bits per pixel</param>
		/// <param name="flags">
		/// specific flags, see SDL documentation for details
		/// </param>
		/// <returns>A Surface representing the screen</returns>
		public static Surface SetVideoMode(int width, int height, int bitsPerPixel, VideoModes flags) 
		{
			return new Surface(Sdl.SDL_SetVideoMode(width, height, bitsPerPixel, (int)flags), true);
		}


		/// <summary>
		/// Gets the surface for the window or screen, 
		/// must be preceded by a call to SetVideoMode
		/// </summary>
		/// <returns>The main screen surface</returns>
		public static Surface Screen
		{
			get
			{
				return Surface.FromScreenPtr(Sdl.SDL_GetVideoSurface());
			}
		}

		/// <summary>
		/// Creates a new empty surface
		/// </summary>
		/// <param name="width">The width of the surface</param>
		/// <param name="height">The height of the surface</param>
		/// <param name="depth">The bits per pixel of the surface</param>
		/// <param name="redMask">
		/// A bitmask giving the range of red color values in the surface 
		/// pixel format
		/// </param>
		/// <param name="greenMask">
		/// A bitmask giving the range of green color values in the surface 
		/// pixel format
		/// </param>
		/// <param name="blueMask">
		/// A bitmask giving the range of blue color values in the surface 
		/// pixel format
		/// </param>
		/// <param name="alphaMask">
		/// A bitmask giving the range of alpha color values in the surface 
		/// pixel format
		/// </param>
		/// <param name="hardware">
		/// A flag indicating whether or not to attempt to place this surface
		///  into video memory</param>
		/// <returns>A new surface</returns>
		public static Surface CreateRgbSurface(
			int width, 
			int height, 
			int depth, 
			int redMask, 
			int greenMask, 
			int blueMask, 
			int alphaMask, 
			bool hardware) 
		{
			return new Surface(Sdl.SDL_CreateRGBSurface(
				hardware?(int)VideoModes.HardwareSurface:(int)VideoModes.None,
				width, height, depth,
				redMask, greenMask, blueMask, alphaMask));
		}

		/// <summary>
		/// Creates a new empty surface
		/// </summary>
		/// <param name="width">The width of the surface</param>
		/// <param name="height">The height of the surface</param>
		/// <returns>A new surface</returns>
		public static Surface CreateRgbSurface(int width, int height)
		{
			return Video.CreateRgbSurface(width, height, VideoInfo.BitsPerPixel,VideoInfo.RedMask, VideoInfo.GreenMask, VideoInfo.BlueMask, VideoInfo.AlphaMask, false);
		}

		/// <summary>
		/// Swaps the OpenGL screen, only if the double-buffered 
		/// attribute was set.
		/// Call this instead of Surface.Flip() for OpenGL windows.
		/// </summary>
		public static void GLSwapBuffers() 
		{
			Sdl.SDL_GL_SwapBuffers();
		}
		/// <summary>
		/// Sets an OpenGL attribute
		/// </summary>
		/// <param name="attribute">The attribute to set</param>
		/// <param name="attributeValue">The new attribute value</param>
		public static void GLSetAttribute(OpenGLAttr attribute, int attributeValue) 
		{
			if (Sdl.SDL_GL_SetAttribute((int)attribute, attributeValue) != 0)
			{
				throw SdlException.Generate();
			}
		}
		/// <summary>
		/// Gets the value of an OpenGL attribute
		/// </summary>
		/// <param name="attribute">The attribute to get</param>
		/// <returns>The current attribute value</returns>
		public static int GLGetAttribute(OpenGLAttr attribute) 
		{
			int returnValue;
			if (Sdl.SDL_GL_GetAttribute((int)attribute, out returnValue) != 0)
			{
				throw SdlException.Generate();
			}
			return returnValue;
		}

		/// <summary>
		/// Gets or sets the size of the GL red framebuffer.
		/// </summary>
		/// <value>The size of the GL red framebuffer.</value>
		public static int GLRedFrameBufferSize
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.RedSize);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.RedSize, value);
			}
		}

		/// <summary>
		/// Gets or sets the size of the GL green framebuffer.
		/// </summary>
		/// <value>The size of the GL green framebuffer.</value>
		public static int GLGreenFrameBufferSize
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.GreenSize);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.GreenSize, value);
			}
		}

		/// <summary>
		/// Gets or sets the size of the GL blue framebuffer.
		/// </summary>
		/// <value>The size of the GL blue framebuffer.</value>
		public static int GLBlueFrameBufferSize
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.BlueSize);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.BlueSize, value);
			}
		}

		/// <summary>
		/// Gets or sets the size of the GL alpha framebuffer.
		/// </summary>
		/// <value>The size of the GL alpha framebuffer.</value>
		public static int GLAlphaFrameBufferSize
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.AlphaSize);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.AlphaSize, value);
			}
		}

		/// <summary>
		/// Gets or sets the size of the GL framebuffer.
		/// </summary>
		/// <value>The size of the GL framebuffer.</value>
		public static int GLFrameBufferSize
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.BufferSize);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.BufferSize, value);
			}
		}

		/// <summary>
		/// Gets or sets the size of the GL depth.
		/// </summary>
		/// <value>The size of the GL depth.</value>
		public static int GLDepthSize
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.DepthSize);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.DepthSize, value);
			}
		}

		/// <summary>
		/// Gets or sets the size of the GL stencil.
		/// </summary>
		/// <value>The size of the GL stencil.</value>
		public static int GLStencilSize
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.StencilSize);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.StencilSize, value);
			}
		}

		/// <summary>
		/// Gets or sets the size of the GL red accumulation buffer.
		/// </summary>
		/// <value>The size of the GL red accumulation buffer.</value>
		public static int GLRedAccumulationBufferSize
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.AccumulationRedSize);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.AccumulationRedSize, value);
			}
		}

		/// <summary>
		/// Gets or sets the size of the GL green accumulation buffer.
		/// </summary>
		/// <value>The size of the GL green accumulation buffer.</value>
		public static int GLGreenAccumulationBufferSize
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.AccumulationGreenSize);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.AccumulationGreenSize, value);
			}
		}

		/// <summary>
		/// Gets or sets the size of the GL blue accumulation buffer.
		/// </summary>
		/// <value>The size of the GL blue accumulation buffer.</value>
		public static int GLBlueAccumulationBufferSize
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.AccumulationBlueSize);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.AccumulationBlueSize, value);
			}
		}

		/// <summary>
		/// Gets or sets the size of the GL alpha accumulation buffer.
		/// </summary>
		/// <value>The size of the GL alpha accumulation buffer.</value>
		public static int GLAlphaAccumulationBufferSize
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.AccumulationAlphaSize);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.AccumulationAlphaSize, value);
			}
		}

		/// <summary>
		/// Gets or sets the GL stereo rendering.
		/// </summary>
		/// <value>The GL stereo rendering.</value>
		public static int GLStereoRendering
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.StereoRendering);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.StereoRendering, value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is GL double buffer enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is GL double buffer enabled; otherwise, <c>false</c>.
		/// </value>
		public static bool GLDoubleBufferEnabled
		{
			get
			{
				int result = Video.GLGetAttribute(OpenGLAttr.DoubleBuffer);
				if (result == 1)
				{
					return false;
				}
				else if (result == 0)
				{
					return true;
				}
				else
				{
					throw new SdlException("Video.GLGetAttribute returned an improper result for DoubleBuffer");
				}
			}
			set
			{
				if (value == true)
				{
					Video.GLSetAttribute(OpenGLAttr.DoubleBuffer, 0);
				}
				else
				{
					Video.GLSetAttribute(OpenGLAttr.DoubleBuffer, 1);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is GL stereo rendering enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is GL stereo rendering enabled; otherwise, <c>false</c>.
		/// </value>
		public static bool GLStereoRenderingEnabled
		{
			get
			{
				int result = Video.GLGetAttribute(OpenGLAttr.StereoRendering);
				if (result == 1)
				{
					return false;
				}
				else if (result == 0)
				{
					return true;
				}
				else
				{
					throw new SdlException("Video.GLGetAttribute returned an improper result for StereoRendering");
				}
			}
			set
			{
				if (value == true)
				{
					Video.GLSetAttribute(OpenGLAttr.StereoRendering, 0);
				}
				else
				{
					Video.GLSetAttribute(OpenGLAttr.DoubleBuffer, 1);
				}
			}
		}

		/// <summary>
		/// Gets or sets the GL multi sample buffers.
		/// </summary>
		/// <value>The GL multi sample buffers.</value>
		public static int GLMultiSampleBuffers
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.MultiSampleBuffers);
			}
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				else if (value > 1)
				{
					value = 1;
				}
				Video.GLSetAttribute(OpenGLAttr.MultiSampleBuffers, value);
			}
		}

		/// <summary>
		/// Gets or sets the GL multi sample samples.
		/// </summary>
		/// <value>The GL multi sample samples.</value>
		public static int GLMultiSampleSamples
		{
			get
			{
				return Video.GLGetAttribute(OpenGLAttr.MultiSampleSamples);
			}
			set
			{
				Video.GLSetAttribute(OpenGLAttr.MultiSampleBuffers, value);
			}
		}


		/// <summary>
		/// gets or sets the text for the current window
		/// </summary>
		public static string WindowCaption 
		{
			get
			{
				string ret;
				string dummy;

				Sdl.SDL_WM_GetCaption(out ret, out dummy);
				return ret;
			}
			set
			{
				Sdl.SDL_WM_SetCaption(value, "");
			}
		}

		/// <summary>
		/// sets the icon for the current window
		/// </summary>
		/// <param name="icon">the surface containing the image</param>
		/// <remarks>This should be called before Video.SetVideoMode</remarks>
		public static void WindowIcon(Surface icon) 
		{
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			Sdl.SDL_WM_SetIcon(icon.Handle, null);
		}

		/// <summary>
		/// sets the icon for the current window
		/// </summary>
		/// <remarks>
		/// On OS X, this method returns nothing since OS X does not use window icons.
		/// </remarks>
		/// <param name="icon">Icon to use</param>
		/// <remarks>This should be called before Video.SetVideoMode</remarks>
		public static void WindowIcon(Icon icon)
		{
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			try
			{
				Bitmap bitmap = icon.ToBitmap();
				Surface surface = new Surface(bitmap);
				surface.TransparentColor = Color.Empty;
				//surface.ClearTransparentColor();
				WindowIcon(surface);
			}
			catch (SdlException e)
			{
				e.ToString();
				return;
			}
		}

		/// <summary>
		/// Sets the icon for the current window. 
		/// This method assumes there is an embedded 
		/// resource named &quot;App.ico&quot;.
		/// </summary>
		/// <remarks>This should be called before Video.SetVideoMode</remarks>
		public static void WindowIcon()
		{
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			string iconName = "";
			foreach (string s in Assembly.GetCallingAssembly().GetManifestResourceNames())
			{
				Console.WriteLine(s);
				if (s.EndsWith("App.ico"))
				{
					iconName = s;
					break;
				}
			}
			if (iconName.Length > 0)
			{
				Video.WindowIcon(new Icon(callingAssembly.GetManifestResourceStream(iconName)));
			}
		}

		/// <summary>
		/// Minimizes the current window
		/// </summary>
		/// <returns>True if the action succeeded, otherwise False</returns>
		public static bool Hide()
		{
			return Video.IconifyWindow();
		}

		/// <summary>
		/// Iconifies (minimizes) the current window
		/// </summary>
		/// <returns>True if the action succeeded, otherwise False</returns>
		public static bool IconifyWindow() 
		{
			return (Sdl.SDL_WM_IconifyWindow() != (int) SdlFlag.Success);
		}
		/// <summary>
		/// Forces keyboard focus and prevents the mouse from leaving the window
		/// </summary>
		public static void GrabInput() 
		{
			Sdl.SDL_WM_GrabInput(Sdl.SDL_GRAB_ON);
		}
		/// <summary>
		/// Queries if input has been grabbed.
		/// </summary>
		public static bool IsInputGrabbed
		{
			get
			{
				return (Sdl.SDL_WM_GrabInput(Sdl.SDL_GRAB_QUERY) == Sdl.SDL_GRAB_ON);
			}
		}
		/// <summary>
		/// Releases keyboard and mouse focus from a previous call to GrabInput()
		/// </summary>
		public static void ReleaseInput() 
		{
			Sdl.SDL_WM_GrabInput(Sdl.SDL_GRAB_OFF);
		}

		/// <summary>
		/// Returns video driver name
		/// </summary>
		public static string VideoDriver
		{
			get
			{
				string buffer="";
				return Sdl.SDL_VideoDriverName(buffer, 100);
			}
		}

		/// <summary>
		/// Sets gamma
		/// </summary>
		/// <param name="red">Red</param>
		/// <param name="green">Green</param>
		/// <param name="blue">Blue</param>
		public static void Gamma(float red, float green, float blue)
		{
			int result = Sdl.SDL_SetGamma(red, green, blue);
			if (result != 0)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Sets gamma for all colors
		/// </summary>
		/// <param name="gammaValue">Gamma to set for all colors</param>
		public static void Gamma(float gammaValue)
		{
			int result = Sdl.SDL_SetGamma(gammaValue, gammaValue, gammaValue);
			if (result != 0)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Gets red gamma ramp
		/// </summary>
		public static short[] GetGammaRampRed()
		{
			short[] red = new short[256];
			int result = Sdl.SDL_GetGammaRamp(red, null, null);
			if (result != 0)
			{
				throw SdlException.Generate();
			}
			return red;
		}

		/// <summary>
		/// Sets red gamma ramp
		/// </summary>
		/// <param name="gammaArray"></param>
		public static void SetGammaRampRed(short[] gammaArray)
		{
			int result = Sdl.SDL_SetGammaRamp(gammaArray, null, null);
			if (result != 0)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Gets blue gamma ramp
		/// </summary>
		public static short[] GetGammaRampBlue()
		{
			short[] blue = new short[256];
			int result = Sdl.SDL_GetGammaRamp(null, null, blue);
			if (result != 0)
			{
				throw SdlException.Generate();
			}
			return blue;
		}

		/// <summary>
		/// Sets blue gamma ramp
		/// </summary>
		/// <param name="gammaArray"></param>
		public static void SetGammaRampBlue(short[] gammaArray)
		{
			int result = Sdl.SDL_SetGammaRamp(null, null, gammaArray);
			if (result != 0)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Gets green gamma ramp
		/// </summary>
		public static short[] GetGammaRampGreen()
		{
			short[] green = new short[256];
			int result = Sdl.SDL_GetGammaRamp(null, green, null);
			if (result != 0)
			{
				throw SdlException.Generate();
			}
			return green;
		}

		/// <summary>
		/// Sets green gamma ramp
		/// </summary>
		/// <param name="gammaArray"></param>
		public static void SetGammaRampGreen(short[] gammaArray)
		{
			int result = Sdl.SDL_SetGammaRamp(null, gammaArray, null);
			if (result != 0)
			{
				throw SdlException.Generate();
			}
		}

		/// <summary>
		/// Update entire screen
		/// </summary>
		public static void Update()
		{
			Video.Screen.Update();
		}

		/// <summary>
		/// Updates rectangle
		/// </summary>
		/// <param name="rectangle">
		/// Rectangle to update
		/// </param>
		public static void Update(System.Drawing.Rectangle rectangle)
		{
			Video.Screen.Update(rectangle);
		}

		/// <summary>
		/// Update an array of rectangles
		/// </summary>
		/// <param name="rectangles">
		/// Array of rectangles to update
		/// </param>
		public static void Update(System.Drawing.Rectangle[] rectangles)
		{
			Video.Screen.Update(rectangles);
		}

		/// <summary>
		/// This returns the platform window handle for the SDL window.
		/// </summary>
		/// <remarks>
		/// TODO: The Unix SysWMinfo struct has not been finished. 
		/// This only runs on Windows right now.
		/// </remarks>
		public static IntPtr WindowHandle
		{
			get
			{
				int p = (int) Environment.OSVersion.Platform;
				if ((p == 4) || (p == 128)) 
				{
					Sdl.SDL_SysWMinfo wmInfo; 
					Sdl.SDL_GetWMInfo(out wmInfo); 
					return new IntPtr(wmInfo.data); 
				} 
				else 
				{
					Sdl.SDL_SysWMinfo_Windows wmInfo; 
					Sdl.SDL_GetWMInfo(out wmInfo); 
					return new IntPtr(wmInfo.window); 
				}
			}
		}
	}
}
