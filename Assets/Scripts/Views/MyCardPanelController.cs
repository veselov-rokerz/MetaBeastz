using System.Collections;
using UnityEngine;

public class MyCardPanelController : MonoBehaviour
{
    public static MyCardPanelController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("My card view to show user cards.")]
    public MyCardViewController MyCardView;

    [Header("My deck view shows user decks.")]
    public MyDeckViewController MyDeckView;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // We wait until loaded.
        yield return new WaitUntil(() => DataService.Instance.IsServiceLoaded);

        // By Default we will load cards.
        OnClickMyDecks();
    }
    
    private void CloseAllViews()
    {
        // We deactivate the deck view.
        MyDeckView.gameObject.SetActive(false);

        // We deactivate the card view.
        MyCardView.gameObject.SetActive(false);
    }

    public void OnClickMyCards()
    {
        // We close older views.
        CloseAllViews();

        // View is going to be active.
        MyCardView.gameObject.SetActive(true);

        // We load user cards.
        MyCardView.LoadUserCardsInToView();
    }

    public void OnClickMyDecks()
    {
        // We close older views.
        CloseAllViews();

        // We activate the view.
        MyDeckView.gameObject.SetActive(true);

        // We load all the decks.
        MyDeckView.LoadMyDecks();
    }

    public void OnClickClose()
    {
        Destroy(gameObject);
    }

}
