//
// SCSharp.UI.SmackerPlayer
//
// Authors:
//	Chris Toshok (toshok@hungry.com)
//
// (C) 2006 The Hungry Programmers (http://www.hungry.com/)
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using SdlDotNet;
using System.Drawing;

namespace SCSharp.UI
{
	public class SmackerPlayer
	{
		enum State {
			STOPPED,
			PLAYING,
			PAUSED
		}

		string filename;
		byte[] buf;
		Surface surface;
		State state;
		Thread decoderThread;
		FFmpeg decoder;
		int width;
		int height;

		static object sync = new object();

		public SmackerPlayer (string filename,
				      Stream smk_stream,
				      int width, int height)
		{
			this.filename = filename;
			this.width = width;
			this.height = height;

			this.buf = GuiUtil.ReadStream (smk_stream);
		}

		public void Play ()
		{
			if (state == State.PAUSED) {
				state = State.PLAYING;
				decoderThread.Resume ();
			}
			else if (state == State.STOPPED) {
				state = State.PLAYING;
				decoderThread = new Thread (DecoderThread);
				decoderThread.IsBackground = true;
				decoderThread.Start();
			}
		}

		public void Stop ()
		{
			if (state == State.STOPPED)
				return;

			state = State.STOPPED;
			decoderThread.Abort ();
			decoder.Stop ();
			decoderThread = null;
			decoder = null;
			surface = null;
		}

		public void Pause ()
		{
			if (state == State.PAUSED || state == State.STOPPED)
				return;

			state = State.PAUSED;
			decoderThread.Suspend();
		}

		public void BlitSurface (Surface dest)
		{
			lock (sync) {
				if (surface != null)
					dest.Blit (surface,
						   new Point ((dest.Width - surface.Width) / 2,
							      (dest.Height - surface.Height) / 2));
			}
		}

		Surface ScaleSurface (Surface surf)
		{
			double horiz_zoom = (double)width / surf.Width;
			double vert_zoom = (double)height / surf.Height;
			double zoom;

			if (horiz_zoom < vert_zoom)
				zoom = horiz_zoom;
			else
				zoom = vert_zoom;

			if (zoom != 1.0)
				surf.Scale (zoom);

			return surf;
		}

		void DecoderThread ()
		{
			decoder = new FFmpeg (filename, buf);

			decoder.Start ();

			Console.WriteLine ("animation is {0}x{1}, we're displaying at {2}x{3}",
					   decoder.Width, decoder.Height,
					   width, height);
			byte[] frame_buf = new byte [decoder.Width * decoder.Height * 3];
			while (decoder.GetNextFrame (frame_buf)) {
				lock (sync) {
					if (surface != null)
						surface.Dispose ();
					surface = ScaleSurface (GuiUtil.CreateSurface (frame_buf,
										       (ushort)decoder.Width,
										       (ushort)decoder.Height,
										       24, decoder.Width * 3,
										       (int)0x000000ff,
										       (int)0x0000ff00,
										       (int)0x00ff0000,
										       (int)0x00000000));
				}
				Thread.Sleep (100);
			}

			decoder.Stop ();

			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (EmitFinished)));
		}

		public event PlayerEvent Finished;

		void EmitFinished ()
		{
			if (Finished != null)
				Finished ();
		}
	}

	public delegate void PlayerEvent ();

	class FFmpeg
	{
		static FFmpeg ()
		{
			ffmpeg_init();
		}

		GCHandle handle;
		string filename;
		byte[] buf;
		int width, height;

		public FFmpeg (string filename, byte[] buf)
		{
			this.filename = filename;
			this.buf = buf;
		}

		public void Start ()
		{
			handle = start_decoder (filename, buf, buf.Length);
			get_dimensions (handle, out width, out height);
		}

		public void Stop ()
		{
			if (handle.Target != null) {
				stop_decoder (handle);
				handle.Target = null;
			}
		}

		public bool GetNextFrame (byte[] buf)
		{
			return get_next_frame (handle, buf);
		}

		public int Width {
			get { return width; }
		}

		public int Height {
			get { return height; }
		}

		[DllImport ("ffmpegglue.dll")]
		extern static void ffmpeg_init ();

		[DllImport ("ffmpegglue.dll")]
		public extern static GCHandle start_decoder (string filename, byte[] buf, int buf_size);

		[DllImport ("ffmpegglue.dll")]
		public extern static void get_dimensions (GCHandle handle, out int width, out int height);

		[DllImport ("ffmpegglue.dll")]
		public extern static bool get_next_frame (GCHandle handle, byte[] buf);

		[DllImport ("ffmpegglue.dll")]
		public extern static void stop_decoder (GCHandle handle);
	}
}
