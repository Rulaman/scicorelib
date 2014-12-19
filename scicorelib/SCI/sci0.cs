using System;
using System.Collections.Generic;
using System.Text;

using SCI.Resource;

namespace SCI
{
	public class SCI0: SciBase
	{


		private readonly List<ResourceInfo> 	ResourceArray 	= new List<ResourceInfo>();
		private readonly List<UInt32> 					OffsetList 		= new List<UInt32>();
		
		/// <summary>
		/// load a compiled game and not the sources and the project file give only the path as the parameter
		/// </summary>
		/// <param name="path">The path to the (compiled) game.</param>
		/// <returns>True if the game could loaded, otherwise false.</returns>
		public override bool Load(string path)
		{
			bool retval = false;
			string file = System.IO.Path.Combine(path, "RESOURCE.MAP");
			
			if (  !System.IO.File.Exists(file) )
			{
			}
			else
			{
				/* Parse the map file */
				System.IO.FileStream stream = System.IO.File.Open(file, System.IO.FileMode.Open);
				ReadMapFile(stream);
				stream.Close();
				/* try to load game now */
				string resfilesave = "";
				SciBinaryReader br = null;

				foreach ( ResourceInfo item in ResourceArray )
				{
					string resfile = System.IO.Path.Combine(path, String.Format("RESOURCE.{0}", item.FileNumber.ToString("000")));
					
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
							br = new SciBinaryReader(stream);
						}
						
						stream.Position = item.FileOffset;
						CResource resource = new CResource();
						
						UInt16 restypenum    = br.ReadUInt16();
						
						resource.Type	= (EResourceType)(restypenum >> 11);			// XXXX X... .... ....
						resource.Number 	= (UInt16)(restypenum & 0x7FF);			// .... .XXX XXXX XXXX
						
						UInt16 paklen = (UInt16)(br.ReadUInt16() - 4);
						UInt16 unplen = br.ReadUInt16();
						UInt16 comptype = br.ReadUInt16();
						
						byte[] UnpackedDataArray = new byte[unplen];
						//br.ReadUInt16();
						byte[] PackedDataArray = br.ReadBytes(paklen);

						switch ( comptype )
						{
						case 0:
							resource.CompressionType = ECompressionType.None;
							break;
						case 1:
							resource.CompressionType = ECompressionType.Lzw;
							SCI.IO.Compression.LZW lzw = new IO.Compression.LZW(IO.Compression.LZW.ELZWOption.Lzw);
							lzw.Unpack(ref PackedDataArray, ref UnpackedDataArray);
							break;
						case 2:
							resource.CompressionType = ECompressionType.Comp3;
							break;
						case 3:
							resource.CompressionType = ECompressionType.Huffman;
							break;
						default:
							resource.CompressionType = ECompressionType.Invalid;
							break;
						};


						
						switch ( resource.Type )
						{
						case EResourceType.Palette:
						case EResourceType.Palette8x:
//							SCI.Resource.SciPalette palette = new SciPalette();
//							palette.CompressionType = ECompressionType.STACpack;
//							palette.CompressedSize = paklen;
//							palette.UncompressedSize = unplen;
//							palette.ReadFromStream(new System.IO.MemoryStream(UnpackedDataArray), true);
//							item.ResourceData = palette;
							break;
						case EResourceType.View:
						case EResourceType.View8x:
//							SCI.Resource.SciView view = new SCI.Resource.SciView();
//							view.CompressionType = ECompressionType.STACpack;
//							view.CompressedSize = paklen;
//							view.UncompressedSize = unplen;
//							view.LoadViewSCI11(new System.IO.MemoryStream(UnpackedDataArray));
//							item.ResourceData = view;
							break;
						case EResourceType.Picture:
						case EResourceType.Picture8x:
//							SCI.Resource.SciPictureRow pict = new SCI.Resource.SciPictureRow();
//							pict.CompressionType = ECompressionType.STACpack;
//							pict.CompressedSize = paklen;
//							pict.UncompressedSize = unplen;
//							pict.FromByteArray(UnpackedDataArray);
//							item.ResourceData = pict;
							break;
						default:
							break;
						};
						
						ResourceList.Add(resource);
					}
				}
				
				stream.Close();
				
				retval = true;
			}
			
			return retval;
		}
		
		private void ReadMapFile(System.IO.Stream stream)
		{
			// List<CResource> _ResourceList from BaseClass
			// RESOURCE.MAP
			SCI.SciBinaryReader mapFileReader = new SciBinaryReader(stream);
			/* ? muss noch geswappt werden ? */
			//mapFileReader.ReverseReading = false;
			UInt16 restypenum = 0;

			while ( 0xFFFF != restypenum )
			{
				ResourceInfo resinfo = new ResourceInfo();
				
				restypenum = mapFileReader.ReadUInt16();
				UInt32 resfileoff = mapFileReader.ReadUInt32();
				
				resinfo.Type 		= (byte)(restypenum >> 11);		// XXXX X... .... ....
				resinfo.Number 		= (UInt16)(restypenum & 0x7FF);	// .... .XXX XXXX XXXX
				resinfo.FileNumber	= (byte)(resfileoff >> 26);		// XXXX XX.. .... .... .... .... .... ....
				resinfo.FileOffset	= (resfileoff & 0x3FFFFFF);		// .... ..XX XXXX XXXX XXXX XXXX XXXX XXXX

				ResourceArray.Add(resinfo);
				OffsetList.Add(resinfo.FileOffset);
			}
		}
	}
}
