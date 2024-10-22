using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MoveList : MonoBehaviour
{
    [SerializeField] Image select;
    [SerializeField]Type type;
    [SerializeField] Text name;
    [SerializeField] Text maxPP;
    [SerializeField] Text currPP;


    public void SetData(Move move)
    {
        select.gameObject.SetActive(false);
        name.text = move.Base.Name;
        maxPP.text = $"{move.Base.Pp}";
        currPP.text = $"{move.pp}";
        type.SetData(move.Base.Type);
    }

    public void SetDataNewMove(MoveBase move)
    {

        select.gameObject.SetActive(false);
        name.text = move.Name;
        maxPP.text = $"{move.Pp}";
        currPP.text = $"{move.Pp}";
        type.SetData(move.Type);
    }

    public void ChoseMove(bool chose)
    {
        select.gameObject.SetActive(chose);
    }
}
