using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGDrawCardFromDeckDTO
    {
        public BattleGamePlayers Player;
        public BGCardDTO CardData;
    }
}
