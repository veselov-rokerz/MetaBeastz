using Assets.Scripts.BSSocket.Enums;
using System;
using TMPro;
using UnityEngine;

public class CardAbilityController : MonoBehaviour
{
    [Header("Currently only one ability exists.")]
    public GameObject GOAbility1;

    /// <summary>
    /// Shown card item.
    /// </summary>
    public CardController CardItem { get; set; }

    public void ShowAbilities(CardController card)
    {
        this.CardItem = card;

        // We store cardId.
        int cardId = this.CardItem.CardData.MetaData.CardId;

        // if this card has no ability just return.
        if (!Enum.IsDefined(typeof(BGAbilities), cardId))
        {
            // We disable it.
            GOAbility1.SetActive(false);

            return;
        }

        // OTherwise we activate the ability.
        GOAbility1.SetActive(IsUseable());

        // Ability name.
        string abilityName = LocalizationController.Instance.GetLanguage($"Ability_{cardId}");

        // Ability description.
        string abilityDescription = LocalizationController.Instance.GetLanguage($"Ability_Desc_{cardId}");

        // We print the ability desc.
        GOAbility1.transform.GetComponentInChild<TMP_Text>("Text").text = $"<b>{abilityName}</b> {abilityDescription}";
    }

    public bool IsUseable()
    {
        // Any special condition will prevent using.
        if (CardItem.SpecialCondition == BGSpecialConditions.Asleep ||
            CardItem.SpecialCondition == BGSpecialConditions.Confused ||
            CardItem.SpecialCondition == BGSpecialConditions.Paralyzed)
            return false;

        // Strike back attack is going to be disabled.
        if (CardItem.CardData.MetaData.CardId == (int)BGAbilities.ABIStrikesBack)
            return false;

        return true;
    }

    public void OnClickAbility()
    {
        // Attack flow started.
        AbilityController.Instance.NewAbility(this.CardItem);

        // We activate the cancel button.
        if (this.CardItem.CardData.MetaData.CardId != (int)BGAbilities.ABIEnergyBurn)
            BattleNotiController.Instance.BTNCancelAbility.gameObject.SetActive(true);

        // We deactive the action.
        if ((BGAbilities)this.CardItem.CardData.MetaData.CardId != BGAbilities.ABIBuzzap)
        {
            // We deactivate the action.
            this.CardItem.Playground.DeactivateAction();
        }

        // We start action depends on action.
        switch ((BGAbilities)this.CardItem.CardData.MetaData.CardId)
        {
            case BGAbilities.ABIDamageSwap:
            case BGAbilities.ABIEnergyTrans:
                {
                    // We ask for select a monster.
                    BattleNotiController.Instance.GOSelectAMonster.gameObject.SetActive(true);
                }
                break;
            case BGAbilities.ABIRainDance:
                {
                    BattleNotiController.Instance.GOAttachOneWaterEnergyTypeToAMonster.gameObject.SetActive(true);
                }
                break;
            case BGAbilities.ABIBuzzap:
                {
                    EnergyTypeSelectController.Instance.ShowEnergySelectViewForAbility(this.CardItem);
                }
                break;
        }
    }

    public void HideAbilities()
    {
        // OTherwise we activate the ability.
        GOAbility1.SetActive(false);
    }
}
