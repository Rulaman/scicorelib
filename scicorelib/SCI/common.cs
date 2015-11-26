namespace System.Runtime.CompilerServices
{
	internal sealed class ExtensionAttribute: Attribute
	{
	}
}

namespace SCI
{
	using Resources;

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
			if ( ReferenceEquals(a1, a2) )
				return true;

			if ( a1 == null || a2 == null )
				return false;

			if ( a1.Length != a2.Length )
				return false;

			System.Collections.Generic.EqualityComparer<T> comparer = System.Collections.Generic.EqualityComparer<T>.Default;
			for ( int i = 0; i < a1.Length; i++ )
			{
				if ( !comparer.Equals(a1[i], a2[i]) )
					return false;
			}
			return true;
		}
	}

	public static class ExtensionMethods
	{
		public static bool IsNullOrEmpty(this string value)
		{
			if ( (value == null) || (value == "") )
			{
				return true;
			}

			return false;
		}
	}
}
