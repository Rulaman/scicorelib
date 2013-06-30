using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SCI.Drawing
{
	public sealed class Picture
	{
		private Size			InternalSize			= new Size();
		private object			Object;
		private Point			ScreenPosition			= new Point();
		private byte			TransparentKey;
		private byte			Compression;
		private UInt16			Flags;
		public Bitmap			Image;
		public Int32			OffsetRLE;
		public byte[]			ColorData;

		public Image FromFile(string filename)
		{
			return Image;
		}
		public void HeaderFromStream(System.IO.Stream stream)
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
		public void ColorDataFromStream(System.IO.Stream stream)
		{
			System.IO.BinaryReader br = new System.IO.BinaryReader(stream);

			ColorData = new byte[Width * Height];
			ColorData = br.ReadBytes(ColorData.Length);
		}
		public void DecodeImage(Color[] Entries)
		{
			Bitmap b = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);

			ColorPalette Palette = b.Palette;

			for ( int pos = 0; pos < 256; pos++ )
			{
				Palette.Entries[pos] = Entries[pos];
			}

			b.Palette = Palette;
			BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
			IntPtr ptr = bmpData.Scan0;
			Int32 startPos = ColorData.Length / bmpData.Height;

			for ( int height = 0; height < b.Height; height++ )
			{
				System.Runtime.InteropServices.Marshal.Copy(ColorData, startPos * height, ptr, bmpData.Width);
				ptr = (IntPtr)((int)ptr + bmpData.Stride);
			}

			b.UnlockBits(bmpData);
			Image = b;
		}

		public void Save(string filename)
		{
			throw new NotImplementedException();
		}
		public void Save(System.IO.Stream stream)
		{
			throw new NotImplementedException();
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
	}
}
