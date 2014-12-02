using System;
using System.Collections.Generic;
using System.Text;

using SCI.Interface;
using SCI.Drawing;

namespace SCI
{
	public abstract class SciBase
	{
		protected List<CResource> _ResourceList = new List<CResource>();

		public List<CResource> ResourceList
		{
			get { return _ResourceList; }
		}


		private string globalpath = "";
		private string selectedpalette	= "";

		public List<CResource> PaletteResourceList;

		public CResource FindPaletteName(int resourceid)
		{
			CResource returnvalue = null;
			int id = resourceid;

			if ( PaletteResourceList == null )
			{
				PaletteResourceList = new List<CResource>();

				foreach ( CResource item in _ResourceList )
				{
					switch ( item.Type )
					{
					case EResourceType.Palette:
					case EResourceType.Palette8x:
						PaletteResourceList.Add(item);
						break;
					};
				}
			}

			while ( (returnvalue == null) && (id >= 0) )
			{
				foreach ( CResource item in PaletteResourceList )
				{
					if ( item.Number == resourceid )
					{
						returnvalue = item;
						break;
					}
				}

				id--;
			}

			if ( returnvalue == null )
			{
				foreach ( CResource item in PaletteResourceList )
				{
					if ( item.Number == 999 )
					{
						returnvalue = item;
						break;
					}
				}
			}

			return returnvalue;
		}
	}


	public class SCI3: SciBase, ISciType
	{
		

		/// <summary>
		/// load a compiled game and not the sources and the project file
		/// give only the path as the parameter
		/// </summary>
		public bool Load(string path)
		{
			bool retval = true;
			System.IO.FileStream stream;

			string file =System.IO.Path.Combine(path, "RESMAP.000");
			
			if (  !System.IO.File.Exists(file) )
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

				_ResourceList = ReadMapFile(ms);

				string resfilesave = "";

				foreach ( CResource item in _ResourceList )
				{
					string resfile = System.IO.Path.Combine(path, String.Format("RESSCI.{0}", item.FileNumber.ToString("000")));
					
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
						
						stream.Position = item.Offset;

						/* Resource entpacken */
						SciBinaryReader br = new SciBinaryReader(stream);
						
						byte typ = br.ReadByte();
						UInt16 id = br.ReadUInt16();
						UInt32 paklen = br.ReadUInt32();
						UInt32 unplen = br.ReadUInt32();
						int pakmeth = br.ReadUInt16();

						byte[] UnpackedDataArray = new byte[unplen];
						byte[] PackedDataArray = br.ReadBytes((int)paklen);

						switch ( pakmeth )
						{
						case 0: //uncompressed
							break;
						case 1: //Lzw
							break;
						case 2: //Comp3
							break;
						case 3: //Unknown0
							break;
						case 4: //Unknown1
							break;
						};

						SCI.IO.Compression.LZS lzs = new IO.Compression.LZS();
						lzs.Unpack(PackedDataArray, ref UnpackedDataArray);

						switch ( item.Type )
						{
						case EResourceType.Palette:
						case EResourceType.Palette8x:
							SCI.Drawing.SciPalette palette = new SciPalette();
							palette.CompressionType = ECompressionType.STACpack;
							palette.CompressedSize = unplen;
							palette.UncompressedSize = paklen;
							palette.ReadFromStream(new System.IO.MemoryStream(UnpackedDataArray), true);

							item.ResourceData = palette;
							break;
						case EResourceType.View:
						case EResourceType.View8x:
							/* Resource entpacken */
							SCI.Drawing.DecodeV56 view = new SCI.Drawing.DecodeV56();
							view.CompressionType = ECompressionType.STACpack;
							view.CompressedSize = unplen;
							view.UncompressedSize = paklen;
							view.LoadViewSCI11(new System.IO.MemoryStream(UnpackedDataArray));
							//FindPaletteName(id);
							item.ResourceData = view;
							break;
						case EResourceType.Picture:
						case EResourceType.Picture8x:
							SCI.Drawing.Picture pict = new SCI.Drawing.Picture();
							pict.CompressionType = ECompressionType.STACpack;
							pict.CompressedSize = unplen;
							pict.UncompressedSize = paklen;
							pict.ReadHeaderFromStream(stream);
							item.ResourceData = pict;
							break;
						default:
							break;
						};
					}
				}

				foreach ( CResource item in _ResourceList )
				{
					switch ( item.Type )
					{
					case EResourceType.View:
					case EResourceType.View8x:
						/* Resource entpacken */
						DecodeV56 view = (DecodeV56)item.ResourceData;
						CResource resource = FindPaletteName(item.Number);
						SciPalette palette = (SciPalette)resource.ResourceData;
						view.DecodeColors(palette.ColorInfo);
						break;
					case EResourceType.Picture:
					case EResourceType.Picture8x:
						Picture pict = (Picture)item.ResourceData;
						CResource resource2 = FindPaletteName(item.Number);
						SciPalette palette2 = (SciPalette)resource2.ResourceData;
						//pict.DecodeImage(palette2.ColorInfo);
						break;
					default:
						break;
					};
				}
			}

			return retval;
		}


		private List<CResource> ReadMapFile(System.IO.Stream stream)
		{
			byte restype = 0;
			List<CResource> resourceindex = new List<CResource>();
			SCI.SciBinaryReader mapFileReader = new SciBinaryReader(stream);
			/* ? muss noch geswappt werden ? */
			mapFileReader.ReverseReading = false;

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
					CResource resource = new CResource();

					resource.ResourceType = item.Key;
					resource.Type = (EResourceType)item.Key;

					resource.Number = mapFileReader.ReadUInt16();
					resource.Offset = mapFileReader.ReadUInt32();

					resourceindex.Add(resource);
				}
				i++;
			}

			return resourceindex;
		}
	}
}
