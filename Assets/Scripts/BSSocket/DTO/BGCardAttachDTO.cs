using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGCardAttachDTO
    {
        public int MCardID;
        public int ECardID;
    }

    [Serializable]
    public class BGCardAttachResponseDTO
    {
        public BattleGamePlayers Player;
        public int MCardID;
        public int ECardID;
        public BGCardDTO ECardData;
    }
}
