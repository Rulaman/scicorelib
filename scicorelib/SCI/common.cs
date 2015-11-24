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

		public static string Get(System.Type tp, string name)
		{
			System.Reflection.MemberInfo[] mi = tp.GetMember(name);
			if (mi != null && mi.Length > 0)
			{
				ResourceTypeAttribute attr = System.Attribute.GetCustomAttribute(mi[0],
					typeof(ResourceTypeAttribute)) as ResourceTypeAttribute;

				if (attr != null)
				{
					return attr.Name;
				}
			}
			return null;
		}
	}

	public static class Common
	{
		public static string GetFileEnding(EResourceType types)
		{
			var memInfo = typeof(EResourceType).GetMember(types.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(ResourceTypeAttribute), false);
			var description = ((ResourceTypeAttribute)attributes[0]).Name;

			return description.ToString();
		}

		static EResourceType? GetValueByAttribute<EResourceType, ResourceTypeAttribute>() where EResourceType : struct
		{
			System.Type type = typeof(EResourceType);
			System.Reflection.FieldInfo[] fields = type.GetFields();

			foreach (System.Reflection.FieldInfo field in fields)
			{
				System.Attribute[] attributes = System.Attribute.GetCustomAttributes(field);

				foreach (System.Attribute attribute in attributes)
				{
					if (attribute is ResourceTypeAttribute) return (EResourceType)field.GetValue(null);
				}
			}
			return null;
		}



		public static EResourceType GetResourceTypeByFileending(string ending)
		{
			ending = ending.Substring(1);
			System.Type type = typeof(EResourceType);
			System.Reflection.FieldInfo[] fields = type.GetFields();

			foreach (System.Reflection.FieldInfo field in fields)
			{
				System.Attribute[] attributes = System.Attribute.GetCustomAttributes(field);

				foreach (System.Attribute attribute in attributes)
				{
					if (attribute is ResourceTypeAttribute && attribute.ToString() == ending)
					{
						EResourceType t =  (EResourceType)field.GetValue(null);
						return t;
					}
				}
			}

			System.Reflection.FieldInfo myf = typeof(EResourceType).GetField(ending);



			return EResourceType.None;// description;
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
