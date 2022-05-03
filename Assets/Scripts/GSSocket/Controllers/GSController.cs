using Assets.Scripts.GSSocket.DTO;
using Assets.Scripts.GSSocket.Extends;
using Assets.Scripts.Socket.Interfaces;
using NativeWebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class GSController : MonoBehaviour
{
    public static GSController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Application.runInBackground = true;

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Web socket to connect client.
    /// </summary>
    public WebSocket WebSocket { get; private set; }

    [Header("Server connection url.")]
    public string ServerUrl;

    /// <summary>
    /// Sent socket requests.
    /// </summary>
    public List<IGSSocketRequest> RequestList = new List<IGSSocketRequest>();

    /// <summary>
    /// Waiting for the excetuing responses.
    /// </summary>
    public List<GSSocketResponse> ExecutableResponse = new List<GSSocketResponse>();

    /// <summary>
    /// Un expected response listeners.
    /// </summary>
    public Action<GSSocketResponse> OnUnExpectedDataReceived { get; set; }

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

    /// <summary>
    /// Configuration.
    /// </summary>
    public ConfigDTO Configuration { get; set; }

    private async void Start()
    {
        // We make sure its not editor.
        if (DeviceController.Instance.DeviceType != DeviceController.Devices.Editor)
        {
            // We read configuration file
            string configFile = $"{Application.dataPath}/../config.json";
            string configContent = File.ReadAllText(configFile);
            this.Configuration = JsonUtility.FromJson<ConfigDTO>(configContent);
            this.ServerUrl = this.Configuration.GameServerIP;
        }

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
        InvokeRepeating("Ping", 0, 5);
    }

    public void Ping()
    {
        // if connection invalid we just wait until connect.
        if (WebSocket.State != WebSocketState.Open)
            WebSocket.Connect().Wait();

        // We ping the server.
        SendToServer(GSMethods.Ping, string.Empty);
    }

    private void WebSocket_OnMessage(byte[] data)
    {
        // We get thre response data.
        GSSocketResponse responseData = Encoding.UTF8.GetString(data).ToObject<GSSocketResponse>();

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
        Debug.Log("Connected");
    }


    public void SendToServer<T>(GSMethods method, T data, Action<GSSocketResponse> responseAction = null) where T : class
    {
        // Request data.
        GSSocketRequest<T> request = new GSSocketRequest<T>(method, data, responseAction);

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
            foreach (GSSocketResponse responseData in ExecutableResponse.Where(x => x.UnExpectedMethods == GSUnExpectedMethods.None))
            {
                // We search for all the requests.
                foreach (IGSSocketRequest request in RequestList)
                {
                    // if removed then its means we was waiting for it.
                    if (request.OnResponseReceived(responseData))
                        break;
                }
            }

            // We execute all unexpected actions.
            foreach (GSSocketResponse responseData in ExecutableResponse.Where(x => x.UnExpectedMethods != GSUnExpectedMethods.None))
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
        await WebSocket.Close();
    }
}
