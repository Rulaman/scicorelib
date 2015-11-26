namespace SCI
{
	using Resources;
	using IO.Compression;

	public class SCI1: SciBase
	{
		public override bool Load(string path)
		{
			return base.Load(path);
		}

		/// <summary>
		/// load a compiled game and not the sources and the project file give only the path as the parameter
		/// </summary>
		/// <param name="path">The path to the (compiled) game.</param>
		/// <returns>True if the game could loaded, otherwise false.</returns>
		public override bool Expand(string path)
		{
			string mapfilename = "RESOURCE.MAP";
			string resourcefilename = "RESOURCE";

			bool retval = true;
			System.IO.FileStream stream;

			string file = System.IO.Path.Combine(path, mapfilename);

			if ( !System.IO.File.Exists(file) )
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

				foreach ( Resources.ResourceBase item in ResourceList )
				{
					string resfile = System.IO.Path.Combine(path, resourcefilename + System.String.Format(".{0}", item.FileNumber.ToString("000")));

					if ( !System.IO.File.Exists(file) )
					{
						continue;
					}
					else
					{
						if ( resfilesave != resfile )
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

						switch ( pakmeth )
						{
						case 0: //uncompressed
							item.CompressionType = ECompressionType.None;

							if ( item.UncompressedSize == (item.CompressedSize - 4) )
							{
								UnpackedDataArray = br.ReadBytes((int)item.UncompressedSize);
							}
							else
							{
#warning //TODO: Bei nicht komprimierten Resourcen und ungleichen Werten wird eine Exception ausgelöst. Wie soll hier vorgegangen werden? Versuchen weiterzulesen? Wieviel Bytes überspringen?
								throw new System.ArgumentException("SCI1 Decompression (uncompressed)! Packed and unpacked length are not the same.");
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

						switch ( item.ResourceType )
						{
						case EResourceType.View:
							//((SciView)item).LoadViewSCI1(new System.IO.MemoryStream(UnpackedDataArray));
							break;
						case EResourceType.Picture:
							//((SciPicture)item).FromStream(stream);
							break;
						default:
							break;
						};

						item.Data = new byte[item.UncompressedSize];
						System.Array.Copy(UnpackedDataArray, item.Data, item.UncompressedSize);
					}
				}
			}

			stream.Close();

			return retval;
		}

		private void ReadMapFile(System.IO.Stream stream, string path)
		{
			byte restype = 0;
			IO.SciBinaryReader mapFileReader = new IO.SciBinaryReader(stream);
			/* ? muss noch geswappt werden ? */
			mapFileReader.ReverseReading = false;

			System.Collections.Generic.Dictionary<byte, int> resourcearray = new System.Collections.Generic.Dictionary<byte, int>();
			System.Collections.Generic.List<int> offsetlist = new System.Collections.Generic.List<int>();
			while ( 0xFF != (restype = mapFileReader.ReadByte()) )
			{
				ushort offset = mapFileReader.ReadUInt16();

				resourcearray.Add(restype, offset);
				offsetlist.Add(offset);
			}

			int i = 0;
			foreach ( System.Collections.Generic.KeyValuePair<byte, int> item in resourcearray )
			{
				mapFileReader.BaseStream.Position = item.Value;

				long off2;

				if ( i < offsetlist.Count - 1 )
				{
					off2 = offsetlist[(byte)(i + 1)];
				}
				else
				{
					off2 = mapFileReader.BaseStream.Length;
				}

				while ( mapFileReader.BaseStream.Position < off2 )
				{
					Resources.ResourceBase resource = GetResourceByType((EResourceType)(item.Key & 0x7F), EGameType.SCI1);

					resource.Path = path;
					resource.ResourceNumber = mapFileReader.ReadUInt16();
					uint temp = mapFileReader.ReadUInt32();

					resource.FileNumber = (byte)(temp >> 28);
					resource.FileOffset = (int)(temp & 0xFFFFFFF);

					ResourceList.Add(resource);
				}
				i++;
			}
		}
	}
}
