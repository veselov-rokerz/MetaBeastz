using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UnityExtends
{
    public static void RemoveAllChildsOfTransform(this Transform transform)
    {
        foreach (Transform child in transform)
            Object.Destroy(child.gameObject);
    }

    public static T GetComponentInChild<T>(this Transform transform, string childName) where T : class
    {
        return transform.Find(childName).GetComponent<T>();
    }

    public static List<RaycastResult> GetHits()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results;
    }

}
