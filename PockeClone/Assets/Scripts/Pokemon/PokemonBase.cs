using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
//база для покемона 

public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] List<Sprite> miniPokeAnim;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;
    //base state
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] int catchRate = 255;

    [SerializeField] List<LearnableMove> learnableMoves;
    public int GetExpForLevel(int level)
    {
        switch (growthRate)
        {
            case GrowthRate.Fast:
                return (4 * (level*level*level))/5;
            case GrowthRate.MediumFast:
                return level * level * level;
            case GrowthRate.MediumSlow:
                return (6 / 5 * (level * level * level)) - (15 * (level * level)) + (100 * level) - 140;
            case GrowthRate.Slow:
                return (5*(level * level * level))/4;
            default:
                return -1;
        }
    }

    public List<Sprite> MiniPokeAnim => miniPokeAnim; 
    public string Name => name;
    public string Description => description;
    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;
    public PokemonType TypeOne => type1;
    public PokemonType TypeTwo => type2;
    public int MaxHP => maxHP;
    public int Attack => attack;
    public int Deffense => defense;
    public int SpAttack => spAttack;
    public int SpDeffense => spDefense;
    public int Speed => speed;

    public List<LearnableMove> LearnableMoves => learnableMoves;

    public int CatchRate => catchRate;

    public int ExpYield => expYield;
    public GrowthRate GrowthRate => growthRate;



}

    [System.Serializable]
    public class LearnableMove
    {
        [SerializeField] MoveBase moveBase;
        [SerializeField] int level;

        public MoveBase Base { get { return moveBase; } }
        public int Level { get { return level; } }

    }

public enum Stat
{
    Attack,
    Deffense,
    SpAttack,
    SpDeffense,
    Speed,

    //буст
    Accuracy,
    Evasion
}

public enum GrowthRate
{
    Fast,
    MediumFast,
    MediumSlow,
    Slow
}
    public enum PokemonType
    {
        None, 
        Normal,
        Fire,
        Water,
        Electric,
        Grass,
        Ice,
        Fighting,
        Poison,
        Ground, 
        Flying,
        Psychic,    
        Bug,
        Rock,
        Ghost,
        Dragon,
        Dark,
        Steel,
        Fairy
    }

public class TypeChart
{
    static float[][] chart =
        {
        //                      nor fir wat ele gra ice fig poi gro fly psy bug roc gho dra dar ste fai
            /*Norm*/new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,0.5f, 0f, 1f, 1f,0.5f, 1f},
            /*Fire*/new float[] {1f,0.5f,0.5f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 2f,0.5f, 1f,0.5f, 1f, 2f, 1f},
            /*Wate*/new float[] {1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 2f, 1f, 1f, 1f, 2f, 1f,0.5f, 1f, 1f, 1f, 1f},
            /*Elec*/new float[] {1f, 1f, 2f,0.5f,0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 1f, 1f, 1f,0.5f, 1f, 1f, 1f},
            /*Gras*/new float[] {1f,0.5f, 2f, 1f,0.5f, 1f, 1f,0.5f, 2f,0.5f, 1f,0.5f, 2f, 1f, 0.5f, 1f, 0.5f, 1f},
            /*Ice */new float[] {1f,0.5f,0.5f, 1f, 2f, 0.5f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f},
            /*Figh*/new float[] {2f, 1f, 1f, 1f, 1f, 2f, 1f,0.5f, 1f,0.5f,0.5f,0.5f, 2f, 0f, 1f, 2f, 2f, 0.5f},
            /*Pois*/new float[] {1f, 1f, 1f, 1f, 2f, 1f, 1f,0.5f,0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 0f, 2f},
            /*Grou*/new float[] {1f, 2f, 1f, 2f,0.5f, 1f, 1f, 2f, 1f, 0f, 1f,0.5f, 2f, 1f, 1f, 1f, 2f, 1f},
            /*Flyi*/new float[] {1f, 1f, 1f,0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 2f,0.5f, 1f, 1f, 1f,0.5f, 1f},
            /*Psyc*/new float[] {1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f,0.5f, 1f, 1f, 1f, 1f, 0f,0.5f, 1f},
            /*Bug */new float[] {1f,0.5f, 1f, 1f, 2f, 1f,0.5f,0.5f, 1f,0.5f, 2f, 1f, 1f, 0.5f, 1f, 2f,0.5f, 0.5f},
            /*Rock*/new float[] {1f, 2f, 1f, 1f, 1f, 2f,0.5f, 1f,0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f,0.5f, 1f},
            /*Ghos*/new float[] {0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f},
            /*Drag*/new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f,0.5f, 0f},
            /*Dark*/new float[] {1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f,0.5f, 1f,0.5f},
            /*Stee*/new float[] {1f,0.5f,0.5f,0.5f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f,0.5f, 2f},
            /*Fair*/new float[] {1f,0.5f, 1f, 1f, 1f, 1f, 2f,0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f,0.5f, 1f},
        };

    public static float GetEffectivness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;
        int row = (int)attackType-1;
        int col = (int)defenseType-1;

        return chart[row][col];
    }
}

    

