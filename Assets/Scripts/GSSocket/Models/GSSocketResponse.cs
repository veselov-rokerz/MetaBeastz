using Assets.Scripts.Extends;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GSSocketResponse
{
    /// <summary>
    /// Unique request id when user sent a request.
    /// </summary>
    public string RequestID;

    /// <summary>
    /// State of request.
    /// </summary>
    public bool IsSuccess;

    /// <summary>
    /// if a message exists message will be filled.
    /// </summary>
    public string Message;

    /// <summary>
    /// When we were waiting for an action response this wont be none.
    /// </summary>
    public GSMethods Method;

    /// <summary>
    /// When an unexpected response then this value will be changed.
    /// </summary>
    public GSUnExpectedMethods UnExpectedMethods;

    /// <summary>
    /// Incoming json response.
    /// </summary>
    public string Data;

    /// <summary>
    /// Eğer data tek bir data ise GetData kullanılır.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetData<T>() where T : class
    {
        return JsonUtility.FromJson<T>(Data);

    }

    /// <summary>
    /// Eğer beklenen data liste ise ToList kullanılır.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<T> GetDataList<T>() where T : class
    {
        return JsonHelper.getJsonArray<T>(Data).ToList();
    }

}