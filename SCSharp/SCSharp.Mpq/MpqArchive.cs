using System;
using System.Collections;
using System.IO;

namespace MpqReader
{
	public class MpqArchive
	{
		private Stream mStream;
		private string mFilename;

		private MpqHeader mHeader;
		private long mHeaderOffset;
		private int mBlockSize;
		private MpqHash[] mHashes;
		private MpqBlock[] mBlocks;
		
		private static uint[] sStormBuffer;
		
		static MpqArchive()
		{
			sStormBuffer = BuildStormBuffer();
		}

		public MpqArchive(string Filename)
		{
			mFilename = Filename;
			mStream = File.Open(Filename, FileMode.Open, FileAccess.Read);
			Init();
		}
		
		public MpqArchive(Stream SourceStream)
		{
			mStream = SourceStream;
			Init();
		}

		private void Init()
		{
			if (LocateMpqHeader() == false) 
				throw new Exception("Unable to find MPQ header");

			BinaryReader br = new BinaryReader(mStream);

			mBlockSize = 0x200 << mHeader.BlockSize;

			// Read hash table
			mStream.Seek(mHeader.HashTablePos, SeekOrigin.Begin);
			// read header.HashTableSize instances of MpqHash
			byte[] hashdata = br.ReadBytes((int)(mHeader.HashTableSize * MpqHash.Size));
			// then decrypt
			DecryptTable(hashdata, "(hash table)");

			// then create another binaryreader and load the Hash instances
			BinaryReader br2 = new BinaryReader(new MemoryStream(hashdata));

			mHashes = new MpqHash[mHeader.HashTableSize];

			for (int i = 0; i < mHeader.HashTableSize; i++)
				mHashes[i] = new MpqHash(br2);

			// Load block table
			mStream.Seek(mHeader.BlockTablePos, SeekOrigin.Begin);
			byte[] blockdata = br.ReadBytes((int)(mHeader.BlockTableSize * MpqBlock.Size));
			
			DecryptTable(blockdata, "(block table)");

			br2 = new BinaryReader(new MemoryStream(blockdata));
			mBlocks = new MpqBlock[mHeader.BlockTableSize];
			for (int i = 0; i < mHeader.BlockTableSize; i++)
				mBlocks[i] = new MpqBlock(br2, (uint)mHeaderOffset);
		}
		
		private bool LocateMpqHeader()
		{
			BinaryReader br = new BinaryReader(mStream);

			// In .mpq files the header will be at the start of the file
			// In .exe files, it will be at a multiple of 0x200
			for (long i = 0; i < mStream.Length; i += 0x200)
			{
				mStream.Seek(i, SeekOrigin.Begin);
				mHeader = new MpqHeader(br);

				if (mHeader.ID == MpqHeader.MpqId)
				{
					mHeaderOffset = i;
					mHeader.HashTablePos += (uint)mHeaderOffset;
					mHeader.BlockTablePos += (uint)mHeaderOffset;
					if (mHeader.DataOffset == 0x6d9e4b86)
					{
						// then this is a protected archive
						mHeader.DataOffset = (uint)(MpqHeader.Size + i);
					}
					return true;
				}
			}
			return false;
		}
		
		public MpqStream OpenFile(string Filename)
		{
			MpqHash hash;
			MpqBlock block;

			hash = GetHashEntry(Filename);
			uint blockindex = hash.BlockIndex;
			
			if (blockindex == uint.MaxValue) 
				throw new FileNotFoundException("File not found: " + Filename);
			
			block = mBlocks[blockindex];

			return new MpqStream(this, block);
		}
		
		internal Stream BaseStream
		{ get { return mStream; } }
		
		internal int BlockSize
		{ get { return mBlockSize; } }

		private MpqHash GetHashEntry(string Filename)
		{
			uint index = HashString(Filename, 0);
			index  &= mHeader.HashTableSize - 1;
			uint name1 = HashString(Filename, 0x100);
			uint name2 = HashString(Filename, 0x200);
			
			for(uint i = index; i < mHashes.Length; ++i)
			{
				MpqHash hash = mHashes[i];
				if (hash.Name1 == name1 && hash.Name2 == name2) return hash;
			}

			MpqHash nullhash = new MpqHash();
			nullhash.BlockIndex = uint.MaxValue;
			return nullhash;
		}

		internal static uint HashString(string Input, int Offset)
		{
			uint seed1 = 0x7fed7fed;
			uint seed2 = 0xeeeeeeee;
			
			foreach(char c in Input)
			{
				int val = (int)char.ToUpper(c);
				seed1 = sStormBuffer[Offset + val] ^ (seed1 + seed2);
				seed2 = (uint)val + seed1 + seed2 + (seed2 << 5) + 3;
			}
			return seed1;
		}
		
		// Used for Hash Tables and Block Tables
		internal static void DecryptTable(byte[] Data, string Key)
		{
			DecryptBlock(Data, HashString(Key, 0x300));
		}

		internal static void DecryptBlock(byte[] Data, uint Seed1)
		{
			uint seed2 = 0xeeeeeeee;

			// NB: If the block is not an even multiple of 4,
			// the remainder is not encrypted
			for (int i = 0; i < Data.Length - 3; i += 4)
			{
				seed2 += sStormBuffer[0x400 + (Seed1 & 0xff)];

				uint result = BitConverter.ToUInt32(Data, i);

				result ^= (Seed1 + seed2);

				Seed1 = ((~Seed1 << 21) + 0x11111111) | (Seed1 >> 11);
				seed2 = result + seed2 + (seed2 << 5) + 3;
				byte[] bytes = BitConverter.GetBytes(result);
				Array.Copy(bytes, 0, Data, i, 4);
			}
		}
		
		internal static void DecryptBlock(uint[] Data, uint Seed1)
		{
			uint seed2 = 0xeeeeeeee;

			for (int i = 0; i < Data.Length; i++)
			{
				seed2 += sStormBuffer[0x400 + (Seed1 & 0xff)];
				uint result = Data[i];
				result ^= Seed1 + seed2;
				
				Seed1 = ((~Seed1 << 21) + 0x11111111) | (Seed1 >> 11);
				seed2 = result + seed2 + (seed2 << 5) + 3;
				Data[i] = result;
			}
		}

		// This function calculates the encryption key based on
		// some assumptions we can make about the headers for encrypted files
		internal static uint DetectFileSeed(uint[] Data, uint Decrypted)
		{
			uint value0 = Data[0];
			uint value1 = Data[1];
			uint temp = (value0 ^ Decrypted) - 0xeeeeeeee;
			
			for (int i = 0; i < 0x100; i++)
			{
				uint seed1 = temp - sStormBuffer[0x400 + i];
				uint seed2 = 0xeeeeeeee + sStormBuffer[0x400 + (seed1 & 0xff)];
				uint result = value0 ^ (seed1 + seed2);

				if (result != Decrypted) continue;

				uint saveseed1 = seed1;
				
				// Test this result against the 2nd value
				seed1 = ((~seed1 << 21) + 0x11111111) | (seed1 >> 11);
				seed2 = result + seed2 + (seed2 << 5) + 3;
				
				seed2 += sStormBuffer[0x400 + (seed1 & 0xff)];
				result = value1 ^ (seed1 + seed2);
				
				if ((result & 0xffff0000) == 0)
					return saveseed1;
			}
			return 0;
		}
	
		private static uint[] BuildStormBuffer()
		{
			uint seed = 0x100001;
			
			uint[] result = new uint[0x500];
			
			for(uint index1 = 0; index1 < 0x100; index1++)
			{
				uint index2 = index1;
				for(int i = 0; i < 5; i++, index2 += 0x100)
				{
					seed = (seed * 125 + 3) % 0x2aaaab;
					uint temp = (seed & 0xffff) << 16;
					seed = (seed * 125 + 3) % 0x2aaaab;

					result[index2]  = temp | (seed & 0xffff);
				}
			}

			return result;
		}
	}
}
