//
// SCSharp.Mpq.FlingyDat
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

/*
  sprite id      at 0x0000. WORD.
  speed          at 0x0170. DWORD.
  turn style     at 0x0450. WORD
  acceleration   at 0x05c0. DWORD.
  turning radius at 0x08a0. BYTE.
  move controls  at ?.. ?.
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp
{
	public class FlingyDat : Dat
	{
		int spriteBlockId;
		int speedBlockId;
		int turnStyleBlockId;
		int accelBlockId;
		//int turnRadiusBlockId;

		const int NUM_ENTRIES = 184;

		public FlingyDat ()
		{
			spriteBlockId = AddVariableBlock (NUM_ENTRIES, DatVariableType.Word);
			speedBlockId = AddVariableBlock (NUM_ENTRIES, DatVariableType.DWord);
			turnStyleBlockId = AddVariableBlock (NUM_ENTRIES, DatVariableType.Word);
			accelBlockId = AddVariableBlock (NUM_ENTRIES, DatVariableType.DWord);
			/* XXX turnRadiusBlockId */
		}

		public DatCollection<ushort> SpriteIds {
			get { return (DatCollection<ushort>)GetCollection (spriteBlockId); }
		}

		public DatCollection<uint> Speeds {
			get { return (DatCollection<uint>)GetCollection (speedBlockId); }
		}

		public DatCollection<ushort> TurnStyles {
			get { return (DatCollection<ushort>)GetCollection (turnStyleBlockId); }
		}

		public DatCollection<uint> Accels {
			get { return (DatCollection<uint>)GetCollection (accelBlockId); }
		}
	}
}
