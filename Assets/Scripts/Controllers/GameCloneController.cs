#if UNITY_EDITOR
using ParrelSync;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCloneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // if device is editor.
        if (DeviceController.Instance.DeviceType == DeviceController.Devices.Editor)
        {
            if (ClonesManager.IsClone())
            {
                // Automatically connect to local host if this is the clone editor
                LoginController.Instance.EditorLoginToken = "2";       
            }
            else
            {
                // Automatically start server if this is the original editor
                LoginController.Instance.EditorLoginToken = "1";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
#endif