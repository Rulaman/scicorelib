using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace SCI
{
    public class Dummy : CResource, ISciLoad
    {
        public bool Load(string path)
        {
            return true;
        }
    }

    public class ResourceInfo
    {
        public byte Type;
        public UInt16 Number;
        public byte FileNumber;
        public UInt32 FileOffset;
    }

    public abstract class CResource: ISciResource
    {
        #region Properties
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EResourceType _ResourceType;

        [Browsable(true), Description("The type of the resource. Picture, View, Script, …")]
        public EResourceType ResourceType
        {
            get { return _ResourceType; }
            internal set { _ResourceType = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ECompressionType _CompressionType;

        [Browsable(true), Description("The compression method of the resource. Huffmann, DCL, STACpack, …")]
        public ECompressionType CompressionType
        {
            get { return _CompressionType; }
            internal set { _CompressionType = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected uint _CompressedSize;

        [Browsable(true), Description("The compressed size of the resource.")]
        public UInt32 CompressedSize
        {
            get { return _CompressedSize; }
            internal set { _CompressedSize = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected uint _UncompressedSize;

        [Browsable(true), Description("The uncompressed size of the resource.")]
        public UInt32 UncompressedSize
        {
            get { return _UncompressedSize; }
            internal set { _UncompressedSize = value; }
        }
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected Int32 _ResourceNumber;

        [Browsable(true), Description("The number of the resource.")]
        public Int32 ResourceNumber
        {
            get { return _ResourceNumber; }
            internal set { _ResourceNumber = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected byte _FileNumber;

        [Browsable(true), Description("The file in with the resource is located.")]
        public byte FileNumber
        {
            get { return _FileNumber; }
            internal set { _FileNumber = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected Int32 _FileOffset;

        [Browsable(true), Description("The offset within the file of the resource.")]
        public Int32 FileOffset
        {
            get { return _FileOffset; }
            internal set { _FileOffset = value; }
        }
        #endregion Properties
	}

    public abstract class CSciBase
    {
        private List<CResource> PaletteResourceList;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected List<CResource> _ResourceList = new List<CResource>();
		
		public List<CResource> ResourceList
		{
			get { return _ResourceList; }
		}

		protected readonly List<UInt32> OffsetList = new List<UInt32>();



		public CResource FindPaletteResource(int resourceid)
		{
			CResource returnvalue = null;
			string id = resourceid.ToString();

			if ( PaletteResourceList == null )
			{
				PaletteResourceList = new List<CResource>();

				foreach ( CResource item in ResourceList )
				{
					switch ( item.ResourceType )
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
				foreach ( CResource item in PaletteResourceList )
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

		public abstract bool Load(string path);
    }

}
