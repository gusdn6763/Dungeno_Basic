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
    public int currentLifetime;

    [Header("이름")]
    public string commandName;

    [Header("설명")]
    public string description;

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

    public void UpdateLifetime()
    {
        currentLifetime++;
        if (currentLifetime >= maxLifetime)
        {
            Debug.Log(commandName + "삭제");
            DestoryObject();
        }
    }

    public virtual void DestoryObject()
    {
        Managers.Object.Despawn(this);
    }
}
