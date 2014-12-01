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
		Int32 Number { get; set; }
		ECompressionType CompressionType { get; set;  }
		Int32 CompressedSize { get; }
		Int32 UncompressedSize { get; }
	}
}
