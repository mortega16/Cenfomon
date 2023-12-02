using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Productos Concretos
public class EasyCenfomonEncounter : AbstractWildCenfomonEncounter
{
    public override Vector2Int GetLevelRange()
    {
        return new Vector2Int(1, 5);
    }
}

public class MediumCenfomonEncounter : AbstractWildCenfomonEncounter
{
    public override Vector2Int GetLevelRange()
    {
        return new Vector2Int(6, 10);
    }
}

public class HardCenfomonEncounter : AbstractWildCenfomonEncounter
{
    public override Vector2Int GetLevelRange()
    {
        return new Vector2Int(11, 15);
    }
}
