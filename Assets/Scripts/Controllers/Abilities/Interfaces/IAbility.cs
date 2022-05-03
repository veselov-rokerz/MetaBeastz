using Assets.Scripts.BSSocket.DTO;
using System;

public interface IAbility
{
    string UniqueID { get; set; }
    CardController PlayedCard { get; set; }
    bool IsPlayer { get; }
    BGAbilityRequestDTO RequestModel { get; set; }
    BGAbilityResponseDTO ResponseData { get; set; }
    IAbility LoadData(CardController cardData);
    void Simulate(BGAbilityResponseDTO response, Action onSimulationCompleted);
    void Play(bool isFirstTime);
    void OnTerminate();
    void Generate();
}
