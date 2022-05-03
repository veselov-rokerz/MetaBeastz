using Assets.Scripts.BSSocket.DTO;
using System.Collections;
using UnityEngine;

public class BGTurnGameController : MonoBehaviour
{
    public static BGTurnGameController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// When was the first time we apply delay to draw a card after prizes drawn..
    /// </summary>
    private bool isFirstTimeDelayApplied;

    public IEnumerator DrawACardWithinSeconds(BGDrawCardFromDeckDTO drawnData)
    {

        // Wait 2 seconds to activate turn card. For the first time!
        if (!isFirstTimeDelayApplied)
            yield return new WaitForSeconds(1.2f);
        else
            // We wait half a seconds.
            yield return new WaitForSeconds(.25f);

        // We apply delay.
        isFirstTimeDelayApplied = true;

        // We draw a card for the player.
        BattleGameController.Instance.GetPlaygroundByPlayer(drawnData.Player).DrawACard(drawnData.CardData);
    }
}
