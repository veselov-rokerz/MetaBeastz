using System;
using System.Collections.Generic;

namespace Assets.Scripts.GSSocket.DTO
{
    [Serializable]
    public class AttackDTO
    {
        public int AttackId;
        public int AbilityValue;
        public int AttackDamage;
        public List<AttackCostDTO> AttackCosts;
    }

    [Serializable]
    public class AttackCostDTO
    {
        public int AttackId;
        public EnergyTypes EnergyTypeId;
        public int Quantity;
    }
}
