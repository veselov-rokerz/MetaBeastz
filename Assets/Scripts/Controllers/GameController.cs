using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("True when the game ready to play.")]
    public bool IsGameLoaded;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => DataService.Instance.IsServiceLoaded);

        // We set the game as loaded.
        IsGameLoaded = true;
    }
}

