using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.Controllers.Trainers.Trainers;
using System;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    public static TrainerController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Current play trainer.
    /// </summary>
    public ITrainer ActiveTrainer { get; private set; }

    public void NewTrainer(CardController playedCard)
    {
        // if already exists just destroy it.
        if (ActiveTrainer != null)
            Destroy(((MonoBehaviour)ActiveTrainer).gameObject);

        // We get the type of comp.
        Type type = GetTrainer((BGTrainers)playedCard.CardData.MetaData.CardId);

        // We get the component.
        Component trainerItem = new GameObject("Trainer").AddComponent(type);

        // We changed the basic attack.
        ActiveTrainer = (ITrainer)trainerItem;

        // We load the default values.
        BaseTrainers baseTrainer = trainerItem.GetComponent<BaseTrainers>();

        // When play any trainer card.
        BattleNotiController.Instance.GODone.SetActive(false);

        // We load the releated data.
        baseTrainer.LoadData(playedCard);
    }

    public Type GetTrainer(BGTrainers trainer)
    {
        // We get the releated trainer.
        switch (trainer)
        {
            default:
                return null;
            case BGTrainers.TRComputerSearch:
                return typeof(TRComputerSearch);
            case BGTrainers.TRDevolutionSpray:
                return typeof(TRDevolutionSpray);
            case BGTrainers.TRImpProOak:
                return typeof(TRImpProOak);
            case BGTrainers.TRItemFinder:
                return typeof(TRItemFinder);
            case BGTrainers.TRLass:
                return typeof(TRLass);
            case BGTrainers.TRPokemonBreeder:
                return typeof(TRPokemonBreeder);
            case BGTrainers.TRPokemonTraider:
                return typeof(TRPokemonTrader);
            case BGTrainers.TRScoopUp:
                return typeof(TRScoopUp);
            case BGTrainers.TRSuperEnergyRemoval:
                return typeof(TRSuperEnergyRemoval);
            case BGTrainers.TRDefender:
                return typeof(TRDefender);
            case BGTrainers.TREnergyRetrieval:
                return typeof(TREnergyRetrieval);
            case BGTrainers.TRFullHeal:
                return typeof(TRFullHeal);
            case BGTrainers.TRMaintenance:
                return typeof(TRMaintenance);
            case BGTrainers.TRPlusPower:
                return typeof(TRPlusPower);
            case BGTrainers.TRPokemonCenter:
                return typeof(TRPokemonCenter);
            case BGTrainers.TRPokemonFlute:
                return typeof(TRPokemonFlute);
            case BGTrainers.TRPokedex:
                return typeof(TRPokedex);
            case BGTrainers.TRProOak:
                return typeof(TRProOak);
            case BGTrainers.TRRevive:
                return typeof(TRRevive);
            case BGTrainers.TRSuperPotion:
                return typeof(TRSuperPotion);
            case BGTrainers.TRBill:
                return typeof(TRBill);
            case BGTrainers.TREnergyRemoval:
                return typeof(TREnergyRemoval);
            case BGTrainers.TRGustOfWind:
                return typeof(TRGustOfWind);
            case BGTrainers.TRPotion:
                return typeof(TRPotion);
            case BGTrainers.TRSwitch:
                return typeof(TRSwitch);
        }
    }

    public bool IsTrainerActive(string uniqueId) => ActiveTrainer?.UniqueID == uniqueId;
    public bool IsTrainerActive(BGTrainers trainer) => (BGTrainers)ActiveTrainer?.PlayedCard.CardData.MetaData.CardId == trainer;

    public void ClearTrainer()
    {
        this.ActiveTrainer = null;
    }
}
