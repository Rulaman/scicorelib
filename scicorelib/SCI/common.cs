namespace System.Runtime.CompilerServices
{
    internal sealed class ExtensionAttribute : Attribute { }
}

namespace SCI
{
	public static class Common
	{
		public static byte[] StringToByteArray(string str)
		{
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
			return enc.GetBytes(str);
		}

		public static string ByteArrayToString(byte[] arr)
		{
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
			return enc.GetString(arr);
		}

		public static bool ArraysEqual<T>(T[] a1, T[] a2)
		{
			if (ReferenceEquals(a1, a2))
				return true;

			if (a1 == null || a2 == null)
				return false;

			if (a1.Length != a2.Length)
				return false;

			System.Collections.Generic.EqualityComparer<T> comparer = System.Collections.Generic.EqualityComparer<T>.Default;
			for (int i = 0; i < a1.Length; i++)
			{
				if (!comparer.Equals(a1[i], a2[i])) return false;
			}
			return true;
		}
	}

	public static class ExtensionMethods
	{
		public static bool IsNullOrEmpty(this string value)
		{
			if ((value == null) || (value == ""))
			{
				return true;
			}

			return false;
		}
	}
}

namespace SCI.IO
{ 
    public class SciBinaryReader : System.IO.BinaryReader
    {
        private bool DoReverse = false;

        public SciBinaryReader(System.IO.Stream stream)
            : base(stream)
        {
            //Stream = stream;
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
