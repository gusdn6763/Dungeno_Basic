using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PartButton : Button
{
    private Slider slider;

    public Part CurrentPart { get; set; }

    public void Init(Part part)
    {
        CurrentPart = part;
        Managers.Player.OnPlayerCanAttackEvent += IsInteractable;
        onClick.AddListener(Damaged);
        slider = GetComponentInChildren<Slider>();
    }

    public void IsInteractable(bool isActive)
    {
        if (CurrentPart.IsBroken)
            interactable = false;
        else
            interactable = isActive;
    }

    public void Refresh(float radio)
    {
        if (radio <= 0)
            IsInteractable(false);

        slider.value = radio;
    }

    public void Damaged()
    {
        //다시한번 체크
        if(Managers.Player.PlayerCanAttack)
        {
            CurrentPart.Damaged(Managers.Player.PlayerDamege);
        }
    }
}
