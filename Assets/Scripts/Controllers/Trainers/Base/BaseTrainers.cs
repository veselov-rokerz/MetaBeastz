using Assets.Scripts.BSSocket.DTO;
using System;
using UnityEngine;

public abstract class BaseTrainers : MonoBehaviour, ITrainer
{
    public string UniqueID { get; set; }
    public CardController PlayedCard { get; set; }
    public bool IsPlayer => this.PlayedCard.Playground.IsRealPlayer;
    public BGTrainerRequestDTO RequestModel { get; set; }
    public BGTrainerResponsePlayerDTO ResponseData { get; set; }
    public ITrainer LoadData(CardController cardData)
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

            // We say start play to server.
            Play(true);
        }

        // Return this.
        return this;
    }

    public virtual void Simulate(BGTrainerResponsePlayerDTO response, Action onSimulationCompleted)
    {
        this.ResponseData = response;
    }

    public virtual void Play(bool firstTime) { }

    public void Generate()
    {
        // We create a request model.
        RequestModel = new BGTrainerRequestDTO
        {
            UniqueID = this.UniqueID,
            CardID = this.PlayedCard.CardData.UniqueCardID
        };
    }
}
