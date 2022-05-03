using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.GSSocket.DTO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardRetreatController : MonoBehaviour
{
    /// <summary>
    /// Shown card informations.
    /// </summary>
    public CardController CardItem { get; set; }

    [Header("Retreat view to activate.")]
    public GameObject GORetreatView;

    [Header("We will use this energy item to instantiate.")]
    public GameObject GoEnergyItem;

    [Header("We will add energies into the field.")]
    public Transform TREnergyContent;

    [Header("Discard view.")]
    public GameObject GODiscardView;

    public void LoadRetreat(CardController cardItem)
    {
        // We store the card informations.
        this.CardItem = cardItem;

        // We activate the view.
        if (this.CardItem.CardData.MetaData.CardId != (int)BGCards.ClefairyDoll)
            this.GORetreatView.gameObject.SetActive(this.IsRetreatPossible());
        else
            this.GORetreatView.gameObject.SetActive(false);

        // if clefairy doll is is going to be activated.
        if (!BattleGameController.Instance.IsFirstRound)
            this.GODiscardView.gameObject.SetActive(this.CardItem.CardData.MetaData.CardId == (int)BGCards.ClefairyDoll);
        else
            this.GODiscardView.gameObject.SetActive(false);

        // We remove older staffs.
        TREnergyContent.RemoveAllChildsOfTransform();

        // We load all the retreat energies..
        cardItem.CardData.MetaData.CardRetreatCosts.ForEach(e =>
        {
            // We loop as much as quantity.
            for (int i = 0; i < e.Quantity; i++)
            {
                // We create an energy cost.
                GameObject energyCostItem = Instantiate(GoEnergyItem, TREnergyContent);

                // We load the retreat icon.
                energyCostItem.GetComponent<Image>().sprite = ResourceController.Instance.GetEnergyType(e.RetreatEnergyTypeId);
            }
        });
    }

    public void OnClickRetreat()
    {
        // if auto detach available then we will detach directly.
        if (IsRetreatWithoutDetachView())
        {
            // We show detach view.
            CardRetreatEnergyDetachController.Instance.ShowEnergyDetachView(this.CardItem);

            // We have to attach costs auto.
            CardRetreatEnergyDetachController.Instance.AttachRetreatEnergyCostAuto();

            // We click the ok.
            CardRetreatEnergyDetachController.Instance.OnClickOk();
        }
        else
        {
            CardRetreatEnergyDetachController.Instance.ShowEnergyDetachView(this.CardItem);
        }
    }

    public void HideRetreat()
    {
        // We remove older staffs.
        TREnergyContent.RemoveAllChildsOfTransform();

        // We deactivate the view.
        GORetreatView.gameObject.SetActive(false);

        // We deactivate the discard.
        GODiscardView.gameObject.SetActive(false);
    }

    public bool IsRetreatWithoutDetachView()
    {
        // if only one type of energy required.
        List<EnergyTypes> energyTypes = this.CardItem.CardData.MetaData.CardRetreatCosts.Select(x => x.RetreatEnergyTypeId).Distinct().ToList();

        // if it is just retreat.
        if (energyTypes.Count <= 1)
        {
            // if cost energy count is smaller than attached cards.
            if (this.CardItem.CardData.MetaData.CardRetreatCosts.Sum(x => x.Quantity) < this.CardItem.CardEnergyAttachment.AttachedEnergies.Count)
            {
                // And if attached energy types are diffrent we will ask for it.
                if (this.CardItem.CardEnergyAttachment.AttachedEnergies.Select(x => x.Item2.CardData.MetaData.EnergyTypeId).Distinct().Count() > 1)
                    return false;
            }

            // Other wise we can just return true.
            return true;
        }

        // if benched monster not exists just return.
        if (this.CardItem.Playground.PlayerBenched.Count == 0)
            return false;

        // if reaches here it is possible.
        return true;
    }

    public bool IsRetreatPossible()
    {
        // if not retreatable return false.
        if (!CardItem.Playground.IsRetreatable) return false;

        // You cant retreat if your in first raund.
        if (BattleGameController.Instance.IsFirstRound) return false;

        // Sleeping monsters cant retreat.
        if (this.CardItem.SpecialCondition == BGSpecialConditions.Asleep) return false;

        // Paralyzed monsters cant retreat.
        if (this.CardItem.SpecialCondition == BGSpecialConditions.Paralyzed) return false;

        // if benched monster not exists just return.
        if (this.CardItem.Playground.PlayerBenched.Count == 0) return false;

        // We get the all attached energies to the card.
        var attachedEnergyCards = this.CardItem.CardEnergyAttachment.AttachedEnergies.SelectMany(x =>
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
        foreach (CardRetreatCostDTO retreat in this.CardItem.CardData.MetaData.CardRetreatCosts.OrderByDescending(x => x.RetreatEnergyTypeId != EnergyTypes.Colorless))
        {
            // We loop as much as required.
            for (int ii = 0; ii < retreat.Quantity; ii++)
            {
                // if count 0 means no card exists.
                if (attachedEnergyCards.Count == 0) return false;

                // if not contains return false.
                if (!attachedEnergyCards.Contains(retreat.RetreatEnergyTypeId))
                {
                    if (retreat.RetreatEnergyTypeId == EnergyTypes.Colorless)
                        attachedEnergyCards.RemoveAt(0);
                    else
                        return false;
                }
                else
                {
                    // if exists we will return.
                    attachedEnergyCards.Remove(retreat.RetreatEnergyTypeId);
                }
            }
        }

        // if reaches here it is possible.
        return true;
    }
}
