using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NOX.UI.RWRConfig;

class RWRButton : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
{
    Action Toggle;
    GameObject tholder;
    Text text;
    Outline outline;
    Image background;
    RectTransform BTf;

    void Awake()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }
}