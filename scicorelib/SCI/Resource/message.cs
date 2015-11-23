namespace SCI.Resources
{
    public class Message: ResourceBase
    {
		public override EResourceType ResourceType
		{
			get { return EResourceType.Message; }
		}

		public Message(EGameType gametype):base(gametype) { }

        public uint Count;

        public struct MessageInfo
        {
            public ushort Offset;
            public string Text;
        }

        public MessageInfo[] Messages;

        public void Decode(string filename)
        {
            System.IO.FileStream filestream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(filestream);

            Decode(binaryReader);
        }

		public void Decode(System.IO.Stream stream)
		{
			System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(stream);
			Decode(binaryReader);
		}

        public void Decode(System.IO.BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(8);

            Count = binaryReader.ReadUInt16();

            Messages = new MessageInfo[Count];

            for (int counter = 0; counter < Count; counter++)
            {
                binaryReader.ReadBytes(5);
                Messages[counter].Offset = binaryReader.ReadUInt16();
                binaryReader.ReadBytes(4);
            }

            for (int counter = 0; counter < Count; counter++)
            {
                binaryReader.BaseStream.Position = Messages[counter].Offset;
                //Message[counter].Text = binaryReader.ReadString();

                byte[] bytearray;
                bytearray = binaryReader.ReadBytes((int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position));
                Messages[counter].Text = System.Text.Encoding.UTF7.GetString(bytearray).Split(new char[] { '\0' })[0];
            }
        }
    }
}
