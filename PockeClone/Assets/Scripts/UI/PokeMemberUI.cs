using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokeMemberUI : MonoBehaviour
{

    [SerializeField] Text namePoke;
    [SerializeField] Text level;
    [SerializeField] Text HP;
    [SerializeField] Text MaxHP;
    [SerializeField] HPBar HPBar;
    [SerializeField] MiniPoke miniPoke;
    [SerializeField] Sprite chose;
    [SerializeField] Sprite unchose;
    [SerializeField] Image image;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        miniPoke.Set(pokemon);
        _pokemon = pokemon;
        namePoke.text = pokemon.Base.Name;

        level.text = pokemon.Level + "";

        HP.text = pokemon.Hp + "";

        MaxHP.text = "" + pokemon.MaxHP;

        HPBar.SetHp((float)pokemon.Hp / pokemon.MaxHP, pokemon.MaxHP);
        HPBar.ChangeColor(pokemon.Hp);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            image.sprite = chose;
        }
        else image.sprite = unchose;
    }
}
