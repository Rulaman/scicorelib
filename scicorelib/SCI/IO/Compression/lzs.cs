using System;
using System.Collections.Generic;
using System.Text;

namespace SCI.IO.Compression
{
	public class LZS: SciCompressionBase, ISciCompression
	{
		private Int32 GetReverseBits(Int32 numbits, ref byte[] value, ref UInt64 srcpos)
		{
			Int32 result = 0;

			if ( numbits > 0 )
			{
				Int32 bytpos = (Int32)(srcpos / 8);
				Int32 bitpos = (Int32)(srcpos % 8);

				if ( bytpos + 2 < value.Length )
				{
					result = value[bytpos] << 16 | value[bytpos + 1] << 8 | value[bytpos + 2];
				}
				else if ( bytpos + 1 < value.Length )
				{
					result = value[bytpos] << 16 | value[bytpos + 1] << 8;
				}
				else
				{
					result = value[bytpos] << 16;
				}

				result = (Int32)((result >> (24 - numbits - bitpos)) & ((1L << numbits) - 1));

				srcpos += (UInt64)numbits;
			}
			return result;
		}
		private Int32 GetLength(ref byte[] value, ref UInt64 srcpos)
		{
			Int32 bits;
			Int32 length = 2;
			do
			{
				bits = GetReverseBits(2, ref value, ref srcpos);
				length += bits;
			} while ( (bits == 3) && (length < 8) );

			if ( length == 8 )
			{
				do
				{
					bits = GetReverseBits(4, ref value, ref srcpos);
					length += bits;
				} while ( bits == 15 );
			}

			return length;
		}
		public override void Init()
		{
			
		}
		public override bool Unpack(byte[] inbuf, ref byte[] outbuf)
		{
			UInt64 sourceBufferPosition = 0;
			Int32 outBufferPosition = 0;
			Int32 tag = 0;
			Int32 offset = 0;

			do
			{
				tag = GetReverseBits((Int32)1, ref inbuf, ref sourceBufferPosition);

				if ( 0 == tag )
				{
					outbuf[outBufferPosition++] = (byte)GetReverseBits(8, ref inbuf, ref sourceBufferPosition);
				}
				else
				{
					if ( 1 == (tag = GetReverseBits(1, ref inbuf, ref sourceBufferPosition)) )
					{
						if ( 0 == (offset = GetReverseBits(7, ref inbuf, ref sourceBufferPosition)) )
						{
							break;
						}
					}
					else
					{
						offset = GetReverseBits(11, ref inbuf, ref sourceBufferPosition);
					}

					Int32 len = GetLength(ref inbuf, ref sourceBufferPosition);
					Int32 dic = outBufferPosition - offset;

					if ( dic < 0 )
					{
						break;
					}

					while ( len-- > 0 )
					{
						outbuf[outBufferPosition++] = outbuf[dic++];
					};
				}
			} while ( true );

			return true;
		}

		#region ISciCompression Member
		public ECompressionType Type
		{
			get { return ECompressionType.STACpack; }
		}
		#endregion
	}
}
