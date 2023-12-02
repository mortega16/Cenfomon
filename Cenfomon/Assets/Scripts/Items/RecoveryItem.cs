using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Create new recovery item")]

public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Cenfomon cenfomon)
    {

        //Revivir
        if (revive)
        {
            if (cenfomon.HP > 0)
                return false;

            cenfomon.IncreaseHP(15);

            return true;
        }

        //Seguido a esto, ningun item puede ser usado en cenfomones caídos
        if (cenfomon.HP == 0)
            return false;

        //Restaurar HP
        if(restoreMaxHP || hpAmount > 0)
        {
            if (cenfomon.HP == cenfomon.MaxHp)
                return false;

            if(restoreMaxHP)
                cenfomon.IncreaseHP(100);
            else
                cenfomon.IncreaseHP(hpAmount);
        }

        //Restaurar PP
        if (restoreMaxPP)
        {
            cenfomon.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
        }
        else if (ppAmount > 0)
        {
            cenfomon.Moves.ForEach(m => m.IncreasePP(ppAmount));
        }

        return true;
    }
}
