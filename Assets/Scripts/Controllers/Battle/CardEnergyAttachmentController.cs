using System;
using System.Collections.Generic;
using UnityEngine;

public class CardEnergyAttachmentController : MonoBehaviour
{
    [Header("We will use this energy item to instantiate.")]
    public GameObject GoEnergyItem;

    [Header("We will add energies into the field.")]
    public Transform TREnergyContent;

    /// <summary>
    /// Attached energies.
    /// </summary>
    public List<Tuple<CardEnergyAttachmentItemController, CardController>> AttachedEnergies
        = new List<Tuple<CardEnergyAttachmentItemController, CardController>>();

    public void AttachAnEnergyToMonsterCard(CardController energyCard)
    {
        // if card already attached return.
        if (AttachedEnergies.Exists(x => x.Item2.CardData.UniqueCardID == energyCard.CardData.UniqueCardID)) return;

        // We create an energy item.
        GameObject energyItem = Instantiate(GoEnergyItem, TREnergyContent);

        // We load the energy data.
        CardEnergyAttachmentItemController energyAttachmentItem = energyItem.GetComponent<CardEnergyAttachmentItemController>();

        // We load the data.
        energyAttachmentItem.LoadEnergyData(energyCard);

        // We add the attached list.
        AttachedEnergies.Add(new Tuple<CardEnergyAttachmentItemController, CardController>(energyAttachmentItem, energyCard));
    }

    public void DetachAll()
    {
        // We clear the list.
        AttachedEnergies.Clear();

        // We remove all the energies from the card.
        TREnergyContent.RemoveAllChildsOfTransform();
    }

    public void DetachCard(CardEnergyAttachmentItemController detachedEnergy)
    {
        // We remove the attached card.
        AttachedEnergies.RemoveAll(x => x.Item2 == detachedEnergy.EnergyCard);

        // We remove the detached card.
        Destroy(detachedEnergy.gameObject);
    }

    public void DetachCard(int cardId)
    {
        // We find the energy.
        var detachedEnergy = AttachedEnergies.Find(x => x.Item1.EnergyCard.CardData.UniqueCardID == cardId);

        // We remove the attached card.
        AttachedEnergies.Remove(detachedEnergy);

        // We remove the detached card.
        Destroy(detachedEnergy.Item1.gameObject);
    }
}
