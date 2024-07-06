using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public HashSet<Item> Items { get; } = new HashSet<Item>();
    public HashSet<Creature> Creature { get; } = new HashSet<Creature>();

    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }
    public Transform MonsterRoot { get { return GetRootTransform("@Creature"); } }
    public Transform ItemRoot { get { return GetRootTransform("@Items"); } }

    public T Spawn<T>(InteractionObject obj) where T : InteractionObject
    {
        System.Type type = typeof(T);

        if (type == typeof(Item))
        {
            InteractionObject item = Instantiate(obj);
            item.transform.SetParent(ItemRoot);
            Items.Add(item as Item);
            return item as T;
        }
        else if (type == typeof(Creature))
        {
            InteractionObject monster = Instantiate(obj);
            monster.transform.SetParent(MonsterRoot);
            Creature.Add(monster as Creature);
            return monster as T;
        }
        return null;
    }

    public void Despawn<T>(T obj) where T : BaseObject
    {
        System.Type type = typeof(T);

        if (type == typeof(Item))
        {
            Items.Remove(obj as Item);
        }
        else if (type == typeof(Creature))
        {
            Creature.Remove(obj as Creature);
        }
    }
}
