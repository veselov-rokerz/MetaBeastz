using Assets.Scripts.BSSocket.DTO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCardViewController : MonoBehaviour
{
    public static SelectCardViewController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("Shown card item.")]
    public GameObject GOSelectItem;

    [Header("Selected")]
    public ScrollRect SRSelectContent;

    [Header("We will store shown cards.")]
    public List<BGCardDTO> ShownCards;

    [Header("We store the selected cards.")]
    public List<BGCardDTO> SelectedCards;

    public void LoadCards(List<BGCardDTO> cards, bool isForced, Action<List<BGCardDTO>> selectedCards)
    {
        // We remove all the childs.
        SRSelectContent.content.RemoveAllChildsOfTransform();

        // We loop all the cards.
        foreach (BGCardDTO card in cards)
        {
            // We create a card.
            GameObject shownCard = Instantiate(GOSelectItem, SRSelectContent.content);

            // We show the card.
            shownCard.GetComponent<Image>().sprite = ResourceController.Instance.GetCardSprite(card.MetaData.CardId);

            shownCard.GetComponent<Button>().onClick.AddListener(() =>
            {
                // When deselected.
                if (SelectedCards.Contains(card))
                {

                }else // When selected.
                {

                }
            });
        }
    }
}
