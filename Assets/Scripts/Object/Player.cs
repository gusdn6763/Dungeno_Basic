using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class Player : BaseObject
{
    [Header("����")]
    [SerializeField] private Stat stat;

    [Header("ü��")]
    [SerializeField] private float hp;
    public float Hp { get => hp; set => hp = value; }

    [Header("����")]
    public List<Part> parts = new List<Part>();
    public List<Part> Parts { get => parts; private set => parts = value; }

    [Header("�����")]
    [SerializeField] private float hungry;
    public float Hungry { get => hungry; set => hungry = value; }
    [Header("���")]
    [SerializeField] private float fatigue;
    public float Fatigue { get => fatigue; set => fatigue = value; }

    [Header("������")]
    [SerializeField] private float damage;
    public float Damage { get => damage; private set => damage = value; }

    public void Damaged(float damage)
    {
        List<Part> activePart = parts.Where(p => !p.IsBroken).ToList();

        int pos = Random.Range(0, activePart.Count);
        activePart[pos].Damaged(damage);
    }
}
