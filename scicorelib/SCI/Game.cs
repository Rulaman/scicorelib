using System;
using System.Collections.Generic;
using System.Text;

using SCI.Data;

namespace SCI
{
	public static class Game
	{
		static public SCI.Data.SCIVersion[] Versions = new SCI.Data.SCIVersion[ ] 
		{
			new SCI.Data.SCIVersion(
 				"SCI 0.000.xxx (ORIGINAL)",
   				SciTypes.SCI_00_EARLY,
				CL.CL_EGA,
				ST.ST_SCI0a,
				VT.VT_EGA,
				PT.PT_EGA00,
				CT.CT_2COL,
				FT.FT_STD,
				WT.WT_00,  
				PaletteType.NONE,
				MT.MT_0,
				2,
				Compression.DECOMP_0,
				rf.VIEW|rf.PIC|rf.SCRIPT|rf.TEXT|rf.SOUND|rf.VOCAB|rf.FONT|rf.CURSOR|rf.PATCH
			),
			new SCI.Data.SCIVersion(
 				"SCI 0.000.xxx (SECOND GEN)",
   				SciTypes.SCI_00_LATE,
				CL.CL_EGA,
				ST.ST_SCI0b,
				VT.VT_EGA,
				PT.PT_EGA00,
				CT.CT_2COL,
				FT.FT_STD,  
				WT.WT_00,  
				PaletteType.NONE,
				MT.MT_0,
				2,
				Compression.DECOMP_0,
				rf.VIEW|rf.PIC|rf.SCRIPT|rf.TEXT|rf.SOUND|rf.VOCAB|rf.FONT|rf.CURSOR|rf.PATCH
			),
			new SCI.Data.SCIVersion(
 				"SCI 1.000.xxx (EGA,ORIGINAL)",
   				SciTypes.SCI_10_EGA_EARLY,
				CL.CL_EGA,
				ST.ST_SCI1,
				VT.VT_EGA,
				PT.PT_EGA10,
				CT.CT_2COL,
				FT.FT_EXT,
				WT.WT_10,   
				PaletteType.NONE,
				MT.MT_0,
				2,
				Compression.DECOMP_0,
				rf.VIEW|rf.PIC|rf.SCRIPT|rf.TEXT|rf.SOUND|rf.VOCAB|rf.FONT|rf.CURSOR|rf.PATCH
			),
			new SCI.Data.SCIVersion(
 				"SCI 1.000.xxx (EGA,SECOND GEN)",
   				SciTypes.SCI_10_EGA_LATE,
				CL.CL_EGA,
				ST.ST_SCI1,
				VT.VT_EGA,
				PT.PT_EGA10,
				CT.CT_2COL,
				FT.FT_EXT, 
				WT.WT_10,  
				PaletteType.NONE,
				MT.MT_0,
				3,
				Compression.DECOMP_1,
				rf.VIEW|rf.PIC|rf.SCRIPT|rf.TEXT|rf.SOUND|rf.VOCAB|rf.FONT|rf.CURSOR|rf.PATCH
			),
			new SCI.Data.SCIVersion(
 				"SCI 1.000.xxx (EGA,THIRD GEN)",
   				SciTypes.SCI_10_VGA_EARLY,
				CL.CL_EGA,
				ST.ST_SCI1,
				VT.VT_EGA,
				PT.PT_EGA10,
				CT.CT_2COL,
				FT.FT_EXT,
				WT.WT_10,
				PaletteType.NONE,
				MT.MT_1,
				4,
				Compression.DECOMP_2,
				rf.VIEW|rf.PIC|rf.SCRIPT|rf.TEXT|rf.SOUND|rf.VOCAB|rf.FONT|rf.CURSOR|rf.PATCH|rf.PALETTE
			),
			new SCI.Data.SCIVersion(
 				"SCI 1.000.xxx (VGA,ORIGINAL)",
   				SciTypes.SCI_10_VGA_EARLY,
				CL.CL_VGA,
				ST.ST_SCI1,
				VT.VT_VGA10,
				PT.PT_VGA10,
				CT.CT_3COL,
				FT.FT_EXT,
				WT.NONE,
				PaletteType.PALT_10,
				MT.MT_1,
				4,
				Compression.DECOMP_2,
				rf.VIEW|rf.PIC|rf.SCRIPT|rf.TEXT|rf.SOUND|rf.VOCAB|rf.FONT|rf.CURSOR|rf.PATCH|rf.PALETTE
			),
			new SCI.Data.SCIVersion(
 				"SCI 1.000.xxx (AMIGA)",
   				SciTypes.SCI_10_AMIGA,
				CL.CL_AMIGA,
				ST.ST_SCI1,
				VT.VT_AMIGA10,
				PT.PT_AMIGA10,
				CT.CT_3COL,
				FT.FT_EXT,
				WT.NONE,
				PaletteType.PALT_AMIGA,
				MT.MT_1,
				4,
				Compression.DECOMP_2,
				rf.VIEW|rf.PIC|rf.SCRIPT|rf.TEXT|rf.SOUND|rf.VOCAB|rf.FONT|rf.CURSOR|rf.PATCH|rf.PALETTE
			),
			new SCI.Data.SCIVersion(
 				"SCI 1.000.xxx (VGA,SECOND GEN)",
   				SciTypes.SCI_10_VGA_LATE,
				CL.CL_VGA,
				ST.ST_SCI1,
				VT.VT_VGA10,
				PT.PT_VGA10,
				CT.CT_3COL,
				FT.FT_EXT,
				WT.NONE,
				PaletteType.PALT_10,
				MT.MT_2,
				4,
				Compression.DECOMP_2,
				rf.VIEW|rf.PIC|rf.SCRIPT|rf.TEXT|rf.SOUND|rf.VOCAB|rf.FONT|rf.CURSOR|rf.PATCH|rf.PALETTE
			),
			new SCI.Data.SCIVersion(
 				"SCI 1.001.xxx (ORIGINAL)",
   				SciTypes.SCI_11_ORIGINAL,
				CL.CL_VGA,
				ST.ST_SCI11,
				VT.VT_VGA32,
				PT.PT_VGA11,
				CT.CT_3COL,
				FT.FT_EXT,   
				WT.NONE,
				PaletteType.PALT_11,
				MT.MT_3,
				123,
				Compression.DECOMP_3,
				rf.VIEW|rf.PIC|rf.SCRIPT|rf.TEXT|rf.SOUND|rf.VOCAB|rf.FONT|rf.CURSOR|rf.PATCH|
				rf.BITMAP|rf.PALETTE|rf.CDAUDIO|rf.AUDIO|rf.SYNC|rf.MESSAGE|rf.MAP|rf.HEAP
			),
			new SCI.Data.SCIVersion(
 				"SCI32 (THE ULTIMATE SCI!)",
   				SciTypes.SCI_32,
				CL.CL_VGA,
				ST.ST_SCI32,
				VT.VT_VGA32,
				PT.PT_VGA32,
				CT.CT_3COL,
				FT.FT_EXT,   
				WT.NONE,        
				PaletteType.PALT_11,
				MT.MT_4,
				123,
				Compression.DECOMP_4,
				rf.VIEW|rf.PIC|rf.SCRIPT|rf.TEXT|rf.SOUND|rf.VOCAB|rf.FONT|rf.CURSOR|rf.PATCH|
				rf.BITMAP|rf.PALETTE|rf.CDAUDIO|rf.AUDIO|rf.SYNC|rf.MESSAGE|rf.MAP|rf.HEAP|
				rf.CHUNK|rf.AUDIO36|rf.SYNC36|rf.TRANSLATION|rf.ROBOT|rf.VMD|rf.DUCK|rf.CLUT|
				rf.TARGA|rf.ZZZ
			),
			new SCI.Data.SCIVersion(
 				"LSCI (On-Line SCI)",
   				SciTypes.SCI_L,
				CL.CL_VGA,
				ST.ST_SCI1,
				VT.VT_VGA10,
				PT.PT_VGA10,
				CT.CT_3COL,
				FT.FT_EXT,
				WT.NONE,       
				PaletteType.PALT_10,
				MT.MT_1,
				8,
				Compression.DECOMP_5,
				rf.VIEW|rf.PIC|rf.SCRIPT|rf.TEXT|rf.SOUND|rf.VOCAB|rf.FONT|rf.CURSOR|rf.PATCH|rf.PALETTE
			)
		};
	}
}
