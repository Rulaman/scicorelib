using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace SCI.Resource
{
	using SCI.Interface;

	public class PictureRow: ISciResource
	{
		private ECompressionType CompType;
		private uint CompSize;
		private uint UncompSize;

		private Size		InternalSize = new Size();
		private object		Object;

		public Int16		Len;
		public byte			NumOfLoops;
		public byte			IsScalable;
		public byte			ScaleRes;
		public Int32		PalOffset;
		public Color[]		Entries;

		public List<Picture> PictureList = new List<Picture>();

		public void FromFile(string filename)
		{
			FileStream fs = new FileStream(filename, System.IO.FileMode.Open);
			FromStream(fs);
			fs.Close();
		}
		public void FromStream(System.IO.Stream stream)
		{
			System.IO.BinaryReader br = new BinaryReader(stream);

			Len = br.ReadInt16();
			NumOfLoops = br.ReadByte();
			IsScalable = br.ReadByte();
			br.ReadByte();
			ScaleRes = br.ReadByte();
			PalOffset = br.ReadInt32();
			InternalSize.Width = br.ReadInt16();
			InternalSize.Height = br.ReadInt16();

			for ( int entry = 0; entry < NumOfLoops; entry++ )
			{
				Picture pict = new Picture();
				pict.ReadHeaderFromStream(stream);
				PictureList.Add(pict);
			}

			br.BaseStream.Position = PalOffset;

			Entries = DecodeColorInformation(br);

			br.BaseStream.Position += 6; // Offset für Bildbeginn

			for ( int entry = 0; entry < NumOfLoops; entry++ )
			{
				PictureList[entry].ReadColorDataFromStream(stream);
			}

			foreach(Picture item in PictureList)
			{
				item.DecodeImage(Entries);
			}

		}

		public void Save(string filename)
		{
			throw new NotImplementedException();
		}
		public void Save(System.IO.Stream stream)
		{
			throw new NotImplementedException();
		}

		public Color[] DecodeColorInformation(string filename)
		{
			FileStream fileStream = new FileStream(filename, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);

			Color[] colors = DecodeColorInformation(binaryReader);

			binaryReader.Close();
			fileStream.Close();

			return colors;
		}
		public Color[] DecodeColorInformation(BinaryReader binaryReader)
		{
			Color[] colorArray;
			Int16 NumberOfColors = 256; // mal fest angenommen
			Int16 ColorOffset = 0;
			Int16 UsedUsed; // Is Color Used flag

			binaryReader.BaseStream.Position += 25;

			ColorOffset = binaryReader.ReadByte();
			binaryReader.BaseStream.Position += 3;

			NumberOfColors = binaryReader.ReadByte();
			binaryReader.BaseStream.Position += 2;
			UsedUsed = binaryReader.ReadByte();
			binaryReader.BaseStream.Position += 4;

			colorArray = new Color[256];
			
			for ( int counter = 0; counter < 256; counter++ )
			{
				colorArray[counter] = Color.Black;
			}

			for ( int counter = ColorOffset; counter < NumberOfColors + ColorOffset; counter++ )
			{
				if ( binaryReader.BaseStream.Position + 4 <= binaryReader.BaseStream.Length )
				{
					if ( UsedUsed > 0 )
					{
					}
					else
					{
						//colorInfoArray[counter].Used = (binaryReader.ReadByte() > 0) ? true : false;
						binaryReader.ReadByte();
					} 
					Int16 red = binaryReader.ReadByte();
					Int16 green = binaryReader.ReadByte();
					Int16 blue = binaryReader.ReadByte();
					colorArray[counter] = Color.FromArgb(255, red, green, blue);
				}
			}
			
			return colorArray;
		}

		public Int32 Height
		{
			get { return InternalSize.Height; }
		}
		public Int32 Width
		{
			get { return InternalSize.Width; }
		}
		public object Tag
		{
			get { return Object; }
			set { Object = value; }
		}
		public Size Size
		{
			get { return InternalSize; }
		}

		#region ISciResource Member
		public EResourceType Type
		{
			get { return EResourceType.Picture; }
		}
		public ECompressionType CompressionType
		{
			get { return CompType; }
			set { CompType = value; }
		}
		public uint CompressedSize
		{
			get { return CompSize; }
			set { CompSize = value; }
		}
		public uint UncompressedSize
		{
			get { return UncompSize; }
			set { UncompSize = value; }
		}
		#endregion
	}
}
