using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceController : MonoBehaviour
{
    public static DeviceController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public enum Devices { Editor, Web, Android, Ios, Windows }

    /// <summary>
    /// Get current device.
    /// </summary>
    public Devices DeviceType
    {
        get
        {
#if UNITY_EDITOR
            return Devices.Editor;
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
                return Devices.Web;
#endif
#if UNITY_ANDROID
                return Devices.Android;
#endif
#if UNITY_IOS
                return Devices.Ios;
#endif
            return Devices.Windows;
        }
    }
}

