//
// SCSharpMac.UI.SmackerPlayer
//
// Authors:
//	Chris Toshok (toshok@gmail.com)
//
// Copyright 2006-2010 Chris Toshok
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

using System.Drawing;

using SCSharp.Smk;
using SCSharp;

namespace SCSharpMac.UI
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

		float timeElapsed=0;
		CALayer layer;
		
		SmackerPlayerDelegate del;
		
		class SmackerPlayerDelegate : CALayerDelegate {
			
			public override void DisplayLayer (CALayer layer)
			{
				if (Contents != null)
					layer.Contents = Contents;
			}
			
			public CGImage Contents { get; set; }
		}

		public SmackerPlayer (Stream smk_stream) : this (smk_stream, BUFFERED_FRAMES)
		{
		}

		public SmackerPlayer (Stream smk_stream, int buffered_frames)
		{
			file = SmackerFile.OpenFromStream(smk_stream);
			decoder= file.Decoder;
			this.buffered_frames = buffered_frames;
    
			waitEvent = new AutoResetEvent (false);
			
			del = new SmackerPlayerDelegate ();

			layer = CALayer.Create ();
			layer.Bounds = new RectangleF (0, 0, Width, Height);
			layer.Delegate = del;
		}

		public int Width {
			get { return (int)file.Header.Width; }
		}

		public int Height {
			get { return (int)file.Header.Height; }
		}

		void Events_Tick(object sender, TickEventArgs e)
		{
			timeElapsed += e.SecondsElapsed;
			
			while (timeElapsed > 1.0 / file.Header.Fps)
			{
				lock (((ICollection)frameQueue).SyncRoot) {
					if (frameQueue.Count <= 0)
						return;
				}
					
				timeElapsed -= (float)(1.0f / file.Header.Fps);
				byte[] argbData = frameQueue.Dequeue();
								
				var image = GuiUtil.CreateImage (argbData, (ushort)Width, (ushort)Height, 32, Width * 4);
				del.Contents = image;

				layer.SetNeedsDisplay ();
			
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
						
						int count;
						lock (((ICollection)frameQueue).SyncRoot) {				
							frameQueue.Enqueue(decoder.ARGBData);
							count = frameQueue.Count;
						}
						
						if (count >= buffered_frames)
							waitEvent.WaitOne ();
					}
					catch (Exception e) {
						Console.WriteLine ("exception in decoder thread"); 
						break;
					}
				}
			}

			firstRun = false;
			NSApplication.SharedApplication.BeginInvokeOnMainThread (EmitFinished);
		}

		public void Play ()
		{
			if (decoderThread != null)
				return;

			decoderThread = new Thread (DecoderThread);
			decoderThread.IsBackground = true;
			decoderThread.Start();
			
			Game.Instance.Tick += Events_Tick;
		}

		public void Stop ()
		{
			if (decoderThread == null)
				return;

			decoderThread.Abort ();
			decoderThread = null;

			Game.Instance.Tick -= Events_Tick;
		}

		public CALayer Layer {
			get { return layer; }
		}

		public event PlayerEvent Finished;

		void EmitFinished ()
		{
			if (Finished != null)
				Finished ();
		}
	}

	public delegate void PlayerEvent ();
}
