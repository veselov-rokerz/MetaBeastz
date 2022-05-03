using System;
using UnityEngine;

public abstract class BaseService : MonoBehaviour
{
    public virtual void LoadOnStartUp(Action onLoaded = null) { }
}
