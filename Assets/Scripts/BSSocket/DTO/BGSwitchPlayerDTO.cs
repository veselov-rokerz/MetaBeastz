using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGSwitchPlayerDTO
    {
        public BattleGamePlayers Player;
        public BGDrawCardFromDeckDTO DrawnCard;
        public BGAsleepDTO Asleep;
        public int Raund;
    }
}
