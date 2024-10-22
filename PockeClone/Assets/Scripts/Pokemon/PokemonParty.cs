using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons{get{ return pokemons; }}
    private void Start()
    {

        foreach (var pokemon in pokemons)
        {

            pokemon.Init();
        }
    }

    public Pokemon GetHelthyPokemon()
    {
       return pokemons.Where(x => x.Hp > 0).FirstOrDefault();
    }

    public void SwitchPokemonToFirstPlace(int oldPlace)
    {
        Pokemon switchPokemon = pokemons[oldPlace];
        pokemons.RemoveAt(oldPlace);
        pokemons.Insert(0, switchPokemon);
    }

    public void AddPokemon(Pokemon newPokemon) 
    {
        if(pokemons.Count < 6)
        {
            pokemons.Add(newPokemon);
        }
        else
        {
            //on PC
        }
    }
}
