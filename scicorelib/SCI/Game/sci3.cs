using SCI.Resources;
using System;

namespace SCI
{
    public class SCI3 : SciBase
    {
		public override bool Load(string path)
		{
			bool retval = true;

			string[] sa = System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.TopDirectoryOnly);
			ResourceBase resource = null;

			foreach (string item in sa)
			{
				string name = System.IO.Path.GetFileNameWithoutExtension(item);
				int number = -1;

				int.TryParse(name, out number);

				if (number == -1)
				{
					/* keine gültige Resourcendatei */
					continue;
				}

				string ending = System.IO.Path.GetExtension(item);

				if  (ending=="")
				{
					continue;
				}

				EResourceType type = Common.GetResourceTypeByFileending(ending);

				switch (type)
				{
					case EResourceType.Audio:
						break;
					case EResourceType.Picture:
					case EResourceType.Picture8x:
						resource = new Picture(EGameType.SCI3);
						break;
					case EResourceType.View:
					case EResourceType.View8x:
						resource = new View(EGameType.SCI3);
						break;
					case EResourceType.Message:
					case EResourceType.Message8x:
						resource = new Message(EGameType.SCI3);
						break;
					default:
						resource = new Dummy(EGameType.None, type);
						break;
				}

				resource.Path = item;
				resource.ResourceNumber = number;
				resource.UncompressedSize = (uint)(new System.IO.FileInfo(item).Length);
				resource.CompressionType = ECompressionType.None;

				ResourceList.Add(resource);
			}

			return retval;
		}


        /// <summary>
        /// load a compiled game and not the sources and the project file give only the path as the parameter
        /// </summary>
        /// <param name="path">The path to the (compiled) game.</param>
        /// <returns>True if the game could loaded, otherwise false.</returns>
        public override bool Expand(string path)
        {
            bool retval = true;
            System.IO.FileStream stream;

            string file = System.IO.Path.Combine(path, "RESMAP.000");

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
                filearray = null;

                string resfilesave = "";

                foreach (ResourceBase item in ResourceList)
                {
                    string resfile = System.IO.Path.Combine(path, String.Format("RESSCI.{0}", item.FileNumber.ToString("000")));

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
                        item.CompressedSize = br.ReadUInt32();
                        item.UncompressedSize = br.ReadUInt32();
                        int pakmeth = br.ReadUInt16(); // immer STACpack bei SCI3

                        byte[] UnpackedDataArray = new byte[item.UncompressedSize];
                        byte[] PackedDataArray = br.ReadBytes((int)item.CompressedSize);

                        SCI.IO.Compression.LZS lzs = new IO.Compression.LZS();
                        lzs.Unpack(ref PackedDataArray, ref UnpackedDataArray);

                        item.CompressionType = ECompressionType.STACpack;

                        switch (item.ResourceType)
                        {
                            case EResourceType.Palette:
                            case EResourceType.Palette8x:
                          //      ((SciPalette)item).ReadFromStream(new System.IO.MemoryStream(UnpackedDataArray), true);
                                break;
                            case EResourceType.View:
                            case EResourceType.View8x:
                         //       ((SciView)item).LoadViewSCI11(new System.IO.MemoryStream(UnpackedDataArray));
                                break;
                            case EResourceType.Picture:
                            case EResourceType.Picture8x:
                        //        ((SciPictureRow)item).FromByteArray(UnpackedDataArray);
                                break;
                            case EResourceType.Message:
                            case EResourceType.Message8x:
                       //         ((SciMessage)item).DecodeMessage(new IO.SciBinaryReader(new System.IO.MemoryStream(UnpackedDataArray)));
                                    break;
                            default:
                                break;
                        };

						item.Data = new byte[item.UncompressedSize];
						Array.Copy(UnpackedDataArray, item.Data, item.UncompressedSize);
                    }
                }

                foreach (ResourceBase item in ResourceList)
                {
                    switch (item.ResourceType)
                    {
                        case EResourceType.View:
                        case EResourceType.View8x:
                            /* Resource entpacken */
                            //CResource resource = FindPaletteResource(item.ResourceNumber);
                            //if (resource != null)
                            //{ 
                            //    ((SciView)item).DecodeColors(((SciPalette)resource).ColorInfo);
                            //}
                            break;
                        case EResourceType.Picture:
                        case EResourceType.Picture8x:
                            break;
                        default:
                            break;
                    };
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

			/*SCI 3*/
			System.Collections.Generic.Dictionary<byte, int> resourcearray = new System.Collections.Generic.Dictionary<byte, int>();
			System.Collections.Generic.List<int> offsetlist = new System.Collections.Generic.List<int>();

            while (0xFF != (restype = mapFileReader.ReadByte()))
            {
                UInt16 offset = mapFileReader.ReadUInt16();

                resourcearray.Add(restype, offset);
                offsetlist.Add(offset);
            }

            int i = 0;
            foreach (System.Collections.Generic.KeyValuePair<byte, int> item in resourcearray)
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
                    ResourceBase resource = null;

                    switch ((EResourceType)item.Key)
                    {
                        case EResourceType.Palette:
                        case EResourceType.Palette8x:
                            resource = new Palette(EGameType.SCI3);
                            break;
                        case EResourceType.View:
                        case EResourceType.View8x:
                            resource = new View(EGameType.SCI3);
                            break;
                        case EResourceType.Picture:
                        case EResourceType.Picture8x:
                            resource = new PictureRow(EGameType.SCI3);
                            break;
                        case EResourceType.Message:
                        case EResourceType.Message8x:
                            resource = new Message(EGameType.SCI3);
                            break;
                        default:
                            resource = new Dummy(EGameType.SCI3, (EResourceType)item.Key);
                            break;
                    };
					resource.Path = path;
                    //resource.ResourceType = (EResourceType)item.Key;
                    resource.ResourceNumber = mapFileReader.ReadUInt16();
                    resource.FileOffset = (Int32)mapFileReader.ReadUInt32();

                    ResourceList.Add(resource);
                }
                i++;
            }
        }
    }
}
