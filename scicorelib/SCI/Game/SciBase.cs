﻿namespace SCI
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
    }
}