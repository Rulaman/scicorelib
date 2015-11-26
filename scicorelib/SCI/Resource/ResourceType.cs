namespace SCI.Resources
{
	public enum EResourceType
	{
		[ResourceType("v56")]
		View = 0,

		[ResourceType("p56")]
		Picture = 1,

		[ResourceType("scr")]
		Script = 2,

		[ResourceType("tex")]
		Text = 3,

		[ResourceType("snd")]
		Sound = 4,

		[ResourceType("etc")]
		Etc = 5,

		[ResourceType("voc")]
		Vocab = 6,

		[ResourceType("fon")]
		Font = 7,

		[ResourceType("cur")]
		Cursor = 8,

		[ResourceType("pat")]
		Patch = 9,

		[ResourceType("bit")]
		Bitmap = 10,

		[ResourceType("pal")]
		Palette = 11,

		[ResourceType("cda")]
		CDAudio = 12,

		[ResourceType("aud")]
		Audio = 13,

		[ResourceType("syn")]
		Sync = 14,

		[ResourceType("msg")]
		Message = 15,

		[ResourceType("map")]
		Map = 16,

		[ResourceType("hep")]
		Heap = 17,

		[ResourceType("chk")]
		Chunk = 18,

		[ResourceType("aud36")]
		Audio36 = 19,

		[ResourceType("syn36")]
		Sync36 = 20,

		[ResourceType("trs")]
		Translation = 21,

		[ResourceType("rob")]
		Robot = 22,

		[ResourceType("vmd")]
		Vmd = 23,

		[ResourceType("dck")]
		Duck = 24,

		[ResourceType("clt")]
		Clut = 25,

		[ResourceType("tga")]
		Targa = 26,

		[ResourceType("zzz")]
		ZZZ = 27,

		//[ResourceType("aud")]
		//Audio368x = 0x92,
		//[ResourceType("syn")]
		//Sync368x = 0x93,
		//[ResourceType("xlt")]
		//XLate8x = 0x94,

		[ResourceType("None")]
		None = 0xFE, // damit man weiß, dass diese Resource ungültig ist

		[ResourceType("eof")]
		EndOfIndex = 0xFF
	}

	public class ResourceTypeAttribute: System.Attribute
	{
		public readonly string Name;

		public ResourceTypeAttribute(string name)
		{ this.Name = name; }

		public override string ToString()
		{
			return Name;
		}

		public static string Get(System.Type tp, string name)
		{
			System.Reflection.MemberInfo[] mi = tp.GetMember(name);
			if ( mi != null && mi.Length > 0 )
			{
				ResourceTypeAttribute attr = System.Attribute.GetCustomAttribute(mi[0],
					typeof(ResourceTypeAttribute)) as ResourceTypeAttribute;

				if ( attr != null )
				{
					return attr.Name;
				}
			}
			return null;
		}
	}

	public static class ResourceTypeConverter
	{
		public static string GetFileEnding(EResourceType types)
		{
			var memInfo = typeof(EResourceType).GetMember(types.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(ResourceTypeAttribute), false);
			var description = ((ResourceTypeAttribute)attributes[0]).Name;

			return description.ToString();
		}

		//private static EResourceType? GetValueByAttribute<EResourceType, ResourceTypeAttribute>() where EResourceType : struct
		//{
		//	System.Type type = typeof(EResourceType);
		//	System.Reflection.FieldInfo[] fields = type.GetFields();

		//	foreach ( System.Reflection.FieldInfo field in fields )
		//	{
		//		System.Attribute[] attributes = System.Attribute.GetCustomAttributes(field);

		//		foreach ( System.Attribute attribute in attributes )
		//		{
		//			if ( attribute is ResourceTypeAttribute )
		//				return (EResourceType)field.GetValue(null);
		//		}
		//	}
		//	return null;
		//}

		public static EResourceType GetResourceTypeByFileending(string ending)
		{
			ending = ending.Substring(1);
			System.Type type = typeof(EResourceType);
			System.Reflection.FieldInfo[] fields = type.GetFields();

			foreach ( System.Reflection.FieldInfo field in fields )
			{
				System.Attribute[] attributes = System.Attribute.GetCustomAttributes(field);

				foreach ( System.Attribute attribute in attributes )
				{
					if ( attribute is ResourceTypeAttribute && attribute.ToString() == ending )
					{
						EResourceType t = (EResourceType)field.GetValue(null);
						return t;
					}
				}
			}

			System.Reflection.FieldInfo myf = typeof(EResourceType).GetField(ending);

			return EResourceType.None;// description;
		}
	}
}
