using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Cenfomon", menuName ="Cenfomon/Create new cenfomon")]
public class CenfomonBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] CenfomonType type1;
    [SerializeField] CenfomonType type2;

    //Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] int catchRate = 255;

    [SerializeField] List<LearnableMove> learnableMoves;

    [SerializeField] List<Evolution> evolutions;

    public static int MaxNumOfMoves { get; set; } = 6;

    public int GetExpForLevel(int level)
    {
        if(growthRate==GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }

        return -1;
    }

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public CenfomonType Type1
    {
        get { return type1; }
    }

    public CenfomonType Type2
    {
        get { return type2; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }

    public List<Evolution> Evolutions => evolutions;

    public int CatchRate => catchRate;

    public int ExpYield => expYield;

    public GrowthRate GrowthRate => growthRate;
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}

[System.Serializable]
public class Evolution
{
    [SerializeField] CenfomonBase evolvesInto;
    [SerializeField] int requiredLevel;
    [SerializeField] EvolutionItem requiredItem;

    public CenfomonBase EvolvesInto => evolvesInto;
    public int RequiredLevel => requiredLevel;
    public EvolutionItem RequiredItem => requiredItem;
}

public enum CenfomonType
{
    None,
    Normal,
    Agua,
    Fuego,
    Planta,
    Eléctrico,
    Fantasma,
    Volador,
    Bicho
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed
}

public enum GrowthRate
{
    Fast, MediumFast
}

public class TypeChart
{
    static float[][] chart =
    {
        //                  Nor   Agu  Fue  Pla   Ele  Fan   Vol  Bic
        /*Nor*/ new float[]{1f, 0.5f, 1f, 0.5f, 1.5f, 1.5f, 1f, 0.5f},
        /*Agu*/ new float[]{1.5f, 1f, 1.5f, 0.5f, 0.5f, 0.5f, 1.5f, 1f},
        /*Fue*/ new float[]{1f, 0.5f, 1f, 1.5f, 0.5f, 0.5f, 1.5f, 1.5f},
        /*Pla*/ new float[]{1.5f, 1.5f, 0.5f, 1f, 1.5f, 1f, 0.5f, 0.5f},
        /*Ele*/ new float[]{0.5f, 1.5f, 1.5f, 0.5f, 1f, 1.5f, 0.5f, 1f},
        /*Fan*/ new float[]{0.5f, 1.5f, 1.5f, 1f, 0.5f, 1f, 1.5f, 0.5f},
        /*Vol*/ new float[]{1f, 0.5f, 0.5f, 1.5f, 1.5f, 0.5f, 1f, 1.5f},
        /*Bic*/ new float[]{1.5f, 0.5f, 0.5f, 1.5f, 1f, 1.5f, 0.5f, 1f}
    };

    public static float GetEffectiveness(CenfomonType attackType, CenfomonType defenseType)
    {
        if (attackType == CenfomonType.None || defenseType == CenfomonType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}
