namespace SCI
{
    public class Game
    {
		public EGameType Type = EGameType.None;
		//public System.Collections.Generic.List<CResource> ResourceList = new System.Collections.Generic.List<CResource>();
		public SciBase GameData;

		/// <summary>
		/// load a compiled game and not the sources and the project file
		/// </summary>
		public void Expand(string path)
        {
			SciBase game = null;

            if (System.IO.File.Exists(System.IO.Path.Combine(path, "RESOURCE.MAP")))
            {
                System.IO.FileStream stream = System.IO.File.Open(System.IO.Path.Combine(path, "RESOURCE.MAP"), System.IO.FileMode.Open);
                int value = stream.ReadByte();

                stream.Position = stream.Length - 6;

                byte[] ba = new byte[6];
                stream.Read(ba, 0, 6);
                stream.Close();

                if ((byte)value == 0x80)
                {
                    /* SCI1 */
                    game = new SCI1();
                    if (game.Expand(path))
                    {
                        Type = EGameType.SCI1;
                        GameData = (SciBase)game;
                    }
                }
				else if  (Common.ArraysEqual(ba, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }))
                //else if ((ba[0] == 0xFF) && (ba[1] == 0xFF) && (ba[2] == 0xFF) && (ba[3] == 0xFF) && (ba[4] == 0xFF) && (ba[5] == 0xFF))
                {
                    /* SCI0 */
                    game = new SCI0();
                    if (game.Expand(path))
                    {
                        Type = EGameType.SCI0;
                        GameData = (SciBase)game;
                    }
                }
            }
            else if (System.IO.File.Exists(System.IO.Path.Combine(path, "RESMAP.000")))
            {
                game = new SCI3();
                if (game.Expand(path))
                {
                    Type = EGameType.SCI3;
                    GameData = (SciBase)game;
                }
            }

			if (game != null)
			{
				foreach(ResourceBase item in GameData.ResourceList)
				{
					item.Save(path);	
				}
			}
        }
    }
}
