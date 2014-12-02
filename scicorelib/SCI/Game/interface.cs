using System;
using System.Collections.Generic;
using System.Text;

namespace SCI.Interface
{
	public interface IGameType
	{
		EGameType Type { get; }
	}

	public interface ISciResource
	{
		EResourceType Type { get; }
//		UInt16 Number { get; set; }
		ECompressionType CompressionType { get; set;  }
		UInt32 CompressedSize { get; set; }
		UInt32 UncompressedSize { get; set; }
	}

	public interface ISciType
	{
		bool Load(string path);
		List<CResource> ResourceList { get; }
	}
}
