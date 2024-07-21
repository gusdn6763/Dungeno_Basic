using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager : MonoBehaviour
{
    public List<Item> Items { get; } = new List<Item>();
    public List<Creature> Creatures { get; } = new List<Creature>();

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
        }
        if(obj)
            Destroy(obj.gameObject);
    }
}
