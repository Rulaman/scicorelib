using System;
using System.Collections.Generic;
using System.Text;

using SCI.Data;
using SCI.GameVersion;

namespace SCI
{
	public class Game
	{
		private CMapResourceIndex ReadSCI1(System.IO.Stream stream)
		{
			SCI.SciBinaryReader mapFileReader = new SciBinaryReader(stream);

			/* ? muss noch geswappt werden ? */
			mapFileReader.ReverseReading = false;

			CMapResourceIndex resourceindex = new CMapResourceIndex();

			byte restype = 0;
			/*SCI 1*/
			Dictionary<byte,int> resourcearray = new Dictionary<byte, int>();
			List<int> offsetlist = new List<int>();
			while ( 0xFF != (restype = mapFileReader.ReadByte()) )
			{
				UInt16 offset = mapFileReader.ReadUInt16();

				resourcearray.Add(restype, offset);
				offsetlist.Add(offset);
			}

			int i = 0;
			foreach ( KeyValuePair<byte,int> item in resourcearray )
			{
				mapFileReader.BaseStream.Position = item.Value;

				long off2;

				if(i < offsetlist.Count-1)
				{
					off2 = offsetlist[(byte)(i + 1)];
				}
				else
				{
					off2 = mapFileReader.BaseStream.Length;
				}


				while ( mapFileReader.BaseStream.Position < off2 )
				{
					CResource resource = new CResource();

					resource.ResourceType = item.Key;
					resource.ResourceType2 = (EResourceType)item.Key;

					resource.ResourceNumber = mapFileReader.ReadUInt16();
					UInt32 temp = mapFileReader.ReadUInt32();

					resource.FileNumber = (byte)(temp >> 28);
					resource.Offset = (Int32)(temp & 0xFFFFFFF);

					resourceindex.ResourceList.Add(resource);
				}
				i++;
			}
			
			return resourceindex;
		}

		/// <summary>
		/// load a compiled game and not the sources and the project file
		/// </summary>
		public CGameData LoadGame(string path)
		{
			CGameData gamedata = new CGameData();
			gamedata.Type = EGameType.None;

			string gamedir = System.IO.Path.GetDirectoryName(path);
			string filename = System.IO.Path.GetFileName(path);
			

			if ( filename.IsNullOrEmpty() )
			{
				/* try to open RESOURCE.MAP or RESMAP.000 */
				if ( System.IO.File.Exists(System.IO.Path.Combine(gamedir, "RESOURCE.MAP")) )
				{
					filename = "RESOURCE.MAP";
				}
				else if ( System.IO.File.Exists(System.IO.Path.Combine(gamedir, "RESMAP.000")) )
				{
					filename = "RESMAP.000";
				}
			}
			else if ( !System.IO.File.Exists(System.IO.Path.Combine(gamedir, filename)) )
			{
				filename = "";
			}

			if ( filename.IsNullOrEmpty() )
			{
			}
			else
			{
				/* try to load game now */
				System.IO.FileStream fs = System.IO.File.Open(System.IO.Path.Combine(gamedir, filename), System.IO.FileMode.Open);
				byte[] filearray = new byte[fs.Length];
				fs.Read(filearray, 0, filearray.Length);
				fs.Close();

				System.IO.MemoryStream ms = new System.IO.MemoryStream(filearray);

				gamedata.ResourcenIndex = ReadSCI1(ms);
				





				//for ( int i = 0; i < Common.TOTAL_RES_TYPES; i++ )
				//{
				//	mapFileReader.BaseStream.Position = 0;
				//	bool first = true;

				//	for ( int j = 0; j < gamedata.MapFileEntries; j++ )
				//	{
				//		UInt16 type = mapFileReader.ReadUInt16();

				//		if ( i == GetType(type) )
				//		{
				//			/* Add Resource */
				//			CResourceInfo ri = new CResourceInfo();



				//			if ( first )
				//			{
				//				first = false;
				//				//gamedata.ResourcenIndex.ResourceInfo = new List<CResourceInfo>();
				//				//gamedata.ResourcenIndex.ResourceInfo.Add(ri);
				//			}
				//		}
				//	}
				//}


			}


			return gamedata;
		}

		public static CResourceInfo AddResInfo(CResourceIndex residx)
		{
			CResourceInfo ri = new CResourceInfo();

			if ( !(residx.LastAlloc == null) || (residx.AllocPtr + 1) >= Common.rsALLOCBUFSZ )
			{
			}
			else
			{
				residx.AllocPtr++;
			}

			//residx.LastAlloc++;



			return ri;
		}


		public static byte GetType(UInt16 value)
		{
			return (byte)((value >> 11) & 0x1F);
		}
		public static byte GetPackage(UInt32 value)
		{
			return (byte)((value >> 26) & 0x3F);
		}
		public static Int16 GetNumber(UInt16 value)
		{
			return (Int16)((value) & 0x7FFF);
		}

		public static Int32 GetMapFileEntries(System.IO.Stream stream, bool swap)
		{
			Int64 pos = stream.Position;
			SCI.SciBinaryReader mapFileReader = new SciBinaryReader(stream);
			/* ? muss noch geswappt werden ? */
			mapFileReader.ReverseReading = swap;

			Int64 currentEntry = stream.Length / 6;

			UInt16 ident = 0;
			UInt32 addr = 0;
			Int32 total = 0;

			do
			{
				ident = mapFileReader.ReadUInt16();
				addr = mapFileReader.ReadUInt32();

				if ( (ident == 0xFFFF) && (addr == 0xFFFFFFFF) )
				{
					break;
				}

			
				total++;
				currentEntry--;
			} 
			while ( currentEntry > 0 );
			
			stream.Position = pos;
			return total;
		}

		public static bool Load(string mapfilepath)
		{
			bool bSCI32Map = false;
			bool bSCIMap = false;

			if ( !(bSCIMap = System.IO.File.Exists(System.IO.Path.Combine(mapfilepath, "RESOURCE.MAP")))
				&& !(bSCI32Map = System.IO.File.Exists(System.IO.Path.Combine(mapfilepath, "RESMAP.000"))) )
			{
				throw new System.IO.FileNotFoundException();
			}
			return true;
		}



		#region GameVersions
		static public SCI.Data.SCIVersion[] Versions = new SCI.Data.SCIVersion[] 
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
		#endregion
	}

	
}
