using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Area : MonoBehaviour
{
    [Header("확인용-지역에 존재하는 오브젝트")]
    public List<InteractionObject> interactionObjects = new List<InteractionObject>();

    protected virtual void HandleOnDead(InteractionObject obj)
    {
        interactionObjects.Remove(obj);     
    }

    public List<T> FindTypeObjects<T>() where T : InteractionObject
    {
        List<T> list = new List<T>();
        for (int i = 0; i < interactionObjects.Count; i++)
        {
            if (interactionObjects[i] is T typedObject)
            {
                list.Add(typedObject);
            }
        }
        return list;
    }
}
