using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public bool IsUpdating {  get; private set; }

    public void SetHP(float hpNormalized)
    {
        hpNormalized = Mathf.Clamp01(hpNormalized);

        if (float.IsNaN(hpNormalized))
        {
            Debug.LogError("Invalid hpNormalized value (NaN). Check your calculations.");
            return;
        }

        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHp)
    {
        IsUpdating = true;

        newHp = Mathf.Clamp01(newHp);

        float curpHp = health.transform.localScale.x;
        float changeAmt = curpHp - newHp;

        while(curpHp - newHp > Mathf.Epsilon)
        {
            curpHp -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curpHp, 1f);
            yield return null;
        }

        health.transform.localScale = new Vector3(newHp, 1f);

        IsUpdating = false;
    }
}
