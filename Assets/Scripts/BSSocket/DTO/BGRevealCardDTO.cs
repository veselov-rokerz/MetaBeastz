using Assets.Scripts.BSSocket.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGRevealCardDTO
    {
        public BattleGamePlayers Player;
        public List<BGCardDTO> Cards;
        public List<BGCardDTO> P1Prizes;
        public List<BGCardDTO> P2Prizes;
    }
}
