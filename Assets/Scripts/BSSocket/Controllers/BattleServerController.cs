using Assets.Scripts.BSSocket.Extends;
using Assets.Scripts.Socket.Interfaces;
using NativeWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BattleServerController : MonoBehaviour, IDisposable
{
    public static BattleServerController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (Application.isEditor)
            Application.runInBackground = true;
    }

    /// <summary>
    /// When the resources are dispoed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Web socket to connect client.
    /// </summary>
    public WebSocket WebSocket { get; private set; }

    [Header("Server connection url.")]
    public string ServerUrl;

    /// <summary>
    /// Sent socket requests.
    /// </summary>
    public List<IBSSocketRequest> RequestList = new List<IBSSocketRequest>();

    /// <summary>
    /// Waiting for the excetuing responses.
    /// </summary>
    public List<BSSocketResponse> ExecutableResponse = new List<BSSocketResponse>();

    /// <summary>
    /// Un expected response listeners.
    /// </summary>
    public Action<BSSocketResponse> OnUnExpectedDataReceived { get; set; }

    /// <summary>
    /// Returns true when the socket is connected.
    /// </summary>
    public bool IsConnected
    {
        get
        {
            if (WebSocket != null)
                return WebSocket.State == WebSocketState.Open;
            return false;
        }
    }

    public async void Start()
    {
        //We store the server url.
        this.ServerUrl = SceneDataController.Instance.BattleServerData.Ip;

        // We create a web socket.
        WebSocket = new WebSocket(ServerUrl);

        // We are listening to server.
        WebSocket.OnOpen += WebSocket_OnOpen;
        WebSocket.OnError += WebSocket_OnError;
        WebSocket.OnClose += WebSocket_OnClose;
        WebSocket.OnMessage += WebSocket_OnMessage;

        // We connect to serveri.
        await WebSocket.Connect();

        // We ping frequently.
        if (!IsDisposed)
            InvokeRepeating("Ping", 0, 5);
    }

    public void Ping()
    {
        // if already disposed just return.
        if (this.IsDisposed) return;

        // if connection invalid we just wait until connect.
        if (WebSocket.State != WebSocketState.Open)
            WebSocket.Connect().Wait();

        // We ping the server.
        SendToServer(BSMethods.Ping, string.Empty);
    }

    private void WebSocket_OnMessage(byte[] data)
    {
        // We get thre response data.
        BSSocketResponse responseData = Encoding.UTF8.GetString(data).ToObject<BSSocketResponse>();

        // We add to waiting list.
        lock (ExecutableResponse)
            ExecutableResponse.Add(responseData);
    }

    private void WebSocket_OnClose(WebSocketCloseCode closeCode)
    {
    }

    private void WebSocket_OnError(string errorMsg)
    {
    }

    private void WebSocket_OnOpen()
    {
    }

    public void SendToServer<T>(BSMethods method, T data, Action<BSSocketResponse> responseAction = null) where T : class
    {
        BSSocketRequest<T> request = new BSSocketRequest<T>(method, data, responseAction);

        // All request that wait response is going to be added to list.
        lock (Instance.RequestList)
            Instance.RequestList.Add(request);

        // We send data to server.
        Instance.WebSocket.SendText(request.ToJson());
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        WebSocket.DispatchMessageQueue();
#endif

        // if no data exists just return.
        if (ExecutableResponse.Count == 0)
            return;

        // Lock the response list.
        lock (ExecutableResponse)
        {
            // We are looking for each request are they owner of response data.
            foreach (BSSocketResponse responseData in ExecutableResponse.Where(x => x.UnExpectedMethods == BSUnExpectedMethods.None))
            {
                // We search for all the requests.
                foreach (IBSSocketRequest request in RequestList)
                {
                    // if removed then its means we was waiting for it.
                    if (request.OnResponseReceived(responseData))
                        break;
                }
            }

            // We execute all unexpected actions.
            foreach (BSSocketResponse responseData in ExecutableResponse.Where(x => x.UnExpectedMethods != BSUnExpectedMethods.None))
            {
                if (OnUnExpectedDataReceived != null)
                    OnUnExpectedDataReceived.Invoke(responseData);
            }

            // We clear all the actions.
            ExecutableResponse.Clear();
        }
    }

    private async void OnApplicationQuit()
    {
        // We dispose resources.
        this.Dispose();

        // We close the sockets.
        await WebSocket.Close();
    }

    public void Dispose()
    {
        // if already disposed just return.
        if (IsDisposed) return;

        // We tell released resources.
        IsDisposed = true;

        // We stop all the invokes.
        CancelInvoke();
    }
}
