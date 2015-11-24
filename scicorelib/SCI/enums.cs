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
		[ResourceType("v56")]
        View = 0,
		[ResourceType("p56")]
		Picture = 1,
		[ResourceType("csc")]
		Script = 2,
		[ResourceType("snd")]
		Sound = 4,
		[ResourceType("etc")]
		Etc = 5,
		[ResourceType("voc")]
		Vocab = 6,
		[ResourceType("fnt")]
		Font = 7,
		[ResourceType("cur")]
		Cursor = 8,
		[ResourceType("pat")]
		Patch = 9,
		[ResourceType("bmp")]
		Bitmap = 10,
		[ResourceType("pal")]
		Palette = 11,
		[ResourceType("aud")]
		Audio = 12,
		[ResourceType("aud2")]
		Audio2 = 13,
		[ResourceType("syn")]
		Sync = 14,
		[ResourceType("msg")]
		Message = 15,
		[ResourceType("map")]
		Map = 16,
		[ResourceType("chk")]
		Chunk = 18,
		[ResourceType("aud36")]
		Audio36 = 39,
		[ResourceType("syn36")]
		Sync36 = 20,
		[ResourceType("trs")]
		Translation = 21,
		[ResourceType("rob")]
		Robot = 22,
		[ResourceType("vmd")]
		Vmd = 23,
		[ResourceType("dck")]
		Duck = 24,
		[ResourceType("clt")]
		Clut = 25,
		[ResourceType("tgs")]
		Targa = 26,
		[ResourceType("zzz")]
		ZZZ = 27,

		[ResourceType("v56")]
		View8x = 0x80,
		[ResourceType("p56")]
		Picture8x = 0x81,
		[ResourceType("scr")]
		Script8x = 0x82,
		[ResourceType("txt")]
		Text8x = 0x83,
		[ResourceType("snd")]
		Sound8x = 0x84,
		[ResourceType("mem")]
		Memory8x = 0x85,
		[ResourceType("voc")]
		Vocab8x = 0x86,
		[ResourceType("fnt")]
		Font8x = 0x87,
		[ResourceType("cur")]
		Cursor8x = 0x88,
		[ResourceType("pat")]
		Patch8x = 0x89,
		[ResourceType("bmp")]
		Bitmap8x = 0x8a,
		[ResourceType("pal")]
		Palette8x = 0x8b,
		[ResourceType("wav")]
		Wave8x = 0x8c,
		[ResourceType("aud")]
		Audio8x = 0x8d,
		[ResourceType("syn")]
		Sync8x = 0x8e,
		[ResourceType("msg")]
		Message8x = 0x8f,
		[ResourceType("map")]
		Map8x = 0x90,
		[ResourceType("heap")]
		Heap8x = 0x91,
		[ResourceType("aud")]
		Audio368x = 0x92,
		[ResourceType("syn")]
		Sync368x = 0x93,
		[ResourceType("xlt")]
		XLate8x = 0x94,

		[ResourceType("None")]
		None = 0xFE, // damit man weiß, dass diese Resource ungültig ist
		[ResourceType("eof")]
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
}
