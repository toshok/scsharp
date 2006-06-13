using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Tao.Sdl;

namespace SCSharp.Smk
{
    public class SDLPCMStream : MemoryStream, IDisposable
    {
        #region "Audioformat description"
        public class SDLPCMStreamFormat
        {
            public SDLPCMStreamFormat(PCMFormat format, int frequency, int nbChannels)
            {
                Format = format;
                SampleRate = frequency;
                NbChannels = nbChannels;
                ChunkSize = 512;
            }

            /// <summary>
            /// LE stands for little-endian, BE big-endian
            /// </summary>
            public enum PCMFormat : short
            {
                Signed16BitLE = Sdl.AUDIO_S16LSB,
                Signed16BitBE = Sdl.AUDIO_S16MSB,
                
                Signed8Bit = Sdl.AUDIO_S8,
                UnSigned16BitLE = Sdl.AUDIO_U16LSB,
                UnSigned16BitBE = Sdl.AUDIO_U16MSB,
                
                UnSigned8Bit = Sdl.AUDIO_U8

            }
            private int frequency;

            public int SampleRate
            {
                get { return frequency; }
                set { frequency = value; }
            }
            private int nbChannels;

            public int NbChannels
            {
                get { return nbChannels; }
                set { nbChannels = value; }
            }
            private PCMFormat format;

            public PCMFormat Format
            {
                get { return format; }
                set { format = value; }
            }
            private int chunkSize;

            public int ChunkSize
            {
                get { return chunkSize; }
                set { chunkSize = value; }
            }
            public int BytesPerSample
            {
                get
                {
                    int byteSize = 1;
                    if (Format == PCMFormat.UnSigned16BitLE || Format == PCMFormat.UnSigned16BitBE || Format == PCMFormat.Signed16BitLE || Format == PCMFormat.Signed16BitBE)
                    {
                        byteSize = 2;
                    }
                    return byteSize;
                }
            }
/// <summary>
/// The BlockSize in bytes
/// </summary>
            public int BlockSize {
                get {

                    return BytesPerSample * NbChannels;
                }
            }
            public int BytesPerSec {
                get{
                    return BlockSize * SampleRate;
                }
            }

        }
        #endregion

        SDLPCMStreamFormat format;

        public SDLPCMStreamFormat Format
        {
            get { return format; }
          
        } 
        /// <summary>
        /// Creates a new stream large enough to hold the specified amount of bytes
        /// </summary>
        /// <param name="bufferSize">the amount of bytes the stream should hold</param>
        public SDLPCMStream(SDLPCMStreamFormat format, int bufferSize) :base(bufferSize)
        {
            OpenDevice(format);
                
        }

        public SDLPCMStream(SDLPCMStreamFormat format)  
        {
            //Directsound uses a 4 second buffer and that is said to be a good number
            int bufferSize = format.BytesPerSec * 4;
         //  this.SetLength(bufferSize);
            OpenDevice(format);
        }
        private void OpenDevice(SDLPCMStreamFormat format)
        {
            this.format = format;
            int result =  SdlMixer.Mix_OpenAudio(format.SampleRate, (short)format.Format, format.NbChannels, format.ChunkSize);
             result =  SdlMixer.Mix_AllocateChannels(1);
            
        }
        private Thread playThread;
        private bool playing;

public bool Playing
{
  get { return playing; }
 
}
        public void Play()
        {
            if (Playing) return;
            playing = true;
            
            playThread = new Thread(new ThreadStart(PlayThreadStart));
          // SdlMixer.Mix_ChannelFinished(new SdlMixer.ChannelFinishedDelegate(ChannelFinished));
           SdlMixer.Mix_Volume(-1, 128);
           playThread.Start();
        }
        IntPtr blob ;
        bool firstTime = true;
        private void PlayThreadStart()
        {
            while (true)
            {
                long dif = WritePointer - ReadPointer;

                if (dif < 0)
                {
                    //We have a circular buffer and we are writing in front of the readpointer
                    dif = WritePointer + Length - ReadPointer;
                }
                if (dif != 0)
                {

                    long nbBytes = Math.Max(512L, dif);
                    byte[] bytes = new byte[nbBytes];
                    Seek(readPointer, SeekOrigin.Begin);
                    Read(bytes, 0, (int)nbBytes);

                    readPointer += nbBytes;
                    if (!firstTime) System.Runtime.InteropServices.Marshal.FreeCoTaskMem(blob);
                    firstTime = false;
                    blob = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(bytes.Length);

                    System.Runtime.InteropServices.Marshal.Copy(bytes, 0, blob, bytes.Length);

                    IntPtr chunk = SdlMixer.Mix_QuickLoad_RAW(blob, bytes.Length);
                    int result = SdlMixer.Mix_PlayChannel(-1, chunk, 0);
					
                    //Sleep for nbBytes 
                    float timeTaken = (float)nbBytes / (float)Format.BytesPerSec * 1000.0f;
                    Thread.Sleep((int)timeTaken);
                    



                }
                else //Suspend, no more data to play
                    playThread.Suspend();
            }
            
            
           
        }
        public void ChannelFinished(int i)
        {
            PlayThreadStart();
        }

        public void Stop()
        {
            if (!playing) return;
            playing = false;
            if (playThread != null)
            {
                playThread.Abort();
            }
            readPointer = 0;
        }
      

        private long readPointer;

        public long ReadPointer
        {
            get { return readPointer; }
            set { readPointer = value; }
        }
        private long writePointer;

        public long WritePointer
        {
            get { return writePointer; }
            set { writePointer = value; }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
        }

        public void WritePCM(byte[] pcmdata)
        {
            Seek(writePointer, SeekOrigin.Begin);
          //  if (writePointer - readPointer > 0 && writePointer - readPointer < pcmdata.Length)
          //      throw new OutOfMemoryException("The buffer is full");
            Write(pcmdata, 0, pcmdata.Length);
            writePointer += pcmdata.Length;

            //Notify the playthread there is more data waiting
            if (playThread != null &&playThread.ThreadState == System.Threading.ThreadState.Suspended)
            	playThread.Resume	();
        }

        public void SaveWAV(string filename)
        {

        }
		public void Dispose()
		{
			base.Dispose();
			Stop();
		}
    }
}
