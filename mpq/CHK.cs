using System;
using System.IO;
using System.Text;

using Gtk;
using Gdk;

namespace Starcraft {

	public class CHK : MPQResource {

		public CHK ()
		{
		}

		void MPQResource.ReadFromStream (Stream stream)
		{
			long stream_length = stream.Length;
			byte[] section_name_buf = new byte[4];
			string section_name;
			uint section_length;

			do {
				stream.Read (section_name_buf, 0, 4);
				section_length = Util.ReadDWord (stream);
				
				section_name = Encoding.ASCII.GetString (section_name_buf, 0, 4);

				byte[] section_buf = new byte[section_length];
				stream.Read (section_buf, 0, (int)section_length);

				ParseSection (section_name, section_buf);
			} while (stream.Position < stream_length);
		}

		void ParseSection (string section_name, byte[] section_data)
		{
			if (section_name == "TYPE")
				scenarioType = Util.ReadWord (section_data, 0);
			else if (section_name == "ERA ")
				tileSet = Util.ReadWord (section_data, 0);
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
			else
				Console.WriteLine ("Unhandled CHK section type {0}, length {1}", section_name, section_data.Length);
		}

		ushort scenarioType;
		public ushort ScenarioType {
			get { throw new NotImplementedException (); }
		}

		ushort tileSet;
		public ushort TileSet {
			get { throw new NotImplementedException (); }
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
	}

}

