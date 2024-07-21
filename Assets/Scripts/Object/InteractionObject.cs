using System;
using TMPro;
using UnityEngine;
using static Define;

public class InteractionObject : BaseObject
{
    [Header("번호")]
    [SerializeField] private int index;
    public int Index { get => index; private set => index = value; }

    [Header("최대 생존 시간")]
    public int maxLifetime;

    [Header("확인용-현재 생존 시간")]
    [SerializeField] int currentLifetime;
    public int CurrentLifetime { get => currentLifetime; set => currentLifetime = value; }

    [Header("이름")]
    public string commandName;

    [Header("설명")]
    public string description;

    [Header("감지 확률")]
    public float detectValue;

    [Header("감지 시간")]
    public float detectionTime = 0f;

    public Area CurrentArea { get; set; }

    protected TextMeshPro text;
    protected RectTransform rectTransform;
    protected BoxCollider2D boxCollider2D;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        text = GetComponent<TextMeshPro>();
        rectTransform = GetComponent<RectTransform>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        text.text = commandName;
        boxCollider2D.size = rectTransform.sizeDelta;
        currentLifetime = 0;

        return true;
    }

    public Vector2 GetSize()
    {
        if (rectTransform)
            return rectTransform.sizeDelta;
        else
            return GetComponent<RectTransform>().sizeDelta;
    }

    public virtual void Spawn()
    {

    }

    public virtual bool Detect()
    {
        return UnityEngine.Random.value <= detectValue / 100f;
    }

    public virtual void DetectAction()
    {
    }

    public void UpdateLifeTime()
    {
        currentLifetime++;

        if (currentLifetime >= maxLifetime)
            DestoryObject();
    }

    public virtual void DestoryObject()
    {
        CurrentArea.DestoryArea(this);
        Managers.Object.Despawn(this);
    }
}
