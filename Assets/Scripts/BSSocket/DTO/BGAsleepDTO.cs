using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGAsleepDTO
    {
        public BGSpecialConditions SC;
        public Flips Coin;
        public BattleGamePlayers PColor;
    }
}
