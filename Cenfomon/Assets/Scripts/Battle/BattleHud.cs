using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;

    Cenfomon _cenfomon;

    public void SetData(Cenfomon cenfomon)
    {
        if(_cenfomon != null)
        {
            _cenfomon.OnHPChanged -= UpdateHP;
        }

        _cenfomon = cenfomon;

        nameText.text = cenfomon.Base.Name;
        SetLevel();
        hpBar.SetHP((float)cenfomon.HP / cenfomon.MaxHp);
        SetExp();

        _cenfomon.OnHPChanged += UpdateHP;
    }

    public void SetLevel()
    {
        levelText.text = "Nvl " + _cenfomon.Level;
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if (expBar == null) yield break;

        if(reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currLevelExp = _cenfomon.Base.GetExpForLevel(_cenfomon.Level);
        int nextLevelExp = _cenfomon.Base.GetExpForLevel(_cenfomon.Level +1);

        float normalizedExp = (float)(_cenfomon.Exp-currLevelExp)/(nextLevelExp-currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPSmooth((float)_cenfomon.HP / _cenfomon.MaxHp);
    }

    public IEnumerator WaitforHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }

    public void ClearData()
    {
        if (_cenfomon != null)
        {
            _cenfomon.OnHPChanged -= UpdateHP;
        }
    }
}
