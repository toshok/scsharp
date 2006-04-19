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

			/* load in the map info */

			/* dimension */
			ParseSection ("DIM ");

			/* tileset info */
			ParseSection ("ERA ");

			/* graphical tile section */
			ParseSection ("MTXM");

			/* tile info */
			if (sections.ContainsKey ("TILE"))
				ParseSection ("TILE");

			/* isometric mapping */
			if (sections.ContainsKey ("ISOM"))
				ParseSection ("ISOM");

			/* fog of war */
			ParseSection ("MASK");

			/* strings */
			ParseSection ("STR ");
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

#if notyet
				try {
					char[] tile = { '0',' ','2','3','4','5','6','7','8','9','a','b','c','d','e','f' };

					for (y = 0; y < height; y ++) {
						for (x = 0; x < height; x ++) {
							Console.Write (tile [(mapTiles[x,y] >> 5) & 0xffff]);
						}
						Console.WriteLine ();
					}
				}
				catch (Exception e) {
					Console.WriteLine (e);
				}
#endif

				Console.WriteLine ("mapTile[0,0] = {0}", mapTiles[0,0]);
			}
			else if (section_name == "MASK") {
				mapMask = new byte[width,height];
				int y, x;
				int i = 0;

				for (y = 0; y < height; y ++)
					for (x = 0; x < width; x ++)
						mapMask[x,y] = section_data [i++];
			}
			else
				Console.WriteLine ("Unhandled Chk section type {0}, length {1}", section_name, section_data.Length);
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
	}
}
