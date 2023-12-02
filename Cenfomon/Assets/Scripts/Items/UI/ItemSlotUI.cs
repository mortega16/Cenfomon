using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI countTexts;

    public TextMeshProUGUI NameText => nameText;
    public TextMeshProUGUI CountTexts => countTexts;

    public void SetData(ItemSlot itemSlot)
    {
        nameText.text = itemSlot.Item.Name;
        countTexts.text = $"X {itemSlot.Count}";
    }

    public void SetNameAndPrice(ItemBase item)
    {
        nameText.text = item.Name;
        countTexts.text = $"$ {item.Price}";
    }
}
