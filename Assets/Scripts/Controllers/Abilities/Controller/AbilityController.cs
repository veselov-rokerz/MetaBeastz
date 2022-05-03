using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.Controllers.Abilities.Abilities;
using System;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public static AbilityController Instance { get; private set; }
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
    public IAbility ActiveAbility { get; private set; }

    public void NewAbility(CardController playedCard)
    {
        // if already exists just destroy it.
        if (ActiveAbility != null)
        {
            // We destroy the gameobject.
            Destroy(((MonoBehaviour)ActiveAbility).gameObject);

            // We clear the ability.
            ClearAbility();
        }
        // We get the type of comp.
        Type type = GetAbility((BGAbilities)playedCard.CardData.MetaData.CardId);

        // We get the component.
        Component abilityItem = new GameObject("Ability").AddComponent(type);

        // We changed the basic attack.
        ActiveAbility = (IAbility)abilityItem;

        // We load the default values.
        BaseAbilities baseTrainer = abilityItem.GetComponent<BaseAbilities>();

        // When play any trainer card.
        BattleNotiController.Instance.GODone.SetActive(false);

        // We load the releated data.
        baseTrainer.LoadData(playedCard);
    }

    public Type GetAbility(BGAbilities ability)
    {
        // We get the releated trainer.
        switch (ability)
        {
            default:
                return null;
            case BGAbilities.ABIDamageSwap:
                return typeof(ABIDamageSwap);
            case BGAbilities.ABIRainDance:
                return typeof(ABIRainDance);
            case BGAbilities.ABIEnergyBurn:
                return typeof(ABIEnergyBurn);
            case BGAbilities.ABIEnergyTrans:
                return typeof(ABIEnergyTrans);
            case BGAbilities.ABIBuzzap:
                return typeof(ABIBuzzap);
        }
    }

    public bool IsAbilityActive(string uniqueId) => ActiveAbility?.UniqueID == uniqueId;
    public bool IsAbilityActive(BGAbilities ability) => ActiveAbility?.PlayedCard.CardData.MetaData.CardId == (int)ability;

    public void ClearAbility()
    {
        // if ability exists we execute the termination panel.
        if (this.ActiveAbility != null)
            this.ActiveAbility.OnTerminate();

        // We remove the ability.
        this.ActiveAbility = null;

        // We disable all the panels.
        BattleNotiController.Instance.GOSelectAMonster.SetActive(false);
        BattleNotiController.Instance.GOSelectAnotherMonster.SetActive(false);
        BattleNotiController.Instance.GOAttachOneWaterEnergyTypeToAMonster.gameObject.SetActive(false);
    }
}
