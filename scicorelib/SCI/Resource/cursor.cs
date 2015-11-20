namespace SCI.Resource
{
	public class Cursor: CResource
	{
		public override EResourceType ResourceType
		{
			get { return EResourceType.Cursor; }
		}

		public Cursor(EGameType gametype): base(gametype) { }

		public System.Drawing.Point Hotspot = System.Drawing.Point.Empty;
		private ushort[] TransparencyMask = new ushort[16];
		private ushort[] ColorMask = new ushort[16];
		private System.Drawing.Size CursorSize = new System.Drawing.Size(16, 16);
		public System.Drawing.Bitmap Image = null;

		public void Decode(string filename)
		{
			System.IO.FileStream filestream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
			System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(filestream);

			Decode(binaryReader);
		}

		public void Decode(System.IO.Stream stream)
		{
			System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(stream);
			Decode(binaryReader);
		}

		public void Decode(System.IO.BinaryReader binaryReader)
		{
			ushort xhotspot = binaryReader.ReadUInt16();
			ushort yhotspot = binaryReader.ReadUInt16();

			switch (GameType)
			{
				case EGameType.SCI0:
					xhotspot = 0;
					break;
			};

			Hotspot = new System.Drawing.Point(xhotspot, yhotspot);

			for (int i = 0; i < 16; i++)
			{
				TransparencyMask[i] = binaryReader.ReadUInt16();
			}

			for (int i = 0; i < 16; i++)
			{
				ColorMask[i] = binaryReader.ReadUInt16();
			}

			/* All Information are read */
			/* Build now the cursor */

			Image = new System.Drawing.Bitmap(CursorSize.Width, CursorSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, CursorSize.Width, CursorSize.Height);
			System.Drawing.Imaging.BitmapData bdat = Image.LockBits(rectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, Image.PixelFormat);

			ushort transpLine = 0;
			ushort colorLine = 0;

			for (int y = 0; y < CursorSize.Width; y++)
			{
				transpLine = TransparencyMask[y];
				colorLine = ColorMask[y];

				for (int x = 0; x < CursorSize.Height; x++)
				{
					int index = y * bdat.Stride + (x * 4);

					bool t = ((transpLine >> x) & 0x1) > 0;
					bool c = ((colorLine >> x) & 0x1) > 0;

					byte vala = 0;
					byte valr = 0;
					byte valg = 0;
					byte valb = 0;

					switch (GameType)
					{
						case EGameType.SCI0:

							if  (t == false /* && c == false */ )
							{
								/* weiße Transparenz */
								vala = 255;
								valr = 255;
								valg = 255;
								valb = 255;
							}
							else
							{
								if (c == false)
								{
									vala = 0;
									valr = 0;
									valg = 0;
									valb = 0;
								}
								else
								{
									vala = 0;
									valr = 255;
									valg = 255;
									valb = 255;
								}
							}
							
							break;
						case EGameType.SCI01:
						case EGameType.SCI1:
							break;
						default:
							/* in other games not used */
							break;
					};

					System.Runtime.InteropServices.Marshal.WriteByte(bdat.Scan0, index, vala);
					System.Runtime.InteropServices.Marshal.WriteByte(bdat.Scan0, index, valr);
					System.Runtime.InteropServices.Marshal.WriteByte(bdat.Scan0, index, valg);
					System.Runtime.InteropServices.Marshal.WriteByte(bdat.Scan0, index, valb);
				}
			}

			Image.UnlockBits(bdat);
		}
    }
}
