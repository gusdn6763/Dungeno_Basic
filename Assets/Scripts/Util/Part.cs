using System;
using Unity.Mathematics;
using UnityEngine;
using static Define;

[System.Serializable]
public class Part
{
    public bool IsBroken { get; set; } = false;

    public Action<Part, float> OnDamaged;
    public Action<Part> OnBroken;

    [SerializeField] private E_PartType partType;
    public E_PartType PartType { get => partType; }

    public float fullHp;
    public float FullHp { get { return fullHp; } }

    public float currentHp;
    public float CurrentHp { get { return currentHp; } }

    public virtual void Damaged(float damage)
    {
        currentHp = Mathf.Clamp(currentHp - damage, 0, FullHp);

        if (currentHp <= 0)
        {
            IsBroken = true;
            OnBroken?.Invoke(this);
        }
        else
            OnDamaged?.Invoke(this, damage);
    }
}


[System.Serializable]
public class CreaturePart : Part
{
    public PartButton partButton;
    private Creature owner;

    public void Init(Creature creature)
    {
        owner = creature;
        partButton.Init(this);
    }

    public override void Damaged(float damage)
    {
        base.Damaged(damage);

        partButton.Refresh(CurrentHp / FullHp);
    }
}
