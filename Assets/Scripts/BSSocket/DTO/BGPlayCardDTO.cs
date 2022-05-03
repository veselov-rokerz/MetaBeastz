using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGPlayCardRequestDTO
    {
        /// <summary>
        /// Unique card id.
        /// </summary>
        public int UCardId;
    }

    [Serializable]
    public class BGPlayCardResponseDTO
    {
        /// <summary>
        /// Action player.
        /// </summary>
        public BattleGamePlayers Player;

        /// <summary>
        /// Card informations.
        /// </summary>
        public BGCardDTO PlayedCard;
    }
}
