using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.GSSocket.DTO;
using UnityEngine;
using UnityEngine.UI;

public class CardEnergyAttachmentItemController : MonoBehaviour
{
    [Header("We store the energy card.")]
    public CardController EnergyCard;

    private void Start()
    {
        // We add the energy comp.
        GetComponent<Button>().onClick.AddListener(() => OnClickEnergy());
    }

    public void LoadEnergyData(CardController cardData)
    {
        // We store the card informations.
        this.EnergyCard = cardData;

        // We store energy icon.
        Sprite energyIcon;
        if (cardData.CardData.MetaData.CardTypeId == CardTypes.Pokemon)
            energyIcon = ResourceController.Instance.GetEnergyType(this.EnergyCard.TempEnergy);
        else
            energyIcon = ResourceController.Instance.GetEnergyType(this.EnergyCard.CardData.MetaData.EnergyTypeId);

        // We change the energy.
        GetComponent<Image>().sprite = energyIcon;
    }

    public void LoadTempEnergy(EnergyTypes energy)
    {
        // We change the energy.
        GetComponent<Image>().sprite = ResourceController.Instance.GetEnergyType(energy);
    }

    public void ClearTempEnergy()
    {
        LoadEnergyData(this.EnergyCard);
    }

    public void OnClickEnergy()
    {
        // if selected monster detail wasnt active just return.
        if (EnergyCard.EnergyAttachedTo.CardState != BGCardStates.Action)
            return;

        // if detail view is open and click on it we will detach the view.
        if (CardRetreatEnergyDetachController.Instance.IsDetachActive)
        {
            // We detach the card.
            CardRetreatEnergyDetachController.Instance.DetachAnEnergyCard(this);
        }

        // if detail view is open and click on it we will detach the view.
        if (CardEnergyDetachController.Instance.IsDetachActive)
        {
            // We detach the card.
            CardEnergyDetachController.Instance.DetachAnEnergyCard(this);
        }
    }

}
