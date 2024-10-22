using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Type : MonoBehaviour
{
    Image type;
    [SerializeField] List<Sprite> typesList;

    private void Awake()
    {
        type = GetComponent<Image>();
    }

    public void SetData(PokemonType setType)
    {

        switch (setType)
        {
            case PokemonType.Normal:
                type.sprite = typesList[0];
                break;
            case PokemonType.Fire:
                type.sprite = typesList[1];
                break;
            case PokemonType.Water:
                type.sprite = typesList[2];
                break;
            case PokemonType.Electric:
                type.sprite = typesList[3];
                break;
            case PokemonType.Grass:
                type.sprite = typesList[4];
                break;
            case PokemonType.Ice:
                type.sprite = typesList[5];
                break;
            case PokemonType.Fighting:
                type.sprite = typesList[6];
                break;
            case PokemonType.Poison:
                type.sprite = typesList[7];
                break;
            case PokemonType.Ground:
                type.sprite = typesList[8];
                break;
            case PokemonType.Flying:
                type.sprite = typesList[9];
                break;
            case PokemonType.Psychic:
                type.sprite = typesList[10];
                break;
            case PokemonType.Bug:
                type.sprite = typesList[11];
                break;
            case PokemonType.Rock:
                type.sprite = typesList[12];
                break;
            case PokemonType.Ghost:
                type.sprite = typesList[13];
                break;
            case PokemonType.Dragon:
                type.sprite = typesList[14];
                break;
            case PokemonType.Dark:
                type.sprite = typesList[15];
                break;
            case PokemonType.Steel:
                type.sprite = typesList[16];
                break;
            case PokemonType.Fairy:
                type.sprite = typesList[17];
                break;
            case PokemonType.None:
                type.gameObject.SetActive(false);
                break;
        }
    }
}
