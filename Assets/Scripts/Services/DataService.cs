using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataService : MonoBehaviour
{
    public static DataService Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("All services must be implemented with IService.")]
    [Header("IServices will be loaded on startup.")]
    public List<MonoBehaviour> ServicesOnStartup;

    /// <summary>
    /// if the game loaded returns true.
    /// </summary>
    public bool IsServiceLoaded => ServicesOnStartup.Count + 1 == CurrentLoadState;

    [Header("Active loading state of services.")]
    public int CurrentLoadState;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // We wait until connect.
        yield return new WaitUntil(() => GSController.Instance.IsConnected);

        // We wait until we login.
        yield return new WaitUntil(() => LoginController.Instance.IsLoggedIn);

        // We load all the services.
        foreach (MonoBehaviour service in ServicesOnStartup)
        {
            // We get the service.
            BaseService baseService = (BaseService)service;

            // We make sure it is implemented with iService
            if (baseService != null)
                baseService.LoadOnStartUp(() => CurrentLoadState++);
            else // We print the log.
                Debug.LogError($"{service.name} was not inherited from BaseService");
        }

        // We always have one state.
        CurrentLoadState++;

        // We make sure all of them is loaded.
        yield return new WaitUntil(() => ServicesOnStartup.Count == CurrentLoadState);

        // We print game is ready to play.
        Debug.Log("Game is ready to play");
    }
}
