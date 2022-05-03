using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.GSSocket.DTO;
using System;
using UnityEngine;

namespace Assets.Scripts.Controllers.Attacks
{
    public abstract class BaseATK : MonoBehaviour, IAttack
    {
        public string UniqueID { get; set; }
        public CardController CardData { get; set; }
        public AttackDTO AttackData { get; set; }
        public bool IsPlayer => this.CardData.Playground.IsRealPlayer;
        public BGAttackRequestDTO RequestModel { get; set; }
        public IAttack LoadData(CardController cardData, AttackDTO attack)
        {
            // We load all the data.
            this.CardData = cardData;
            this.AttackData = attack;

            // We make sure it is real player.
            if (cardData.Playground.IsRealPlayer)
            {
                // We create a unique attack.
                this.UniqueID = Guid.NewGuid().ToString();

                // We load required data.
                Generate();

                // We say start attack to server.
                Attack(0);
            }

            // Return this.
            return this;
        }

        public virtual void Simulate(BGAttackResponseDTO response, Action onSimulationCompleted) { }

        public virtual void Attack(int actionIndex) { }

        public void Generate()
        {
            // We create a request model.
            RequestModel = new BGAttackRequestDTO
            {
                UniqueID = this.UniqueID,
                AttackID = this.AttackData.AttackId
            };
        }
    }
}
