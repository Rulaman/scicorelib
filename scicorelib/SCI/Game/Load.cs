namespace SCI
{
	public class Game
	{
		public EGameType Type = EGameType.None;

		//public System.Collections.Generic.List<CResource> ResourceList = new System.Collections.Generic.List<CResource>();
		public SciBase GameData;

		public void ClearGame()
		{
			GameData = null;
		}

		public void Load(string path)
		{
			if ( GameData != null )
			{
				throw new System.Exception("Unload a previously loaded game and then start from a fresh one.");
			}
			else
			{
				string[] sa = System.IO.Directory.GetFiles(path, "Game-*", System.IO.SearchOption.TopDirectoryOnly);
				string name = "";
				SciBase game = null;

				if ( sa.Length == 1 )
				{
					name = System.IO.Path.GetFileName(sa[0]).Replace("Game-", "");

					switch ( name )
					{
					case "SCI0":
						game = new SCI0();

						if ( game.Load(path) )
						{
							Type = EGameType.SCI0;
							GameData = (SciBase)game;
						}
						break;
					case "SCI1":
						game = new SCI1();

						if ( game.Load(path) )
						{
							Type = EGameType.SCI1;
							GameData = (SciBase)game;
						}
						break;
					case "SCI3":
						game = new SCI3();

						if ( game.Load(path) )
						{
							Type = EGameType.SCI3;
							GameData = (SciBase)game;
						}
						break;
					}
				}
			}
		}

		/// <summary>
		/// load a compiled game and not the sources and the project file
		/// </summary>
		public void Expand(string path)
		{
			if ( GameData != null )
			{
				throw new System.Exception("Unload a previously loaded game and then start from a fresh one.");
			}
			else
			{
				SciBase game = null;

				if ( System.IO.File.Exists(System.IO.Path.Combine(path, "RESOURCE.MAP")) )
				{
					System.IO.FileStream stream = System.IO.File.Open(System.IO.Path.Combine(path, "RESOURCE.MAP"), System.IO.FileMode.Open);
					int value = stream.ReadByte();

					stream.Position = stream.Length - 6;

					byte[] ba = new byte[6];
					stream.Read(ba, 0, 6);
					stream.Close();

					if ( (byte)value == 0x80 )
					{
						/* SCI1 */
						game = new SCI1();
						if ( game.Expand(path) )
						{
							Type = EGameType.SCI1;
							GameData = (SciBase)game;
						}
					}
					else if ( Common.ArraysEqual(ba, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }) )
					//else if ((ba[0] == 0xFF) && (ba[1] == 0xFF) && (ba[2] == 0xFF) && (ba[3] == 0xFF) && (ba[4] == 0xFF) && (ba[5] == 0xFF))
					{
						/* SCI0 */
						game = new SCI0();
						if ( game.Expand(path) )
						{
							Type = EGameType.SCI0;
							GameData = (SciBase)game;
						}
					}
				}
				else if ( System.IO.File.Exists(System.IO.Path.Combine(path, "RESMAP.000")) )
				{
					game = new SCI3();
					if ( game.Expand(path) )
					{
						Type = EGameType.SCI3;
						GameData = (SciBase)game;
					}
				}

				if ( game != null )
				{
					foreach ( Resources.ResourceBase item in GameData.ResourceList )
					{
						item.Save(path);
					}

					System.IO.FileStream fs = System.IO.File.Open(path, System.IO.FileMode.OpenOrCreate);
					byte[] b = Common.StringToByteArray("Game-" + Type);
					fs.Write(b, 0, b.Length);
					fs.Close();
				}
			}
		}
	}
}
