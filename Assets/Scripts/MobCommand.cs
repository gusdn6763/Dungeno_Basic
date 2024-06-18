using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MobCommand : MonoBehaviour
{
    private TextMeshPro text;
    private RectTransform rectTransform;

    [Header("이름")]
    public string commandName;

    [Header("설명")]
    public string description;

    [Header("스텟")]
    public Stat stat;

    [Header("사라지는 시간")]
    public float destoryTime;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        rectTransform = GetComponent<RectTransform>();

        text.text = commandName;

        Destroy(gameObject, destoryTime);
    }

    private void Update()
    {
        
    }

    public Vector2 GetSize()
    {
        return rectTransform.sizeDelta;
    }
}
