using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleNotiController : MonoBehaviour
{
    public static BattleNotiController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("Ask for player to select a side.")]
    public GameObject GOAskForFlipCoin;

    [Header("Other player is going to wait until player select one side.")]
    public GameObject GOAskForWaitFlipCoin;

    [Header("if opponent starts the game this view is going to be active.")]
    public GameObject GOYourOpponentStartGameInfo;

    [Header("if player starts the game this view is going to be active.")]
    public GameObject GOYouStartGameInfo;

    [Header("When player play a card ready button will be activated.")]
    public GameObject GODone;

    [Header("When player monster forced to select.")]
    public GameObject GOForceToSelectActiveCard;

    [Header("When the game is over this panel is going to show.")]
    public GameObject GoGameOverPanel;

    [Header("When required to select an opponent benched monster.")]
    public GameObject GOSelectOpponentBenchMonster;

    [Header("When required to select player benched monster by opponent.")]
    public GameObject GOSelectOpponentFromYourBenchMonster;

    [Header("We wait for the opponent to select a monster.")]
    public GameObject GOYourOpSelectingAMonster;

    [Header("Please select a benched monster..")]
    public GameObject GOSelectABenchedAMonster;

    [Header("When required the detach from the target.")]
    public GameObject GODetachOneEnergyCardAlert;

    [Header("When player has to select an opponent attack.")]
    public GameObject GOSelectYourOpponentAttack;

    [Header("Select two card from your hand.")]
    public GameObject GOSelect2CardFromYourHand;

    [Header("We ask for select a card from hand.")]
    public GameObject GOSelect1CardFromYourHand;

    [Header("When player has to select a monster.")]
    public GameObject GOSelectAMonster;
    
    [Header("When player has to select a monster.")]
    public GameObject GOSelectAnotherMonster;

    [Header("Select your opponent monster.")]
    public GameObject GOSelectYourOpponentMonster;

    [Header("Select one monster card from your hand.")]
    public GameObject GOSelect1MonsterCardFromYourHand;

    [Header("Please select second stage evolution")]
    public GameObject GOSelectEvoluteOneOfYourMonstersToSecondStage;

    [Header("When water energy type can be attached we will show alert.")]
    public GameObject GOAttachOneWaterEnergyTypeToAMonster;

    [Header("When player hand is mulligan we will show the alert")]
    public GameObject GOPlayerMulligan;

    [Header("When you want to cancel ability this button will use..")]
    public Button BTNCancelAbility;

    public void AskForTheHeadsOrTails()
    {
        // if current player is start player then we will ask for the select a side.
        if (BattleGameController.Instance.GameStartData.SP == BattleGameController.Instance.GameStartData.PColor)
            GOAskForFlipCoin.SetActive(true);
        else // Otherwise we just flip the coin.
            GOAskForWaitFlipCoin.SetActive(true);
    }
    public void HideTheHeadsOrTails()
    {
        // We disable all the releated views.
        GOAskForFlipCoin.SetActive(false);
        GOAskForWaitFlipCoin.SetActive(false);
    }

    public void FlipHeadsCoinForStart()
    {
        BattleGameController.Instance.SendGameAction(BattleGameActions.FlipCoinHeads);
    }
    public void FlipTailsCoinForStart()
    {
        BattleGameController.Instance.SendGameAction(BattleGameActions.FlipCoinTails);
    }

    public void ExecuteFlipCoin(BGGameStartFlipDTO flipData)
    {
        // We close the quesiton panel.
        BattleNotiController.Instance.HideTheHeadsOrTails();

        // if the start player itself then we will show the selection.
        if (BattleGameController.Instance.GameStartData.SP == BattleGameController.Instance.GameStartData.PColor)
            BattleGameController.Instance.Player.PlayerCoin.FlipCoin(() => DrawCards(flipData), flipData.Result);
        else // if starting player is second player.
            BattleGameController.Instance.Opponent.PlayerCoin.FlipCoin(() => DrawCards(flipData), flipData.Result);
    }

    public void DrawCards(BGGameStartFlipDTO flipData)
    {
        // if active player is the start player.
        if (BattleGameController.Instance.GameStartData.SP == BattleGameController.Instance.GameStartData.PColor)
        {
            // We check if player win the coin flip.
            if (flipData.Result == flipData.Choose)
                GOYouStartGameInfo.SetActive(true);
            else
                GOYourOpponentStartGameInfo.SetActive(true);
        }
        else // if player not active player..
        {
            // We check active player win the game?
            if (flipData.Result == flipData.Choose)
                GOYourOpponentStartGameInfo.SetActive(true);
            else
                GOYouStartGameInfo.SetActive(true);
        }

        // We destroy the views.
        Destroy(GOYourOpponentStartGameInfo, 2);
        Destroy(GOYouStartGameInfo, 2);

        // We draw all the start cards.
        BattleGameController.Instance.Player.StartCoroutine(BattleGameController.Instance.Player.DrawMultipleCards(flipData.PCards, () =>
        {
            // We check mulligan state.
            StartCoroutine(BattleGameController.Instance.CheckPlayerHandIsMulligan());
        }));

        // We draw other player hidden cards.
        BattleGameController.Instance.Opponent.StartCoroutine(BattleGameController.Instance.Opponent.DrawMultipleCards(flipData.OPCards));

        // We update the start player.
        BattleGameController.Instance.GameStartData.SP = flipData.SP;

        // We update the current player.
        StartCoroutine(BattleGameController.Instance.SwitchPlayer(flipData.SP));
    }

    public void ShowDoneButton()
    {
        // if not player turn just return.
        if (!BattleGameController.Instance.IsPlayerTurn) return;

        // if forcing selecting an active monster return.
        if (BattleGameController.Instance.Player.ForceToSelectActive) return;

        // We just activate.
        GODone.SetActive(true);
    }

    public void OnClickDone()
    {
        // We send to server as we are ready.
        BattleGameController.Instance.SendGameAction(BattleGameActions.DoneAction);

        // We activate the ready button.
        GODone.SetActive(false);
    }
    public void OnClickCancelAbility()
    {
        // We cancel the ability from the server.
        BattleGameController.Instance.SendGameAction(BattleGameActions.CancelAbility);

        // We disable the button.
        BTNCancelAbility.gameObject.SetActive(false);

        // We clear the ability.
        AbilityController.Instance.ClearAbility();

        // When the trainer card played its going to be true.
        BattleNotiController.Instance.ShowDoneButton();
    }

    public void ShowGameOverForWin()
    {
        // We show the game over panel.
        GoGameOverPanel.SetActive(true);
        GoGameOverPanel.GetComponent<TMP_Text>().text = $"You Won the game!";
    }
    public void ShowGameOverForLose()
    {
        // We show the game over panel.
        GoGameOverPanel.SetActive(true);
        GoGameOverPanel.GetComponent<TMP_Text>().text = $"You lost the game!";
    }
    public void ShowGameOverForTie()
    {
        // We show the game over panel.
        GoGameOverPanel.SetActive(true);
        GoGameOverPanel.GetComponent<TMP_Text>().text = $"TIE!";
    }
}
