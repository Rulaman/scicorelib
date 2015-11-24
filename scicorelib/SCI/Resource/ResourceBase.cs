namespace SCI
{
	public abstract class ResourceBase
	{
		public ResourceBase(EGameType gametype)
		{
			_GameType = gametype;
		}

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private byte[] _Data;

		[System.ComponentModel.Browsable(false), System.ComponentModel.Description("The uncompressed Data.")]
		public byte[] Data
		{
			get { return _Data; }
			//set { CheckSetAndSend(ref _Data, value);
			set
			{
				_Data = new byte[value.Length];
				System.Array.Copy(value, _Data, value.Length);
			}
		}

		[System.ComponentModel.Browsable(true), System.ComponentModel.Description("The type of the resource. Picture, View, Script, …")]
		public abstract EResourceType ResourceType { get; }

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private EGameType _GameType;

		[System.ComponentModel.Browsable(true), System.ComponentModel.Description("The type of the game for the reource. SCI0, SCI01, SCI1, SCI2, SCI3 …")]
		public EGameType GameType
		{
			get { return _GameType; }
			internal set { _GameType = value; }
		}

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        protected ECompressionType _CompressionType;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Description("The compression method of the resource. Huffmann, DCL, STACpack, …")]
        public ECompressionType CompressionType
        {
            get { return _CompressionType; }
            internal set { _CompressionType = value; }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        protected uint _CompressedSize;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Description("The compressed size of the resource.")]
        public uint CompressedSize
        {
            get { return _CompressedSize; }
            internal set { _CompressedSize = value; }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        protected uint _UncompressedSize;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Description("The uncompressed size of the resource.")]
        public uint UncompressedSize
        {
            get { return _UncompressedSize; }
            internal set { _UncompressedSize = value; }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        protected int _ResourceNumber;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Description("The number of the resource.")]
        public int ResourceNumber
        {
            get { return _ResourceNumber; }
            internal set { _ResourceNumber = value; }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        protected byte _FileNumber;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Description("The file in with the resource is located.")]
        public byte FileNumber
        {
            get { return _FileNumber; }
            internal set { _FileNumber = value; }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        protected int _FileOffset;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Description("The offset within the file of the resource.")]
        public int FileOffset
        {
            get { return _FileOffset; }
            internal set { _FileOffset = value; }
        }

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private string _Path;

		[System.ComponentModel.Browsable(true), System.ComponentModel.Description("Pfad, von wo die Resource geladen wurde.")]
		public string Path
		{
			get { return _Path; }
			//set { CheckSetAndSend(ref _Path, value);
			set { _Path = value; }
		}

		public void Save(string path)
		{
			string filename = System.IO.Path.Combine(path, string.Format("{0}\\{1}.{2}", "entpackt", ResourceNumber, Common.GetFileEnding(ResourceType)));
			System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filename));
			System.IO.FileStream fs = System.IO.File.Open(filename, System.IO.FileMode.OpenOrCreate);
			fs.Write(Data, 0, Data.Length);
			fs.Close();
		}
    }

    public class Dummy : ResourceBase
    {
		public Dummy(EGameType gametype, EResourceType type) : base(gametype) { _ResourceType = type; }

		private EResourceType _ResourceType = EResourceType.None;

		public override EResourceType ResourceType
		{
			get { return _ResourceType; }
		}
	}
}
