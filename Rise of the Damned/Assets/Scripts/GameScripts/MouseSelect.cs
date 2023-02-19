using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
 
[RequireComponent(typeof(Selectable))]
public class MouseSelect : MonoBehaviour, IPointerEnterHandler, IDeselectHandler, ISelectHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EventSystem.current.alreadySelecting)
            EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.GetComponent<Selectable>().OnPointerExit(null);
        foreach(Text text in GetComponentsInChildren<Text>()){
            if (text != null && text.name == "Text"){
                text.color = new Color(180f/255f,39f/255f,39f/255f,1f);
            }
            if (text != null && text.name == "TextShadow"){
                text.color = new Color(221f/255f,95f/255f,95f/255f,1f);
            }
        }
    }

    public void OnSelect(BaseEventData eventData){
        foreach(Text text in GetComponentsInChildren<Text>()){
            if (text != null && text.name == "Text"){
                text.color = new Color(221f/255f,95f/255f,95f/255f,1f);
            }
            if (text != null && text.name == "TextShadow"){
                text.color = new Color(180f/255f,39f/255f,39f/255f,1f);
            }
        }
        Text texta = this.GetComponentInChildren<Text>();
    }
}