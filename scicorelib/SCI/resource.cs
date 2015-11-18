using System;
using System.ComponentModel;
using System.Diagnostics;

namespace SCI
{
    public abstract class CResource : ISciResource
    {
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
    }

    public class Dummy : CResource
    {
    }
}
