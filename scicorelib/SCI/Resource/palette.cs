using System;
using System.IO;
using System.Drawing;

namespace SCI.Resource
{
	public class SciPalette: ISciResource
	{
		private EGameType GameType;
		public SciPalette(EGameType gametype)
		{
			GameType = gametype;
		}

		private Color[] ColorField;

		public Color[] ColorInfo
		{
			get { return ColorField; }
		}

		public Color[] ReadFromSierraPalFile(string filename)
		{
			Color[] colorInfoArray;
			Int16 NumberOfColors;

			FileStream fileStream = new FileStream(filename, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);

			binaryReader.BaseStream.Position = 29;

			NumberOfColors = binaryReader.ReadByte();

			binaryReader.BaseStream.Position = 37;

			colorInfoArray = new Color[NumberOfColors];

			for ( int counter = 0; counter < NumberOfColors; counter++ )
			{
				if ( binaryReader.BaseStream.Position + 4 <= binaryReader.BaseStream.Length )
				{
					//colorInfoArray[counter].Used = (binaryReader.ReadByte() > 0) ? true : false;
					binaryReader.ReadByte(); // Color used

					Int16 red = binaryReader.ReadByte();
					Int16 green = binaryReader.ReadByte();
					Int16 blue = binaryReader.ReadByte();
					colorInfoArray[counter] = Color.FromArgb(255, red, green, blue);
				}
			}

			fileStream.Close();

			ColorField = new Color[colorInfoArray.Length];
			Array.Copy(colorInfoArray, ColorField, colorInfoArray.Length);

			return colorInfoArray;
		}
		public void ReadFromStream(BinaryReader binaryReader, bool inversive)
		{
			Int64 positionInStream = 0;
			Int16 NumberOfColors;
			Int16 ColorOffset;
			Int16 UsedUsed; // Is Color Used flag

			if ( !inversive )
			{
				positionInStream = binaryReader.BaseStream.Position;
			}

			binaryReader.BaseStream.Position += 25;

			ColorOffset = binaryReader.ReadByte();
			binaryReader.BaseStream.Position += 3;

			NumberOfColors = binaryReader.ReadByte();
			binaryReader.BaseStream.Position += 2;
			UsedUsed = binaryReader.ReadByte();
			binaryReader.BaseStream.Position += 4;

			ColorField = new Color[NumberOfColors];

			for ( int counter = ColorOffset; counter < NumberOfColors + ColorOffset; counter++ )
			{
				if ( binaryReader.BaseStream.Position + 4 <= binaryReader.BaseStream.Length )
				{
					if ( UsedUsed > 0 )
					{
					}
					else
					{
						binaryReader.ReadByte(); // color used
					}
					Int16 red = binaryReader.ReadByte();
					Int16 green = binaryReader.ReadByte();
					Int16 blue = binaryReader.ReadByte();
					ColorField[counter] = Color.FromArgb(255, red, green, blue);
				}
			}

			if ( !inversive )
			{
				binaryReader.BaseStream.Position = positionInStream;
			}
		}
		public void ReadFromStream(System.IO.Stream stream, bool inversive)
		{
			ReadFromStream(new BinaryReader(stream), inversive);
		}

		#region ISciResource Member
		public EResourceType ResourceType
		{
			get { return EResourceType.Palette; }
		}
		public UInt16 ResourceNumber
		{
			get;
			set;
		}
		public byte FileNumber
		{
			get;
			set;
		}
		public UInt32 FileOffset
		{
			get;
			set;
		}
		public ECompressionType CompressionType
		{
			get;
			set;
		}
		public uint CompressedSize
		{
			get;
			set;
		}
		public uint UncompressedSize
		{
			get;
			set;
		}
		#endregion
	}
}
