using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Fábrica Abstracta
public interface ICenfomonEncounterFactory
{
    AbstractWildCenfomonEncounter GiveRange();
}