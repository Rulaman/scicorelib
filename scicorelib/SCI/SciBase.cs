using System.Collections.Generic;
using System.Diagnostics;

namespace SCI
{
    public abstract class CSciBase
    {
        private List<CResource> PaletteResourceList;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<CResource> _ResourceList = new List<CResource>();

        public List<CResource> ResourceList
        {
            get { return _ResourceList; }
            protected set { _ResourceList = value; }
        }

        public CResource FindPaletteResource(int resourceid)
        {
            CResource returnvalue = null;
            string id = resourceid.ToString();

            if (PaletteResourceList == null)
            {
                PaletteResourceList = new List<CResource>();

                foreach (CResource item in ResourceList)
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
                foreach (CResource item in PaletteResourceList)
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
                foreach (CResource item in PaletteResourceList)
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
