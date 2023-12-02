using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new crystal")]

public class CrystalItem : ItemBase
{
    [SerializeField] float catchRateModifier = 1;

    public override bool Use(Cenfomon cenfomon)
    {
        return true;
    }

    public override bool CanUseOutsideBattle => false;

    public float CatchRateModifier => catchRateModifier;
}
