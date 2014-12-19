using System;
using System.Collections.Generic;
using System.Text;

namespace SCI
{
	public interface ISciResource
	{
		EResourceType 		ResourceType { get; }
		UInt16 				ResourceNumber { get; set; }
		byte 				FileNumber { get; }
		UInt32 				FileOffset { get; }
		ECompressionType 	CompressionType { get; }
		UInt32 				CompressedSize { get; }
		UInt32 				UncompressedSize { get; }
	}

	public interface ISciType
	{
		bool Load(string path);
		List<CResource> ResourceList { get; }
	}
}
