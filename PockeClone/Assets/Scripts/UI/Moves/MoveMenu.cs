using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveMenu : MonoBehaviour
{
    MoveInfo moveInfo;
    MoveList[] moveList;
    List<Move> movesFromPoke;
    PokemonMovesMenu pokeMenu;
    MoveBase newMove;
    public void Init()
    {
        pokeMenu = GetComponentInChildren<PokemonMovesMenu>();
        moveList = GetComponentsInChildren<MoveList>();
        moveInfo = GetComponentInChildren<MoveInfo>();

    }

    public void SetData(Pokemon pokemon, MoveBase newMove = null)
    {
        pokeMenu.SetData(pokemon);
        for (int i = 0; i < 5; i++)
        {
            if (i < pokemon.Moves.Count)
            {
                moveList[i].gameObject.SetActive(true);
                moveList[i].SetData(pokemon.Moves[i]);
            } else moveList[i].gameObject.SetActive(false);
        }

        if (newMove == null)
        {
            moveList[4].gameObject.SetActive(false);
        }
        else
        {
            moveList[4].gameObject.SetActive(true);
            moveList[4].SetDataNewMove(newMove);

        }
        movesFromPoke = pokemon.Moves;
        this.newMove = newMove;
        moveInfo.SetData(pokemon.Moves[0].Base);
    }

    public void UpdateMoveSelection(int selectedMember)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == selectedMember&& i<4)
            {
                Debug.Log($"{i}");
                moveList[i].ChoseMove(true);
                moveInfo.SetData(movesFromPoke[i].Base);
            }
            else if (i == selectedMember && i == 4)
            {
                Debug.Log($"{i}");
                moveList[i].ChoseMove(true);
                moveInfo.SetData(newMove);
            }
            else
                moveList[i].ChoseMove(false);
        }
    }

}
