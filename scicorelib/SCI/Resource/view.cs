namespace SCI.Resource
{
    public class View : CResource
    {
		public override EResourceType ResourceType
		{
			get { return EResourceType.View; }
		}

        public View(EGameType gametype) : base(gametype) { }

        public struct Header56
        {
            public short Len;
            public byte NumOfLoops;
            public byte IsScalable;
            public byte unknown;
            public byte ScaleRes;
            public int PalOffset;
            public byte LoopSize;
            public byte CellSize;
            public byte ViewType;
            public byte SystemType;

            public short Width;
            public short Height;

            public System.Drawing.Color[] colorInfo;
            public System.Drawing.Imaging.ColorPalette Palette;
            public System.Drawing.Bitmap Image;
        }

        public struct Loop56
        {
            public byte NumOfCells;
            public short CellLoopStart;

            public struct Cell56
            {
                public short Width;
                public short Height;
                public short XPos;
                public short YPos;
                public byte TransparentKey;
                public byte Compression;
                public ushort Flags;
                public int OffsetRLE;
                public int OffsetLiteral;
                public byte[] CellData;
                public byte[] ColorData;
                public System.Drawing.Bitmap Image;
            }

            public Cell56[] Cell;
        }

        public Header56 Header;
        public Loop56[] Loop;
        public string Filename;
        public string Palname;

        public View LoadView(string filename)
        {
            Filename = filename;
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open);
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);

            byte[] All = new byte[br.BaseStream.Length];

            long position = br.BaseStream.Position;
            All = br.ReadBytes((int)br.BaseStream.Length);
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

            long PalSave = br.BaseStream.Position;

            if (Header.PalOffset == 0) // Palette über externe Datei laden
            {
            }
            else
            {
                br.BaseStream.Position = Header.PalOffset;
                Palette pal = new Palette(GameType);
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

            long Start = 0;
            long Stop = 0;

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

                    br.ReadBytes((int)(Header.CellSize - (Stop - Start)));

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
										System.Array.Copy(All, rlepos, Loop[entryloop].Cell[entrycell].ColorData, counter, System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        rlepos += runlen;
                                    }
                                    else
                                    {
										System.Array.Copy(All, ltrpos, Loop[entryloop].Cell[entrycell].ColorData, counter, System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        ltrpos += runlen;
                                    }
                                }
                                break;
                            case 0x00: // copy bytes as-is
                                {
                                    if (ltrpos == 0)
                                    {
										System.Array.Copy(All, rlepos, Loop[entryloop].Cell[entrycell].ColorData, counter, System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        rlepos += runlen;
                                    }
                                    else
                                    {
										System.Array.Copy(All, ltrpos, Loop[entryloop].Cell[entrycell].ColorData, counter, System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        ltrpos += runlen;
                                    }
                                }
                                break;
                            case 0x80: // fill with color
                                {
                                    if (ltrpos == 0)
                                    {
                                        for (int i = 0; i < System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter); i++)
                                        {
                                            Loop[entryloop].Cell[entrycell].ColorData[counter + i] = All[rlepos];
                                        }
                                        rlepos++;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter); i++)
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

        public View LoadViewSCI1(System.IO.Stream stream)
        {
			System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
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

            long Start = 0;
            long Stop = 0;

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
                    br.ReadBytes((int)(Header.CellSize - (Stop - Start)));

                    NumOfCellsCounter++;
                }
            }
            br.ReadBytes(8);

            /* ab jetzt kommt der Text Picture */

            return this;
        }

        public View LoadViewSCI11(System.IO.Stream stream)
        {
			System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
            byte[] All = new byte[br.BaseStream.Length];

            long position = br.BaseStream.Position;
            All = br.ReadBytes((int)br.BaseStream.Length);
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

            long PalSave = br.BaseStream.Position;

            if (Header.PalOffset == 0) // Palette über externe Datei laden
            {
            }
            else
            {
                br.BaseStream.Position = Header.PalOffset;
                Palette pal = new Palette(GameType);
                pal.ReadFromStream(br, true);
                Header.colorInfo = pal.ColorInfo;

                Header.Image = new System.Drawing.Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
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

			long Start = 0;
			long Stop = 0;

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
                    br.ReadBytes((int)(Header.CellSize - (Stop - Start)));

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
										System.Array.Copy(All, rlepos, Loop[entryloop].Cell[entrycell].ColorData, counter, System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        rlepos += runlen;
                                    }
                                    else
                                    {
										System.Array.Copy(All, ltrpos, Loop[entryloop].Cell[entrycell].ColorData, counter, System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        ltrpos += runlen;
                                    }
                                }
                                break;
                            case 0x00: // copy bytes as-is
                                {
                                    if (ltrpos == 0)
                                    {
										System.Array.Copy(All, rlepos, Loop[entryloop].Cell[entrycell].ColorData, counter, System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        rlepos += runlen;
                                    }
                                    else
                                    {
										System.Array.Copy(All, ltrpos, Loop[entryloop].Cell[entrycell].ColorData, counter, System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter));
                                        ltrpos += runlen;
                                    }
                                }
                                break;
                            case 0x80: // fill with color
                                {
                                    if (ltrpos == 0)
                                    {
                                        for (int i = 0; i < System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter); i++)
                                        {
                                            Loop[entryloop].Cell[entrycell].ColorData[counter + i] = All[rlepos];
                                        }
                                        rlepos++;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < System.Math.Min(runlen, Loop[entryloop].Cell[entrycell].ColorData.Length - counter); i++)
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

        public System.Drawing.Color[] DecodeColors(string filename)
        {
            Palname = filename;
            Palette palette = new Palette(GameType);
            return DecodeColors(palette.ReadFromSierraPalFile(filename));
        }

        public System.Drawing.Color[] DecodeColors()
        {
            return DecodeColors(this.Header.colorInfo);
        }

        public System.Drawing.Color[] DecodeColors(System.Drawing.Color[] colorinfo)
        {
            for (int entryloop = 0; entryloop < Loop.Length; entryloop++)
            {
                for (int entrycell = 0; entrycell < Loop[entryloop].NumOfCells; entrycell++)
                {
					System.Drawing.Bitmap b = new System.Drawing.Bitmap(Loop[entryloop].Cell[entrycell].Width, Loop[entryloop].Cell[entrycell].Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
					System.Drawing.Imaging.ColorPalette palette = b.Palette;

                    for (int i = 0; i < System.Math.Min(256, colorinfo.Length); i++)
                    {
                        palette.Entries[i] = colorinfo[i];
                    }

                    b.Palette = palette;
					System.Drawing.Imaging.BitmapData bmpData = b.LockBits(new System.Drawing.Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, b.PixelFormat);
					System.IntPtr ptr = bmpData.Scan0;
                    int startPos = Loop[entryloop].Cell[entrycell].ColorData.Length / bmpData.Height;

                    for (int height = 0; height < b.Height; height++)
                    {
                        System.Runtime.InteropServices.Marshal.Copy(Loop[entryloop].Cell[entrycell].ColorData, startPos * height, ptr, bmpData.Width);
                        ptr = (System.IntPtr)((long)ptr + bmpData.Stride);
                    }

                    b.UnlockBits(bmpData);
                    Loop[entryloop].Cell[entrycell].Image = b;
                }
            }

            return colorinfo;
        }
    }
}
