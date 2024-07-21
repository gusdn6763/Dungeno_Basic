using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreaturePartUi : UIScript, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject panelObject;

    public Creature Owner { get; set; } = null;

    protected override void Awake()
    {
        base.Awake();
        panelObject.SetActive(false);
        Owner = GetComponentInParent<Creature>();
    }

    public override void OpenClose(bool on)
    {
        if (on == false)
            Debug.Log("");
        panelObject.SetActive(on);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        panelObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        panelObject.SetActive(false);
    }
}
