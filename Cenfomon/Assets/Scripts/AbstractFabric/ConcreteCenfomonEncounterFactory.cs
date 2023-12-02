using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Fábricas Concretas
public class EasyCenfomonEncounterFactory : ICenfomonEncounterFactory
{
    public AbstractWildCenfomonEncounter GiveRange()
    {
        return new EasyCenfomonEncounter();
    }
}

public class MediumCenfomonEncounterFactory : ICenfomonEncounterFactory
{
    public AbstractWildCenfomonEncounter GiveRange()
    {
        return new MediumCenfomonEncounter();
    }
}

public class HardCenfomonEncounterFactory : ICenfomonEncounterFactory
{
    public AbstractWildCenfomonEncounter GiveRange()
    {
        return new HardCenfomonEncounter();
    }
}
