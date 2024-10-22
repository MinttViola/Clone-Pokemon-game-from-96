using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField]Text massageText;

    PokeMemberUI[] memberSlots;
    List<Pokemon> pokemons;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PokeMemberUI>(true);
    }
    public void SetPartyData(List<Pokemon> pokemons)
    {

        this.pokemons = pokemons;
        for(int i = 0;i< memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemons[i]);
            }
            else memberSlots[i].gameObject.SetActive(false);    
        }
        massageText.text = "Choose a pokemon";
    }

    public void UpdatePartySelection(int selectedMember)
    {
        for(int i = 0;i< pokemons.Count;i++)
        {
            if(i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void MessageText(string message) {
        massageText.text = message;
    }
}
