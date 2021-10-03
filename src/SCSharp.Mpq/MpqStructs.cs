//
// MpqStructs.cs
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
using System.IO;

namespace MpqReader
{
	enum MpqFileFlags : uint
	{
		Changed = 1,
		Protected = 2,
		CompressedPK = 0x100,
		CompressedMulti = 0x200,
		Compressed = 0xff00,
		Encrypted = 0x10000,
		FixSeed = 0x20000,
		SingleUnit = 0x1000000,
		Exists = 0x80000000
	}

	struct MpqHeader
	{
		public uint ID; // Signature.  Should be 0x1a51504d
		public uint DataOffset; // Offset of the first file
		public uint ArchiveSize;
		public ushort Offs0C; // Unknown
		public ushort BlockSize; // Size of file block is 0x200 << BlockSize
		public uint HashTablePos;
		public uint BlockTablePos;
		public uint HashTableSize;
		public uint BlockTableSize;
		
		public static readonly uint MpqId = 0x1a51504d;
		public static readonly uint Size = 32;
		
		public MpqHeader(BinaryReader br)
		{
			ID = br.ReadUInt32();
			DataOffset = br.ReadUInt32();
			ArchiveSize = br.ReadUInt32();
			Offs0C = br.ReadUInt16();
			BlockSize = br.ReadUInt16();
			HashTablePos = br.ReadUInt32();
			BlockTablePos = br.ReadUInt32();
			HashTableSize = br.ReadUInt32();
			BlockTableSize = br.ReadUInt32();
		}
	}
	
	struct MpqHash
	{
		public uint Name1;
		public uint Name2;
		public uint Locale;
		public uint BlockIndex;
		
		public static readonly uint Size = 16;

		public MpqHash(BinaryReader br)
		{
			Name1 = br.ReadUInt32();
			Name2 = br.ReadUInt32();
			Locale = br.ReadUInt32();
			BlockIndex = br.ReadUInt32();
		}

		public bool IsValid
		{
			get
			{
				return Name1 != uint.MaxValue && Name2 != uint.MaxValue;
			}
		}
	}
	
	struct MpqBlock
	{
		public uint FilePos;
		public uint CompressedSize;
		public uint FileSize;
		public MpqFileFlags Flags;
		
		public static readonly uint Size = 16;
		
		public MpqBlock(BinaryReader br, uint HeaderOffset)
		{
			FilePos = br.ReadUInt32() + HeaderOffset;
			CompressedSize = br.ReadUInt32();
			FileSize = br.ReadUInt32();
			Flags = (MpqFileFlags)br.ReadUInt32();
		}
		
		public bool IsEncrypted
		{
			get
			{
				return (Flags & MpqFileFlags.Encrypted) != 0;
			}
		}
		
		public bool IsCompressed
		{
			get
			{
				return (Flags & MpqFileFlags.Compressed) != 0;
			}
		}
	}
}
