using System;
using System.Collections.Generic;
using System.Text;

namespace SCI.GameVersion
{
	public enum GameType
	{
		SCI0,
		SCI1,
		SCI11,
	}

	public interface IGameType
	{
		GameType Type { get; }
	}
}
