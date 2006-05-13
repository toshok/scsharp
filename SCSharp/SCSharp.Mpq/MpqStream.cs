// Copyright 2006 Foole (fooleau@gmail.com)
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams; // Zlib

namespace MpqReader
{
	/// <summary>
	/// A Stream based class for reading a file from an MPQ file
	/// </summary>
	public class MpqStream : Stream
	{
		private Stream mStream;
		private int mBlockSize;

		private MpqBlock mBlock;
		private uint[] mBlockPositions;
		private uint mSeed1;
		
		private long mPosition;
		private byte[] mCurrentData;
		private int mCurrentBlockIndex = -1;

		private MpqStream()
		{}

		internal MpqStream(MpqArchive File, MpqBlock Block)
		{
			mBlock = Block;
			
			mStream = File.BaseStream;
			mBlockSize = File.BlockSize;

			if (mBlock.IsCompressed) LoadBlockPositions();
		}

		// Compressed files start with an array of offsets to make seeking possible
		private void LoadBlockPositions()
		{
			int blockposcount = (int)((mBlock.FileSize + mBlockSize - 1) / mBlockSize) + 1;

			mBlockPositions = new uint[blockposcount];

			lock(mStream)
			{
				mStream.Seek(mBlock.FilePos, SeekOrigin.Begin);
				BinaryReader br = new BinaryReader(mStream);
				for (int i = 0; i < blockposcount; i++)
					mBlockPositions[i] = br.ReadUInt32();
			}

			uint blockpossize = (uint)blockposcount * 4;

			// StormLib takes this to mean the data is encrypted
			if (mBlockPositions[0] != blockpossize)
			{
				if (mSeed1 == 0)
				{
					mSeed1 = MpqArchive.DetectFileSeed(mBlockPositions, blockpossize);
					if (mSeed1 == 0)
						throw new Exception("Unable to determine encyption seed");
				}
				MpqArchive.DecryptBlock(mBlockPositions, mSeed1);
				mSeed1++; // Add 1 because the first block is the offset list
			}
		}

		private byte[] LoadBlock(int BlockIndex, int ExpectedLength)
		{
			uint offset;
			int toread;

			if (mBlock.IsCompressed)
			{
				offset = mBlockPositions[BlockIndex];
				toread = (int)(mBlockPositions[BlockIndex + 1] - offset);
			} else
			{
				offset = (uint)(BlockIndex * mBlockSize);
				toread = ExpectedLength;
			}
			offset += mBlock.FilePos;

			byte[] data = new byte[toread];
			lock(mStream)
			{
				mStream.Seek(offset, SeekOrigin.Begin);
				mStream.Read(data, 0, toread);
			}

			if (mBlock.IsEncrypted)
				MpqArchive.DecryptBlock(data, (uint)(mSeed1 + BlockIndex));

			if (mBlock.IsCompressed)
			{
				if ((mBlock.Flags & MpqFileFlags.CompressedMulti) != 0)
					data = DecompressMulti(data, ExpectedLength);
				else
					data = PKDecompress(new MemoryStream(data), ExpectedLength);
			}

			return data;
		}

#region Stream overrides
		public override bool CanRead
		{ get { return true; } }
		
		public override bool CanSeek
		{ get { return true; } }

		public override bool CanWrite
		{ get { return false; } }
		
		public override long Length
		{ get { return mBlock.FileSize; } }
		
		public override long Position
		{
			get 
			{
				return mPosition; 
			}
			set 
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("Attempt to set the position to a negative value");
				Seek(value, SeekOrigin.Begin);
			}
		}
		
		public override void Flush()
		{
			// NOP
		}
		
		public override long Seek(long Offset, SeekOrigin Origin)
		{
			long target;
			
			switch (Origin)
			{
				case SeekOrigin.Begin:
					target = Offset;
					break;
				case SeekOrigin.Current:
					target = Position + Offset;
					break;
				case SeekOrigin.End:
					target = Length + Offset;
					break;
				default:
					throw new ArgumentException("Origin", "Invalid SeekOrigin");
			}

			if (target < 0)
				throw new ArgumentOutOfRangeException("Attmpted to Seek before the beginning of the stream");
			if (target >= Length)
				throw new ArgumentOutOfRangeException("Attmpted to Seek beyond the end of the stream");
			
			mPosition = target;

			return mPosition;
		}

		public override void SetLength(long Value)
		{
			throw new NotSupportedException("SetLength is not supported");
		}
		
		public override int Read(byte[] Buffer, int Offset, int Count)
		{
			BufferData();

			int localposition = (int)(mPosition % mBlockSize);
			int bytestocopy = Math.Min(mBlockSize - localposition, Count);
			Array.Copy(mCurrentData, localposition, Buffer, Offset, bytestocopy);

			mPosition += bytestocopy;
			return bytestocopy;
		}
		
		public override int ReadByte()
		{
			if (mPosition >= Length) return -1;

			BufferData();
			
			int localposition = (int)(mPosition % mBlockSize);
			mPosition++;
			return mCurrentData[localposition];
		}
		
		private void BufferData()
		{
			int requiredblock = (int)(mPosition / mBlockSize);
			if (requiredblock != mCurrentBlockIndex)
			{
				int expectedlength = (int)Math.Min(Length - (requiredblock * mBlockSize), mBlockSize);
				mCurrentData = LoadBlock(requiredblock, expectedlength);
				mCurrentBlockIndex = requiredblock;
			}
		}

		public override void Write(byte[] Buffer, int Offset, int Count)
		{
			throw new NotSupportedException("Writing is not supported");
		}
#endregion Strem overrides

		private static byte[] DecompressMulti(byte[] Input, int OutputLength)
		{
			Stream sinput = new MemoryStream(Input);

			byte comptype = (byte)sinput.ReadByte();

			// These 3 cases are the most common
			// only 8 and 81 appear to be used by Starcraft
			if (comptype == 2) // WoW/WC3
			{
				return ZlibDecompress(sinput, OutputLength);
			} else if (comptype == 8) // SC
			{
				return PKDecompress(sinput, OutputLength);
			} else if (comptype == 0x81 || comptype == 0x41) // SC WAV
			{
				// TODO: Wave file compression
				// 1 = Huffmann compression
				// 80 = IMA ADPCM stereo compression
				// 40 = IMA ADPCM mono compression
				throw new Exception(String.Format("Unhandled multi compression: {0:X}", comptype));
			} else
			{
				throw new Exception(String.Format("Unhandled multi compression: {0:X}", comptype));
			}
		}

		private static byte[] PKDecompress(Stream Data, int ExpectedLength)
		{
			PKLibDecompress pk = new PKLibDecompress(Data);
			return pk.Explode(ExpectedLength);
		}

		private static byte[] ZlibDecompress(Stream Data, int ExpectedLength)
		{
			byte[] Output = new byte[ExpectedLength];
			Stream s = new InflaterInputStream(Data);
			int Offset = 0;
			while(true)
			{
				int size = s.Read(Output, Offset, ExpectedLength);
				if (size == ExpectedLength) break;
				Offset += size;
				ExpectedLength -= size;
			}
			return Output;
		}
	}
}
