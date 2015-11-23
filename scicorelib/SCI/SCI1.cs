using SCI.Resource;
using System;
using System.Collections.Generic;

namespace SCI
{
    public class SCI1 : SciBase
    {
        /// <summary>
        /// load a compiled game and not the sources and the project file
        /// give only the path as the parameter
        /// </summary>
        public override bool Expand(string path)
        {
            string mapfilename = "RESOURCE.MAP";
            string resourcefilename = "RESOURCE";

            bool retval = true;
            System.IO.FileStream stream;

            string file = System.IO.Path.Combine(path, mapfilename);

            if (!System.IO.File.Exists(file))
            {
                return false;
            }
            else
            {
                /* try to load game now */
                stream = System.IO.File.Open(file, System.IO.FileMode.Open);
                byte[] filearray = new byte[stream.Length];
                stream.Read(filearray, 0, filearray.Length);
                stream.Close();

                System.IO.MemoryStream ms = new System.IO.MemoryStream(filearray);

                ReadMapFile(ms, path);

                string resfilesave = "";

                foreach (CResource item in ResourceList)
                {
                    string resfile = System.IO.Path.Combine(path, resourcefilename + String.Format(".{0}", item.FileNumber.ToString("000")));

                    if (!System.IO.File.Exists(file))
                    {
                        continue;
                    }
                    else
                    {
                        if (resfilesave != resfile)
                        {
                            stream.Close();
                            stream = System.IO.File.Open(resfile, System.IO.FileMode.Open);

                            resfilesave = resfile;
                        }

                        stream.Position = item.FileOffset;

						/* Resource entpacken */
						IO.SciBinaryReader br = new IO.SciBinaryReader(stream);
                        /* byte typ = */
                        br.ReadByte();
                        /* UInt16 id = */
                        br.ReadUInt16();

                        item.CompressedSize = br.ReadUInt16();
                        item.UncompressedSize = br.ReadUInt16();
                        int pakmeth = br.ReadUInt16();

                        byte[] UnpackedDataArray = null;
                        byte[] PackedDataArray = null;

                        switch (pakmeth)
                        {
                            case 0: //uncompressed
                                item.CompressionType = ECompressionType.None;

                                if (item.UncompressedSize == (item.CompressedSize - 4))
                                {
                                    UnpackedDataArray = br.ReadBytes((int)item.UncompressedSize);
                                }
                                else
                                {
#warning //TODO: Bei nicht komprimierten Resourcen und ungleichen Werten wird eine Exception ausgelöst. Wie soll hier vorgegangen werden? Versuchen weiterzulesen? Wieviel Bytes überspringen?
                                    throw new ArgumentException("SCI1 Decompression (uncompressed)! Packed and unpacked length are not the same.");
                                }
                                break;
                            case 1: //Lzw
                                item.CompressionType = ECompressionType.Lzw;
                                break;
                            case 2: //Comp3
                                item.CompressionType = ECompressionType.Comp3;
                                break;
                            case 3: //Unknown0
                                item.CompressionType = ECompressionType.Unknown0;
                                break;
                            case 4: //Unknown1
                                item.CompressionType = ECompressionType.Unknown1;
                                break;
                            default:
                                UnpackedDataArray = new byte[item.UncompressedSize];
                                PackedDataArray = br.ReadBytes((int)item.CompressedSize);
                                break;
                        };

                        //SCI.IO.Compression.LZS lzs = new IO.Compression.LZS();
                        //lzs.Unpack(PackedDataArray, ref UnpackedDataArray);

                        switch (item.ResourceType)
                        {
                            case EResourceType.View:
                            case EResourceType.View8x:
                                //((SciView)item).LoadViewSCI1(new System.IO.MemoryStream(UnpackedDataArray));
                                break;
                            case EResourceType.Picture:
                            case EResourceType.Picture8x:
                                //((SciPicture)item).FromStream(stream);
                                break;
                            default:
                                break;
                        };
                    }
                }
            }

            return retval;
        }

        private void ReadMapFile(System.IO.Stream stream, string path)
        {
            byte restype = 0;
			IO.SciBinaryReader mapFileReader = new IO.SciBinaryReader(stream);
            /* ? muss noch geswappt werden ? */
            mapFileReader.ReverseReading = false;

            /*SCI 1*/
            Dictionary<byte, int> resourcearray = new Dictionary<byte, int>();
            List<int> offsetlist = new List<int>();
            while (0xFF != (restype = mapFileReader.ReadByte()))
            {
                UInt16 offset = mapFileReader.ReadUInt16();

                resourcearray.Add(restype, offset);
                offsetlist.Add(offset);
            }

            int i = 0;
            foreach (KeyValuePair<byte, int> item in resourcearray)
            {
                mapFileReader.BaseStream.Position = item.Value;

                long off2;

                if (i < offsetlist.Count - 1)
                {
                    off2 = offsetlist[(byte)(i + 1)];
                }
                else
                {
                    off2 = mapFileReader.BaseStream.Length;
                }

                while (mapFileReader.BaseStream.Position < off2)
                {
                    CResource resource = null;

                    switch ((EResourceType)item.Key)
                    {
                        case EResourceType.View8x:
                            resource = new View(EGameType.SCI1);
                            break;
                        case EResourceType.Palette8x:
                            resource = new Palette(EGameType.SCI1);
                            break;
                        case EResourceType.Picture8x:
                            resource = new Picture(EGameType.SCI1);
                            break;
                        default:
                            resource = new Dummy(EGameType.SCI1);
                            break;
                    }
					resource.Path = path;
                    //resource.ResourceType = (EResourceType)item.Key;
                    resource.ResourceNumber = mapFileReader.ReadUInt16();
                    UInt32 temp = mapFileReader.ReadUInt32();

                    resource.FileNumber = (byte)(temp >> 28);
                    resource.FileOffset = (Int32)(temp & 0xFFFFFFF);

                    ResourceList.Add(resource);
                }
                i++;
            }
        }
    }
}
