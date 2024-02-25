using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityButtonHoverListener : HoverListener
{
    public Ability ability;

    public event Action<PointerEventData, Ability> OnAbilityButtonEnter;
    public event Action<PointerEventData, Ability> OnAbilityButtonExit;

    public new void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        OnAbilityButtonEnter?.Invoke(eventData, ability);
    }

    public new void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        OnAbilityButtonExit?.Invoke(eventData, ability);
    }
}
