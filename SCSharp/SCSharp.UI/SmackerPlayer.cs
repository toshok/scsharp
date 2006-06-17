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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using SdlDotNet;
using System.Drawing;

using SCSharp.Smk;

namespace SCSharp.UI
{
	public class SmackerPlayer
	{
		//Buffer this many frames
		private  const int BUFFERED_FRAMES = 100;

		Thread decoderThread;
		bool firstRun = true;
		int buffered_frames;

		Queue<byte[]> frameQueue = new Queue<byte[]>();

		SmackerFile file;
		SmackerDecoder decoder;

		AutoResetEvent waitEvent;

		//SDLPCMStream audioStream;
		public SmackerPlayer (Stream smk_stream) : this (smk_stream, BUFFERED_FRAMES)
		{
		}

		public SmackerPlayer (Stream smk_stream, int buffered_frames)
		{
			file = SmackerFile.OpenFromStream(smk_stream);
			decoder= file.Decoder;
			this.buffered_frames = buffered_frames;
    
			//stream.Close();

			//Init audio
			//SDLPCMStream.SDLPCMStreamFormat format = new SDLPCMStream.SDLPCMStreamFormat(SDLPCMStream.SDLPCMStreamFormat.PCMFormat.UnSigned16BitLE, file.Header.GetSampleRate(0), (file.Header.IsStereoTrack(0)) ? 2 : 1);
			//audioStream = new SDLPCMStream(format);

			waitEvent = new AutoResetEvent (false);
		}

		public int Width {
			get { return (int)file.Header.Width; }
		}

		public int Height {
			get { return (int)file.Header.Height; }
		}

		float timeElapsed=0;
		Surface surf;
		void Events_Tick(object sender, TickEventArgs e)
		{
			//decoder.ReadNextFrame();

			//There should be an easier way to get the video data to SDL
            
			timeElapsed += (e.SecondsElapsed);
			while (timeElapsed > 1.0 / file.Header.Fps && frameQueue.Count > 0)
			{
				timeElapsed -= (float)(1.0f / file.Header.Fps);
				byte[] rgbData = frameQueue.Dequeue();

				if (surf == null) {
					surf = GuiUtil.CreateSurface (rgbData, (ushort)file.Header.Width, (ushort)file.Header.Height,
								      32, (int)file.Header.Width * 4,
								      (int)0x00ff0000,
								      (int)0x0000ff00,
								      (int)0x000000ff,
								      unchecked ((int)0xff000000));
				}
				else {
					surf.Lock();
					Marshal.Copy(rgbData, 0, surf.Pixels, rgbData.Length);
					surf.Unlock();
					surf.Update();
				}

				EmitFrameReady ();

				if (frameQueue.Count < (buffered_frames / 2) + 1)
					waitEvent.Set ();
			}
		}

		void DecoderThread()
		{
			while (firstRun || file.Header.HasRingFrame())
			{
				decoder.Reset();
				while (decoder.CurrentFrame < file.Header.NbFrames)
				{
					try {
						decoder.ReadNextFrame();
						frameQueue.Enqueue(decoder.BGRAData);
						//audioStream.WritePCM(decoder.GetAudioData(0));
						//if (firstRun) audioStream.Play();
						// memAudioStream.Write(decoder.GetAudioData(0), 0, decoder.GetAudioData(0).Length);
						if (frameQueue.Count >= buffered_frames)
							waitEvent.WaitOne ();
					}
					catch {
						break;
					}
				}
			}

			firstRun = false;
			Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (EmitFinished)));
		}

		public void Play ()
		{
			if (decoderThread != null)
				return;

			decoderThread = new Thread (DecoderThread);
			decoderThread.IsBackground = true;
			decoderThread.Start();

			Events.Tick += Events_Tick;
		}

		public void Stop ()
		{
			if (decoderThread == null)
				return;

			decoderThread.Abort ();
			decoderThread = null;

			Events.Tick -= Events_Tick;
		}

		public Surface Surface {
			get { return surf; }
		}

		public event PlayerEvent Finished;
		public event PlayerEvent FrameReady;

		void EmitFinished ()
		{
			if (Finished != null)
				Finished ();
		}

		void EmitFrameReady ()
		{
			if (FrameReady != null)
				FrameReady ();
		}
	}

	public delegate void PlayerEvent ();
}
