

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Starcraft {

	public class SpritesDat : MpqResource
	{
		const int NUM_SPRITES = 517;
		const int NUM_DOODADS = 130;

		const int IMAGESDAT_FIELD = 0;
		const int BARLENGTH_FIELD = 1;
		const int SELECTIONCIRCLE_FIELD = 4;
		const int SELECTIONCIRCLE_OFFSET_FIELD = 5;

		const int NUM_FIELDS = 6;

		int[] size_of_variable = { 2, 1, 1, 1, 1, 1 };
		int[] size_of_variable_block = {
			NUM_SPRITES * 2,
			NUM_SPRITES - NUM_DOODADS,
			NUM_SPRITES,
			NUM_SPRITES,
			NUM_SPRITES - NUM_DOODADS,
			NUM_SPRITES - NUM_DOODADS };

		int[] offset_to_variable_block;

		byte[] buf;

		public SpritesDat ()
		{
			offset_to_variable_block = new int [NUM_FIELDS];
			offset_to_variable_block[0] = 0;
			for (int i = 1; i < NUM_FIELDS; i ++)
				offset_to_variable_block [i] = offset_to_variable_block[i-1] + size_of_variable_block[i-1];
		}

		void MpqResource.ReadFromStream (Stream stream)
		{
			int size = 0;
			for (int i = 0; i < NUM_FIELDS; i ++)
				size += size_of_variable_block [i];
			buf = new byte [size];
			stream.Read (buf, 0, size);
		}

		int GetIndexLocation (int field, int index)
		{
			return offset_to_variable_block[field] + index * size_of_variable[field];
		}

		public ushort GetImagesDatEntry (int index)
		{
			return Util.ReadWord (buf, GetIndexLocation (IMAGESDAT_FIELD, index));
		}
	}

}
