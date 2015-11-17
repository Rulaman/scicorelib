using System;
using System.Collections.Generic;
using System.Text;

namespace SCI
{
	public interface ISciResource
	{
		EResourceType 		ResourceType { get; }
		Int32 				ResourceNumber { get; }
		byte 				FileNumber { get; }
		Int32 				FileOffset { get; }
		ECompressionType 	CompressionType { get; }
		UInt32 				CompressedSize { get; }
		UInt32 				UncompressedSize { get; }
	}

	public interface ISciLoad
    {
		bool Load(string path);
		//List<CResource> ResourceList { get; }
	}
}
