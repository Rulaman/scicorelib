using System;
using System.Collections.Generic;
using System.Text;

namespace SCI.IO.Compression
{
	public abstract class SciCompressionBase
	{
		public abstract void Init();
		public abstract bool Unpack(byte[] inbuf, ref byte[] outbuf);
	}

	public interface ISciCompression
	{
		ECompressionType Type { get; }
	}
}