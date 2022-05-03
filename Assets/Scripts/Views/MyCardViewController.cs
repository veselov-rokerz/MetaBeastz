using Assets.Scripts.GSSocket.DTO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MyCardViewController : MonoBehaviour
{
    [Header("User card item to print screen.")]
    public GameObject GOUserCardItem;

    [Header("User card content")]
    public ScrollRect SRUserCards;

    [Header("We will show while loading.")]
    public GameObject GOLoading;

    public void LoadUserCardsInToView()
    {
        // We remove older childs.
        SRUserCards.content.RemoveAllChildsOfTransform();

        // We show the view.
        GOLoading.SetActive(true);

        // We loop all the cards.
        UserCardService.Instance.LoadUserCards((userCards) =>
        {
            // To receive card informations.
            int[] cardIds = userCards.Select(x => x.CardId).ToArray();

            // We load all the card informations.
            CardService.Instance.LoadCards(cardIds, (systemCards) =>
             {
                 // We show the view.
                 GOLoading.SetActive(false);

                 // We remove older childs.
                 SRUserCards.content.RemoveAllChildsOfTransform();

                 // We create all the cards.
                 foreach (UserCardDTO userCard in userCards)
                 {
                     // We use the cards.
                     GameObject userCardItem = Instantiate(GOUserCardItem, SRUserCards.content);

                     // We get the card information.
                     CardDTO cardData = systemCards.Find(x => x.CardId == userCard.CardId);

                     // To prevent error we make sure they are not exists.
                     if (cardData == null) continue;

                     // We load the card details from user card.
                     userCardItem.GetComponent<MyCardViewItemController>().LoadCardInfo(cardData, userCard);
                 }
             });
        });
    }
}
