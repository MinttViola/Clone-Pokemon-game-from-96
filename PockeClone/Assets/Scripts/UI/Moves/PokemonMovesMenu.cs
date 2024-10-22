using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonMovesMenu : MonoBehaviour
{
    [SerializeField] MiniPoke miniPoke;
    [SerializeField] Type type1;
    [SerializeField] Type type2;
    [SerializeField] Text name;

    public void SetData(Pokemon pokemon) 
    {
        miniPoke.Set(pokemon);
        name.text = pokemon.Base.Name;
        type1.SetData(pokemon.Base.TypeOne);
        type2.SetData(pokemon.Base.TypeTwo);
    }
}
