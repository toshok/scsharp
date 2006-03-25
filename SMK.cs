/* written using

  http://wiki.multimedia.cx/index.php?title=Smacker

   as a guide
*/

using System;
using System.IO;
using System.Text;

using Gtk;
using Gdk;

namespace Starcraft {

	public class SMK : MPQResource {

		[Flags]
		enum ARFlags {
			Compressed = 0x80,
			DataPresent = 0x40,
			Is16Bit = 0x20,
			Stereo = 0x10,

			CompressionMask = 0x0c
		}

		const int V2_COMPRESSION = 0x00;

		Stream stream;

		/* SMK header info */
		uint width;
		uint height;
		uint frames;
		uint frameRate;
		uint flags;
		uint[/*7*/] audioSize;
		uint treesSize;
		uint mmap_Size;
		uint mclr_Size;
		uint full_Size;
		uint type_Size;
		uint[/*7*/] audioRate;
		uint[/*7*/] audioFlags;

		uint[] frameSizes;
		byte[] frameFlags;

		Dynamic16HuffmanTree mmap_Tree;
		Dynamic16HuffmanTree mclr_Tree;
		Dynamic16HuffmanTree full_Tree;
		Dynamic16HuffmanTree type_Tree;


		public SMK ()
		{
			palette = new byte[256 * 3];
		}

		public uint Width {
			get { return width; }
		}

		public uint PaddedWidth {
			get { return width + (width % 4); }
		}

		public uint Height { 
			get { return height; }
		}

		public uint PaddedHeight {
			get { return height + (height % 4); }
		}

		public uint[] AudioRate {
			get { return audioRate; }
		}

		void ReadHeader ()
		{
			int i;
			uint signature;

			audioSize = new uint[7];
			audioRate = new uint[7];
			audioFlags = new uint[7];

			signature = Util.ReadDWord (stream);
			if (signature != 0x324b4d53 /* SMK2 */
			    && signature != 0x344b4d53 /* SMK4 */)
				throw new Exception ("invalid file");
			width = Util.ReadDWord (stream);
			height = Util.ReadDWord (stream);
			frames = Util.ReadDWord (stream);
			frameRate = Util.ReadDWord (stream);
			flags = Util.ReadDWord (stream);
			for (i = 0; i < audioSize.Length; i ++)
				audioSize[i] = Util.ReadDWord (stream);
			treesSize = Util.ReadDWord (stream);
			mmap_Size = Util.ReadDWord (stream);
			mclr_Size = Util.ReadDWord (stream);
			full_Size = Util.ReadDWord (stream);
			type_Size = Util.ReadDWord (stream);
			for (i = 0; i < audioRate.Length; i ++) {
				audioRate[i] = Util.ReadDWord (stream);
				audioFlags[i] = audioRate[i] >> 24;
				audioRate[i] &= 0x00ffffff;
			}
			/* dummy = */ Util.ReadDWord (stream);
		}

		double CalcFps ()
		{
			if ((int)frameRate > 0)
				return 1000.0 / (int)frameRate;
			else if ((int)frameRate < 0)
				return 100000.0 / (-(int)frameRate);
			else
				return 10.0;
		}

		void ReadFrameSizes ()
		{
			int i;
			frameSizes = new uint[frames];
			for (i = 0; i < frames; i ++) {
				frameSizes[i] = Util.ReadDWord (stream);
				if ((frameSizes[i] & 0x03) == 0x03)
					Console.WriteLine (" frame {0} has some weird flags", i);
			}
		}

		void ReadFrameFlags ()
		{
			frameFlags = new byte[frames];
			stream.Read (frameFlags, 0, (int)frames);
		}

		void ReadTrees ()
		{
			Console.WriteLine ("Building huffman trees (from position {0:x}, length {1:x}):", stream.Position, treesSize);
			byte[] tree_buf = new byte[treesSize];
			stream.Read (tree_buf, 0, (int)treesSize);

			BitStream bs = new BitStream (tree_buf);

			int tag;

			tag = bs.ReadBit ();
			if (tag == 1) {
				Console.WriteLine (" + MMap");
				mmap_Tree = new Dynamic16HuffmanTree ();
				mmap_Tree.Build (bs);
				Console.WriteLine (" + After MMap tree, read {0} bytes", bs.ReadCount);
			}

			bs.ZeroReadCount ();
			tag = bs.ReadBit ();
			if (tag == 1) {
				Console.WriteLine (" + MClr");
				mclr_Tree = new Dynamic16HuffmanTree ();
				mclr_Tree.Build (bs);
				Console.WriteLine (" + After MClr tree, read {0} bytes", bs.ReadCount);
			}

			bs.ZeroReadCount ();
			tag = bs.ReadBit ();
			if (tag == 1) {
				Console.WriteLine (" + Full");
				full_Tree = new Dynamic16HuffmanTree ();
				full_Tree.Build (bs);
				Console.WriteLine (" + After Full tree, read {0} bytes", bs.ReadCount);
			}

			bs.ZeroReadCount ();
			tag = bs.ReadBit ();
			if (tag == 1) {
				Console.WriteLine (" + Type");
				type_Tree = new Dynamic16HuffmanTree ();
				type_Tree.Build (bs);
				Console.WriteLine (" + After Type tree, read {0} bytes", bs.ReadCount);
			}

			Console.WriteLine ("done (read {0} out of {1} bytes).", bs.ReadCount, treesSize);
		}

		byte[] palmap = {
			0x00, 0x04, 0x08, 0x0C, 0x10, 0x14, 0x18, 0x1C,
			0x20, 0x24, 0x28, 0x2C, 0x30, 0x34, 0x38, 0x3C,
			0x41, 0x45, 0x49, 0x4D, 0x51, 0x55, 0x59, 0x5D,
			0x61, 0x65, 0x69, 0x6D, 0x71, 0x75, 0x79, 0x7D,
			0x82, 0x86, 0x8A, 0x8E, 0x92, 0x96, 0x9A, 0x9E,
			0xA2, 0xA6, 0xAA, 0xAE, 0xB2, 0xB6, 0xBA, 0xBE,
			0xC3, 0xC7, 0xCB, 0xCF, 0xD3, 0xD7, 0xDB, 0xDF,
			0xE3, 0xE7, 0xEB, 0xEF, 0xF3, 0xF7, 0xFB, 0xFF
		};

		void ReadPaletteChunk (BitStream bs)
		{
			byte[] new_palette = new byte[256 * 3];

			bs.AssertAtByteBoundary ();

			bs.ZeroReadCount ();

			int paletteLength = bs.ReadByte();
			Console.WriteLine ("palette chunk is {0} bytes long", paletteLength * 4);

			//			byte[] palBuf = new byte[paletteLength * 4 - 1];
			//			bs.Read (palBuf, 0, palBuf.Length);
			//			MemoryStream palStream = new MemoryStream (palBuf, false);

			BitStream palStream = bs;
			int new_index = 0, index = 0;
			while (new_index < 256) {
				byte p = (byte)palStream.ReadByte();
				if ((p & 0x80) == 0x80) {
					int count = (p & 0x7f) + 1;
					//					Console.WriteLine ("copy1 {0} entries, from {1} to {2}", count, index, new_index);
					Array.Copy (palette, index * 3, new_palette, new_index * 3, count * 3);
					index += count;
					new_index += count;
				}
				else if ((p & 0x40) == 0x40) {
					int count = (p & 0x3f) + 1;
					int tmp_index = palStream.ReadByte();
					//					Console.WriteLine ("copy2 {0} entries, from {1} to {2}", count, tmp_index, new_index);
					Array.Copy (palette, tmp_index * 3, new_palette, new_index * 3, count * 3);
					new_index += count;
				}
				else if ((p & 0xc0) == 0x00) {
					int b = p & 0x3f;
					int g = palStream.ReadByte() & 0x3f;
					int r = palStream.ReadByte() & 0x3f;
					new_palette [new_index * 3] = palmap[r];
					new_palette [new_index * 3 + 1] = palmap[g];
					new_palette [new_index * 3 + 2] = palmap[b];
					//					Console.WriteLine ("Assign palette[{0}] = {1},{2},{3}", new_index,
					//							   palmap[r], palmap[g], palmap[b]);
					new_index ++;
				}
			}

			palette = new_palette;
			//			Console.WriteLine ("read {0} bytes toward filling the palette", palBuf.Length + 1);
		}

                uint[] block_chain_size_table = {
			1,    2,    3,    4,    5,    6,    7,    8,
			9,   10,   11,   12,   13,   14,   15,   16,
			17,   18,   19,   20,   21,   22,   23,   24,
			25,   26,   27,   28,   29,   30,   31,   32,
			33,   34,   35,   36,   37,   38,   39,   40,
			41,   42,   43,   44,   45,   46,   47,   48,
			49,   50,   51,   52,   53,   54,   55,   56,
			57,   58,   59,  128,  256,  512, 1024, 2048
		};

		enum BlockType {
			Mono = 0x00,
			Full = 0x01,
			Void = 0x02,
			Solid = 0x03
		}

		void ReadVideoChunk (BitStream bs)
		{
			int current_block_x = 0, current_block_y = 0;
			int block_rez_x = (int)PaddedWidth / 4;
			int block_rez_y = (int)PaddedHeight / 4;

			Console.WriteLine ("reading video chunk");
			bs.DebugFirst ();

#if false
			while (true) {
				ushort type_descriptor;
				type_descriptor = (ushort)type_Tree.Decode (bs);
				BlockType type = (BlockType)(type_descriptor & 0x3);
				uint chain_length = block_chain_size_table [(type_descriptor & 0xfc) >> 2];

				for (int i = 0; i < chain_length; i ++) {
					switch (type) {
					case BlockType.Mono:
						ushort pixel = (ushort) mclr_Tree.Decode (bs);
						byte color1 = (byte)((pixel >> 8) & 0xff);
						byte color0 = (byte)(pixel & 0xff);
						ushort map = (ushort) mmap_Tree.Decode (bs);
						int mask = 1;
						int bx, by;
						bx = current_block_x * 4;
						by = current_block_y * 4;
						for (int bi = 0; bi < 16;) {
							if ((map & mask) == 0)
								frameData[by * PaddedWidth + bx] = color0;
							else
								frameData[by * PaddedWidth + bx] = color1;
							mask <<= 1;
							bx ++;
							bi++;
							if ((bi % 4) == 0) {
								bx = current_block_x * 4;
								by ++;
							}
						}
						break;
					case BlockType.Full:
						break;
					case BlockType.Void:
						break;
					case BlockType.Solid:
						break;
					}
				}
				current_block_x ++;
				if (current_block_x == block_rez_x) {
					current_block_x = 0;
					current_block_y ++;
					if (current_block_y == block_rez_y)
						return;
				}
			}
#endif
		}

		byte[][] audioOut = new byte[7][];

		void OutputSample (Stream outputStream, byte[] left, byte[] right)
		{
			OutputSample (outputStream, left);
			OutputSample (outputStream, right);
		}

		void OutputSample (Stream outputStream, byte[] sample)
		{
			outputStream.Write (sample, 0, sample.Length);
		}

		void OutputSample (Stream outputStream, byte left, byte right)
		{
			outputStream.WriteByte (left);
			outputStream.WriteByte (right);
		}

		void OutputSample (Stream outputStream, byte sample)
		{
			outputStream.WriteByte (sample);
		}

		void OutputSample (Stream outputStream, short left, short right)
		{
#if true
			byte[] lb = BitConverter.GetBytes (left);
			byte[] rb = BitConverter.GetBytes (right);

			OutputSample (outputStream, lb);
			OutputSample (outputStream, rb);
#else
			Util.WriteWord (outputStream, (ushort)left);
			Util.WriteWord (outputStream, (ushort)right);
#endif
		}

		void OutputAudio (Stream outputStream, byte[] buf, int index, int length)
		{
			outputStream.Write (buf, index, length);
		}

		void OutputAudio (Stream outputStream, byte[] buf)
		{
			outputStream.Write (buf, 0, buf.Length);
		}

		void Decompress16Stereo (BitStream bs, Stream outputStream)
		{
			int flag;

			flag = bs.ReadBit(); /* DataPresent */
			Console.WriteLine (flag == 1 ? "data present" : "data not present");
			flag = bs.ReadBit(); /* IsStereo */
			Console.WriteLine (flag == 1 ? "stereo" : "mono");
			flag = bs.ReadBit(); /* Is16Bits */
			Console.WriteLine (flag == 1 ? "16 bits" : "8 bits");

			HuffmanTree ll = new HuffmanTree();
			HuffmanTree lh = new HuffmanTree();
			HuffmanTree rl = new HuffmanTree();
			HuffmanTree rh = new HuffmanTree();

			ll.Build (bs);
			lh.Build (bs);
			rl.Build (bs);
			rh.Build (bs);

			byte[] tmp = new byte[2];
			short rightbase;
			short leftbase;

			tmp[1] = (byte)bs.ReadByte();
			tmp[0] = (byte)bs.ReadByte();
			rightbase = BitConverter.ToInt16 (tmp, 0);

			tmp[1] = (byte)bs.ReadByte();
			tmp[0] = (byte)bs.ReadByte();
			leftbase = BitConverter.ToInt16 (tmp, 0);

			Console.WriteLine ("initial bases = {0}, {1}", leftbase, rightbase);
			OutputSample (outputStream, leftbase, rightbase);

			while (bs.ReadCount < bs.Length) {
				/* read the huffman encoded deltas */
				tmp[0] = (byte)(ll == null ? 0 : ll.Decode (bs));
				tmp[1] = (byte)(lh == null ? 0 : lh.Decode (bs));

				short leftdelta = BitConverter.ToInt16 (tmp, 0);

				tmp[0] = (byte)(rl == null ? 0 : rl.Decode (bs));
				tmp[1] = (byte)(rh == null ? 0 : rh.Decode (bs));

				short rightdelta = BitConverter.ToInt16 (tmp, 0);

				rightbase += rightdelta;
				leftbase += leftdelta;

				OutputSample (outputStream, leftbase, rightbase);
			}

			Console.WriteLine ("outputted {0} bytes in samples for this frame.", outputStream.Position);
		}

		void Decompress16Mono (BitStream bs, Stream outputStream)
		{
			int flag;

			flag = bs.ReadBit(); /* DataPresent */
			Console.WriteLine (flag == 1 ? "data present" : "data not present");
			flag = bs.ReadBit(); /* Is16Bits */
			Console.WriteLine (flag == 1 ? "16 bits" : "8 bits");
			flag = bs.ReadBit(); /* IsStereo */
			Console.WriteLine (flag == 1 ? "stereo" : "mono");

			HuffmanTree h = new HuffmanTree();
			HuffmanTree l = new HuffmanTree();
			byte[] _base = new byte[2];

			l.Build (bs);
			h.Build (bs);

			_base[1] = (byte)bs.ReadByte();
			_base[0] = (byte)bs.ReadByte();

			OutputSample (outputStream, _base);

			while (bs.ReadCount < bs.Length) {
				/* read the huffman encoded deltas */
				int ld = (int)l.Decode (bs);
				int hd = (int)h.Decode (bs);

				if (ld + _base[0] > 255) {
					_base[1] += 1;
					_base[0] = (byte)(ld + _base[0] - 255);
				}
				else
					_base[0] = (byte)(hd + _base[0]);
				_base[1] = (byte)(hd + _base[1]);

				OutputSample (outputStream, _base);
			}
			Console.WriteLine ("outputted {0} bytes in samples for this frame.", outputStream.Position);
		}

		void Decompress8Stereo (BitStream bs, Stream outputStream)
		{
			int flag;

			flag = bs.ReadBit(); /* DataPresent */
			Console.WriteLine (flag == 1 ? "data present" : "data not present");
			flag = bs.ReadBit(); /* Is16Bits */
			Console.WriteLine (flag == 1 ? "16 bits" : "8 bits");
			flag = bs.ReadBit(); /* IsStereo */
			Console.WriteLine (flag == 1 ? "stereo" : "mono");

			HuffmanTree r = new HuffmanTree();
			HuffmanTree l = new HuffmanTree();
			byte leftbase;
			byte rightbase;

			l.Build (bs);
			r.Build (bs);

			rightbase = (byte)bs.ReadByte();
			leftbase = (byte)bs.ReadByte();

			OutputSample (outputStream, leftbase, rightbase);

			while (bs.ReadCount < bs.Length) {
				/* read the huffman encoded deltas */
				int rd = (int)r.Decode (bs);
				int ld = (int)l.Decode (bs);

				rightbase = (byte)(rightbase + rd);
				leftbase = (byte)(leftbase + ld);

				OutputSample (outputStream, leftbase, rightbase);
			}
			Console.WriteLine ("outputted {0} bytes in samples for this frame.", outputStream.Position);
		}

		void Decompress8Mono (BitStream bs, Stream outputStream)
		{
			int flag;

			flag = bs.ReadBit(); /* DataPresent */
			Console.WriteLine (flag == 1 ? "data present" : "data not present");
			flag = bs.ReadBit(); /* Is16Bits */
			Console.WriteLine (flag == 1 ? "16 bits" : "8 bits");
			flag = bs.ReadBit(); /* IsStereo */
			Console.WriteLine (flag == 1 ? "stereo" : "mono");

			HuffmanTree t = new HuffmanTree();
			byte _base;

			t.Build (bs);

			_base = (byte)bs.ReadByte();

			OutputSample (outputStream, _base);

			while (bs.ReadCount < bs.Length) {
				/* read the huffman encoded deltas */
				int d = (int)t.Decode (bs);

				_base = (byte)(_base + d);

				OutputSample (outputStream, _base);
			}
			Console.WriteLine ("outputted {0} bytes in samples for this frame.", outputStream.Position);
		}

		void ReadAudioChunk (BitStream bs, int channel)
		{
			uint frameAudioLength = 0;
			uint audioDecompressedLength = 0;

			bool compressed = ((audioFlags[channel] & (int)ARFlags.Compressed) == (int)ARFlags.Compressed);
			bool is16 = ((audioFlags[channel] & (int)ARFlags.Is16Bit) == (int)ARFlags.Is16Bit);
			bool stereo = ((audioFlags[channel] & (int)ARFlags.Stereo) == (int)ARFlags.Stereo);

			bs.AssertAtByteBoundary ();

			bs.DebugFirst ();

			Console.WriteLine ("reading audio chunk length");
			frameAudioLength = bs.ReadDWord ();
			Console.WriteLine ("audio chunk length = {0}", frameAudioLength);

			if (compressed) {
				audioDecompressedLength = bs.ReadDWord ();
				Console.WriteLine ("decompressed audio length = {0}", audioDecompressedLength);


				byte[] streamBuf = new byte[frameAudioLength];
				bs.Read (streamBuf, 0, streamBuf.Length);

				audioOut[channel] = new byte[audioDecompressedLength];

				if (is16)
					if (stereo)
						Decompress16Stereo (new BitStream (streamBuf), new MemoryStream (audioOut[channel]));
					else
						Decompress16Mono (new BitStream (streamBuf), new MemoryStream (audioOut[channel]));
				else
					if (stereo)
						Decompress8Stereo (new BitStream (streamBuf), new MemoryStream (audioOut[channel]));
					else
						Decompress8Mono (new BitStream (streamBuf), new MemoryStream (audioOut[channel]));
			}
			else {
				audioOut[channel] = new byte[frameAudioLength];
				bs.Read (audioOut[channel], 0, (int)frameAudioLength);
			}
		}

		void ReadFrame (BitStream bs)
		{
			if ((frameFlags [curFrame] & 0x01) == 0x01) {
				ReadPaletteChunk (bs);
			}

			for (int i = 1; i < 8; i ++)
				if ((frameFlags [curFrame] & (1<<i)) == (1<<i))
					ReadAudioChunk (bs, i-1);

			if (bs.Position == bs.Length)
				return;

			ReadVideoChunk(bs);
		}

		void MPQResource.ReadFromStream (Stream stream)
		{
			this.stream = stream;

			ReadHeader ();
			ReadFrameSizes ();
			ReadFrameFlags ();
			ReadTrees ();

			DumpHeader ();
			DumpFrameInfo ();

			frameDataPosition = stream.Position;
			Console.WriteLine ("frames start at 0x{0:x}", frameDataPosition);
			Console.WriteLine ("allocating video data buffer at {0} x {1}", PaddedWidth, PaddedHeight);

			frameData = new byte[PaddedWidth * PaddedHeight];
		}

		long frameDataPosition;
		int curFrame;

		byte[] palette;
		byte[] frameData;

		public void Play ()
		{
			stream.Position = frameDataPosition;

			curFrame = 0;
			NextFrame ();
		}

		public void NextFrame ()
		{
			if (curFrame >= frames) {
				Console.WriteLine ("ending stream position = {0}", stream.Position);
				if (AnimationDone != null)
					AnimationDone ();
				return;
			}

			uint frameSize = frameSizes[curFrame];

			Console.WriteLine ("reading frame {0}, position = {1}, size = {2}", curFrame, stream.Position, frameSize);

			if ((frameSize & 0x1) == 0x1)
				Console.WriteLine (" + frame is a key frame");
			if ((frameSize & 0x2) == 0x2)
				Console.WriteLine (" + frame is <something>");

			frameSize &= 0xfffffffc;

			Console.WriteLine (" + frame size = {0}", frameSize);

			byte[] frame_buf = new byte[frameSize];
			stream.Read (frame_buf, 0, (int)frameSize);
			ReadFrame (new BitStream (frame_buf));
			if (FrameReady != null)
				FrameReady (frameData, palette, audioOut);
			curFrame++;
		}

		public event SMKFrameReady FrameReady;
		public event SMKAnimationDone AnimationDone;


		void DumpHeader ()
		{
			Console.WriteLine ("width: {0}", width);
			Console.WriteLine ("height: {0}", height);
			Console.WriteLine ("frame count: {0}", frames);
			Console.WriteLine ("frame rate: {0} {1}", frameRate, CalcFps ());
			Console.WriteLine ("flags: {0}", flags);
			Console.WriteLine ("TreesSize: {0}", treesSize);
			Console.WriteLine ("MMap_Size: {0}", mmap_Size);
			Console.WriteLine ("MClr_Size: {0}", mclr_Size);
			Console.WriteLine ("Full_Size: {0}", full_Size);
			Console.WriteLine ("Type_Size: {0}", type_Size);

			for (int i = 0; i < 7; i ++) {
				Console.Write ("Channel {0}: ", i);
				if ((audioFlags[i] & (int)ARFlags.DataPresent) == (int)ARFlags.DataPresent) {
					Console.Write ("Size = {0}, ", audioSize[i]);
					Console.Write ("Freq = {0} ", audioRate[i]);
					Console.Write ("Flags = {0} [", audioFlags[i]);
					if ((audioFlags[i] & (int)ARFlags.Compressed) == (int)ARFlags.Compressed)
						Console.Write ("compressed ");
					if ((audioFlags[i] & (int)ARFlags.CompressionMask) == V2_COMPRESSION)
						Console.Write ("(hdpcm)");
					else
						Console.Write ("(unknown)");
					if ((audioFlags[i] & (int)ARFlags.DataPresent) == (int)ARFlags.DataPresent)
						Console.Write (" data present");
					if ((audioFlags[i] & (int)ARFlags.Is16Bit) == (int)ARFlags.Is16Bit)
						Console.Write (" 16 bit");
					if ((audioFlags[i] & (int)ARFlags.Stereo) == (int)ARFlags.Stereo)
						Console.Write (" stereo");
					Console.WriteLine ("]");
				}
				else {
					Console.WriteLine ("(no data)");
				}
			}
		}

		void DumpFrameInfo ()
		{
			for (int i = 0; i < frames; i ++) {
				Console.Write ("Frame {0}: ", i);
				if ((frameFlags[i] & 0x01) != 0)
					Console.Write ("(palette)");
				for (int ch = 0; ch <= 7; ch++)
					if ((frameFlags[i] & (1 << (ch + 1))) != 0)
						Console.Write ("(channel {0})", ch);
				Console.WriteLine ();
			}
		}
	}

	public delegate void SMKFrameReady (byte[] pixelData, byte[] palette, byte[][] audioBuffers);
	public delegate void SMKAnimationDone ();

	class BitStream
	{
		public void DebugFirst ()
		{
			int num_bytes = 12;
			if (num_bytes >= buf.Length - current_byte_index)
				num_bytes = buf.Length - current_byte_index - 1;

			for (int i = 0; i < num_bytes; i ++)
				Console.Write ("0x{0:x} ", buf[current_byte_index + i]);
			Console.WriteLine ("\ncurrent byte = 0x{0:x}", current_byte);
		}

		int current_byte;
		int current_byte_index;
		int current_bit;
		
		byte [] buf;

		public BitStream (byte[] buf)
		{
			this.buf = buf;
			this.current_byte_index = 0;
			this.current_byte = buf[current_byte_index];
			this.current_bit = 8;
		}

		int GetBit ()
		{
			int rv = current_byte & 0x1;

			current_bit --;
			current_byte >>= 1;
			if (current_bit == 0) {
				current_bit = 8;
				if (current_byte_index + 1 > buf.Length)
					throw new Exception (String.Format ("about to read off end of {0} byte buffer", buf.Length));
				current_byte_index++;
				current_byte = buf[current_byte_index];
			}

			return rv;
		}

		public int ReadBit ()
		{
			int rv = GetBit ();

			Console.WriteLine ("bit: {0}", rv);

			return rv;
		}

		public int ReadByte ()
		{
			int rv;
			if (current_bit == 8) {
				rv = current_byte;
				current_byte_index++;
				current_byte = buf[current_byte_index];
			}
			else {
				rv = 0;
				for (int i = 0; i < 8; i ++)
					rv |= GetBit () << i;
			}

			Console.WriteLine ("byte: {0:x}", rv);

			return rv;
		}

		public ushort ReadWord ()
		{
			return ((ushort)(ReadByte () | (ReadByte() << 8)));
		}

		public uint ReadDWord ()
		{
			return (uint)(ReadByte () | (ReadByte() << 8) | (ReadByte() << 16) | (ReadByte() << 24));
		}

		public void Read (byte[] dest, int index, int length)
		{
			AssertAtByteBoundary();

			if (current_byte_index + length > buf.Length)
				throw new Exception ();

			Array.Copy (buf, current_byte_index, dest, index, length);

			current_byte_index += length;
			if (current_byte_index < buf.Length) {
				current_byte = buf[current_byte_index];
				current_bit = 8;
			}
		}

		public void AssertAtByteBoundary ()
		{
			if (current_bit != 8)
				throw new Exception ("this operation only works on byte boundaries");
		}

		int saved_start;
		public void ZeroReadCount ()
		{
			saved_start = current_byte_index;
		}

		public int ReadCount {
			get { return current_byte_index - saved_start; }
		}

		public int Position {
			get { return current_byte_index; }
			set { current_byte_index = value; }
		}

		public int Length {
			get { return buf.Length; }
		}
	}

	class HuffmanNode {
		public uint value;
		public HuffmanNode branch_0;
		public HuffmanNode branch_1;
	}

	class HuffmanTree {
		class HuffIter {
			HuffmanTree tree;
			HuffmanNode currentNode;

			public HuffIter (HuffmanTree tree)
			{
				this.tree = tree;
				this.currentNode = tree.Root;
			}

			public void ProcessBit (uint b, int bit)
			{
				currentNode = (b & (1<<bit)) == 0 ? currentNode.branch_0 : currentNode.branch_1;
				if (currentNode == null)
					throw new Exception ("can't advance to child nodes from a leaf node");
			}

			public void ProcessBit (BitStream bs)
			{
				currentNode = bs.ReadBit() == 0 ? currentNode.branch_0 : currentNode.branch_1;
				if (currentNode == null)
					throw new Exception ("can't advance to child nodes from a leaf node");
			}

			public void Reset ()
			{
				currentNode = tree.Root;
			}

			public uint Value { 
				get {
					if (!IsLeaf) throw new Exception ("this node is not a leaf");
					return currentNode.value;
				}
			}

			public bool IsLeaf {
				get {
					return (currentNode.branch_0 == null
						&& currentNode.branch_1 == null);
				}
			}
		}

		HuffmanNode root;

		public HuffmanTree ()
		{
			iter = new HuffIter (this);
		}

		public virtual void Build (BitStream bs)
		{
			Read (bs);

#if false
			/* output some foo about the tree */
			int depth = getmaxdepth (root);
			Console.WriteLine ("+ max depth = {0}", depth);
			int num_leaves = 0, num_nodes = 0;
			getcounts (root, ref num_leaves, ref num_nodes);
			Console.WriteLine ("+ num leaves = {0}, num_nodes = {1}", num_leaves, num_nodes);
			dumpleaves (root, "");
#endif
		}

		protected int getmaxdepth (HuffmanNode node)
		{
			int max_0 = 0, max_1 = 0;

			if (node.branch_0 == null && node.branch_1 == null)
				return 1;

			if (node.branch_0 != null)
				max_0 = getmaxdepth (node.branch_0);
			if (node.branch_1 != null)
				max_1 = getmaxdepth (node.branch_1);
			if (max_0 > max_1)
				return max_0 + 1;
			else
				return max_1 + 1;
		}

		protected internal void getcounts (HuffmanNode node, ref int num_leaves, ref int num_nodes)
		{
			if (node.branch_0 == null && node.branch_1 == null) {
				num_leaves++;
				return;
			}

			num_nodes++;
			getcounts (node.branch_0, ref num_leaves, ref num_nodes);
			getcounts (node.branch_1, ref num_leaves, ref num_nodes);
		}

		protected internal void dumpleaves (HuffmanNode node, string accum)
		{
			if (node.branch_0 == null && node.branch_1 == null)
				Console.WriteLine ("{0}: {1}", accum, node.value);

			if (node.branch_0 != null)
				dumpleaves (node.branch_0, accum+"0");
			if (node.branch_1 != null)
				dumpleaves (node.branch_1, accum+"1");
		}

		protected virtual void ReadValue (BitStream bs, HuffmanNode node)
		{
			node.value = (uint)bs.ReadByte();
		}

		protected void Read (BitStream bs)
		{
			root = new HuffmanNode ();
			ReadNode (bs, Root);
		}

		protected void ReadNode (BitStream bs, HuffmanNode node)
		{
			int flag = bs.ReadBit();
			if (flag == 0) {
				/* it's a leaf */
				ReadValue (bs, node);
			}
			else {
				/* it's a node */
				node.branch_0 = new HuffmanNode ();
				ReadNode (bs, node.branch_0);
				node.branch_1 = new HuffmanNode ();
				ReadNode (bs, node.branch_1);
			}
		}

		public HuffmanNode Root {
			get { return root; }
		}

		protected HuffIter iter;
		public uint Decode (BitStream bs)
		{
			iter.Reset ();
			while (!iter.IsLeaf)
				iter.ProcessBit (bs);

			return iter.Value;
		}

		public uint Decode (byte b)
		{
			iter.Reset ();
			for (int i = 0; i < 8 && !iter.IsLeaf; i ++)
				iter.ProcessBit (b, i);

			return iter.Value;
		}
	}

	class Dynamic16HuffmanTree : HuffmanTree {
		HuffmanTree highTree;
		HuffmanTree lowTree;

		ushort marker1;
		ushort marker2;
		ushort marker3;

		HuffmanNode shortest1;
		HuffmanNode shortest2;
		HuffmanNode shortest3;

		public Dynamic16HuffmanTree ()
		{
		}

		uint DecodeValue (BitStream bs)
		{
			byte lowValue, highValue;

			lowValue = 0;
			if (lowTree != null) {
				Console.WriteLine (" + getting low byte");
				lowValue = (byte)lowTree.Decode (bs);
			}

			highValue = 0;
			if (highTree != null) {
				Console.WriteLine (" + getting high byte");
				highValue = (byte)highTree.Decode (bs);
			}

			uint rv = (uint)(lowValue | ((uint)highValue << 8));

			Console.WriteLine (" + value = {0:x}", rv);

			return rv;
		}

		protected override void ReadValue (BitStream bs, HuffmanNode node)
		{
			uint value = DecodeValue (bs);

			if (value == marker1)
				shortest1 = node;
			else if (value == marker2)
				shortest2 = node;
			else if (value == marker3)
				shortest3 = node;
			else 
				node.value = value;
		}

		public override void Build (BitStream bs)
		{
			int tag = bs.ReadBit();
			if (tag == 1) {
				lowTree = new HuffmanTree ();
				Console.WriteLine ("building low tree");
				lowTree.Build (bs);
			}

			tag = bs.ReadBit ();
			if (tag == 1) {
				highTree = new HuffmanTree ();
				Console.WriteLine ("building high tree");
				highTree.Build (bs);
			}

			marker1 = bs.ReadWord ();
			marker2 = bs.ReadWord ();
			marker3 = bs.ReadWord ();

			Console.WriteLine ("markers = {0:x}, {1:x}, {2:x}", marker1, marker2, marker3);

			Read (bs);

			Console.WriteLine ("shortest1 = {0}", shortest1);
			Console.WriteLine ("shortest2 = {0}", shortest2);
			Console.WriteLine ("shortest3 = {0}", shortest3);

			/* output some foo about the tree */
			int depth = getmaxdepth (Root);
			Console.WriteLine ("+ max depth = {0}", depth);
			int num_leaves = 0, num_nodes = 0;
			getcounts (Root, ref num_leaves, ref num_nodes);
			Console.WriteLine ("+ num leaves = {0}, num_nodes = {1}", num_leaves, num_nodes);
		}
	}
}

