using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// F�brica Abstracta
public interface ICenfomonEncounterFactory
{
    AbstractWildCenfomonEncounter GiveRange();
}