using Assets.Scripts.GSSocket.DTO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UserCardService : MonoBehaviour
{
    public static UserCardService Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    public void LoadUserCards(Action<List<UserCardDTO>> onLoaded = null)
    {
        GSController.Instance.SendToServer(GSMethods.UserCards, string.Empty, (GSSocketResponse response) =>
        {
            // We set the cards.
            List<UserCardDTO> userCards = response.GetDataList<UserCardDTO>();

            // We say it is loaded.
            if (onLoaded != null) onLoaded.Invoke(userCards);
        });
    }
}
