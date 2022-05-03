using Assets.Scripts.GSSocket.DTO;
#if UNITY_EDITOR
using ParrelSync;
#endif
using UnityEngine;

public class SceneDataController : MonoBehaviour
{
    public static SceneDataController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    [Header("Battle game server data to connect.")]
    public BattleServerDataDTO BattleServerData;
    
    [Header("When game server not required.")]
    public bool SoloModeOnly;

    public string GetToken()
    {
        if (SoloModeOnly)
        {
#if UNITY_EDITOR
            if (ClonesManager.IsClone())
                return "6c20c93f-2a90-489a-b5c9-028fcf5b1d01";
#endif
            return "abfbee28-0f28-4dfa-a114-b8e2306346d5";
        }else
        {
            return BattleServerData.AccessToken;
        }
    }
}
