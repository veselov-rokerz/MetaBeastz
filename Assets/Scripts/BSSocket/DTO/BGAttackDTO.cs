using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.GSSocket.DTO;
using System;

namespace Assets.Scripts.BSSocket.DTO
{
    [Serializable]
    public class BGAttackRequestDTO
    {
        /// <summary>
        /// Unique generated for the attack.
        /// </summary>
        public string UniqueID;

        /// <summary>
        /// Attacker attack id.
        /// </summary>
        public int AttackID;

        /// <summary>
        /// When required to select any monster in game we will store it in this.
        /// </summary>
        public int TargetCardId;

        /// <summary>
        /// When sometimes required detach we will use this.
        /// </summary>
        public int[] DCardIds;

        /// <summary>
        /// Sometimes can be select the attack. This is the attack id.
        /// </summary>
        public int TAttackID;

        /// <summary>
        /// When energy type is required to an action we will use.
        /// </summary>
        public EnergyTypes TEnergy;
    }

    [Serializable]
    public class BGAttackResponseDTO
    {
        /// <summary>
        /// When damage failed true.
        /// </summary>
        public bool IsFailed;

        /// <summary>
        /// When was the damage taken.
        /// </summary>
        public int DamageRound;

        /// <summary>
        /// Unique attack id.
        /// </summary>
        public string UniqueID;

        /// <summary>
        /// Attacker player.
        /// </summary>
        public BattleGamePlayers APlayer;

        /// <summary>
        /// Opponent player.
        /// </summary>
        public BattleGamePlayers OPlayer;

        /// <summary>
        /// When monster health recovered it is going to work.
        /// </summary>
        public int Recover;

        /// <summary>
        /// Hit Damage.
        /// </summary>
        public DamageDTO Damage;

        /// <summary>
        /// Flip counts.
        /// </summary>
        public Flips[] Flips;

        /// <summary>
        /// Stores the damage taken monster special condition.
        /// </summary>
        public BGSpecialConditions TargetSC;

        /// <summary>
        /// Attacker attack id.
        /// </summary>
        public int AttackID;

        /// <summary>
        /// When multiple action required we will store the action number in this.
        /// </summary>
        public int ActionNumber;

        /// <summary>
        /// Benched monsters damage.
        /// </summary>
        public int BenchDamage;

        /// <summary>
        /// Hit self damage.
        /// </summary>
        public int SelfDamage;

        /// <summary>
        /// Detached cards from the attacker cards.
        /// </summary>
        public int[] DACards;

        /// <summary>
        /// Detached cards from the defender cards.
        /// </summary>
        public int[] DDCards;

        /// <summary>
        /// Attacker of defender shield.
        /// </summary>
        public BGShieldTypes Shield;

        /// <summary>
        /// When the attacker monster state confused and hitself.
        /// </summary>
        public bool IsConfused;

        /// <summary>
        /// When poisoned the damage it will take per turn.
        /// </summary>
        public int PoDamage;

        /// <summary>
        /// When any monster force to attacker knockout like Gastly.
        /// </summary>
        public bool AKnockedOut;

        /// <summary>
        /// Whenever selected a card for the attacker card requirement.
        /// </summary>
        public int TargetCID;

        /// <summary>
        /// We store the effected attack id.
        /// Like blocking it.
        /// </summary>
        public int TAttackID;

        /// <summary>
        /// When the metronome copies the attack of the opponent this field is true.
        /// </summary>
        public bool IsCopy;

        /// <summary>
        /// When energy type is required to an action we will use.
        /// </summary>
        public EnergyTypes TEnergy;
    }

    [Serializable]
    public class DamageDTO
    {
        /// <summary>
        /// When resist the damage its true.
        /// </summary>
        public bool IsResist;

        /// <summary>
        /// When weakness applied returns.
        /// </summary>
        public bool IsWeakness;

        /// <summary>
        /// Damage value.
        /// </summary>
        public int Damage;

        /// <summary>
        /// Is that attack tried to dodge.
        /// </summary>
        public bool DodgedTried;

        /// <summary>
        /// When damage dodged successfully.
        /// Dodged works only if coin flips.
        /// </summary>
        public bool DodgeSucceed;

        /// <summary>
        /// When dodged its coin state.
        /// </summary>
        public Flips DodgedCoin;

        /// <summary>
        /// When the damage all damage is blocked.
        /// </summary>
        public bool Blocked;
    }
}
