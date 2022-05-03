using Assets.Scripts.BSSocket.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGTrainerRequestDTO
    {
        /// <summary>
        /// Unique action id.
        /// </summary>
        public string UniqueID;

        /// <summary>
        /// Played card id.
        /// </summary>
        public int CardID;

        /// <summary>
        /// When an additional card selected we will store here.
        /// </summary>
        public int TCardID;

        /// <summary>
        /// When additional cards selected we will store here.
        /// </summary>
        public List<int> TCardIDs;

        /// <summary>
        /// When an additional card selected we will store here.
        /// </summary>
        public int STCardID;

        public BGTrainerRequestDTO()
        {
            TCardIDs = new List<int>();
        }
    }

    [Serializable]
    public class BGTrainerResponsePlayerDTO
    {
        /// <summary>
        /// Player who played card.
        /// </summary>
        public BattleGamePlayers Player;

        /// <summary>
        /// Player to send.
        /// </summary>
        public BattleGamePlayers NotiPlayer;

        /// <summary>
        /// Used card id.
        /// </summary>
        public int CardID;

        /// <summary>
        /// Execution number.
        /// </summary>
        public int ActionNumber;

        /// <summary>
        /// Unique action id.
        /// </summary>
        public string UniqueID;

        /// <summary>
        /// Played card information.
        /// </summary>
        public BGCardDTO PCard;

        /// <summary>
        /// When card drawn we will store the drawn cards.
        /// </summary>
        public List<BGCardDTO> DrawnCards;

        /// <summary>
        /// When any card selected we will store it.
        /// </summary>
        public int TCardID;

        /// <summary>
        /// if multiple card selected ids will be stored here.
        /// </summary>
        public int[] TCardIDs;

        /// <summary>
        ///  We store the deck.
        /// </summary>
        public BGCardDTO[] Deck;


        /// <summary>
        /// When this field is full, we will show the cards on screen.
        /// </summary>
        public BGCardDTO[] SCards;

        /// <summary>
        /// When an additional card selected we will store here.
        /// </summary>
        public int STCardID;
    }
}
