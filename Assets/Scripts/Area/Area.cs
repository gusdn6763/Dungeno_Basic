using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    [Header("Ȯ�ο�-������ �����ϴ� ������Ʈ")]
    public List<InteractionObject> interactionObjects = new List<InteractionObject>();

    [SerializeField] private string areaName;
    public string AreaName { get => areaName; }

    public void DestoryArea(InteractionObject obj)
    {
        if(interactionObjects.Contains(obj))
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
