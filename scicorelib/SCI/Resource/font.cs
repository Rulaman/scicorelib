namespace SCI.Resources
{
	public class Font: ResourceBase
	{
		public override EResourceType ResourceType
		{
			get { return EResourceType.Font; }
		}

		public Font(EGameType gametype) : base(gametype)
		{ }

		public FontCharacter[] Character;

		public void Decode(string filename)
		{
			System.IO.FileStream filestream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
			System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(filestream);

			Decode(binaryReader);

			filestream.Close();
		}

		public void Decode(System.IO.Stream stream)
		{
			System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(stream);

			Decode(binaryReader);
		}

		public void Decode(System.IO.BinaryReader binaryReader)
		{
			FontHeader fh = new FontHeader();

			fh.Type = binaryReader.ReadUInt16();
			fh.Dummy = binaryReader.ReadUInt16();
			fh.NumberOfCharacters = binaryReader.ReadUInt16();
			fh.LineDistance = binaryReader.ReadUInt16();

			System.Collections.Generic.List<ushort> posList = new System.Collections.Generic.List<ushort>();

			for ( int i = 0; i < fh.NumberOfCharacters; i++ )
			{
				posList.Add(binaryReader.ReadUInt16());
			}

			/* now read font data */

			ushort[] positionArray = new ushort[fh.NumberOfCharacters];
			Character = new FontCharacter[fh.NumberOfCharacters];

			for ( int counter = 0; counter < fh.NumberOfCharacters; counter++ )
			{
				positionArray[counter] = (ushort)(binaryReader.ReadUInt16() + 2);
			}

			for ( int counter = 0; counter < fh.NumberOfCharacters; counter++ )
			{
				binaryReader.BaseStream.Position = positionArray[counter];

				Character[counter] = new FontCharacter();

				Character[counter].Index = counter;
				Character[counter].Width = binaryReader.ReadByte();
				Character[counter].Height = binaryReader.ReadByte();

				Character[counter].WidthOriginal = Character[counter].Width;
				Character[counter].HeightOriginal = Character[counter].Height;

				if ( counter == fh.NumberOfCharacters - 1 )
				{
					Character[fh.NumberOfCharacters - 1].ByteLines = binaryReader.ReadBytes((ushort)(binaryReader.BaseStream.Length - (positionArray[counter] + 2)));
					Character[fh.NumberOfCharacters - 1].ByteLinesOriginal = new byte[Character[fh.NumberOfCharacters - 1].ByteLines.Length];
					System.Array.Copy(Character[fh.NumberOfCharacters - 1].ByteLines, Character[fh.NumberOfCharacters - 1].ByteLinesOriginal, Character[fh.NumberOfCharacters - 1].ByteLines.Length);
				}
				else
				{
					Character[counter].ByteLines = binaryReader.ReadBytes(positionArray[counter + 1] - (positionArray[counter] + 2));
					Character[counter].ByteLinesOriginal = new byte[Character[counter].ByteLines.Length];
					System.Array.Copy(Character[counter].ByteLines, Character[counter].ByteLinesOriginal, Character[counter].ByteLines.Length);
				}
			}
		}

		public override void Decode()
		{
			if ( Path != null )
			{
				Decode(Path);
			}
			else if ( Data != null )
			{
				Decode(new System.IO.MemoryStream(Data));
			}
		}
	}

	public class FontHeader
	{
		public ushort Type;
		public ushort Dummy;
		public ushort NumberOfCharacters;
		public ushort LineDistance;
	}

	public class FontCharacter
	{
		public System.UInt16 Width;
		public System.UInt16 Height;
		public byte[] ByteLines;

		public System.UInt16 WidthOriginal;
		public System.UInt16 HeightOriginal;
		public byte[] ByteLinesOriginal;

		public System.Drawing.Image UnscaledImage;
		public System.Int32 Index;

		public System.Collections.Generic.List<byte[]> UndoRedoList = new System.Collections.Generic.List<byte[]>();
		public System.Int32 UndoRedoPosition = 0;

		public void UndoRedoListAdd(byte[] bytearray)
		{
			if ( UndoRedoList.Count == 0 )
			{
				UndoRedoList.Add(bytearray);
			}
			else if ( ArraysEqual(UndoRedoList[UndoRedoPosition], bytearray) )
			{
			}
			else
			{
				UndoRedoList.Add(bytearray);
				UndoRedoPosition++;
			}
		}

		public void UndoRedoListClear()
		{
			UndoRedoList.Clear();
		}

		public void UndoRedoListTidyUp()
		{
			while ( UndoRedoList.Count - 1 > UndoRedoPosition )
			{
				UndoRedoList.RemoveAt(UndoRedoList.Count - 1);
			}
		}

		public bool RedoPossible
		{
			get { return (UndoRedoList.Count - 1 > UndoRedoPosition); }
		}

		public bool UndoPossible
		{
			get { return (UndoRedoPosition > 0); }
		}

		public void Undo()
		{
			if ( UndoRedoPosition > 0 )
			{
				UndoRedoPosition--;
				ByteLines = UndoRedoList[UndoRedoPosition];
			}
		}

		public void Redo()
		{
			if ( UndoRedoPosition < (UndoRedoList.Count) )
			{
				UndoRedoPosition++;
				ByteLines = UndoRedoList[UndoRedoPosition];
			}
		}

		private static bool ArraysEqual(byte[] a1, byte[] a2)
		{
			if ( a1 == a2 )
				return true;

			if ( a1 == null || a2 == null )
				return false;

			if ( a1.Length != a2.Length )
				return false;

			for ( int i = 0; i < a1.Length; i++ )
			{
				if ( a1[i] != a2[i] )
					return false;
			}
			return true;
		}
	}
}
