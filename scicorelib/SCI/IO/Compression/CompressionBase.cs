namespace SCI.IO.Compression
{
	public enum ECompressionValue
	{
		Ok,
		DecompressionError
	}

	public abstract class SciCompressionBase
	{
		public abstract void Init();

		public abstract ECompressionValue Unpack(ref byte[] inbuf, ref byte[] outbuf);
	}

	public interface ISciCompression
	{
		ECompressionType Type
		{ get; }
	}
}
