using Assets.Scripts.Socket.Interfaces;
using System;
using UnityEngine;

[Serializable]
public class BSSocketRequest<T> : IBSSocketRequest where T : class
{
    /// <summary>
    /// Callback action when the response received.
    /// </summary>
    public Action<BSSocketResponse> Action { get; set; }

    /// <summary>
    /// Authenticated token.
    /// </summary>
    public string AuthToken;

    /// <summary>
    /// Unique request id for each request.
    /// </summary>
    public string RequestID;

    /// <summary>
    /// Action method type.
    /// </summary>
    public BSMethods Method;

    /// <summary>
    /// Data to send server.
    /// </summary>
    public T Data;

    public BSSocketRequest(BSMethods method, T source, Action<BSSocketResponse> onCallBackAction)
    {
        // We are going to use this token for requests.
        this.AuthToken = LoginController.Instance?.LoginData?.RequestToken;

        // Unique identifier of action.
        this.Method = method;

        // Data that will send to server.
        this.Data = source;

        // Response action is going to be here.
        this.Action = onCallBackAction;

        // We are creating a unique id for the same action types.
        this.RequestID = Guid.NewGuid().ToString();
    }

    public bool OnResponseReceived(BSSocketResponse s)
    {
        // We make sure this is the owner of response.
        bool isRequestResponse = s.RequestID == RequestID;

        // if true than execute the action.
        if (isRequestResponse)
        {
            // We remove from the list.
            lock (BattleServerController.Instance.RequestList)
                BattleServerController.Instance.RequestList.Remove(this);

            // We run the action.
            try
            {
                if (Action != null)
                    Action.Invoke(s);
            }
            catch (Exception exc)
            {
                Debug.LogException(exc);
            }
        }

        // We return is that response owner is this.
        return isRequestResponse;
    }
}