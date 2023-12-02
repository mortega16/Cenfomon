using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[System.Serializable]
public class Cenfomon
{
    [SerializeField] CenfomonBase _base;
    [SerializeField] int level;

    public Cenfomon(CenfomonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;

        Init();
    }

    public CenfomonBase Base
    {
        get
        {
            return _base;
        }
    }
    public int Level {
        get
        {
            return level;
        }
    }

    public int Exp { get; set; }
    public int HP {  get; set; }

    public List<Move> Moves { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Move CurrentMove { get; set; }

    public event System.Action OnHPChanged;

    public void Init()
    {
        //Generar movimientos de acuerdo a su nivel y poniendo un límite de 6

        Moves = new List<Move>();
        foreach(var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));
            if (Moves.Count >= CenfomonBase.MaxNumOfMoves)
                break;
        }

        CalculateStats();
        HP = MaxHp;

        Exp = Base.GetExpForLevel(Level);
    }

    public Cenfomon(CenfomonSaveData saveData)
    {
        _base = CenfomonDB.GetObjectByName(saveData.name);
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;

        Moves = saveData.moves.Select(s=>new Move(s)).ToList();
    }

    public CenfomonSaveData GetSaveData()
    {
        var saveData = new CenfomonSaveData()
        {
            name = Base.name,
            hp = HP,
            level = Level,
            exp = Exp,
            moves = Moves.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        int oldMaxHP = MaxHp;
        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10;

        if(oldMaxHP != 0)
            HP += MaxHp - oldMaxHP;
    }

    public bool CheckForLevelUp()
    {
        if(Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            CalculateStats();
            return true;
        }

        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnMove(MoveBase moveToLearn)
    {
        if (Moves.Count > CenfomonBase.MaxNumOfMoves)
            return;

        Moves.Add(new Move(moveToLearn));
    }

    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;
    }

    public Evolution CheckForEvolution()
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequiredLevel <= level);
    }

    public Evolution CheckForEvolution(ItemBase item)
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequiredItem == item);
    }

    public void Evolve(Evolution evolution)
    {
        _base = evolution.EvolvesInto;
        CalculateStats();
    }

    public void Heal()
    {
        HP = MaxHp;
        OnHPChanged?.Invoke();
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        return statVal;
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHp { get; private set; }

    public DamageDetails TakeDamage(Move move, Cenfomon attacker)
    {
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Fainted = false
        };
        
        float modifiers = Random.Range(0.85f, 1f)*type;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d*modifiers);

        HP -= damage;
        if(HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }
        DecreaseHP(damage);

        return damageDetails;
    }

    public void IncreaseHP(int amount)
    {
        HP = Mathf.Clamp(HP + (MaxHp*amount/100), 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(x=>x.PP>0).ToList();

        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public class DamageDetails
    {
        public bool Fainted { get; set;}
        public float TypeEffectiveness { get; set;}
    }

}

[System.Serializable]
public class CenfomonSaveData
{
    public string name;
    public int hp;
    public int level;
    public int exp;
    public List<MoveSaveData> moves;
}
