using Assets.Scripts.BSSocket.DTO;
using System;
using UnityEngine;

public abstract class BaseAbilities : MonoBehaviour, IAbility
{
    public string UniqueID { get; set; }
    public CardController PlayedCard { get; set; }
    public bool IsPlayer => this.PlayedCard.Playground.IsRealPlayer;
    public BGAbilityRequestDTO RequestModel { get; set; }
    public BGAbilityResponseDTO ResponseData { get; set; }
    public IAbility LoadData(CardController cardData)
    {
        // We load all the data.
        this.PlayedCard = cardData;

        // We make sure it is real player.
        if (cardData.Playground.IsRealPlayer)
        {
            // We create a unique attack.
            this.UniqueID = Guid.NewGuid().ToString();

            // We load required data.
            Generate();

            // We play the card.
            Play(true);
        }

        // Return this.
        return this;
    }

    public virtual void Simulate(BGAbilityResponseDTO response, Action onSimulationCompleted)
    {
        this.ResponseData = response;
    }

    public virtual void Play(bool isFirstTime) { }
    public virtual void OnTerminate() { }

    public void Generate()
    {
        // We create a request model.
        RequestModel = new BGAbilityRequestDTO
        {
            UniqueID = this.UniqueID,
            CardID = this.PlayedCard.CardData.UniqueCardID
        };
    }
    
}
