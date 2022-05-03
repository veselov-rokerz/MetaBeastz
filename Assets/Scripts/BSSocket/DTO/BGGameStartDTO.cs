using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.GSSocket.DTO;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGGameStartDTO
    {
        /// <summary>
        /// Start Player
        /// </summary>
        public BattleGamePlayers SP;

        /// <summary>
        /// Player 1 name.
        /// </summary>
        public string P1Name;

        /// <summary>
        /// Player 2 name.
        /// </summary>
        public string P2Name;

        /// <summary>
        /// We set the player state.
        /// </summary>
        public BattleGamePlayers PColor;

        /// <summary>
        /// We store the deck of player.
        /// </summary>
        public List<CardDTO> Deck;

        public BGGameStartDTO()
        {
            this.Deck = new List<CardDTO>();
        }
    }
}
