//
// BitStream.cs
//
// Authors:
//		Foole (fooleau@gmail.com)
//
// (C) 2006 Foole (fooleau@gmail.com)
// Based on code from StormLib by Ladislav Zezula
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

namespace MpqReader
{
	/// <summary>
	/// A utility class for reading groups of bits from a stream
	/// </summary>
	internal class BitStream
	{
		private Stream mStream;
		private int mCurrent;
		private int mBitCount;

		public BitStream(Stream SourceStream)
		{
			mStream = SourceStream;
		}
		
		public Stream BaseStream
		{ get { return mStream; } }

		public int ReadBits(int BitCount)
		{
			if (BitCount > 16)
				throw new ArgumentOutOfRangeException("BitCount", "Maximum BitCount is 16");
			if (EnsureBits(BitCount) == false) return -1;
			int result = mCurrent & (0xffff >> (16 - BitCount));
			WasteBits(BitCount);
			return result;
		}

		public int PeekByte()
		{
			if (EnsureBits(8) == false) return -1;
			return mCurrent & 0xff;
		}

		public bool EnsureBits(int BitCount)
		{
			if (BitCount <= mBitCount) return true;
			
			if (mStream.Position >= mStream.Length) return false;
			int nextvalue = mStream.ReadByte();
			mCurrent |= nextvalue << mBitCount;
			mBitCount += 8;
			return true;
		}
		
		private bool WasteBits(int BitCount)
		{
			mCurrent >>= BitCount;
			mBitCount -= BitCount;
			return true;
		}
	}
}
