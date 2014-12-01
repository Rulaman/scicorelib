using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SCI.Drawing
{
	using SCI.Interface;

	public sealed class SciPicture: ISciResource
	{
		private EResourceType ResourceType;

		internal SciPicture(EResourceType resourcetype)
		{
			ResourceType = resourcetype;
		}

		#region ISciResource Member
		public EResourceType Type
		{
			get
			{
				return ResourceType;
			}
		}
		public int Number
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		public ECompressionType CompressionType
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		public int CompressedSize
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		public int UncompressedSize
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		#endregion
	}

	public sealed class Picture
	{
		private Size			InternalSize			= new Size();
		
		private Point			ScreenPosition			= new Point();
		private byte			TransparentKey;
		private byte			Compression;
		private UInt16			Flags;
		public Bitmap			Image;
		public Int32			OffsetRLE;
		public byte[]			ColorData;
		public object			Tag;

		public Image FromFile(string filename)
		{
			return Image;
		}
		public void ReadHeaderFromStream(System.IO.Stream stream)
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
		public Size Size
		{
			get { return InternalSize; }
		}
	}
}
