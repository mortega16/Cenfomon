using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public bool IsPlayerUnit
    {
        get{ return isPlayerUnit;}
    }

    public BattleHud Hud
    {
        get { return hud; }
    }

    public Cenfomon Cenfomon { get; set; }

    Image image;
    Vector3 originalPos;
    Color originalColor;

    public void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void SetUp(Cenfomon cenfomon)
    {
        Cenfomon = cenfomon;
        if (isPlayerUnit)
            GetComponent<Image>().sprite = Cenfomon.Base.BackSprite;
        else
            GetComponent<Image>().sprite = Cenfomon.Base.FrontSprite;

        hud.gameObject.SetActive(true);
        hud.SetData(cenfomon);

        transform.localScale = new Vector3(1, 1, 1);
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
}
