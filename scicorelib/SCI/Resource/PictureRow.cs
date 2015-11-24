namespace SCI.Resources
{
    public class PictureRow : ResourceBase
    {
		public override EResourceType ResourceType
		{
			get { return EResourceType.Picture; }
		}

        public PictureRow(EGameType gametype) : base(gametype) { }

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private System.Drawing.Size InternalSize = new System.Drawing.Size();

        public object Tag;

        public short Len;
        public byte NumOfLoops;
        public byte IsScalable;
        public byte ScaleRes;
        public int PalOffset;
        public System.Drawing.Color[] Entries;

        public System.Collections.Generic.List<Picture> PictureList = new System.Collections.Generic.List<Picture>();

        public void FromFile(string filename)
        {
			System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open);
            FromStream(fs, fs.Length);
            fs.Close();
        }

        public void FromByteArray(byte[] array)
        {
            FromStream(new System.IO.MemoryStream(array), array.Length);
        }

        public void FromStream(System.IO.Stream stream, long length)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(stream);

            Len = br.ReadInt16();
            NumOfLoops = br.ReadByte();
            IsScalable = br.ReadByte();
            br.ReadByte();
            ScaleRes = br.ReadByte();
            PalOffset = br.ReadInt32();
            InternalSize.Width = br.ReadInt16();
            InternalSize.Height = br.ReadInt16();

            for (int entry = 0; entry < NumOfLoops; entry++)
            {
                Picture pict = new Picture(GameType);
                pict.FromStream(stream);
                PictureList.Add(pict);
            }

            br.BaseStream.Position = PalOffset;

            Entries = DecodeColorInformation(br);

            br.BaseStream.Position += 6; // Offset für Bildbeginn

            for (int entry = 0; entry < NumOfLoops; entry++)
            {
                PictureList[entry].ReadColorDataFromStream(stream);
            }

            foreach (Picture item in PictureList)
            {
                item.DecodeImage(Entries);
            }
        }

        //public void Save(string filename)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public void Save(System.IO.Stream stream)
        //{
        //    throw new System.NotImplementedException();
        //}

        public System.Drawing.Color[] DecodeColorInformation(string filename)
        {
			System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
			System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fileStream);

			System.Drawing.Color[] colors = DecodeColorInformation(binaryReader);

            binaryReader.Close();
            fileStream.Close();

            return colors;
        }

        public System.Drawing.Color[] DecodeColorInformation(System.IO.BinaryReader binaryReader)
        {
			System.Drawing.Color[] colorArray;
            short NumberOfColors = 256; // mal fest angenommen
            short ColorOffset = 0;
            short UsedUsed; // Is Color Used flag

            binaryReader.BaseStream.Position += 25;

            ColorOffset = binaryReader.ReadByte();
            binaryReader.BaseStream.Position += 3;

            NumberOfColors = binaryReader.ReadByte();
            binaryReader.BaseStream.Position += 2;
            UsedUsed = binaryReader.ReadByte();
            binaryReader.BaseStream.Position += 4;

            colorArray = new System.Drawing.Color[256];

            for (int counter = 0; counter < 256; counter++)
            {
                colorArray[counter] = System.Drawing.Color.Black;
            }

            for (int counter = ColorOffset; counter < NumberOfColors + ColorOffset; counter++)
            {
                if (binaryReader.BaseStream.Position + 4 <= binaryReader.BaseStream.Length)
                {
                    if (UsedUsed > 0)
                    {
                    }
                    else
                    {
                        //colorInfoArray[counter].Used = (binaryReader.ReadByte() > 0) ? true : false;
                        binaryReader.ReadByte();
                    }
                    short red = binaryReader.ReadByte();
                    short green = binaryReader.ReadByte();
                    short blue = binaryReader.ReadByte();
                    colorArray[counter] = System.Drawing.Color.FromArgb(255, red, green, blue);
                }
            }

            return colorArray;
        }

        public bool Load(string path)
        {
            throw new System.NotImplementedException();
        }

        public int Height
        {
            get { return InternalSize.Height; }
        }

        public int Width
        {
            get { return InternalSize.Width; }
        }

        public System.Drawing.Size Size
        {
            get { return InternalSize; }
        }

		public override void Decode()
		{
			if ( Path != null )
			{
				FromFile(Path);
			}
			else  if ( Data != null )
			{
				FromByteArray(Data);
			}
		}
	}
}
