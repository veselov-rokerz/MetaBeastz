using Assets.Scripts.Socket.Interfaces;
using System;
using UnityEngine;

[Serializable]
public class GSSocketRequest<T> : IGSSocketRequest where T : class
{
    /// <summary>
    /// Callback action when the response received.
    /// </summary>
    public Action<GSSocketResponse> Action { get; set; }

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
    public GSMethods Method;

    /// <summary>
    /// Data to send server.
    /// </summary>
    public T Data;

    public GSSocketRequest(GSMethods method, T source, Action<GSSocketResponse> onCallBackAction)
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

    public bool OnResponseReceived(GSSocketResponse s)
    {
        // We make sure this is the owner of response.
        bool isRequestResponse = s.RequestID == RequestID;

        // if true than execute the action.
        if (isRequestResponse)
        {
            // We remove from the request list.
            lock (GSController.Instance.RequestList)
                GSController.Instance.RequestList.Remove(this);

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