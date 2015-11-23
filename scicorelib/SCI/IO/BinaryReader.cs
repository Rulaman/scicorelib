namespace SCI.IO
{
	public class SciBinaryReader : System.IO.BinaryReader
	{
		private bool DoReverse = false;

		public SciBinaryReader(System.IO.Stream stream)
			: base(stream)
		{
		}

		public bool ReverseReading
		{
			get { return DoReverse; }
			set { DoReverse = value; }
		}

		public override short ReadInt16()
		{
			byte[] array = base.ReadBytes(2);
			if (DoReverse) { System.Array.Reverse(array); }
			return System.BitConverter.ToInt16(array, 0);
		}

		public override int ReadInt32()
		{
			byte[] array = base.ReadBytes(4);
			if (DoReverse) { System.Array.Reverse(array); }
			return System.BitConverter.ToInt32(array, 0);
		}

		public override long ReadInt64()
		{
			byte[] array = base.ReadBytes(8);
			if (DoReverse) { System.Array.Reverse(array); }
			return System.BitConverter.ToInt64(array, 0);
		}

		public override ushort ReadUInt16()
		{
			byte[] array = base.ReadBytes(2);
			if (DoReverse) { System.Array.Reverse(array); }
			return System.BitConverter.ToUInt16(array, 0);
		}

		public override uint ReadUInt32()
		{
			byte[] array = base.ReadBytes(4);
			if (DoReverse) { System.Array.Reverse(array); }
			return System.BitConverter.ToUInt32(array, 0);
		}

		public override ulong ReadUInt64()
		{
			byte[] array = base.ReadBytes(8);
			if (DoReverse) { System.Array.Reverse(array); }
			return System.BitConverter.ToUInt64(array, 0);
		}
	}
}
