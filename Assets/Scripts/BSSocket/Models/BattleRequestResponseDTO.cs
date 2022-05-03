using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.BSSocket.Models
{
    [Serializable]
    public class BattleRequestDataDTO
    {
        /// <summary>
        /// Unique action id for each requests.
        /// </summary>
        public string UAId;

        /// <summary>
        /// Unique game id.
        /// </summary>
        public string UGID;

        /// <summary>
        /// Unique generated user id.
        /// </summary>
        public string UUT;

        /// <summary>
        /// Battle action to do an action.
        /// </summary>
        public BattleGameActions BA;

        /// <summary>
        /// We store the request data.
        /// </summary>
        public string Data;
    }

    [Serializable]
    public class BattleResponseDataDTO
    {
        /// <summary>
        /// Battle action to do an action.
        /// </summary>
        public BattleGameActions BA;
        /// <summary>
        /// Data to send to players.
        /// </summary>
        public string Data;
    }
}
