using System;
using System.Collections.Generic;

namespace Assets.Scripts.GSSocket.DTO
{
    [Serializable]
    public enum CardTypes
    {
        None = 0,
        Pokemon = 1,
        Energy = 2,
        Trainer = 3
    }

    [Serializable]
    public class CardDTO
    {
        public int CardId;
        public CardTypes CardTypeId;
        public EnergyTypes EnergyTypeId;
        public int CardLevel;
        public int CardHp;
        public EnergyTypes WeaknessEnergyTypeId;
        public int WeaknessMultiplier;
        public EnergyTypes ResistanceEnergyTypeId;
        public int ResistanceBonus;
        public int EvolutedCardId;
        public int CardValue;
        public List<AttackDTO> CardAttacks;
        public List<CardRetreatCostDTO> CardRetreatCosts;


        /// <summary>
        /// Is this card is a basic card?
        /// </summary>
        public bool IsBasic => EvolutedCardId == 0;

        /// <summary>
        /// Is this card evolution card.
        /// </summary>
        public bool IsEvolutionCard => EvolutedCardId > 0;

        public AttackDTO GetAttackByID(int attackId) => this.CardAttacks.Find(x => x.AttackId == attackId);
    }

    [Serializable]
    public class CardAttackDTO
    {
        public int CardAttackId;
        public int CardId;
        public int AttackId;
    }

    [Serializable]
    public class CardRetreatCostDTO
    {
        public int CardRetreatCostId;
        public int CardId;
        public EnergyTypes RetreatEnergyTypeId;
        public int Quantity;
    }

    public class CardDataRequestDTO
    {
        public int[] CardIds { get; set; }
    }
}
