using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPBar hpBar;

    Cenfomon _cenfomon;

    public void Init(Cenfomon cenfomon)
    {
        _cenfomon = cenfomon;
        UpdateData();

        _cenfomon.OnHPChanged += UpdateData;
        
    }

    void UpdateData()
    {
        nameText.text = _cenfomon.Base.Name;
        levelText.text = "Nvl " + _cenfomon.Level;
        hpBar.SetHP((float)_cenfomon.HP / _cenfomon.MaxHp);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = GlobalSettings.i.HighlightedColor;
        else
            nameText.color = Color.black;
    }
}
