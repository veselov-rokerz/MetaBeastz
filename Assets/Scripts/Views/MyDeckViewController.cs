using Assets.Scripts.GSSocket.DTO;
using UnityEngine;
using UnityEngine.UI;

public class MyDeckViewController : MonoBehaviour
{
    [Header("My deck item to print out decks.")]
    public GameObject GOMyDeckItem;

    [Header("Deck content where we will load.")]
    public ScrollRect SVMyDeckContent;

    [Header("When we are loading its going to active.")]
    public GameObject GOLoading;

    public void LoadMyDecks()
    {
        // We remove all the children.
        SVMyDeckContent.content.RemoveAllChildsOfTransform();

        // We activate loading.
        GOLoading.SetActive(true);

        // We loop all the deck items.
        UserDeckService.Instance.LoadUserDecks(userDecks =>
        {
            // We remove all the children.
            SVMyDeckContent.content.RemoveAllChildsOfTransform();

            // We close loading view.
            GOLoading.SetActive(false);

            // We loop all the deck.
            foreach (UserDeckDTO myDeck in userDecks)
            {
                // We create a deck item.
                GameObject myDeckItem = Instantiate(GOMyDeckItem, SVMyDeckContent.content);

                // We load the deck details.
                myDeckItem.GetComponent<MyDeckViewItemController>().LoadDeckDetails(myDeck);
            }
        });
    }
}
