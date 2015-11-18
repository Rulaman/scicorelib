using System;

namespace SCI.IO.Compression
{
    public class DCL : SciCompressionBase, ISciCompression
    {
        private struct Node
        {
            public UInt32 Path;     //binary path
            public byte Len;        //number of bits in path
            public UInt16 Value;        //tree value at this path

            public Node(UInt32 path, byte len, UInt16 Value)
            {
                this.Path = path;
                this.Len = len;
                this.Value = Value;
            }
        }

        #region Nodes (Literals, Lengths, Distances

        private Node[] Literals = new Node[]
        {
            new Node(0xf   , 4,0x20),new Node(0x17  , 5,0x45),new Node(0x7   , 5,0x61),new Node(0x1b  , 5,0x65),new Node(0xb   , 5,0x69),new Node(0x13  , 5,0x6c),new Node(0x3   , 5,0x6e),new Node(0x1d  , 5,0x6f),
            new Node(0xd   , 5,0x72),new Node(0x15  , 5,0x73),new Node(0x5   , 5,0x74),new Node(0x19  , 5,0x75),new Node(0x29  , 6,0x2d),new Node(0x9   , 6,0x31),new Node(0x31  , 6,0x41),new Node(0x11  , 6,0x43),
            new Node(0x21  , 6,0x44),new Node(0x1   , 6,0x49),new Node(0x3e  , 6,0x4c),new Node(0x1e  , 6,0x4e),new Node(0x2e  , 6,0x4f),new Node(0xe   , 6,0x52),new Node(0x36  , 6,0x53),new Node(0x16  , 6,0x54),
            new Node(0x26  , 6,0x62),new Node(0x6   , 6,0x63),new Node(0x3a  , 6,0x64),new Node(0x1a  , 6,0x66),new Node(0x2a  , 6,0x67),new Node(0xa   , 6,0x68),new Node(0x32  , 6,0x6d),new Node(0x12  , 6,0x70),
            new Node(0x62  , 7,0x0a),new Node(0x22  , 7,0x0d),new Node(0x42  , 7,0x28),new Node(0x2   , 7,0x29),new Node(0x7c  , 7,0x2c),new Node(0x3c  , 7,0x2e),new Node(0x5c  , 7,0x30),new Node(0x1c  , 7,0x32),
            new Node(0x6c  , 7,0x33),new Node(0x2c  , 7,0x34),new Node(0x4c  , 7,0x35),new Node(0xc   , 7,0x37),new Node(0x74  , 7,0x38),new Node(0x34  , 7,0x3d),new Node(0x54  , 7,0x42),new Node(0x14  , 7,0x46),
            new Node(0x64  , 7,0x4d),new Node(0x24  , 7,0x50),new Node(0x44  , 7,0x55),new Node(0x4   , 7,0x6b),new Node(0x78  , 7,0x77),new Node(0xb8  , 8,0x09),new Node(0x38  , 8,0x22),new Node(0xd8  , 8,0x27),
            new Node(0x58  , 8,0x2a),new Node(0x98  , 8,0x2f),new Node(0x18  , 8,0x36),new Node(0xe8  , 8,0x39),new Node(0x68  , 8,0x3a),new Node(0xa8  , 8,0x47),new Node(0x28  , 8,0x48),new Node(0xc8  , 8,0x57),
            new Node(0x48  , 8,0x5b),new Node(0x88  , 8,0x5f),new Node(0x8   , 8,0x76),new Node(0xf0  , 8,0x78),new Node(0x70  , 8,0x79),new Node(0x1b0 , 9,0x2b),new Node(0xb0  , 9,0x3e),new Node(0x130 , 9,0x4b),
            new Node(0x30  , 9,0x56),new Node(0x1d0 , 9,0x58),new Node(0xd0  , 9,0x59),new Node(0x150 , 9,0x5d),new Node(0x250 ,10,0x21),new Node(0x50  ,10,0x24),new Node(0x390 ,10,0x26),new Node(0x190 ,10,0x71),
            new Node(0x290 ,10,0x7a),new Node(0x490 ,11,0x00),new Node(0x90  ,11,0x3c),new Node(0x710 ,11,0x3f),new Node(0x310 ,11,0x4a),new Node(0x510 ,11,0x51),new Node(0x110 ,11,0x5a),new Node(0x610 ,11,0x5c),
            new Node(0x210 ,11,0x6a),new Node(0x410 ,11,0x7b),new Node(0x10  ,11,0x7c),new Node(0xfe0 ,12,0x01),new Node(0x7e0 ,12,0x02),new Node(0xbe0 ,12,0x03),new Node(0x3e0 ,12,0x04),new Node(0xde0 ,12,0x05),
            new Node(0x5e0 ,12,0x06),new Node(0x9e0 ,12,0x07),new Node(0x1e0 ,12,0x08),new Node(0xee0 ,12,0x0b),new Node(0x6e0 ,12,0x0c),new Node(0xae0 ,12,0x0e),new Node(0x2e0 ,12,0x0f),new Node(0xce0 ,12,0x10),
            new Node(0x4e0 ,12,0x11),new Node(0x8e0 ,12,0x12),new Node(0xe0  ,12,0x13),new Node(0xf60 ,12,0x14),new Node(0x760 ,12,0x15),new Node(0xb60 ,12,0x16),new Node(0x360 ,12,0x17),new Node(0xd60 ,12,0x18),
            new Node(0x560 ,12,0x19),new Node(0x960 ,12,0x1b),new Node(0x160 ,12,0x1c),new Node(0xe60 ,12,0x1d),new Node(0x660 ,12,0x1e),new Node(0xa60 ,12,0x1f),new Node(0x260 ,12,0x23),new Node(0xc60 ,12,0x25),
            new Node(0x460 ,12,0x3b),new Node(0x860 ,12,0x40),new Node(0x60  ,12,0x5e),new Node(0xfa0 ,12,0x60),new Node(0x7a0 ,12,0x7d),new Node(0xba0 ,12,0x7e),new Node(0x3a0 ,12,0x7f),new Node(0xda0 ,12,0xb0),
            new Node(0x5a0 ,12,0xb1),new Node(0x9a0 ,12,0xb2),new Node(0x1a0 ,12,0xb3),new Node(0xea0 ,12,0xb4),new Node(0x6a0 ,12,0xb5),new Node(0xaa0 ,12,0xb6),new Node(0x2a0 ,12,0xb7),new Node(0xca0 ,12,0xb8),
            new Node(0x4a0 ,12,0xb9),new Node(0x8a0 ,12,0xba),new Node(0xa0  ,12,0xbb),new Node(0xf20 ,12,0xbc),new Node(0x720 ,12,0xbd),new Node(0xb20 ,12,0xbe),new Node(0x320 ,12,0xbf),new Node(0xd20 ,12,0xc0),
            new Node(0x520 ,12,0xc1),new Node(0x920 ,12,0xc2),new Node(0x120 ,12,0xc3),new Node(0xe20 ,12,0xc4),new Node(0x620 ,12,0xc5),new Node(0xa20 ,12,0xc6),new Node(0x220 ,12,0xc7),new Node(0xc20 ,12,0xc8),
            new Node(0x420 ,12,0xc9),new Node(0x820 ,12,0xca),new Node(0x20  ,12,0xcb),new Node(0xfc0 ,12,0xcc),new Node(0x7c0 ,12,0xcd),new Node(0xbc0 ,12,0xce),new Node(0x3c0 ,12,0xcf),new Node(0xdc0 ,12,0xd0),
            new Node(0x5c0 ,12,0xd1),new Node(0x9c0 ,12,0xd2),new Node(0x1c0 ,12,0xd3),new Node(0xec0 ,12,0xd4),new Node(0x6c0 ,12,0xd5),new Node(0xac0 ,12,0xd6),new Node(0x2c0 ,12,0xd7),new Node(0xcc0 ,12,0xd8),
            new Node(0x4c0 ,12,0xd9),new Node(0x8c0 ,12,0xda),new Node(0xc0  ,12,0xdb),new Node(0xf40 ,12,0xdc),new Node(0x740 ,12,0xdd),new Node(0xb40 ,12,0xde),new Node(0x340 ,12,0xdf),new Node(0xd40 ,12,0xe1),
            new Node(0x540 ,12,0xe5),new Node(0x940 ,12,0xe9),new Node(0x140 ,12,0xee),new Node(0xe40 ,12,0xf2),new Node(0x640 ,12,0xf3),new Node(0xa40 ,12,0xf4),new Node(0x1240,13,0x1a),new Node(0x240 ,13,0x80),
            new Node(0x1c40,13,0x81),new Node(0xc40 ,13,0x82),new Node(0x1440,13,0x83),new Node(0x440 ,13,0x84),new Node(0x1840,13,0x85),new Node(0x840 ,13,0x86),new Node(0x1040,13,0x87),new Node(0x40  ,13,0x88),
            new Node(0x1f80,13,0x89),new Node(0xf80 ,13,0x8a),new Node(0x1780,13,0x8b),new Node(0x780 ,13,0x8c),new Node(0x1b80,13,0x8d),new Node(0xb80 ,13,0x8e),new Node(0x1380,13,0x8f),new Node(0x380 ,13,0x90),
            new Node(0x1d80,13,0x91),new Node(0xd80 ,13,0x92),new Node(0x1580,13,0x93),new Node(0x580 ,13,0x94),new Node(0x1980,13,0x95),new Node(0x980 ,13,0x96),new Node(0x1180,13,0x97),new Node(0x180 ,13,0x98),
            new Node(0x1e80,13,0x99),new Node(0xe80 ,13,0x9a),new Node(0x1680,13,0x9b),new Node(0x680 ,13,0x9c),new Node(0x1a80,13,0x9d),new Node(0xa80 ,13,0x9e),new Node(0x1280,13,0x9f),new Node(0x280 ,13,0xa0),
            new Node(0x1c80,13,0xa1),new Node(0xc80 ,13,0xa2),new Node(0x1480,13,0xa3),new Node(0x480 ,13,0xa4),new Node(0x1880,13,0xa5),new Node(0x880 ,13,0xa6),new Node(0x1080,13,0xa7),new Node(0x80  ,13,0xa8),
            new Node(0x1f00,13,0xa9),new Node(0xf00 ,13,0xaa),new Node(0x1700,13,0xab),new Node(0x700 ,13,0xac),new Node(0x1b00,13,0xad),new Node(0xb00 ,13,0xae),new Node(0x1300,13,0xaf),new Node(0x300 ,13,0xe0),
            new Node(0x1d00,13,0xe2),new Node(0xd00 ,13,0xe3),new Node(0x1500,13,0xe4),new Node(0x500 ,13,0xe6),new Node(0x1900,13,0xe7),new Node(0x900 ,13,0xe8),new Node(0x1100,13,0xea),new Node(0x100 ,13,0xeb),
            new Node(0x1e00,13,0xec),new Node(0xe00 ,13,0xed),new Node(0x1600,13,0xef),new Node(0x600 ,13,0xf0),new Node(0x1a00,13,0xf1),new Node(0xa00 ,13,0xf5),new Node(0x1200,13,0xf6),new Node(0x200 ,13,0xf7),
            new Node(0x1c00,13,0xf8),new Node(0xc00 ,13,0xf9),new Node(0x1400,13,0xfa),new Node(0x400 ,13,0xfb),new Node(0x1800,13,0xfc),new Node(0x800 ,13,0xfd),new Node(0x1000,13,0xfe),new Node(0x0   ,13,0xff),
            new Node(0,255,0) // ursprünglich (0,-1,0)
		};

        private Node[] Lengths = new Node[]
        {
            new Node(0x3   , 2,(0<<12)+  3),new Node(0x5   , 3,(0<<12)+  2),new Node(0x1   , 3,(0<<12)+  4),new Node(0x6   , 3,(0<<12)+  5),
            new Node(0xa   , 4,(0<<12)+  6),new Node(0x2   , 4,(0<<12)+  7),new Node(0xc   , 4,(0<<12)+  8),new Node(0x14  , 5,(0<<12)+  9),
            new Node(0x4   , 5,(1<<12)+ 10),new Node(0x18  , 5,(2<<12)+ 12),new Node(0x8   , 5,(3<<12)+ 16),new Node(0x30  , 6,(4<<12)+ 24),
            new Node(0x10  , 6,(5<<12)+ 40),new Node(0x20  , 6,(6<<12)+ 72),new Node(0x40  , 7,(7<<12)+136),new Node(0x0   , 7,(8<<12)+264),
            new Node(0,255,0) // ursprünglich (0,-1,0)
		};

        private Node[] Distances = new Node[]
        {
            new Node(0x3   , 2,0x00),new Node(0xd   , 4,0x01),new Node(0x5   , 4,0x02),new Node(0x19  , 5,0x03),new Node(0x9   , 5,0x04),new Node(0x11  , 5,0x05),new Node(0x1   , 5,0x06),new Node(0x3e  , 6,0x07),
            new Node(0x1e  , 6,0x08),new Node(0x2e  , 6,0x09),new Node(0xe   , 6,0x0a),new Node(0x36  , 6,0x0b),new Node(0x16  , 6,0x0c),new Node(0x26  , 6,0x0d),new Node(0x6   , 6,0x0e),new Node(0x3a  , 6,0x0f),
            new Node(0x1a  , 6,0x10),new Node(0x2a  , 6,0x11),new Node(0xa   , 6,0x12),new Node(0x32  , 6,0x13),new Node(0x12  , 6,0x14),new Node(0x22  , 6,0x15),new Node(0x42  , 7,0x16),new Node(0x2   , 7,0x17),
            new Node(0x7c  , 7,0x18),new Node(0x3c  , 7,0x19),new Node(0x5c  , 7,0x1a),new Node(0x1c  , 7,0x1b),new Node(0x6c  , 7,0x1c),new Node(0x2c  , 7,0x1d),new Node(0x4c  , 7,0x1e),new Node(0xc   , 7,0x1f),
            new Node(0x74  , 7,0x20),new Node(0x34  , 7,0x21),new Node(0x54  , 7,0x22),new Node(0x14  , 7,0x23),new Node(0x64  , 7,0x24),new Node(0x24  , 7,0x25),new Node(0x44  , 7,0x26),new Node(0x4   , 7,0x27),
            new Node(0x78  , 7,0x28),new Node(0x38  , 7,0x29),new Node(0x58  , 7,0x2a),new Node(0x18  , 7,0x2b),new Node(0x68  , 7,0x2c),new Node(0x28  , 7,0x2d),new Node(0x48  , 7,0x2e),new Node(0x8   , 7,0x2f),
            new Node(0xf0  , 8,0x30),new Node(0x70  , 8,0x31),new Node(0xb0  , 8,0x32),new Node(0x30  , 8,0x33),new Node(0xd0  , 8,0x34),new Node(0x50  , 8,0x35),new Node(0x90  , 8,0x36),new Node(0x10  , 8,0x37),
            new Node(0xe0  , 8,0x38),new Node(0x60  , 8,0x39),new Node(0xa0  , 8,0x3a),new Node(0x20  , 8,0x3b),new Node(0xc0  , 8,0x3c),new Node(0x40  , 8,0x3d),new Node(0x80  , 8,0x3e),new Node(0x0   , 8,0x3f),
            new Node(0,255,0) // ursprünglich (0,-1,0)
		};

        #endregion Nodes (Literals, Lengths, Distances

        private Int32 pksrcidx = 0;
        private Int32 pksrcbuf = 0;
        private Int32 pkdstbuf = 0;

        private Int32 GetBits(Int32 bufPos, Int32 numbits, ref byte[] value, ref Int32 index)
        {
            Int32 bits = 0;
            Int32 bytpos = (Int32)(index / 8);
            Int32 bitpos = (Int32)(index % 8);

            Int32 bb = (Int32)(value[bufPos + bytpos] << 24 + value[bufPos + bytpos + 1] << 16 + value[bufPos + bytpos + 2] << 8 + value[bufPos + bytpos + 3]);

            bb >>= bitpos;

            if (numbits != 32)
            {
                bb &= (Int32)((1 << numbits) - 1);
            }

            index += (Int32)numbits;

            return bits;
        }

        private Int32 ReadBits(Int32 bufPos, Int32 numbits, ref byte[] value, ref Int32 index)
        {
            Int32 temp = index;

            return GetBits(bufPos, numbits, ref value, ref temp);
        }

        private Int32 GetNode(Node[] node, ref byte[] value, Int32 maxbits)
        {
            //Int32 result = -1;
            Int32 allBits = ReadBits(pksrcbuf, maxbits, ref value, ref pksrcidx);

            while (true /*TODO*/ )
            {
            }
        }

        public override void Init()
        {
        }

        public override ECompressionValue Unpack(ref byte[] inbuf, ref byte[] outbuf)
        {
            Int32 inBufPosition = 0;
            Int32 outBufPosition = 0;

            Int32 flexLiteral = inbuf[inBufPosition++];
            Int32 windowBits = inbuf[inBufPosition++];

            pksrcbuf = inBufPosition;
            pkdstbuf = outBufPosition;

            do
            {
                if (GetBits(pksrcbuf, 1, ref inbuf, ref inBufPosition) > 0)
                {
                    if (flexLiteral == 0)
                    {
                        outbuf[outBufPosition++] = (byte)GetBits(pksrcbuf, 8, ref inbuf, ref inBufPosition);
                    }
                    else
                    {
                        Int32 lit = GetNode(Literals, ref inbuf, 13);

                        if (lit == 1)
                        {
                            break;
                        }
                        else
                        {
                            outbuf[outBufPosition++] = (byte)lit;
                        }
                    }
                }
                else
                {
                    Int32 len = GetNode(Literals, ref inbuf, 7);

                    if (len == 1)
                    {
                        break;
                    }
                    else
                    {
                        len = (len & 0xFFF) + GetBits(pksrcbuf, ((len >> 12) & 0xF), ref inbuf, ref inBufPosition);

                        if (len == 519)
                        {
                            break;
                        }

                        Int32 ofs = GetNode(Distances, ref inbuf, 8);

                        if (ofs == 1)
                        {
                            break;
                        }

                        if (len == 2)
                        {
                            ofs = (ofs << 2) | GetBits(pksrcbuf, 2, ref inbuf, ref inBufPosition);
                        }
                        else
                        {
                            ofs = (ofs << windowBits) | GetBits(pksrcbuf, windowBits, ref inbuf, ref inBufPosition);
                        }

                        Int32 dic = outBufPosition - ofs - 1;

                        if (dic < 0)
                        {
                            break;
                        }

                        while (len-- > 0)
                        {
                            outbuf[outBufPosition++] = outbuf[dic++];
                        };
                    }
                }
            } while (true);

            return ECompressionValue.Ok; //outBufPosition;
        }

        #region ISciCompression Member
        public ECompressionType Type
        {
            get { return ECompressionType.DclExplode; }
        }

        #endregion ISciCompression Member
    }
}
