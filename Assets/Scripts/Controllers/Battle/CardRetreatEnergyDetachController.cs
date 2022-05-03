using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.GSSocket.DTO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardRetreatEnergyDetachController : MonoBehaviour
{
    public static CardRetreatEnergyDetachController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// We store the shown card detail.
    /// </summary>
    public CardController ShownCard { get; private set; }

    /// <summary>
    /// When detach is active.
    /// </summary>
    public bool IsDetachActive => GODetachEnergyView?.activeSelf == true;

    /// <summary>
    /// When retreating a card. Prop is going to be true.
    /// </summary>
    public bool IsRetreating { get; private set; }

    [Header("We will store the detached cards.")]
    public List<CardEnergyAttachmentItemController> DetachedEnergies;

    [Header("We will activate when detach an energy.")]
    public GameObject GODetachEnergyView;

    [Header("Energy item when detach.")]
    public GameObject DetachEnergyItem;

    [Header("Energies to detach")]
    public Transform TRDetachEnergies;

    [Header("Send to server.")]
    public Button BTNOk;

    [Header("We will show the alert when required to select a monster from bench.")]
    public GameObject GOABenchedMonsterAlert;

    public void ShowEnergyDetachView(CardController movementItem)
    {
        // When retreat active its going to be disabled.
        BattleNotiController.Instance.GODone.SetActive(false);

        // We make sure button interactable is disabled.
        BTNOk.interactable = false;

        // We set the shown card.
        this.ShownCard = movementItem;

        // We clear the list.
        DetachedEnergies.Clear();

        // We show the detach view.
        GODetachEnergyView.SetActive(true);

        // We refresh all the attachments.
        RefreshEnergyDetachView();
    }

    public void AttachRetreatEnergyCostAuto()
    {
        // We get the attached energy cards.
        var attachedCards = this.ShownCard.CardEnergyAttachment.AttachedEnergies.ToList();

        // We will attach all the cost to the view.
        this.ShownCard.CardData.MetaData.CardRetreatCosts.ForEach(e =>
        {
            // We loop as much as quantity.
            for (int ii = 0; ii < e.Quantity; ii++)
            {
                // We find the attached card.
                var autoAttachedCard = attachedCards.Find(x => x.Item1.EnergyCard.CardData.MetaData.EnergyTypeId == e.RetreatEnergyTypeId);

                // if type is colorless then we are search for any other type.
                if (autoAttachedCard == null && e.RetreatEnergyTypeId == EnergyTypes.Colorless)
                    autoAttachedCard = attachedCards.FirstOrDefault();

                // if exists we will remove it.
                if (autoAttachedCard != null)
                {
                    // We remove it.
                    attachedCards.Remove(autoAttachedCard);

                    // We add to detached energies.
                    this.DetachedEnergies.Add(autoAttachedCard.Item1);
                }
            }
        });

        // we refresh all the deteach view.
        RefreshEnergyDetachView();
    }

    public void RefreshEnergyDetachView()
    {
        // We make sure button interactable is disabled.
        BTNOk.interactable = IsDetachable();

        // We remove all the children.
        TRDetachEnergies.RemoveAllChildsOfTransform();

        // We store the detached energies to prevent using dublicates.
        List<CardEnergyAttachmentItemController> detachedEnergies = this.DetachedEnergies
            .OrderByDescending(x => x.EnergyCard.CardData.MetaData.EnergyTypeId != EnergyTypes.Colorless)
            .ToList();

        // We instantiate retreat cost.
        this.ShownCard.CardData.MetaData.CardRetreatCosts.ForEach(e =>
        {
            // We have to loop as much as required cost.
            for (int ii = 0; ii < e.Quantity; ii++)
            {
                // We create an energy item.
                GameObject energyItem = Instantiate(DetachEnergyItem, TRDetachEnergies);

                // We are looking for the detach.
                // First we are looking for same type.
                CardEnergyAttachmentItemController detachedEnergy = detachedEnergies.Find(x => x.EnergyCard.CardData.MetaData.EnergyTypeId == e.RetreatEnergyTypeId);

                // if not exists we are going to look for it. We make sure the retreat cost is colorless.
                // if type is colorless then we are search for any other type.
                if (detachedEnergy == null && e.RetreatEnergyTypeId == EnergyTypes.Colorless)
                    detachedEnergy = detachedEnergies.FirstOrDefault();

                // We disable because it is empty.
                if (detachedEnergy != null)
                {
                    // We make detachable.
                    energyItem.GetComponent<Button>().interactable = true;

                    // We add a listener to make removeable.
                    energyItem.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        // We reactivate the detached energy.
                        detachedEnergy.gameObject.SetActive(true);

                        // We remove from the list.
                        this.DetachedEnergies.Remove(detachedEnergy);

                        // We will refresh the attachment view.
                        RefreshEnergyDetachView();
                    });

                    // We store energy icon.
                    Sprite energyIcon;
                    if (detachedEnergy.EnergyCard.CardData.MetaData.CardTypeId == CardTypes.Pokemon)
                        energyIcon = ResourceController.Instance.GetEnergyType(detachedEnergy.EnergyCard.TempEnergy);
                    else
                        energyIcon = ResourceController.Instance.GetEnergyType(detachedEnergy.EnergyCard.CardData.MetaData.EnergyTypeId);

                    // We update the image.
                    energyItem.GetComponent<Image>().sprite = energyIcon;

                    // We remove from the list prevent dublications.
                    detachedEnergies.Remove(detachedEnergy);
                }
                else
                {
                    // We make detachable.
                    energyItem.GetComponent<Button>().interactable = false;
                }
            }
        });
    }

    public void DetachAnEnergyCard(CardEnergyAttachmentItemController energyItem)
    {
        // if not a valid energy just return.
        if (!IsValidEnergy(energyItem.EnergyCard.CardData.MetaData.EnergyTypeId))
            return;

        // We have to detach if not already detached..
        if (this.DetachedEnergies.Contains(energyItem))
            return;

        // We add to detached list.
        this.DetachedEnergies.Add(energyItem);

        // We hide the energy item.
        energyItem.gameObject.SetActive(false);

        // We refresh the view.
        RefreshEnergyDetachView();
    }

    public bool IsValidEnergy(EnergyTypes energyType)
    {
        // We get the required energies.
        List<EnergyTypes> requireds = GetRequiredEnergies();

        // We remove already detached energies.
        foreach (var detachedEnergy in this.DetachedEnergies.OrderByDescending(x => x.EnergyCard.CardData.MetaData.EnergyTypeId != EnergyTypes.Colorless))
        {
            // We remove from requireds.
            requireds.Remove(detachedEnergy.EnergyCard.CardData.MetaData.EnergyTypeId);
        }

        // if it is required just return true.
        if (requireds.Contains(energyType))
            return true;
        else // if not contains the given type we will check is that for colorless.
            return requireds.Contains(EnergyTypes.Colorless);
    }


    public void RollbackAllDetachedEnergiesAndClose()
    {
        // We rollback all the energies.
        this.DetachedEnergies.ForEach(e => e.gameObject.SetActive(true));

        // We clear the list.
        this.DetachedEnergies.Clear();

        // We close the detach view.
        CloseDetachView();
    }

    public void CloseDetachView()
    {
        // We close the energy view.
        this.GODetachEnergyView.SetActive(false);

        // When retreat is closed just set true.
        BattleNotiController.Instance.ShowDoneButton();
    }

    public void OnClickOk()
    {
        // We say it is true.
        this.IsRetreating = true;

        // We activate the alert.
        GOABenchedMonsterAlert.SetActive(true);

        // We deactivate the action.
        this.ShownCard.Playground.DeactivateAction();
    }

    public List<EnergyTypes> GetRequiredEnergies()
    {
        // Needed energy.
        List<EnergyTypes> neededEnergies = new List<EnergyTypes>();

        // We get the required energy cost for current attack.
        List<EnergyTypes> requiredEnergies = this.ShownCard.CardData.MetaData.CardRetreatCosts.SelectMany(x =>
        {
            // We store the required energies.
            List<EnergyTypes> energies = new List<EnergyTypes>();

            // We loop as much as quantity.
            for (int ii = 0; ii < x.Quantity; ii++)
                energies.Add(x.RetreatEnergyTypeId);

            // We return.
            return energies;
        }).ToList();

        // We get the all attached energies to the card.
        var attachedEnergyCards = this.DetachedEnergies.ToList();

        // We check all the condition.
        foreach (var requiredEnergy in requiredEnergies.OrderByDescending(x => x != EnergyTypes.Colorless))
        {
            // We are looking for the energy type exists in attached energy types..
            var retreatData = attachedEnergyCards.Find(x => x.EnergyCard.CardData.MetaData.EnergyTypeId == requiredEnergy);

            // if it is colourless just search for any.
            if (retreatData == null && requiredEnergy == EnergyTypes.Colorless)
                retreatData = attachedEnergyCards.FirstOrDefault();

            // if attached card not found just return.
            if (retreatData == null)
            {
                neededEnergies.Add(requiredEnergy);
                continue;
            }

            // if exists we will return.
            attachedEnergyCards.Remove(retreatData);
        }

        return neededEnergies;
    }
    public bool IsDetachable()
    {
        // We get the all attached energies to the card.
        var attachedEnergyCards = this.DetachedEnergies.ToList();

        // We check all the condition.
        foreach (CardRetreatCostDTO retreat in this.ShownCard.CardData.MetaData.CardRetreatCosts.OrderByDescending(x => x.RetreatEnergyTypeId != EnergyTypes.Colorless))
        {
            // We loop as much as required.
            for (int ii = 0; ii < retreat.Quantity; ii++)
            {
                // We are looking for the energy type exists in attached energy types..
                var retreatData = attachedEnergyCards.Find(x => x.EnergyCard.CardData.MetaData.EnergyTypeId == retreat.RetreatEnergyTypeId);

                // if it is colourless just search for any.
                if (retreatData == null && retreat.RetreatEnergyTypeId == EnergyTypes.Colorless)
                    retreatData = attachedEnergyCards.FirstOrDefault();

                // if attached card not found just return.
                if (retreatData == null)
                    return false;

                // if exists we will return.
                attachedEnergyCards.Remove(retreatData);
            }
        }

        // if reaches here it is possible.
        return true;
    }
    public IEnumerator SwitchTwoCard(CardController targetCard)
    {
        // We activate the aler.
        GOABenchedMonsterAlert.SetActive(false);

        // We will no more wait for swa.
        this.IsRetreating = false;

        // We get the detached card ids.
        List<int> detachedEnergyCardIds = DetachedEnergies.Select(x => x.EnergyCard.CardData.UniqueCardID).ToList();

        // We detach the card.
        DetachedEnergies.ForEach(detachedEnergy => ShownCard.CardEnergyAttachment.DetachCard(detachedEnergy));

        // We get the active monster.
        CardController activeMonster = targetCard.Playground.PlayerActive;

        // We switch two card.
        if (targetCard.Playground.SwitchBetweenActiveToBench(activeMonster, targetCard))
        {
            // Clefairy will goto discard automatically.
            if (activeMonster.CardData.MetaData.CardId == (int)BGCards.ClefairyDoll)
                targetCard.Playground.MoveFromBenchToDiscard(activeMonster.CardData.UniqueCardID);

            // We detach all the cards.
            foreach (CardEnergyAttachmentItemController detachedEnergy in DetachedEnergies)
            {
                // We activate the card.
                detachedEnergy.EnergyCard.gameObject.SetActive(true);

                // We add card to discard pile.
                detachedEnergy.EnergyCard.Playground.AddToDiscard(detachedEnergy.EnergyCard);

                // We wait some seconds.
                yield return new WaitForSeconds(.3f);
            }

            // We clear all the energies.
            DetachedEnergies.Clear();
            
            // We remove retreat state.
            activeMonster.Playground.IsRetreatable = false;

            // We send to server the info.
            BattleGameController.Instance.SendGameAction(BattleGameActions.RetreatAMonster, new BGRetreatACardDTO
            {
                ReCardId = activeMonster.CardData.UniqueCardID,
                TargetCardId = targetCard.CardData.UniqueCardID,
                DtCards = detachedEnergyCardIds
            });
        }
    }

}
