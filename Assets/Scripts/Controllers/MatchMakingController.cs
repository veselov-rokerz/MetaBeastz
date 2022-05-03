using Assets.Scripts.Enums;
using Assets.Scripts.GSSocket.DTO;
#if UNITY_EDITOR
using ParrelSync;
#endif
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchMakingController : MonoBehaviour
{
    public static MatchMakingController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("To print passed time in seconds.")]
    public TMP_Text TXTPassedTime;

    [Header("When any error exists just print it here.")]
    public TMP_Text TXTAlert;

    private float passedTime;

    // Start is called before the first frame update
    void Start()
    {
        GSController.Instance.OnUnExpectedDataReceived += new System.Action<GSSocketResponse>((response) =>
        {
            if (response.UnExpectedMethods == GSUnExpectedMethods.MatchFound)
            {
                CancelInvoke("ShowPassedSeconds");
                TXTPassedTime.text = $"Ready";
            }

            // When the game started.
            if (response.UnExpectedMethods == GSUnExpectedMethods.BattleGameInit)
            {
                // We get the server informations.
                SceneDataController.Instance.BattleServerData = response.GetData<BattleServerDataDTO>();

                // We load the scene.
                SceneManager.LoadScene(1);
            }
        });
    }

    public void OnClickMatch()
    {
        if (!Enum.IsDefined(typeof(Decks), UserDeckService.Instance.SelectedDeck))
            TXTAlert.text = $"PLEASE SELECT A DECK";
        else
            TXTAlert.text = string.Empty;

        MatchRequestDTO requestData = new MatchRequestDTO { DeckID = (int)UserDeckService.Instance.SelectedDeck };
        
        // We look timer.
        InvokeRepeating("ShowPassedSeconds", 0, 1);

        // We send it to server.
        GSController.Instance.SendToServer(GSMethods.Matchmaking, requestData, (response) =>
          {
              if (!response.IsSuccess)
              {
                  CancelInvoke("ShowPassedSeconds");
                  TXTPassedTime.text = $"";
              }
          });
    }

    public void ShowPassedSeconds()
    {
        // We activate the timer.
        TXTPassedTime.gameObject.SetActive(true);

        // We update the passed time text.
        TXTPassedTime.text = TimeSpan.FromSeconds(passedTime).ToString(@"mm\:ss");

        // We increased the passed time.
        passedTime += 1;
    }

    public void HidePassedSeconds()
    {
        // We remove the invokes.
        CancelInvoke();

        // We refresh the passed time.
        passedTime = 0;

        // We deactivate the passed time.
        TXTPassedTime.gameObject.SetActive(false);
    }
}
