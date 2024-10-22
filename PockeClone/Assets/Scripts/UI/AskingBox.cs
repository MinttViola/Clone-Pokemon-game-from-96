using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AskingBox : MonoBehaviour
{
    [SerializeField] List<GameObject> Arrow;
    public bool Ask(int selectedArrow)
    {
        if (selectedArrow == 0) return true;
        else return false;
    }

    public void UpdateAskBoxArrow(int selectedArrow)
    {

        for (int i = 0; i < Arrow.Count; ++i)
        {
            if (i == selectedArrow)
            {
                Arrow[i].SetActive(true);
            }
            else
                Arrow[i].SetActive(false);
        }
    }

}
