using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGEvolveMonsterDTO
    {
        /// <summary>
        /// Active or benched card.
        /// </summary>
        public int UCardID;

        /// <summary>
        /// Evolve card id.
        /// </summary>
        public int EvUCardID;
    }

    [Serializable]
    public class BGEvolveMonsterResponseDTO
    {
        /// <summary>
        /// Player information.
        /// </summary>
        public BattleGamePlayers Player;

        /// <summary>
        /// Active or benched card.
        /// </summary>
        public int UCardID;

        /// <summary>
        /// Evolve card id.
        /// </summary>
        public int EvUCardID;

        /// <summary>
        /// Evolution card data.
        /// </summary>
        public BGCardDTO EvCardData;
    }
}
