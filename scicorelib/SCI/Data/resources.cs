using System;

namespace SCI.Data
{
    public enum ResType
    {
        TOTAL_RES_TYPES = 28,
        TOTAL_RES_TYPES2 = 29,
        TOTAL_RES_TYPES3 = 30,
    }

    public class Resources
    {
        public class ResInfo
        {
            public byte type;
            public byte pack;
            public UInt16 encType;
            public UInt16 number;
            public UInt32 offset;
            public UInt32 size;
            public UInt32 encSize;
            public ResInfo prev;
            public ResInfo next;
        }

        public class RESIDFO
        {
            public byte type;
            public byte pack;
            public UInt16 number;
            public UInt16 size;
        }

        public static int rsMAX_ALLOCS = 64;

        public class RESIDX
        {
            public ResInfo[] resInfo = new ResInfo[(int)ResType.TOTAL_RES_TYPES2];
            public int[] totalRes = new int[(int)ResType.TOTAL_RES_TYPES2];
            public ResInfo[] selResItems = new ResInfo[(int)ResType.TOTAL_RES_TYPES2];
            public int selRes;
            public int totalAllocs;
            public int allocPtr;
            public int maxPack;
            public ResInfo[] allocBufs = new ResInfo[rsMAX_ALLOCS];
            public ResInfo lastAlloc;
        }
    }

    public class RESTYPE
    {
        public byte index;
        public byte id;
        public string name;
        public string namePl;
        public string ext;
        public string abbr;
        public string description;
        public int iconIndex;

        //struct _EDTWND *edtWnd;
        //TTabSheet* previewPage;
        public string filter;
    }
}
