using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MobCommand : MonoBehaviour
{
    private TextMeshPro text;
    private RectTransform rectTransform;

    [Header("�̸�")]
    public string commandName;

    [Header("����")]
    public string description;

    [Header("����")]
    public Stat stat;

    [Header("������� �ð�")]
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
