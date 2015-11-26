namespace SCI.Resources
{
	public sealed class Picture: ResourceBase
	{
		public override EResourceType ResourceType
		{
			get { return EResourceType.Picture; }
		}

		public Picture(EGameType gametype) : base(gametype)
		{ }

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private System.Drawing.Size InternalSize = new System.Drawing.Size();

		private System.Drawing.Point ScreenPosition = new System.Drawing.Point();
		private byte TransparentKey;
		private byte Compression;
		private ushort Flags;
		public System.Drawing.Bitmap Image;
		public int OffsetRLE;
		public byte[] ColorData;
		public object Tag;

		public System.Drawing.Image FromFile(string filename)
		{
			return Image;
		}

		public void FromStream(System.IO.Stream stream)
		{
			System.IO.BinaryReader br = new System.IO.BinaryReader(stream);

			InternalSize.Width = br.ReadInt16();
			InternalSize.Height = br.ReadInt16();
			ScreenPosition.X = br.ReadInt16();
			ScreenPosition.Y = br.ReadInt16();
			TransparentKey = br.ReadByte();
			Compression = br.ReadByte();
			Flags = br.ReadUInt16();
			br.ReadBytes(12);
			OffsetRLE = br.ReadInt32();

			br.ReadBytes(14); // Dummy-Bytes ??
		}

		public void ReadColorDataFromStream(System.IO.Stream stream)
		{
			System.IO.BinaryReader br = new System.IO.BinaryReader(stream);

			ColorData = new byte[Width * Height];
			ColorData = br.ReadBytes(ColorData.Length);
		}

		public void DecodeImage(System.Drawing.Color[] Entries)
		{
			System.Drawing.Bitmap b = new System.Drawing.Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

			System.Drawing.Imaging.ColorPalette Palette = b.Palette;

			for ( int pos = 0; pos < 256; pos++ )
			{
				Palette.Entries[pos] = Entries[pos];
			}

			b.Palette = Palette;
			System.Drawing.Imaging.BitmapData bmpData = b.LockBits(new System.Drawing.Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, b.PixelFormat);
			System.IntPtr ptr = bmpData.Scan0;
			int startPos = ColorData.Length / bmpData.Height;

			for ( int height = 0; height < b.Height; height++ )
			{
				System.Runtime.InteropServices.Marshal.Copy(ColorData, startPos * height, ptr, bmpData.Width);
				ptr = (System.IntPtr)((int)ptr + bmpData.Stride);
			}

			b.UnlockBits(bmpData);
			Image = b;
		}

		//public void Save(string filename)
		//{
		//    throw new System.NotImplementedException();
		//}

		//public void Save(System.IO.Stream stream)
		//{
		//    throw new System.NotImplementedException();
		//}

		//public bool Load(string path)
		//{
		//    throw new System.NotImplementedException();
		//}

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
			//if ( Path != null )
			//{
			//	System.IO.Stream fs = System.IO.File.Open(Path, System.IO.FileMode.Open);
			//	Data = new byte[fs.Length];
			//	fs.Read(Data, 0, Data.Length);
			//	fs.Close();
			//}

			//if ( Data != null )
			//{
			//	System.IO.MemoryStream memorystream = new System.IO.MemoryStream(Data);
			//	System.IO.BinaryReader br = new System.IO.BinaryReader(memorystream);

			//}
		}
	}
}
