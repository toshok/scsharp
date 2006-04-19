using System;
using System.IO;
using System.Collections.Generic;

namespace SCSharp {
	public class Util {
		// read in a LE word
		public static ushort ReadWord (Stream fs)
		{
			return ((ushort)(fs.ReadByte () | (fs.ReadByte() << 8)));
		}
		public static ushort ReadWord (byte[] buf, int position)
		{
			return ((ushort)((int)buf[position] | (int)buf[position+1] << 8));
		}

		// read in a LE doubleword
		public static uint ReadDWord (Stream fs)
		{
			return (uint)(fs.ReadByte () | (fs.ReadByte() << 8) | (fs.ReadByte() << 16) | (fs.ReadByte() << 24));
		}
		public static uint ReadDWord (byte[] buf, int position)
		{
			return ((uint)((uint)buf[position] | (uint)buf[position+1] << 8 | (uint)buf[position+2] << 16 | (uint)buf[position+3] << 24));
		}

		// read in a byte
		public static byte ReadByte (Stream fs)
		{
			return (byte)fs.ReadByte();
		}

		// write a LE word
		public static void WriteWord (Stream fs, ushort word)
		{
			fs.WriteByte ((byte)(word & 0xff));
			fs.WriteByte ((byte)((word >> 8) & 0xff));
		}

		// write a LE doubleword
		public static void WriteDWord (Stream fs, uint dword)
		{
			fs.WriteByte ((byte)(dword & 0xff));
			fs.WriteByte ((byte)((dword >> 8) & 0xff));
			fs.WriteByte ((byte)((dword >> 16) & 0xff));
			fs.WriteByte ((byte)((dword >> 24) & 0xff));
		}

		public static char[] RaceChar = { 'P','T','Z' };
		public static char[] RaceCharLower = { 'p','t','z' };


		public static string[] TilesetNames = {
			"badlands",
			"platform",
			"install",
			"ashworld",
			"jungle",
			"desert",
			"ice",
			"twilight"
		};
	}
}

