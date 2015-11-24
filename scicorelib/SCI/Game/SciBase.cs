namespace SCI
{
    public abstract class SciBase
    {
        private System.Collections.Generic.List<ResourceBase> PaletteResourceList;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private System.Collections.Generic.List<ResourceBase> _ResourceList = new System.Collections.Generic.List<ResourceBase>();

        public System.Collections.Generic.List<ResourceBase> ResourceList
        {
            get { return _ResourceList; }
            protected set { _ResourceList = value; }
        }

        public ResourceBase FindPaletteResource(int resourceid)
        {
            ResourceBase returnvalue = null;
            string id = resourceid.ToString();

            if (PaletteResourceList == null)
            {
                PaletteResourceList = new System.Collections.Generic.List<ResourceBase>();

                foreach (ResourceBase item in ResourceList)
                {
                    switch (item.ResourceType)
                    {
                        case EResourceType.Palette:
                        case EResourceType.Palette8x:
                            PaletteResourceList.Add(item);
                            break;
                    };
                }
            }

            while ((returnvalue == null) && (int.Parse(id) <= 99999))
            {
                foreach (ResourceBase item in PaletteResourceList)
                {
                    if (item.ResourceNumber == int.Parse(id))
                    {
                        returnvalue = item;
                        break;
                    }
                }

                id += "0";
            }

            if (returnvalue == null)
            {
                foreach (ResourceBase item in PaletteResourceList)
                {
                    if (item.FileNumber == 99)
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
			ResourceBase resource = null;

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

				EResourceType type = Common.GetResourceTypeByFileending(ending);

				switch ( type )
				{
				case EResourceType.Audio:
					break;
				case EResourceType.Picture:
				case EResourceType.Picture8x:
					resource = new Resources.Picture(EGameType.SCI3);
					break;
				case EResourceType.View:
				case EResourceType.View8x:
					resource = new Resources.View(EGameType.SCI3);
					break;
				case EResourceType.Message:
				case EResourceType.Message8x:
					resource = new Resources.Message(EGameType.SCI3);
					break;
				default:
					resource = new Dummy(EGameType.None, type);
					break;
				}

				resource.Path = item;
				resource.ResourceNumber = number;
				resource.UncompressedSize = (uint)(new System.IO.FileInfo(item).Length);
				resource.CompressionType = ECompressionType.None;

				ResourceList.Add(resource);
			}

			return retval;
		}
	}
}
