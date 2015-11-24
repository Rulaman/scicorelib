namespace SCI.Resources
{
    public class Palette : ResourceBase
    {
		public override EResourceType ResourceType
		{
			get { return EResourceType.Palette; }
		}

        public Palette(EGameType gametype):base(gametype) { }

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private System.Drawing.Color[] ColorField;

        public System.Drawing.Color[] ColorInfo
        {
            get { return ColorField; }
        }

        public System.Drawing.Color[] ReadFromSierraPalFile(string filename)
        {
			System.Drawing.Color[] colorInfoArray;
            short NumberOfColors;

			System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
			System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fileStream);

            binaryReader.BaseStream.Position = 29;

            NumberOfColors = binaryReader.ReadByte();

            binaryReader.BaseStream.Position = 37;

            colorInfoArray = new System.Drawing.Color[NumberOfColors];

            for (int counter = 0; counter < NumberOfColors; counter++)
            {
                if (binaryReader.BaseStream.Position + 4 <= binaryReader.BaseStream.Length)
                {
                    //colorInfoArray[counter].Used = (binaryReader.ReadByte() > 0) ? true : false;
                    binaryReader.ReadByte(); // Color used

					short red = binaryReader.ReadByte();
					short green = binaryReader.ReadByte();
					short blue = binaryReader.ReadByte();
                    colorInfoArray[counter] = System.Drawing.Color.FromArgb(255, red, green, blue);
                }
            }

            fileStream.Close();

            ColorField = new System.Drawing.Color[colorInfoArray.Length];
            System.Array.Copy(colorInfoArray, ColorField, colorInfoArray.Length);

            return colorInfoArray;
        }

        public void ReadFromStream(System.IO.BinaryReader binaryReader, bool inversive)
        {
            long positionInStream = 0;
			short NumberOfColors;
			short ColorOffset;
			short UsedUsed; // Is Color Used flag

            if (!inversive)
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

            ColorField = new System.Drawing.Color[256];

            for (int counter = ColorOffset; counter < NumberOfColors + ColorOffset; counter++)
            {
                if (binaryReader.BaseStream.Position + 4 <= binaryReader.BaseStream.Length)
                {
                    if (UsedUsed > 0)
                    {
                    }
                    else
                    {
                        binaryReader.ReadByte(); // color used
                    }
                    short red = binaryReader.ReadByte();
					short green = binaryReader.ReadByte();
					short blue = binaryReader.ReadByte();
                    ColorField[counter] = System.Drawing.Color.FromArgb(255, red, green, blue);
                }
            }

            if (!inversive)
            {
                binaryReader.BaseStream.Position = positionInStream;
            }
        }

        public void ReadFromStream(System.IO.Stream stream, bool inversive)
        {
            ReadFromStream(new System.IO.BinaryReader(stream), inversive);
        }

		public override void Decode()
		{
			throw new System.NotImplementedException();
		}
	}
}
