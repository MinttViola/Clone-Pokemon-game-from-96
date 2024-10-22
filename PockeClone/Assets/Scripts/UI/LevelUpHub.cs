using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpHub : MonoBehaviour
{
    [SerializeField] Text textBox;
    public void StatsUp(Pokemon newStats)
    {
        var oldStats = new Pokemon(newStats.Base, newStats.Level - 1);
        newStats.CalculateStats();
        oldStats.CalculateStats();
        textBox.text = $"+{newStats.MaxHP - oldStats.MaxHP}\n+{newStats.Attack - oldStats.Attack}\n+{newStats.Deffense - oldStats.Deffense}\n+{newStats.SpAttack - oldStats.SpAttack}\n+{newStats.SpDefense - oldStats.SpDefense}\n+{newStats.Speed - oldStats.Speed}";
    }

    public void NewStats(Pokemon newStats)
    {
        textBox.text = $"{newStats.MaxHP}\n{newStats.Attack}\n{newStats.Deffense}\n{newStats.SpAttack}\n{newStats.SpDefense}\n{newStats.Speed}";
    }
}
