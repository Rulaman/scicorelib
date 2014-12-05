namespace SCI.IO.Compression
{
	using System;

	public class HUffman: SciCompressionBase, ISciCompression
	{
		private byte[] nodes;
		private Int16 nodepos = 0;
		private Int32 _dwBits;		///< bits buffer
		private byte _nBits;		///< number of unread bits in _dwBits
		private Int32 _szPacked;	///< size of the compressed data
		private Int32 _szUnpacked;	///< size of the decompressed data
		private Int32 _dwRead;		///< number of bytes read from _src
		private Int32 _dwWrote;		///< number of bytes written to _dest
		private byte[] _src;
		private byte[] _dest;

		System.IO.BinaryReader br;

		public override void Init()
		{

		}
		public override bool Unpack(byte[] inbuf, ref byte[] outbuf)
		{
			_src = inbuf;
			_dest = outbuf;
			_szPacked = inbuf.Length;
			_szUnpacked = outbuf.Length;
			_nBits = 0;
			_dwRead = 0;
			_dwWrote = 0;
			_dwBits = 0;

			byte numnodes;
			Int16 c;
			Int32 terminator;

			br = new System.IO.BinaryReader(new System.IO.MemoryStream(inbuf));

			numnodes = br.ReadByte();
			terminator = br.ReadByte() | 0x100;

			nodes = br.ReadBytes(numnodes << 1);

			while ( (c = getc2() != terminator) && (SCI >= 0) && !isFinished() )
			{
				putByte(c);
			}

			br.Close();

			return true;
		}



		public bool Decode(byte[] packed, ref byte[] unpacked)
		{
			
		}
		
		private Int16 getc2()
		{
			byte node = nodes[nodepos];
			Int16 next;
			
			while ( nodes[nodepos+1] )
			{
				if ( getBitsMSB(1) )
				{
					next = node[nodepos+1] & 0x0F; // use lower 4 bits
					
					if ( next == 0 )
					{
						return getByteMSB() | 0x100;
					}
				}
				else
				{
					next = node[nodepos+1] >> 4; // use higher 4 bits
				}
				
				nodepos = next << 1;
			}
			
			return (Int16)(node[nodepos] | (node[nodepos+1] << 8));
		}

		void fetchBitsMSB()
		{
			while ( (_nBits <= 24) > 0 )
			{
				_dwBits |= ((UInt32)_src.readByte()) << (24 - _nBits);
				_nBits += 8;
				_dwRead++;
			}
		}

		UInt32 getBitsMSB(int n)
		{
			// fetching more data to buffer if needed
			if ( _nBits < n )
				fetchBitsMSB();
			UInt32 ret = _dwBits >> (32 - n);
			_dwBits <<= n;
			_nBits -= n;
			return ret;
		}

		byte getByteMSB()
		{
			return getBitsMSB(8);
		}

		void fetchBitsLSB()
		{
			while ( _nBits <= 24 )
			{
				_dwBits |= ((UInt32)_src->readByte()) << _nBits;
				_nBits += 8;
				_dwRead++;
			}
		}

		UInt32 getBitsLSB(int n)
		{
			// fetching more data to buffer if needed
			if ( _nBits < n )
			{
				fetchBitsLSB();
			}
			UInt32 ret = (_dwBits & ~((~0) << n));
			_dwBits >>= n;
			_nBits -= n;
			return ret;
		}

		byte getByteLSB()
		{
			return getBitsLSB(8);
		}

		void putByte(byte b)
		{
			_dest[_dwWrote++] = b;
		}

		#region ISciCompression Member
		public ECompressionType Type
		{
			get { return ECompressionType.Huffman; }
		}
		#endregion
	}
}
