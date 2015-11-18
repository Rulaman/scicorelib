using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SCI.Resource
{
    public class SciView : CResource
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EGameType GameType;

        public SciView(EGameType gametype)
        {
            GameType = gametype;
        }

        public struct Header56
        {
            public Int16 Len;
            public byte NumOfLoops;
            public byte IsScalable;
            public byte unknown;
            public byte ScaleRes;
            public Int32 PalOffset;
            public byte LoopSize;
            public byte CellSize;
            public byte ViewType;
            public byte SystemType;

            public Int16 Width;
            public Int16 Height;

            public Color[] colorInfo;
            public ColorPalette Palette;
            public Bitmap Image;
        }

        public struct Loop56
        {
            public byte NumOfCells;
            public Int16 CellLoopStart;

            public struct Cell56
            {
                public Int16 Width;
                public Int16 Height;
                public Int16 XPos;
                public Int16 YPos;
                public byte TransparentKey;
                public byte Compression;
                public UInt16 Flags;
                public Int32 OffsetRLE;
                public Int32 OffsetLiteral;
                public byte[] CellData;
                public byte[] ColorData;
                public Bitmap Image;
            }

            public Cell56[] Cell;
        }

        public Header56 Header;
        public Loop56[] Loop;
        public string Filename;
        public string Palname;

        public SciView LoadView(string filename)
        {
            Filename = filename;
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open);
            System.IO.BinaryReader br = new BinaryReader(fs);

            byte[] All = new byte[br.BaseStream.Length];

            Int64 position = br.BaseStream.Position;
            All = br.ReadBytes((Int32)br.BaseStream.Length);
            br.BaseStream.Position = position;

            Header.Len = br.ReadInt16();
            Header.NumOfLoops = br.ReadByte();
            Header.IsScalable = br.ReadByte();
            br.ReadByte();
            Header.ScaleRes = br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            Header.PalOffset = br.ReadInt32();
            Header.LoopSize = br.ReadByte();

            Int64 PalSave = br.BaseStream.Position;

            if (Header.PalOffset == 0) // Palette über externe Datei laden
            {
            }
            else
            {
                br.BaseStream.Position = Header.PalOffset;
                SciPalette pal = new SciPalette(GameType);
                pal.ReadFromStream(br, true);
                Header.colorInfo = pal.ColorInfo;
            }

            br.BaseStream.Position = PalSave;

            Loop = new Loop56[Header.NumOfLoops];

            Header.CellSize = br.ReadByte();
            Header.ViewType = br.ReadByte();
            br.ReadUInt16();
            Header.SystemType = br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();

            br.BaseStream.Position = Header.Len + 4;

            for (int entry = 0; entry < Loop.Length; entry++)
            {
                Loop[entry].NumOfCells = br.ReadByte();
                br.ReadUInt32();
                br.ReadByte();
                br.ReadUInt32();
                Loop[entry].CellLoopStart = br.ReadInt16();

                br.ReadBytes(4);

                Loop[entry].Cell = new Loop56.Cell56[Loop[entry].NumOfCells];
            }

            Int64 Start = 0;
            Int64 Stop = 0;

            for (int entryinner = 0; entryinner < Loop.Length; entryinner++)
            {
                br.BaseStream.Position = Loop[entryinner].CellLoopStart;

                int NumOfCellsCounter = 0;

                //Jetzt Behandlung von anderen Cells in den Loops
                while (NumOfCellsCounter < Loop[entryinner].NumOfCells)
                {
                    Start = br.BaseStream.Position;

                    Loop[entryinner].Cell[NumOfCellsCounter].Width = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].Height = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].XPos = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].YPos = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].TransparentKey = br.ReadByte();
                    Loop[entryinner].Cell[NumOfCellsCounter].Compression = br.ReadByte();
                    Loop[entryinner].Cell[NumOfCellsCounter].Flags = br.ReadUInt16();
                    br.ReadBytes(12);
                    Loop[entryinner].Cell[NumOfCellsCounter].OffsetRLE = br.ReadInt32();
                    Loop[entryinner].Cell[NumOfCellsCounter].OffsetLiteral = br.ReadInt32();
                    Loop[entryinner].Cell[NumOfCellsCounter].CellData = new byte[Loop[entryinner].Cell[NumOfCellsCounter].Height * Loop[entryinner].Cell[NumOfCellsCounter].Width];
                    Loop[entryinner].Cell[NumOfCellsCounter].ColorData = new byte[Loop[entryinner].Cell[NumOfCellsCounter].Height * Loop[entryinner].Cell[NumOfCellsCounter].Width];

                    Stop = br.BaseStream.Position;

                    br.ReadBytes((Int32)(Header.CellSize - (Stop - Start)));

                    NumOfCellsCounter++;
                }
            }

            for (int entryloop = 0; entryloop < Loop.Length; entryloop++)
            {
                for (int entrycell = 0; entrycell < Loop[entryloop].NumOfCells; entrycell++)
                {
                    br.BaseStream.Position = Loop[entryloop].Cell[entrycell].OffsetRLE;
                    Loop[entryloop].Cell[entrycell].CellData = br.ReadBytes(Loop[entryloop].Cell[entrycell].CellData.Length);
                }
            }

            #region Decode
            for (int entryloop = 0; entryloop < Loop.Length; entryloop++)
            {
                for (int entrycell = 0; entrycell < Loop[entryloop].NumOfCells; entrycell++)
                {
                    int rlepos = Loop[entryloop].Cell[entrycell].OffsetRLE;
                    int ltrpos = Loop[entryloop].Cell[entrycell].OffsetLiteral;
                    int counter = 0;

                    while (counter < Loop[entryloop].Cell[entrycell].ColorData.Length)
                    {
                        byte test = All[rlepos++];
                        byte runlen = (byte)(test & 0x3F);

                        switch (test & 0xC0)
                        {
                            case 0x40:
                                {
                                    runlen += 64;

                                    /* do here the same as in 0x00 */
                                    if (ltrpos == 0)
                                    {
                                        Array.Copy(All, rlepos, Loop[entryloop].Cell[entrycell].ColorData, counter, Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        rlepos += runlen;
                                    }
                                    else
                                    {
                                        Array.Copy(All, ltrpos, Loop[entryloop].Cell[entrycell].ColorData, counter, Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        ltrpos += runlen;
                                    }
                                }
                                break;
                            case 0x00: // copy bytes as-is
                                {
                                    if (ltrpos == 0)
                                    {
                                        Array.Copy(All, rlepos, Loop[entryloop].Cell[entrycell].ColorData, counter, Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        rlepos += runlen;
                                    }
                                    else
                                    {
                                        Array.Copy(All, ltrpos, Loop[entryloop].Cell[entrycell].ColorData, counter, Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        ltrpos += runlen;
                                    }
                                }
                                break;
                            case 0x80: // fill with color
                                {
                                    if (ltrpos == 0)
                                    {
                                        for (int i = 0; i < Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter); i++)
                                        {
                                            Loop[entryloop].Cell[entrycell].ColorData[counter + i] = All[rlepos];
                                        }
                                        rlepos++;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter); i++)
                                        {
                                            Loop[entryloop].Cell[entrycell].ColorData[counter + i] = All[ltrpos];
                                        }
                                        ltrpos++;
                                    }
                                }
                                break;
                            case 0xc0:
                                {
                                    // Transparency; skip next pixel
                                }
                                break;
                        };

                        counter += runlen;
                    }
                }
            }
            #endregion Decode

            fs.Close();

            return this;
        }

        public SciView LoadViewSCI1(System.IO.Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            //byte[] All = new byte[br.BaseStream.Length];

            //Int64 position = br.BaseStream.Position;
            //All = br.ReadBytes((Int32)br.BaseStream.Length);
            //br.BaseStream.Position = position;

            Header.Len = br.ReadInt16();
            br.ReadInt16();
            Header.Width = br.ReadInt16();
            Header.Height = br.ReadInt16();
            br.ReadInt16();
            br.ReadInt16();
            br.ReadInt16();
            br.ReadInt16();
            br.ReadBytes(10);
            Header.LoopSize = br.ReadByte();
            br.ReadByte();
            Header.NumOfLoops = br.ReadByte();
            br.ReadByte();

            br.ReadByte();
            br.ReadByte();

            br.ReadBytes(2);

            Header.CellSize = br.ReadByte();
            br.ReadByte();

            br.ReadBytes(8);

            Loop = new Loop56[Header.NumOfLoops];
            /* für Anzahl an Loops 16 Bytes lesen */
            for (int entry = 0; entry < Loop.Length; entry++)
            {
                br.ReadByte();
                br.ReadByte();
                Loop[entry].NumOfCells = br.ReadByte();
                br.ReadUInt32();
                br.ReadByte(); // 0x03
                br.ReadUInt32();
                br.ReadUInt32();

                Loop[entry].Cell = new Loop56.Cell56[Loop[entry].NumOfCells];
            }

            Int64 Start = 0;
            Int64 Stop = 0;

            for (int entryinner = 0; entryinner < Loop.Length; entryinner++)
            {
                int NumOfCellsCounter = 0;

                while (NumOfCellsCounter < Loop[entryinner].NumOfCells)
                {
                    Start = br.BaseStream.Position;

                    Loop[entryinner].Cell[NumOfCellsCounter].Width = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].Height = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].XPos = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].YPos = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].TransparentKey = br.ReadByte();
                    Loop[entryinner].Cell[NumOfCellsCounter].Compression = br.ReadByte();
                    Loop[entryinner].Cell[NumOfCellsCounter].Flags = br.ReadUInt16();
                    br.ReadBytes(12);
                    Loop[entryinner].Cell[NumOfCellsCounter].OffsetRLE = br.ReadInt32();
                    Loop[entryinner].Cell[NumOfCellsCounter].OffsetLiteral = br.ReadInt32();
                    Loop[entryinner].Cell[NumOfCellsCounter].CellData = new byte[Loop[entryinner].Cell[NumOfCellsCounter].Height * Loop[entryinner].Cell[NumOfCellsCounter].Width];
                    Loop[entryinner].Cell[NumOfCellsCounter].ColorData = new byte[Loop[entryinner].Cell[NumOfCellsCounter].Height * Loop[entryinner].Cell[NumOfCellsCounter].Width];

                    Stop = br.BaseStream.Position;
                    //br.ReadBytes(20);
                    br.ReadBytes((Int32)(Header.CellSize - (Stop - Start)));

                    NumOfCellsCounter++;
                }
            }
            br.ReadBytes(8);

            /* ab jetzt kommt der Text Picture */

            return this;
        }

        public SciView LoadViewSCI11(System.IO.Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            byte[] All = new byte[br.BaseStream.Length];

            Int64 position = br.BaseStream.Position;
            All = br.ReadBytes((Int32)br.BaseStream.Length);
            br.BaseStream.Position = position;

            Header.Len = br.ReadInt16();
            Header.NumOfLoops = br.ReadByte();
            Header.IsScalable = br.ReadByte();
            br.ReadByte();
            Header.ScaleRes = br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            Header.PalOffset = br.ReadInt32();
            Header.LoopSize = br.ReadByte();

            Int64 PalSave = br.BaseStream.Position;

            if (Header.PalOffset == 0) // Palette über externe Datei laden
            {
            }
            else
            {
                br.BaseStream.Position = Header.PalOffset;
                SciPalette pal = new SciPalette(GameType);
                pal.ReadFromStream(br, true);
                Header.colorInfo = pal.ColorInfo;

                Header.Image = new Bitmap(16, 16, PixelFormat.Format8bppIndexed);
                Header.Palette = Header.Image.Palette;

                for (int pos = 0; pos < 256; pos++)
                {
                    Header.Palette.Entries[pos] = Header.colorInfo[pos];
                }
            }

            br.BaseStream.Position = PalSave;

            Loop = new Loop56[Header.NumOfLoops];

            Header.CellSize = br.ReadByte();
            Header.ViewType = br.ReadByte();
            br.ReadUInt16();
            Header.SystemType = br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();
            br.ReadByte();

            br.BaseStream.Position = Header.Len + 4; //0x16; <- war noch bei Larry // Start der CellLoopStart

            for (int entry = 0; entry < Loop.Length; entry++)
            {
                Loop[entry].NumOfCells = br.ReadByte();
                br.ReadUInt32();
                br.ReadByte();
                br.ReadUInt32();
                Loop[entry].CellLoopStart = br.ReadInt16();

                br.ReadBytes(4);

                Loop[entry].Cell = new Loop56.Cell56[Loop[entry].NumOfCells];
            }

            Int64 Start = 0;
            Int64 Stop = 0;

            for (int entryinner = 0; entryinner < Loop.Length; entryinner++)
            {
                br.BaseStream.Position = Loop[entryinner].CellLoopStart;

                int NumOfCellsCounter = 0;

                //Jetzt Behandlung von anderen Cells in den Loops
                while (NumOfCellsCounter < Loop[entryinner].NumOfCells)
                {
                    Start = br.BaseStream.Position;

                    Loop[entryinner].Cell[NumOfCellsCounter].Width = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].Height = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].XPos = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].YPos = br.ReadInt16();
                    Loop[entryinner].Cell[NumOfCellsCounter].TransparentKey = br.ReadByte();
                    Loop[entryinner].Cell[NumOfCellsCounter].Compression = br.ReadByte();
                    Loop[entryinner].Cell[NumOfCellsCounter].Flags = br.ReadUInt16();
                    br.ReadBytes(12);
                    Loop[entryinner].Cell[NumOfCellsCounter].OffsetRLE = br.ReadInt32();
                    Loop[entryinner].Cell[NumOfCellsCounter].OffsetLiteral = br.ReadInt32();
                    Loop[entryinner].Cell[NumOfCellsCounter].CellData = new byte[Loop[entryinner].Cell[NumOfCellsCounter].Height * Loop[entryinner].Cell[NumOfCellsCounter].Width];
                    Loop[entryinner].Cell[NumOfCellsCounter].ColorData = new byte[Loop[entryinner].Cell[NumOfCellsCounter].Height * Loop[entryinner].Cell[NumOfCellsCounter].Width];

                    Stop = br.BaseStream.Position;
                    //br.ReadBytes(20);
                    br.ReadBytes((Int32)(Header.CellSize - (Stop - Start)));

                    NumOfCellsCounter++;
                }
            }

            for (int entryloop = 0; entryloop < Loop.Length; entryloop++)
            {
                for (int entrycell = 0; entrycell < Loop[entryloop].NumOfCells; entrycell++)
                {
                    br.BaseStream.Position = Loop[entryloop].Cell[entrycell].OffsetRLE;
                    Loop[entryloop].Cell[entrycell].CellData = br.ReadBytes(Loop[entryloop].Cell[entrycell].CellData.Length);
                }
            }

            #region Decode
            for (int entryloop = 0; entryloop < Loop.Length; entryloop++)
            {
                for (int entrycell = 0; entrycell < Loop[entryloop].NumOfCells; entrycell++)
                {
                    int rlepos = Loop[entryloop].Cell[entrycell].OffsetRLE;
                    int ltrpos = Loop[entryloop].Cell[entrycell].OffsetLiteral;
                    int counter = 0;

                    while (counter < Loop[entryloop].Cell[entrycell].ColorData.Length)
                    {
                        byte test = All[rlepos++];
                        byte runlen = (byte)(test & 0x3F);

                        switch (test & 0xC0)
                        {
                            case 0x40:
                                {
                                    runlen += 64;

                                    /* do here the same as in 0x00 */
                                    if (ltrpos == 0)
                                    {
                                        Array.Copy(All, rlepos, Loop[entryloop].Cell[entrycell].ColorData, counter, Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        rlepos += runlen;
                                    }
                                    else
                                    {
                                        Array.Copy(All, ltrpos, Loop[entryloop].Cell[entrycell].ColorData, counter, Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        ltrpos += runlen;
                                    }
                                }
                                break;
                            case 0x00: // copy bytes as-is
                                {
                                    if (ltrpos == 0)
                                    {
                                        Array.Copy(All, rlepos, Loop[entryloop].Cell[entrycell].ColorData, counter, Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        rlepos += runlen;
                                    }
                                    else
                                    {
                                        Array.Copy(All, ltrpos, Loop[entryloop].Cell[entrycell].ColorData, counter, Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        ltrpos += runlen;
                                    }
                                }
                                break;
                            case 0x80: // fill with color
                                {
                                    if (ltrpos == 0)
                                    {
                                        for (int i = 0; i < Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter); i++)
                                        {
                                            Loop[entryloop].Cell[entrycell].ColorData[counter + i] = All[rlepos];
                                        }
                                        rlepos++;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter); i++)
                                        {
                                            Loop[entryloop].Cell[entrycell].ColorData[counter + i] = All[ltrpos];
                                        }
                                        ltrpos++;
                                    }
                                }
                                break;
                            case 0xc0:
                                {
                                    // Transparency; skip next pixel
                                }
                                break;
                        };

                        counter += runlen;
                    }
                }
            }
            #endregion Decode

            return this;
        }

        public Color[] DecodeColors(string filename)
        {
            Palname = filename;
            SciPalette palette = new SciPalette(GameType);
            return DecodeColors(palette.ReadFromSierraPalFile(filename));
        }

        public Color[] DecodeColors()
        {
            return DecodeColors(this.Header.colorInfo);
        }

        public Color[] DecodeColors(Color[] colorinfo)
        {
            for (int entryloop = 0; entryloop < Loop.Length; entryloop++)
            {
                for (int entrycell = 0; entrycell < Loop[entryloop].NumOfCells; entrycell++)
                {
                    Bitmap b = new Bitmap(Loop[entryloop].Cell[entrycell].Width, Loop[entryloop].Cell[entrycell].Height, PixelFormat.Format8bppIndexed);
                    ColorPalette palette = b.Palette;

                    for (int i = 0; i < Math.Min(256, colorinfo.Length); i++)
                    {
                        palette.Entries[i] = colorinfo[i];
                    }

                    b.Palette = palette;
                    BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
                    IntPtr ptr = bmpData.Scan0;
                    Int32 startPos = Loop[entryloop].Cell[entrycell].ColorData.Length / bmpData.Height;

                    for (int height = 0; height < b.Height; height++)
                    {
                        System.Runtime.InteropServices.Marshal.Copy(Loop[entryloop].Cell[entrycell].ColorData, startPos * height, ptr, bmpData.Width);
                        ptr = (IntPtr)((long)ptr + bmpData.Stride);
                    }

                    b.UnlockBits(bmpData);
                    Loop[entryloop].Cell[entrycell].Image = b;
                }
            }

            return colorinfo;
        }
    }
}
