using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class Player : BaseObject
{
    [Header("스텟")]
    [SerializeField] private Stat stat;

    [Header("체력")]
    [SerializeField] private float hp;
    public float Hp { get => hp; set => hp = value; }

    [Header("부위")]
    public List<Part> parts = new List<Part>();
    public List<Part> Parts { get => parts; private set => parts = value; }

    [Header("배고픔")]
    [SerializeField] private float hungry;
    public float Hungry { get => hungry; set => hungry = value; }
    [Header("허기")]
    [SerializeField] private float fatigue;
    public float Fatigue { get => fatigue; set => fatigue = value; }

    [Header("데미지")]
    [SerializeField] private float damage;
    public float Damage { get => damage; private set => damage = value; }

    public void Damaged(float damage)
    {
        List<Part> activePart = parts.Where(p => !p.IsBroken).ToList();

        int pos = Random.Range(0, activePart.Count);
        activePart[pos].Damaged(damage);
    }
}
