using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Area : MonoBehaviour
{
    [Header("Ȯ�ο�-������ �����ϴ� ������Ʈ")]
    public List<InteractionObject> interactionObjects = new List<InteractionObject>();

    [SerializeField] private string areaName;
    public string AreaName { get => areaName; }

    public virtual void Init()
    {

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

    public virtual void DestoryArea(InteractionObject obj)
    {
        if(interactionObjects.Contains(obj))
            interactionObjects.Remove(obj);     
    }

    public virtual void AreaStateEnter(E_AreaState areaState)
    {
    }
}
