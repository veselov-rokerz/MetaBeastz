using Assets.Scripts.BSSocket.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGRetreatACardDTO
    {
        /// <summary>
        /// Retreat card id.
        /// </summary>
        public int ReCardId;

        /// <summary>
        /// Target card id.
        /// </summary>
        public int TargetCardId;

        /// <summary>
        /// Detached card ids.
        /// </summary>
        public List<int> DtCards;
    }

    [Serializable]
    public class BGRetreatACardResponseDTO
    {
        /// <summary>
        /// Action player.
        /// </summary>
        public BattleGamePlayers Player;

        /// <summary>
        /// Retreat card id.
        /// </summary>
        public int ReCardId;

        /// <summary>
        /// Target card id.
        /// </summary>
        public int TargetCardId;

        /// <summary>
        /// Detached card idss.
        /// </summary>
        public List<int> DtCards;
    }
}
