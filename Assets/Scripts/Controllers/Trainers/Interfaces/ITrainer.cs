using Assets.Scripts.BSSocket.DTO;
using System;

public interface ITrainer
{
    string UniqueID { get; set; }
    CardController PlayedCard { get; set; }
    bool IsPlayer { get; }
    BGTrainerRequestDTO RequestModel { get; set; }
    BGTrainerResponsePlayerDTO ResponseData { get; set; }
    ITrainer LoadData(CardController cardData);
    void Simulate(BGTrainerResponsePlayerDTO response, Action onSimulationCompleted);
    void Play(bool firstTime);
    void Generate();
}
