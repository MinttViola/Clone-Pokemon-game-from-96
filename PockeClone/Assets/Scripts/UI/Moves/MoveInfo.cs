using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveInfo : MonoBehaviour
{
    [SerializeField] Text power;
    [SerializeField] Text accuracy;
    [SerializeField] Text effect;

    public void SetData(MoveBase move)
    {

        if (move.Power > 0)
            power.text= $"{move.Power}";
        else power.text = "---";
        if (move.Accuracy > 0)
            accuracy.text = $"{move.Accuracy}";
        else accuracy.text = "---";
        effect.text = move.Description;
    }
}
