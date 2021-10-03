//
// SCSharp.Mpq.UnitsDat
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

/*
227 units

general flingy   at 0x0000.  BYTE   - index into flingy.dat
overlay          at 0x00e4.  WORD
      ?          at 0x02ab.
construct sprite at 0x0534.  DWORD  - index into images.dat
shields          at 0x0a8c.  WORD 
hitpoints        at 0x0c54.  DWORD
animation level  at 0x0fe4.  BYTE
movement flags   at 0x10c8.  BYTE
ground weapon?   at 0x11ac.  BYTE?

subunit range    at 0x1d40.  BYTE
sight range      at 0x1e24.  BYTE
      ?          at 0x1f08.  BYTE

portraits        at 0x367c?  WORD

mineral cost at 0x3844. WORD
gas cost at 0x3a0c. WORD
build time at 0x3bd4.  WORD

Restricts at 0x3d9c. WORD
? at 0x3f64.  BYTE

unit width at 0x2f5c.  WORD
unit height at 0x30ac.  WORD

space required at 0x4120.  BYTE


score for producing at 0x43d8. WORD
score for destroying at 0x45a0. WORD

*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SCSharp
{
	public class UnitsDat : Dat
	{
		const int NUM_UNITS = 228;

		const int flingy_offset = 0x0000;
		const int overlay_offset = 0x00e4;
		const int construct_sprite_offset = 0x0534;
		const int animation_level_offset = 0x0fe4;
		const int create_score_offset = 0x43d8;
		const int destroy_score_offset = 0x45a0;
		const int shield_offset = 0x0a8c;
		const int hitpoint_offset = 0x0c54;
#if false
		// these are for some past revision of units.mpq
		const int width_offset = 0x2f5c;
		const int height_offset = 0x30ac;
		const int portrait_offset = 0x367c;
#else
		// these are from the patch_rt.mpq for 1.13f
		const int width_offset = 0x3124;
		const int height_offset = 0x32ec;
		const int portrait_offset = 0x3844;
#endif
		const int compaiidle_offset = 0x1290;
		const int yessoundstart_offset = 0x2a6c;
		const int yessoundend_offset = 0x2b40;

		const int whatsoundstart_offset = 0x2534;
		const int whatsoundend_offset = 0x26fc;

		int flingyBlockId;
		//int overlayBlockId;
		int constructSpriteBlockId;
		int animationLevelBlockId;
		int createScoreBlockId;
		int destroyScoreBlockId;
		int shieldBlockId;
		int hitpointsBlockId;
		int portraitBlockId;
		int widthBlockId;
		int heightBlockId;

		int yesSoundStartBlockId;
		int yesSoundEndBlockId;

		int whatSoundStartBlockId;
		int whatSoundEndBlockId;

		int compAIIdleBlockId;

		public UnitsDat ()
		{
			flingyBlockId = AddPlacedVariableBlock (flingy_offset, NUM_UNITS, DatVariableType.Byte);
			//overlayBlockId = AddPlacedVariableBlock (overlay_offset, NUM_UNITS, DatVariableType.Word);
			constructSpriteBlockId = AddPlacedVariableBlock (construct_sprite_offset, NUM_UNITS, DatVariableType.DWord);
			animationLevelBlockId = AddPlacedVariableBlock (animation_level_offset, NUM_UNITS, DatVariableType.Byte);
			createScoreBlockId = AddPlacedVariableBlock (create_score_offset, NUM_UNITS, DatVariableType.Word);
			destroyScoreBlockId = AddPlacedVariableBlock (destroy_score_offset, NUM_UNITS, DatVariableType.Word);
			shieldBlockId = AddPlacedVariableBlock (shield_offset, NUM_UNITS, DatVariableType.Word);
			hitpointsBlockId = AddPlacedVariableBlock (hitpoint_offset, NUM_UNITS, DatVariableType.DWord);
			portraitBlockId = AddPlacedVariableBlock (portrait_offset, NUM_UNITS, DatVariableType.Word);
			widthBlockId = AddPlacedVariableBlock (width_offset, NUM_UNITS, DatVariableType.Word);
			heightBlockId = AddPlacedVariableBlock (height_offset, NUM_UNITS, DatVariableType.Word);
			compAIIdleBlockId = AddPlacedVariableBlock (compaiidle_offset, NUM_UNITS, DatVariableType.Byte);
			yesSoundStartBlockId = AddPlacedVariableBlock (yessoundstart_offset, 105/*Is this right?*/, DatVariableType.Word);
			yesSoundEndBlockId = AddPlacedVariableBlock (yessoundend_offset, 105/*Is this right?*/, DatVariableType.Word);

			whatSoundStartBlockId = AddPlacedVariableBlock (whatsoundstart_offset, NUM_UNITS, DatVariableType.Word);
			whatSoundEndBlockId = AddPlacedVariableBlock (whatsoundend_offset, NUM_UNITS, DatVariableType.Word);
		}

		/* offsets from the stardat.mpq version */

		public DatCollection<byte> FlingyIds {
			get { return (DatCollection<byte>)GetCollection (flingyBlockId); }
		}

		public DatCollection<uint> ConstructSpriteIds {
			get { return (DatCollection<uint>)GetCollection (constructSpriteBlockId); }
		}

		public DatCollection<byte> AnimationLevels {
			get { return (DatCollection<byte>)GetCollection (animationLevelBlockId); }
		}

		public DatCollection<ushort> CreateScores {
			get { return (DatCollection<ushort>)GetCollection (createScoreBlockId); }
		}

		public DatCollection<ushort> DestroyScores {
			get { return (DatCollection<ushort>)GetCollection (destroyScoreBlockId); }
		}

		public DatCollection<ushort> Shields {
			get { return (DatCollection<ushort>)GetCollection (shieldBlockId); }
		}

		public DatCollection<uint> Hitpoints {
			get { return (DatCollection<uint>)GetCollection (hitpointsBlockId); }
		}

		public DatCollection<ushort> Portraits {
			get { return (DatCollection<ushort>)GetCollection (portraitBlockId); }
		}

		public DatCollection<ushort> Widths {
			get { return (DatCollection<ushort>)GetCollection (widthBlockId); }
		}

		public DatCollection<ushort> Heights {
			get { return (DatCollection<ushort>)GetCollection (heightBlockId); }
		}

		public DatCollection<byte> CompAIIdles {
			get { return (DatCollection<byte>)GetCollection (compAIIdleBlockId); }
		}

		public DatCollection<ushort> YesSoundStarts {
			get { return (DatCollection<ushort>)GetCollection (yesSoundStartBlockId); }
		}

		public DatCollection<ushort> YesSoundEnds {
			get { return (DatCollection<ushort>)GetCollection (yesSoundEndBlockId); }
		}

		public DatCollection<ushort> WhatSoundStarts {
			get { return (DatCollection<ushort>)GetCollection (whatSoundStartBlockId); }
		}

		public DatCollection<ushort> WhatSoundEnds {
			get { return (DatCollection<ushort>)GetCollection (whatSoundEndBlockId); }
		}
	}
}
