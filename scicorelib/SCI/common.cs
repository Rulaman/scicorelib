namespace System.Runtime.CompilerServices
{
    internal sealed class ExtensionAttribute : Attribute { }
}

namespace SCI
{
    using System;

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

    public class SciBinaryReader : System.IO.BinaryReader
    {
        //private System.IO.Stream Stream;
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

        public override Int16 ReadInt16()
        {
            byte[] array = base.ReadBytes(2);
            if (DoReverse) { Array.Reverse(array); }
            return BitConverter.ToInt16(array, 0);
        }

        public override Int32 ReadInt32()
        {
            byte[] array = base.ReadBytes(4);
            if (DoReverse) { Array.Reverse(array); }
            return BitConverter.ToInt32(array, 0);
        }

        public override Int64 ReadInt64()
        {
            byte[] array = base.ReadBytes(8);
            if (DoReverse) { Array.Reverse(array); }
            return BitConverter.ToInt64(array, 0);
        }

        public override UInt16 ReadUInt16()
        {
            byte[] array = base.ReadBytes(2);
            if (DoReverse) { Array.Reverse(array); }
            return BitConverter.ToUInt16(array, 0);
        }

        public override UInt32 ReadUInt32()
        {
            byte[] array = base.ReadBytes(4);
            if (DoReverse) { Array.Reverse(array); }
            return BitConverter.ToUInt32(array, 0);
        }

        public override UInt64 ReadUInt64()
        {
            byte[] array = base.ReadBytes(8);
            if (DoReverse) { Array.Reverse(array); }
            return BitConverter.ToUInt64(array, 0);
        }
    }
}
