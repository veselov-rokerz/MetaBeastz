using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.GSSocket.DTO;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGMulliganDTO
    {
        /// <summary>
        /// Mulligan player.
        /// </summary>
        public BattleGamePlayers Player;

        /// <summary>
        /// Mulligan cards of opponent.
        /// </summary>
        public CardDTO[] MCards;

        /// <summary>
        /// Player new drawn cards.
        /// </summary>
        public BGCardDTO DCard;

        /// <summary>
        /// Opponent new hand.
        /// </summary>
        public List<BGCardDTO> NewHand;
    }
}
