using Assets.Scripts.BSSocket.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGGameOverDTO
    {
        public List<BattleGamePlayers> Winners;
    }
}
