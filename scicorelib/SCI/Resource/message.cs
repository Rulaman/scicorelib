using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;

namespace SCI.Resource
{
    public class SciMessage: CResource
    {
		public override EResourceType ResourceType
		{
			get { return EResourceType.Message; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EGameType GameType;

        public SciMessage(EGameType gametype)
        {
            GameType = gametype;
        }

        public UInt32 Count;

        public struct MessageInfo
        {
            public UInt16 Offset;
            public string Text;
        }

        public MessageInfo[] Message;

        public void Decode(string filename)
        {
            System.IO.FileStream filestream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
            System.IO.BinaryReader binaryReader = new BinaryReader(filestream);

            Decode(binaryReader);
        }

		public void Decode(Stream stream)
		{
			System.IO.BinaryReader binaryReader = new BinaryReader(stream);
			Decode(binaryReader);
		}

        public void Decode(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(8);

            Count = binaryReader.ReadUInt16();

            Message = new MessageInfo[Count];

            for (Int32 counter = 0; counter < Count; counter++)
            {
                binaryReader.ReadBytes(5);
                Message[counter].Offset = binaryReader.ReadUInt16();
                binaryReader.ReadBytes(4);
            }

            for (Int32 counter = 0; counter < Count; counter++)
            {
                binaryReader.BaseStream.Position = Message[counter].Offset;
                //Message[counter].Text = binaryReader.ReadString();

                byte[] bytearray;
                bytearray = binaryReader.ReadBytes((int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position));
                Message[counter].Text = System.Text.Encoding.UTF7.GetString(bytearray).Split(new char[] { '\0' })[0];
            }
        }
    }
}
