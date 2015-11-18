namespace SCI.IO.Compression
{
    using System;

    public class LZW : SciCompressionBase, ISciCompression
    {
        public enum EOption
        {
            Lzw,
            Lzw1,
            Lzw1View,
            Lzw1Pic
        }

        private UInt16 _numbits;
        private UInt16 _curtoken;
        private UInt16 _endtoken;
        private UInt16 _dwWrote;
        private UInt16 _szUnpacked;
        private UInt16 _szPacked;
        private UInt16 _dwRead;

        private System.IO.BinaryReader br;
        private System.IO.BinaryWriter bw;

        private Int32 _dwBits;		///< bits buffer
		private Int32 _nBits;       ///< number of unread bits in _dwBits

        private EOption Option;

        public LZW(EOption option)
        {
            Option = option;
        }

        public override void Init()
        {
        }

        public override ECompressionValue Unpack(ref byte[] inbuf, ref byte[] outbuf)
        {
            ECompressionValue retval = ECompressionValue.Ok;
            _numbits = 9;
            _curtoken = 0x102;
            _endtoken = 0x1ff;

            switch (Option)
            {
                case EOption.Lzw:
                    retval = UnpackLZW(ref inbuf, ref outbuf);
                    break;
                case EOption.Lzw1:
                    break;
                case EOption.Lzw1Pic:
                    break;
                case EOption.Lzw1View:
                    break;
            };

            return retval;
        }

        private bool isFinished()
        {
            return (_dwWrote == _szUnpacked) && (_dwRead >= _szPacked);
        }

        private UInt32 getBitsLSB(int n)
        {
            // fetching more data to buffer if needed
            if (_nBits < n)
            {
                fetchBitsLSB();
            }
            UInt32 ret = (UInt32)(_dwBits & ~((~0) << n));
            _dwBits >>= n;
            _nBits -= n;
            return ret;
        }

        private void fetchBitsLSB()
        {
            while (_nBits <= 24)
            {
                if (br.BaseStream.Position < br.BaseStream.Length)
                {
                    _dwBits |= br.ReadByte() << _nBits;
                }
                else
                {
                    _dwBits <<= _nBits;
                }

                _nBits += 8;
                _dwRead++;
            }
        }

        private ECompressionValue UnpackLZW(ref byte[] inbuf, ref byte[] outbuf)
        {
            ECompressionValue retval = ECompressionValue.Ok;
            bool isEndReached = false;

            UInt32 token; // The last received value
            Int32 tokenlastlength = 0;
            UInt16[] tokenlist = new UInt16[4096];
            UInt16[] tokenlengthlist = new UInt16[4096];
            _szUnpacked = (UInt16)outbuf.Length;

            br = new System.IO.BinaryReader(new System.IO.MemoryStream(inbuf));
            bw = new System.IO.BinaryWriter(new System.IO.MemoryStream(outbuf));

            while (!isFinished() || !isEndReached)
            {
                token = getBitsLSB(_numbits);

                switch (token)
                {
                    case 0x101: /* 257; end of stream  */
                        isEndReached = true;
                        break;
                    case 0x100: /* 256; reset command  */
                        _numbits = 9;
                        _curtoken = 0x102;
                        _endtoken = 0x1ff;
                        break;
                    default:
                        if (token > 0xFF) /* 255 */
                        {
                            if (token >= _curtoken)
                            {
                                /* set error */
                                //warning("unpackLZW: Bad token %x", token);
                                isEndReached = true;
                                retval = ECompressionValue.DecompressionError;
                            }
                            else
                            {
                                tokenlastlength = tokenlengthlist[token] + 1;

                                if (_dwWrote + tokenlastlength > _szUnpacked)
                                {
                                    // warning("unpackLZW: Trying to write beyond the end of array(len=%d, destctr=%d, tok_len=%d)",_szUnpacked, _dwWrote, tokenlastlength);

                                    for (int i = 0; _dwWrote < _szUnpacked; i++)
                                    {
                                        bw.Write((byte)outbuf[tokenlist[token] + i]);
                                        _dwWrote++;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < tokenlastlength; i++)
                                    {
                                        // If-Abfrage selbst eingefügt
                                        if (tokenlist[token] + i < outbuf.Length - 1)
                                        {
                                            bw.Write((byte)outbuf[tokenlist[token] + i]);
                                            _dwWrote++;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            tokenlastlength = 1;
                            if (_dwWrote >= _szUnpacked)
                            {
                                //	warning("unpackLZW: Try to write single byte beyond end of array");
                            }
                            else
                            {
                                bw.Write((byte)token);
                                _dwWrote++;
                            }
                        }

                        if (_curtoken > _endtoken && _numbits < 12)
                        {
                            _numbits++;
                            _endtoken = (UInt16)((_endtoken << 1) + 1);
                        }
                        if (_curtoken <= _endtoken)
                        {
                            tokenlist[_curtoken] = (UInt16)(_dwWrote - tokenlastlength);
                            tokenlengthlist[_curtoken] = (UInt16)tokenlastlength;
                            _curtoken++;
                        }

                        break;
                };
            };

            return retval;
        }

        #region ISciCompression Member
        public ECompressionType Type
        {
            get { return ECompressionType.Lzw; }
        }

        #endregion ISciCompression Member
    }
}
