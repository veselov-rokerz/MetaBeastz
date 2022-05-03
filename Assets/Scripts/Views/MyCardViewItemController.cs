using Assets.Scripts.GSSocket.DTO;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyCardViewItemController : MonoBehaviour
{
    [Header("Card information.")]
    public CardDTO CardData;

    [Header("User card information.")]
    public UserCardDTO UserCardData;

    [Header("Image of card.")]
    public Image IMGCard;

    #region MANUAL BINDING UI (CLOSED)

    /*[Header("Name of the card.")]
    public TMP_Text TXTCardName;

    [Header("Hp of the card.")]
    public TMP_Text TXTHp;

    [Header("Image of card.")]
    public Image IMGCard;

    [Header("Template of the card. Like fire, grass etc.")]
    public Image IMGTemplate;

    [Header("We will create attack costs from the prefab.")]
    public GameObject PREFAttackCost;

    [Header("First attack field.")]
    public GameObject GOFirstAttack;

    [Header("Second attack field")]
    public GameObject GOSecondAttack;

    [Header("Seperator between attacks when more then 1 attack exists.")]
    public GameObject GOSeperatorBetweenAttacks;

    [Header("Weakness image if exists.")]
    public Image IMGWeakness;

    [Header("Resistance image if exists.")]
    public Image IMGResistance;

    [Header("We will use prefab when retreat cost exists.")]
    public GameObject PREFRetreatItem;

    [Header("We will print the retreats here.")]
    public Transform TRRetreatContent;*/

    #endregion

    public void LoadCardInfo(CardDTO card, UserCardDTO userCard)
    {
        this.CardData = card;
        this.UserCardData = userCard;

        // We get the sprite of the card.
        IMGCard.sprite = ResourceController.Instance.GetCardSprite(card.CardId);

        #region Manual Bindings (CLOSED)

        #region Card General Informations
        /*
        // We print the card name.
        TXTCardName.text = CardService.Instance.GetCardName(card.CardId);

        // We print the hp of card.
        TXTHp.text = $"{card.CardHp}";

        // We will create pokemon type cards.
        if (card.CardTypeId == CardTypes.Pokemon)
        {
            // We load the card id.
            IMGTemplate.sprite = ResourceController.Instance.GetCardTemplate(card.EnergyTypeId);

            // We get the sprite of the card.
            IMGCard.sprite = ResourceController.Instance.GetCardSprite(card.CardId);

            // We make sure card sprite is active.
            IMGCard.gameObject.SetActive(true);

            // We disable the card name.
            TXTCardName.gameObject.SetActive(true);
        }
        else if (card.CardTypeId == CardTypes.Energy)
        {
            // We load the card template for the type.
            IMGTemplate.sprite = ResourceController.Instance.GetCardSprite(card.CardId);

            // We deactivate the game card.
            IMGCard.gameObject.SetActive(false);

            // We disable the card name.
            TXTCardName.gameObject.SetActive(false);
        }
        else if (card.CardTypeId == CardTypes.Trainer) // if trainer then we just disable it.
        {
            gameObject.SetActive(false);
        }
        */
        #endregion

        #region Weakness

        // We deactivate the weakness if it is none.
        /*IMGWeakness.gameObject.SetActive(card.WeaknessEnergyTypeId != EnergyTypes.None);

        // if activated then we will replace with new weakness energy.
        if (IMGWeakness.gameObject.activeSelf)
        {
            // We are looking for the energy type.
            IMGWeakness.sprite = ResourceController.Instance.GetEnergyType(card.WeaknessEnergyTypeId);

            // We also set the multiplier.
            IMGWeakness.GetComponentInChildren<TMP_Text>().text = $"{card.WeaknessMultiplier}x";
        }*/

        #endregion

        #region Resistance

        // We deactivate the resistance if it is none.
        /*IMGResistance.gameObject.SetActive(card.ResistanceEnergyTypeId != EnergyTypes.None);

        // if activated then we will replace with new resistance energy.
        if (IMGResistance.gameObject.activeSelf)
        {
            // We are looking for the energy type.
            IMGResistance.sprite = ResourceController.Instance.GetEnergyType(card.ResistanceEnergyTypeId);

            // We also set the reduction.
            IMGResistance.GetComponentInChildren<TMP_Text>().text = $"-{card.ResistanceBonus}";
        }*/

        #endregion

        #region Retreat

        // We remove older retreat items.
        /*TRRetreatContent.RemoveAllChildsOfTransform();

        // We loop retreat costs.
        foreach (CardRetreatCostDTO retreatCost in card.CardRetreatCosts)
        {
            // We have to loop as much as retreat cost.
            for (int ii = 0; ii < retreatCost.Quantity; ii++)
            {
                // We instantiate retreat item.
                GameObject retreatItem = Instantiate(PREFRetreatItem, TRRetreatContent);

                // We set the energy type.
                retreatItem.GetComponent<Image>().sprite = ResourceController.Instance.GetEnergyType(retreatCost.RetreatEnergyTypeId);
            }
        }*/

        #endregion

        #region First Attack

        // if attack counts more than 0 first attack going to be active.
        //GOFirstAttack.gameObject.SetActive(card.CardAttacks.Count >= 1);

        // We will fill all the card informations.
        /*if (GOFirstAttack.gameObject.activeSelf)
        {
            // This is the first attack.
            CardAttackDTO firstAttack = card.CardAttacks[0];

            // We get the attack informations.
            AttackDTO attackData = AttackService.Instance.GetAttackById(firstAttack.AttackId);

            // First attack costs.
            Transform cardFirstAttackCostContent = GOFirstAttack.transform.Find("Cost");

            // We remove older stuffs in attack cost.
            cardFirstAttackCostContent.RemoveAllChildsOfTransform();

            // We print all the attack costs.
            foreach (AttackCostDTO attackCost in attackData.AttackCosts)
            {
                // We initiate attack costs as much as quantity.
                for (int ii = 0; ii < attackCost.Quantity; ii++)
                {
                    // We instantiate cost item.
                    GameObject attackCostItem = Instantiate(PREFAttackCost, cardFirstAttackCostContent);

                    // We load the energy.
                    attackCostItem.GetComponent<Image>().sprite = ResourceController.Instance.GetEnergyType(attackCost.EnergyTypeId);
                }
            }

            // First attack description.
            TMP_Text descriptionOfFirstAttack = GOFirstAttack.transform.GetComponentInChild<TMP_Text>("Description");

            // We set the attack description.
            descriptionOfFirstAttack.text = $"<b>{AttackService.Instance.GetAttackName(firstAttack.AttackId)}</b>{Environment.NewLine}";
            descriptionOfFirstAttack.text += $"{AttackService.Instance.GetAttackDescription(firstAttack.AttackId)}";

            // We will text the damage of attack.
            TMP_Text damageOfFirstAttack = GOFirstAttack.transform.GetComponentInChild<TMP_Text>("Value");

            // We print the damage.
            damageOfFirstAttack.text = $"{attackData.AttackDamage}";

            // We deactivate if 0.
            damageOfFirstAttack.gameObject.SetActive(attackData.AttackDamage != 0);
        }
        */

        #endregion

        #region Attack Seperator

        // if two attack exists we are going to activate.
        //GOSeperatorBetweenAttacks.SetActive(card.CardAttacks.Count >= 2);

        #endregion

        #region Second Attack

        // if attack counts more than 1 second attack going to be active.
        /*GOSecondAttack.gameObject.SetActive(card.CardAttacks.Count >= 2);

        // We will fill all the card informations.
        if (GOSecondAttack.gameObject.activeSelf)
        {
            // This is the second attack.
            CardAttackDTO secondAttack = card.CardAttacks[1];

            // We get the attack informations.
            AttackDTO attackData = AttackService.Instance.GetAttackById(secondAttack.AttackId);

            // Second attack costs.
            Transform cardSecondAttackCostContent = GOSecondAttack.transform.Find("Cost");

            // We remove older stuffs in attack cost.
            cardSecondAttackCostContent.RemoveAllChildsOfTransform();

            // We print all the attack costs.
            foreach (AttackCostDTO attackCost in attackData.AttackCosts)
            {
                // We initiate attack costs as much as quantity.
                for (int ii = 0; ii < attackCost.Quantity; ii++)
                {
                    // We instantiate cost item.
                    GameObject attackCostItem = Instantiate(PREFAttackCost, cardSecondAttackCostContent);

                    // We load the energy.
                    attackCostItem.GetComponent<Image>().sprite = ResourceController.Instance.GetEnergyType(attackCost.EnergyTypeId);
                }
            }

            // Second attack description.
            TMP_Text descriptionOfSecondAttack = GOSecondAttack.transform.GetComponentInChild<TMP_Text>("Description");

            // We set the attack description.
            descriptionOfSecondAttack.text = $"<b>{AttackService.Instance.GetAttackName(secondAttack.AttackId)}</b>{Environment.NewLine}";
            descriptionOfSecondAttack.text += $"{AttackService.Instance.GetAttackDescription(secondAttack.AttackId)}";

            // We will text the damage of attack.
            TMP_Text damageOfSecondAttack = GOSecondAttack.transform.GetComponentInChild<TMP_Text>("Value");

            // We print the damage.
            damageOfSecondAttack.text = $"{attackData.AttackDamage}";

            // We deactivate if 0.
            damageOfSecondAttack.gameObject.SetActive(attackData.AttackDamage != 0);
        }
        */
        #endregion

        #endregion

    }
}
