using System;
using System.Collections.Generic;
using System.Text;

using SCI.Interface;

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
		STACpack,		// RFC 1974
	}

	public class CGame
	{
		public EGameType			Type;
		//public UInt16				Identifier;		// Typ des MapFiles
		//public UInt32				Address;
		//public Int32				MapFileEntries;
		public List<CResource>		ResourceList = new List<CResource>();
		//public List<ISciResource>	ResourceList;
		public ISciType				GameData;
	}

	//public class CResourceIndex
	//{
	//	public CResourceInfo[]		ResourceInfo		= new CResourceInfo[Common.TOTAL_RES_TYPES2];	// TOTAL_RES_TYPES2 = 11
	//	public int[]				TotalResources		= new int[Common.TOTAL_RES_TYPES2];				// TOTAL_RES_TYPES22 = 11
	//	public CResourceInfo[]		SelResItems			= new CResourceInfo[Common.TOTAL_RES_TYPES2];	// TOTAL_RES_TYPES22 = 11
	//	public int					SelRes;
	//	public int					TotalAllocs;
	//	public int					AllocPtr;
	//	public int 					MaxPack;
	//	public CResourceInfo[]		AllocBufs			= new CResourceInfo[Common.rsMAX_ALLOCS];	// rsMAX_ALLOCS = 64
	//	public CResourceInfo		LastAlloc;
	//} 

	//public class CResourceInfo 
	//{
	//	public UInt16		Number;
	//	public byte			Type;
	//	public UInt32		Offset;
	//	public byte			Pack;

	//	public UInt16		EncodingType;
	//	public UInt16		Size;
	//	public UInt16		EncodedSize;
	//	//public CResourceInfo 	prev;
	//	//public CResourceInfo 	next;
	//}


	//public static class ResourceMapFileString
	//{
	//	private const string[] ResourceTypes = {
	//			// 0     1       2      3      4      5      6       7     8       9        10       11        12      13     14     15      
	//			"view","pic","script","<3>","sound","etc","vocab","font","cursor","patch","bitmap","palette","audio","audio","sync","message",
	//			//16     17     18    19         20      21             22     23    24      25     26    27   28  29 30 31   
	//			"map","<17>","chunk","audio36","sync36","translation","robot","vmd","duck","clut","targa","zzz","","","","",
	//			// 0x20
	//			"","","","","","","","","","","","","","","","",
	//			"","","","","","","","","","","","","","","","",
	//			"","","","","","","","","","","","","","","","",
	//			"","","","","","","","","","","","","","","","",
	//			"","","","","","","","","","","","","","","","",
	//			"","","","","","","","","","","","","","","","",
	//			//0x80
	//			"view","pic","script","text","sound","memory","vocab","font","cursor","patch","bitmap","palette","wave","audio","sync","message",
	//			"map","heap","audio36","sync36","xlate"};
	//}
	//public class CMapResourceIndex
	//{
	//	/*SCI 1*/
	//	//byte Type; // Type of the resourcefile
	//	public EResourceType ResourceType; // Type of the resourcefile
	//	public UInt16 Offset; /// Offset in lookup table
	//}

	public class CResource
	{
		/*SCI 1*/
		private EResourceType ResourceType2;
		private UInt16 ResourceNumber; /// the number of the resource (0...999)
		
		public byte ResourceType;
		public byte FileNumber; /// in this resource file are the data (RESOURCE.<nr>)
		public UInt32 Offset; /// absolute offset withini the resource file
		public ISciResource ResourceData;

		#region ISciResource Member

		public EResourceType Type
		{
			get { return ResourceType2; }
			set { ResourceType2 = value;  }
		}

		public UInt16 Number
		{
			get
			{
				return ResourceNumber;
			}
			set
			{
				ResourceNumber = value;
			}
		}

		public ECompressionType CompressionType
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int CompressedSize
		{
			get { throw new NotImplementedException(); }
		}

		public int UncompressedSize
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

}
