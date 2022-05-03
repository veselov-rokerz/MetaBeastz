using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.Controllers.Attacks;
using Assets.Scripts.Controllers.Trainers.Trainers;
using Assets.Scripts.GSSocket.DTO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardEnergyDetachController : MonoBehaviour
{
    public static CardEnergyDetachController Instance { get; private set; }
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
    /// We store the selected attack data.
    /// </summary>
    public int AttackID { get; private set; }

    /// <summary>
    /// We store the played trainer card id.
    /// </summary>
    public int TrainerID { get; private set; }

    /// <summary>
    /// When detach is active.
    /// </summary>
    public bool IsDetachActive => GODetachEnergyView?.activeSelf == true;

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

    [Header("Is it forced to detach.")]
    public bool IsForced;

    [Header("if all cost must select then true.")]
    public bool IsAllRequired = true;

    public void ShowEnergyDetachViewForTrainer(CardController movementItem, CardController trainerCard, bool isAllRequired)
    {
        // We check is all required.
        this.IsAllRequired = isAllRequired;

        // We update the trainer id.
        this.TrainerID = trainerCard.CardData.MetaData.CardId;

        // We show the energy detach view.
        ShowEnergyDetachView(movementItem, true);
    }
    public void ShowEnergyDetachViewForAttack(CardController movementItem, AttackDTO attackData, bool isForced)
    {
        // We set the attack data.
        this.AttackID = attackData.AttackId;

        // We show the view.
        ShowEnergyDetachView(movementItem, isForced);
    }
    private void ShowEnergyDetachView(CardController movementItem, bool isForced)
    {
        // We change the force state.
        IsForced = isForced;

        // We set the shown card.
        this.ShownCard = movementItem;

        // We make sure button interactable is disabled.
        BTNOk.interactable = false;

        // We clear the list.
        DetachedEnergies.Clear();

        // We show the detach view.
        GODetachEnergyView.SetActive(true);

        // We refresh all the attachments.
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

        // We get the cost.
        List<EnergyTypes> requiredEnergies = GetEnergyCost();

        // We instantiate retreat cost.
        requiredEnergies.ForEach(e =>
        {
            // We create an energy item.
            GameObject energyItem = Instantiate(DetachEnergyItem, TRDetachEnergies);

            // We are looking for the detach.
            // First we are looking for same type.
            CardEnergyAttachmentItemController detachedEnergy = detachedEnergies.Find(x => x.EnergyCard.CardData.MetaData.EnergyTypeId == e);

            // if not exists we are going to look for it. We make sure the retreat cost is colorless.
            // if type is colorless then we are search for any other type.
            if (detachedEnergy == null && e == EnergyTypes.Colorless)
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

                // We update the image.
                energyItem.GetComponent<Image>().sprite = ResourceController.Instance.GetEnergyType(detachedEnergy.EnergyCard.CardData.MetaData.EnergyTypeId);

                // We remove from the list prevent dublications.
                detachedEnergies.Remove(detachedEnergy);
            }
            else
            {
                // We make detachable.
                energyItem.GetComponent<Button>().interactable = false;
            }
        });
    }
    public void AttachRetreatEnergyCostAuto()
    {
        // We get the attached energy cards.
        var attachedCards = this.ShownCard.CardEnergyAttachment.AttachedEnergies.ToList();

        // We get the required energy cost.
        List<EnergyTypes> requiredEnergies = this.GetEnergyCost();

        // We will attach all the cost to the view.
        requiredEnergies.ForEach(e =>
        {
            // We find the attached card.
            var autoAttachedCard = attachedCards.Find(x => x.Item1.EnergyCard.CardData.MetaData.EnergyTypeId == e);

            // if type is colorless then we are search for any other type.
            if (autoAttachedCard == null && e == EnergyTypes.Colorless)
                autoAttachedCard = attachedCards.FirstOrDefault();

            // if exists we will remove it.
            if (autoAttachedCard != null)
            {
                // We remove it.
                attachedCards.Remove(autoAttachedCard);

                // We add to detached energies.
                this.DetachedEnergies.Add(autoAttachedCard.Item1);
            }
        });

        // we refresh all the deteach view.
        RefreshEnergyDetachView();
    }
    public void DetachAnEnergyCard(CardEnergyAttachmentItemController energyItem)
    {
        // if not a valid energy just return.
        if (!IsValidEnergy(energyItem.EnergyCard.CardData.MetaData.EnergyTypeId))
            return;

        // We have to detach if not already detached..
        if (this.DetachedEnergies.Contains(energyItem))
            return;

        // We detach.
        this.DetachedEnergies.Add(energyItem);

        // We hide the energy item.
        energyItem.gameObject.SetActive(false);

        // We refresh the view.
        RefreshEnergyDetachView();
    }
    public void CloseDetachView()
    {
        // We close the energy view.
        this.GODetachEnergyView.SetActive(false);
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

    public bool IsDetachable()
    {
        // if all of them is not required just return true.
        if (!IsAllRequired) return true;

        // We get the required energy cost for current attack.
        List<EnergyTypes> requiredEnergies = this.GetEnergyCost();

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
                return false;

            // if exists we will return.
            attachedEnergyCards.Remove(retreatData);
        }

        // if reaches here it is possible.
        return true;
    }

    public List<EnergyTypes> GetRequiredEnergies()
    {
        // Needed energy.
        List<EnergyTypes> neededEnergies = new List<EnergyTypes>();

        // We get the required energy cost for current attack.
        List<EnergyTypes> requiredEnergies = this.GetEnergyCost();

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

    public List<EnergyTypes> GetEnergyCost()
    {
        // We will store in the list.
        List<EnergyTypes> requiredEnergies = new List<EnergyTypes>();

        // We add cost depends on attack type.
        if (this.AttackID > 0)
        {
            // We switch between attacks.
            switch ((BGAttacks)this.AttackID)
            {
                case BGAttacks.ATKFireSpin:
                    {
                        // We add two fire energy types.
                        requiredEnergies.AddRange(new EnergyTypes[] { EnergyTypes.Colorless, EnergyTypes.Colorless });
                    }
                    break;
                case BGAttacks.ATKWhirlpool:
                    {
                        // We add two fire energy types.
                        requiredEnergies.Add(EnergyTypes.Colorless);
                    }
                    break;
            }
        }

        // if this card is shown its because trainer card played.
        if (this.TrainerID > 0)
        {
            switch ((BGTrainers)this.TrainerID)
            {
                case BGTrainers.TRSuperPotion:
                    {
                        requiredEnergies.Add(EnergyTypes.Colorless);
                    }
                    break;
                case BGTrainers.TRSuperEnergyRemoval:
                    {
                        if (TrainerController.Instance.ActiveTrainer.ResponseData.ActionNumber == 1)
                        {
                            requiredEnergies.Add(EnergyTypes.Colorless);
                        }
                        else if (TrainerController.Instance.ActiveTrainer.ResponseData.ActionNumber == 2)
                        {
                            requiredEnergies.Add(EnergyTypes.Colorless);
                            requiredEnergies.Add(EnergyTypes.Colorless);
                        }
                    }
                    break;
                case BGTrainers.TREnergyRemoval:
                    {
                        requiredEnergies.Add(EnergyTypes.Colorless);
                    }
                    break;
            }
        }

        // We return the required energies.
        return requiredEnergies;
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

    public void OnClickOk()
    {
        // We update the forced state.
        this.IsForced = false;

        // We make sure it is the same.
        if (this.AttackID > 0)
        {
            // We get the attack data.
            IAttack attackData = AttackController.Instance.ActiveAttack;

            // We switch between attacks.
            switch ((BGAttacks)this.AttackID)
            {
                case BGAttacks.ATKFireSpin:
                    {
                        // We get the fire spin data.
                        ATKFireSpin fireSpin = (ATKFireSpin)attackData;

                        // We store detached energies.
                        int[] detachedIds = DetachedEnergies.Select(x => x.EnergyCard.CardData.UniqueCardID).ToArray();

                        // We detach all the energies.
                        fireSpin.AttackManual(detachedIds);
                    }
                    break;
                case BGAttacks.ATKWhirlpool:
                    {
                        // We close the select an energy.
                        BattleNotiController.Instance.GODetachOneEnergyCardAlert.SetActive(false);

                        // We get the whirlpool data.
                        ATKWhirlpool whirlpool = (ATKWhirlpool)attackData;

                        // We store detached energies.
                        int[] detachedIds = DetachedEnergies.Select(x => x.EnergyCard.CardData.UniqueCardID).ToArray();

                        // We detach all the energies.
                        whirlpool.AttackManual(detachedIds);
                    }
                    break;
                case BGAttacks.ATKHyperBeam:
                    {
                        // We close the select an energy.
                        BattleNotiController.Instance.GODetachOneEnergyCardAlert.SetActive(false);

                        // We get the hyper beam data.
                        ATKHyperBeam hyperBeam = (ATKHyperBeam)attackData;

                        // We store detached energies.
                        int[] detachedIds = DetachedEnergies.Select(x => x.EnergyCard.CardData.UniqueCardID).ToArray();

                        // We detach all the energies.
                        hyperBeam.AttackManual(detachedIds);
                    }
                    break;
            }
        }

        // We make sure the trainer is active.
        if (this.TrainerID > 0)
        {
            // We get the active trainerdata.
            ITrainer trainerData = TrainerController.Instance.ActiveTrainer;

            // We switch between trainers.
            switch ((BGTrainers)trainerData.PlayedCard.CardData.MetaData.CardId)
            {
                case BGTrainers.TRSuperPotion:
                    {
                        // We get the super potion data.
                        TRSuperPotion sp = (TRSuperPotion)trainerData;

                        // We store detached energies.
                        int[] detachedIds = DetachedEnergies.Select(x => x.EnergyCard.CardData.UniqueCardID).ToArray();

                        // We update detached energies.
                        trainerData.RequestModel.TCardIDs = detachedIds.ToList();

                        // We play it.
                        trainerData.Play(false);

                    }
                    break;
                case BGTrainers.TRSuperEnergyRemoval:
                    {
                        // We get the super potion data.
                        TRSuperEnergyRemoval sp = (TRSuperEnergyRemoval)trainerData;

                        // We store detached energies.
                        int[] detachedIds = DetachedEnergies.Select(x => x.EnergyCard.CardData.UniqueCardID).ToArray();

                        // We update detached energies.
                        trainerData.RequestModel.TCardIDs = detachedIds.ToList();

                        // We play it.
                        trainerData.Play(false);
                    }
                    break;
                case BGTrainers.TREnergyRemoval:
                    {
                        // We get the super potion data.
                        TREnergyRemoval sp = (TREnergyRemoval)trainerData;

                        // We store detached energies.
                        int[] detachedIds = DetachedEnergies.Select(x => x.EnergyCard.CardData.UniqueCardID).ToArray();

                        // We update detached energies.
                        trainerData.RequestModel.TCardIDs = detachedIds.ToList();

                        // We play it.
                        trainerData.Play(false);
                    }
                    break;
            }
        }

        // We deactivate the action. if in bench
        this.ShownCard.Playground.DeactivateAction();
    }
}
