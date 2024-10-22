using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB : MonoBehaviour
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>() 
    {
        {ConditionID.psn, new Condition(){
                Name = "Poison",
                StartMassage = "has been poisoned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.HpChange = true;
                    pokemon.UpdateHP(CalculateDamage(pokemon,8));
                    pokemon.StatusChange.Enqueue($"{pokemon.Base.Name} hurt itself due to poison");
                }
            }
        },
        {ConditionID.brn, new Condition(){
                Name = "Burn",
                StartMassage = "has been burned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.HpChange = true;
                    pokemon.UpdateHP(CalculateDamage(pokemon,16));
                    pokemon.StatusChange.Enqueue($"{pokemon.Base.Name} hurt itself due to burn");
                    
                }
            }
        },
        {ConditionID.par, new Condition(){
                Name = "Paralyzed",
                StartMassage = "has been paralyzed",
                OnBeforeMove =(Pokemon pokemon) =>
                {
                    if (Random.Range(1,5) == 1)
                    {
                        pokemon.StatusChange.Enqueue($"{pokemon.Base.Name} paralyzed and cant move");
                        return false;
                    } else return true;

                }
            }
        },
        {ConditionID.frz, new Condition(){
                Name = "Freeze",
                StartMassage = "has been frozen",
                OnBeforeMove =(Pokemon pokemon) =>
                {
                    if (Random.Range(1,5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChange.Enqueue($"{pokemon.Base.Name} not frozen anymore");
                        return true;
                    } else return false;

                }
            }
        },
        {ConditionID.slp, new Condition(){
                Name = "Sleep",
                StartMassage = "has fallen asslep",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.StatusTime = Random.Range(1,4);
                    Debug.Log($"Sleep {pokemon.StatusTime} times");
                },
                OnBeforeMove =(Pokemon pokemon) =>
                {
                    if(pokemon.StatusTime == 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChange.Enqueue($"{pokemon.Base.Name} woke up");
                        return true;
                    }
                    else{
                    pokemon.StatusTime--;
                    pokemon.StatusChange.Enqueue($"{pokemon.Base.Name} is sleeping");
                    return false;}

                }
            }
        }, 
        //Volatile status|непостоянные статусы (https://bulbapedia.bulbagarden.net/wiki/Status_condition#Volatile_status)
        {ConditionID.confusion, new Condition(){
                Name = "Confusion",
                StartMassage = "has fallen confused",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusTime = Random.Range(1,5);
                    Debug.Log($"Will be confused for {pokemon.VolatileStatusTime} times");
                },
                OnBeforeMove =(Pokemon pokemon) =>
                {
                    if(pokemon.VolatileStatusTime == 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChange.Enqueue($"{pokemon.Base.Name} kciked out of confusion!");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;

                    //50% шанс провести атаку
                    if (Random.Range(1,3) == 1)
                    {
                        return true;
                    }

                    pokemon.StatusChange.Enqueue($"{pokemon.Base.Name} is confused");
                    pokemon.UpdateHP(CalculateDamage(pokemon,8));
                    pokemon.StatusChange.Enqueue($"It hurt itseld due to confusion");
                    return false;

                }
            }
        },
    };

    static int CalculateDamage(Pokemon pokemon, int sec)
    {
        return Mathf.Clamp((pokemon.MaxHP / sec), 1, pokemon.MaxHP);
    }

    public static float GetStatusBonus(Condition condition) {
        if (condition == null)
            return 1f;
        switch (condition.Name)
        {
            case null:
                return 1f;
            case "Sleep":
                case "Freeze":
                return 2f;
            case "Paralyzed":
            case "Burn":
            case "Poison":
                return 1.5f;
            default:
                return 1f;

        }

    }
}

public enum ConditionID
{
    none,psn,brn,slp,par,frz,
    confusion
}
