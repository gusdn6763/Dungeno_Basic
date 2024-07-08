using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public HashSet<Item> Items { get; } = new HashSet<Item>();
    public HashSet<Creature> Creatures { get; } = new HashSet<Creature>();

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

        if (obj is Item item)
        {
            Item spawnedItem = Instantiate(item);
            spawnedItem.transform.SetParent(ItemRoot);
            Items.Add(spawnedItem);
            return spawnedItem as T;
        }
        else if (obj is Creature creature)
        {
            Creature spawnedCreature = Instantiate(creature);
            Managers.Battle.Spawn<Creature>(spawnedCreature);
            spawnedCreature.transform.SetParent(MonsterRoot);
            Creatures.Add(spawnedCreature);
            return spawnedCreature as T;
        }
        return null;
    }

    public void Despawn<T>(T obj) where T : InteractionObject
    {
        System.Type type = typeof(T);

        if (obj is Item item)
        {
            Items.Remove(item);
        }
        else if (obj is Creature creature)
        {
            Creatures.Remove(creature);
            Managers.Battle.Despawn(creature);
        }

        obj.CurrentArea.DestoryArea(obj);
        Destroy(obj.gameObject);
    }
}
