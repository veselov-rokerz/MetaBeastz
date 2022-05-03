using System;

namespace Assets.Scripts.GSSocket.DTO
{
    [Serializable]
    public class UserDeckDTO
    {
        public long UserDeckId;
        public long UserId;
        public string DeckName; 
    }
}
