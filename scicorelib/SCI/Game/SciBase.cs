namespace SCI
{
	using Resources;
	using IO.Compression;

	public abstract class SciBase
	{
		private System.Collections.Generic.List<Resources.ResourceBase> PaletteResourceList;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private System.Collections.Generic.List<Resources.ResourceBase> _ResourceList = new System.Collections.Generic.List<Resources.ResourceBase>();

		public System.Collections.Generic.List<Resources.ResourceBase> ResourceList
		{
			get { return _ResourceList; }
			protected set { _ResourceList = value; }
		}

		public Resources.ResourceBase FindPaletteResource(int resourceid)
		{
			Resources.ResourceBase returnvalue = null;
			string id = resourceid.ToString();

			if ( PaletteResourceList == null )
			{
				PaletteResourceList = new System.Collections.Generic.List<Resources.ResourceBase>();

				foreach ( Resources.ResourceBase item in ResourceList )
				{
					switch ( item.ResourceType )
					{
					case EResourceType.Palette:
						PaletteResourceList.Add(item);
						break;
					};
				}
			}

			while ( (returnvalue == null) && (int.Parse(id) <= 99999) )
			{
				foreach ( Resources.ResourceBase item in PaletteResourceList )
				{
					if ( item.ResourceNumber == int.Parse(id) )
					{
						returnvalue = item;
						break;
					}
				}

				id += "0";
			}

			if ( returnvalue == null )
			{
				foreach ( Resources.ResourceBase item in PaletteResourceList )
				{
					if ( item.FileNumber == 99 )
					{
						returnvalue = item;
						break;
					}
				}
			}

			return returnvalue;
		}

		public abstract bool Expand(string path);

		public virtual bool Load(string path)
		{
			bool retval = true;

			string[] sa = System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.TopDirectoryOnly);
			Resources.ResourceBase resource = null;

			foreach ( string item in sa )
			{
				string name = System.IO.Path.GetFileNameWithoutExtension(item);
				int number = -1;

				int.TryParse(name, out number);

				if ( number == -1 )
				{
					/* keine gültige Resourcendatei */
					continue;
				}

				string ending = System.IO.Path.GetExtension(item);

				if ( ending == "" )
				{
					continue;
				}

				EResourceType type = ResourceTypeConverter.GetResourceTypeByFileending(ending);

				resource = GetResourceByType(type, EGameType.SCI3);
				resource.Path = item;
				resource.ResourceNumber = number;
				resource.UncompressedSize = (uint)(new System.IO.FileInfo(item).Length);
				resource.CompressionType = ECompressionType.None;

				ResourceList.Add(resource);
			}

			return retval;
		}

		public Resources.ResourceBase GetResourceByType(EResourceType resourcetype, EGameType gametype)
		{
			Resources.ResourceBase resource = null;

			switch ( resourcetype )
			{
			case EResourceType.Cursor:
				resource = new Resources.Cursor(gametype);
				break;
			case EResourceType.Palette:
				resource = new Resources.Palette(gametype);
				break;
			case EResourceType.Font:
				resource = new Resources.Font(gametype);
				break;
			case EResourceType.View:
				resource = new Resources.View(gametype);
				break;
			case EResourceType.Picture:
				resource = new Resources.PictureRow(gametype);
				break;
			case EResourceType.Message:
				resource = new Resources.Message(gametype);
				break;
			default:
				resource = new Resources.Dummy(gametype, resourcetype);
				break;
			};

			return resource;
		}
	}
}
