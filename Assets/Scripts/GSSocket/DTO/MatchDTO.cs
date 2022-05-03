using System;

namespace Assets.Scripts.GSSocket.DTO
{
    [Serializable]
    public class MatchDTO
    {
        public long SearcherUserID;
        public string SearcherUsername;
    }

    [Serializable]
    public class MatchmakingFoundDTO
    {
        public string UniqueGameID;
        public long P1UserID;
        public string P1UserName;
        public long P2UserID;
        public string P2UserName;
    }

}
