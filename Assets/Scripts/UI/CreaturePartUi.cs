using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreaturePartUi : UIScript, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject panelObject;

    public void Init()
    {
        panelObject.SetActive(false);
    }

    public override void OpenClose(bool on)
    {
        canvasGroup.alpha = on ? 1 : 0;
        canvasGroup.blocksRaycasts = on ? true : false;
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
