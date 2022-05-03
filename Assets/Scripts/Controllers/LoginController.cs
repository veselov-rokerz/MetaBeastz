using Assets.Scripts.GSSocket.DTO;
using System;
using System.Collections;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    public static LoginController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// User login informations.
    /// </summary>
    public LoginResponseDTO LoginData { get; private set; }

    [Header("Temp login token.")]
    public string EditorLoginToken;

    [Header("Returns true when user logged in.")]
    public bool IsLoggedIn;

    IEnumerator Start()
    {
        // We wait until connect.
        yield return new WaitUntil(() => GSController.Instance.IsConnected);

        // if device is editor.
        if (DeviceController.Instance.DeviceType == DeviceController.Devices.Editor)
        {
            // We try to login.
            LoginController.Instance.LoginToServer(EditorLoginToken);
        }
        else
        {
            // We try to login.
            LoginController.Instance.LoginToServer(EditorLoginToken);
        }
    }

    // Start is called before the first frame update
    public void LoginToServer(string wallet, Action<bool> onCallBack = null)
    {
        // We login.
        GSController.Instance.SendToServer(GSMethods.Login, new LoginRequestDTO
        {
            Wallet = wallet
        }, (GSSocketResponse response) =>
        {
            // We update login informations.
            if (response.IsSuccess)
                LoginData = response.GetData<LoginResponseDTO>();

            // Set as true when logged in.
            IsLoggedIn = response.IsSuccess;

            // We callback the listener.
            if (onCallBack != null)
                onCallBack.Invoke(IsLoggedIn);
        });
    }
}
