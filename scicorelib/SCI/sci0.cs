using SCI.Resource;
using System;

namespace SCI
{
    public class SCI0 : CSciBase
    {
        /// <summary>
        /// load a compiled game and not the sources and the project file give only the path as the parameter
        /// </summary>
        /// <param name="path">The path to the (compiled) game.</param>
        /// <returns>True if the game could loaded, otherwise false.</returns>
        public override bool Expand(string path)
        {
            bool retval = false;
            string file = System.IO.Path.Combine(path, "RESOURCE.MAP");

            if (!System.IO.File.Exists(file))
            {
            }
            else
            {
                /* Parse the map file */
                System.IO.FileStream stream = System.IO.File.Open(file, System.IO.FileMode.Open);
                ReadMapFile(stream, path);
                stream.Close();

                /* try to load game now */
                string resfilesave = "";
                IO.SciBinaryReader br = null;

                foreach (CResource item in ResourceList)
                {
                    string resfile = System.IO.Path.Combine(path, String.Format("RESOURCE.{0}", item.FileNumber.ToString("000")));

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
                            br = new IO.SciBinaryReader(stream);
                        }

                        stream.Position = item.FileOffset;

                        UInt16 resourceInfo = br.ReadUInt16();
                        Int32 resourceType = (resourceInfo >> 11);              // XXXX X... .... ....
                        UInt16 resourceNumber = (UInt16)(resourceInfo & 0x7FF);     // .... .XXX XXXX XXXX
                        UInt16 packedLength = (UInt16)(br.ReadUInt16() - 4);
                        UInt16 unpackedLength = br.ReadUInt16();
                        UInt16 compressionType = br.ReadUInt16();

                        byte[] PackedDataArray = br.ReadBytes(packedLength);

                        //                CResource resource = null;

                        //                switch ((EResourceType)resourceType)
                        //                {
                        //                    case EResourceType.Palette:
                        //                    case EResourceType.Palette8x:
                        //                        resource = new SciPalette(EGameType.SCI0);
                        //                        // palette.ReadFromStream(new System.IO.MemoryStream(UnpackedDataArray), true);
                        //                        // item.ResourceData = palette;
                        //                        break;
                        //                    case EResourceType.View:
                        //                    case EResourceType.View8x:
                        //resource = new SciView(EGameType.SCI0);
                        //                        // view.LoadViewSCI11(new System.IO.MemoryStream(UnpackedDataArray));
                        //                        // item.ResourceData = view;
                        //                        break;
                        //                    case EResourceType.Picture:
                        //                    case EResourceType.Picture8x:
                        //resource = new SciPictureRow(EGameType.SCI0);
                        //                        // pict.FromByteArray(UnpackedDataArray);
                        //                        // item.ResourceData = pict;
                        //                        break;
                        //                    default:
                        //                        resource = new Dummy();
                        //                        break;
                        //                };

                        item.ResourceType = (EResourceType)resourceType;
                        item.ResourceNumber = resourceNumber;
                        item.CompressedSize = unpackedLength;
                        item.UncompressedSize = packedLength;

                        switch (compressionType)
                        {
                            case 0:
                                item.CompressionType = ECompressionType.None;
                                break;
                            case 1:
                                item.CompressionType = ECompressionType.Lzw;
                                SCI.IO.Compression.LZW lzw = new IO.Compression.LZW(IO.Compression.LZW.EOption.Lzw);
                                byte[] UnpackedDataArray = new byte[unpackedLength];
                                lzw.Unpack(ref PackedDataArray, ref UnpackedDataArray);
                                break;
                            case 2:
                                item.CompressionType = ECompressionType.Comp3;
                                break;
                            case 3:
                                item.CompressionType = ECompressionType.Huffman;
                                break;
                            default:
                                item.CompressionType = ECompressionType.Invalid;
                                break;
                        };
                    }
                }

                stream.Close();

                retval = true;
            }

            return retval;
        }

        private void ReadMapFile(System.IO.Stream stream, string path)
        {
			// RESOURCE.MAP
			IO.SciBinaryReader mapFileReader = new IO.SciBinaryReader(stream);
            /* ? muss noch geswappt werden ? */
            //mapFileReader.ReverseReading = false;
            UInt16 restypenum = 0;
            CResource resource = null;

            while (0xFFFF != restypenum)
            {
                restypenum = mapFileReader.ReadUInt16();
                UInt32 resfileoff = mapFileReader.ReadUInt32();

                switch ((EResourceType)(restypenum >> 11))
                {
                    case EResourceType.Palette:
                    case EResourceType.Palette8x:
                        resource = new SciPalette(EGameType.SCI0);
                        // palette.ReadFromStream(new System.IO.MemoryStream(UnpackedDataArray), true);
                        // item.ResourceData = palette;
                        break;
                    case EResourceType.View:
                    case EResourceType.View8x:
                        resource = new SciView(EGameType.SCI0);
                        // view.LoadViewSCI11(new System.IO.MemoryStream(UnpackedDataArray));
                        // item.ResourceData = view;
                        break;
                    case EResourceType.Picture:
                    case EResourceType.Picture8x:
                        resource = new SciPictureRow(EGameType.SCI0);
                        // pict.FromByteArray(UnpackedDataArray);
                        // item.ResourceData = pict;
                        break;
                    default:
                        resource = new Dummy();
                        break;
                };

				resource.Path = path;

                resource.ResourceType = (EResourceType)(restypenum >> 11);        // XXXX X... .... ....
                resource.ResourceNumber = (UInt16)(restypenum & 0x7FF);  // .... .XXX XXXX XXXX
                resource.FileNumber = (byte)(resfileoff >> 26);      // XXXX XX.. .... .... .... .... .... ....
                resource.FileOffset = (int)(resfileoff & 0x3FFFFFF);      // .... ..XX XXXX XXXX XXXX XXXX XXXX XXXX

                ResourceList.Add(resource);
            }
        }
    }
}
