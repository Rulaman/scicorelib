using System;
using System.Collections.Generic;
using System.Text;

using SCI.Resource;

namespace SCI
{
	public class SCI1: SciBase
	{
		//private List<CResource> _ResourceList = new List<CResource>();

//		public List<CResource> ResourceList
//		{
//			get { return _ResourceList; }
//		}

		/// <summary>
		/// load a compiled game and not the sources and the project file
		/// give only the path as the parameter
		/// </summary>
		public override bool Load(string path)
		{
			string mapfilename = "RESOURCE.MAP";
			string resourcefilename = "RESOURCE";

			bool retval = true;
			System.IO.FileStream stream;

			string file =System.IO.Path.Combine(path, mapfilename);
			
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
					string resfile = System.IO.Path.Combine(path, resourcefilename + String.Format(".{0}", item.FileNumber.ToString("000")));
					
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
						/* byte typ = */ br.ReadByte();
						/* UInt16 id = */ br.ReadUInt16();

						uint paklen = br.ReadUInt16();
						uint unplen = br.ReadUInt16();

						int pakmeth = br.ReadUInt16();

						byte[] UnpackedDataArray;
						byte[] PackedDataArray;


						switch ( pakmeth )
						{
						case 0: //uncompressed
							if ( unplen == paklen )
							{
								UnpackedDataArray = br.ReadBytes((int)unplen);
							}
							else
							{
#warning //TODO: Bei nicht komprimierten Resourcen und ungleichen Werten wird eine Exception ausgelöst. Wie swoll hier vorgegangen werden? Versuchen weiterzulesen? Wieviel Bytes überspringen?
								throw new ArgumentException("SCI1 Decompression (uncompressed)! Packed and unpacked length are not the same.");
							}
							break;
						default:
							UnpackedDataArray = new byte[unplen];
							PackedDataArray = br.ReadBytes((int)paklen);
							break;
						};

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

						//SCI.IO.Compression.LZS lzs = new IO.Compression.LZS();
						//lzs.Unpack(PackedDataArray, ref UnpackedDataArray);

						switch ( item.Type )
						{
						case EResourceType.View:
						case EResourceType.View8x:

							/* Resource entpacken */
							SciView view = new SciView(EGameType.SCI1);
							view.CompressionType = ECompressionType.STACpack;
							view.CompressedSize = paklen;
							view.UncompressedSize = unplen;
							view.LoadViewSCI1(new System.IO.MemoryStream(UnpackedDataArray));
							item.ResourceData = view;
							break;
						case EResourceType.Picture:
						case EResourceType.Picture8x:
							SciPicture pict = new SciPicture(EGameType.SCI1);
							pict.FromStream(stream);
							item.ResourceData = pict;
							break;
						default:
							break;
						};
					}
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
					UInt32 temp = mapFileReader.ReadUInt32();

					resource.FileNumber = (byte)(temp >> 28);
					resource.Offset = (UInt32)(temp & 0xFFFFFFF);

					resourceindex.Add(resource);
				}
				i++;
			}

			return resourceindex;
		}
	}
}
