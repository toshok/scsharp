using System;
using System.IO;
using System.Text;

using System.Collections.Generic;

namespace SCSharp {

	public enum Tileset {
		Badlands = 0,
		Platform = 1,
		Installation = 2,
		Ashworld = 3,
		Jungle = 4,
		Desert = 5,
		Ice = 6,
		Twilight = 7
	}

	public class Chk : MpqResource {

		public Chk ()
		{
		}

		const string DIM = "DIM ";
		const string MTXM = "MTXM";

		Dictionary<string,byte[]> sections;

		public void ReadFromStream (Stream stream)
		{
			long stream_length = stream.Length;
			byte[] section_name_buf = new byte[4];
			string section_name;
			uint section_length;

			sections = new Dictionary<string,byte[]>();

			do {
				stream.Read (section_name_buf, 0, 4);
				section_length = Util.ReadDWord (stream);
				
				section_name = Encoding.ASCII.GetString (section_name_buf, 0, 4);

				Console.WriteLine ("section '{0}' length = {1}", section_name, section_length);

				byte[] section_buf = new byte[section_length];
				stream.Read (section_buf, 0, (int)section_length);

				sections.Add (section_name, section_buf);
			} while (stream.Position < stream_length);

			/* find out what version we're dealing with */
			ParseSection ("VER ");
			if (sections.ContainsKey ("IVER"))
				ParseSection ("IVER");
			if (sections.ContainsKey ("IVE2"))
				ParseSection ("IVE2");

			/* strings */
			ParseSection ("STR ");

			/* load in the map info */

			/* map name/description */
			ParseSection ("SPRP");

			/* dimension */
			ParseSection ("DIM ");

			/* tileset info */
			ParseSection ("ERA ");

			/* graphical tile section */
			ParseSection ("MTXM");

			/* player information */
			//			ParseSection ("OWNR");
			ParseSection ("SIDE");

			/* tile info */
			if (sections.ContainsKey ("TILE"))
				ParseSection ("TILE");

			/* isometric mapping */
			if (sections.ContainsKey ("ISOM"))
				ParseSection ("ISOM");

			/* fog of war */
			ParseSection ("MASK");
		}
		
		void ParseSection (string section_name)
		{
			byte[] section_data = sections[section_name];
			if (section_data == null)
				throw new Exception (String.Format ("map file is missing section {0}, cannot load", section_name));

			if (section_name == "TYPE") {
				scenarioType = Util.ReadWord (section_data, 0);
			}
			else if (section_name == "ERA ")
				tileSet = (Tileset)Util.ReadWord (section_data, 0);
			else if (section_name == "DIM ") {
				width = Util.ReadWord (section_data, 0);
				height = Util.ReadWord (section_data, 2);

				Console.WriteLine ("map is {0} x {1}", width, height);
			}
			else if (section_name == "MTXM") {
				Console.WriteLine ("section_data == {0} bytes, width * height * 2 == {1}", section_data.Length, width * height * 2);
				mapTiles = new ushort[width,height];
				int y, x;
				for (y = 0; y < height; y ++)
					for (x = 0; x < width; x ++)
						mapTiles[x,y] = Util.ReadWord (section_data, (y*width + x)*2);
			}
			else if (section_name == "MASK") {
				mapMask = new byte[width,height];
				int y, x;
				int i = 0;

				for (y = 0; y < height; y ++)
					for (x = 0; x < width; x ++)
						mapMask[x,y] = section_data [i++];
			}
			else if (section_name == "SPRP") {
				int nameStringIndex = Util.ReadWord (section_data, 0);
				int descriptionStringIndex = Util.ReadWord (section_data, 2);

				mapName = GetMapString (nameStringIndex);
				mapDescription = GetMapString (descriptionStringIndex);
			}
			else if (section_name == "STR ") {
				ReadStrings (section_data);
			}
			else if (section_name == "OWNR") {
				numPlayers = 0;
				for (int i = 0; i < 12; i ++) {
					/* 
					   00 - Unused
					   03 - Rescuable
					   05 - Computer
					   06 - Human
					   07 - Neutral
					*/
					if (section_data[i] == 0x05 ||
					    section_data[i] == 0x06)
						numPlayers ++;
				}
			}
			else if (section_name == "SIDE") {
				/*
				  00 - Zerg
				  01 - Terran
				  02 - Protoss
				  03 - Independent
				  04 - Neutral
				  05 - User Select
				  07 - Inactive
				  10 - Human
				*/
				numPlayers = 0;
				for (int i = 0; i < 12; i ++) {
					if (section_data[i] == 0x05) /* user select */
						numPlayers++;
				}
			}
			else if (section_name == "UNIT") {
				ReadUnits (section_data);
			}
			else
				Console.WriteLine ("Unhandled Chk section type {0}, length {1}", section_name, section_data.Length);
		}

		List<UnitInfo> units;
		void ReadUnits (byte[] data)
		{
			units = new List<UnitInfo> ();

			MemoryStream stream = new MemoryStream (data);
			Console.WriteLine ("unit section data = {0} bytes long", data.Length);

			int i = 0;
			while (i <= data.Length / 36) {
				Console.WriteLine ("unit {0}", i);
				Console.WriteLine ("========");
				uint serial = Util.ReadDWord (stream); Console.WriteLine ("serial = {0}", serial);
				ushort x = Util.ReadWord (stream); Console.WriteLine ("x = {0}", x);
				ushort y = Util.ReadWord (stream); Console.WriteLine ("y = {0}", y);
				ushort type = Util.ReadWord (stream); Console.WriteLine ("type = {0}", type);
				Util.ReadWord (stream);
				Util.ReadWord (stream);
				Util.ReadWord (stream);
				byte player = Util.ReadByte (stream); Console.WriteLine ("player = {0}", player);
				Util.ReadByte (stream);
				Util.ReadByte (stream);
				Util.ReadByte (stream);
				Util.ReadDWord (stream);
				Util.ReadWord (stream);
				Util.ReadWord (stream);

				Util.ReadByte (stream);
				Util.ReadByte (stream);
				Util.ReadByte (stream);
				Util.ReadByte (stream);
				Util.ReadByte (stream);
				Util.ReadByte (stream);
				Util.ReadByte (stream);
				Util.ReadByte (stream);
				i++;

				UnitInfo info = new UnitInfo ();
				info.unit_id = type;
				info.x = x;
				info.y = y;

				units.Add (info);
			}
		}

		void ReadStrings (byte[] data)
		{
			MemoryStream stream = new MemoryStream (data);

			int i;

			int num_strings = Util.ReadWord (stream);

			int[] offsets = new int[num_strings];

			for (i = 0; i < num_strings; i ++) {
				offsets[i] = Util.ReadWord (stream);
			}

			StreamReader tr = new StreamReader (stream);
			strings = new string[num_strings];

			for (i = 0; i < num_strings; i++) {
				if (tr.BaseStream.Position != offsets[i]) {
					tr.BaseStream.Seek (offsets[i], SeekOrigin.Begin);
					tr.DiscardBufferedData ();
				}

				strings[i] = Util.ReadUntilNull (tr);
			}
		}

		string[] strings;
		string GetMapString (int idx)
		{
			return strings[idx-1];
		}

		string mapName;
		public string Name {
			get { return mapName; }
		}

		string mapDescription;
		public string Description {
			get { return mapDescription; }
		}

		ushort scenarioType;
		public ushort ScenarioType {
			get { return scenarioType; }
		}

		Tileset tileSet;
		public Tileset Tileset {
			get { return tileSet; }
		}

		ushort width;
		public ushort Width {
			get { return width; }
		}

		ushort height;
		public ushort Height {
			get { return height; }
		}

		ushort[,] mapTiles;
		public ushort[,] MapTiles {
			get { return mapTiles; }
		}

		byte[,] mapMask;
		public byte[,] MapMask {
			get { return mapMask; }
		}

		int numPlayers;
		public int NumPlayers {
			get { return numPlayers; }
		}

		public List<UnitInfo> Units {
			get {
				if (units == null)
					/* initial units on the map */
					ParseSection ("UNIT");

				return units;
			}
		}
	}

	public class UnitInfo {
		public int unit_id;
		public int x;
		public int y;
	};

}
