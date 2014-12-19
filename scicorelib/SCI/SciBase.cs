using System;
using System.Collections.Generic;
using System.Text;

using SCI.Resource;

namespace SCI
{
	public abstract class SciBase
	{
		protected class ResourceInfo
		{
			public byte 	Type;
			public UInt16 	Number;
			public byte 	FileNumber;
			public UInt32 	FileOffset;
		}

		protected List<CResource> _ResourceList = new List<CResource>();
		private List<CResource> PaletteResourceList;
		public List<CResource> ResourceList
		{
			get { return _ResourceList; }
		}

		public CResource FindPaletteResource(int resourceid)
		{
			CResource returnvalue = null;
			string id = resourceid.ToString();

			if ( PaletteResourceList == null )
			{
				PaletteResourceList = new List<CResource>();

				foreach ( CResource item in ResourceList )
				{
					switch ( item.Type )
					{
					case EResourceType.Palette:
					case EResourceType.Palette8x:
						PaletteResourceList.Add(item);
						break;
					};
				}
			}

			while ( (returnvalue == null) && (int.Parse(id) <= 99999) )
			{
				foreach ( CResource item in PaletteResourceList )
				{
					if ( item.Number == int.Parse(id) )
					{
						returnvalue = item;
						break;
					}
				}

				id += "0";
			}

			if ( returnvalue == null )
			{
				foreach ( CResource item in PaletteResourceList )
				{
					if ( item.Number == 999 )
					{
						returnvalue = item;
						break;
					}
				}
			}

			return returnvalue;
		}

		public abstract bool Load(string path);
	}
}
