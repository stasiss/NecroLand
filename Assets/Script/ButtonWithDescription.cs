using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonWithDescription : Button
{
    public bool isMouseOn()
    {
        return IsHighlighted();
    }
}
