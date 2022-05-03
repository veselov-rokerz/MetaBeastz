using Assets.Scripts.GSSocket.DTO;
using System;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGCardDTO
    {
        public int CardHp;
        public int UniqueCardID;
        public CardDTO MetaData;

        public static BGCardDTO ToEmpty(CardController card) => new BGCardDTO()
        {
            UniqueCardID = card.CardData.UniqueCardID,
            CardHp = card.CardData.CardHp,
            MetaData = new CardDTO()
        };

        public static BGCardDTO ToEmpty(BGCardDTO card) => new BGCardDTO()
        {
            UniqueCardID = card.UniqueCardID,
            CardHp = card.CardHp,
            MetaData = new CardDTO()
        };
    }
}
