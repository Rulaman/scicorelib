using System.Collections.Generic;

namespace SCI
{
    public enum EGameType
    {
        None,
        SCI0,
        SCI01,
        SCI1,
        SCI11,
        SCI2,
        SCI3
    }

    public enum EResourceType
    {
        View = 0,
        Picture = 1,
        Script = 2,
        Sound = 4,
        Etc = 5,
        Vocab = 6,
        Font = 7,
        Cursor = 8,
        Patch = 9,
        Bitmap = 10,
        Palette = 11,
        Audio = 12,
        Audio2 = 13,
        Sync = 14,
        Message = 15,
        Map = 16,
        Chunk = 18,
        Audio36 = 39,
        Sync36 = 20,
        Translation = 21,
        Robot = 22,
        Vmd = 23,
        Duck = 24,
        Clut = 25,
        Targa = 26,
        ZZZ = 27,

        View8x = 0x80,
        Picture8x = 0x81,
        Script8x = 0x82,
        Text8x = 0x83,
        Sound8x = 0x84,
        Memory8x = 0x85,
        Vocab8x = 0x86,
        Font8x = 0x87,
        Cursor8x = 0x88,
        Patch8x = 0x89,
        Bitmap8x = 0x8a,
        Palette8x = 0x8b,
        Wave8x = 0x8c,
        Audio8x = 0x8d,
        Sync8x = 0x8e,
        Message8x = 0x8f,
        Map8x = 0x90,
        Heap8x = 0x91,
        Audio368x = 0x92,
        Sync368x = 0x93,
        XLate8x = 0x94,

		None = 0xFE, // damit man weiß, dass diese Resource ungültig ist
        EndOfIndex = 0xFF
    }

    public enum ECompressionType
    {
        Invalid,
        None,
        Lzw,
        Huffman,
        Comp3,
        Unknown0,
        Unknown1,
        DclExplode,
        STACpack,       // RFC 1974
    }

    public class CGame
    {
        public EGameType Type;
        public List<CResource> ResourceList = new List<CResource>();
        public SciBase GameData;
    }
}
