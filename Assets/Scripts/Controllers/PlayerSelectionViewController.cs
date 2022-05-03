using Assets.Scripts.BSSocket.DTO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionViewController : MonoBehaviour
{
    public static PlayerSelectionViewController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("We will activate view when its required.")]
    public GameObject GOView;

    [Header("When items will be displayed this is the prefab we clone.")]
    public GameObject GOSelectionItem;

    [Header("We will print selection field.")]
    public Transform TRSelection;

    public void LoadCards(BGCardDTO[] cards)
    {
        // if no  card selected just return.
        if (cards.Length == 0) return;

        // We activate the view.
        GOView.SetActive(true);

        // We remove older items.
        TRSelection.RemoveAllChildsOfTransform();

        // We loop all the cards.
        foreach(BGCardDTO card in cards)
        {
            // We create the selected item.
            GameObject selectedItem = Instantiate(GOSelectionItem, TRSelection);

            // We show the image.
            selectedItem.GetComponent<Image>().sprite = ResourceController.Instance.GetCardSprite(card.MetaData.CardId);
        }

        // if exists we will remove older one.
        CancelInvoke();

        // We will close after .
        Invoke("CloseAuto", 2.5f);
    }

    private void CloseAuto()
    {
        // We remove older items.
        TRSelection.RemoveAllChildsOfTransform();

        // We close the view.
        GOView.SetActive(false);
    }

}
