
using System;
using System.IO;
using System.Collections.Generic;

namespace Starcraft {
	public class Grp : MpqResource {
		// file header info
		ushort frame_count;
		ushort width;
		ushort height;

		struct FrameTabEntry {
			public byte xOffset;
			public byte yOffset;
			public byte unknown1;
			public byte unknown2;
			public uint pFrameData;
		}

		Stream stream;

		List<FrameTabEntry> entries;
		Dictionary<int,byte[,]> frame_cache;

		public Grp ()
		{
			entries = new List<FrameTabEntry> ();
			frame_cache = new Dictionary<int,byte[,]> ();
		}

		void MpqResource.ReadFromStream (Stream stream)
		{
			this.stream = stream;

			frame_count = Util.ReadWord (stream);
			width = Util.ReadWord (stream);
			height = Util.ReadWord (stream);

			for (int i = 0; i < frame_count; i ++) {
				FrameTabEntry entry = new FrameTabEntry ();
				entry.xOffset = Util.ReadByte (stream);
				entry.yOffset = Util.ReadByte (stream);
				entry.unknown1 = Util.ReadByte (stream);
				entry.unknown2 = Util.ReadByte (stream);
				entry.pFrameData = Util.ReadDWord (stream);

				entries.Add (entry);
			}
		}

		public byte[,] GetFrame (int frame)
		{
			int i;

			if (frame > frame_count || frame < 0)
				throw new Exception ();

			if (frame_cache.ContainsKey (frame))
				return frame_cache[frame];

			stream.Seek (entries[frame].pFrameData, SeekOrigin.Begin);

			ushort[] line_offsets;

			ushort old = Util.ReadWord (stream);

			line_offsets = new ushort[old/2+1];
			line_offsets[0] = old;

			for (i = 1; i < old / 2; i ++)
				line_offsets[i] = Util.ReadWord(stream);

			stream.Seek (entries[frame].pFrameData + line_offsets[0], SeekOrigin.Begin);

			byte[,] grid = new byte[height,width];

			for (i = 0; i < line_offsets.Length - 2; i ++) {
				DecodeLine (grid, height - i - entries[frame].yOffset, entries[frame].xOffset, (ushort)(line_offsets[i + 1] - line_offsets[i]), width);
			}

			frame_cache[frame] = grid;

			return grid;
		}

		public ushort FrameCount {
			get { return frame_count; }
		}

		public ushort Width {
			get { return width; }
		}

		public ushort Height {
			get { return height; }
		}

		void DecodeLine (byte[,] grid, int line, byte xOffset, ushort line_length, ushort width)
		{
			byte[] line_data = new byte[line_length];
			stream.Read (line_data, 0, line_length);

			int x = width - xOffset - 1;
			int i = 0;

			//		Console.WriteLine ("decoding line {0}, {1} bytes", line, line_length);

			try { 
				while (i < line_length) {
					//			Console.Write ("[{0}, {1}] ", i, x);
					if (line_data[i] >= 0x80) {
						//				Console.WriteLine (">= 0x80, offset = -{0}", line_data[i] - 0x80);
						x -= line_data[i] - 0x80;
						i++;
					}
					else if (line_data[i] >= 0x40) {
						int loop = line_data[i++] - 0x40;
						byte b = line_data[i++];
						//				Console.WriteLine (">= 0x40, loop = {0} x {1} times", b, loop);
						for (int l = 0; l < loop; l ++)
							grid[line,x--] = b;
					}
					else {
						//				Console.WriteLine ("run of {0} bytes", line_data[i]);
						int loop = line_data[i++];
						for (int l = 0; l < loop; l++)
							grid[line,x--] = line_data[i++];
					}
				}
			} catch (Exception e) { Console.WriteLine ("EXCEPTION on line {0}, x = {2}, i = {3}, line length = {2} (line data length = {4}):", line, x, line_length, i, line_data.Length); }
		}
	}

	/*
	  http://pipin.tmd.ns.ac.yu/extra/fileformat/games/grp/grp.txt


	  some notes
	  ----------

	  All decriptions below are based on experiences during coding the CV-tool.
	  I'll give no guaranties for completeness and correction - it works as 
	  descripted with at least my tools. Feel free to use that doc at your own risk.
	  Modifications are only allowed after correspondation with the author. Feel free
	  to contact me at http://www.cs.tu-berlin.de/~mickyk/2XS.html. 
	  There you'll find some C++ classes for use with that format.

	  Thanx for reading this,

	  TeLAMoN of 2XS in 1998.
	  (mickyk@cs.tu-berlin.de)


	  The Blizzard GRP-Format
	  -----------------------

	  The GRP's are used as graphic-formats for Blizzards StarCraft(TM). The GRP
	  is a multiple frame format. There are no signature information 
	  available (so you can only check the file-extension for valid files) nor 
	  any palette information. After decoding you get a pixel-map which consists
	  of 8-bit-index-values refering to a local palette.
	  How it works:

	  Header:
	  ~~~~~~~
	  Offset
	  +---------------+ 
	  0x0000| L  frame-     | one WORD (2Bytes, Low first) for the number of 
	  | H  count      | following frames
	  +---------------+
	  0x0002| L  X-Dim      | one WORD for the X-dimension of the whole frame
	  | H             |
	  +---------------+
	  0x0004| L  Y-Dim      | one WORD for the Y-dimension of the whole frame
	  | H             |
	  +---------------+

	  FrameTab:
	  ~~~~~~~~~
	  Offset
	  +---------------+ 
	  0x0006| xOffset       | one Byte for the xOffSet of the first frame
	  +---------------+
	  0x0007| yOffset       | one Byte for the yOffSet of the first frame
	  +---------------+
	  0x0008| ?             | these two Bytes I've no idea - maybe you ?!
	  | ?             |
	  +---------------+
	  0x000a| L Offset in   | one DOUBLEWORD (Low first) where we can find
	  |   the file,   | the frame-data
	  |   to find the | <pFrameData>
	  | H framedata   |
	  +---------------+
	  ... 

	  That block repeates as often as frames are present (see framecount)

	  FrameData:
	  ~~~~~~~~~~
	  Offset
	  +---------------+ 
	  <pFrameData>   | L Offset first| one WORD for the offset, where to find the first
	  | H line        | line-data  <oL1> (to get the number of lines-1 
	  +---------------+ for that frame you have to calculate <oL1>/2)
	  | L Offset 2.   |
	  | H line        |
	  +---------------+
	  ...

	  That block repeats until <oL1> is reached (the same as the number 
	  of lines are present within this frame)
               
	  LineData:
	  ~~~~~~~~~
	  Offset
	  +---------------+ 
	  <pFrameData>+<oL1> | BYTE 1        | Bytestream coded with the following 
	  +---------------+ algorythm: see below
	  | BYTE 2        |
	  ...
	  | BYTE <oL2>-1  |
	  +---------------+

	  +---------------+ 
	  <pFrameData>+<oL2> | BYTE 1        | Bytestream  
	  +---------------+ 
	  | BYTE 2        |
	  ...
	  | BYTE <oL3>-1  |
	  +---------------+

	  ... repeated for all lines...


	  Ok, how we get the whole frame composed ? Well, let's start:

	  First to say, we start at the lower-right corner of the frame. So a line
	  will be assembled from right to left. AND don't forget the xOffset and yOffset
	  for the start position. 

	  on_Top:

	  If the BYTE we get is >=0x80 (Values in Hex):

	  -subtract 0x80 from that BYTE and subtract the result from our 
	  current_X_position. 

	  Example: We get 0x85 as value. Subract 0x80 and we get 0x05. If our
	  current_X_position is 0x43 the new value will be (0x43-0x05) 0x3e.
	  How it looks in our framebuffer: ( ^ - is the current_X_position)

	  before:    [... 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 20 10 1f ...] 
	  ^
	  after:     [... 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 20 10 1f ...] 
	  ^


	  else

	  if the BYTE we get is >0x40:

	  -subtract 0x40 from that BYTE and store the result as LOOP
	  -get the next BYTE and put that BYTE LOOP-times into our frame-buffer

	  Example: We get 0x43. Then LOOP will be 0x03. The next BYTE is 0x62.
	  Then our framebuffer will look like this:

	  before:    [... 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 20 10 1f ...] 
	  ^
	  after:     [... 00 00 00 00 00 00 00 62 62 62 00 00 00 00 00 20 10 1f ...] 
	  ^

	  else

	  if the BYTE we get is <0x40:

	  -put that BYTE into LOOP
	  -copy the following LOOP BYTES into out framebuffer

	  Example: The grp looks like ... 06 14 2a 34 3a a1 f1 ... then LOOP will be 0x06
	  and out framebuffer looks like:

	  before:    [... 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 20 10 1f ...] 
	  ^
	  after:     [... 00 f1 a1 3a 34 2a 14 62 62 62 00 00 00 00 00 20 10 1f ...] 
	  ^

	  now get the next BYTE (until the end of the line is reached) and go back to on_Top:.


	  Mhhh, thats all. With that you should be able to read GRP's. If you don't want to
	  code your own C++ classes or routines then you can take a look at 
	  http://www.cs.tu-berlin.de/~mickyk/cv.html . There you'll find some sources
	  and maybe an actuall version of these expl. If you have questions or
	  corrections or whatever else please write a mail.


	  LOF with that expl and remember: REAL CODE IS TIMELESS !!!


	  TeLAMoN of 2XS in 1998
	*/
}
