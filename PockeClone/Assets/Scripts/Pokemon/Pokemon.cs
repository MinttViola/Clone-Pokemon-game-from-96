using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
//база для конкретного покемона в игре

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
        Init();
    }

    public PokemonBase Base => _base;
    public int Level => level;


    public int Exp { get; set; }
    public int Hp {  get; set; }

    public List<Move> Moves {  get; set; }
    public Move CurrentMove { get; set; }

    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoost { get; private set; }
    public Condition Status { get; private set; }
    public Condition VolatileStatus { get; private set; }
    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }
    public Queue<string> StatusChange { get; private set; }
    public bool HpChange { get; set; }
    public event System.Action OnStatusChanged;

    public void Init()
    {
        Moves = new List<Move>();

        //получение атак
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));

            if (Moves.Count >= 4)
                break;
        }

        Exp = Base.GetExpForLevel(level);
        CalculateStats();
        Hp = MaxHP;

        StatusChange = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }
    //заполнение вычеслений статов
    public void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Deffense, Mathf.FloorToInt((Base.Deffense * Level) / 100f) + 5);
        Stats.Add(Stat.SpDeffense, Mathf.FloorToInt((Base.SpDeffense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHP = Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10;
    }

    void ResetStatBoost() { 
        StatBoost = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Deffense, 0},
            {Stat.Speed, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDeffense, 0 },
            {Stat.Accuracy, 0 },
            {Stat.Evasion, 0 },
        };}


    //для проп
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //стат буст
        int boost = StatBoost[stat];
        var boostValue = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >=0)
            statVal = Mathf.FloorToInt(statVal * boostValue[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValue[-boost]);

        return statVal;
    }
    public void ApplyBoost(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoost[stat] = Mathf.Clamp(StatBoost[stat] + boost, -6, 6);
            if (boost > 0)
                StatusChange.Enqueue($"{Base.Name} {stat} rose");
            else
                StatusChange.Enqueue($"{Base.Name} {stat} fell");
        }
    }

    //статус
    public void SetStatus (ConditionID conditionID)
    {
        /*if (Status != null) return;*/
        Status = ConditionDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChange.Enqueue($"{Base.Name} {Status.StartMassage}");
        OnStatusChanged.Invoke();
    }
    public void CureStatus()
    {
        Status = null;
    }
    
    //неустойчивый статус
    public void SetVolatileStatus (ConditionID conditionID)
    {
        VolatileStatus = ConditionDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChange.Enqueue($"{Base.Name} {VolatileStatus.StartMassage}");
    }
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public void UpdateHP(int dmg)
    {
        Hp = Mathf.Clamp(Hp - dmg, 0, MaxHP);
    }
    //пропы
    public bool CheckForLevelUp() { 
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        else return false; }
    public int MaxHP { get; private set; }
    public int Attack {  get { return GetStat(Stat.Attack); } }
    public int Deffense {  get { return GetStat(Stat.Deffense); } }
    public int SpAttack {  get { return GetStat(Stat.SpAttack); } }
    public int SpDefense { get { return GetStat(Stat.SpDeffense); } }
    public int Speed {  get { return GetStat(Stat.Speed); } }


    //получение урона покемоном 
    public DamageDetail TakeDamage(Move move, Pokemon attaker)
    {
        float critical = 1f;
        if(Random.value * 100f < 6.25)
        {
            critical = 2f;
        }

        float type = TypeChart.GetEffectivness(move.Base.Type, this.Base.TypeOne) * TypeChart.GetEffectivness(move.Base.Type, this.Base.TypeTwo);


        var damageDetails = new DamageDetail()
        {
            Type = type,
            Critical = critical,
            Fainted = false
        };

        float attak = (move.Base.Category == MoveCategory.Special) ? attaker.SpAttack : attaker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Deffense;

        float modifaer = Random.Range(0.85f, 1f) * type*critical;
        float a = (2 * attaker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attak / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifaer);

        UpdateHP(damage);
        HpChange = true;

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(x => x.pp > 0).ToList();
        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
        HpChange = true;
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if ((Status?.OnBeforeMove != null && !Status.OnBeforeMove(this) )||( VolatileStatus?.OnBeforeMove != null && !VolatileStatus.OnBeforeMove(this)))
            canPerformMove = false;
        return canPerformMove;
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
    public LearnableMove GetLearnableMoveAtCurrLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }
    public void LearnMove(LearnableMove moveToLearn)
    {
        if (Moves.Count > 4)
            return;
        Moves.Add(new Move(moveToLearn.Base));
    }
}

public class DamageDetail
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float Type { get; set; }
}