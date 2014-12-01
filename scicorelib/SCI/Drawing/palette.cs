using System;
using System.IO;
using System.Drawing;

namespace SCI.Drawing
{
	public class DecodePalette
	{
		public ColorFieldColorInfo[] ReadFromSierraPalFile(string filename)
		{
			ColorFieldColorInfo[] colorInfoArray;
			Int16 NumberOfColors = 256; // mal fest angenommen

			FileStream fileStream = new FileStream(filename, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);

			binaryReader.BaseStream.Position = 29;

			NumberOfColors = binaryReader.ReadByte();

			binaryReader.BaseStream.Position = 37;

			colorInfoArray = new ColorFieldColorInfo[NumberOfColors];

			for ( int counter = 0; counter < NumberOfColors; counter++ )
			{
				if ( binaryReader.BaseStream.Position + 4 <= binaryReader.BaseStream.Length )
				{
					colorInfoArray[counter].Used = (binaryReader.ReadByte() > 0) ? true : false;
					Int16 red = binaryReader.ReadByte();
					Int16 green = binaryReader.ReadByte();
					Int16 blue = binaryReader.ReadByte();
					colorInfoArray[counter].Color = Color.FromArgb(255, red, green, blue);
				}
			}

			fileStream.Close();

			return colorInfoArray;
		}
		public ColorFieldColorInfo[] ReadFromStream(BinaryReader binaryReader, bool inversive)
		{
			Int64 positionInStream = 0;
			ColorFieldColorInfo[] colorInfoArray;
			Int16 NumberOfColors = 256; // mal fest angenommen
			Int16 ColorOffset = 0;
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

			colorInfoArray = new ColorFieldColorInfo[256];
			for ( int counter = 0; counter < 256; counter++ )
			{
				colorInfoArray[counter].Used = false;
				colorInfoArray[counter].Color = Color.Black;
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
						colorInfoArray[counter].Used = (binaryReader.ReadByte() > 0) ? true : false;
					}
					Int16 red = binaryReader.ReadByte();
					Int16 green = binaryReader.ReadByte();
					Int16 blue = binaryReader.ReadByte();
					colorInfoArray[counter].Color = Color.FromArgb(255, red, green, blue);
				}
			}

			if ( !inversive )
			{
				binaryReader.BaseStream.Position = positionInStream;
			}

			return colorInfoArray;
		}
	}
}
