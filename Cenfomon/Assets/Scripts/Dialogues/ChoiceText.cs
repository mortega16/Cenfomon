using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceText : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetSelected(bool selected)
    {
        text.color = (selected) ? GlobalSettings.i.HighlightedColor : Color.black;
    }

    public TextMeshProUGUI TextField => text;
}
