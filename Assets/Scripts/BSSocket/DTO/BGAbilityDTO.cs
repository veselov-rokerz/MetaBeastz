using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.GSSocket.DTO;
using System;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGAbilityRequestDTO
    {
        /// <summary>
        /// Unique generated for the attack.
        /// </summary>
        public string UniqueID;

        /// <summary>
        /// Used card id.
        /// </summary>
        public int CardID;

        /// <summary>
        /// When required to select any monster in game we will store it in this.
        /// </summary>
        public int TCardId;
       
        /// <summary>
        /// When secondary selection exists we will store its id.
        /// </summary>
        public int SCardId;

        /// <summary>
        /// When any value required we will use this.
        /// </summary>
        public int TValue;

        /// <summary>
        /// Player color.
        /// </summary>
        public BattleGamePlayers Player;

        /// <summary>
        /// if any energy selected we will store it.
        /// </summary>
        public EnergyTypes Energy;
    }

    [Serializable]
    public class BGAbilityResponseDTO
    {
        /// <summary>
        /// When damage failed true.
        /// </summary>
        public bool IsFailed;

        /// <summary>
        /// We store ability request in it.
        /// </summary>
        public BGAbilityRequestDTO RequestData;

        /// <summary>
        /// Current action number
        /// </summary>
        public int ActionNumber;

        /// <summary>
        /// When any energy played its id here.
        /// </summary>
        public int DECardID;
    }
}
