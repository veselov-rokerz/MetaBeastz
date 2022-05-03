using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.Controllers.Attacks;
using Assets.Scripts.GSSocket.DTO;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardAttackController : MonoBehaviour
{
    [Header("Attack list.")]
    public Button[] GOAttacks;

    [Header("Energy cost item to print.")]
    public GameObject GOEnergyCost;

    public void ShowAttacks(CardController cardItem)
    {
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
            attackItem.gameObject.SetActive(IsUseable(cardItem, attack));

            // We add a listener.
            attackItem.onClick.AddListener(() => OnClickAttack(cardItem, attack));

            // Attack name.
            string attackName = LocalizationController.Instance.GetLanguage($"Attack_{attack.AttackId}");

            // Attack description.
            string attackDescription = LocalizationController.Instance.GetLanguage($"Attack_Desc_{attack.AttackId}");

            // We print the attack description.
            attackItem.transform.GetComponentInChild<TMP_Text>("Text").text = $"<b>{attackName}</b> {attackDescription }";

            // We print the damage.
            if (attack.AttackDamage > 0)
                attackItem.transform.GetComponentInChild<TMP_Text>("Damage").text = $"{attack.AttackDamage}";
            else
                attackItem.transform.GetComponentInChild<TMP_Text>("Damage").text = $"{string.Empty}";

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
    public void OnClickAttack(CardController sender, AttackDTO attack)
    {
        // We close the done action.
        BattleNotiController.Instance.GODone.SetActive(false);

        // Attack flow started.
        AttackController.Instance.NewAttack(sender, attack);

        // Some attacks may required additional actions.
        switch ((BGAttacks)AttackController.Instance.ActiveAttack?.AttackData.AttackId)
        {
            case BGAttacks.ATKFireSpin:
                {
                    // We show the energy detach view.
                    CardEnergyDetachController.Instance.ShowEnergyDetachViewForAttack(sender, attack, true);
                }
                break;
            case BGAttacks.ATKWhirlpool:
                {
                    // We activate the opponent action detail to detach..
                    PlaygroundController op = BattleGameController.Instance.GetOpponentOfPlayground(sender.Playground);

                    // We make sure energy exists.
                    if (op.PlayerActive.CardEnergyAttachment.AttachedEnergies.Count > 0)
                    {
                        // We tell the select an energy.
                        BattleNotiController.Instance.GODetachOneEnergyCardAlert.SetActive(true);

                        // We close the action view.
                        sender.Playground.DeactivateAction();

                        // We activate the opponent action.
                        op.ActivateAction(op.PlayerActive);

                        // We ask for detach controller.
                        CardEnergyDetachController.Instance.ShowEnergyDetachViewForAttack(op.PlayerActive, attack, true);
                    }
                    else // Otherwise just hit.
                    {
                        // We get the opponent active monster.
                        ATKWhirlpool whirlpool = (ATKWhirlpool)AttackController.Instance.ActiveAttack;

                        // We send empty because there is no attached energy to opponent active monster.
                        whirlpool.AttackManual(new int[] { });
                    }
                }
                break;
            case BGAttacks.ATKHyperBeam:
                {
                    // We activate the opponent action detail to detach..
                    PlaygroundController op = BattleGameController.Instance.GetOpponentOfPlayground(sender.Playground);

                    // We make sure energy exists.
                    if (op.PlayerActive.CardEnergyAttachment.AttachedEnergies.Count > 0)
                    {
                        // We tell the select an energy.
                        BattleNotiController.Instance.GODetachOneEnergyCardAlert.SetActive(true);

                        // We close the action view.
                        sender.Playground.DeactivateAction();

                        // We activate the opponent action.
                        op.ActivateAction(op.PlayerActive);

                        // We ask for detach controller.
                        CardEnergyDetachController.Instance.ShowEnergyDetachViewForAttack(op.PlayerActive, attack, true);
                    }
                    else // Otherwise just hit.
                    {
                        // We get the opponent active monster.
                        ATKHyperBeam hyperbeam = (ATKHyperBeam)AttackController.Instance.ActiveAttack;

                        // We send empty because there is no attached energy to opponent active monster.
                        hyperbeam.AttackManual(new int[] { });
                    }
                }
                break;
            case BGAttacks.ATKAmnesia:
                {
                    // We tell the select an energy.
                    BattleNotiController.Instance.GOSelectYourOpponentAttack.SetActive(true);

                    // We close the action view.
                    sender.Playground.DeactivateAction();

                    // We activate the opponent action detail to detach..
                    PlaygroundController op = BattleGameController.Instance.GetOpponentOfPlayground(sender.Playground);

                    // We activate the opponent action.
                    op.ActivateAction(op.PlayerActive);

                    // We force to select an attack to block.
                    op.PlayerActive.CardAttackBlocker.ShowAttacks(op.PlayerActive);
                }
                break;
            case BGAttacks.ATKMetronome:
                {
                    // We tell the select an energy.
                    BattleNotiController.Instance.GOSelectYourOpponentAttack.SetActive(true);

                    // We close the action view.
                    sender.Playground.DeactivateAction();

                    // We activate the opponent action detail to detach..
                    PlaygroundController op = BattleGameController.Instance.GetOpponentOfPlayground(sender.Playground);

                    // We activate the opponent action.
                    op.ActivateAction(op.PlayerActive);

                    // We force to select an attack to block.
                    op.PlayerActive.CardAttackBlocker.ShowAttacks(op.PlayerActive);
                }
                break;
            case BGAttacks.ATKConversion1:
            case BGAttacks.ATKConversion2:
                {
                    // We show the energy selection view.
                    EnergyTypeSelectController.Instance.ShowEnergySelectView(sender, attack, EnergyTypes.Colorless);
                }
                break;
        }
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
    }
    public bool IsUseable(CardController card, AttackDTO attack)
    {
        // in first raund cant play any attack.
        if (BattleGameController.Instance.IsFirstRound) return false;

        // if attacker card is in paralyzed just return false.
        if (card.SpecialCondition == BGSpecialConditions.Paralyzed) return false;

        // if attacker card is in asleep just return false.
        if (card.SpecialCondition == BGSpecialConditions.Asleep) return false;

        // if this attack is blocked just return.
        if (card.Playground.BlockedAttack == (BGAttacks)attack.AttackId) return false;

        // if waiting for opponent to switch an active monster.
        if (BattleNotiController.Instance.GOForceToSelectActiveCard.activeSelf) return false;

        // We wait for opponent to select a monster.
        if (BattleNotiController.Instance.GOYourOpSelectingAMonster.activeSelf) return false;

        // Some attacks can be required some conditions.
        switch ((BGAttacks)attack.AttackId)
        {
            // Dream eater required opponent to be asleep.
            case BGAttacks.ATKDreamEater:
                {
                    if (BattleGameController.Instance.GetOpponentOfPlayground(card.Playground).PlayerActive?.SpecialCondition != BGSpecialConditions.Asleep)
                        return false;
                }
                break;
            case BGAttacks.ATKLeekSlap:
                {
                    // Attack leek slap can be used only once.
                    if (card.AttackDataPersistInPlay)
                        return false;
                }
                break;
            case BGAttacks.ATKMirrorMove:
                {
                    // Last damage data is null then you cant use this power.
                    if (card.LastDamageData == null)
                        return false;

                    // Level dif between last attack and raund.
                    int diff = BattleGameController.Instance.CurrentRaund - card.LastDamageData.DamageRound;

                    // if level diff is not equals 1 just return false.
                    if (diff != 1)
                        return false;
                }
                break;
            case BGAttacks.ATKConversion1:
                {
                    // We get the opponent.
                    PlaygroundController opponent = BattleGameController.Instance.GetOpponentOfPlayground(card.Playground);

                    // if opponent has a weakness.
                    if (opponent.PlayerActive?.CardData.MetaData.WeaknessEnergyTypeId == EnergyTypes.None)
                        return false;
                }
                break;
        }

        // We get the all attached energies to the card.
        var attachedEnergyCards = card.CardEnergyAttachment.AttachedEnergies.SelectMany(x =>
        {
            List<EnergyTypes> energies = new List<EnergyTypes>();

            // if a double colorless energy we add 2 times.
            if (x.Item2.CardData.MetaData.CardId == (int)BGCards.DoubleColourlessEnergy)
            {
                for (int ii = 0; ii < 2; ii++)
                    energies.Add(EnergyTypes.Colorless);
            }
            // Otherwise if its electrode we just energy with temp energy type.
            else if (x.Item2.CardData.MetaData.CardId == (int)BGCards.Electrode)
            {
                for (int ii = 0; ii < 2; ii++)
                    energies.Add(x.Item2.TempEnergy);
            }
            else // We add energy.
                energies.Add(x.Item1.EnergyCard.CardData.MetaData.EnergyTypeId);

            return energies;
        }).ToList();

        // We check all the condition.
        foreach (var attackCost in attack.AttackCosts.OrderByDescending(x => x.EnergyTypeId != EnergyTypes.Colorless))
        {
            // This is the energy of attack.
            EnergyTypes attackEnergyCost = attackCost.EnergyTypeId;

            // if the card is charizard.
            if (card.CardData.MetaData.CardId == (int)BGCards.Charizard)
            {
                // if the energy burn ability active we will change as colorless.
                if (card.PerTurnData)
                    attackEnergyCost = EnergyTypes.Colorless;
            }

            // We store the quantity.
            int quantity = attackCost.Quantity;

            // We wait untill quantity is bigger than 0.
            while (quantity > 0)
            {
                // if the retreat cost colorless.
                if (attackEnergyCost == EnergyTypes.Colorless)
                {
                    // We make sure it is exists.
                    if (attachedEnergyCards.Count > 0)
                    {
                        // We remove from the attached energy list.
                        attachedEnergyCards.RemoveAt(0);

                        // We reduce the quantity of expected.
                        quantity -= 1;

                        // Contine for the next.
                        continue;
                    }
                }
                else
                {
                    // We make sure it is exists.
                    if (attachedEnergyCards.Contains(attackEnergyCost))
                    {
                        // We remove from the attached list.
                        attachedEnergyCards.Remove(attackEnergyCost);

                        // We decrease the energy.
                        quantity -= 1;

                        // We continue for the next.
                        continue;
                    }
                }

                // We return false its because condition is not ready.
                return false;
            }
        }

        // if reaches here it is possible.
        return true;
    }
}
