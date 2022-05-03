using Assets.Scripts.Enums;
using Assets.Scripts.GSSocket.DTO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UserDeckService : MonoBehaviour
{
    public static UserDeckService Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("When true its going to use predefined decks.")]
    public bool UseThemeDecks;

    [Header("The deck selected by users.")]
    public Decks SelectedDeck;

    public void LoadUserDecks(Action<List<UserDeckDTO>> onLoaded = null)
    {
        if (UseThemeDecks)
        {
            // We store the list.
            List<UserDeckDTO> decks = new List<UserDeckDTO>();
            
            // We add all the included decks into the list.
            foreach (Decks deck in Enum.GetValues(typeof(Decks)))
            {
                decks.Add(new UserDeckDTO
                {
                    DeckName = deck.ToString(),
                    UserDeckId = (int)deck,
                });
            }

            // we invoke back.
            if (onLoaded != null) onLoaded.Invoke(decks);
        }
        else
        {
            GSController.Instance.SendToServer(GSMethods.UserDecks, string.Empty, (GSSocketResponse response) =>
            {
                // We set the users decks.
                List<UserDeckDTO> userDecks = response.GetDataList<UserDeckDTO>();

                // We trigger the listenrs.
                if (onLoaded != null)
                    onLoaded.Invoke(userDecks);
            });
        }
    }
}
