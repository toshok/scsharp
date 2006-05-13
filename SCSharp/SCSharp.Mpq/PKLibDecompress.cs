// Copyright 2006 Foole (fooleau@gmail.com)
using System;
using System.IO;

namespace MpqReader
{
	public enum CompressionType
	{
		Binary = 0,
		Ascii = 1,
	}
	// The functionality of this class could be implemented as a single public static function
	// to do this, compression type would need to be passed to DecodeLit and dictionary size 
	// would need to be passed to DecodeDist
	// The input stream would need to be passed to WasteBits, DecodeDist and DecodeLit

	/// <summary>
	/// A decompressor for PKLib implode/explode
	/// </summary>
	public class PKLibDecompress
	{
		private Stream mInput;

		private CompressionType mCType;

		private int mDSizeBits;	// Dictionary size in bits
		private int mDSizeMask;

		private UInt32 mBitBuff;
		private int mExtraBits;
		
		private static byte[] sPosition1;
		private static byte[] sPosition2;

		private static readonly byte[] sLenBits =
		{
			3, 2, 3, 3, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 7, 7
		};

		private static readonly byte[] sLenCode =
		{
			5, 3, 1, 6, 10, 2, 12, 20, 4, 24, 8, 48, 16, 32, 64, 0
		};
		
		private static readonly byte[] sExLenBits = 
		{
			0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8
		};

		private static readonly UInt16[] sLenBase =
		{
			0x0000, 0x0001, 0x0002, 0x0003, 0x0004, 0x0005, 0x0006, 0x0007,
			0x0008, 0x000A, 0x000E, 0x0016, 0x0026, 0x0046, 0x0086, 0x0106
		};

		private static readonly byte[] sDistBits =
		{
			2, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8
		};

		private static readonly byte[] sDistCode =
		{
		    0x03, 0x0D, 0x05, 0x19, 0x09, 0x11, 0x01, 0x3E, 0x1E, 0x2E, 0x0E, 0x36, 0x16, 0x26, 0x06, 0x3A,
		    0x1A, 0x2A, 0x0A, 0x32, 0x12, 0x22, 0x42, 0x02, 0x7C, 0x3C, 0x5C, 0x1C, 0x6C, 0x2C, 0x4C, 0x0C,
		    0x74, 0x34, 0x54, 0x14, 0x64, 0x24, 0x44, 0x04, 0x78, 0x38, 0x58, 0x18, 0x68, 0x28, 0x48, 0x08,
		    0xF0, 0x70, 0xB0, 0x30, 0xD0, 0x50, 0x90, 0x10, 0xE0, 0x60, 0xA0, 0x20, 0xC0, 0x40, 0x80, 0x00
		};
		
		static PKLibDecompress()
		{
			sPosition1 = GenerateDecodeTable(sDistBits, sDistCode);
			sPosition2 = GenerateDecodeTable(sLenBits, sLenCode);
		}

		public PKLibDecompress(Stream Input)
		{
			mInput = Input;

			mCType = (CompressionType)mInput.ReadByte();
			if (mCType != CompressionType.Binary && mCType != CompressionType.Ascii)
				throw new Exception("Invalid compression type: " + mCType);

			mDSizeBits = mInput.ReadByte();
			// This is 6 in test cases
			if(4 > mDSizeBits || mDSizeBits > 6)
				throw new Exception("Invalid dictionary size: " + mDSizeBits);
			mDSizeMask = 0xFFFF >> (0x10 - mDSizeBits);

			mBitBuff = (UInt32)mInput.ReadByte();
			mExtraBits = 0;
		}

		public byte[] Explode(int ExpectedSize)
		{
			//if (mCType == CompressionType.Ascii) GenAscTabs();

			byte[] outputbuffer = new byte[ExpectedSize];
			Stream outputstream = new MemoryStream(outputbuffer);
			BinaryWriter outputwriter = new BinaryWriter(outputstream);

			UInt32 instruction;
			while((instruction = DecodeLit()) < 0x305)
			{
				if(instruction < 0x100)
				{
					outputstream.WriteByte((byte)instruction);
				} else
				{
					// If instruction is greater than 0x100, means "Repeat n - 0xFE bytes"
					UInt32 copylength = instruction - 0xFE;
					int moveback = DecodeDist(copylength);
					if (moveback == 0) break;

					int source = (int)outputstream.Position - moveback;
					for (int i = 0; i < copylength; i++)
						outputstream.WriteByte(outputbuffer[source++]);
				}
			}
			if (outputstream.Position != ExpectedSize)
				throw new Exception(String.Format("Decompressed to {0}, but was expecting {1}", outputstream.Position, ExpectedSize));
			
			return outputbuffer;
		}

		private static byte[] GenerateDecodeTable(byte[] Bits, byte[] Codes)
		{
			byte[] result = new byte[256];

			for(int i = Bits.Length - 1; i >= 0; i--)
			{
				UInt32 idx1 = Codes[i];
				UInt32 idx2 = (UInt32)1 << Bits[i];

				do
				{
					result[idx1] = (byte)i;
					idx1         += idx2;
				} while(idx1 < 0x100);
			}
			return result;
		}

		// Return values:
		// 0x000 - 0x0FF : One byte from compressed file.
		// 0x100 - 0x305 : Copy previous block (0x100 = 1 byte)
		// 0x306         : EOF
		private UInt32 DecodeLit()
		{
			UInt32 value;
			
			if ((mBitBuff & 1) != 0)
			{
				// Skip current bit in the buffer
				if(WasteBits(1)) return 0x306;

				// The next bits are position in buffers
				value = sPosition2[(byte)mBitBuff];

				// Get number of bits to skip
				if(WasteBits(sLenBits[value])) return 0x306;

				int nbits = sExLenBits[value];
				if(nbits != 0)
				{
					UInt32 val2 = (UInt32)(mBitBuff & ((1 << nbits) - 1));

					if(WasteBits(nbits))
					{
						if((value + val2) != 0x10E) return 0x306;
					}
					value = sLenBase[value] + val2;
				}
				return value + 0x100; // Return number of bytes to repeat
			} else
			{
				if(WasteBits(1)) return 0x306;

				if (mCType == CompressionType.Binary)
				{
					value = (byte)mBitBuff;
					if(WasteBits(8)) return 0x306;
					return value;
				}
				// TODO: Text mode
				throw new NotImplementedException("Text mode is not yet implemented");
			}
		}

		// Returns true if there is no more data left
		private bool WasteBits(int BitCount)
		{
			if (BitCount <= mExtraBits)
			{
				mExtraBits -= BitCount;
				mBitBuff >>= BitCount;
				return false;
			}
			if (mInput.Position >= mInput.Length)
				return true;

			int next = mInput.ReadByte();
			mBitBuff |= (UInt32)next << (8 + mExtraBits);
			mBitBuff >>= BitCount;
			mExtraBits += 8 - BitCount;
			return false;
		}

		private int DecodeDist(UInt32 Length)
		{
			int pos = sPosition1[mBitBuff & 0xFF];
			byte skip = sDistBits[pos];     // Number of bits to skip

			// Skip the appropriate number of bits
			if (WasteBits(skip)) return 0;

			if(Length == 2)
			{
				pos = ((int)(pos << 2)) | ((int)mBitBuff & 0x03);
				if(WasteBits(2)) return 0;
			} else
			{
				pos = ((int)(pos << mDSizeBits)) | ((int)mBitBuff & mDSizeMask);
				if(WasteBits(mDSizeBits)) return 0;
			}

			return pos+1;
		}
	}
}
