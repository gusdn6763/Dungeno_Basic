using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Area : MonoBehaviour
{
    [Header("Ȯ�ο�-������ �����ϴ� ������Ʈ")]
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
