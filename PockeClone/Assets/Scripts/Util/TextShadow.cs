using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//скрипт для корректного изменения тени каждому тексту
public class TextShadow : MonoBehaviour
{
    [SerializeField] Text Shadow;
    [SerializeField] Text Maintext;
    void Update()
    {
        Maintext.text = Shadow.text;
    }

    public void ZeroPP(bool yes)
    {
        if (yes)
            Maintext.color = Color.red;
        else Maintext.color = new Color32(72,72,72,255);
    }
}
