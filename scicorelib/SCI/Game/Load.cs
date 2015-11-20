using SCI.Data;
using System;

namespace SCI
{
    public class Game
    {
        /// <summary>
        /// load a compiled game and not the sources and the project file
        /// </summary>
        public CGame LoadGame(string path)
        {
            CGame gamedata = new CGame();
            gamedata.Type = EGameType.None;

            if (System.IO.File.Exists(System.IO.Path.Combine(path, "RESOURCE.MAP")))
            {
                System.IO.FileStream stream = System.IO.File.Open(System.IO.Path.Combine(path, "RESOURCE.MAP"), System.IO.FileMode.Open);
                Int32 value = stream.ReadByte();

                stream.Position = stream.Length - 6;

                byte[] ba = new byte[6];
                stream.Read(ba, 0, 6);
                stream.Close();

                if ((byte)value == 0x80)
                {
                    /* SCI1 */
                    SCI1 game = new SCI1();
                    if (game.Load(path))
                    {
                        gamedata.Type = EGameType.SCI1;
                        gamedata.ResourceList = game.ResourceList;
                        gamedata.GameData = (SciBase)game;
                    }
                }
                else if ((ba[0] == 0xFF) && (ba[1] == 0xFF) && (ba[2] == 0xFF) && (ba[3] == 0xFF) && (ba[4] == 0xFF) && (ba[5] == 0xFF))
                {
                    /* SCI0 */
                    SCI0 game = new SCI0();
                    if (game.Load(path))
                    {
                        gamedata.Type = EGameType.SCI0;
                        gamedata.ResourceList = game.ResourceList;
                        gamedata.GameData = (SciBase)game;
                    }
                }
            }
            else if (System.IO.File.Exists(System.IO.Path.Combine(path, "RESMAP.000")))
            {
                SCI3 game = new SCI3();
                if (game.Load(path))
                {
                    gamedata.Type = EGameType.SCI3;
                    gamedata.ResourceList = game.ResourceList;
                    gamedata.GameData = (SciBase)game;
                }
            }

            return gamedata;
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

                if ((ident == 0xFFFF) && (addr == 0xFFFFFFFF))
                {
                    break;
                }

                total++;
                currentEntry--;
            }
            while (currentEntry > 0);

            stream.Position = pos;
            return total;
        }

        //		public static bool Load(string mapfilepath)
        //		{
        //			if ( !(System.IO.File.Exists(System.IO.Path.Combine(mapfilepath, "RESOURCE.MAP")))
        //				&& !(System.IO.File.Exists(System.IO.Path.Combine(mapfilepath, "RESMAP.000"))) )
        //			{
        //				throw new System.IO.FileNotFoundException();
        //			}
        //			return true;
        //		}

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

        #endregion GameVersions
    }
}
