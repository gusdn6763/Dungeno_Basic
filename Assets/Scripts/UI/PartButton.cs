using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PartButton : MonoBehaviour
{
    private Slider slider;
    private Button button;

    public CreaturePart CurrentPart { get; set; }

    public void Init(CreaturePart part)
    {
        CurrentPart = part;

        button = GetComponent<Button>();
        slider = GetComponentInChildren<Slider>();

        IsInteractable(true);
        Managers.Player.CanSelectEvent += IsInteractable;
        CurrentPart.Owner.OnDestoryEvent += PartDestory;
        button.onClick.AddListener(() => Managers.Player.SelectAttackPart(CurrentPart));
    }

    protected void PartDestory(Creature obj)
    {
        Managers.Player.CanSelectEvent -= IsInteractable;
        CurrentPart.Owner.OnDestoryEvent -= PartDestory;

        if (Managers.Player.AttackList.Contains(CurrentPart))
            Managers.Player.AttackList.Remove(CurrentPart);
    }

    public void IsInteractable(bool isActive)
    {
        //부위가 파괴되었거나 선택하지 않았을 경우
        if (CurrentPart.IsBroken || Managers.Player.PartCanSelect == false)
            button.interactable = false;
        else
            button.interactable = isActive;
    }

    public void Refresh(float radio)
    {
        if (radio <= 0)
            IsInteractable(false);

        slider.value = radio;
    }
}
