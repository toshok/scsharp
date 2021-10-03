
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

//using NUnit.Framework;

namespace SCSharp.Smk
{
    /// <summary>
    /// A utility class for reading groups of bits from a stream
    /// </summary>
    public class BitStream
    {
        private Stream mStream;
        private int mCurrentByte;
        private int mCurrentBit;
        private int nbBytes;
        //Raising this value causes more bytes to be cached in the stream and reduces the number of accesses to disk
        private const int MAX_BYTES = 512;
        private byte [] bytes = new byte[MAX_BYTES];
        
        public BitStream(Stream SourceStream)
        {
            mStream = SourceStream;
        }

        public Stream BaseStream
        { get { return mStream; } }

        //This method needs to be lightning fast: it's run thousends of time when decoding an SMK Frame.
        public int ReadBits(int BitCount)
        {
            if (BitCount > 16)
                throw new ArgumentOutOfRangeException("BitCount", "Maximum BitCount is 16");
            
            //We need BitCount bits
            int result = 0;
            int bitsRead = 0;
            while (BitCount > 0)
            {
                if (mCurrentByte >= nbBytes)
                {
                    if (mStream.Position >= mStream.Length)
                        throw new EndOfStreamException();
                    nbBytes = mStream.Read(bytes, 0, MAX_BYTES);
                    mCurrentByte = 0;
                    mCurrentBit = 0;
                }
                
                if (mCurrentBit + BitCount < 8)  //Everything fits in this byte
                {
                    result |= (((int)bytes[mCurrentByte] >> mCurrentBit) & (0xffff >> (16 - BitCount))) << bitsRead;
                    mCurrentBit = BitCount + mCurrentBit;
                    BitCount = 0;
                }
                else //Read all bits left in this byte
                {
                    int bitsToRead = 8 - mCurrentBit;
                    result |= (((int)bytes[mCurrentByte] >> mCurrentBit) & (0xffff >> (16 - bitsToRead))) << bitsRead;
                    bitsRead += bitsToRead;
                    mCurrentByte++;
                    mCurrentBit = 0;
                    BitCount -= bitsToRead;
                }
            }
            
            return result;
        }

        //public int PeekByte()
        //{
        //   // if (EnsureBits(8) == false) return -1;
        //   // return mCurrent & 0xff;
        //}
        
        //public void EnsureBits()
        //{
        //}
        
        //private bool WasteBits(int BitCount)
        //{
        //    mCurrent >>= BitCount;
        //    mBitCount -= BitCount;
        //    return true;
        //}
        
        public void Reset()
        {
            mStream.Seek(0, SeekOrigin.Begin);
            mCurrentByte = 0;
            mCurrentBit = 0;
        }
    }
    
    //This should move to another file
    // [TestFixture]
    // public class BitStreamTest
    // {
        // [Test]
        // public void TestStream()
        // {
            // MemoryStream stream = new MemoryStream();
            // stream.WriteByte(0x5C);
            // stream.WriteByte(0x96);
            // stream.WriteByte(0xEF);
            // stream.Seek(0, SeekOrigin.Begin);
            // BitStream bs = new BitStream(stream);
            // Assert.AreEqual(bs.ReadBits(5), 0x1C);
            // Assert.AreEqual(bs.ReadBits(6), 0x32);
            // Assert.AreEqual(bs.ReadBits(7), 0x72);

        // }
    // }
}
