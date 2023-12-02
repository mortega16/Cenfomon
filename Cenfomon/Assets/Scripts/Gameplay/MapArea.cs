using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<CenfomonEncounterRecord> wildCenfomons;
    //[SerializeField] List<ICenfomonEncounterFactory> encounterFactories;

    [HideInInspector]
    [SerializeField] int totalChance = 0;

    private void OnValidate()
    {
        CalculateChancePercentage();
    }

    private void Start()
    {
        CalculateChancePercentage();
    }

    void CalculateChancePercentage()
    {
        totalChance = -1;

        if (wildCenfomons.Count > 0)
        {
            totalChance = 0;
            foreach (var record in wildCenfomons)
            {
                record.chanceLower = totalChance;
                record.chanceUpper = totalChance + record.chancePercentage;

                totalChance = totalChance + record.chancePercentage;
            }
        }
    }

    public Cenfomon GetRandomWildCenfomon()
    {
        int randVal = UnityEngine.Random.Range(1, 101);
        var cenfomonRecord = wildCenfomons.First(p => randVal >= p.chanceLower && randVal <= p.chanceUpper);

        if(cenfomonRecord.Dificultad == 1)
        {
            var factory = new EasyCenfomonEncounterFactory();
            var levelRange = factory.GiveRange().GetLevelRange();
            int level = levelRange.y == 0 ? levelRange.x : UnityEngine.Random.Range(levelRange.x, levelRange.y + 1);

            var wildCenfomon = new Cenfomon(cenfomonRecord.cenfomon, level);
            wildCenfomon.Init();
            return wildCenfomon;
        }
        else if (cenfomonRecord.Dificultad == 2)
        {
            var factory = new MediumCenfomonEncounterFactory();
            var levelRange = factory.GiveRange().GetLevelRange();
            int level = levelRange.y == 0 ? levelRange.x : UnityEngine.Random.Range(levelRange.x, levelRange.y + 1);

            var wildCenfomon = new Cenfomon(cenfomonRecord.cenfomon, level);
            wildCenfomon.Init();
            return wildCenfomon;
        }
        else if (cenfomonRecord.Dificultad == 3)
        {
            var factory = new HardCenfomonEncounterFactory();
            var levelRange = factory.GiveRange().GetLevelRange();
            int level = levelRange.y == 0 ? levelRange.x : UnityEngine.Random.Range(levelRange.x, levelRange.y + 1);

            var wildCenfomon = new Cenfomon(cenfomonRecord.cenfomon, level);
            wildCenfomon.Init();
            return wildCenfomon;
        }
        else
        {
            return null;
        }

        //var levelRange = cenfomonRecord.levelRange;
        //int level = levelRange.y == 0 ? levelRange.x : UnityEngine.Random.Range(levelRange.x, levelRange.y + 1);


        
    }
}

[System.Serializable]
public class CenfomonEncounterRecord
{
    public CenfomonBase cenfomon;
    public Vector2Int levelRange;
    public int Dificultad;
    public int chancePercentage;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }
}
