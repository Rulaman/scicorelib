namespace System.Runtime.CompilerServices
{
    internal sealed class ExtensionAttribute : Attribute { }
}

namespace SCI
{
	public class ResourceTypeAttribute : System.Attribute
	{
		public readonly string Name;

		public ResourceTypeAttribute(string name) { this.Name = name; }

		public override string ToString()
		{
			return Name;
		}
	}

	public static class Common
	{
		public static string GetFileEnding(EResourceType types)
		{
			//System.Type dataType = System.Enum.GetUnderlyingType(typeof(EResourceType));

			//foreach (System.Reflection.FieldInfo field in 	
			//	typeof(EResourceType).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Public))
			//{
			//	object value = field.GetValue(null);
			//	//Console.WriteLine("{0}={1}", field.Name,Convert.ChangeType(value, dataType));
			//	foreach (System.Attribute attrib in field.GetCustomAttributes(true))
			//	{
			//		//Console.WriteLine("\t{0}", attrib);
			//	}
			//}
			////Console.WriteLine("Any key to quit.");
			////Console.ReadLine();


			var type = typeof(EResourceType);
			var memInfo = type.GetMember(types.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(ResourceTypeAttribute), false);
			var description = ((ResourceTypeAttribute)attributes[0]).Name;

			return description.ToString();
		}

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
