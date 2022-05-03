using Assets.Scripts.BSSocket.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGGameStartFlipDTO
    {
        public Flips Choose;
        public Flips Result;
        public BattleGamePlayers SP;
        public List<BGCardDTO> PCards;
        public List<BGCardDTO> OPCards;
    }
}
