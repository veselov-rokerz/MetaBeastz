using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.Controllers.Attacks;
using Assets.Scripts.GSSocket.DTO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardBlockAttackSelectController : MonoBehaviour
{
    [Header("We close attack parent when enabled.")]
    public GameObject GOAttacksParent;

    [Header("Attack list.")]
    public Button[] GOAttacks;

    [Header("Energy cost item to print.")]
    public GameObject GOEnergyCost;

    public void ShowAttacks(CardController cardItem)
    {
        // We activate the attacks parent.
        this.GOAttacksParent.SetActive(true);

        // We deactivate all of them.
        foreach (Button attack in GOAttacks)
        {
            // We activate the attack by default.
            attack.gameObject.SetActive(false);

            // We clear all the listeners.
            attack.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        // if first exists we will show it.
        foreach (AttackDTO attack in cardItem.CardData.MetaData.CardAttacks)
        {
            // We get the attack index.
            int index = cardItem.CardData.MetaData.CardAttacks.IndexOf(attack);

            // we prevent going out of boundry.
            if (index >= GOAttacks.Length)
                break;

            // We get the attack.
            Button attackItem = GOAttacks[index];

            // We activate the attack item.
            attackItem.gameObject.SetActive(IsSelectable(attack));

            // We add a listener.
            attackItem.onClick.AddListener(() => OnClickAttack(cardItem, attack));

            // Attack name.
            string attackName = LocalizationController.Instance.GetLanguage($"Attack_{attack.AttackId}");

            // Attack description.
            string attackDescription = LocalizationController.Instance.GetLanguage($"Attack_Desc_{attack.AttackId}");

            // We print the attack description.
            attackItem.transform.GetComponentInChild<TMP_Text>("Text").text = $"<b>{attackName}</b> {attackDescription }";

            // We print the damage.
            attackItem.transform.GetComponentInChild<TMP_Text>("Damage").text = $"{attack.AttackDamage}";

            // Attack costs.
            Transform costs = attackItem.transform.Find("Costs");

            // We remove all the old energies in card.
            costs.RemoveAllChildsOfTransform();

            // We add all the attack costs.
            attack.AttackCosts.ForEach(e =>
            {
                // As much as quantity.
                for (int ii = 0; ii < e.Quantity; ii++)
                {
                    // We create an energy
                    GameObject energyCost = Instantiate(GOEnergyCost, costs);

                    // We add energy sprite.
                    energyCost.GetComponent<Image>().sprite = ResourceController.Instance.GetEnergyType(e.EnergyTypeId);
                }
            });
        }
    }
    public bool IsSelectable(AttackDTO attack)
    {
        // if current attack is metronome
        if (AttackController.Instance.ActiveAttack.AttackData.AttackId == (int)BGAttacks.ATKMetronome)
        {
            // Metronome cant copy its same.
            if (attack.AttackId == AttackController.Instance.ActiveAttack.AttackData.AttackId)
                return false;
        }

        // Otherwise return true.
        return true;
    }

    public void HideAttacks()
    {
        // We close all the attacks.
        foreach (Button attack in GOAttacks)
        {
            // We disable all the attacks.
            attack.gameObject.SetActive(false);

            // We clear all the listeners.
            attack.onClick.RemoveAllListeners();
        }

        // We deactivate the attacks parent.
        this.GOAttacksParent.SetActive(false);
    }
    public void OnClickAttack(CardController sender, AttackDTO attack)
    {
        // if no attack exists return.
        if (AttackController.Instance.ActiveAttack == null)
            return;

        // We switch between attacks.
        switch ((BGAttacks)AttackController.Instance.ActiveAttack.AttackData.AttackId)
        {
            case BGAttacks.ATKAmnesia:
                {
                    // We get the amnesia.
                    ATKAmnesia atkAmnesia = (ATKAmnesia)AttackController.Instance.ActiveAttack;

                    // We tell the block the attack.
                    atkAmnesia.ManualAttack(attack.AttackId);
                }
                break;
            case BGAttacks.ATKMetronome:
                {
                    // We close the select an energy.
                    BattleNotiController.Instance.GOSelectYourOpponentAttack.SetActive(false);

                    // We make sure they are not same.
                    if (attack.AttackId != AttackController.Instance.ActiveAttack.AttackData.AttackId)
                        sender.CardAttack.OnClickAttack(AttackController.Instance.ActiveAttack.CardData, attack);
                }
                break;
        }

        // We deeactivate the action details.
        sender.Playground.DeactivateAction();

    }
}
