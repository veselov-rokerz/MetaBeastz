using System;

namespace Assets.Scripts.GSSocket.DTO
{
    [Serializable]
    public class BattleServerDataDTO
    {
        public string P1Name;
        public string P2Name;
        public string Ip;
        public string AccessToken;
        public string UniqueGameID;
    }

    [Serializable]
    public class MatchRequestDTO
    {
        public int DeckID;
    }
}
