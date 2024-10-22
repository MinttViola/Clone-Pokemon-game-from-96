using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PokemonBase;

public class Move 
{
    //вывод информации о атаке
    public MoveBase Base { get; set; }

    public int pp;

    public Move (MoveBase @base)
    {
        Base = @base;
        this.pp = @base.Pp;
    }
}
