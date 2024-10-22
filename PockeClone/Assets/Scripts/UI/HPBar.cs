using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{

    [SerializeField] Image health;
    float maxHp;
    //изменение хп бара в зависимости от хп покемона
    public void SetHp(float HPNormalized, int _maxHp)
    {
        maxHp = _maxHp;
        health.transform.localScale = new Vector3(HPNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHp, int nowHp)
    {
        float curHp = health.transform.localScale.x;
        float changeAmt = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3 (curHp, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3 (newHp, 1f);

        ChangeColor(nowHp);

    }

    public void ChangeColor(int nowHp) {
        if (nowHp < (int)(maxHp / 4) + 1)
            health.color = new Color32(248, 88, 56, 255);
        else if (nowHp < (int)(maxHp / 2) + 1)
            health.color = new Color32(248, 224, 56, 255);
        else health.color = new Color32(112, 248, 168, 255);
        
    }
}
