//
// SCSharp.Mpq.SpritesDat
//
// Authors:
//	Chris Toshok (toshok@hungry.com)
//
// (C) 2006 The Hungry Programmers (http://www.hungry.com/)
//

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

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp
{
	public class SpritesDat : Dat
	{
		const int NUM_SPRITES = 386;
		const int NUM_DOODADS = 130;

		int imageIndexBlockId;
		int barLengthBlockId;
		int selectionCircleBlockId;
		int selectionCircleOffsetBlockId;

		public SpritesDat ()
		{
			Console.WriteLine ("SpritesDat");
			imageIndexBlockId = AddVariableBlock (386, DatVariableType.Word);
			barLengthBlockId = AddVariableBlock (NUM_SPRITES - NUM_DOODADS, DatVariableType.Byte);
			selectionCircleBlockId = AddVariableBlock (NUM_SPRITES - NUM_DOODADS, DatVariableType.Byte);
			selectionCircleOffsetBlockId = AddVariableBlock (NUM_SPRITES - NUM_DOODADS, DatVariableType.Byte);

			Console.WriteLine ("imageIndexBlockId = {0}", GetVariableOffset (imageIndexBlockId));
			Console.WriteLine ("barLengthBlockId = {0}", GetVariableOffset (barLengthBlockId));
			Console.WriteLine ("selectionCircleBlockId = {0}", GetVariableOffset (selectionCircleBlockId));
			Console.WriteLine ("selectionCircleOffsetBlockId = {0}", GetVariableOffset (selectionCircleOffsetBlockId));
		}

		public DatCollection<ushort> ImagesDatEntries {
			get { return (DatCollection<ushort>)GetCollection (imageIndexBlockId); }
		}

		public DatCollection<byte> BarLengths {
			get { return (DatCollection<byte>)GetCollection (barLengthBlockId); }
		}

		public DatCollection<byte> SelectionCircles {
			get { return (DatCollection<byte>)GetCollection (selectionCircleBlockId); }
		}

		public DatCollection<byte> SelectionCircleOffsets {
			get { return (DatCollection<byte>)GetCollection (selectionCircleOffsetBlockId); }
		}
	}

}
