using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPbarShadow : MonoBehaviour
{
    [SerializeField] Image mainBar;
    [SerializeField] Image shadow;
    Color colorLow = new Color32(248, 224, 56,255);
    Color colorSoLow = new Color32(248, 88, 56, 255);
    Color colorMax = new Color32(112, 248, 168, 255);

    // Update is called once per frame
    void Update()
    {
        shadow.transform.localScale = mainBar.transform.localScale;
        
        if(mainBar.color == colorLow)
        {
            shadow.color = new Color32(200,168, 8, 255);

        }else if(mainBar. color == colorSoLow)
        {
            shadow.color = new Color32(168, 64, 72, 255);
        }
        else if (mainBar.color == colorMax)
        {
            shadow.color = new Color32(88,208,128, 255);
        }

    }
}
