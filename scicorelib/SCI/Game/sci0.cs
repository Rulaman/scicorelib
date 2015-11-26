namespace SCI
{
	public class SCI0: SciBase
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
			bool retval = false;
			string file = System.IO.Path.Combine(path, "RESOURCE.MAP");

			if ( !System.IO.File.Exists(file) )
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

				foreach ( Resources.ResourceBase item in ResourceList )
				{
					string resfile = System.IO.Path.Combine(path, System.String.Format("RESOURCE.{0}", item.FileNumber.ToString("000")));

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
							br = new IO.SciBinaryReader(stream);
						}

						stream.Position = item.FileOffset;

						ushort resourceInfo = br.ReadUInt16();
						int resourceType = (resourceInfo >> 11);                    // XXXX X... .... ....
						ushort resourceNumber = (ushort)(resourceInfo & 0x7FF);     // .... .XXX XXXX XXXX
						ushort packedLength = (ushort)(br.ReadUInt16() - 4);
						ushort unpackedLength = br.ReadUInt16();
						ushort compressionType = br.ReadUInt16();

						byte[] PackedDataArray = br.ReadBytes(packedLength);
						item.ResourceNumber = resourceNumber;
						item.CompressedSize = packedLength;
						item.UncompressedSize = unpackedLength;

						byte[] UnpackedDataArray = new byte[item.UncompressedSize];

						switch ( compressionType )
						{
						case 0:
							item.CompressionType = ECompressionType.None;
							break;
						case 1:
							item.CompressionType = ECompressionType.Lzw;
							IO.Compression.LZW lzw = new IO.Compression.LZW(IO.Compression.LZW.EOption.Lzw);
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

						item.Data = new byte[unpackedLength];
						System.Array.Copy(UnpackedDataArray, item.Data, item.UncompressedSize);
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
			ushort restypenum = 0;

			while ( 0xFFFF != restypenum )
			{
				restypenum = mapFileReader.ReadUInt16();
				uint resfileoff = mapFileReader.ReadUInt32();

				Resources.ResourceBase resource = GetResourceByType((EResourceType)((restypenum >> 11) & 0x7F), EGameType.SCI0);

				resource.Path = path;
				resource.ResourceNumber = (ushort)(restypenum & 0x7FF); // .... .XXX XXXX XXXX
				resource.FileNumber = (byte)(resfileoff >> 26);         // XXXX XX.. .... .... .... .... .... ....
				resource.FileOffset = (int)(resfileoff & 0x3FFFFFF);    // .... ..XX XXXX XXXX XXXX XXXX XXXX XXXX

				ResourceList.Add(resource);
			}
		}
	}
}
