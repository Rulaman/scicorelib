using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SCI.Data
{
	public enum SciTypes
	{
		SCI_00_EARLY		= 0,  // like KQ4
		SCI_00_LATE			= 1,  // like LSL2/3
		SCI_10_EGA_EARLY	= 2,  // like KQ1 remake
		SCI_10_EGA_LATE		= 3,  // like QFG 2

		//-- the freaked up version which is the exact same as the normal
		//   VGA interpreter, but it uses EGA graphic resources
		SCI_10_EGA_LATEST	= 4,  // like JITFL EGA

		SCI_10_VGA_EARLY	= 5,  // like LSL1VGA
		SCI_10_AMIGA		= 6,
		SCI_10_VGA_LATE		= 7,  // like LSL5
		SCI_11_ORIGINAL		= 8,  // like LSL6
		SCI_32				= 9,  // like LSL7
		SCI_L				= 10,  // INN
		TOTAL				= 11,
	}
	public enum CL
	{
		CL_EGA				= 1,
		CL_VGA				= 2,
		CL_AMIGA			= 3,
	}
	public enum Compression
	{
		DECOMP_0			= 0,
		DECOMP_1			= 1,
		DECOMP_2			= 2,
		DECOMP_3			= 3,
		DECOMP_4			= 4,
		DECOMP_5			= 5,
	}
	public enum MT
	{
		MT_0				= 0,
		MT_1				= 1,
		MT_2				= 2,
		MT_3				= 3,
		MT_4				= 4,
	}
	public enum PaletteType
	{
		NONE = -1,
		PALT_10				= 1,
		PALT_AMIGA			= 2,
		PALT_11				= 3,
	}
	public enum ST
	{
		ST_SCI0a = 1,
		ST_SCI0b = 2,
		ST_SCI1 = 3,
		ST_SCI11 = 4,
		ST_SCI32 = 5,
	}
	public enum VT
	{
		VT_EGA = 1,
		VT_VGA10 = 2,
		VT_AMIGA10 = 3,
		VT_VGA11 = 4,
		VT_VGA32 = 5,
	}
	public enum PT
	{
		PT_EGA00 = 1,
		PT_EGA10 = 2,
		PT_VGA10 = 3,
		PT_AMIGA10 = 4,
		PT_VGA11 = 5,
		PT_VGA32 = 6,
	}
	public enum CT
	{
		CT_2COL = 1,
		CT_3COL = 2,
	}
	public enum FT
	{
		FT_STD = 1,
		FT_EXT = 2,
	}
	public enum WT
	{
		NONE = -1,
		WT_00				= 1,
		WT_10				= 2,
	}

	[Flags]
	public enum rf
	{
		VIEW                  = 0x00000001,
		PIC                   = 0x00000002,
		SCRIPT                = 0x00000004,
		TEXT                  = 0x00000008,
		SOUND                 = 0x00000010,
		MEMORY                = 0x00000020,
		VOCAB                 = 0x00000040,
		FONT                  = 0x00000080,
		CURSOR                = 0x00000100,
		PATCH                 = 0x00000200,
		BITMAP                = 0x00000400,
		PALETTE               = 0x00000800,
		CDAUDIO              	= 0x00001000,
		AUDIO              	= 0x00002000,
		SYNC		           	= 0x00004000,
		MESSAGE           	= 0x00008000,
		MAP		         	= 0x00010000,
		HEAP		         	= 0x00020000,
		CHUNK					= 0x00040000,
		AUDIO36				= 0x00080000,
		SYNC36				= 0x00100000,
		TRANSLATION			= 0x00200000,
		ROBOT					= 0x00400000,
		VMD					= 0x00800000,
		DUCK					= 0x01000000,
		CLUT					= 0x02000000,
		TARGA					= 0x04000000,
		ZZZ					= 0x08000000,
	}

	public class SCIVersion
	{
		public string Name;
		public SciTypes Num;
		public CL Colours;
		public ST ScrType;
		public VT ViewType;
		public PT PicType;
		public CT CursorType;
		public FT FontType;
		public WT WordsType;
		public PaletteType PalType;
		public MT MapType;
		public UInt16 MaxEnc;
		public Compression DecType;
		public rf Types;

		public SCIVersion(string name, SciTypes num, CL colours,
			ST scrType, VT viewType, PT picType, CT cursorType,
			FT fontType, WT wordsType, PaletteType palType, MT mapType,
			UInt16 maxEnc, Compression decType, rf types)
		{
			Name = name;
			Num = num;
			Colours = colours;
			ScrType = scrType;
			ViewType = viewType;
			PicType = picType;
			CursorType = cursorType;
			FontType = fontType;
			WordsType = wordsType;
			PalType = palType;
			MapType = mapType;
			MaxEnc = maxEnc;
			DecType = decType;
			Types = types;
		}
	}
	public class GameInfo
	{
		public string Path;
		public string Name;
		public UInt16 Flags;
		public byte DefaultPack;
		public SCIVersion Version;
	}
}
