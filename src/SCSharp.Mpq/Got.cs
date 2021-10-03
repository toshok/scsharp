//
// SCSharp.Mpq.Got
//
// Authors:
//	Chris Toshok (toshok@gmail.com)
//
// Copyright 2006-2010 Chris Toshok
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
using System.Collections;
using System.Collections.Generic;

namespace SCSharp
{
	/*
0x00 (byte) - Always 03, anything else will make it not show up
0x01 (string) - The name listed for this game type in the dropdown menu.
0x21 (string) - Label for the variable dropdown list (ie, the one used to set greed ammount, etc)
0x41 (byte) - order of the game type in the selection list
0x43 (byte) - Enabled? Setting this to 0 will make it not show up in starcraft.
0x45 (unsigned short) - The variable setting label (the variable setting's name in the list box)
0x49 (byte) - The game type settings to use, sets the trg and misc. exe data for the game type
0x4A (flag) - Sets weather or not to set the inital mineral ammount (as defined in 0x58) (0 = No starting minerals set to 0, 1 = Yes)
0x4C (byte) - Unknown (always 2?)
0x4D (byte) - Start-up options (0 = Preplaced units (UMS), 1 = 4 workers, 2 = Town hall, 4 workers)
0x4E (byte) - Unknown (0 usually, 1 in UMS)
0x4F (byte) - Playability (0 = 0 Comp can't play, multi only, 1 = Comp can play, multi only, 2 = Disabled?, 3 = Comp can play, multi or single)
0x50 (flag) - Ally Status (0 = Alliances not allowed, 1 = Default ally status)
0x51 (byte) - Number of teams
0x52 (byte) - Unknown, (always 1?)
0x54 (unsigned short) - Game type's variable (actual setting rather than just the label as in 0x45)
0x58 (unsigned long) - Starting mineral ammount
	*/
	public enum InitialUnits {
		UseMapSettings = 0x00,
		HarvestersOnly = 0x01,
		BaseAndHarvesters = 0x02
	}

	public class Got : MpqResource {
		byte[] contents;

		public Got ()
		{
		}

		public void ReadFromStream (Stream stream)
		{
			contents = new byte[stream.Length];
			stream.Read (contents, 0, (int)stream.Length);
		}

		public string UIGameTypeName {
			get { return Util.ReadUntilNull (contents, 0x01); }
		}

		public string UISubtypeLabel {
			get { return Util.ReadUntilNull (contents, 0x21); }
		}

		public bool ComputerPlayersAllowed {
			get { return (contents[0x4f] & 0x01) != 0; }
		}

		public int NumberOfTeams {
			get { return contents[0x51]; }
		}

		public int ListPosition {
			get { return contents[0x41]; }
		}

		public bool UseMapSettings {
			get { return (contents[0x4e] == 1); }
		}

		public InitialUnits InitialUnits {
			get { return (InitialUnits)contents[0x4d]; }
		}
	}
}
